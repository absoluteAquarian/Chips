using Chips.Compiler.Utility;
using System.Collections.Generic;
using System.IO;

namespace Chips.Compiler.IO.PDB {
	internal class CPDBFileMemberCollection {
		private readonly List<CPDBFileNamespaceSegment> namespaces = new();

		public IReadOnlyList<CPDBFileNamespaceSegment> Namespaces => namespaces;

		public CPDBFileNamespaceSegment AddNamespace(string name) {
			CPDBFileNamespaceSegment ns = new(name);
			namespaces.Add(ns);
			return ns;
		}

		public void AddNamespace(CPDBFileNamespaceSegment ns) => namespaces.Add(ns);

		public void Write(BinaryWriter writer, StringHeap heap) {
			writer.Write7BitEncodedInt(namespaces.Count);
			foreach (var ns in namespaces)
				ns.Write(writer, heap);
		}

		public void Read(BinaryReader reader, StringHeap heap) {
			int count = reader.Read7BitEncodedInt();
			for (int i = 0; i < count; i++)
				namespaces.Add(CPDBFileNamespaceSegment.Read(reader, heap));
		}
	}
}
