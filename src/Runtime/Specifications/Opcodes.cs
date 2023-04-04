namespace Chips.Runtime.Specifications{
	public static unsafe class Opcodes{
		public static readonly Opcode Abs              = new(0x39, &Opcode.Functions.Abs,     "abs"                          , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Acos             = new(0xA3, &Opcode.Functions.Acos,    "acos"                         , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Acsh             = new(0xA5, &Opcode.Functions.Acsh,    "acsh"                         , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Add_obj          = new(0x30, &Opcode.Functions.Add,     "add <obj>"                    , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Add_var          = new(0x40, &Opcode.Functions.Add,     "add <var>"                    , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Aems             = new(0xAC, &Opcode.Functions.Aems,    "aems"                         , 0, true,
			OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode And_obj          = new(0x00, &Opcode.Functions.And,     "and <obj>"                    , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode And_var          = new(0x20, &Opcode.Functions.And,     "and <var>"                    , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Art_obj          = new(0x35, &Opcode.Functions.Art,     "art <obj>"                    , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Art_var          = new(0x45, &Opcode.Functions.Art,     "art <var>"                    , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Asin             = new(0xA2, &Opcode.Functions.Asin,    "asin"                         , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Asl              = new(0x0D, &Opcode.Functions.Asl,     "asl"                          , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Asnh             = new(0x95, &Opcode.Functions.Asnh,    "asnh"                         , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Asr              = new(0x0E, &Opcode.Functions.Asr,     "asr"                          , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Atan             = new(0xA4, &Opcode.Functions.Atan,    "atan"                         , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Atnh             = new(0xB5, &Opcode.Functions.Atnh,    "atnh"                         , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Atnt_obj         = new(0xC4, &Opcode.Functions.Atnt,    "atnt <obj>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Atnt_var         = new(0xD4, &Opcode.Functions.Atnt,    "atnt <var>"                   , 1, false,
			OpcodeClassification.Arithmetic);

		public static readonly Opcode Bfc              = new(0x11, &Opcode.Functions.Br,      "bfc <label>"                  , 1, false,
			OpcodeClassification.Branching);
		public static readonly Opcode Bfn              = new(0x13, &Opcode.Functions.Br,      "bfn <label>"                  , 1, false,
			OpcodeClassification.Branching);
		public static readonly Opcode Bfo              = new(0x15, &Opcode.Functions.Br,      "bfo <label>"                  , 1, false,
			OpcodeClassification.Branching);
		public static readonly Opcode Bfp              = new(0x84, &Opcode.Functions.Br,      "bfp <label>"                  , 1, false,
			OpcodeClassification.Branching);
		public static readonly Opcode Bfr              = new(0x17, &Opcode.Functions.Br,      "bfr <label>"                  , 1, false,
			OpcodeClassification.Branching);
		public static readonly Opcode Bfz              = new(0x19, &Opcode.Functions.Br,      "bfz <label>"                  , 1, false,
			OpcodeClassification.Branching);
		public static readonly Opcode Blg              = new(0x38, &Opcode.Functions.Blg,     "blg"                          , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Br               = new(0x1A, &Opcode.Functions.Br,      "br <label>"                   , 1, false,
			OpcodeClassification.Branching);
		public static readonly Opcode Brf_obj          = new(0x7C, &Opcode.Functions.Br,      "brf <obj>, <label>"           , 2, false,
			OpcodeClassification.Branching);
		public static readonly Opcode Brf_var          = new(0x7D, &Opcode.Functions.Br,      "brf <var>, <label>"           , 2, false,
			OpcodeClassification.Branching);
		public static readonly Opcode Brt_obj          = new(0x7A, &Opcode.Functions.Br,      "brt <obj>, <label>"           , 2, false,
			OpcodeClassification.Branching);
		public static readonly Opcode Brt_var          = new(0x7B, &Opcode.Functions.Br,      "brt <var>, <label>"           , 2, false,
			OpcodeClassification.Branching);
		public static readonly Opcode Btc              = new(0x10, &Opcode.Functions.Br,      "btc <label>"                  , 1, false,
			OpcodeClassification.Branching);
		public static readonly Opcode Btn              = new(0x12, &Opcode.Functions.Br,      "btn <label>"                  , 1, false,
			OpcodeClassification.Branching);
		public static readonly Opcode Bto              = new(0x14, &Opcode.Functions.Br,      "bto <label>"                  , 1, false,
			OpcodeClassification.Branching);
		public static readonly Opcode Btp              = new(0x74, &Opcode.Functions.Br,      "btp <label>"                  , 1, false,
			OpcodeClassification.Branching);
		public static readonly Opcode Btr              = new(0x16, &Opcode.Functions.Br,      "btr <label>"                  , 1, false,
			OpcodeClassification.Branching);
		public static readonly Opcode Btz              = new(0x18, &Opcode.Functions.Br,      "btz <label>"                  , 1, false,
			OpcodeClassification.Branching);
		public static readonly Opcode Bin              = new(0x67, &Opcode.Functions.Bin,     "bin"                          , 0, false,
			OpcodeClassification.ModifiesRegister | OpcodeClassification.NoOperand);
		public static readonly Opcode Binz             = new(0x68, &Opcode.Functions.Binz,    "binz"                         , 0, false,
			OpcodeClassification.ModifiesRegister | OpcodeClassification.NoOperand);
		public static readonly Opcode Bit_obj          = new(0x69, &Opcode.Functions.Bit,     "bit <obj>"                    , 1, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Bit_var          = new(0x79, &Opcode.Functions.Bit,     "bit <var>"                    , 1, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Bits             = new(0x6A, &Opcode.Functions.Bits,    "bits"                         , 0, false,
			OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);

		public static readonly Opcode Call             = new(0x1B, &Opcode.Functions.Call,    "call <func>"                  , 1, false,
			OpcodeClassification.ModifiesEvaluationStack | OpcodeClassification.Branching);
		public static readonly Opcode Caps             = new(0xF7, &Opcode.Functions.Caps,    "caps"                         , 0, false,
			OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Cast             = new(0x63, &Opcode.Functions.Cast,    "cast <type>"                  , 1, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Cclb             = new(0xF5, &Opcode.Functions.Cclb,    "cclb"                         , 0, false,
			OpcodeClassification.Console | OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Cclf             = new(0xF6, &Opcode.Functions.Cclf,    "cclf"                         , 0, false,
			OpcodeClassification.Console | OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Ceil             = new(0xA1, &Opcode.Functions.Ceil,    "ceil"                         , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Ceq_obj          = new(0x05, &Opcode.Functions.Ceq,     "ceq <obj>"                    , 1, false,
			OpcodeClassification.Comparison);
		public static readonly Opcode Ceq_var          = new(0x25, &Opcode.Functions.Ceq,     "ceq <var>"                    , 1, false,
			OpcodeClassification.Comparison);
		public static readonly Opcode Cge_obj          = new(0x08, &Opcode.Functions.Cge,     "cge <obj>"                    , 1, false,
			OpcodeClassification.Comparison);
		public static readonly Opcode Cge_var          = new(0x28, &Opcode.Functions.Cge,     "cge <var>"                    , 1, false,
			OpcodeClassification.Comparison);
		public static readonly Opcode Cgt_obj          = new(0x06, &Opcode.Functions.Cgt,     "cgt <obj>"                    , 1, false,
			OpcodeClassification.Comparison);
		public static readonly Opcode Cgt_var          = new(0x26, &Opcode.Functions.Cgt,     "cgt <var>"                    , 1, false,
			OpcodeClassification.Comparison);
		public static readonly Opcode Clc              = new(0x0F, &Opcode.Functions.Clc,     "clc"                          , 0, true,
			OpcodeClassification.ModifiesFlag | OpcodeClassification.NoOperand);
		public static readonly Opcode Cle_obj          = new(0x09, &Opcode.Functions.Cle,     "cle <obj>"                    , 1, false,
			OpcodeClassification.Comparison);
		public static readonly Opcode Cle_var          = new(0x29, &Opcode.Functions.Cle,     "cle <var>"                    , 1, false,
			OpcodeClassification.Comparison);
		public static readonly Opcode Cln              = new(0x1F, &Opcode.Functions.Cln,     "cln"                          , 0, true,
			OpcodeClassification.ModifiesFlag | OpcodeClassification.NoOperand);
		public static readonly Opcode Clo              = new(0x2F, &Opcode.Functions.Clo,     "clo"                          , 0, true,
			OpcodeClassification.ModifiesFlag | OpcodeClassification.NoOperand);
		public static readonly Opcode Clp              = new(0x6F, &Opcode.Functions.Clp,     "clp"                          , 0, true,
			OpcodeClassification.ModifiesFlag | OpcodeClassification.NoOperand);
		public static readonly Opcode Clr              = new(0x3F, &Opcode.Functions.Clr,     "clr"                          , 0, true,
			OpcodeClassification.ModifiesFlag | OpcodeClassification.NoOperand);
		public static readonly Opcode Cls              = new(0xFD, &Opcode.Functions.Cls,     "cls"                          , 0, false,
			OpcodeClassification.Console | OpcodeClassification.NoOperand);
		public static readonly Opcode Clt_obj          = new(0x05, &Opcode.Functions.Clt,     "clt <obj>"                    , 1, false,
			OpcodeClassification.Comparison);
		public static readonly Opcode Ctl_var          = new(0x25, &Opcode.Functions.Clt,     "clt <var>"                    , 1, false,
			OpcodeClassification.Comparison);
		public static readonly Opcode Clz              = new(0x4F, &Opcode.Functions.Clz,     "clz"                          , 0, true,
			OpcodeClassification.ModifiesFlag | OpcodeClassification.NoOperand);
		public static readonly Opcode Cnrb             = new(0xFC, &Opcode.Functions.Cnrb,    "cnrb"                         , 0, false,
			OpcodeClassification.Console | OpcodeClassification.NoOperand);
		public static readonly Opcode Cnrf             = new(0xFB, &Opcode.Functions.Cnrf,    "cnrf"                         , 0, false,
			OpcodeClassification.Console | OpcodeClassification.NoOperand);
		public static readonly Opcode Cnwh             = new(0xF9, &Opcode.Functions.Cnwh,    "cnwh"                         , 0, false,
			OpcodeClassification.Console | OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Cnww             = new(0xF8, &Opcode.Functions.Cnww,    "cnww"                         , 0, false,
			OpcodeClassification.Console | OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Conh             = new(0xF1, &Opcode.Functions.Conh,    "conh"                         , 0, false,
			OpcodeClassification.Console | OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Conr             = new(0xFA, &Opcode.Functions.Conr,    "conr"                         , 0, false,
			OpcodeClassification.Console | OpcodeClassification.NoOperand);
		public static readonly Opcode Cont             = new(0xF2, &Opcode.Functions.Cont,    "cont"                         , 0, false,
			OpcodeClassification.Console | OpcodeClassification.ModifiesRegister | OpcodeClassification.NoOperand);
		public static readonly Opcode Conw             = new(0xF0, &Opcode.Functions.Conw,    "conw"                         , 0, false,
			OpcodeClassification.Console | OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Cos              = new(0x93, &Opcode.Functions.Cos,     "cos"                          , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Cosh             = new(0xB3, &Opcode.Functions.Cosh,    "cosh"                         , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Cpcj             = new(0xE4, &Opcode.Functions.Cpcj,    "cpcj"                         , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Cpco             = new(0xE2, &Opcode.Functions.Cpco,    "cpco"                         , 0, true,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Cpi              = new(0xE8, &Opcode.Functions.Cpi,     "cpi"                          , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Cpnr             = new(0xE6, &Opcode.Functions.Cpnr,    "cpnr"                         , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Cpo              = new(0xE3, &Opcode.Functions.Cpo,     "cpo"                          , 0, true,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Cpr              = new(0xE7, &Opcode.Functions.Cpr,     "cpr"                          , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Cpro             = new(0xE1, &Opcode.Functions.Cpro,    "cpro"                         , 0, true,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Cprv             = new(0xE5, &Opcode.Functions.Cprv,    "cprv"                         , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Cpz              = new(0xE0, &Opcode.Functions.Cpz,     "cpz"                          , 0, true,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Csrv             = new(0xED, &Opcode.Functions.Csrv,    "csrv"                         , 0, false,
			OpcodeClassification.Console | OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Csrx             = new(0xF3, &Opcode.Functions.Csrx,    "csrx"                         , 0, false,
			OpcodeClassification.Console | OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Csry             = new(0xF4, &Opcode.Functions.Csry,    "csry"                         , 0, false,
			OpcodeClassification.Console | OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);

		public static readonly Opcode Dex              = new(0x81, &Opcode.Functions.Dex,     "dex"                          , 0, true,
			OpcodeClassification.ArithmeticNoAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Dey              = new(0x83, &Opcode.Functions.Dey,     "dey"                          , 0, true,
			OpcodeClassification.ArithmeticNoAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Div_obj          = new(0x33, &Opcode.Functions.Div,     "div <obj>"                    , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Div_var          = new(0x43, &Opcode.Functions.Div,     "div <var>"                    , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Dtad_obj         = new(0x00, &Opcode.Functions.Dtad,    "dtad <obj>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Dtad_var         = new(0x08, &Opcode.Functions.Dtad,    "dtad <var>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Dtah_obj         = new(0x01, &Opcode.Functions.Dtah,    "dtah <obj>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Dtah_var         = new(0x09, &Opcode.Functions.Dtah,    "dtah <var>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Dtai_obj         = new(0x02, &Opcode.Functions.Dtai,    "dtai <obj>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Dtai_var         = new(0x0A, &Opcode.Functions.Dtai,    "dtai <var>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Dtam_obj         = new(0x03, &Opcode.Functions.Dtam,    "dtam <obj>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Dtam_var         = new(0x0B, &Opcode.Functions.Dtam,    "dtam <var>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Dtao_obj         = new(0x04, &Opcode.Functions.Dtao,    "dtao <obj>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Dtao_var         = new(0x0C, &Opcode.Functions.Dtao,    "dtao <var>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Dtat_obj         = new(0x05, &Opcode.Functions.Dtat,    "dtat <obj>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Dtat_var         = new(0x0D, &Opcode.Functions.Dtat,    "dtat <var>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Dtas_obj         = new(0x06, &Opcode.Functions.Dtas,    "dtas <obj>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Dtas_var         = new(0x0E, &Opcode.Functions.Dtas,    "dtas <var>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Dtay_obj         = new(0x07, &Opcode.Functions.Dtay,    "dtay <obj>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Dtay_var         = new(0x0F, &Opcode.Functions.Dtay,    "dtay <var>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Dtd              = new(0x10, &Opcode.Functions.Dtd,     "dtd"                          , 0, false,
			OpcodeClassification.ValueAccess | OpcodeClassification.NoOperand | OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Dte              = new(0x22, &Opcode.Functions.Dte,     "dte"                          , 0, true,
			OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Dtfm             = new(0x20, &Opcode.Functions.Dtfm,    "dtfm"                         , 0, false,
			OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Dth              = new(0x11, &Opcode.Functions.Dth,     "dth"                          , 0, false,
			OpcodeClassification.ValueAccess | OpcodeClassification.NoOperand | OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Dti              = new(0x12, &Opcode.Functions.Dti,     "dti"                          , 0, false,
			OpcodeClassification.ValueAccess | OpcodeClassification.NoOperand | OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Dtm              = new(0x13, &Opcode.Functions.Dtm,     "dtm"                          , 0, false,
			OpcodeClassification.ValueAccess | OpcodeClassification.NoOperand | OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Dtn              = new(0x21, &Opcode.Functions.Dtn,     "dtn"                          , 0, true,
			OpcodeClassification.ObjectCreation | OpcodeClassification.NoOperand | OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Dto              = new(0x14, &Opcode.Functions.Dto,     "dto"                          , 0, false,
			OpcodeClassification.ValueAccess | OpcodeClassification.NoOperand | OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Dtt              = new(0x15, &Opcode.Functions.Dtt,     "dtt"                          , 0, false,
			OpcodeClassification.ValueAccess | OpcodeClassification.NoOperand | OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Dts              = new(0x16, &Opcode.Functions.Dts,     "dts"                          , 0, false,
			OpcodeClassification.ValueAccess | OpcodeClassification.NoOperand | OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Dty              = new(0x17, &Opcode.Functions.Dty,     "dty"                          , 0, false,
			OpcodeClassification.ValueAccess | OpcodeClassification.NoOperand | OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Dt               = new(0x5E, &Opcode.Functions.Ext,     "<extended opcode>"            , 0, false,
			OpcodeClassification.ExtendedOpcode,
			Dtad_obj, Dtad_var,
			Dtah_obj, Dtah_var,
			Dtai_obj, Dtai_var,
			Dtam_obj, Dtam_var,
			Dtao_obj, Dtao_var,
			Dtat_obj, Dtat_var,
			Dtas_obj, Dtas_var,
			Dtay_obj, Dtay_var,
			Dtd,
			Dte,
			Dtfm,
			Dth,
			Dti,
			Dtm,
			Dtn,
			Dto,
			Dtt,
			Dts,
			Dty);
		public static readonly Opcode Dup              = new(0x4D, &Opcode.Functions.Dup,     "dup"                          , 0, true,
			OpcodeClassification.ModifiesStack | OpcodeClassification.NoOperand);
		public static readonly Opcode Dupd             = new(0x5D, &Opcode.Functions.Dupd,    "dupd"                         , 0, true,
			OpcodeClassification.ModifiesStack | OpcodeClassification.NoOperand);

		public static readonly Opcode Err              = new(0x61, &Opcode.Functions.Err,     "err"                          , 0, true,
			OpcodeClassification.ExceptionHandling | OpcodeClassification.NoOperand);
		public static readonly Opcode Err_obj          = new(0x71, &Opcode.Functions.Err,     "err <obj>"                    , 1, true,
			OpcodeClassification.ExceptionHandling);
		public static readonly Opcode Exp              = new(0x78, &Opcode.Functions.Exp,     "exp"                          , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);

		public static readonly Opcode Flor             = new(0x91, &Opcode.Functions.Flor,    "flor"                         , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);

		public static readonly Opcode Halt             = new(0xFF, &Opcode.Functions.Halt,    "halt"                         , 0, true,
			OpcodeClassification.NoSpecification | OpcodeClassification.NoOperand);

		public static readonly Opcode Idx_obj          = new(0x75, &Opcode.Functions.Idx,     "idx <obj>"                    , 1, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Idx_var          = new(0x85, &Opcode.Functions.Idx,     "idx <var>"                    , 1, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Idxv             = new(0x87, &Opcode.Functions.Idxv,    "idxv"                         , 0, false,
			OpcodeClassification.ValueAccess | OpcodeClassification.NoOperand | OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Inc              = new(0x48, &Opcode.Functions.Inc,     "inc"                          , 0, false,
			OpcodeClassification.Console | OpcodeClassification.NoOperand | OpcodeClassification.ModifiesRegister | OpcodeClassification.InputOutput);
		public static readonly Opcode Incb             = new(0x49, &Opcode.Functions.Incb,    "incb"                         , 0, false,
			OpcodeClassification.Console | OpcodeClassification.NoOperand | OpcodeClassification.ModifiesRegister | OpcodeClassification.InputOutput);
		public static readonly Opcode Inl              = new(0x47, &Opcode.Functions.Inl,     "inl"                          , 0, false,
			OpcodeClassification.Console | OpcodeClassification.NoOperand | OpcodeClassification.ModifiesRegister | OpcodeClassification.InputOutput);
		public static readonly Opcode Intp             = new(0x57, &Opcode.Functions.Intp,    "intp"                         , 0, false,
			OpcodeClassification.ModifiesRegister | OpcodeClassification.NoOperand);
		public static readonly Opcode Inv              = new(0x5A, &Opcode.Functions.Inv,     "inv"                          , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Inx              = new(0x80, &Opcode.Functions.Inx,     "inx"                          , 0, true,
			OpcodeClassification.ArithmeticNoAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Iny              = new(0x82, &Opcode.Functions.Iny,     "iny"                          , 0, true,
			OpcodeClassification.ArithmeticNoAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Is               = new(0x60, &Opcode.Functions.Is,      "is <type>"                    , 1, false,
			OpcodeClassification.ModifiesFlag);
		public static readonly Opcode Isa              = new(0x62, &Opcode.Functions.Isa,     "isa <type>"                   , 1, false,
			OpcodeClassification.ModifiesFlag);

		public static readonly Opcode Lda              = new(0x7E, &Opcode.Functions.Lda,     "lda <obj>"                    , 1, false,
			OpcodeClassification.ModifiesEvaluationStack);
		public static readonly Opcode Ldrg             = new(0x5B, &Opcode.Functions.Ldrg,    "ldrg"                         , 0, true,
			OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Len              = new(0x22, &Opcode.Functions.Len,     "len"                          , 0, false,
			OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Lens             = new(0x73, &Opcode.Functions.Lens,    "lens"                         , 0, false,
			OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Ln               = new(0x36, &Opcode.Functions.Ln,      "ln"                           , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Log              = new(0x37, &Opcode.Functions.Log,     "log"                          , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Lscp             = new(0x6E, &Opcode.Functions.Lscp,    "lscp"                         , 0, false,
			OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Lsct             = new(0x6D, &Opcode.Functions.Lsct,    "lsct"                         , 0, false,
			OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);

		public static readonly Opcode Mov_var_obj      = new(0x46, &Opcode.Functions.Mov,     "mov <var>, <obj>"             , 2, false,
			OpcodeClassification.ValueTransfer);
		public static readonly Opcode Mov_var_var      = new(0x56, &Opcode.Functions.Mov,     "mov <var>, <var2>"            , 2, false,
			OpcodeClassification.ValueTransfer);
		public static readonly Opcode Mov_arrX_obj     = new(0x66, &Opcode.Functions.Mov,     "mov [<arr>, &X], <obj>"       , 2, false,
			OpcodeClassification.ValueTransfer | OpcodeClassification.ValueAccess);
		public static readonly Opcode Mov_arrY_obj     = new(0x76, &Opcode.Functions.Mov,     "mov [<arr>, &Y], <obj>"       , 2, false,
			OpcodeClassification.ValueTransfer | OpcodeClassification.ValueAccess);
		public static readonly Opcode Mov_var_arrX     = new(0x86, &Opcode.Functions.Mov,     "mov <var>, [<arr>, &X]"       , 2, false,
			OpcodeClassification.ValueTransfer | OpcodeClassification.ValueAccess);
		public static readonly Opcode Mov_var_arrY     = new(0x96, &Opcode.Functions.Mov,     "mov <var>, [<arr>, &Y]"       , 2, false,
			OpcodeClassification.ValueTransfer | OpcodeClassification.ValueAccess);
		public static readonly Opcode Mov_arrX_var     = new(0xA6, &Opcode.Functions.Mov,     "mov [<arr>, &X], <var>"       , 2, false,
			OpcodeClassification.ValueTransfer | OpcodeClassification.ValueAccess);
		public static readonly Opcode Mov_arrY_var     = new(0xB6, &Opcode.Functions.Mov,     "mov [<arr>, &Y], <var>"       , 2, false,
			OpcodeClassification.ValueTransfer | OpcodeClassification.ValueAccess);
		public static readonly Opcode Mov_arrX_arrY    = new(0xC6, &Opcode.Functions.Mov,     "mov [<arr>, &X], [<arr2>, &Y]", 2, false,
			OpcodeClassification.ValueTransfer | OpcodeClassification.ValueAccess);
		public static readonly Opcode Mov_arrY_arrX    = new(0xD6, &Opcode.Functions.Mov,     "mov [<arr>, &Y], [<arr2>, &X]", 2, false,
			OpcodeClassification.ValueTransfer | OpcodeClassification.ValueAccess);
		public static readonly Opcode Mov_arrX_arrX    = new(0xC5, &Opcode.Functions.Mov,     "mov [<arr>, &X], [<arr2>, &X]", 2, false,
			OpcodeClassification.ValueTransfer | OpcodeClassification.ValueAccess);
		public static readonly Opcode Mov_arrY_arrY    = new(0xD5, &Opcode.Functions.Mov,     "mov [<arr>, &Y], [<arr2>, &Y]", 2, false,
			OpcodeClassification.ValueTransfer | OpcodeClassification.ValueAccess);
		public static readonly Opcode Mul_obj          = new(0x32, &Opcode.Functions.Mul,     "mul <obj>"                    , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Mul_var          = new(0x42, &Opcode.Functions.Mul,     "mul <var>"                    , 1, false,
			OpcodeClassification.Arithmetic);

		public static readonly Opcode Neg              = new(0x3A, &Opcode.Functions.Neg,     "neg"                          , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode New_indexer      = new(0x00, &Opcode.Functions.New,     "new ^u32"                     , 0, false,
			OpcodeClassification.ObjectCreation);
		public static readonly Opcode New_array        = new(0x10, &Opcode.Functions.New,     "new ~arr:<type>"              , 0, false,
			OpcodeClassification.ObjectCreation);
		public static readonly Opcode New_date         = new(0x60, &Opcode.Functions.New,     "new ~date"                    , 0, false,
			OpcodeClassification.ObjectCreation);
		public static readonly Opcode New_date_obj     = new(0x61, &Opcode.Functions.New,     "new ~date, <obj>"             , 1, false,
			OpcodeClassification.ObjectCreation);
		public static readonly Opcode New_date_var     = new(0x62, &Opcode.Functions.New,     "new ~date, <var>"             , 1, false,
			OpcodeClassification.ObjectCreation);
		public static readonly Opcode New_list         = new(0x30, &Opcode.Functions.New,     "new ~list, &Y"                , 1, false,
			OpcodeClassification.ObjectCreation);
		public static readonly Opcode New_rand         = new(0x80, &Opcode.Functions.New,     "new ~rand"                    , 0, false,
			OpcodeClassification.ObjectCreation);
		public static readonly Opcode New_rand_obj     = new(0x81, &Opcode.Functions.New,     "new ~rand, <obj>"             , 1, false,
			OpcodeClassification.ObjectCreation);
		public static readonly Opcode New_rand_var     = new(0x82, &Opcode.Functions.New,     "new ~rand, <var>"             , 1, false,
			OpcodeClassification.ObjectCreation);
		public static readonly Opcode New_range        = new(0x20, &Opcode.Functions.New,     "new ~range"                   , 0, false,
			OpcodeClassification.ObjectCreation);
		public static readonly Opcode New_regex        = new(0x70, &Opcode.Functions.New,     "new ~regex"                   , 0, false,
			OpcodeClassification.ObjectCreation);
		public static readonly Opcode New_set          = new(0x50, &Opcode.Functions.New,     "new ~set"                     , 0, false,
			OpcodeClassification.ObjectCreation);
		public static readonly Opcode New_set_obj      = new(0x51, &Opcode.Functions.New,     "new ~set, <obj>"              , 1, false,
			OpcodeClassification.ObjectCreation);
		public static readonly Opcode New_set_var      = new(0x52, &Opcode.Functions.New,     "new ~set, <var>"              , 1, false,
			OpcodeClassification.ObjectCreation);
		public static readonly Opcode New_time         = new(0x40, &Opcode.Functions.New,     "new ~time"                    , 0, false,
			OpcodeClassification.ObjectCreation);
		public static readonly Opcode New_time_obj     = new(0x41, &Opcode.Functions.New,     "new ~time, <obj>"             , 1, false,
			OpcodeClassification.ObjectCreation);
		public static readonly Opcode New_time_var     = new(0x42, &Opcode.Functions.New,     "new ~time, <var>"             , 1, false,
			OpcodeClassification.ObjectCreation);
		public static readonly Opcode New_userdef      = new(0x90, &Opcode.Functions.New,     "new ~ud:<file::type>-><func>" , 0, false,
			OpcodeClassification.ObjectCreation);
		public static readonly Opcode New              = new(0x2E, &Opcode.Functions.Ext,     "<extended opcode>"            , 0, false,
			OpcodeClassification.ExtendedOpcode,
			New_indexer,
			New_array,
			New_date, New_date_obj, New_date_var,
			New_list,
			New_rand, New_rand_obj, New_rand_var,
			New_range,
			New_regex,
			New_set, New_set_obj, New_set_var,
			New_time, New_time_obj, New_time_var,
			New_userdef);
		public static readonly Opcode Not              = new(0x02, &Opcode.Functions.Not,     "not"                          , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);

		public static readonly Opcode Or_obj           = new(0x01, &Opcode.Functions.Or,      "or <obj>"                     , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Or_var           = new(0x21, &Opcode.Functions.Or,      "or <var>"                     , 1, false,
			OpcodeClassification.Arithmetic);

		public static readonly Opcode Pntl             = new(0x4C, &Opcode.Functions.Pntl,    "pntl"                         , 0, true,
			OpcodeClassification.InputOutput);
		public static readonly Opcode Poa              = new(0x2A, &Opcode.Functions.Poa,     "poa"                          , 0, true,
			OpcodeClassification.ModifiesAccumulator | OpcodeClassification.ModifiesStack);
		public static readonly Opcode Poed             = new(0x98, &Opcode.Functions.Poed,    "poed"                         , 0, false,
			OpcodeClassification.ModifiesEvaluationStack);
		public static readonly Opcode Poev             = new(0x98, &Opcode.Functions.Poev,    "poev <type>"                  , 1, false,
			OpcodeClassification.ModifiesAccumulator | OpcodeClassification.ModifiesEvaluationStack);
		public static readonly Opcode Pop              = new(0x1C, &Opcode.Functions.Pop,     "pop"                          , 0, false,
			OpcodeClassification.ModifiesStack | OpcodeClassification.ModifiesFlag);
		public static readonly Opcode Pop_var          = new(0x3C, &Opcode.Functions.Pop,     "pop <var>"                    , 1, false,
			OpcodeClassification.ModifiesStack | OpcodeClassification.ModifiesFlag);
		public static readonly Opcode Pos              = new(0x3E, &Opcode.Functions.Pos,     "pos"                          , 0, true,
			OpcodeClassification.ModifiesRegister | OpcodeClassification.ModifiesStack);
		public static readonly Opcode Pow_obj          = new(0x34, &Opcode.Functions.Pow,     "pow <obj>"                    , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Pow_var          = new(0x44, &Opcode.Functions.Pow,     "pow <var>"                    , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Pox              = new(0x2B, &Opcode.Functions.Pox,     "pox"                          , 0, true,
			OpcodeClassification.ModifiesRegister | OpcodeClassification.ModifiesStack);
		public static readonly Opcode Poy              = new(0x2C, &Opcode.Functions.Poy,     "poy"                          , 0, true,
			OpcodeClassification.ModifiesRegister | OpcodeClassification.ModifiesStack);
		public static readonly Opcode Prnt             = new(0x4A, &Opcode.Functions.Prnt,    "prnt"                         , 0, true,
			OpcodeClassification.InputOutput);
		public static readonly Opcode Prse             = new(0x64, &Opcode.Functions.Prse,    "prse"                         , 0, false,
			OpcodeClassification.ModifiesRegister);
		public static readonly Opcode Psa              = new(0x0A, &Opcode.Functions.Psa,     "psa"                          , 0, true,
			OpcodeClassification.ModifiesStack | OpcodeClassification.ModifiesFlag);
		public static readonly Opcode Psev             = new(0x88, &Opcode.Functions.Psev,    "psev <type>"                  , 1, false,
			OpcodeClassification.ModifiesEvaluationStack);
		public static readonly Opcode Psh_obj          = new(0x3B, &Opcode.Functions.Psh,     "psh <obj>"                    , 1, true,
			OpcodeClassification.ModifiesStack | OpcodeClassification.ModifiesFlag);
		public static readonly Opcode Psh_var          = new(0x4B, &Opcode.Functions.Psh,     "psh <var>"                    , 1, true,
			OpcodeClassification.ModifiesStack | OpcodeClassification.ModifiesFlag);
		public static readonly Opcode Pss              = new(0x3D, &Opcode.Functions.Pss,     "pss"                          , 0, true,
			OpcodeClassification.ModifiesStack | OpcodeClassification.ModifiesFlag);
		public static readonly Opcode Psx              = new(0x0B, &Opcode.Functions.Psx,     "psx"                          , 0, true,
			OpcodeClassification.ModifiesStack | OpcodeClassification.ModifiesFlag);
		public static readonly Opcode Psy              = new(0x0C, &Opcode.Functions.Psy,     "psy"                          , 0, true,
			OpcodeClassification.ModifiesStack | OpcodeClassification.ModifiesFlag);

		public static readonly Opcode Rem_obj          = new(0x5C, &Opcode.Functions.Rem,     "rem <obj>"                    , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Rem_var          = new(0x6C, &Opcode.Functions.Rem,     "rem <var>"                    , 1, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Ret              = new(0x04, &Opcode.Functions.Ret,     "ret"                          , 0, false,
			OpcodeClassification.NoSpecification | OpcodeClassification.NoOperand);
		public static readonly Opcode Rge              = new(0x8A, &Opcode.Functions.Rge,     "rge"                          , 0, false,
			OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Rge_obj          = new(0x9A, &Opcode.Functions.Rge,     "rge <obj>"                    , 1, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rge_var          = new(0xAA, &Opcode.Functions.Rge,     "rge <var>"                    , 1, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rgs              = new(0x89, &Opcode.Functions.Rgs,     "rgs"                          , 0, false,
			OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Rgs_obj          = new(0x99, &Opcode.Functions.Rgs,     "rgs <obj>"                    , 1, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rgs_var          = new(0xA9, &Opcode.Functions.Rgs,     "rgs <var>"                    , 1, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rgxf_obj         = new(0x97, &Opcode.Functions.Rgxf,    "rgxf <obj>"                   , 1, false,
			OpcodeClassification.ModifiesRegister);
		public static readonly Opcode Rgxf_var         = new(0xA7, &Opcode.Functions.Rgxf,    "rgxf <var>"                   , 1, false,
			OpcodeClassification.ModifiesRegister);
		public static readonly Opcode Rgxm_obj         = new(0xB1, &Opcode.Functions.Rgxm,    "rgxm <obj>"                   , 1, false,
			OpcodeClassification.ModifiesRegister);
		public static readonly Opcode Rgxm_var         = new(0xA8, &Opcode.Functions.Rgxm,    "rgxm <var>"                   , 1, false,
			OpcodeClassification.ModifiesRegister);
		public static readonly Opcode Rgxs             = new(0xB0, &Opcode.Functions.Rgxs,    "rgxf"                         , 0, false,
			OpcodeClassification.ModifiesRegister);
		public static readonly Opcode Rndb             = new(0x40, &Opcode.Functions.Rndb,    "rndb <arr>"                   , 1, false,
			OpcodeClassification.NoSpecification | OpcodeClassification.NoOperand);
		public static readonly Opcode Rndd             = new(0x20, &Opcode.Functions.Rndd,    "rndd"                         , 0, false,
			OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Rndd_obj         = new(0x21, &Opcode.Functions.Rndd,    "rndd <obj>"                   , 1, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rndd_var         = new(0x22, &Opcode.Functions.Rndd,    "rndd <var>"                   , 1, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rndd_obj_obj     = new(0x23, &Opcode.Functions.Rndd,    "rndd <obj>, <obj2>"           , 2, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rndd_obj_var     = new(0x24, &Opcode.Functions.Rndd,    "rndd <obj>, <var>"            , 2, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rndd_var_obj     = new(0x25, &Opcode.Functions.Rndd,    "rndd <var>, <obj>"            , 2, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rndd_var_var     = new(0x26, &Opcode.Functions.Rndd,    "rndd <var>, <var2>"           , 2, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rndf             = new(0x10, &Opcode.Functions.Rndf,    "rndf"                         , 0, false,
			OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Rndf_obj         = new(0x11, &Opcode.Functions.Rndf,    "rndf <obj>"                   , 1, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rndf_var         = new(0x12, &Opcode.Functions.Rndf,    "rndf <var>"                   , 1, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rndf_obj_obj     = new(0x13, &Opcode.Functions.Rndf,    "rndf <obj>, <obj2>"           , 2, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rndf_obj_var     = new(0x14, &Opcode.Functions.Rndf,    "rndf <obj>, <var>"            , 2, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rndf_var_obj     = new(0x15, &Opcode.Functions.Rndf,    "rndf <var>, <obj>"            , 2, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rndf_var_var     = new(0x16, &Opcode.Functions.Rndf,    "rndf <var>, <var2>"           , 2, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rndi             = new(0x00, &Opcode.Functions.Rndi,    "rndi"                         , 0, false,
			OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Rndi_obj         = new(0x01, &Opcode.Functions.Rndi,    "rndi <obj>"                   , 1, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rndi_var         = new(0x02, &Opcode.Functions.Rndi,    "rndi <var>"                   , 1, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rndi_obj_obj     = new(0x03, &Opcode.Functions.Rndi,    "rndi <obj>, <obj2>"           , 2, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rndi_obj_var     = new(0x04, &Opcode.Functions.Rndi,    "rndi <obj>, <var>"            , 2, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rndi_var_obj     = new(0x05, &Opcode.Functions.Rndi,    "rndi <var>, <obj>"            , 2, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rndi_var_var     = new(0x06, &Opcode.Functions.Rndi,    "rndi <var>, <var2>"           , 2, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rndl             = new(0x30, &Opcode.Functions.Rndl,    "rndl"                         , 0, false,
			OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Rndl_obj         = new(0x31, &Opcode.Functions.Rndl,    "rndl <obj>"                   , 1, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rndl_var         = new(0x32, &Opcode.Functions.Rndl,    "rndl <var>"                   , 1, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rndl_obj_obj     = new(0x33, &Opcode.Functions.Rndl,    "rndl <obj>, <obj2>"           , 2, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rndl_obj_var     = new(0x34, &Opcode.Functions.Rndl,    "rndl <obj>, <var>"            , 2, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rndl_var_obj     = new(0x35, &Opcode.Functions.Rndl,    "rndl <var>, <obj>"            , 2, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rndl_var_var     = new(0x36, &Opcode.Functions.Rndl,    "rndl <var>, <var2>"           , 2, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Rnd              = new(0x4E, &Opcode.Functions.Ext,     "<extended opcode>"            , 0, false,
			OpcodeClassification.ExtendedOpcode,
			Rndb,
			Rndd, Rndd_obj, Rndd_var, Rndd_obj_obj, Rndd_obj_var, Rndd_var_obj, Rndd_var_var,
			Rndf, Rndf_obj, Rndf_var, Rndf_obj_obj, Rndf_obj_var, Rndf_var_obj, Rndf_var_var,
			Rndi, Rndi_obj, Rndi_var, Rndi_obj_obj, Rndi_obj_var, Rndi_var_obj, Rndi_var_var,
			Rndl, Rndl_obj, Rndl_var, Rndl_obj_obj, Rndl_obj_var, Rndl_var_obj, Rndl_var_var);
		public static readonly Opcode Rol              = new(0x1D, &Opcode.Functions.Rol,     "rol"                          , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Ror              = new(0x1E, &Opcode.Functions.Ror,     "ror"                          , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);

		public static readonly Opcode Sbs              = new(0x70, &Opcode.Functions.Sbs,     "sbs"                          , 0, false,
			OpcodeClassification.ModifiesRegister | OpcodeClassification.NoOperand);
		public static readonly Opcode Sdiv_obj         = new(0xC1, &Opcode.Functions.Sdiv,    "sdiv <obj>"                   , 1, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Sdiv_var         = new(0xD1, &Opcode.Functions.Sdiv,    "sdiv <var>"                   , 1, false,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Shas_obj         = new(0xAB, &Opcode.Functions.Shas,    "shas <obj>"                   , 1, false,
			OpcodeClassification.ModifiesFlag);
		public static readonly Opcode Shas_var         = new(0xBB, &Opcode.Functions.Shas,    "shas <var>"                   , 1, false,
			OpcodeClassification.ModifiesFlag);
		public static readonly Opcode Sin              = new(0x92, &Opcode.Functions.Sin,     "sin"                          , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Sinh             = new(0xB2, &Opcode.Functions.Sinh,    "sinh"                         , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Size             = new(0x6B, &Opcode.Functions.Size,    "size"                         , 0, false,
			OpcodeClassification.ModifiesAccumulator | OpcodeClassification.NoOperand);
		public static readonly Opcode Sjn_obj          = new(0xC0, &Opcode.Functions.Sjn,     "sjn <obj>"                    , 1, false,
			OpcodeClassification.ModifiesRegister);
		public static readonly Opcode Sjn_var          = new(0xD0, &Opcode.Functions.Sjn,     "sjn <var>"                    , 1, false,
			OpcodeClassification.ModifiesRegister);
		public static readonly Opcode Sqrt             = new(0x77, &Opcode.Functions.Sqrt,    "sqrt"                         , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Srep             = new(0xC3, &Opcode.Functions.Srep,    "srep"                         , 0, false,
			OpcodeClassification.ModifiesRegister | OpcodeClassification.NoOperand);
		public static readonly Opcode Srmv_obj         = new(0xC2, &Opcode.Functions.Srmv,    "srmv <obj>"                   , 1, false,
			OpcodeClassification.ModifiesRegister);
		public static readonly Opcode Srmv_var         = new(0xD2, &Opcode.Functions.Srmv,    "srmv <var>"                   , 1, false,
			OpcodeClassification.ModifiesRegister);
		public static readonly Opcode Stc              = new(0x5F, &Opcode.Functions.Stc,     "stc"                          , 0, true,
			OpcodeClassification.ModifiesFlag);
		public static readonly Opcode Stco_obj         = new(0x8E, &Opcode.Functions.Stco,    "stco <obj>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Stco_var         = new(0x9E, &Opcode.Functions.Stco,    "stco <var>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Stdf_obj         = new(0x8F, &Opcode.Functions.Stdf,    "stdf <obj>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Stdf_var         = new(0x9F, &Opcode.Functions.Stdf,    "stdf <var>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Stdj_obj         = new(0x8D, &Opcode.Functions.Stdj,    "stdj <obj>"                   , 1, false,
			OpcodeClassification.ModifiesFlag);
		public static readonly Opcode Stdj_var         = new(0x9D, &Opcode.Functions.Stdj,    "stdj <var>"                   , 1, false,
			OpcodeClassification.ModifiesFlag);
		public static readonly Opcode Stin_obj         = new(0x8C, &Opcode.Functions.Stin,    "stin <obj>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Stin_var         = new(0x9C, &Opcode.Functions.Stin,    "stin <var>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Stp              = new(0x7F, &Opcode.Functions.Stp,     "stp"                          , 0, true,
			OpcodeClassification.ModifiesFlag);
		public static readonly Opcode Stun_obj         = new(0x8B, &Opcode.Functions.Stun,    "stun <obj>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Stun_var         = new(0x9B, &Opcode.Functions.Stun,    "stun <var>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Sub_obj          = new(0x31, &Opcode.Functions.Sub,     "sub <obj>"                    , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Sub_var          = new(0x41, &Opcode.Functions.Sub,     "sub <var>"                    , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Swap             = new(0x2D, &Opcode.Functions.Swap,    "swap"                         , 0, true,
			OpcodeClassification.ModifiesStack);
		public static readonly Opcode Sys_obj          = new(0xDF, &Opcode.Functions.Sys,     "sys <obj>"                    , 1, false,
			OpcodeClassification.NoSpecification);
		public static readonly Opcode Sys_var          = new(0xEF, &Opcode.Functions.Sys,     "sys <var>"                    , 1, false,
			OpcodeClassification.NoSpecification);

		public static readonly Opcode Tan              = new(0x94, &Opcode.Functions.Tan,     "tan"                          , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Tanh             = new(0xB4, &Opcode.Functions.Tanh,    "tanh"                         , 0, false,
			OpcodeClassification.Arithmetic | OpcodeClassification.NoOperand);
		public static readonly Opcode Tas              = new(0x58, &Opcode.Functions.Tas,     "tas"                          , 0, true,
			OpcodeClassification.ModifiesRegister);
		public static readonly Opcode Tax              = new(0x51, &Opcode.Functions.Tax,     "tax"                          , 0, true,
			OpcodeClassification.ModifiesRegister);
		public static readonly Opcode Tay              = new(0x52, &Opcode.Functions.Tay,     "tay"                          , 0, true,
			OpcodeClassification.ModifiesRegister);
		public static readonly Opcode Tmad_obj         = new(0x00, &Opcode.Functions.Tmad,    "tmad <obj>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Tmad_var         = new(0x01, &Opcode.Functions.Tmad,    "tmad <var>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Tmah_obj         = new(0x02, &Opcode.Functions.Tmah,    "tmah <obj>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Tmah_var         = new(0x03, &Opcode.Functions.Tmah,    "tmah <var>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Tmai_obj         = new(0x04, &Opcode.Functions.Tmai,    "tmai <obj>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Tmai_var         = new(0x05, &Opcode.Functions.Tmai,    "tmai <var>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Tmam_obj         = new(0x06, &Opcode.Functions.Tmam,    "tmam <obj>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Tmam_var         = new(0x07, &Opcode.Functions.Tmam,    "tmam <var>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Tmas_obj         = new(0x0A, &Opcode.Functions.Tmas,    "tmas <obj>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Tmas_var         = new(0x0B, &Opcode.Functions.Tmas,    "tmas <var>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Tmat_obj         = new(0x08, &Opcode.Functions.Tmat,    "tmat <obj>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Tmat_var         = new(0x09, &Opcode.Functions.Tmat,    "tmat <var>"                   , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Tmcd             = new(0x10, &Opcode.Functions.Tmcd,    "tmcd"                         , 0, false,
			OpcodeClassification.ValueAccess);
		public static readonly Opcode Tmch             = new(0x11, &Opcode.Functions.Tmch,    "tmch"                         , 0, false,
			OpcodeClassification.ValueAccess);
		public static readonly Opcode Tmci             = new(0x12, &Opcode.Functions.Tmci,    "tmci"                         , 0, false,
			OpcodeClassification.ValueAccess);
		public static readonly Opcode Tmcm             = new(0x13, &Opcode.Functions.Tmcm,    "tmcm"                         , 0, false,
			OpcodeClassification.ValueAccess);
		public static readonly Opcode Tmcs             = new(0x15, &Opcode.Functions.Tmcs,    "tmcs"                         , 0, false,
			OpcodeClassification.ValueAccess);
		public static readonly Opcode Tmfm             = new(0x20, &Opcode.Functions.Tmfm,    "tmfm"                         , 0, false,
			OpcodeClassification.ModifiesRegister);
		public static readonly Opcode Tmt              = new(0x14, &Opcode.Functions.Tmt,     "tmt"                          , 0, false,
			OpcodeClassification.ValueAccess);
		public static readonly Opcode Tmtd             = new(0x16, &Opcode.Functions.Tmtd,    "tmtd"                         , 0, false,
			OpcodeClassification.ValueAccess);
		public static readonly Opcode Tmth             = new(0x17, &Opcode.Functions.Tmth,    "tmth"                         , 0, false,
			OpcodeClassification.ValueAccess);
		public static readonly Opcode Tmti             = new(0x18, &Opcode.Functions.Tmti,    "tmti"                         , 0, false,
			OpcodeClassification.ValueAccess);
		public static readonly Opcode Tmtm             = new(0x19, &Opcode.Functions.Tmtm,    "tmtm"                         , 0, false,
			OpcodeClassification.ValueAccess);
		public static readonly Opcode Tmts             = new(0x1A, &Opcode.Functions.Tmts,    "tmts"                         , 0, false,
			OpcodeClassification.ValueAccess);
		public static readonly Opcode Tm               = new(0x24, &Opcode.Functions.Ext,     "<extended opcode"             , 0, false,
			OpcodeClassification.ExtendedOpcode,
			Tmad_obj, Tmad_var,
			Tmah_obj, Tmah_var,
			Tmai_obj, Tmai_var,
			Tmam_obj, Tmam_var,
			Tmas_obj, Tmas_var,
			Tmat_obj, Tmat_var,
			Tmcd,
			Tmch,
			Tmci,
			Tmcm,
			Tmcs,
			Tmfm,
			Tmt,
			Tmtd,
			Tmth,
			Tmti,
			Tmtm,
			Tmts);
		public static readonly Opcode Tryc             = new(0x90, &Opcode.Functions.Tryc,    "tryc <label>"                 , 1, false,
			OpcodeClassification.ExceptionHandling);
		public static readonly Opcode Tryf             = new(0xA0, &Opcode.Functions.Tryf,    "tryf <label>, <label2>"       , 2, false,
			OpcodeClassification.ExceptionHandling);
		public static readonly Opcode Tryn             = new(0xD3, &Opcode.Functions.Tryn,    "tryn <label>"                 , 1, false,
			OpcodeClassification.ExceptionHandling);
		public static readonly Opcode Tsa              = new(0x59, &Opcode.Functions.Tsa,     "tsa"                          , 0, true,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Txa              = new(0x53, &Opcode.Functions.Txa,     "txa"                          , 0, true,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Txy              = new(0x54, &Opcode.Functions.Txy,     "txy"                          , 0, true,
			OpcodeClassification.ModifiesRegister);
		public static readonly Opcode Tya              = new(0x55, &Opcode.Functions.Tya,     "tya"                          , 0, true,
			OpcodeClassification.ModifiesAccumulator);
		public static readonly Opcode Type             = new(0x72, &Opcode.Functions.Type_fn, "type"                         , 0, true,
			OpcodeClassification.ModifiesRegister);
		public static readonly Opcode Tyx              = new(0x56, &Opcode.Functions.Tyx,     "tyx"                          , 0, true,
			OpcodeClassification.ModifiesRegister);

		public static readonly Opcode Wait_obj         = new(0xEE, &Opcode.Functions.Wait,    "wait <obj>"                   , 1, false,
			OpcodeClassification.NoSpecification);
		public static readonly Opcode Wait_var         = new(0xFE, &Opcode.Functions.Wait,    "wait <var>"                   , 1, false,
			OpcodeClassification.NoSpecification);

		public static readonly Opcode Xor_obj          = new(0x03, &Opcode.Functions.Xor,     "xor <obj>"                    , 1, false,
			OpcodeClassification.Arithmetic);
		public static readonly Opcode Xor_var          = new(0x23, &Opcode.Functions.Xor,     "xor <var>"                    , 1, false,
			OpcodeClassification.Arithmetic);
	}
}
