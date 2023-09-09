using Sprache;
using System.Text;

namespace Chips.Parsing {
	internal static class ParsingSequences {
		private static readonly Parser<string> ManyWhitespace =
			Parse.WhiteSpace.Many().Text();

		/// <summary>
		/// Parses an inline string argument
		/// </summary>
		public static readonly Parser<string> VariableString =
			Parse.Identifier(Parse.Char('"'), Parse.CharExcept('"'));

		/// <summary>
		/// Parses a standard representation of an integer number (e.g. "25" and "-341uL")
		/// </summary>
		public static readonly Parser<string> Integer =
			from signed in IntegerWithSign.Text()
			from u in Parse.Chars('u', 'U').Optional()
			from l in Parse.Chars('l', 'L').Optional()
			select CombineStringWithOptions(signed, strFirst: true, u, l);

		private static readonly Parser<string> IntegerWithSign =
			from sign in Parse.Chars('+', '-').Optional()
			from num in Digits.Text()
			select CombineStringWithOptions(num, strFirst: false, sign);

		private static readonly Parser<string> Digits =
			from num in Parse.Digit.Many().Text()
			select num;

		/// <summary>
		/// Parses a binary representation of an integer number (e.g. "%1101" and "%1011_0001L")
		/// </summary>
		public static readonly Parser<string> BinaryNumber =
			from token in Parse.Char('%')
			from msb in Parse.Chars('0', '1')
			from rest in Parse.Chars('0', '1', '_').Many().Text().Optional()
			select CombineStringWithOptions(token.ToString() + msb, strFirst: true, rest);

		/// <summary>
		/// Parses a hexedecimal representation of an integer number (e.g. "45h" and "6AhL")
		/// </summary>
		public static readonly Parser<string> HexadecimalNumber =
			from num in HexadecimalDigit.Many().Text()
			from token in Parse.Chars('h', 'H')
			from u in Parse.Chars('u', 'U').Optional()
			from l in Parse.Chars('l', 'L').Optional()
			select CombineStringWithOptions(num + token, strFirst: true, u, l);

		private static readonly Parser<char> HexadecimalDigit =
			from nibble in Parse.Digit.Or(Parse.Chars('a', 'A', 'b', 'B', 'c', 'C', 'd', 'D', 'e', 'E', 'f', 'F'))
			select nibble;

		/// <summary>
		/// Parses any representation of an integer number
		/// </summary>
		public static readonly Parser<string> NumberAnyInteger =
			from num in IntegerWithSign.Or(Integer).Or(BinaryNumber).Or(HexadecimalNumber)
			select num;

		/// <summary>
		/// Parses a <see cref="float"/> number
		/// </summary>
		public static readonly Parser<string> NumberFloat =
			from num in Parse.DecimalInvariant
			from token in Parse.Chars('f', 'F')
			select num + token;

		/// <summary>
		/// Parses a <see cref="double"/> number
		/// </summary>
		public static readonly Parser<string> NumberDouble =
			from num in Parse.DecimalInvariant
			from token in Parse.Chars('d', 'D')
			select num + token;

		/// <summary>
		/// Parses a <see cref="decimal"/> number
		/// </summary>
		public static readonly Parser<string> NumberDecimal =
			from num in Parse.DecimalInvariant
			from token in Parse.Chars('m', 'M')
			select num + token;

		/// <summary>
		/// Parses a floating-point number
		/// </summary>
		public static readonly Parser<string> NumberAnyFloatingPoint =
			from num in NumberFloat.Or(NumberDouble).Or(NumberDecimal)
			select num;

		/// <summary>
		/// Parses an integer or floating-point number
		/// </summary>
		public static readonly Parser<string> AnyNumber =
			from num in NumberAnyInteger.Or(NumberAnyFloatingPoint)
			select num;

		/// <summary>
		/// Parses a method/local/field identifier
		/// </summary>
		public static readonly Parser<string> IdentifierString =
			from nameStart in Parse.Letter.XOr(Parse.Char('_'))
			from name in Parse.LetterOrDigit.XOr(Parse.Char('_')).Many().Text()
			select nameStart + name;

		/// <summary>
		/// Parses a field/local/argument type definition, returning the type and then "{ }" surrounding any modifiers to it (e.g. "[ ]d2" for a 2D array) in a comma-separated list
		/// </summary>
		public static readonly Parser<string> VariableType =
			from name in TypeString
			from suffix in ArraySuffix.Optional()
			select $"{name}{{{suffix.GetOrElse("")}}}";
		
		// NOTE: any modifications to this parser needs to be reflected in src/Utility/Extensions.Type.cs
		private static readonly Parser<string> TypeModifier =
			from modifier in ArraySuffix  // TODO: .Or() with more suffixes
			from additional in TypeModifier.Optional()
			select $"{modifier}{(additional.IsDefined ? $",{additional}" : "")}";

		private static readonly Parser<string> ArraySuffix =
			from start in Parse.Char('[')
			from commas in Parse.Char(',').Many().Text().Optional()
			from end in Parse.Char(']')
			select $"{start}{end}d{commas.GetOrElse("").Length}";

		private static readonly Parser<string> TypeString =
			from assemblyOrType in IdentifierString
			from subsequent in Parse.Char('.').Then(_ => TypeString.Or(NestedTypeString)).Optional()
			select CombineStringWithOptions(assemblyOrType, strFirst: true, subsequent);

		private static readonly Parser<string> NestedTypeString =
			from type in IdentifierString
			from subsequent in Parse.Char('+').Then(_ => NestedTypeString).Optional()
			select CombineStringWithOptions(type, strFirst: true, subsequent);

		/// <summary>
		/// Parses a method argument list, returning a comma-separated list of the argument definitions
		/// </summary>
		public static readonly Parser<string> FunctionArguments =
			from open in Parse.Char('(')
			from args in FunctionArgument.Once().Then(_ => ManyWhitespace.Then(_ => Parse.Char(',')).Then(_ => ManyWhitespace).Then(_ => FunctionArgument).Many().Optional()).Optional()
			from close in Parse.Char(')')
			select !args.IsDefined ? "" : string.Join(",", args.Get().Get());

		private static readonly Parser<string> FunctionArgument =
			from name in IdentifierString
			from whitespace in ManyWhitespace
			from separator in Parse.Char(':')
			from whitespace2 in ManyWhitespace
			from type in VariableType
			select name + whitespace + separator + whitespace2 + type;

		/// <summary>
		/// Parses a local definition, returning a comma-separated list of the following:<br/>
		/// <c>[constant],name,type</c>
		/// </summary>
		public static readonly Parser<string> LocalDefinition =
			from token in Parse.String(".local").Text()
			from ws in ManyWhitespace
			from constant in Parse.String("const").Then(_ => ManyWhitespace).Text().Optional()
			from name in IdentifierString
			from ws2 in ManyWhitespace
			from colon in Parse.Char(':')
			from ws3 in ManyWhitespace
			from type in VariableType
			select $"{(constant.IsDefined ? "constant" : "")},{name},{type}";

		/// <summary>
		/// Parses an instruction line, returning a comma-separated list including the opcode and its arguments
		/// </summary>
		public static readonly Parser<string> OpcodeAndOperand =
			from code in Opcode
			from ws in ManyWhitespace.Optional()
			from operands in Parse.AnyChar.Until(Parse.LineTerminator).Text().Optional()
			select $"{code},{operands.GetOrElse("")}";

		private static readonly Parser<string> Opcode =
			from code in Parse.Letter.Until(Parse.WhiteSpace).Text()
			select code;

		/// <summary>
		/// Parses a field token, returning a comma-separated list of the type, the type modifier, and field name
		/// </summary>
		public static readonly Parser<string> FieldAccess =
			from type in TypeString
			from sep in Parse.String("::")
			from field in IdentifierString
			select $"{type},{field}";

		private static string CombineStringWithOptions(string str, bool strFirst, params IOption<char>[] options) {
			StringBuilder sb = new();

			if (strFirst) {
				sb.Append(str);

				foreach (var o in options)
					if (o.IsDefined)
						sb.Append(o.Get());
			} else {
				foreach (var o in options)
					if (o.IsDefined)
						sb.Append(o.Get());

				sb.Append(str);
			}

			return sb.ToString();
		}

		private static string CombineStringWithOptions(string str, bool strFirst, params IOption<string>[] options) {
			StringBuilder sb = new();

			if (strFirst) {
				sb.Append(str);

				foreach (var o in options)
					if (o.IsDefined)
						sb.Append(o.Get());
			} else {
				foreach (var o in options)
					if (o.IsDefined)
						sb.Append(o.Get());

				sb.Append(str);
			}

			return sb.ToString();
		}
	}
}
