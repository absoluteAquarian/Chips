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
			[OpcodeID.Ldrg] = new OpcodeLdrg(),
			[OpcodeID.Ldlc] = new OpcodeLdlc(),
		};

		private static string RemapOpcodeName(string code) {
			return code switch {
				"ldci" => "ldc.i",
				"ldcf" => "ldc.f",
				"ldcs" => "ldc.s",
				"ldzs" => "ldz.s",
				"ldzi" => "ldz.i",
				"ldzf" => "ldz.f",
				"ldziu" => "ldz.iu",
				"ldzd" => "ldz.d",
				"muls" => "mul.s",
				"ldzl" => "ldz.l",
				"addf" => "add.f",
				"subf" => "sub.f",
				"mulf" => "mul.f",
				"divf" => "div.f",
				"modf" => "mod.f",
				"repf" => "rep.f",
				"negf" => "neg.f",
				"ldzlu" => "ldz.lu",
				"trims" => "trim.s",
				"trime" => "trim.e",
				"ldzm" => "ldz.m",
				"ldcd" => "ldc.d",
				"ldciu" => "ldc.iu",
				"ldcm" => "ldc.m",
				"ldcl" => "ldc.l",
				"ldclu" => "ldc.lu",
				// Extended Arithmetic opcodes
				"addl" => "add.l",
				"subl" => "sub.l",
				"mull" => "mul.l",
				"divl" => "div.l",
				"modl" => "mod.l",
				"repl" => "rep.l",
				"andl" => "and.l",
				"orl" => "or.l",
				"xorl" => "xor.l",
				"addiu" => "add.iu",
				"subiu" => "sub.iu",
				"muliu" => "mul.iu",
				"diviu" => "div.iu",
				"modiu" => "mod.iu",
				"repiu" => "rep.iu",
				"addlu" => "add.lu",
				"sublu" => "sub.lu",
				"mullu" => "mul.lu",
				"divlu" => "div.lu",
				"modlu" => "mod.lu",
				"replu" => "rep.lu",
				"addd" => "add.d",
				"subd" => "sub.d",
				"muld" => "mul.d",
				"divd" => "div.d",
				"modd" => "mod.d",
				"repd" => "rep.d",
				"addm" => "add.m",
				"subm" => "sub.m",
				"mulm" => "mul.m",
				"divm" => "div.m",
				"modm" => "mod.m",
				"repm" => "rep.m",
				_ => code
			};
		}
	}
}
