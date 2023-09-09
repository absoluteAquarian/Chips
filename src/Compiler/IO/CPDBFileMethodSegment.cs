using Chips.Compiler.Compilation;
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

			writer.Write(localName.EncodeToCPDB());
			writer.Write7BitEncodedInt(index);

			locals.Add(new CPDBLocalVariable(method, ms.ToArray()));
		}

		public void AddLocal(CPDBLocalVariable local) => locals.Add(local);

		public void AddLabel(string method, ChipsLabel label) {
			using MemoryStream ms = new();
			using BinaryWriter writer = new(ms);

			writer.Write(label.Name.EncodeToCPDB());
			writer.Write7BitEncodedInt(label.Index);
			writer.Write7BitEncodedInt(label.OpcodeOffset);

			labels.Add(new CPDBFunctionLabel(method, ms.ToArray()));
		}

		public void AddLabel(CPDBFunctionLabel label) => labels.Add(label);

		public void Write(BinaryWriter writer) {
			writer.Write(name);

			writer.Write7BitEncodedInt(locals.Count);
			foreach (var local in locals)
				local.Write(writer);

			writer.Write7BitEncodedInt(labels.Count);
			foreach (var label in labels)
				label.Write(writer);
		}

		public static CPDBFileMethodSegment Read(BinaryReader reader) {
			string name = reader.ReadString();
			CPDBFileMethodSegment segment = new(name);

			int localCount = reader.Read7BitEncodedInt();
			for (int i = 0; i < localCount; i++) {
				byte[] data = CPDBFileInfo.Read(reader);
				segment.locals.Add(new CPDBLocalVariable(name, data));
			}

			int labelCount = reader.Read7BitEncodedInt();
			for (int i = 0; i < labelCount; i++) {
				byte[] data = CPDBFileInfo.Read(reader);
				segment.labels.Add(new CPDBFunctionLabel(name, data));
			}

			return segment;
		}
	}
}
