using Chips.Compiler.Utility;
using System.IO;

namespace Chips.Compiler.IO.PDB {
	internal abstract class CPDBFileInfo {
		public readonly string parentMethod;
		public readonly string name;

		public CPDBFileInfo(string parentMethod, string name) {
			this.parentMethod = parentMethod;
			this.name = name;
		}

		public abstract void Write(BinaryWriter writer, StringHeap heap);
	}

	internal class CPDBLocalVariable : CPDBFileInfo {
		public readonly int variableIndex;

		public CPDBLocalVariable(string parentMethod, string name, int index) : base(parentMethod, name) {
			variableIndex = index;
		}

		public override void Write(BinaryWriter writer, StringHeap heap) {
			heap.WriteString(writer, name);
			writer.Write7BitEncodedInt(variableIndex);
		}

		public static CPDBLocalVariable Read(BinaryReader reader, StringHeap heap, string parentMethod) {
			string name = heap.ReadString(reader);
			int index = reader.Read7BitEncodedInt();
			return new CPDBLocalVariable(parentMethod, name, index);
		}
	}

	internal class CPDBFunctionLabel : CPDBFileInfo {
		public readonly int labelIndex;

		public readonly int opcodeOffset;

		public CPDBFunctionLabel(string parentMethod, string name, int index, int opcodeOffset) : base(parentMethod, name) {
			labelIndex = index;
			this.opcodeOffset = opcodeOffset;
		}

		public override void Write(BinaryWriter writer, StringHeap heap) {
			heap.WriteString(writer, name);
			writer.Write7BitEncodedInt(labelIndex);
			writer.Write7BitEncodedInt(opcodeOffset);
		}

		public static CPDBFunctionLabel Read(BinaryReader reader, StringHeap heap, string parentMethod) {
			string name = heap.ReadString(reader);
			int index = reader.Read7BitEncodedInt();
			int offset = reader.Read7BitEncodedInt();
			return new CPDBFunctionLabel(parentMethod, name, index, offset);
		}
	}
}
