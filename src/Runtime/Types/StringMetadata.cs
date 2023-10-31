using System.IO;

namespace Chips.Runtime.Types {
	public readonly struct StringMetadata {
		public readonly int Offset;

		public StringMetadata(int offset) {
			Offset = offset;
		}

		public void Serialize(BinaryWriter writer) {
			writer.Write(Offset);
		}

		public static StringMetadata Deserialize(BinaryReader reader) {
			int offset = reader.ReadInt32();
			return new(offset);
		}
	}
}
