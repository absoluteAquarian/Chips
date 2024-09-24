using AsmResolver.DotNet;
using AsmResolver.DotNet.Signatures;
using Chips.Compiler.IO;
using Chips.Compiler.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Chips {
	public static partial class ChipsCompiler {
		public static readonly Version ChipsVersion = new Version(1, 0);

		internal static Dictionary<string, string> buildOptions = new();
		internal static AssemblyDefinition buildingAssembly;

		internal static ModuleDefinition ManifestModule => buildingAssembly?.ManifestModule ?? throw new NullReferenceException("Assembly or manifest module was not created");

		private static ITypeDefOrRef _valueType;
		internal static ITypeDefOrRef ValueTypeDefinition => _valueType ??= ManifestModule.CorLibTypeFactory.CorLibScope.CreateTypeReference("System", "ValueType").ImportWith(ManifestModule.DefaultImporter);

		// CPDB files are included by default
		private static bool? _includeSource;
		public static bool IncludeSourceInformation => _includeSource ??= !buildOptions.TryGetValue("include-source", out string? value) || (bool.TryParse(value, out bool result) && result);

		private static bool? _noEntryPoint;
		public static bool NoEntryPoint => _noEntryPoint ??= buildOptions.TryGetValue("no-entry", out string? value) && bool.TryParse(value, out bool result) && result;

		public static void Main(string[] args) {
			// Test case
			#if DEBUG
			if (System.Diagnostics.Debugger.IsAttached) {
				args = [ "Test/test.chpproj" ];
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

		private static SignatureComparer _signatureComparer = new SignatureComparer(SignatureComparisonFlags.AcceptNewerVersions);

		public static bool AreTypesEqual(ITypeDescriptor first, ITypeDescriptor second) {
			ArgumentNullException.ThrowIfNull(first);
			ArgumentNullException.ThrowIfNull(second);

			return _signatureComparer.Equals(first.ToTypeSignature(), second.ToTypeSignature());
		}

		private static void Compile(ChipsProject project) {
			// TODO: Compile the source files into intermediary binary file objects (and optionally write to disk)

			// TODO: Compile the binary files into CIL objects

			// TODO: Report any warnings or errors
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