using AsmResolver;
using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.DotNet.Signatures;
using AsmResolver.PE.DotNet.Cil;
using AsmResolver.PE.DotNet.Metadata.Tables.Rows;
using Chips.Compiler;
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
			opcodeNames = Enum.GetValues<OpcodeID>()
				.Where(static id => opcodes.ContainsKey(id))
				.Select(static id => id.ToString())
				.Select(static name => (name.Contains('_') ? name[..name.IndexOf('_')] : name).ToLower())
				.Distinct()
				.Select(RemapOpcodeName)
				.ToArray();
		}

		public static void Main(string[] args) {
			// Expected arguments: source-files [-out output-file]
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
		internal static AssemblyDefinition DotNetAssemblyDefinition => _dotNetAssembly ??= DotNetAssembly.Resolve()!;

		private static List<IDelayedResolver> _delayedResolvers = new();

		internal static void AddDelayedResolver(IDelayedResolver resolver) => _delayedResolvers.Add(resolver);

		private static void Compile(ChipsProject project) {
			// Compile the source files
			CompileSourceFiles(project, out var context);

			// Compile the binary files into CIL objects
			CompileBinaryFiles(project, context);

			// Resolve any delayed resolvers
			foreach (IDelayedResolver resolver in _delayedResolvers)
				resolver.Resolve(context);

			// Report all errors if there were any, then exit
			if (exceptions.Count > 0) {
				foreach (CompilationException exception in exceptions.OrderBy(static e => e.SourceFile, OrderedFiles.Instance).ThenBy(static e => e.SourceLine))
					Logging.Error($"{exception.Reason} at {exception.SourceFile}{(exception.SourceLine >= 0 ? $" on line {exception.SourceLine + 1}" : "")}");

				Environment.Exit(-1);
				return;
			}

			// Create the assembly
			buildingAssembly.Write(buildOptions["out"]);
		}

		private class OrderedFiles : IComparer<string?> {
			public static OrderedFiles Instance { get; } = new();

			public int Compare(string? x, string? y) {
				// Always sort .bchp files after .chp files
				if (Path.GetExtension(x) == ".bchp" && Path.GetExtension(y) == ".chp")
					return 1;
				if (Path.GetExtension(x) == ".chp" && Path.GetExtension(y) == ".bchp")
					return -1;

				// Else, default to string comparison
				return x?.CompareTo(y) ?? y?.CompareTo(x) ?? 0;
			}
		}

		private static void CompileSourceFiles(ChipsProject project, out CompilationContext context) {
			string name = buildOptions["out"];

			AssemblyDefinition assembly = new AssemblyDefinition(Path.GetFileNameWithoutExtension(name), ChipsVersion);
			buildingAssembly = assembly;

			ModuleDefinition module = new ModuleDefinition("ChipsManifest", DotNetAssembly);
			buildingAssembly.Modules.Add(module);

			// Context initialization has to be delayed to here
			project.context = new CompilationContext(module.DefaultImporter);

			string workingDir = Directory.GetCurrentDirectory();

			Utility.Extensions.StringExtensionsLinkedToSourceLines = true;

			context = new CompilationContext(ManifestModule.DefaultImporter);
			project.SetupReferences(context.resolver);

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
				var code = src.CompileToBytecode(context, source.fileInfo.LastWriteTime, path);

				using (BinaryWriter writer = new BinaryWriter(File.Create(path)))
					writer.Write(code._rawData);

				if (code._rawDataCPDB is not null) {
					path = Path.ChangeExtension(path, ".cpdb");

					using BinaryWriter writer = new BinaryWriter(File.Create(path));
					writer.Write(code._rawDataCPDB);
				}
			}

			Utility.Extensions.StringExtensionsLinkedToSourceLines = false;
		}

		private static void CompileBinaryFiles(ChipsProject project, CompilationContext context) {
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
				code.CompileToCIL(context);
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

		internal static int ErrorCount => exceptions.Count;

		internal static void RestoreExceptionState(int count) {
			exceptions.RemoveRange(count, exceptions.Count - count);
		}

		public static bool OpcodeIsDefined(string code) {
			for (int i = 0; i < opcodeNames.Length; i++)
				if (opcodeNames[i] == code)
					return true;

			return false;
		}

		public static IEnumerable<Opcode> FindOpcode(string name) {
			foreach (var (id, code) in opcodes) {
				string opcodeName = id.ToString().ToLower();
				
				int index = opcodeName.IndexOf('_');
				if (index > 0)
					opcodeName = opcodeName[..index];

				opcodeName = RemapOpcodeName(opcodeName);

				if (opcodeName == name)
					yield return code;
			}
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