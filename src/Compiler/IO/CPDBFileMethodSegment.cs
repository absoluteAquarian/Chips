using Chips.Compiler.Compilation;
using Chips.Compiler.Utility;
using Chips.Utility;
using System.Collections.Generic;
using System.IO;

namespace Chips.Compiler.IO {
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
			using MemoryStream ms = new();
			using BinaryWriter writer = new(ms);

			writer.Write7BitEncodedInt(index);

			locals.Add(new CPDBLocalVariable(method, localName, ms.ToArray()));
		}

		public void AddLocal(CPDBLocalVariable local) => locals.Add(local);

		public void AddLabel(string method, ChipsLabel label) {
			using MemoryStream ms = new();
			using BinaryWriter writer = new(ms);

			writer.Write7BitEncodedInt(label.Index);
			writer.Write7BitEncodedInt(label.OpcodeOffset);

			labels.Add(new CPDBFunctionLabel(method, label.Name, ms.ToArray()));
		}

		public void AddLabel(CPDBFunctionLabel label) => labels.Add(label);

		public void Write(BinaryWriter writer, StringHeap heap) {
			StringMetadata token = heap.GetOrAdd(name);
			token.Serialize(writer);

			writer.Write7BitEncodedInt(locals.Count);
			foreach (var local in locals)
				local.Write(writer, heap);

			writer.Write7BitEncodedInt(labels.Count);
			foreach (var label in labels)
				label.Write(writer, heap);
		}

		public static CPDBFileMethodSegment Read(BinaryReader reader, StringHeap heap) {
			StringMetadata token = StringMetadata.Deserialize(reader);
			string name = heap.GetString(token);
			CPDBFileMethodSegment segment = new(name);

			int localCount = reader.Read7BitEncodedInt();
			for (int i = 0; i < localCount; i++) {
				byte[] data = CPDBFileInfo.Read(reader, heap, out string localName);
				segment.locals.Add(new CPDBLocalVariable(name, localName, data));
			}

			int labelCount = reader.Read7BitEncodedInt();
			for (int i = 0; i < labelCount; i++) {
				byte[] data = CPDBFileInfo.Read(reader, heap, out string labelName);
				segment.labels.Add(new CPDBFunctionLabel(name, labelName, data));
			}

			return segment;
		}
	}
}
