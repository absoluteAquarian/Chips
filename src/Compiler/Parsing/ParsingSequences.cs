using AsmResolver.PE.DotNet.Metadata.Tables.Rows;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chips.Compiler.Parsing {
	internal static class ParsingSequences {
		/// <summary>
		/// Parses a method/local/field identifier
		/// </summary>
		public static readonly Parser<string> IdentifierString =
			from nameStart in Parse.Letter.XOr(Parse.Char('_'))
			from name in Parse.LetterOrDigit.XOr(Parse.Char('_')).Many().Text()
			select nameStart + name;

		public static Parser<string> IdentifierStringWithSuffix(string suffix) {
			return from name in IdentifierString
				   from end in Parse.String(suffix).Token()
				   select $"{name}{end}";
		}

		public static Parser<string> IdentifierStringWithSuffix(char suffix) {
			return from name in IdentifierString
				   from end in Parse.Char(suffix).Token()
				   select $"{name}{end}";
		}

		private static readonly Parser<string> ArraySuffix =
			from start in Parse.Char('[')
			from commas in Parse.Char(',').Many().Text().Optional()
			from end in Parse.Char(']')
			select $"{start}{end}d{commas.GetOrElse("").Length + 1}";
		
		// NOTE: any modifications to this parser needs to be reflected in src/Utility/Extensions.Type.cs
		private static readonly Parser<string> TypeModifier =
			from modifier in ArraySuffix  // TODO: .Or() with more suffixes
			from additional in TypeModifier.Optional()
			select $"{modifier}{(additional.IsDefined ? $",{additional}" : "")}";

		private static readonly Parser<string> NestedTypeString =
			from type in IdentifierString
			from subsequent in Parse.Char('+').Then(_ => NestedTypeString).Optional()
			select CombineStringWithOptions(type, strFirst: true, subsequent);

		private static readonly Parser<string> TypeString =
			from assemblyOrType in IdentifierString
			from subsequent in Parse.Char('.').Then(_ => TypeString.Or(NestedTypeString)).Optional()
			select CombineStringWithOptions(assemblyOrType, strFirst: true, subsequent);

		/// <summary>
		/// Parses a field/local/argument type definition, returning the type and then "{ }" surrounding any modifiers to it (e.g. "[ ]d2" for a 2D array) in a comma-separated list
		/// </summary>
		public static readonly Parser<string> VariableType =
			from name in TypeString
			from suffix in TypeModifier.Optional()
			select $"{name}{{{suffix.GetOrElse("")}}}";

		public static readonly Parser<ParsedTypeAndModifiers> TypeAndModifiers =
			from name in TypeString
			from suffix in TypeModifier.Optional()
			select new ParsedTypeAndModifiers(name, suffix.GetOrElse(""));

		public static readonly Parser<string> TokenizedVariableType =
			from type in VariableType.Token()
			select type;

		private static readonly Parser<string> FunctionArgument =
			from name in IdentifierStringWithSuffix(':').Token()
			from type in VariableType.Token()
			select $"{name}{type}";

		private static readonly Parser<string> SubsequentFunctionArgument =
			from comma in Parse.Char(',').Token()
			from arg in FunctionArgument.Token()
			select arg;

		private static readonly Parser<IEnumerable<string>> FunctionArgumentList =
			from firstArg in FunctionArgument.Once()
			from remainingArgs in SubsequentFunctionArgument.Many().Optional()
			select firstArg.Concat(remainingArgs.GetOrElse(Array.Empty<string>()));

		/// <summary>
		/// Parses a method argument list, returning an array of the argument definitions
		/// </summary>
		public static readonly Parser<ParsedMethodVariable[]> FunctionArguments =
			from open in Parse.Char('(').Token()
			from args in FunctionArgumentList.Optional()
			from close in Parse.Char(')').Token()
			select !args.IsDefined ? Array.Empty<ParsedMethodVariable>() : args.Get().Select(static s => new ParsedMethodVariable(s)).ToArray();

		/// <summary>
		/// Parses a function local definition
		/// </summary>
		public static readonly Parser<ParsedMethodVariable> FunctionLocal =
			from def in FunctionArgument.Token()
			select new ParsedMethodVariable(def);

		private static readonly Parser<string> OpcodeText =
			from code in Parse.Lower.Many().Text()
			from dot in Parse.Char('.').Optional()
			from rest in Parse.Lower.Many().Text().Optional()
			select $"{code}{(dot.IsDefined ? dot.Get().ToString() : string.Empty)}{rest.GetOrElse(string.Empty)}";

		/// <summary>
		/// Parses an instruction opcode
		/// </summary>
		public static readonly Parser<string> Opcode =
			from code in OpcodeText.Token()
			select code;

		/// <summary>
		/// Parses a field token, returning a comma-separated list of the type, the type modifier, and field name
		/// </summary>
		public static readonly Parser<string> FieldAccess =
			from type in TypeString
			from sep in Parse.String("::")
			from field in IdentifierString
			select $"{type},{field}";

		private static readonly Parser<string[]> MethodAccessParameters =
			from open in Parse.Char('(').Token()
			from args in VariableType.Once().Then(_ => Parse.Char(',').Token().Then(_ => VariableType).Many().Optional()).Optional()
			from close in Parse.Char(')').Token()
			select !args.IsDefined ? Array.Empty<string>() : args.Get().Get().ToArray();

		/// <summary>
		/// Parses a method token, returning a structure representing the token's components
		/// </summary>
		public static readonly Parser<ParsedMethodReference> MethodAccess =
			from type in TypeString
			from sep in Parse.String("::")
			from methodName in IdentifierString
			from args in MethodAccessParameters
			select new ParsedMethodReference(type, methodName, args);

		private static readonly Parser<string> TypeClassOrStruct =
			from name in Parse.String("class").Or(Parse.String("struct")).Token().Text()
			select name;

		private static readonly Parser<TypeAttributes> TypeStaticOrAbstract =
			from type in Parse.String("static").Or(Parse.String("abstract")).Token().Text()
			select (type == "static" ? TypeAttributes.Sealed : 0) | TypeAttributes.Abstract;

		/// <summary>
		/// Parses the access modifiers for a type, returning an attributes constant
		/// </summary>
		public static readonly Parser<TypeAttributes> TypeAccessModifiers =
			from access in Parse.String("public").Or(Parse.String("assembly")).Token().Text()
			from type in TypeStaticOrAbstract.Optional()
			select (access == "public" ? TypeAttributes.Public : TypeAttributes.NotPublic) | type.GetOrDefault();

		private static readonly Parser<IEnumerable<char>> TypeMemberDualAccessModifier =
			from access in Parse.String("derived").Token().Text()
			from hide in Parse.String("assembly").Or(Parse.String("private")).Token().Text().Optional()
			select $"{access}{(hide.IsDefined ? $" {hide.Get()}" : "")}";

		private static readonly Parser<string> TypeMemberAccess =
			from access in Parse.String("public").Or(Parse.String("assembly")).Or(Parse.String("derived")).Or(Parse.String("private")).Or(TypeMemberDualAccessModifier).Token().Text()
			select access;

		/// <summary>
		/// Parses a type definition, returning an object representing the type's classification and access modifiers
		/// </summary>
		public static readonly Parser<ParsedType> TypeClassificationAndAttributes =
			from classification in TypeClassOrStruct
			from access in TypeAccessModifiers
			select new ParsedType(classification == "class", access);

		/// <summary>
		/// Parses the access modifiers for a nested type, returning an attributes constant
		/// </summary>
		public static readonly Parser<TypeAttributes> NestedTypeAccessModifiers =
			from access in TypeMemberAccess
			from type in TypeStaticOrAbstract.Optional()
			select access switch {
				"public" => TypeAttributes.NestedPublic,
				"assembly" => TypeAttributes.NestedAssembly,
				"derived" => TypeAttributes.NestedFamily,
				"private" => TypeAttributes.NestedPrivate,
				"derived assembly" => TypeAttributes.NestedFamilyOrAssembly,
				"derived private" => TypeAttributes.NestedFamilyAndAssembly,
				_ => 0
			} | type.GetOrDefault();

		public static readonly Parser<ParsedType> NestedTypeClassificationAndAttributes =
			from classification in TypeClassOrStruct
			from access in NestedTypeAccessModifiers
			select new ParsedType(classification == "class", access);

		private static readonly Parser<FieldAttributes> FieldStatic =
			from type in Parse.String("static").Or(Parse.String("abstract")).Token().Text()
			select type == "static" ? FieldAttributes.Static : 0;

		/// <summary>
		/// Parses the access modifiers for a field, returning an attributes constant
		/// </summary>
		public static readonly Parser<FieldAttributes> FieldAccessModifiers = 
			from access in TypeMemberAccess
			from type in FieldStatic.Optional()
			select access switch {
				"public" => FieldAttributes.Public,
				"assembly" => FieldAttributes.Assembly,
				"derived" => FieldAttributes.Family,
				"private" => FieldAttributes.Private,
				"derived assembly" => FieldAttributes.FamilyOrAssembly,
				"derived private" => FieldAttributes.FamilyAndAssembly,
				_ => 0
			} | type.GetOrDefault();

		private static readonly Parser<MethodAttributes> MethodStaticOrAbstract =
			from type in Parse.String("static").Or(Parse.String("abstract")).Token().Text()
			select type == "static" ? MethodAttributes.Static : MethodAttributes.Abstract;

		/// <summary>
		/// Parses the access modifiers for a method, returning an attributes constant
		/// </summary>
		public static readonly Parser<MethodAttributes> MethodAccessModifiers =
			from access in TypeMemberAccess
			from type in MethodStaticOrAbstract.Optional()
			select access switch {
				"public" => MethodAttributes.Public,
				"assembly" => MethodAttributes.Assembly,
				"derived" => MethodAttributes.Family,
				"private" => MethodAttributes.Private,
				"derived assembly" => MethodAttributes.FamilyOrAssembly,
				"derived private" => MethodAttributes.FamilyAndAssembly,
				_ => 0
			} | type.GetOrDefault();

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
