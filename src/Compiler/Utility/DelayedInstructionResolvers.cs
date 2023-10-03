using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.DotNet.Signatures.Types;
using AsmResolver.PE.DotNet.Cil;
using AsmResolver.PE.DotNet.Metadata.Tables.Rows;
using System;

namespace Chips.Compiler.Utility {
	public interface IDelayedInstructionResolver {
		CilMethodBody Body { get; }

		int InstructionIndex { get; }

		public CilInstruction Instruction => Body.Instructions[InstructionIndex];

		void Resolve(StrictEvaluationStackSimulator stack);
	}

	public abstract class DelayedArrayIndexerResolver : IDelayedInstructionResolver {
		public CilMethodBody Body { get; }

		public int InstructionIndex { get; }

		public abstract bool LoadsValue { get; }

		public abstract bool LoadsAddress { get; }

		public DelayedArrayIndexerResolver(CilMethodBody body, int instructionIndex) {
			Body = body;
			InstructionIndex = instructionIndex;
		}

		public void Resolve(StrictEvaluationStackSimulator stack) {
			var instruction = ((IDelayedInstructionResolver)this).Instruction;

			// Expected stack:
			//   [ index, array, ... ] for loading value/address
			//   [ value, index, array, ... ] for storing value
			var arrayType = stack.PeekSkip(LoadsValue || LoadsAddress ? 1 : 2);

			if (arrayType is BoxedTypeDefOrRef boxed)
				arrayType = boxed.boxedType;  // Redirect to the boxed object type

			// Check if the array type is valid
			if (arrayType is not SzArrayTypeSignature szArrayType)
				throw new Exception($"Expected array type on stack, found \"{arrayType?.Name ?? "null"}\" instead");

			var elementType = szArrayType.BaseType;

			ITypeDefOrRef resolvedType;
			switch (elementType.ElementType) {
				case ElementType.None:
				case ElementType.Void:
					goto default;
				case ElementType.Boolean:
					if (LoadsAddress)
						instruction.ReplaceWith(CilOpCodes.Ldelema, ChipsCompiler.ManifestModule.CorLibTypeFactory.Boolean.Type);
					else
						instruction.ReplaceWith(LoadsValue ? CilOpCodes.Ldelem_I1 : CilOpCodes.Stelem_I1);
					break;
				case ElementType.Char:
					if (LoadsAddress)
						instruction.ReplaceWith(CilOpCodes.Ldelema, ChipsCompiler.ManifestModule.CorLibTypeFactory.Char.Type);
					else
						instruction.ReplaceWith(LoadsValue ? CilOpCodes.Ldelem_U2 : CilOpCodes.Stelem_I2);
					break;
				case ElementType.I1:
					if (LoadsAddress)
						instruction.ReplaceWith(CilOpCodes.Ldelema, ChipsCompiler.ManifestModule.CorLibTypeFactory.SByte.Type);
					else
						instruction.ReplaceWith(LoadsValue ? CilOpCodes.Ldelem_I1 : CilOpCodes.Stelem_I1);
					break;
				case ElementType.U1:
					if (LoadsAddress)
						instruction.ReplaceWith(CilOpCodes.Ldelema, ChipsCompiler.ManifestModule.CorLibTypeFactory.Byte.Type);
					else
						instruction.ReplaceWith(LoadsValue ? CilOpCodes.Ldelem_U1 : CilOpCodes.Stelem_I1);
					break;
				case ElementType.I2:
					if (LoadsAddress)
						instruction.ReplaceWith(CilOpCodes.Ldelema, ChipsCompiler.ManifestModule.CorLibTypeFactory.Int16.Type);
					else
						instruction.ReplaceWith(LoadsValue ? CilOpCodes.Ldelem_I2 : CilOpCodes.Stelem_I2);
					break;
				case ElementType.U2:
					if (LoadsAddress)
						instruction.ReplaceWith(CilOpCodes.Ldelema, ChipsCompiler.ManifestModule.CorLibTypeFactory.UInt16.Type);
					else
						instruction.ReplaceWith(LoadsValue ? CilOpCodes.Ldelem_U2 : CilOpCodes.Stelem_I2);
					break;
				case ElementType.I4:
					if (LoadsAddress)
						instruction.ReplaceWith(CilOpCodes.Ldelema, ChipsCompiler.ManifestModule.CorLibTypeFactory.Int32.Type);
					else
						instruction.ReplaceWith(LoadsValue ? CilOpCodes.Ldelem_I4 : CilOpCodes.Stelem_I4);
					break;
				case ElementType.U4:
					if (LoadsAddress)
						instruction.ReplaceWith(CilOpCodes.Ldelema, ChipsCompiler.ManifestModule.CorLibTypeFactory.UInt32.Type);
					else
						instruction.ReplaceWith(LoadsValue ? CilOpCodes.Ldelem_U4 : CilOpCodes.Stelem_I4);
					break;
				case ElementType.I8:
					if (LoadsAddress)
						instruction.ReplaceWith(CilOpCodes.Ldelema, ChipsCompiler.ManifestModule.CorLibTypeFactory.Int64.Type);
					else
						instruction.ReplaceWith(LoadsValue ? CilOpCodes.Ldelem_I8 : CilOpCodes.Stelem_I8);
					break;
				case ElementType.U8:
					if (LoadsAddress)
						instruction.ReplaceWith(CilOpCodes.Ldelema, ChipsCompiler.ManifestModule.CorLibTypeFactory.UInt64.Type);
					else
						instruction.ReplaceWith(LoadsValue ? CilOpCodes.Ldelem_I8 : CilOpCodes.Stelem_I8);
					break;
				case ElementType.R4:
					if (LoadsAddress)
						instruction.ReplaceWith(CilOpCodes.Ldelema, ChipsCompiler.ManifestModule.CorLibTypeFactory.Single.Type);
					else
						instruction.ReplaceWith(LoadsValue ? CilOpCodes.Ldelem_R4 : CilOpCodes.Stelem_R4);
					break;
				case ElementType.R8:
					if (LoadsAddress)
						instruction.ReplaceWith(CilOpCodes.Ldelema, ChipsCompiler.ManifestModule.CorLibTypeFactory.Double.Type);
					else
						instruction.ReplaceWith(LoadsValue ? CilOpCodes.Ldelem_R8 : CilOpCodes.Stelem_R8);
					break;
				case ElementType.String:
					if (LoadsAddress)
						instruction.ReplaceWith(CilOpCodes.Ldelema, ChipsCompiler.ManifestModule.CorLibTypeFactory.String.Type);
					else
						instruction.ReplaceWith(LoadsValue ? CilOpCodes.Ldelem_Ref : CilOpCodes.Stelem_Ref);
					break;
				case ElementType.Ptr:
					if (LoadsAddress)
						instruction.ReplaceWith(CilOpCodes.Ldelema, ChipsCompiler.ManifestModule.CorLibTypeFactory.IntPtr.Type);
					else
						instruction.ReplaceWith(LoadsValue ? CilOpCodes.Ldelem_I : CilOpCodes.Stelem_I);
					break;
				case ElementType.ByRef:
					goto default;
				case ElementType.ValueType:
					resolvedType = (ITypeDefOrRef?)elementType.Resolve() ?? throw new Exception($"Could not resolve type: {elementType}");

					if (LoadsAddress)
						instruction.ReplaceWith(CilOpCodes.Ldelema, resolvedType);
					else
						instruction.ReplaceWith(LoadsValue ? CilOpCodes.Ldelem : CilOpCodes.Stelem, resolvedType);
					break;
				case ElementType.Class:
					resolvedType = (ITypeDefOrRef?)elementType.Resolve() ?? throw new Exception($"Could not resolve type: {elementType}");

					if (LoadsAddress)
						instruction.ReplaceWith(CilOpCodes.Ldelema, resolvedType);
					else
						instruction.ReplaceWith(LoadsValue ? CilOpCodes.Ldelem_Ref : CilOpCodes.Stelem_Ref);
					break;
				case ElementType.Var:
					goto default;
				case ElementType.Array:
					resolvedType = (ITypeDefOrRef?)elementType.Resolve() ?? throw new Exception($"Could not resolve type: {elementType}");

					if (LoadsAddress)
						instruction.ReplaceWith(CilOpCodes.Ldelema, resolvedType);
					else
						instruction.ReplaceWith(LoadsValue ? CilOpCodes.Ldelem_Ref : CilOpCodes.Stelem_Ref);
					break;
				case ElementType.GenericInst:
					goto default;
				case ElementType.TypedByRef:
					resolvedType = (ITypeDefOrRef?)elementType.Resolve() ?? throw new Exception($"Could not resolve type: {elementType}");

					if (LoadsAddress)
						instruction.ReplaceWith(CilOpCodes.Ldelema, resolvedType);
					else
						instruction.ReplaceWith(LoadsValue ? CilOpCodes.Ldelem : CilOpCodes.Stelem, resolvedType);
					break;
				case ElementType.I:
					if (LoadsAddress)
						instruction.ReplaceWith(CilOpCodes.Ldelema, ChipsCompiler.ManifestModule.CorLibTypeFactory.IntPtr.Type);
					else
						instruction.ReplaceWith(LoadsValue ? CilOpCodes.Ldelem_I : CilOpCodes.Stelem_I);
					break;
				case ElementType.U:
					if (LoadsAddress)
						instruction.ReplaceWith(CilOpCodes.Ldelema, ChipsCompiler.ManifestModule.CorLibTypeFactory.UIntPtr.Type);
					else
						instruction.ReplaceWith(LoadsValue ? CilOpCodes.Ldelem_I : CilOpCodes.Stelem_I);
					break;
				case ElementType.FnPtr:
					if (LoadsAddress)
						instruction.ReplaceWith(CilOpCodes.Ldelema, ChipsCompiler.ManifestModule.CorLibTypeFactory.IntPtr.Type);
					else
						instruction.ReplaceWith(LoadsValue ? CilOpCodes.Ldelem_I : CilOpCodes.Stelem_I);
					break;
				case ElementType.Object:
				case ElementType.SzArray:
					resolvedType = (ITypeDefOrRef?)elementType.Resolve() ?? throw new Exception($"Could not resolve type: {elementType}");

					if (LoadsAddress)
						instruction.ReplaceWith(CilOpCodes.Ldelema, resolvedType);
					else
						instruction.ReplaceWith(LoadsValue ? CilOpCodes.Ldelem_Ref : CilOpCodes.Stelem_Ref);
					break;
				case ElementType.MVar:
				case ElementType.CModReqD:
				case ElementType.CModOpt:
				case ElementType.Internal:
				case ElementType.Modifier:
				case ElementType.Sentinel:
				case ElementType.Pinned:
					goto default;
				case ElementType.Type:
					resolvedType = (ITypeDefOrRef?)elementType.Resolve() ?? throw new Exception($"Could not resolve type: {elementType}");

					if (LoadsAddress)
						instruction.ReplaceWith(CilOpCodes.Ldelema, resolvedType);
					else
						instruction.ReplaceWith(LoadsValue ? CilOpCodes.Ldelem_Ref : CilOpCodes.Stelem_Ref);
					break;
				case ElementType.Boxed:
					goto default;
				case ElementType.Enum:
					resolvedType = (ITypeDefOrRef?)elementType.Resolve() ?? throw new Exception($"Could not resolve type: {elementType}");

					if (LoadsAddress)
						instruction.ReplaceWith(CilOpCodes.Ldelema, resolvedType);
					else
						instruction.ReplaceWith(LoadsValue ? CilOpCodes.Ldelem_Ref : CilOpCodes.Stelem_Ref);
					break;
				default:
					throw new Exception($"Unsupported array type on stack: {elementType.ElementType}");
			}
		}
	}

	public sealed class DelayedArrayLoadResolver : DelayedArrayIndexerResolver {
		public override bool LoadsValue => true;
		public override bool LoadsAddress => false;

		public DelayedArrayLoadResolver(CilMethodBody body, int instructionIndex) : base(body, instructionIndex) { }
	}

	public sealed class DelayedArrayStoreResolver : DelayedArrayIndexerResolver {
		public override bool LoadsValue => false;
		public override bool LoadsAddress => false;

		public DelayedArrayStoreResolver(CilMethodBody body, int instructionIndex) : base(body, instructionIndex) { }
	}

	public sealed class DelayedArrayLoadAddressResolver : DelayedArrayIndexerResolver {
		public override bool LoadsValue => false;
		public override bool LoadsAddress => true;

		public DelayedArrayLoadAddressResolver(CilMethodBody body, int instructionIndex) : base(body, instructionIndex) { }
	}

	public sealed class DelayedBoxResolver : IDelayedInstructionResolver {
		public CilMethodBody Body { get; }

		public int InstructionIndex { get; }

		public DelayedBoxResolver(CilMethodBody body, int instructionIndex) {
			Body = body;
			InstructionIndex = instructionIndex;
		}

		public void Resolve(StrictEvaluationStackSimulator stack) {
			var instruction = ((IDelayedInstructionResolver)this).Instruction;

			// Expected stack: [ value, ... ]

			var type = stack.Peek();
			instruction.ReplaceWith(CilOpCodes.Box, type);
		}
	}

	public sealed class DelayedUnboxResolver : IDelayedInstructionResolver {
		public CilMethodBody Body { get; }

		public int InstructionIndex { get; }

		public DelayedUnboxResolver(CilMethodBody body, int instructionIndex) {
			Body = body;
			InstructionIndex = instructionIndex;
		}

		public void Resolve(StrictEvaluationStackSimulator stack) {
			var instruction = ((IDelayedInstructionResolver)this).Instruction;

			// Expected stack: [ boxed value, ... ]

			var type = stack.Peek();

			if (type is not BoxedTypeDefOrRef boxed)
				throw new Exception($"Expected boxed object on stack, got \"{type?.Name ?? "null"}\" instead");

			instruction.ReplaceWith(CilOpCodes.Unbox_Any, boxed.boxedType);
		}
	}
}
