using AsmResolver.DotNet;
using AsmResolver.PE.DotNet.Cil;
using Chips.Compiler.Utility;
using Chips.Runtime;
using Chips.Runtime.Specifications;
using Chips.Runtime.Types;

namespace Chips.Compiler.Compilation {
	public sealed class CompilingOpcodeClc : ModifyFlagsRegisterCompilingOpcode<OpcodeClc> { }

	public sealed class CompilingOpcodeClv : ModifyFlagsRegisterCompilingOpcode<OpcodeClv> { }

	public sealed class CompilingOpcodeCln : ModifyFlagsRegisterCompilingOpcode<OpcodeCln> { }

	public sealed class CompilingOpcodeClz : ModifyFlagsRegisterCompilingOpcode<OpcodeClz> { }

	public sealed class CompilingOpcodeCls : ModifyFlagsRegisterCompilingOpcode<OpcodeCls> { }

	public sealed class CompilingOpocdeStfi : StoreToFieldCompilingOpcode<OpcodeStfi> { }

	public sealed class CompilingOpcodeStfs : StoreToFieldCompilingOpcode<OpcodeStfs> { }

	public sealed class CompilingOpcodeStrg : StoreToMethodVariableCompilingOpcode<OpcodeStrg> { }

	public sealed class CompilingOpcodeStlc : StoreToMethodVariableCompilingOpcode<OpcodeStlc> { }

	public sealed class CompilingOpcodeLdzf : LoadZeroCompilingOpcode<OpcodeLdzf> {
		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			EmitRegisterAccess(context);

			context.Cursor.Emit(CilOpCodes.Ldc_R4, 0f);
			context.EmitNumberRegisterAssignment<FloatRegister, float>();
		}
	}

	public sealed class CompilingOpcodeStind : TypeOperandCompilingOpcode<OpcodeStind> {
		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			/*     C# code:
			 *     
			 *     Implementation.StoreIndirect<T>(VALUE);
			 */

			int index = context.Cursor.Index;
			var body = context.Cursor.Body;
			context.Cursor.Emit(CilOpCodes.Nop);
			ChipsCompiler.AddDelayedResolver(new DelayedGenericImplMethodResolver(body, index, args.GetValue<ITypeDefOrRef>(0), nameof(Implementation.StoreIndirect)));
		}
	}

	public sealed class CompilingOpocdeStelX : StoreToElementInArrayCompilingOpcode<OpcodeStelX> { }

	public sealed class CompilingOpocdeBzc : LabelOperandCompilingOpcode<OpcodeBzc> {
		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			/*     C# code:
			 *     
			 *     if (!Registers.F.Zero)
			 *         goto LABEL;
			 */

			context.EmitFlagRetrieval(nameof(FlagsRegister.Zero));
			context.EmitNopAndDelayedResolver<DelayedBranchIfFalseResolver, BytecodeMethodBody, ChipsLabel>(context.ActiveMethod, args.GetValue<ChipsLabel>(0));
		}
	}

	public sealed class CompilingOpcodeBle : LabelOperandCompilingOpcode<OpcodeBle> {
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
			context.EmitNopAndDelayedResolver<DelayedBranchIfTrueResolver, BytecodeMethodBody, ChipsLabel>(context.ActiveMethod, label);
		}
	}

	public sealed class CompilingOpcodeTostrFmt : BasicCompilingOpcode<OpcodeTostrFmt> {
		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			/*    C# code:
			 *    
			 *    string __tostr = FORMAT;
			 *    Implementation.ToStringFormat((object)VALUE, __tostr);
			 */

			int local = context.CreateOrGetLocal<string>("__tostr");
			context.Cursor.Emit(CilOpCodes.Stloc, local);

			// Need to make sure that the value is an object
			context.EmitNopAndDelayedResolver<DelayedBoxOrImplicitObjectResolver>();
			context.Cursor.Emit(CilOpCodes.Ldloc, local);
			context.EmitImplementationCall(nameof(Implementation.ToStringFormat));
		}
	}

	// kbline
	public sealed class CompilingOpcodeKbline : BasicCompilingOpcode<OpcodeKbline> {
		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			throw this.ThrowNotImplemented();
		}
	}
}
