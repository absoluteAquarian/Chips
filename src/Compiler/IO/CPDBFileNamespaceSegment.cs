using Chips.Compiler.Utility;
using System.Collections.Generic;
using System.IO;

namespace Chips.Compiler.IO {
	internal class CPDBFileNamespaceSegment {
		public readonly string name;

		private readonly List<CPDBFileTypeSegment> types = new();

		public IReadOnlyList<CPDBFileTypeSegment> Methods => types.AsReadOnly();

		public CPDBFileNamespaceSegment(string name) {
			this.name = name;
		}

		public CPDBFileTypeSegment AddType(string name) {
			var type = new CPDBFileTypeSegment(name);
			types.Add(type);
			return type;
		}

		public void AddType(CPDBFileTypeSegment type) => types.Add(type);

		public void Write(BinaryWriter writer, StringHeap heap) {
			StringMetadata token = heap.GetOrAdd(name);
			token.Serialize(writer);

			writer.Write7BitEncodedInt(types.Count);

			foreach (var type in types)
				type.Write(writer, heap);
		}

		public static CPDBFileNamespaceSegment Read(BinaryReader reader, StringHeap heap) {
			StringMetadata token = StringMetadata.Deserialize(reader);
			string name = heap.GetString(token);

			CPDBFileNamespaceSegment segment = new(name);
			
			int count = reader.Read7BitEncodedInt();
			for (int i = 0; i < count; i++)
				segment.AddType(CPDBFileTypeSegment.Read(reader, heap));

			return segment;
		}
	}
}
