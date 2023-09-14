using Chips.Utility;
using System.Collections.Generic;
using System.IO;

namespace Chips.Compiler.IO.Project {
	internal class ChipsProject {
		private readonly SourceResolver sources = new();
		private readonly Dictionary<string, string> assemblyNameToFileLookup = new();

		public readonly string file;

		private ChipsProject(string file) {
			this.file = file;
		}

		public static ChipsProject FromFile(string file) {
			if (Path.GetExtension(file) != ".chpproj")
				throw new IOException("File extension was not \".chpproj\"");

			using StreamReader reader = new(File.OpenRead(file));

			ChipsProject project = new(file);

			while (!reader.EndOfStream) {
				// Check for comments
				if (reader.Peek() == '#') {
					ReadComment(reader);
					continue;
				}

				string word = reader.ReadWord();

				switch (word) {
					case ".source":
						ReadSource(reader, project);
						break;
					case ".reference":
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

		private static void ReadComment(StreamReader reader) {
			while (reader.Peek() != '\n')
				reader.Read();

			// Consume the newline
			reader.Read();
		}

		private static void ReadSource(StreamReader reader, ChipsProject project) {
			string scope = reader.ReadWord();

			bool include = scope switch {
				"include" => true,
				"exclude" => false,
				_ => throw new IOException($"Unknown source scope \"{scope}\", expected \"include\" or \"exclude\"")
			};

			string path = reader.ReadWordOrQuotedString();

			project.sources.AddFiles(path, include);
		}

		private static void ReadReference(StreamReader reader, ChipsProject project) {
			string alias = reader.ReadWord();

			string wordFrom = reader.ReadWord();
			if (wordFrom != "from")
				throw new IOException($"Expected \"from\" within \".reference\" directive, found \"{wordFrom}\" instead");

			string path = reader.ReadWordOrQuotedString();

			// If the alias already exists, throw an error
			if (project.assemblyNameToFileLookup.ContainsKey(alias))
				throw new IOException($"Duplicate alias \"{alias}\" in \".reference\" directive");

			project.assemblyNameToFileLookup.Add(alias, path);
		}
	}
}
