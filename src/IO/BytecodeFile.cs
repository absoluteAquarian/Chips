using Chips.Compilation;
using Chips.Core;
using Chips.Core.Utility;
using Mono.Cecil;
using System.CodeDom.Compiler;

namespace Chips.IO{
	internal class BytecodeFile{
		private readonly BinaryReader reader;

		public readonly CPDBFile? cpdbInfo;

		private static int stackSize = 1000;
		private static string asmName = "CompiledChipsProgram";

		private bool stackSizeDefined, asmNameDefined;

		internal HashSet<string> definedGlobalVariables = new();
		internal HashSet<string> definedGlobalFunctions = new();

		private readonly List<BytecodeFunction> globalFunctions = new();
		private readonly List<VariableInformation> globalVariables = new();

		public readonly string SourceFile;
		private readonly FileInfo sourceFileInfo;

		private static AssemblyDefinition assembly;
		private static MethodDefinition mainMethod;

		private readonly List<BytecodeFile> importedFiles = new();

		public static AssemblyInformationTree informationTree;
		private static BytecodeFile targetFileForEntryPoint;

		private static readonly Dictionary<string, BinaryReader> readersByFile = new();

		public BytecodeFile(string file){
			if(Path.GetExtension(file) != ".bchp"){
				Compiler.exceptions.Add(new(file, "File extension was not \".bchp\""));
				return;
			}

			SourceFile = Path.GetFileName(file);
			sourceFileInfo = new(file);

			if(readersByFile.ContainsKey(file)){
				Compiler.exceptions.Add(new(null, "File is already being read: " + file));
				return;
			}

			reader = new(File.OpenRead(file));

			var cpdb = Path.ChangeExtension(file, ".cpdb");
			if(File.Exists(cpdb))
				cpdbInfo = new(cpdb);
		}

		public void Compile(){
			targetFileForEntryPoint = this;
			Compile(null);
		}

		private void Compile(BytecodeFile? importTarget = null){
			long fileLength = reader.BaseStream.Length;
			while(reader.BaseStream.Position < fileLength){
				BytecodeFileMember member = (BytecodeFileMember)reader.ReadByte();

				switch(member){
					case BytecodeFileMember.AssemblyInfo:
						byte asmInfo = reader.ReadByte();

						switch(asmInfo){
							case 0:
								if(asmNameDefined){
									Compiler.exceptions.Add(new(SourceFile, "Duplicate assembly name token found"));
									break;
								}

								asmNameDefined = true;
								asmName = reader.ReadString();

								if(!CodeGenerator.IsValidLanguageIndependentIdentifier(asmName))
									Compiler.exceptions.Add(new(SourceFile, "Assembly name was not a valid name identifier: " + asmName));
								break;
							case 1:
								if(stackSizeDefined){
									Compiler.exceptions.Add(new(SourceFile, "Duplicate stack size token found"));
									break;
								}

								stackSizeDefined = true;
								stackSize = reader.Read7BitEncodedInt();
								break;
							default:
								Compiler.exceptions.Add(new(SourceFile, $"Unknown assembly information token: 0x{asmInfo :X2}"));
								break;
						}
						break;
					case BytecodeFileMember.GlobalVariable:
						string name = reader.ReadString();
						if(!definedGlobalVariables.Add(name)){
							Compiler.exceptions.Add(new(SourceFile, "Duplicate global variable declaration: " + name));
							break;
						}
						
						string type = reader.ReadString();
						globalVariables.Add(new(global: true, name, type));
						break;
					case BytecodeFileMember.GlobalFunction:
						globalFunctions.Add(new(this, reader));
						break;
					case BytecodeFileMember.ImportedFile:
						bool isChipsStdlib = reader.ReadByte() != 0;

						string directory = isChipsStdlib ? System.Reflection.Assembly.GetExecutingAssembly().Location : sourceFileInfo.Directory!.FullName;
						string fileToImport = Path.Combine(directory, reader.ReadString());

						if(!readersByFile.ContainsKey(fileToImport)){
							//Only one instance of a "file" should be used, regardless of how many imports there are
							var byteCode = new BytecodeFile(fileToImport);
							byteCode.Compile(this);
							byteCode.reader.Dispose();

							importedFiles.Add(byteCode);
						}
						break;
				}
			}

			if(importTarget is not null)
				return;

			if(!definedGlobalFunctions.Contains("main")){
				Compiler.exceptions.Add(new(SourceFile, "Global function \"main\" was not defined."));
				return;
			}

			readersByFile.Clear();

			assembly = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition(asmName, new Version(Sandbox.Version)), asmName, ModuleKind.Console);

			for(int i = 0; i < importedFiles.Count; i++)
				CompileEverything(importedFiles[i]);

			CompileEverything(this);

			CompileEntryPoint();

			// TODO: execute "peverify.exe" after saving the file
		}

		private static void CompileEntryPoint(){
			//Creates the static Main() method, which invokes Chips.Core.Sandbox.Main()
		}

		private static void CompileEverything(BytecodeFile target){
			// TODO: parse "struct" definitions and add them as types
			string typeName = Path.ChangeExtension(target.SourceFile, null);
			if(!CodeGenerator.IsValidLanguageIndependentIdentifier(typeName)){
				typeName = "_" + typeName;

				if(!CodeGenerator.IsValidLanguageIndependentIdentifier(typeName)){
					Compiler.exceptions.Add(new(target.SourceFile, "Source file had a name which could not be compiled to an identifier: " + typeName[1..]));
					return;
				}
			}

			ModuleDefinition typeModule = ModuleDefinition.CreateModule(typeName, ModuleKind.Console);
			informationTree.Add(typeModule.Name, new(typeModule));

			//Result:  AsmName::TypeName
			TypeDefinition importType = new(asmName, typeName,
				TypeAttributes.Class |
				TypeAttributes.Public |
				TypeAttributes.Sealed |
				TypeAttributes.Abstract |
				TypeAttributes.AutoClass |
				TypeAttributes.AnsiClass |
				TypeAttributes.BeforeFieldInit |
				TypeAttributes.AutoLayout);
			informationTree[typeModule.Name].Add(importType.FullName!, new(importType));

			CompileGlobalVariables(target, typeModule, importType);

			CompileFunctions(target, typeModule, importType);
		}

		private static void CompileGlobalVariables(BytecodeFile target, ModuleDefinition module, TypeDefinition type){
			for(int i = 0; i < target.globalVariables.Count; i++){
				var global = target.globalVariables[i];

				var globalType = TypeTracking.GetCSharpType(global.type);
				if(globalType is null){
					Compiler.exceptions.Add(new(target.SourceFile, "Variables cannot have a \"null\" type"));
					continue;
				}

				FieldDefinition field = new(global.name, FieldAttributes.Public | FieldAttributes.Static, module.ImportReference(type));
				informationTree[module.Name][type.FullName!].Add(field.Name, field);
			}
		}

		private static void CompileFunctions(BytecodeFile target, ModuleDefinition module, TypeDefinition type){
			for(int i = 0; i < target.globalFunctions.Count; i++){
				var function = target.globalFunctions[i];

				MethodDefinition method = new(function.name, function.attributes, module.TypeSystem.Void);

				if(target == targetFileForEntryPoint && function.name == "main"){
					//Method names can't be duplicated
					mainMethod = method;

					if(function.attributes != (MethodAttributes.Public | MethodAttributes.Static)){
						Compiler.exceptions.Add(new(target.SourceFile, "Entry point \"main\" must have the \"pub\" and \"stat\" accessors"));
						continue;
					}

					if(target.SourceFile == function.name){
						Compiler.exceptions.Add(new(target.SourceFile, "Function name cannot match file name without the extension"));
						continue;
					}
				}
				
				informationTree[module.Name][type.FullName!].Add(method.Name, method);

				function.Compile(module, type, method);
			}
		}
	}
}
