using Chips.Compiler.Utility;
using Chips.Runtime.Specifications;
using System.IO;

namespace Chips.Compiler.Compilation {
	public sealed class ChipsInstruction {
		public OpcodeID Opcode;
		public readonly OpcodeArgumentCollection Operands;

		public int Offset { get; internal set; }

		internal ChipsInstruction(OpcodeID opcode, params object?[] operands) {
			Opcode = opcode;
			Operands = new();
			Operands.AddRange(operands);
		}

		internal ChipsInstruction(OpcodeID opcode, OpcodeArgumentCollection? operands) {
			Opcode = opcode;
			Operands = operands ?? new();
		}

		public bool Write(BinaryWriter writer, TypeResolver resolver, StringHeap heap) {
			try {
				writer.Write((byte)Opcode);
				ChipsCompiler.opcodes[Opcode].SerializeArguments(writer, Operands, resolver, heap);
				return true;
			} catch {
				// An error was thrown; the instruction is malformed
				return false;
			}
		}

		public static ChipsInstruction Read(BinaryReader reader, TypeResolver resolver, StringHeap heap) {
			OpcodeID opcode = (OpcodeID)reader.ReadByte();

			// Read extended opcode if necessary
			byte code = (byte)opcode;
			if (code is 0xFD)
				opcode = (OpcodeID)((code << 8) | reader.ReadByte());

			if (!ChipsCompiler.opcodes.TryGetValue(opcode, out var opcodeInstance))
				throw ChipsCompiler.ErrorAndThrow(new InvalidDataException($"Opcode {opcode:X4} is not supported"));

			OpcodeArgumentCollection? operands = opcodeInstance.DeserializeArguments(reader, resolver, heap);

			return new ChipsInstruction(opcode, operands);
		}
	}
}
