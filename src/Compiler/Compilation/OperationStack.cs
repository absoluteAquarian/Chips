using Chips.Runtime.Specifications;
using System.Collections.Generic;

namespace Chips.Compilation {
	internal class OperationStack {
		public struct Operation {
			public ushort opcode;

			public object?[]? operands;
		}

		public readonly Stack<Operation> stack = new();

		public void Push(Opcode opcode, params object?[]? operands) {
			int code = opcode.code;

			if (opcode.HasParent)
				code |= opcode.Parent!.code << 8;

			stack.Push(new() { opcode = (ushort)code, operands = operands });
		}
	}
}
