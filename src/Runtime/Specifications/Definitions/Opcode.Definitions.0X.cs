using Chips.Runtime.Types;
using Chips.Runtime.Types.NumberProcessing;
using System;

namespace Chips.Runtime.Specifications {
	public sealed class OpcodeNop : Opcode {
		public override OpcodeID Code => OpcodeID.Nop;
	}

	public sealed class OpcodeBrk : Opcode {
		public override OpcodeID Code => OpcodeID.Brk;
	}

	public sealed class OpcodeLdci : LoadConstantOpcode<Int32_T> {
		public override Register Register => Registers.A;

		public override OpcodeID Code => OpcodeID.Ldci;
	}

	public sealed class OpcodeLdcf : LoadConstantOpcode<Single_T> {
		public override Register Register => Registers.I;

		public override OpcodeID Code => OpcodeID.Ldcf;
	}

	public sealed class OpcodeLdcs : LoadConstantOpcode<StringMetadata> {
		public override Register Register => Registers.S;

		public override OpcodeID Code => OpcodeID.Ldcs;
	}

	public sealed class OpcodeLdfi : LoadFieldOpcode {
		public override bool LoadsAddress => false;
		
		public override bool LoadsStaticField => false;

		public override OpcodeID Code => OpcodeID.Ldfi;
	}

	public sealed class OpcodeLdfs : LoadFieldOpcode {
		public override bool LoadsAddress => false;

		public override bool LoadsStaticField => true;

		public override OpcodeID Code => OpcodeID.Ldfs;
	}

	public sealed class OpcodeLdrg : LoadMethodVariableOpcode {
		public override OpcodeID Code => OpcodeID.Ldrg;

		public override bool LoadsLocal => false;

		public override bool LoadsAddress => false;
	}

	public sealed class OpcodeLdlc : LoadMethodVariableOpcode {
		public override OpcodeID Code => OpcodeID.Ldlc;

		public override bool LoadsLocal => true;

		public override bool LoadsAddress => false;
	}

	// ldmtd

	public sealed class OpcodeLdzs : LoadZeroOpcode {
		public override string Register => nameof(Registers.S);

		public override OpcodeID Code => OpcodeID.Ldzs;
	}

	public sealed class OpcodeLdelX : LoadElementInArrayOpcode {
		public override bool LoadsAddress => false;

		public override bool IndexWithXRegister => true;

		public override OpcodeID Code => OpcodeID.Ldel_X;
	}

	public sealed class OpcodeComp : Opcode {
		public override OpcodeID Code => OpcodeID.Comp;
	}

	public sealed class OpcodeIs : TypeOperandOpcode {
		public override OpcodeID Code => OpcodeID.Is;

		public override bool AllowsNull => true;
	}

	// conv

	public sealed class OpcodeConv : TypeOperandOpcode {
		public override OpcodeID Code => OpcodeID.Conv;

		public override bool AllowsNull => false;
	}

	public sealed class OpcodeKbrdy : Opcode {
		public override OpcodeID Code => OpcodeID.Kbrdy;

		public override unsafe nint Method => (nint)(delegate*<bool>)&Implementation.Kbrdy;

		public override void GetMethodSignature(out Type returnType, out Type[] parameterTypes) {
			returnType = typeof(bool);
			parameterTypes = Type.EmptyTypes;
		}
	}
}
