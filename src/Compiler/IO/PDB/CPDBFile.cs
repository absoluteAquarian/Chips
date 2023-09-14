using Chips.Compiler.Utility;
using Chips.Utility;
using System;
using System.IO;
using System.Text;

namespace Chips.Compiler.IO.PDB {
	internal class CPDBFile {
		public readonly CPDBFileMemberCollection Information = new();

		private CPDBFile() { }

		public static CPDBFile FromFile(string file) {
			if (Path.GetExtension(file) != ".cpdb")
				throw new IOException("File extension was not \".cpdb\"");

			using BinaryReader reader = new(File.OpenRead(file), Encoding.Unicode);

			ValidateReader(reader);

			CPDBFile cpdb = new();
			cpdb.PopulateInformation(reader);
			return cpdb;
		}

		public static CPDBFile FromStream(Stream stream) {
			using BinaryReader reader = new(stream, Encoding.Unicode);

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
			StringHeap heap = new StringHeap();
			heap.Deserialize(reader);

			Information.Read(reader, heap);
		}

		public void WriteToFile(string file) => WriteToStream(File.Create(file));

		public void WriteToStream(Stream stream) {
			using BinaryWriter writer = new(stream);
			
			// Write the magic header
			writer.Write("CPDB\x1A\xEE\xFE".EncodeASCIISpan());

			StringHeap heap = new StringHeap();

			using MemoryStream infoData = new();
			using (BinaryWriter infoWriter = new(infoData))
				Information.Write(writer, heap);

			// Write heap first so that it is available when reading
			heap.Serialize(writer);
			writer.Write(infoData.ToArray());
		}
	}
}
