using AsmResolver;
using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.DotNet.Signatures;
using AsmResolver.PE.DotNet.Cil;
using AsmResolver.PE.DotNet.Metadata.Tables.Rows;
using Chips.Compiler;
using Chips.Compiler.Compilation;
using Chips.Compiler.IO;
using Chips.Compiler.IO.Project;
using Chips.Compiler.Utility;
using Chips.Runtime.Specifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Chips {
	public static partial class ChipsCompiler {
		public static readonly Version ChipsVersion = new Version(1, 0);

		public static readonly string[] opcodeNames;

		private static readonly List<CompilationException> exceptions = new();

		internal static Dictionary<string, string> buildOptions = new();
		internal static AssemblyDefinition buildingAssembly;

		internal static ModuleDefinition ManifestModule => buildingAssembly?.ManifestModule ?? throw new NullReferenceException("Assembly or manifest module was not created");

		private static ITypeDefOrRef _valueType;
		internal static ITypeDefOrRef ValueTypeDefinition => _valueType ??= ManifestModule.CorLibTypeFactory.CorLibScope.CreateTypeReference("System", "ValueType").ImportWith(ManifestModule.DefaultImporter);

		public static string CompilingSourceFile;
		public static int CompilingSourceLine;
		public static int? CompilingSourceLineOverride;

		// CPDB files are included by default
		private static bool? _includeSource;
		public static bool IncludeSourceInformation => _includeSource ??= !buildOptions.TryGetValue("include-source", out string? value) || (bool.TryParse(value, out bool result) && result);

		private static bool? _noEntryPoint;
		public static bool NoEntryPoint => _noEntryPoint ??= buildOptions.TryGetValue("no-entry", out string? value) && bool.TryParse(value, out bool result) && result;

		static ChipsCompiler() {
			compilingOpcodes = new();

			// Load the opcodes and assign them in the dictionary
			foreach (Type type in typeof(ChipsCompiler).Assembly.GetExportedTypes().Where(static t => t.IsSubclassOf(typeof(CompilingOpcode)))) {
				System.Reflection.ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes)
					?? throw new InvalidOperationException($"Compiling opcode {type.Name} does not have a parameterless constructor");

				CompilingOpcode opcode = (CompilingOpcode)constructor.Invoke(null);
				OpcodeID id = opcode.GetRuntimeOpcode().Code;

				if (compilingOpcodes.ContainsKey(id))
					throw new InvalidOperationException($"OpcodeID.{id} was used by more than one opcode");

				compilingOpcodes.Add(id, opcode);
			}

			opcodeNames = Enum.GetValues<OpcodeID>()
				.Where(static id => compilingOpcodes.ContainsKey(id))
				.Select(static id => id.ToString())
				.Select(static name => (name.Contains('_') ? name[..name.IndexOf('_')] : name).ToLower())
				.Distinct()
				.Select(RemapOpcodeName)
				.ToArray();
		}

		public static void Main(string[] args) {
			// Test case
			#if DEBUG
			if (System.Diagnostics.Debugger.IsAttached) {
				args = new string[] { "Test/test.chpproj" };
			//	System.Diagnostics.Debugger.Break();
			}
			#endif

			if (args.Length == 0) {
				Logging.Error("No input files were specified.");
				return;
			}

			if (args.Length == 1 && (args[0] == "/?" || args[0] == "--help")) {
				Console.WriteLine("Usage: chips source-files [options]");
				Console.WriteLine("Options:");
				Console.WriteLine("  -out <file>             Specify the output file.");
				Console.WriteLine("  -include-source <bool>  Indicate whether CPDB files should be generated");
				Console.WriteLine("  -no-entry <bool>        Indicate whether the built assembly should have an entry point");
				return;
			}

			// If the first argument is a .chpproj file, initialize the project using that file
			// Otherwise, initialize the project using the first argument as the file search specifier
			bool directProject = Path.GetExtension(args[0]) == ".chpproj";
			ChipsProject project = directProject ? ChipsProject.FromFile(args[0]) : ChipsProject.FromCommandline(args[0]);

			if (!project.EnumerateSources().Any()) {
				Logging.Error(directProject ? "No source files were specified in the project file." : "No source files were found.");
				return;
			}

			buildOptions = ParseArguments(args[1..]);

			if (!buildOptions.ContainsKey("out")) {
				// Default to the first input file's name
				string extension = NoEntryPoint ? "dll" : "exe";

				buildOptions.Add("out", $"bin/out.{extension}");
			}

			Compile(project);
		}

		internal static AssemblyReference DotNetAssembly => KnownCorLibs.SystemPrivateCoreLib_v7_0_0_0;

		private static AssemblyDefinition _dotNetAssembly;
		internal static AssemblyDefinition DotNetAssemblyDefinition => _dotNetAssembly ??= DotNetAssembly.ImportWith(ManifestModule.DefaultImporter).Resolve() ?? throw new NullReferenceException("Could not resolve .NET assembly");

		private static readonly List<IDelayedSourceResolver> _delayedSourceResolvers = new();
		private static readonly List<IDelayedInstructionResolver> _delayedInstructionResolvers = new();
		private static readonly List<IDelayedCILMetadataResolver> _delayedCILResolvers = new();

		internal static void AddDelayedResolver(IDelayedSourceResolver resolver) => _delayedSourceResolvers.Add(resolver);

		internal static void AddDelayedResolver(IDelayedInstructionResolver resolver) => _delayedInstructionResolvers.Add(resolver);

		internal static void AddDelayedResolver(IDelayedCILMetadataResolver resolver) => _delayedCILResolvers.Add(resolver);

		internal static BytecodeMethodSegment? FoundEntryPoint;

		private static SignatureComparer _signatureComparer = new SignatureComparer(SignatureComparisonFlags.AcceptNewerVersions);

		public static bool AreTypesEqual(ITypeDescriptor first, ITypeDescriptor second) {
			ArgumentNullException.ThrowIfNull(first);
			ArgumentNullException.ThrowIfNull(second);

			return _signatureComparer.Equals(first.ToTypeSignature(), second.ToTypeSignature());
		}

		private static void Compile(ChipsProject project) {
			// Compile the source files
			CompileSourceFiles(project);

			// Compile the binary files into CIL objects
			CompileBinaryFiles(project);

			// Resolve any delayed resolvers
			foreach (IDelayedSourceResolver resolver in _delayedSourceResolvers)
				resolver.Resolve(project.context);

			// Resolve any delayed CIL metadata resolvers
			foreach (IDelayedCILMetadataResolver resolver in _delayedCILResolvers)
				resolver.Resolve(project.context);

			// Resolve any delayed instructions
			foreach (var resolvers in _delayedInstructionResolvers.GroupBy(static i => i.Body.Owner.MetadataToken.ToUInt32())) {
				StrictEvaluationStackSimulator? stack = null;
				int lastIndex = -1;

				// Ensure that the offsets are updated so that branch instructions can be handled (offsets are stored rather than instruction indices)
				resolvers.FirstOrDefault()?.Body.Instructions.CalculateOffsets();

				int offsetAdjustment = 0;

				foreach (IDelayedInstructionResolver resolver in resolvers.OrderBy(static i => i.InstructionIndex).ToList()) {  // ToList() needed since InstructionIndex is updated in the loop
					try {
						resolver.InstructionIndex += offsetAdjustment;

						if (stack is null)
							stack = resolver.Body.GetStackUpTo(resolver.InstructionIndex);
						else
							stack.ModifyStack(resolver.Body, lastIndex, resolver.InstructionIndex);

						int oldSize = resolver.Body.Instructions.Count;

						resolver.Resolve(stack);

						int newSize = resolver.Body.Instructions.Count;

						if (newSize < oldSize)
							throw new InvalidOperationException("Instruction resolvers cannot remove instructions");

						offsetAdjustment += newSize - oldSize;

						lastIndex = resolver.InstructionIndex;
					} catch (Exception ex) {
						// Add the error to the error list
						CompilingSourceFile = $"$CIL_method {resolver.Body.Owner.FullName}";
						CompilingSourceLine = -1;
						CompilingSourceLineOverride = null;
						ErrorAndThrow(ex);
					}
				}
			}

			// Report all errors if there were any, then exit
			if (exceptions.Count > 0) {
				foreach (CompilationException exception in exceptions.OrderBy(static e => e.SourceFile, OrderedFiles.Instance).ThenBy(static e => e.SourceLine))
					Logging.Error(exception);

				Environment.Exit(-1);
				return;
			}

			// Create the assembly
			buildingAssembly.Write(buildOptions["out"]);
		}

		private class OrderedFiles : IComparer<string?> {
			public static OrderedFiles Instance { get; } = new();

			public int Compare(string? x, string? y) {
				// Make CIL errors appear last
				if (x?.StartsWith("$CIL_method ") is true && y?.StartsWith("$CIL_method ") is false)
					return 1;
				else if (x?.StartsWith("$CIL_method ") is false && y?.StartsWith("$CIL_method ") is true)
					return -1;

				// Always sort .bchp files after .chp files
				string? xExt = Path.GetExtension(x);
				string? yExt = Path.GetExtension(y);

				if (xExt == ".bchp" && yExt == ".chp")
					return 1;
				else if (xExt == ".chp" && yExt == ".bchp")
					return -1;

				// Else, default to string comparison
				return x?.CompareTo(y) ?? y?.CompareTo(x) ?? 0;
			}
		}

		private static void CompileSourceFiles(ChipsProject project) {
			string name = buildOptions["out"];

			AssemblyDefinition assembly = new AssemblyDefinition(Path.GetFileNameWithoutExtension(name), ChipsVersion);
			buildingAssembly = assembly;

			ModuleDefinition module = new ModuleDefinition("ChipsManifest", DotNetAssembly);
			buildingAssembly.Modules.Add(module);

			// Context initialization has to be delayed to here
			project.context = new CompilationContext(module.DefaultImporter);

			string workingDir = Directory.GetCurrentDirectory();

			project.SetupReferences();

			// Assemble the binary object files
			foreach (ProjectSource source in project.EnumerateSources()) {
				// Ignore invalid files
				if (Path.GetExtension(source.file) != ".chp")
					continue;

				string fileDir = Path.GetRelativePath(workingDir, source.directory);
				string path = Path.Combine("obj", fileDir, Path.GetFileNameWithoutExtension(source.file) + ".bchp");

				project.AddCompiledFile(path);

				// Check if the file exists.
				if (File.Exists(path)) {
					// Read the byte-encoded last modification date and check if it's the same as the source file's last modification date.
					// If they are equal, go to the next file.
					// Otherwise, recompile the file and save it to disk.
					DateTime lastCompiledModificationDate;
					using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
						lastCompiledModificationDate = DateTime.FromBinary(reader.ReadInt64());

					DateTime lastSourceModificationDate = source.fileInfo.LastWriteTime;

					if (lastCompiledModificationDate == lastSourceModificationDate)
						continue;
				}

				var src = new SourceFile(source.fileInfo.FullName);
				var code = src.CompileToBytecode(project.context, source.fileInfo.LastWriteTime, path);

				using (BinaryWriter writer = new BinaryWriter(File.Create(path)))
					writer.Write(code._rawData);

				if (code._rawDataCPDB is not null) {
					path = Path.ChangeExtension(path, ".cpdb");

					using BinaryWriter writer = new BinaryWriter(File.Create(path));
					writer.Write(code._rawDataCPDB);
				}
			}
		}

		private static void CompileBinaryFiles(ChipsProject project) {
			project.SetupReferences();

			foreach (string file in project.EnumerateCompiledFiles()) {
				byte[] _rawData;
				byte[]? _rawDataCPDB;

				_rawData = File.ReadAllBytes(file);

				string cpdbFile = Path.ChangeExtension(file, ".cpdb");
				if (File.Exists(cpdbFile))
					_rawDataCPDB = File.ReadAllBytes(cpdbFile);
				else
					_rawDataCPDB = null;

				BytecodeFile code = new BytecodeFile(file, _rawData, _rawDataCPDB);
				code.CompileToCIL(project.context);
			}
		}

		public static object? Error(string reason) {
			exceptions.Add(new CompilationException(reason));
			return null;
		}

		public static Exception ErrorAndThrow(Exception exception) {
			ArgumentNullException.ThrowIfNull(exception);
			exceptions.Add(new CompilationException(exception.GetType().FullName + ": " + exception.Message));
			return exception;
		}

		public static Dictionary<string, string> ParseArguments(string[] args) {
			string? key = null;
			StringBuilder sb = new();

			Dictionary<string, string> dictionary = new();
			for (int i = 0; i < args.Length; i++) {
				string arg = args[i];

				if (arg.Length == 0)
					continue;

				if (arg[0] == '-' || arg[0] == '+') {
					if (key is not null) {
						dictionary[key.ToLower()] = sb.ToString();
						sb.Length = 0;
					}

					key = arg;
					sb.Length = 0;
				} else {
					if (sb.Length > 0)
						sb.Append(' ');

					sb.Append(arg);
				}
			}

			if (key is not null)
				dictionary[key.ToLower()] = sb.ToString();

			return dictionary;
		}

		public static MethodDefinition CreateDefaultConstructor(ReferenceImporter importer) {
			var method = new MethodDefinition(".ctor",
				MethodAttributes.SpecialName | MethodAttributes.RuntimeSpecialName | MethodAttributes.Public,
				MethodSignature.CreateInstance(ManifestModule.CorLibTypeFactory.Void));

			var body = method.CilMethodBody = new CilMethodBody(method);
			body.Instructions.Add(CilOpCodes.Ldarg_0);
			body.Instructions.Add(CilOpCodes.Call, importer.ImportMethod(typeof(object).GetConstructor(Type.EmptyTypes)!));
			body.Instructions.Add(CilOpCodes.Ret);

			return method;
		}
	}
}