using Chips.Utility;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Chips.Compiler.IO {
	internal class CPDBFile {
		public readonly CPDBFileMemberCollection Information = new();

		private CPDBFile() { }

		public static CPDBFile FromFile(string file) {
			if (Path.GetExtension(file) != ".cpdb")
				throw new IOException("File extension was not \".cpdb\"");

			using BinaryReader reader = new(File.OpenRead(file));

			ValidateReader(reader);

			CPDBFile cpdb = new();
			cpdb.PopulateInformation(reader);
			return cpdb;
		}

		public static CPDBFile FromStream(Stream stream) {
			using BinaryReader reader = new(stream);

			ValidateReader(reader);

			CPDBFile cpdb = new();
			cpdb.PopulateInformation(reader);
			return cpdb;
		}

		private static void ValidateReader(BinaryReader reader) {
			Span<byte> header = stackalloc byte[7];
			reader.Read(header);

			if (header.DecodeSpan() != "CPDB\x1A\xEE\xFE")
				throw new IOException("Magic header was invalid");
		}

		private void PopulateInformation(BinaryReader reader) {
			Information.Read(reader);
		}

		public void WriteToFile(string file) => WriteToStream(File.Create(file));

		public void WriteToStream(Stream stream) {
			using BinaryWriter writer = new(stream);
			
			// Write the magic header
			writer.Write("CPDB\x1A\xEE\xFE".EncodeASCIISpan());

			Information.Write(writer);
		}
	}
}
