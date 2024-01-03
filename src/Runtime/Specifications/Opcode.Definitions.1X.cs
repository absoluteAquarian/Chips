using Chips.Runtime.Types;
using System;

namespace Chips.Runtime.Specifications {
	public sealed class OpcodePush : Opcode {
		public override OpcodeID Code => OpcodeID.Push;
	}

	public sealed class OpcodePopToRegister : Opcode {
		public override OpcodeID Code => OpcodeID.Pop_reg;
	}

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

	public sealed class OpcodeLdind : TypeOperandOpcode {
		public override OpcodeID Code => OpcodeID.Ldind;

		public override bool AllowsNull => false;
	}

	public sealed class OpcodeLdelaX : LoadElementInArrayOpcode {
		public override bool LoadsAddress => true;

		public override bool IndexWithXRegister => true;

		public override OpcodeID Code => OpcodeID.Ldela_X;
	}

	public sealed class OpcodeBzs : Opcode {
		public override OpcodeID Code => OpcodeID.Bzs;
	}

	public sealed class OpcodeBge : Opcode {
		public override OpcodeID Code => OpcodeID.Bge;
	}

	public sealed class OpcodeTostr : Opcode {
		public override OpcodeID Code => OpcodeID.Tostr;
	}

	public sealed class OpcodeKbkey : Opcode {
		public override OpcodeID Code => OpcodeID.Kbkey;

		public override unsafe nint Method => (nint)(delegate*<void>)&Implementation.Kbkey;

		// Method signature matches the default implementation for GetMethodSignature
	}
}
