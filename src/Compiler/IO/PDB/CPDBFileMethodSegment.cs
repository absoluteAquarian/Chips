using Chips.Compiler.Compilation;
using Chips.Compiler.Utility;
using System.Collections.Generic;
using System.IO;

namespace Chips.Compiler.IO.PDB {
	internal class CPDBFileMethodSegment {
		public readonly string name;

		private readonly List<CPDBLocalVariable> locals = new();
		private readonly List<CPDBFunctionLabel> labels = new();

		public IReadOnlyList<CPDBLocalVariable> Locals => locals.AsReadOnly();

		public IReadOnlyList<CPDBFunctionLabel> Labels => labels.AsReadOnly();

		public CPDBFileMethodSegment(string name) {
			this.name = name;
		}

		public void AddLocal(string method, string localName, int index) {
			locals.Add(new CPDBLocalVariable(method, localName, index));
		}

		public void AddLocal(CPDBLocalVariable local) => locals.Add(local);

		public void AddLabel(string method, ChipsLabel label) {
			labels.Add(new CPDBFunctionLabel(method, label.Name, label.Index, label.OpcodeOffset));
		}

		public void AddLabel(CPDBFunctionLabel label) => labels.Add(label);

		public void Write(BinaryWriter writer, StringHeap heap) {
			heap.WriteString(writer, name);

			writer.Write7BitEncodedInt(locals.Count);
			foreach (var local in locals)
				local.Write(writer, heap);

			writer.Write7BitEncodedInt(labels.Count);
			foreach (var label in labels)
				label.Write(writer, heap);
		}

		public static CPDBFileMethodSegment Read(BinaryReader reader, StringHeap heap) {
			string name = heap.ReadString(reader);
			CPDBFileMethodSegment segment = new(name);

			int localCount = reader.Read7BitEncodedInt();
			for (int i = 0; i < localCount; i++)
				segment.locals.Add(CPDBLocalVariable.Read(reader, heap, name));

			int labelCount = reader.Read7BitEncodedInt();
			for (int i = 0; i < labelCount; i++)
				segment.labels.Add(CPDBFunctionLabel.Read(reader, heap, name));

			return segment;
		}
	}
}
