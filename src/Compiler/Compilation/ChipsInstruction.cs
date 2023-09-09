using Chips.Compiler.Utility;
using Chips.Runtime.Specifications;
using System.IO;

namespace Chips.Compiler.Compilation {
	public sealed class ChipsInstruction {
		public OpcodeID Opcode;
		public readonly OpcodeArgumentCollection Operands = new();

		public int Offset { get; internal set; }

		internal ChipsInstruction(OpcodeID opcode, params object?[] operands) {
			Opcode = opcode;
			Operands.AddRange(operands);
		}

		public bool Write(BinaryWriter writer, TypeResolver resolver) {
			try {
				writer.Write((byte)Opcode);
				ChipsCompiler.opcodes[Opcode].SerializeArguments(writer, Operands, resolver);
				return true;
			} catch {
				// An error was thrown; the instruction is malformed
				return false;
			}
		}
	}
}
