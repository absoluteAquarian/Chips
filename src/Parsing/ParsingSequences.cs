using Sprache;
using System.Text;

namespace Chips.Parsing{
	internal static class ParsingSequences{
		private static readonly Parser<char> OneQuote = Parse.Char('"');

		private static readonly Parser<string> ManyWhitespace = Parse.WhiteSpace.Many().Text();
		
		public static readonly Parser<string> VariableString = Parse.Identifier(OneQuote, Parse.CharExcept('"'));

		public static readonly Parser<string> IdentifierString =
			from nameStart in Parse.Letter.XOr(Parse.Char('_'))
			from name in Parse.LetterOrDigit.XOr(Parse.Char('_')).Many().Text()
			select nameStart + name;

		public static readonly Parser<string> VariableInteger =
			from num in Parse.Digit.Many().Text()
			select num;

		public static readonly Parser<string> IntegerWithSign =
			from sign in Parse.Chars('+', '-').Optional()
			from num in VariableInteger.Text()
			select CombineStringWithOptions(num, strFirst: false, sign);

		public static readonly Parser<string> IntegerWithType =
			from signed in IntegerWithSign.Text()
			from u in Parse.Chars('u', 'U').Optional()
			from l in Parse.Chars('l', 'L').Optional()
			select CombineStringWithOptions(signed, strFirst: true, u, l);

		public static readonly Parser<string> BinaryNumber =
			from token in Parse.Char('%')
			from msb in Parse.Chars('0', '1')
			from rest in Parse.Chars('0', '1', '_').Many().Text().Optional()
			select CombineStringWithOptions(token.ToString() + msb, strFirst: true, rest);

		private static readonly Parser<char> HexadecimalDigit =
			from nibble in Parse.Digit.Or(Parse.Chars('a', 'A', 'b', 'B', 'c', 'C', 'd', 'D', 'e', 'E', 'f', 'F'))
			select nibble;

		public static readonly Parser<string> HexadecimalNumber =
			from num in HexadecimalDigit.Many().Text()
			from token in Parse.Chars('h', 'H')
			select num + token;

		public static readonly Parser<string> NumberAnyInteger =
			from num in IntegerWithSign.XOr(IntegerWithType).XOr(BinaryNumber).XOr(HexadecimalNumber)
			select num;

		public static readonly Parser<string> NumberFloat =
			from num in Parse.DecimalInvariant
			from token in Parse.Chars('f', 'F')
			select num + token;

		public static readonly Parser<string> NumberDouble =
			from num in Parse.DecimalInvariant
			from token in Parse.Chars('d', 'D')
			select num + token;

		public static readonly Parser<string> NumberDecimal =
			from num in Parse.DecimalInvariant
			from token in Parse.Chars('m', 'M')
			select num + token;

		public static readonly Parser<string> NumberHalf =
			from num in Parse.DecimalInvariant
			from token in Parse.Chars('h', 'H')
			select num + token;

		public static readonly Parser<string> NumberAnyFloatingPoint =
			from num in NumberFloat.XOr(NumberDouble).XOr(NumberDecimal).XOr(NumberHalf)
			select num;

		public static readonly Parser<string> NumberComplexImaginary =
			from num in Parse.DecimalInvariant
			from i in Parse.Char('i')
			select num + i;

		public static readonly Parser<string> AssemblyName =
			from token in Parse.String(".asm_name")
			from space in ManyWhitespace
			from name in IdentifierString
			select name;

		public static readonly Parser<string> StackSize =
			from token in Parse.String(".stacksize")
			from space in ManyWhitespace
			from size in VariableInteger.Or(BinaryNumber).Or(HexadecimalNumber)
			select size;

		private static readonly Parser<string> VariableUserDefined =
			from ud in Parse.String("~ud:").Text()
			from type in IdentifierString
			select ud + type;

		private static readonly Parser<string> VariableArray =
			from arr in Parse.String("~arr:").Text()
			from type in VariableType
			select arr + type;

		private static readonly Parser<string> ChipsType =
			from prefix in Parse.Char('~').Optional()
			from name in IdentifierString
			select CombineStringWithOptions(name, strFirst: false, prefix);

		public static readonly Parser<string> VariableType =
			from name in VariableUserDefined.XOr(VariableArray).XOr(IdentifierString).XOr(ChipsType)
			select name;

		public static readonly Parser<string> FunctionArgument =
			from name in IdentifierString
			from whitespace in ManyWhitespace
			from separator in Parse.Char(':')
			from whitespace2 in ManyWhitespace
			from type in VariableType
			select name + whitespace + separator + whitespace2 + type;

		private static readonly Parser<string> FunctionArguments =
			from open in Parse.Char('(')
			from args in FunctionArgument.Once().Then(_ => ManyWhitespace.Then(_ => Parse.Char(',')).Then(_ => ManyWhitespace).Then(_ => FunctionArgument).Many().Optional()).Optional()
			from close in Parse.Char(')')
			select !args.IsDefined ? "()" : "(" + string.Join(", ", args.Get().Get()) + ")";

		private static readonly Parser<string> FunctionAccessiblity =
			from type in Parse.String("pub").XOr(Parse.String("asm")).XOr(Parse.String("hide")).Text()
			from ws in ManyWhitespace
			from loc in Parse.String("loc").Then(_ => ManyWhitespace).Text().Optional()
			from stat in Parse.String("stat").Text().Optional()
			select type + ws + (loc.IsDefined ? loc.Get() : "") + (stat.IsDefined ? stat.Get() : "");

		private static readonly Parser<string> LocalDefinition =
			from token in Parse.String(".local").Text()
			from ws in ManyWhitespace
			from name in IdentifierString
			from ws2 in ManyWhitespace
			from colon in Parse.Char(':')
			from ws3 in ManyWhitespace
			from type in VariableType
			select token + ws + name + ws2 + colon + ws3 + type;

		private static readonly Parser<string> OpcodeAndOperand =
			from code in IdentifierString.Where(s => Compiler.OpcodeIsDefined(s))
			from ws in ManyWhitespace.Optional()
			from operands in Parse.AnyChar.Until(Parse.LineTerminator).Text().Optional()
			select CombineStringWithOptions(code, strFirst: true, ws, operands);

		public static readonly Parser<string> GlobalFunction =
			from token in Parse.String(".func")
			from ws in ManyWhitespace
			from name in IdentifierString.Token()
			from ws2 in ManyWhitespace
			from args in FunctionArguments
			from ws3 in ManyWhitespace
			from colon in Parse.Char(':')
			from ws4 in ManyWhitespace
			from access in FunctionAccessiblity
			from ws5 in ManyWhitespace
			from nl in Parse.LineEnd
			from contents in LocalDefinition.Or(OpcodeAndOperand).Or(Parse.LineEnd).Until(Parse.String("end").Text()).Many()
			select token + ws + name + ws2 + args + ws3 + colon + ws4 + access + ws5 + nl
				+ string.Concat(contents.SelectMany(e => string.Join('\n', e)))
				+ "\nend";

		private static string CombineStringWithOptions(string str, bool strFirst, params IOption<char>[] options){
			StringBuilder sb = new();

			if(strFirst){
				sb.Append(str);

				foreach(var o in options)
					if(o.IsDefined)
						sb.Append(o.Get());
			}else{
				foreach(var o in options)
					if(o.IsDefined)
						sb.Append(o.Get());

				sb.Append(str);
			}

			return sb.ToString();
		}

		private static string CombineStringWithOptions(string str, bool strFirst, params IOption<string>[] options){
			StringBuilder sb = new();

			if(strFirst){
				sb.Append(str);

				foreach(var o in options)
					if(o.IsDefined)
						sb.Append(o.Get());
			}else{
				foreach(var o in options)
					if(o.IsDefined)
						sb.Append(o.Get());

				sb.Append(str);
			}

			return sb.ToString();
		}
	}
}
