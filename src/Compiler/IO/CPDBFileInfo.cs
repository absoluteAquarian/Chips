using Chips.Utility;
using System;
using System.IO;

namespace Chips.Compiler.IO {
	internal abstract class CPDBFileInfo {
		public readonly string parentMethod;

		private readonly byte[] data;

		public ReadOnlySpan<byte> Data => data;

		public CPDBFileInfo(string parentMethod, byte[] data) {
			this.parentMethod = parentMethod;
			this.data = data;
		}

		public void Write(BinaryWriter writer) {
			writer.Write7BitEncodedInt(data.Length);
			writer.Write(data);
		}

		public static byte[] Read(BinaryReader reader) {
			int length = reader.Read7BitEncodedInt();
			return reader.ReadBytes(length);
		}
	}

	internal class CPDBLocalVariable : CPDBFileInfo {
		public readonly string variableName;

		public readonly int variableIndex;

		public CPDBLocalVariable(string parentMethod, byte[] data) : base(parentMethod, data) {
			int index = 0;

			variableName = data.GetCPDBString(ref index);

			variableIndex = data.Get7BitEncodedInt(index, out _);
		}
	}

	internal class CPDBFunctionLabel : CPDBFileInfo {
		public readonly string labelName;

		public readonly int labelIndex;

		public readonly int opcodeOffset;

		public CPDBFunctionLabel(string parentMethod, byte[] data) : base(parentMethod, data) {
			int index = 0;

			labelName = data.GetCPDBString(ref index);

			labelIndex = data.Get7BitEncodedInt(index, out int bytesRead);
			index += bytesRead;

			opcodeOffset = data.Get7BitEncodedInt(index, out _);
		}
	}
}
