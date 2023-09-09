using AsmResolver;
using AsmResolver.DotNet;
using Chips.Compilation;
using Chips.Compiler.IO;
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

		private static Dictionary<string, string> buildOptions = new();
		internal static AssemblyInformationTree buildingAssembly;

		internal static ModuleDefinition ManifestModule => buildingAssembly?.Assembly.ManifestModule ?? throw new NullReferenceException("Assembly or manifest module was not created");

		internal static readonly StringBuilder currentNamespace = new();

		static ChipsCompiler() {
			opcodeNames = Enum.GetNames<OpcodeID>()
				.Select(s => (s.Contains('_') ? s[.. s.IndexOf('_')] : s).ToLower())
				.Distinct()
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
				Console.WriteLine("  -out <file>                  Specify the output file.");
				return;
			}

			string[] inputFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), args[0], SearchOption.AllDirectories);

			if (inputFiles.Length == 0) {
				Logging.Error($"No files were found matching the pattern \"{args[0]}\".");
				return;
			}	

			buildOptions = ParseArguments(args[1..]);

			if (!buildOptions.ContainsKey("-out")) {
				// Default to the first input file's name
				buildOptions.Add("-out", Path.ChangeExtension(inputFiles[0], ".exe"));
			}

			Compile(inputFiles);
		}

		internal static AssemblyReference DotNetAssembly => KnownCorLibs.SystemPrivateCoreLib_v7_0_0_0;

		private static AssemblyDefinition _dotNetAssembly;
		internal static AssemblyDefinition DotNetAssemblyDefinition => _dotNetAssembly ??= DotNetAssembly.Resolve()!;

		private static void Compile(string[] inputFiles) {
			// Compile the source files
			CompileSourceFiles(inputFiles);

			// Use the binary object files to create the final executable
		}

		private static void CompileSourceFiles(string[] inputFiles) {
			string name = buildOptions["-out"];

			AssemblyDefinition assembly = new AssemblyDefinition(Path.GetFileNameWithoutExtension(name), ChipsVersion);
			buildingAssembly = new AssemblyInformationTree(assembly);

			ModuleDefinition module = new ModuleDefinition(Path.ChangeExtension(name, ".exe"), DotNetAssembly);
			buildingAssembly.Assembly.Modules.Add(module);

			ModuleInformationTree moduleTree = new ModuleInformationTree(module);
			buildingAssembly.Add(moduleTree);

			string workingDir = Directory.GetCurrentDirectory();

			// Assemble the binary object files
			foreach (string file in inputFiles) {
				// Ignore invalid files
				if (Path.GetExtension(file) != ".chp")
					continue;

				string fileDir = Path.GetRelativePath(workingDir, file);
				Directory.CreateDirectory(fileDir);

				string path = Path.Combine("obj", fileDir, Path.GetFileNameWithoutExtension(file) + ".bchp");

				// Check if the file exists.
				if (File.Exists(path)) {
					// Read the byte-encoded last modification date and check if it's the same as the source file's last modification date.
					// If they are equal, go to the next file.
					// Otherwise, recompile the file and save it to disk.
					DateTime lastCompiledModificationDate;
					using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
						lastCompiledModificationDate = DateTime.FromBinary(reader.ReadInt64());

					DateTime lastSourceModificationDate = new FileInfo(file).LastWriteTime;

					if (lastCompiledModificationDate == lastSourceModificationDate)
						continue;
				}

				var src = new SourceFile(file);
				var code = src.CompileToBytecode(moduleTree);

				using (BinaryWriter writer = new BinaryWriter(File.Create(path)))
					writer.Write(code._rawData);

				if (code.cpdbInfo is not null) {
					path = Path.ChangeExtension(path, ".cpdb");

					using (BinaryWriter writer = new BinaryWriter(File.Create(path)))
						writer.Write(code._rawDataCPDB);
				}
			}
		}

		internal static void AssignNamespace(string name, bool appendToCurrent) {
			if (appendToCurrent) {
				// Increase the current namespace
				currentNamespace.Append("." + name);
			} else {
				currentNamespace.Clear();
				currentNamespace.Append(name);
			}
		}

		internal static void RemoveNamespace(string name) {
			if (currentNamespace.Equals(name))
				currentNamespace.Clear();  // Namespace matches entirely; clear it
			else if (currentNamespace.ToString().EndsWith(name))
				currentNamespace.Length -= name.Length + 1;  // Remove part of it
			else
				currentNamespace.Clear();  // Failsafe, assume something went wrong
		}

		public static object? Error(string? sourceFile, string reason) {
			exceptions.Add(new CompilationException(sourceFile, reason));
			return null;
		}

		public static Exception ErrorAndThrow(string? sourceFile, Exception exception) {
			ArgumentNullException.ThrowIfNull(exception);
			exceptions.Add(new CompilationException(sourceFile, exception.Message));
			return exception;
		}

		public static bool OpcodeIsDefined(string code) {
			for (int i = 0; i < opcodeNames.Length; i++)
				if (opcodeNames[i] == code)
					return true;

			return false;
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
	}
}