using Chips.Compiler.Compilation;
using Chips.Runtime.Specifications;
using System.Collections.Generic;

namespace Chips {
	partial class ChipsCompiler {
		public static readonly Dictionary<OpcodeID, CompilingOpcode> compilingOpcodes;

		public static bool OpcodeIsDefined(string code) {
			for (int i = 0; i < opcodeNames.Length; i++)
				if (opcodeNames[i] == code)
					return true;

			return false;
		}

		public static List<CompilingOpcode> FindOpcode(string name) {
			List<CompilingOpcode> opcodes = new();

			foreach (var (id, code) in compilingOpcodes) {
				string opcodeName = id.ToString().ToLower();
				
				int index = opcodeName.IndexOf('_');
				if (index > 0)
					opcodeName = opcodeName[..index];

				opcodeName = RemapOpcodeName(opcodeName);

				if (opcodeName == name)
					opcodes.Add(code);
			}

			return opcodes;
		}

		private static string RemapOpcodeName(string code) {
			return code switch {
				// Main opcodes
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
				
				"addd" => "add.d",
				"subd" => "sub.d",
				"ldciu" => "ldc.iu",
				"ldcm" => "ldc.m",
				
				"muld" => "mul.d",
				"divd" => "div.d",
				"ldcl" => "ldc.l",

				"modd" => "mod.d",
				"repd" => "rep.d",
				"ldclu" => "ldc.lu",

				// Extended Type Opcodes
				"ldindi" => "ldind.i",
				"ldindbi" => "ldind.bi",
				"ldindsi" => "ldind.si",
				"ldindli" => "ldind.li",
				"ldindni" => "ldind.ni",

				"ldindu" => "ldind.u",
				"ldindbu" => "ldind.bu",
				"ldindsu" => "ldind.su",
				"ldindlu" => "ldind.lu",
				"ldindnu" => "ldind.nu",

				"ldindf" => "ldind.f",
				"ldindd" => "ldind.d",
				"ldindm" => "ldind.m",

				"ldindo" => "ldind.o",

				"convi" => "conv.i",
				"convbi" => "conv.bi",
				"convsi" => "conv.si",
				"convli" => "conv.li",
				"convni" => "conv.ni",

				"convu" => "conv.u",
				"convbu" => "conv.bu",
				"convsu" => "conv.su",
				"convlu" => "conv.lu",
				"convnu" => "conv.nu",

				"convf" => "conv.f",
				"convd" => "conv.d",
				"convm" => "conv.m",

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
