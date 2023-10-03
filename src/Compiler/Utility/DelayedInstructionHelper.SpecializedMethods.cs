using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.DotNet.Signatures;
using AsmResolver.PE.DotNet.Cil;
using AsmResolver.PE.DotNet.Metadata.Tables;
using AsmResolver.PE.DotNet.Metadata.Tables.Rows;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chips.Compiler.Utility {
	partial class DelayedInstructionHelper {
		private static ITypeDefOrRef? GetArgument(this CilInstruction instruction, CilMethodBody body) {
			return instruction.GetParameter(body.Owner.Parameters).ParameterType.Resolve()!;
		}

		private static ITypeDefOrRef? GetLocal(this CilInstruction instruction, CilMethodBody body) {
			return instruction.GetLocalVariable(body.LocalVariables).VariableType.Resolve()!;
		}

		private static ITypeDefOrRef? GetAddress() {
			return ChipsCompiler.ManifestModule.CorLibTypeFactory.IntPtr.Type;
		}

		private static ITypeDefOrRef? GetNull() {
			return null;
		}

		private static ITypeDefOrRef? GetInt32() {
			return ChipsCompiler.ManifestModule.CorLibTypeFactory.Int32.Type;
		}

		private static ITypeDefOrRef? GetInt64() {
			return ChipsCompiler.ManifestModule.CorLibTypeFactory.Int64.Type;
		}

		private static ITypeDefOrRef? GetNativeInt() {
			return GetAddress();
		}

		private static ITypeDefOrRef? GetNativeUnsignedInt() {
			return ChipsCompiler.ManifestModule.CorLibTypeFactory.UIntPtr.Type;
		}

		private static ITypeDefOrRef? GetSingle() {
			return ChipsCompiler.ManifestModule.CorLibTypeFactory.Single.Type;
		}

		private static ITypeDefOrRef? GetDouble() {
			return ChipsCompiler.ManifestModule.CorLibTypeFactory.Double.Type;
		}

		private static ITypeDefOrRef? GetObject() {
			return ChipsCompiler.ManifestModule.CorLibTypeFactory.Object.Type;
		}

		private static ITypeDefOrRef? GetString() {
			return ChipsCompiler.ManifestModule.CorLibTypeFactory.String.Type;
		}

		private static MethodSignature GetSignature(this CilInstruction instruction) {
			object? operand = instruction.Operand;
			MethodSignature? signature;
			if (operand is IMethodDescriptor methodDescriptor)
				signature = methodDescriptor.Signature;
			else
				signature = operand is StandAloneSignature { Signature: MethodSignature methodSig } ? methodSig : null;

			return signature!;
		}

		private static void PushAndPopMethodArgsAndReturn(this CilInstruction instruction, StrictEvaluationStackSimulator stack) {
			// calli has an additional operand for the method address
			// callvirt has an additional operand for the object reference
			if (instruction.OpCode.Code is CilCode.Calli or CilCode.Callvirt)
				stack.Pop();

			var signature = instruction.GetSignature();
			
			int argCount = signature.ParameterTypes.Count;
			for (int i = 0; i < argCount; i++)
				stack.Pop();

			if (signature.ReturnType.ElementType != ElementType.Void)
				stack.Push(signature.ReturnType.Resolve() ?? throw new ArgumentException("Invalid operand type for instruction."));
		}

		private static int GetBranchTarget(this CilInstruction instruction) {
			return ((ICilLabel)instruction.Operand!).Offset;
		}

		private static void UpdateSwitchOffsets(this CilInstruction instruction, int instructionIndex, StrictEvaluationStackSimulator stack) {
			// Pop the offset from the stack
			stack.Pop();

			IEnumerable<int> offsets = instruction.Operand switch {
				IEnumerable<ICilLabel> labels => labels.Select(static label => label.Offset),
				IEnumerable<int> directOffsets => directOffsets,
				_ => throw new ArgumentException("Invalid operand type for switch instruction.")
			};

			// Update the stack at the offsets
			foreach (int offset in offsets)
				stack.SetBranch(instructionIndex + offset);

			// Update the stack at the next instruction (falls through to next instruction)
			stack.SetBranch(instructionIndex + 1);
		}

		private static void PushAndPopArithmetic(StrictEvaluationStackSimulator stack) {
			// Assume the values can be used
			var type = stack.Pop();
			var type2 = stack.Pop();

			if (!_typeComparer.Equals(type, type2))
				throw new ArgumentException("Values on stack must have equivalent types for arithmetic instructions");

			stack.Push(type);
		}

		private static void PushAndPopComparison(StrictEvaluationStackSimulator stack) {
			// Assume the values can be used
			var type = stack.Pop();
			var type2 = stack.Pop();

			if (!_typeComparer.Equals(type, type2))
				throw new ArgumentException("Values on stack must have equivalent types for comparison instructions");

			stack.Push(GetInt32());
		}

		private static ITypeDefOrRef GetInlineTypeOperand(this CilInstruction instruction) {
			IMetadataMember member = (IMetadataMember)instruction.Operand!;

			return member.MetadataToken.Table switch {
				TableIndex.TypeDef => (TypeDefinition)member,
				TableIndex.TypeRef => (TypeReference)member,
				TableIndex.TypeSpec => (TypeSpecification)member,
				_ => throw new ArgumentException("Invalid operand type for instruction.")
			};
		}

		private static ITypeDefOrRef GetFieldOperandType(this CilInstruction instruction) {
			IMetadataMember member = (IMetadataMember)instruction.Operand!;

			IFieldDescriptor def = member.MetadataToken.Table switch {
				TableIndex.Field => (FieldDefinition)member,
				TableIndex.MemberRef => (MemberReference)member,
				_ => throw new ArgumentException("Invalid operand type for instruction.")
			};

			return def.DeclaringType?.ToTypeDefOrRef() ?? throw new ArgumentException("Invalid operand type for instruction.");
		}

		private static ITypeDefOrRef GetArrayOperandType(this CilInstruction instruction) {
			var type = instruction.GetInlineTypeOperand();
			return type.MakeSzArrayType().Resolve() ?? throw new ArgumentException("Invalid operand type for instruction.");
		}

		private static ITypeDefOrRef GetTypedReference() {
			return ChipsCompiler.ManifestModule.DefaultImporter.ImportType(typeof(TypedReference));
		}

		private static ITypeDefOrRef GetRuntimeHandle() {
			return ChipsCompiler.ManifestModule.DefaultImporter.ImportType(typeof(RuntimeMethodHandle));
		}

		private static ITypeDefOrRef GetTypeHandle() {
			return ChipsCompiler.ManifestModule.DefaultImporter.ImportType(typeof(RuntimeTypeHandle));
		}
	}
}
