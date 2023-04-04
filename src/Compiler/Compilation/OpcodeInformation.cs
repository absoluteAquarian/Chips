using Chips.Runtime.Meta;
using Chips.Runtime.Specifications;
using Chips.Utility;
using System;
using System.IO;

namespace Chips.Compilation {
	internal class OpcodeInformation {
		public readonly Opcode opcode;

		public readonly OpcodeArgument[] operands;

		public readonly string? contextSourceFile;
		public readonly int? contextSourceLine;

		public OpcodeInformation(byte[] stream, ref int readIndex) {
			ReadOpcode(stream, ref readIndex, out opcode, out operands);

			bool hasContext = stream[readIndex] != 0;
			readIndex++;

			if (hasContext) {
				//Has context...
				contextSourceFile = stream.GetStringFromData(ref readIndex);
				contextSourceLine = stream.Get7BitEncodedInt(readIndex, out int read);
				readIndex += read;
			}
		}

		private void ReadOpcode(byte[] stream, ref int readIndex, out Opcode opcode, out OpcodeArgument[] operands, bool previousByteIsParent = false) {
			//Read opcode byte, then the opcode data
			byte code = stream[readIndex];
			readIndex++;

			if (Metadata.op[code].IsParent) {
				ReadOpcode(stream, ref readIndex, out opcode, out operands, previousByteIsParent: true);
				return;
			}

			opcode = !previousByteIsParent ? Metadata.op[code] : Metadata.op[stream[readIndex - 1]].table![code];

			if (opcode.operandCount == 0) {
				operands = Array.Empty<OpcodeArgument>();
				return;
			}

			operands = new OpcodeArgument[opcode.operandCount];

			for (int i = 0; i < operands.Length; i++)
				ReadOperand(stream, ref readIndex, out operands[i]);
		}

		private static void ReadOperand(byte[] stream, ref int readIndex, out OpcodeArgument operand) {
			OperandInformationType type = (OperandInformationType)stream[readIndex];
			readIndex++;

			int dataLength = stream.Get7BitEncodedInt(readIndex, out int read);
			readIndex += read;

			byte[] data = stream.GetSlice(readIndex, dataLength);
			readIndex += dataLength;

			operand = type switch {
				OperandInformationType.Constant => new OpcodeArgumentConstant(data),
				OperandInformationType.Variable => new OpcodeArgumentVariable(data),
				OperandInformationType.Label => new OpcodeArgumentLabel(data),
				OperandInformationType.TypeString => new OpcodeArgumentTypeString(data),
				OperandInformationType.TypeCode => new OpcodeArgumentTypeCode(data),
				OperandInformationType.FunctionCall => new OpcodeArgumentFunctionCall(data),
				OperandInformationType.CollectionAccessIndexByX => new OpcodeArgumentCollectionAccessIndexByX(data),
				OperandInformationType.CollectionAccessIndexByY => new OpcodeArgumentCollectionAccessIndexByY(data),
				_ => throw new IOException("Unknown operand type read: " + type)
			};
		}
	}
}
