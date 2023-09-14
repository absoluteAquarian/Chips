using Chips.Compiler.Utility;
using System.Collections.Generic;
using System.IO;

namespace Chips.Compiler.IO.PDB {
	internal class CPDBFileTypeSegment {
		public readonly string name;

		private readonly List<CPDBFileMethodSegment> methods = new();

		public IReadOnlyList<CPDBFileMethodSegment> Methods => methods.AsReadOnly();

		public CPDBFileTypeSegment(string name) {
			this.name = name;
		}

		public CPDBFileMethodSegment AddMethod(string name) {
			var method = new CPDBFileMethodSegment(name);
			methods.Add(method);
			return method;
		}

		public void AddMethod(CPDBFileMethodSegment method) => methods.Add(method);

		public void Write(BinaryWriter writer, StringHeap heap) {
			heap.WriteString(writer, name);
			writer.Write7BitEncodedInt(methods.Count);
			foreach (var method in methods)
				method.Write(writer, heap);
		}

		public static CPDBFileTypeSegment Read(BinaryReader reader, StringHeap heap) {
			string name = heap.ReadString(reader);
			
			CPDBFileTypeSegment type = new(name);

			int methodCount = reader.Read7BitEncodedInt();
			for (int i = 0; i < methodCount; i++)
				type.AddMethod(CPDBFileMethodSegment.Read(reader, heap));

			return type;
		}
	}
}
