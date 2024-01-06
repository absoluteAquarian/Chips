using Chips.Runtime.Types;

namespace Chips.Runtime.Specifications {
	public sealed class OpcodeClc : ModifyFlagsRegisterOpcode {
		public override bool SetsFlag => true;

		public override bool FlagValue => false;
		
		public override string Flag => nameof(FlagsRegister.Carry);
		
		public override OpcodeID Code => OpcodeID.Cli;
	}

	public sealed class OpcodeClv : ModifyFlagsRegisterOpcode {
		public override bool SetsFlag => true;

		public override bool FlagValue => false;

		public override string Flag => nameof(FlagsRegister.Overflow);

		public override OpcodeID Code => OpcodeID.Clv;
	}

	public sealed class OpcodeCln : ModifyFlagsRegisterOpcode {
		public override bool SetsFlag => true;

		public override bool FlagValue => false;

		public override string Flag => nameof(FlagsRegister.Negative);

		public override OpcodeID Code => OpcodeID.Cln;
	}

	public sealed class OpcodeClz : ModifyFlagsRegisterOpcode {
		public override bool SetsFlag => true;

		public override bool FlagValue => false;

		public override string Flag => nameof(FlagsRegister.Zero);

		public override OpcodeID Code => OpcodeID.Clz;
	}

	public sealed class OpcodeCls : ModifyFlagsRegisterOpcode {
		public override bool SetsFlag => true;

		public override bool FlagValue => false;

		public override string Flag => nameof(FlagsRegister.Conversion);

		public override OpcodeID Code => OpcodeID.Cls;
	}

	public sealed class OpcodeStfi : StoreToFieldOpcode {
		public override bool StoresToStaticField => false;

		public override OpcodeID Code => OpcodeID.Stfi;
	}

	public sealed class OpcodeStfs : StoreToFieldOpcode {
		public override bool StoresToStaticField => true;

		public override OpcodeID Code => OpcodeID.Stfs;
	}

	public sealed class OpcodeStrg : StoreToMethodVariableOpcode {
		public override bool StoresToLocal => false;

		public override OpcodeID Code => OpcodeID.Strg;
	}

	public sealed class OpcodeStlc : StoreToMethodVariableOpcode {
		public override bool StoresToLocal => true;

		public override OpcodeID Code => OpcodeID.Stlc;
	}

	public sealed class OpcodeLdzf : LoadZeroOpcode {
		public override string Register => nameof(Registers.F);

		public override OpcodeID Code => OpcodeID.Ldzf;
	}

	public sealed class OpcodeStind : TypeOperandOpcode {
		public override bool AllowsNull => false;

		public override OpcodeID Code => OpcodeID.Stind;
	}

	public sealed class OpcodeStelX : StoreToElementInArrayOpcode {
		public override bool IndexWithXRegister => true;

		public override OpcodeID Code => OpcodeID.Stel_X;
	}

	public sealed class OpcodeBzc : Opcode {
		public override OpcodeID Code => OpcodeID.Bzc;
	}

	public sealed class OpcodeBle : Opcode {
		public override OpcodeID Code => OpcodeID.Ble;
	}

	public sealed class OpcodeTostrFmt : Opcode {
		public override OpcodeID Code => OpcodeID.Tostr_fmt;
	}

	public sealed class OpcodeKbline : Opcode {
		public override OpcodeID Code => OpcodeID.Kbline;

		public override unsafe nint Method => (nint)(delegate*<void>)&Implementation.Kbline;

		// Method signature matches the default implementation for GetMethodSignature
	}
}
