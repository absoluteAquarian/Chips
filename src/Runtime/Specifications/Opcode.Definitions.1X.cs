using AsmResolver.PE.DotNet.Cil;
using Chips.Compiler;
using Chips.Compiler.Utility;
using Chips.Runtime.Types;
using System.IO;

namespace Chips.Runtime.Specifications {
	// push

	// pop

	public sealed class OpcodePop : Opcode {
		public sealed override int ExpectedArgumentCount => 0;

		public override OpcodeID Code => OpcodeID.Pop;

		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			context.Cursor.Emit(CilOpCodes.Pop);
		}

		public sealed override OpcodeArgumentCollection? DeserializeArguments(BinaryReader reader, TypeResolver resolver, StringHeap heap) => null;

		public sealed override OpcodeArgumentCollection? ParseArguments(CompilationContext context, string[] args) => null;

		public sealed override void SerializeArguments(BinaryWriter writer, OpcodeArgumentCollection args, TypeResolver resolver, StringHeap heap) { }
	}

	// dup

	public sealed class OpcodeCli : ModifyFlagsRegisterOpcode {
		public override bool SetsFlag => true;
		
		public override bool FlagValue => false;
		
		public override string Flag => nameof(FlagsRegister.InvalidFloat);

		public override OpcodeID Code => OpcodeID.Cli;
	}

	public sealed class OpcodeLdfia : LoadFieldOpcode {
		public override bool LoadsAddress => true;

		public override bool LoadsStaticField => false;

		public override OpcodeID Code => OpcodeID.Ldfia;
	}

	public sealed class OpcodeLdfsa : LoadFieldOpcode {
		public override bool LoadsAddress => true;

		public override bool LoadsStaticField => true;

		public override OpcodeID Code => OpcodeID.Ldfsa;
	}

	public sealed class OpcodeLdrga : LoadMethodVariableOpcode {
		public override bool LoadsAddress => true;

		public override bool LoadsLocal => false;

		public override OpcodeID Code => OpcodeID.Ldrga;
	}

	public sealed class OpcodeLdlca : LoadMethodVariableOpcode {
		public override bool LoadsAddress => true;

		public override bool LoadsLocal => true;

		public override OpcodeID Code => OpcodeID.Ldlca;
	}

	public sealed class OpcodeLdzi : LoadZeroOpcode {
		public override string Register => nameof(Registers.A);

		public override OpcodeID Code => OpcodeID.Ldzi;

		public override void Compile(CompilationContext context, OpcodeArgumentCollection args) {
			EmitRegisterAccess(context);

			context.Cursor.Emit(CilOpCodes.Ldc_I4_0);
			context.EmitNumberRegisterAssignment<IntegerRegister, int>();
		}
	}

	// ldind

	public sealed class OpcodeLdelaX : LoadElementInArrayOpcode {
		public override bool LoadsAddress => true;

		public override bool IndexWithXRegister => true;

		public override OpcodeID Code => OpcodeID.Ldela_X;
	}

	// bzs

	// bge

	// tostr

	// kbkey
}
