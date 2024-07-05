using Chips.Runtime.Types;

namespace Chips.Runtime.Specifications {
	public sealed class OpcodeStc : ModifyFlagsRegisterOpcode {
		public override bool SetsFlag => true;

		public override bool FlagValue => true;

		public override string Flag => nameof(FlagsRegister.Carry);

		public override OpcodeID Code => OpcodeID.Stc;
	}

	// inca

	// deca

	// inci

	// deci

	// cat

	// cat

	// find

	// find

	// ldz.iu

	// split

	// rem

	// bcs

	// call

	// calli

	// print
}
