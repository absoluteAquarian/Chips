using System.IO;

namespace Chips.Compiler.IO {
	internal class BytecodeFile {
		public readonly CPDBFile? cpdbInfo;

		public readonly string sourceFile;

		internal readonly byte[] _rawData;
		internal readonly byte[] _rawDataCPDB;

		public BytecodeFile(string sourceFile, byte[] code, byte[]? cpdbCode) {
			this.sourceFile = sourceFile;
			_rawData = code;

			if (cpdbCode is not null) {
				_rawDataCPDB = cpdbCode;
				cpdbInfo = CPDBFile.FromStream(new MemoryStream(cpdbCode));
			}
		}

		public void CompileToCIL() {
			using MemoryStream data = new MemoryStream(_rawData);
			using BinaryReader reader = new BinaryReader(data);


		}
	}
}
