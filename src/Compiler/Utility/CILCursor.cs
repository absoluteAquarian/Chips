using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.DotNet.Collections;
using AsmResolver.PE.DotNet.Cil;
using AsmResolver.PE.DotNet.Metadata.Tables;
using System;
using System.Collections.Generic;

namespace Chips.Compiler.Utility {
	/// <summary>
	/// An object containing information about a CIL method body
	/// </summary>
	public sealed class CILCursor {
		public readonly CilMethodBody Body;

		public CilInstructionCollection Instructions => Body.Instructions;

		/// <summary>
		/// The location of the cursor within the method body
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// The instruction that this cursor is targetting
		/// </summary>
		public CilInstruction? Next => Index < Instructions.Count ? Instructions[Index] : null;

		/// <summary>
		/// The instruction before the instruction that this cursor is targetting
		/// </summary>
		public CilInstruction? Previous => Index > 0 ? Instructions[Index - 1] : null;

		public CILCursor(CilMethodBody body) {
			ArgumentNullException.ThrowIfNull(body);
			Body = body;
		}

		public CilInstructionLabel DefineLabel() => Next is null ? new CilInstructionLabel() : new CilInstructionLabel(Next);

		public void MarkLabel(ICilLabel label) {
			if (label is not CilInstructionLabel instructionLabel)
				throw new ArgumentException("Label is not a CilInstructionLabel", nameof(label));

			instructionLabel.Instruction = Next;
		}

		/// <inheritdoc cref="CilInstructionCollection.Insert(int, CilOpCode)"/>
		public CILCursor Emit(CilOpCode opcode) {
			Instructions.Insert(Index++, opcode);
			return this;
		}

		/// <inheritdoc cref="CilInstructionCollection.Insert(int, CilOpCode, ICilLabel)"/>
		public CILCursor Emit(CilOpCode code, ICilLabel label) {
			Instructions.Insert(Index++, code, label);
			return this;
		}

		/// <inheritdoc cref="CilInstructionCollection.Insert(int, CilOpCode, ICilLabel[])"/>
		public CILCursor Emit(CilOpCode code, params ICilLabel[] labels) {
			Instructions.Insert(Index++, code, labels);
			return this;
		}

		/// <inheritdoc cref="CilInstructionCollection.Insert(int, CilOpCode, IEnumerable{ICilLabel})"/>
		public CILCursor Emit(CilOpCode code, IEnumerable<ICilLabel> labels) {
			Instructions.Insert(Index++, code, labels);
			return this;
		}

		/// <inheritdoc cref="CilInstructionCollection.Insert(int, CilOpCode, int)"/>
		public CILCursor Emit(CilOpCode code, int constant) {
			Instructions.Insert(Index++, code, constant);
			return this;
		}

		/// <inheritdoc cref="CilInstructionCollection.Insert(int, CilOpCode, long)"/>
		public CILCursor Emit(CilOpCode code, long constant) {
			Instructions.Insert(Index++, code, constant);
			return this;
		}

		/// <inheritdoc cref="CilInstructionCollection.Insert(int, CilOpCode, float)"/>
		public CILCursor Emit(CilOpCode code, float constant) {
			Instructions.Insert(Index++, code, constant);
			return this;
		}

		/// <inheritdoc cref="CilInstructionCollection.Insert(int, CilOpCode, double)"/>
		public CILCursor Emit(CilOpCode code, double constant) {
			Instructions.Insert(Index++, code, constant);
			return this;
		}

		/// <inheritdoc cref="CilInstructionCollection.Insert(int, CilOpCode, string)"/>
		public CILCursor Emit(CilOpCode code, string constant) {
			Instructions.Insert(Index++, code, constant);
			return this;
		}

		/// <inheritdoc cref="CilInstructionCollection.Insert(int, CilOpCode, CilLocalVariable)"/>
		public CILCursor Emit(CilOpCode code, CilLocalVariable variable) {
			Instructions.Insert(Index++, code, variable);
			return this;
		}

		/// <inheritdoc cref="CilInstructionCollection.Insert(int, CilOpCode, Parameter)"/>
		public CILCursor Emit(CilOpCode code, Parameter parameter) {
			Instructions.Insert(Index++, code, parameter);
			return this;
		}

		/// <inheritdoc cref="CilInstructionCollection.Insert(int, CilOpCode, IFieldDescriptor)"/>
		public CILCursor Emit(CilOpCode code, IFieldDescriptor field) {
			Instructions.Insert(Index++, code, field);
			return this;
		}

		/// <inheritdoc cref="CilInstructionCollection.Insert(int, CilOpCode, IMethodDescriptor)"/>
		public CILCursor Emit(CilOpCode code, IMethodDescriptor method) {
			Instructions.Insert(Index++, code, method);
			return this;
		}

		/// <inheritdoc cref="CilInstructionCollection.Insert(int, CilOpCode, MemberReference)"/>
		public CILCursor Emit(CilOpCode code, MemberReference member) {
			Instructions.Insert(Index++, code, member);
			return this;
		}

		/// <inheritdoc cref="CilInstructionCollection.Insert(int, CilOpCode, ITypeDefOrRef)"/>
		public CILCursor Emit(CilOpCode code, ITypeDefOrRef type) {
			Instructions.Insert(Index++, code, type);
			return this;
		}

		/// <inheritdoc cref="CilInstructionCollection.Insert(int, CilOpCode, IMetadataMember)"/>
		public CILCursor Emit(CilOpCode code, IMetadataMember member) {
			Instructions.Insert(Index++, code, member);
			return this;
		}

		/// <inheritdoc cref="CilInstructionCollection.Insert(int, CilOpCode, StandAloneSignature)"/>
		public CILCursor Emit(CilOpCode code, StandAloneSignature signature) {
			Instructions.Insert(Index++, code, signature);
			return this;
		}

		/// <inheritdoc cref="CilInstructionCollection.Insert(int, CilOpCode, MetadataToken)"/>
		public CILCursor Emit(CilOpCode code, MetadataToken token) {
			Instructions.Insert(Index++, code, token);
			return this;
		}
	}
}
