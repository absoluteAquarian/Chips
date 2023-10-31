using AsmResolver.PE.DotNet.Cil;
using Chips.Runtime.Specifications;
using Chips.Runtime.Types;

namespace Chips.Compiler.Compilation {
	// push

	// pop

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

	// ldind

	public sealed class CompilingOpcodeLdelaX : LoadElementInArrayCompilingOpcode<OpcodeLdelaX> { }

	// bzs

	// bge

	// tostr

	// kbkey
}
