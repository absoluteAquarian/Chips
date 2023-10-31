using AsmResolver.DotNet;
using Chips.Compiler.Parsing;
using System;
using System.Collections.Generic;
using System.IO;

namespace Chips.Compiler.IO.Project {
	internal class ChipsProject {
		private readonly SourceResolver sources;
		private readonly List<string> assemblies = new();

		private readonly List<string> bytecodeFiles = new();

		public readonly string? file;

		public CompilationContext context;

		private ChipsProject(string? file) {
			this.file = file;
			sources = new(file is null ? "." : new FileInfo(file).DirectoryName ?? throw new ArgumentException($"Source file \"{file}\" does not exist"));
		}

		public IEnumerable<ProjectSource> EnumerateSources() => sources.EnumerateFiles();

		public IEnumerable<string> EnumerateCompiledFiles() => bytecodeFiles;

		public void SetupReferences() {
			context.resolver.Clear(clearAssemblies: true);

			foreach (string assembly in assemblies)
				context.resolver.AddAssembly(AssemblyDefinition.FromFile(assembly));
		}

		public void AddCompiledFile(string path) {
			if (Path.GetExtension(path) != ".bchp")
				throw new IOException("File extension was not \".bchp\"");

			bytecodeFiles.Add(path);
		}

		public static ChipsProject FromCommandline(string inputArg) {
			ChipsProject project = new(null);
			project.sources.AddFiles(inputArg, include: true);
			return project;
		}

		public static ChipsProject FromFile(string file) {
			if (Path.GetExtension(file) != ".chpproj")
				throw new IOException("File extension was not \".chpproj\"");

			using SourceReader reader = new SourceReader(new StreamReader(File.OpenRead(file)));

			ChipsProject project = new(file);

			while (!reader.BaseReader.EndOfStream) {
				// Check for comments
				if (reader.BaseReader.Peek() == '#') {
					ReadComment(reader);
					continue;
				}

				string word = reader.ReadWord();

				switch (word) {
					case ".source":
						// .source <include|exclude> <path>
						ReadSource(reader, project);
						break;
					case ".reference":
						// .reference <path>
						ReadReference(reader, project);
						break;
					default:
						throw new IOException($"Unknown directive \"{word}\"");
				}

				// Consume characters until a newline is reached
				// If a character is a #, skip to the next line immediately, else if it is not whitespace, throw an error
				while (reader.TryReadExcept('\n', out char read, alwaysConsume: true)) {
					if (read == '#') {
						ReadComment(reader);
						break;
					} else if (!char.IsWhiteSpace(read))
						throw new IOException($"Detected excess characters after \"{word}\" directive");
				}
			}

			return project;
		}

		private static void ReadComment(SourceReader reader) {
			reader.ReadUntilNewline();
		}

		private static void ReadSource(SourceReader reader, ChipsProject project) {
			string scope = reader.ReadWord();

			bool include = scope switch {
				"include" => true,
				"exclude" => false,
				_ => throw new IOException($"Unknown source scope \"{scope}\", expected \"include\" or \"exclude\"")
			};

			string path = reader.ReadWordOrQuotedString(out _);

			project.sources.AddFiles(path, include);
		}

		private static void ReadReference(SourceReader reader, ChipsProject project) {
			string path = reader.ReadWordOrQuotedString(out _);

			project.assemblies.Add(path);
		}
	}
}
