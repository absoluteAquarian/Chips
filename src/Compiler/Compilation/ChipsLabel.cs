using System.IO;

namespace Chips.Compiler.Compilation {
	public sealed class ChipsLabel {
		public readonly string Name;

		public int Index { get; internal set; }

		public int OpcodeOffset { get; private set; }

		internal ChipsLabel(string name) {
			Name = name;
		}

		public void Mark(ChipsInstruction target) {
			OpcodeOffset = target.Offset;
		}

		internal void WriteMember(CompilationContext context, BinaryWriter writer) {
			if (string.IsNullOrWhiteSpace(Name))
				throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new InvalidDataException("Label name cannot be empty"));

			writer.Write7BitEncodedInt(OpcodeOffset);
		}
	}
}
