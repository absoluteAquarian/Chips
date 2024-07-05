using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.DotNet.Signatures;
using AsmResolver.DotNet.Signatures.Types;
using AsmResolver.PE.DotNet.Cil;
using AsmResolver.PE.DotNet.Metadata.Tables.Rows;
using Chips.Compiler.Compilation;
using Chips.Runtime.Types.NumberProcessing;
using System;
using System.Linq;

namespace Chips.Compiler.Utility {
	public interface IDelayedInstructionResolver {
		CilMethodBody Body { get; }

		int InstructionIndex { get; set; }

		public CilInstruction Instruction => Body.Instructions[InstructionIndex];

		void Resolve(StrictEvaluationStackSimulator stack);
	}

	public interface IDelayedInstructionResolver<T> : IDelayedInstructionResolver where T : IDelayedInstructionResolver<T> {
		abstract static IDelayedInstructionResolver Create(CilMethodBody body, int instructionIndex);
	}

	public interface IDelayedInstructionResolver<T, TArg> : IDelayedInstructionResolver where T : IDelayedInstructionResolver<T, TArg> {
		abstract static IDelayedInstructionResolver Create(CilMethodBody body, int instructionIndex, TArg arg);
	}

	public interface IDelayedInstructionResolver<T, TArg1, TArg2> : IDelayedInstructionResolver where T : IDelayedInstructionResolver<T, TArg1, TArg2> {
		abstract static IDelayedInstructionResolver Create(CilMethodBody body, int instructionIndex, TArg1 arg1, TArg2 arg2);
	}

	public abstract class DelayedArrayIndexerResolver : IDelayedInstructionResolver {
		public CilMethodBody Body { get; }

		public int InstructionIndex { get; set; }

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

	public sealed class DelayedArrayLoadResolver : DelayedArrayIndexerResolver, IDelayedInstructionResolver<DelayedArrayLoadResolver> {
		public override bool LoadsValue => true;
		public override bool LoadsAddress => false;

		public DelayedArrayLoadResolver(CilMethodBody body, int instructionIndex) : base(body, instructionIndex) { }

		public static IDelayedInstructionResolver Create(CilMethodBody body, int instructionIndex) {
			return new DelayedArrayLoadResolver(body, instructionIndex);
		}
	}

	public sealed class DelayedArrayStoreResolver : DelayedArrayIndexerResolver, IDelayedInstructionResolver<DelayedArrayStoreResolver> {
		public override bool LoadsValue => false;
		public override bool LoadsAddress => false;

		public DelayedArrayStoreResolver(CilMethodBody body, int instructionIndex) : base(body, instructionIndex) { }

		public static IDelayedInstructionResolver Create(CilMethodBody body, int instructionIndex) {
			return new DelayedArrayStoreResolver(body, instructionIndex);
		}
	}

	public sealed class DelayedArrayLoadAddressResolver : DelayedArrayIndexerResolver, IDelayedInstructionResolver<DelayedArrayLoadAddressResolver> {
		public override bool LoadsValue => false;
		public override bool LoadsAddress => true;

		public DelayedArrayLoadAddressResolver(CilMethodBody body, int instructionIndex) : base(body, instructionIndex) { }

		public static IDelayedInstructionResolver Create(CilMethodBody body, int instructionIndex) {
			return new DelayedArrayLoadAddressResolver(body, instructionIndex);
		}
	}

	public sealed class DelayedBoxResolver : IDelayedInstructionResolver<DelayedBoxResolver> {
		public CilMethodBody Body { get; }

		public int InstructionIndex { get; set; }

		public DelayedBoxResolver(CilMethodBody body, int instructionIndex) {
			Body = body;
			InstructionIndex = instructionIndex;
		}

		public static IDelayedInstructionResolver Create(CilMethodBody body, int instructionIndex) {
			return new DelayedBoxResolver(body, instructionIndex);
		}

		public void Resolve(StrictEvaluationStackSimulator stack) {
			var instruction = ((IDelayedInstructionResolver)this).Instruction;

			// Expected stack: [ value, ... ]

			var type = stack.Peek();

			if (type?.IsValueType is not true)
				throw new Exception($"Expected value type object on stack, got \"{type?.Name ?? "null"}\" instead");

			instruction.ReplaceWith(CilOpCodes.Box, type);
		}
	}

	public sealed class DelayedBoxOrImplicitObjectResolver : IDelayedInstructionResolver<DelayedBoxOrImplicitObjectResolver> {
		public CilMethodBody Body { get; }

		public int InstructionIndex { get; set; }

		public DelayedBoxOrImplicitObjectResolver(CilMethodBody body, int instructionIndex) {
			Body = body;
			InstructionIndex = instructionIndex;
		}

		public static IDelayedInstructionResolver Create(CilMethodBody body, int instructionIndex) {
			return new DelayedBoxOrImplicitObjectResolver(body, instructionIndex);
		}

		public void Resolve(StrictEvaluationStackSimulator stack) {
			var instruction = ((IDelayedInstructionResolver)this).Instruction;

			// Expected stack: [ value, ... ]

			var type = stack.Peek();

			if (type?.IsValueType is not true)
				return;  // Reference types are implicitly readable as System.Object

			instruction.ReplaceWith(CilOpCodes.Box, type);
		}
	}

	public sealed class DelayedBoxOrCastclassObjectResolver : IDelayedInstructionResolver<DelayedBoxOrCastclassObjectResolver> {
		public CilMethodBody Body { get; }

		public int InstructionIndex { get; set; }

		public DelayedBoxOrCastclassObjectResolver(CilMethodBody body, int instructionIndex) {
			Body = body;
			InstructionIndex = instructionIndex;
		}

		public static IDelayedInstructionResolver Create(CilMethodBody body, int instructionIndex) {
			return new DelayedBoxOrCastclassObjectResolver(body, instructionIndex);
		}

		public void Resolve(StrictEvaluationStackSimulator stack) {
			var instruction = ((IDelayedInstructionResolver)this).Instruction;

			// Expected stack: [ value, ... ]

			var type = stack.Peek();

			if (type?.IsValueType is not true)
				instruction.ReplaceWith(CilOpCodes.Castclass, ChipsCompiler.ManifestModule.CorLibTypeFactory.Object.Type);
			else
				instruction.ReplaceWith(CilOpCodes.Box, type);
		}
	}

	public sealed class DelayedUnboxResolver : IDelayedInstructionResolver<DelayedUnboxResolver> {
		public CilMethodBody Body { get; }

		public int InstructionIndex { get; set; }

		public DelayedUnboxResolver(CilMethodBody body, int instructionIndex) {
			Body = body;
			InstructionIndex = instructionIndex;
		}

		public static IDelayedInstructionResolver Create(CilMethodBody body, int instructionIndex) {
			return new DelayedUnboxResolver(body, instructionIndex);
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

	public sealed class DelayedUnboxToExactTypeResolver : IDelayedInstructionResolver<DelayedUnboxToExactTypeResolver, ITypeDefOrRef> {
		public CilMethodBody Body { get; }

		public int InstructionIndex { get; set; }

		public ITypeDefOrRef Type { get; }

		public DelayedUnboxToExactTypeResolver(CilMethodBody body, int instructionIndex, ITypeDefOrRef type) {
			Body = body;
			InstructionIndex = instructionIndex;
			Type = type;
		}

		public static IDelayedInstructionResolver Create(CilMethodBody body, int instructionIndex, ITypeDefOrRef type) {
			return new DelayedUnboxToExactTypeResolver(body, instructionIndex, type);
		}

		private static SignatureComparer _typeComparer = new SignatureComparer(SignatureComparisonFlags.VersionAgnostic);

		public void Resolve(StrictEvaluationStackSimulator stack) {
			var instruction = ((IDelayedInstructionResolver)this).Instruction;
			var type = stack.Peek();

			// Expected stack: [ boxed value, ... ]

			if (type is null || (type is not BoxedTypeDefOrRef && _typeComparer.Equals(type.ToTypeSignature(), ChipsCompiler.ManifestModule.CorLibTypeFactory.Object)))
				throw new Exception($"Expected object on stack, got \"{type?.Name ?? "null"}\" instead");

			instruction.ReplaceWith(CilOpCodes.Unbox_Any, Type);
		}
	}

	public sealed class DelayedIsinstResolver : IDelayedInstructionResolver<DelayedIsinstResolver, ITypeDefOrRef> {
		public CilMethodBody Body { get; }

		public int InstructionIndex { get; set; }

		public ITypeDefOrRef Type { get; }

		public DelayedIsinstResolver(CilMethodBody body, int instructionIndex, ITypeDefOrRef type) {
			Body = body;
			InstructionIndex = instructionIndex;
			Type = type;
		}

		public static IDelayedInstructionResolver Create(CilMethodBody body, int instructionIndex, ITypeDefOrRef arg) {
			return new DelayedIsinstResolver(body, instructionIndex, arg);
		}

		public void Resolve(StrictEvaluationStackSimulator stack) {
			var instruction = ((IDelayedInstructionResolver)this).Instruction;

			// Expected stack: [ object, ... ]

			instruction.ReplaceWith(CilOpCodes.Isinst, Type);
		}
	}

	public sealed class DelayedUnboxOrCastclassResolver : IDelayedInstructionResolver<DelayedUnboxOrCastclassResolver, ITypeDefOrRef> {
		public CilMethodBody Body { get; }

		public int InstructionIndex { get; set; }

		public ITypeDefOrRef Type { get; }

		public DelayedUnboxOrCastclassResolver(CilMethodBody body, int instructionIndex, ITypeDefOrRef type) {
			Body = body;
			InstructionIndex = instructionIndex;
			Type = type;
		}

		public static IDelayedInstructionResolver Create(CilMethodBody body, int instructionIndex, ITypeDefOrRef arg) {
			return new DelayedUnboxOrCastclassResolver(body, instructionIndex, arg);
		}

		public void Resolve(StrictEvaluationStackSimulator stack) {
			var instruction = ((IDelayedInstructionResolver)this).Instruction;

			// Expected stack: [ object, ... ]

			if (Type.IsValueType) {
				if (stack.Peek() is not BoxedTypeDefOrRef)
					throw new Exception($"Expected boxed object on stack, got \"{stack.Peek()?.Name ?? "null"}\" instead");

				instruction.ReplaceWith(CilOpCodes.Unbox_Any, Type);
			} else
				instruction.ReplaceWith(CilOpCodes.Castclass, Type);
		}
	}

	public sealed class DelayedINumberValueRetrievalResolver : IDelayedInstructionResolver<DelayedINumberValueRetrievalResolver> {
		public CilMethodBody Body { get; }

		public int InstructionIndex { get; set; }

		public DelayedINumberValueRetrievalResolver(CilMethodBody body, int instructionIndex) {
			Body = body;
			InstructionIndex = instructionIndex;
		}

		public static IDelayedInstructionResolver Create(CilMethodBody body, int instructionIndex) {
			return new DelayedINumberValueRetrievalResolver(body, instructionIndex);
		}

		public void Resolve(StrictEvaluationStackSimulator stack) {
			var instruction = ((IDelayedInstructionResolver)this).Instruction;

			// Expected stack: [ INumber, ... ]

			var type = stack.Peek();

			if (type?.Resolve() is not TypeDefinition def || !def.Implements(typeof(INumber).FullName!))
				throw new Exception($"Expected INumber on stack, got \"{type?.Name ?? "null"}\" instead");

			if (def.Properties.FirstOrDefault(static p => p.Name?.Equals("ActualValue") ?? false) is not PropertyDefinition { GetMethod: MethodDefinition getMethod })
				throw new Exception($"Expected INumber on stack to have an ActualValue property");

			instruction.ReplaceWith(CilOpCodes.Call, getMethod);
		}
	}

	public abstract class DelayedBranchResolver : IDelayedInstructionResolver {
		public CilMethodBody Body { get; }

		public int InstructionIndex { get; set; }

		public BytecodeMethodBody ChipsBody { get; }

		public ChipsLabel Label { get; }

		public abstract CilOpCode Opcode { get; }

		public DelayedBranchResolver(CilMethodBody body, int instructionIndex, BytecodeMethodBody chipsBody, ChipsLabel label) {
			Body = body;
			InstructionIndex = instructionIndex;
			ChipsBody = chipsBody;
			Label = label;
		}

		public void Resolve(StrictEvaluationStackSimulator stack) {
			var instruction = ((IDelayedInstructionResolver)this).Instruction;

			// Determine the CilInstruction from the ChipsLabel
			var chipsOffset = Label.OpcodeOffset;
			var chipsInstruction = ChipsBody.Instructions[chipsOffset];
			var instructionReference = chipsInstruction.FirstCilInstruction;

			instruction.ReplaceWith(Opcode, new CilInstructionLabel(instructionReference));
		}
	}

	public sealed class DelayedBranchIfTrueResolver : DelayedBranchResolver, IDelayedInstructionResolver<DelayedBranchIfTrueResolver, BytecodeMethodBody, ChipsLabel> {
		public override CilOpCode Opcode => CilOpCodes.Brtrue;

		public DelayedBranchIfTrueResolver(CilMethodBody body, int instructionIndex, BytecodeMethodBody chipsBody, ChipsLabel label) : base(body, instructionIndex, chipsBody, label) { }

		public static IDelayedInstructionResolver Create(CilMethodBody body, int instructionIndex, BytecodeMethodBody arg1, ChipsLabel arg2) {
			return new DelayedBranchIfTrueResolver(body, instructionIndex, arg1, arg2);
		}
	}

	public sealed class DelayedBranchIfFalseResolver : DelayedBranchResolver, IDelayedInstructionResolver<DelayedBranchIfFalseResolver, BytecodeMethodBody, ChipsLabel> {
		public override CilOpCode Opcode => CilOpCodes.Brfalse;

		public DelayedBranchIfFalseResolver(CilMethodBody body, int instructionIndex, BytecodeMethodBody chipsBody, ChipsLabel label) : base(body, instructionIndex, chipsBody, label) { }

		public static IDelayedInstructionResolver Create(CilMethodBody body, int instructionIndex, BytecodeMethodBody arg1, ChipsLabel arg2) {
			return new DelayedBranchIfFalseResolver(body, instructionIndex, arg1, arg2);
		}
	}

	public sealed class DelayedLdobjResolver : IDelayedInstructionResolver<DelayedLdobjResolver, ITypeDefOrRef> {
		public CilMethodBody Body { get; }

		public int InstructionIndex { get; set; }

		public ITypeDefOrRef Type { get; }

		public DelayedLdobjResolver(CilMethodBody body, int instructionIndex, ITypeDefOrRef type) {
			Body = body;
			InstructionIndex = instructionIndex;
			Type = type;
		}

		public static IDelayedInstructionResolver Create(CilMethodBody body, int instructionIndex, ITypeDefOrRef arg) {
			return new DelayedLdobjResolver(body, instructionIndex, arg);
		}

		private static SignatureComparer _typeComparer = new SignatureComparer(SignatureComparisonFlags.VersionAgnostic);

		public void Resolve(StrictEvaluationStackSimulator stack) {
			var instruction = ((IDelayedInstructionResolver)this).Instruction;

			var signature = Type.ToTypeSignature();
			var factory = ChipsCompiler.ManifestModule.CorLibTypeFactory;

			if (_typeComparer.Equals(signature, factory.IntPtr))
				instruction.ReplaceWith(CilOpCodes.Ldind_I);
			else if (_typeComparer.Equals(signature, factory.SByte) || _typeComparer.Equals(signature, factory.Boolean))
				instruction.ReplaceWith(CilOpCodes.Ldind_I1);
			else if (_typeComparer.Equals(signature, factory.Int16))
				instruction.ReplaceWith(CilOpCodes.Ldind_I2);
			else if (_typeComparer.Equals(signature, factory.Int32))
				instruction.ReplaceWith(CilOpCodes.Ldind_I4);
			else if (_typeComparer.Equals(signature, factory.Int64) || _typeComparer.Equals(signature, factory.UInt64))
				instruction.ReplaceWith(CilOpCodes.Ldind_I8);
			else if (_typeComparer.Equals(signature, factory.Single))
				instruction.ReplaceWith(CilOpCodes.Ldind_R4);
			else if (_typeComparer.Equals(signature, factory.Double))
				instruction.ReplaceWith(CilOpCodes.Ldind_R8);
			else if (_typeComparer.Equals(signature, factory.Byte))
				instruction.ReplaceWith(CilOpCodes.Ldind_U1);
			else if (_typeComparer.Equals(signature, factory.UInt16))
				instruction.ReplaceWith(CilOpCodes.Ldind_U2);
			else if (_typeComparer.Equals(signature, factory.UInt32))
				instruction.ReplaceWith(CilOpCodes.Ldind_U4);
			else if (signature.IsValueType)
				instruction.ReplaceWith(CilOpCodes.Ldobj, Type);
			else
				instruction.ReplaceWith(CilOpCodes.Ldind_Ref);
		}
	}

	public sealed class DelayedStobjResolver : IDelayedInstructionResolver<DelayedStobjResolver, ITypeDefOrRef> {
		public CilMethodBody Body { get; }

		public int InstructionIndex { get; set; }

		public ITypeDefOrRef Type { get; }

		public DelayedStobjResolver(CilMethodBody body, int instructionIndex, ITypeDefOrRef type) {
			Body = body;
			InstructionIndex = instructionIndex;
			Type = type;
		}

		public static IDelayedInstructionResolver Create(CilMethodBody body, int instructionIndex, ITypeDefOrRef arg) {
			return new DelayedStobjResolver(body, instructionIndex, arg);
		}

		private static SignatureComparer _typeComparer = new SignatureComparer(SignatureComparisonFlags.VersionAgnostic);

		public void Resolve(StrictEvaluationStackSimulator stack) {
			var instruction = ((IDelayedInstructionResolver)this).Instruction;

			var signature = Type.ToTypeSignature();
			var factory = ChipsCompiler.ManifestModule.CorLibTypeFactory;

			if (_typeComparer.Equals(signature, factory.IntPtr))
				instruction.ReplaceWith(CilOpCodes.Stind_I);
			else if (_typeComparer.Equals(signature, factory.SByte) || _typeComparer.Equals(signature, factory.Byte) || _typeComparer.Equals(signature, factory.Boolean))
				instruction.ReplaceWith(CilOpCodes.Stind_I1);
			else if (_typeComparer.Equals(signature, factory.Int16) || _typeComparer.Equals(signature, factory.UInt16))
				instruction.ReplaceWith(CilOpCodes.Stind_I2);
			else if (_typeComparer.Equals(signature, factory.Int32) || _typeComparer.Equals(signature, factory.UInt32))
				instruction.ReplaceWith(CilOpCodes.Stind_I4);
			else if (_typeComparer.Equals(signature, factory.Int64) || _typeComparer.Equals(signature, factory.UInt64))
				instruction.ReplaceWith(CilOpCodes.Stind_I8);
			else if (_typeComparer.Equals(signature, factory.Single))
				instruction.ReplaceWith(CilOpCodes.Stind_R4);
			else if (_typeComparer.Equals(signature, factory.Double))
				instruction.ReplaceWith(CilOpCodes.Stind_R8);
			else if (signature.IsValueType)
				instruction.ReplaceWith(CilOpCodes.Stobj, Type);
			else
				instruction.ReplaceWith(CilOpCodes.Stind_Ref);
		}
	}

	public sealed class MethodStackScanner : IDelayedInstructionResolver {
		public CilMethodBody Body { get; }

		public int InstructionIndex {
			get => Body.Instructions.Count;
			set { }
		}

		public MethodStackScanner(CilMethodBody body) {
			Body = body;
		}

		public void Resolve(StrictEvaluationStackSimulator stack) {
			// Empty method, all that matters is that the full stack is scanned
		}
	}
}
