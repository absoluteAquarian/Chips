using AsmResolver.PE.DotNet.Cil;
using Chips.Runtime.Specifications;
using System.IO;

namespace Chips.Compiler.Compilation {
	public sealed class ChipsInstruction {
		public OpcodeID Opcode;
		public readonly OpcodeArgumentCollection Operands;

		public int Offset { get; internal set; }

		internal CilInstruction FirstCilInstruction { get; set; }

		internal ChipsInstruction(OpcodeID opcode, params object?[] operands) {
			Opcode = opcode;
			Operands = new();
			Operands.AddRange(operands);
		}

		internal ChipsInstruction(OpcodeID opcode, OpcodeArgumentCollection? operands) {
			Opcode = opcode;
			Operands = operands ?? new();
		}

		public bool Write(CompilationContext context, BinaryWriter writer) {
			try {
				ushort code = (ushort)Opcode;
				if (code <= byte.MaxValue) {
					// Not an extended opcode
					writer.Write((byte)code);
				} else {
					// High byte first, then low byte
					writer.Write((byte)(code >> 8));
					writer.Write((byte)code);
				}

				ChipsCompiler.compilingOpcodes[Opcode].SerializeArguments(context, writer, Operands);
				return true;
			} catch {
				// An error was thrown; the instruction is malformed
				return false;
			}
		}

		public static ChipsInstruction Read(CompilationContext context, BinaryReader reader) {
			OpcodeID opcode = (OpcodeID)reader.ReadByte();

			// Read extended opcode if necessary
			byte code = (byte)opcode;
			if (code is 0xFD)
				opcode = (OpcodeID)((code << 8) | reader.ReadByte());

			if (!ChipsCompiler.compilingOpcodes.TryGetValue(opcode, out var opcodeInstance))
				throw ChipsCompiler.ErrorAndThrow(new InvalidDataException($"Opcode {opcode:X4} is not supported"));

			OpcodeArgumentCollection? operands = opcodeInstance.DeserializeArguments(context, reader);

			return new ChipsInstruction(opcode, operands);
		}
	}
}
