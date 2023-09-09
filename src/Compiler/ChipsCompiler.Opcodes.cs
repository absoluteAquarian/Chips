using Chips.Runtime.Specifications;
using System.Collections.Generic;

namespace Chips {
	partial class ChipsCompiler {
		public static readonly Dictionary<OpcodeID, Opcode> opcodes = new Dictionary<OpcodeID, Opcode>() {
			[OpcodeID.Nop] = new OpcodeNop(),
			[OpcodeID.Brk] = new OpcodeBrk(),
			[OpcodeID.Ldci] = new OpcodeLdci(),
			[OpcodeID.Ldcf] = new OpcodeLdcf(),
			[OpcodeID.Ldcs] = new OpcodeLdcs(),
			[OpcodeID.Ldfi] = new OpcodeLdfi(),
			[OpcodeID.Ldfs] = new OpcodeLdfs(),
		};
	}
}
