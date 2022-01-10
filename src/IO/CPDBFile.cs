using Chips.Compilation;
using Chips.Core;
using Chips.Utility;

namespace Chips.IO{
	internal class CPDBFile{
		private Dictionary<CPDBClassification, List<CPDBFileInfo>> information = new();

		public CPDBFile(string file){
			if(Path.GetExtension(file) != ".cpdb")
				throw new IOException("File extension was not \".cpdb\"");

			using BinaryReader reader = new(File.OpenRead(file));

			Span<byte> header = stackalloc byte[7];
			reader.Read(header);

			if(header.DecodeSpan() != "CPDB\x1A\xEE\xFE")
				throw new IOException("Magic header was invalid");

			string version = reader.ReadString();
			if(version != Sandbox.Version)
				throw new IOException($"Chips PDB file \"{Path.GetFileName(file)}\" was built for Chips version v{version}, but the current version of Chips is v{Sandbox.Version}");

			long streamLength = reader.BaseStream.Length;

			while(reader.BaseStream.Position < streamLength){
				CPDBClassification classification = (CPDBClassification)reader.ReadByte();

				if(!information.TryGetValue(classification, out var list))
					information[classification] = list = new();

				list.Add(ConstructInfo(classification, reader));
			}
		}

		private static CPDBFileInfo ConstructInfo(CPDBClassification classification, BinaryReader reader){
			int dataLength = reader.Read7BitEncodedInt();

			byte[] data = reader.ReadBytes(dataLength);

			return classification switch{
				CPDBClassification.GlobalVariable => new CPDBGlobalVariable(data),
				CPDBClassification.LocalVariable => new CPDBLocalVariable(data),
				CPDBClassification.Label => new CPDBFunctionLabel(data),
				_ => throw new ArgumentException("Unknown classification: " + classification)
			};
		}
	}
}
