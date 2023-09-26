using System.IO;

namespace Chips.Compiler.Compilation {
	public sealed class ChipsLabel {
		public string Name { get; internal set; }

		public int Index { get; internal set; }

		public int OpcodeOffset { get; internal set; }

		internal ChipsLabel(string name) {
			Name = name;
		}

		public void Mark(ChipsInstruction target) {
			OpcodeOffset = target.Offset;
		}

		internal static ChipsLabel ReadMember(BinaryReader reader) {
			int offset = reader.Read7BitEncodedInt();

			// Resolve label later once the CPDB file is read, if it exists
			ChipsLabel label = new($"CHP_{offset:x4}") {
				OpcodeOffset = offset
			};

			return label;
		}

		internal void WriteMember(BinaryWriter writer) {
			if (string.IsNullOrWhiteSpace(Name))
				throw ChipsCompiler.ErrorAndThrow(new InvalidDataException("Label name cannot be empty"));

			writer.Write7BitEncodedInt(OpcodeOffset);
		}
	}
}
