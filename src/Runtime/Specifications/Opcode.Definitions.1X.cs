using Chips.Runtime.Types;

namespace Chips.Runtime.Specifications {
	// push

	// pop

	public sealed class OpcodePop : Opcode {
		public override OpcodeID Code => OpcodeID.Pop;
	}

	public sealed class OpcodeDup : Opcode {
		public override OpcodeID Code => OpcodeID.Dup;
	}

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
