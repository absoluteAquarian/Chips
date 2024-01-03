using AsmResolver.DotNet;
using AsmResolver.PE.DotNet.Cil;
using Chips.Compiler.Utility;
using Chips.Runtime;
using Chips.Runtime.Specifications;
using Chips.Runtime.Types;

namespace Chips.Compiler.Compilation {
	public sealed class CompilingOpcodePush : RegisterOperandCompilingOpcode<OpcodePush> {
		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			Register arg = args.GetValue<Register>(0);

			context.EmitRegisterLoad(Registers.GetRegisterNameFromID(arg.ID));
			context.EmitRegisterValueRetrieval(arg);

			if (arg is IntegerRegister or FloatRegister)
				context.EmitNopAndDelayedResolver<DelayedINumberValueRetrievalResolver>();
		}
	}

	public sealed class CompilingOpcodePopToRegister : RegisterOperandCompilingOpcode<OpcodePopToRegister> {
		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			Register arg = args.GetValue<Register>(0);

			context.EmitRegisterLoad(Registers.GetRegisterNameFromID(arg.ID));
			context.EmitNopAndDelayedResolver<DelayedBoxOrImplicitObjectResolver>();
		}
	}

	public sealed class CompilingOpcodePop : BasicCompilingOpcode<OpcodePop> {
		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			context.Cursor.Emit(CilOpCodes.Pop);
		}
	}

	public sealed class CompilingOpcodeDup : BasicCompilingOpcode<OpcodeDup> {
		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			context.Cursor.Emit(CilOpCodes.Dup);
		}
	}

	public sealed class CompilingOpcodeCli : ModifyFlagsRegisterCompilingOpcode<OpcodeCli> { }

	public sealed class CompilingOpcodeLdfia : LoadFieldCompilingOpcode<OpcodeLdfia> { }

	public sealed class CompilingOpcodeLdfsa : LoadFieldCompilingOpcode<OpcodeLdfsa> { }

	public sealed class CompilingOpcodeLdrga : LoadMethodVariableCompilingOpcode<OpcodeLdrga> { }

	public sealed class CompilingOpcodeLdlca : LoadMethodVariableCompilingOpcode<OpcodeLdlca> { }

	public sealed class CompilingOpcodeLdzi : LoadZeroCompilingOpcode<OpcodeLdzi> {
		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			EmitRegisterAccess(context);

			context.Cursor.Emit(CilOpCodes.Ldc_I4_0);
			context.EmitNumberRegisterAssignment<IntegerRegister, int>();
		}
	}

	public sealed class CompilingOpcodeLdind : TypeOperandCompilingOpcode<OpcodeLdind> {
		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			/*     C# code:
			 *     
			 *     _ = (T)*(void*)(ValueConverter.CastToIntPtr(Registers.A.Value));
			 */

			context.EmitRegisterLoad(nameof(Registers.A));
			context.EmitRegisterValueRetrieval<IntegerRegister>();
			context.EmitCastToIntPtr();
			context.EmitNopAndDelayedResolver<DelayedLdobjResolver, ITypeDefOrRef>(args.GetValue<ITypeDefOrRef>(0));
		}
	}

	public sealed class CompilingOpcodeLdelaX : LoadElementInArrayCompilingOpcode<OpcodeLdelaX> { }

	public sealed class CompilingOpcodeBzs : LabelOperandCompilingOpcode<OpcodeBzs> {
		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			/*     C# code:
			 *     
			 *     if (Registers.F.Zero)
			 *         goto LABEL;
			 */

			context.EmitFlagRetrieval(nameof(FlagsRegister.Zero));
			context.EmitNopAndDelayedResolver<DelayedBranchIfTrueResolver, BytecodeMethodBody, ChipsLabel>(context.ActiveMethod, args.GetValue<ChipsLabel>(0));
		}
	}

	public sealed class CompilingOpcodeBge : LabelOperandCompilingOpcode<OpcodeBge> {
		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			/*     C# code:
			 *     
			 *     if (Registers.F.Zero || Registers.F.Negative)
			 *         goto LABEL;
			 */

			var label = args.GetValue<ChipsLabel>(0);

			context.EmitFlagRetrieval(nameof(FlagsRegister.Zero));
			context.EmitNopAndDelayedResolver<DelayedBranchIfTrueResolver, BytecodeMethodBody, ChipsLabel>(context.ActiveMethod, label);
			context.EmitFlagRetrieval(nameof(FlagsRegister.Negative));
			context.EmitNopAndDelayedResolver<DelayedBranchIfFalseResolver, BytecodeMethodBody, ChipsLabel>(context.ActiveMethod, label);
		}
	}

	public sealed class CompilingOpcodeTostr : BasicCompilingOpcode<OpcodeTostr> {
		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			/*    C# code:
			 *    
			 *    __tostr = (object)value;
			 *    Registers.S.Value = __tostr.ToString();
			 */

			int local = context.CreateOrGetLocal<object>("__tostr");

			// Need to make sure that the value is an object
			context.EmitNopAndDelayedResolver<DelayedBoxOrImplicitObjectResolver>();
			context.Cursor.Emit(CilOpCodes.Stloc, local);

			context.EmitRegisterLoad(nameof(Registers.S));

			context.Cursor.Emit(CilOpCodes.Ldloc, local);
			context.EmitFunctionCall<object>(nameof(object.ToString));

			context.EmitRegisterValueAssignment<StringRegister>();
		}
	}

	public sealed class CompilingOpcodeKbkey : BasicCompilingOpcode<OpcodeKbkey> {
		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			throw this.ThrowNotImplemented();
		}
	}
}
