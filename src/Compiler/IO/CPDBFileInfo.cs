using Chips.Compiler.Utility;
using Chips.Utility;
using System;
using System.IO;

namespace Chips.Compiler.IO {
	internal abstract class CPDBFileInfo {
		public readonly string parentMethod;
		public readonly string name;

		private readonly byte[] data;

		public ReadOnlySpan<byte> Data => data;

		public CPDBFileInfo(string parentMethod, string name, byte[] data) {
			this.parentMethod = parentMethod;
			this.name = name;
			this.data = data;
		}

		public void Write(BinaryWriter writer, StringHeap heap) {
			StringMetadata token = heap.GetOrAdd(name);
			token.Serialize(writer);

			writer.Write7BitEncodedInt(data.Length);
			writer.Write(data);
		}

		public static byte[] Read(BinaryReader reader, StringHeap heap, out string name) {
			StringMetadata token = StringMetadata.Deserialize(reader);
			name = heap.GetString(token);

			int length = reader.Read7BitEncodedInt();
			return reader.ReadBytes(length);
		}
	}

	internal class CPDBLocalVariable : CPDBFileInfo {
		public readonly int variableIndex;

		public CPDBLocalVariable(string parentMethod, string name, byte[] data) : base(parentMethod, name, data) {
			int index = 0;

			variableIndex = data.Get7BitEncodedInt(index, out _);
		}
	}

	internal class CPDBFunctionLabel : CPDBFileInfo {
		public readonly string labelName;

		public readonly int labelIndex;

		public readonly int opcodeOffset;

		public CPDBFunctionLabel(string parentMethod, string name, byte[] data) : base(parentMethod, name, data) {
			int index = 0;

			labelIndex = data.Get7BitEncodedInt(index, out int bytesRead);
			index += bytesRead;

			opcodeOffset = data.Get7BitEncodedInt(index, out _);
		}
	}
}
