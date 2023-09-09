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

		public void Write(BinaryWriter writer) {
			writer.Write(name);
			writer.Write7BitEncodedInt(types.Count);

			foreach (var type in types)
				type.Write(writer);
		}

		public static CPDBFileNamespaceSegment Read(BinaryReader reader) {
			string name = reader.ReadString();

			CPDBFileNamespaceSegment segment = new(name);
			
			int count = reader.Read7BitEncodedInt();
			for (int i = 0; i < count; i++)
				segment.AddType(CPDBFileTypeSegment.Read(reader));

			return segment;
		}
	}
}
