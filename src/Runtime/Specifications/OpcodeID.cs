﻿namespace Chips.Runtime.Specifications {
	public enum OpcodeID : byte {
		Nop        = 0x00,
		Brk        = 0x01,
		Ldci       = 0x02,
		Ldcf       = 0x03,
		Ldcs       = 0x04,
		Ldfi       = 0x05,
		Ldfs       = 0x06,
		Ldrg       = 0x07,
		Ldlc       = 0x08,
		Ldmtd      = 0x09,
		Ldns       = 0x0A,
		Ldel_X     = 0x0B,
		Comp       = 0x0C,
		Is         = 0x0D,
		Conv       = 0x0E,
		Kbrdy      = 0x0F,

		Push       = 0x10,
		Pop_reg    = 0x11,
		Pop        = 0x12,
		Dup        = 0x13,
		Cli        = 0x14,
		Ldfia      = 0x15,
		Ldfsa      = 0x16,
		Ldrga      = 0x17,
		Ldlca      = 0x18,
		Ldzi       = 0x19,
		Ldind      = 0x1A,
		Ldela_X    = 0x1B,
		Bzs        = 0x1C,
		Bge        = 0x1D,
		Tostr      = 0x1E,
		Kbkey      = 0x1F,

		Clc        = 0x20,
		Clv        = 0x21,
		Cln        = 0x22,
		Clz        = 0x23,
		Cls        = 0x24,
		Stfi       = 0x25,
		Stfs       = 0x26,
		Strg       = 0x27,
		Stlc       = 0x28,
		Ldzf       = 0x29,
		Stind      = 0x2A,
		Stel_X     = 0x2B,
		Bzc        = 0x2C,
		Ble        = 0x2D,
		Tostr_fmt  = 0x2E,
		Kblin      = 0x2F,

		Stc        = 0x30,
		Inca       = 0x31,
		Deca       = 0x32,
		Inci       = 0x33,
		Deci       = 0x34,
		Cat_str    = 0x35,
		Cat        = 0x36,
		Find       = 0x37,
		Find_str   = 0x38,
		Ldziu      = 0x39,
		Split      = 0x3A,
		Rem        = 0x3B,
		Bcs        = 0x3C,
		Call       = 0x3D,
		Calli      = 0x3E,
		Print      = 0x3F,

		Add        = 0x40,
		Sub        = 0x41,
		Mul        = 0x42,
		Div        = 0x43,
		Mod        = 0x44,
		Rep        = 0x45,
		And        = 0x46,
		Or         = 0x47,
		Xor        = 0x48,
		Ldzd       = 0x49,
		Muls       = 0x4A,
		Ldel_Y     = 0x4B,
		Bcc        = 0x4C,
		Ret        = 0x4D,
		// 0x4E
		Prntl      = 0x4F,

		Add_int    = 0x50,
		Sub_int    = 0x51,
		Mul_int    = 0x52,
		Div_int    = 0x53,
		Mod_int    = 0x54,
		Rep_int    = 0x55,
		And_int    = 0x56,
		Or_int     = 0x57,
		Xor_int    = 0x58,
		Ldzl       = 0x59,
		Trim       = 0x5A,
		Ldela_Y    = 0x5B,
		Bns        = 0x5C,
		// 0x5D
		// 0x5E
		// 0x5F

		Addf       = 0x60,
		Subf       = 0x61,
		Mulf       = 0x62,
		Divf       = 0x63,
		Modf       = 0x64,
		Repf       = 0x65,
		Not        = 0x66,
		Neg        = 0x67,
		Negf       = 0x68,
		Ldzlu      = 0x69,
		Trims      = 0x6A,
		Stel_Y     = 0x6B,
		Bnc        = 0x6C,
		// 0x6D
		// 0x6E
		// 0x6F

		Addf_float = 0x70,
		Subf_float = 0x71,
		Mulf_float = 0x72,
		Divf_float = 0x73,
		Modf_float = 0x74,
		Repf_float = 0x75,
		Asr        = 0x76,
		Asl        = 0x77,
		Ror        = 0x78,
		Rol        = 0x79,
		Trime      = 0x7A,
		// 0x7B
		Bvs        = 0x7C,
		// 0x7D
		// 0x7E
		// 0x7F

		Traf       = 0x80,
		Trad       = 0x81,
		Trfi       = 0x82,
		Trfl       = 0x83,
		Trax       = 0x84,
		Tray       = 0x85,
		Asr_int    = 0x86,
		Asl_int    = 0x87,
		// 0x88,
		Ldzm       = 0x89,
		// 0x8A
		// 0x8B
		Bvc        = 0x8C,
		// 0x8D
		// 0x8E
		// 0x8F

		Nwarr      = 0x90,
		Nwobj      = 0x91,
		Len        = 0x92,
		// 0x93
		Trxa       = 0x94,
		Trya       = 0x95,
		// 0x96
		// 0x97
		// 0x98
		// 0x99
		// 0x9A
		// 0x9B
		Bss        = 0x9C,
		// 0x9D
		// 0x9E
		// 0x9F

		// 0xA0
		// 0xA1
		// 0xA2
		// 0xA3
		// 0xA4
		// 0xA5
		// 0xA6
		// 0xA7
		// 0xA8
		// 0xA9
		// 0xAA
		// 0xAB
		Bsc        = 0xAC,
		// 0xAD
		// 0xAE
		// 0xAF

		// 0xB0
		// 0xB1
		// 0xB2
		// 0xB3
		// 0xB4
		// 0xB5
		// 0xB6
		// 0xB7
		// 0xB8
		// 0xB9
		// 0xBA
		// 0xBB
		Bis        = 0xBC,
		// 0xBD
		// 0xBE
		// 0xBF

		// 0xC0
		// 0xC1
		// 0xC2
		// 0xC3
		// 0xC4
		// 0xC5
		// 0xC6
		// 0xC7
		// 0xC8
		// 0xC9
		// 0xCA
		// 0xCB
		Bic        = 0xCC,
		// 0xCD
		// 0xCE
		// 0xCF

		// 0xD0
		// 0xD1
		// 0xD2
		// 0xD3
		// 0xD4
		// 0xD5
		// 0xD6
		// 0xD7
		// 0xD8
		// 0xD9
		// 0xDA
		// 0xDB
		// 0xDC
		// 0xDD
		// 0xDE
		// 0xDF

		// 0xE0
		// 0xE1
		// 0xE2
		// 0xE3
		// 0xE4
		// 0xE5
		// 0xE6
		// 0xE7
		// 0xE8
		// 0xE9
		// 0xEA
		// 0xEB
		// 0xEC
		// 0xED
		// 0xEE
		// 0xEF

		// 0xF0
		// 0xF1
		// 0xF2
		// 0xF3
		// 0xF4
		// 0xF5
		// 0xF6
		// 0xF7
		// 0xF8
		// 0xF9
		// 0xFA
		// 0xFB
		// 0xFC
		// 0xFD
		Wait       = 0xFE,
		Halt       = 0xFF
	}
}
