using Chips.Runtime.Specifications;
using System;
using System.Collections.Generic;

namespace Chips.Compiler.Compilation {
	/// <summary>
	/// An object representing the instructions of a Chips method
	/// </summary>
	public sealed class BytecodeMethodBody {
		public readonly BytecodeMethodSegment Method;
		public readonly List<ChipsInstruction> Instructions;

		public readonly List<ChipsLabel> Labels;

		internal BytecodeMethodBody(BytecodeMethodSegment method) {
			Method = method;
			Instructions = new();
			Labels = new();
		}

		public ChipsLabel ReserveLabel(string name) {
			var label = new ChipsLabel(name) {
				Index = Labels.Count
			};
			Labels.Add(label);
			return label;
		}

		public ChipsLabel ReserveLabel() => ReserveLabel("CHP_" + Instructions.Count);

		public ChipsLabel FindLabel(string name) {
			foreach (ChipsLabel label in Labels) {
				if (label.Name == name)
					return label;
			}

			throw new ArgumentException("Label not found: " + name);
		}

		public ChipsInstruction Emit(OpcodeID opcode, object? arg) {
			var instruction = new ChipsInstruction(opcode, arg) {
				Offset = Instructions.Count
			};
			Instructions.Add(instruction);
			return instruction;
		}

		public ChipsInstruction Emit(OpcodeID opcode, object? arg1, object? arg2) {
			var instruction = new ChipsInstruction(opcode, arg1, arg2) {
				Offset = Instructions.Count
			};
			Instructions.Add(instruction);
			return instruction;
		}

		public ChipsInstruction Emit(OpcodeID opcode, object? arg1, object? arg2, object? arg3) {
			var instruction = new ChipsInstruction(opcode, arg1, arg2, arg3) {
				Offset = Instructions.Count
			};
			Instructions.Add(instruction);
			return instruction;
		}

		public ChipsInstruction Emit(OpcodeID opcode, params object?[] args) {
			var instruction = new ChipsInstruction(opcode, args) {
				Offset = Instructions.Count
			};
			Instructions.Add(instruction);
			return instruction;
		}
	}
}
