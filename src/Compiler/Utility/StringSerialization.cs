using Chips.Compiler.Parsing;
using Sprache;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System;
using AsmResolver.DotNet;
using Chips.Utility;
using System.Linq;

namespace Chips.Compiler.Utility {
	public static partial class StringSerialization {
		public static object? ParseIntegerArgument(CompilationContext context, string arg) {
			if (arg.Length == 0)
				throw ChipsCompiler.ErrorAndThrow(new FormatException("Argument cannot be empty"));

			bool numberUnsigned = false;
			bool numberLarge = false;

			ReadOnlySpan<char> span = arg;

			// Check for unsigned/large number suffix
			if (arg.Length > 2) {
				ReadOnlySpan<char> slice = span[^2..];

				if (slice.Equals("ul", StringComparison.OrdinalIgnoreCase)) {
					numberUnsigned = true;
					numberLarge = true;
					span = span[..^2];
				} else if (slice.Equals("u", StringComparison.OrdinalIgnoreCase)) {
					numberUnsigned = true;
					span = span[..^1];
				} else if (slice.Equals("l", StringComparison.OrdinalIgnoreCase)) {
					numberLarge = true;
					span = span[..^1];
				}
			}

			bool hexNumber = false;
			bool binaryNumber = false;
			
			// Check for hex/binary prefix
			if (span.Length > 2) {
				ReadOnlySpan<char> slice = span[..2];

				if (slice.Equals("0x", StringComparison.OrdinalIgnoreCase)) {
					hexNumber = true;
					span = span[2..];
				} else if (slice.Equals("0b", StringComparison.OrdinalIgnoreCase)) {
					binaryNumber = true;
					span = span[2..];
				}
			}

			// Remove any underscores from hexadecimal/binary number strings
			if (hexNumber || binaryNumber)
				span = span.ToString().Replace("_", "");

			// Parse the number
			if (hexNumber) {
				// Force the number to be large if it's longer than 8 nibbles
				if (span.Length > 8)
					numberLarge = true;

				if (numberLarge) {
					if (numberUnsigned) {
						if (!ulong.TryParse(span, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ulong result))
							throw ChipsCompiler.ErrorAndThrow(new FormatException($"Argument \"{arg}\" could not be parsed as a hexadecimal UInt64 number"));

						return result;
					} else {
						if (!long.TryParse(span, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out long result))
							throw ChipsCompiler.ErrorAndThrow(new FormatException($"Argument \"{arg}\" could not be parsed as a hexadecimal Int64 number"));

						return result;
					}
				} else {
					if (numberUnsigned) {
						if (!uint.TryParse(span, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint result))
							throw ChipsCompiler.ErrorAndThrow(new FormatException($"Argument \"{arg}\" could not be parsed as a hexadecimal UInt32 number"));

						return result;
					} else {
						if (!int.TryParse(span, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int result))
							throw ChipsCompiler.ErrorAndThrow(new FormatException($"Argument \"{arg}\" could not be parsed as a hexadecimal Int32 number"));

						return result;
					}
				}
			} else if (binaryNumber) {
				// Force the number to be large if it's longer than 32 bits
				if (span.Length > 32)
					numberLarge = true;

				if (numberLarge) {
					if (span.IndexOfAnyExcept("01") != -1)
						throw ChipsCompiler.ErrorAndThrow(new FormatException($"Argument \"{arg}\" could not be parsed as a binary Int64 number"));

					return numberUnsigned ? Convert.ToUInt64(span.ToString(), 2) : Convert.ToInt64(span.ToString(), 2);
				} else {
					if (span.IndexOfAnyExcept("01") != -1)
						throw ChipsCompiler.ErrorAndThrow(new FormatException($"Argument \"{arg}\" could not be parsed as a binary Int32 number"));

					return numberUnsigned ? Convert.ToUInt32(span.ToString(), 2) : Convert.ToInt32(span.ToString(), 2);
				}
			} else {
				if (numberLarge) {
					if (numberUnsigned) {
						if (!ulong.TryParse(span, out ulong result))
							throw ChipsCompiler.ErrorAndThrow(new FormatException($"Argument \"{arg}\" could not be parsed as a UInt64 number"));

						return result;
					} else {
						if (!long.TryParse(span, out long result))
							throw ChipsCompiler.ErrorAndThrow(new FormatException($"Argument \"{arg}\" could not be parsed as an Int64 number"));

						return result;
					}
				} else {
					if (numberUnsigned) {
						if (!uint.TryParse(span, out uint result))
							throw ChipsCompiler.ErrorAndThrow(new FormatException($"Argument \"{arg}\" could not be parsed as a UInt32 number"));

						return result;
					} else {
						if (!int.TryParse(span, out int result))
							throw ChipsCompiler.ErrorAndThrow(new FormatException($"Argument \"{arg}\" could not be parsed as an Int32 number"));

						return result;
					}
				}
			}
		}

		public static ushort ParseSimpleSmallIntegerArgument(CompilationContext context, string arg) {
			if (arg.Length == 0)
				throw ChipsCompiler.ErrorAndThrow(new FormatException("Argument cannot be empty"));

			// Parse an unsigned 16-bit integer
			if (!ushort.TryParse(arg, out ushort result))
				throw ChipsCompiler.ErrorAndThrow(new FormatException($"Argument \"{arg}\" could not be parsed as an unsigned 16-bit integer"));

			return result;
		}

		public static object? ParseFloatArgument(CompilationContext context, string arg) {
			if (arg.Length == 0)
				return ChipsCompiler.ErrorAndThrow(new FormatException("Argument cannot be empty"));

			ReadOnlySpan<char> span = arg;

			if (span[^1] == 'f') {
				// Single constant
				if (!float.TryParse(span[..^1], NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
					throw ChipsCompiler.ErrorAndThrow(new FormatException($"Argument \"{arg}\" could not be parsed as a Single number"));
					
				return result;
			} else if (span[^1] == 'm') {
				// Decimal constant
				if (!decimal.TryParse(span[..^1], NumberStyles.Float, CultureInfo.InvariantCulture, out decimal result))
					throw ChipsCompiler.ErrorAndThrow(new FormatException($"Argument \"{arg}\" could not be parsed as a Decimal number"));

				return result;
			} else if (span[^1] == 'd') {
				// Double constant
				if (!double.TryParse(span[..^1], NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
					throw ChipsCompiler.ErrorAndThrow(new FormatException($"Argument \"{arg}\" could not be parsed as a Double number"));

				return result;
			} else {
				// Could not parse as float
				return ChipsCompiler.ErrorAndThrow(new FormatException($"Argument \"{arg}\" could not be parsed as a floating-point number"));
			}
		}

		[StringSyntax("regex")]
		private const string TYPE_IDENTIFIER_REGEX = @"([a-zA-Z_][a-zA-Z_\d.+]*){((?:\[\]d\d+|\?|,)*)}";

		[GeneratedRegex(TYPE_IDENTIFIER_REGEX, RegexOptions.Compiled)]
		private static partial Regex TypeRegex();
		private static readonly Regex typeParseResultRegex = TypeRegex();

		[StringSyntax("regex")]
		private const string FIELD_IDENTIFIER_REGEX = @",((?:[a-zA-Z_][a-zA-Z_\d]*)*)";

		[GeneratedRegex(TYPE_IDENTIFIER_REGEX + FIELD_IDENTIFIER_REGEX, RegexOptions.Compiled)]
		private static partial Regex FieldRegex();
		private static readonly Regex fieldParseResultRegex = FieldRegex();

		public static TypeDefinition ParseTypeIdentifierArgument(CompilationContext context, string arg, bool argWasAlreadyParsed) {
			ExtractTypeInformation(arg, argWasAlreadyParsed, out string typeName, out string modifiers);

			if (!context.resolver.Resolve(typeName, out _, out TypeDefinition? typeDefinition))
				throw ChipsCompiler.ErrorAndThrow(new InvalidDataException($"Type \"{typeName.DesanitizeString()}\" could not be resolved"));

			if (modifiers is { Length: >0 })
				typeDefinition = typeDefinition.AdjustTypeBasedOnSuffix(modifiers);

			return typeDefinition;
		}

		public static void ExtractTypeInformation(string arg, bool argWasAlreadyParsed, out string name, out string modifiers) {
			string type;
			if (!argWasAlreadyParsed) {
				if (ParsingSequences.VariableType.TryParse(arg) is not IResult<string> { WasSuccessful: true } result)
					throw ChipsCompiler.ErrorAndThrow(new InvalidDataException($"Type indicator \"{arg.DesanitizeString()}\" is malformed"));

				type = result.Value;
			} else
				type = arg;

			if (!typeParseResultRegex.IsMatch(type))
				throw ChipsCompiler.ErrorAndThrow(new InvalidDataException(argWasAlreadyParsed ? "Type indicator is malformed" : $"Type indicator \"{arg.DesanitizeString()}\" is malformed"));

			var match = typeParseResultRegex.Match(type);
			name = match.Groups[1].Value;
			modifiers = match.Groups[2].Value;
		}

		public static FieldDefinition ParseFieldIdentifierArgument(CompilationContext context, string arg) {
			ExtractFieldInformation(arg, out string typeName, out string modifiers, out string fieldName);

			if (!context.resolver.Resolve(typeName, out _, out TypeDefinition? typeDefinition))
				throw ChipsCompiler.ErrorAndThrow(new InvalidDataException($"Type \"{typeName.DesanitizeString()}\" could not be resolved"));

			if (modifiers is { Length: >0 })
				typeDefinition = typeDefinition.AdjustTypeBasedOnSuffix(modifiers);

			if (typeDefinition.Fields.FirstOrDefault(f => f.Name?.ToString() == fieldName) is not FieldDefinition fieldDefinition)
				throw ChipsCompiler.ErrorAndThrow(new InvalidDataException($"Field \"{fieldName.DesanitizeString()}\" does not exist in type \"{typeDefinition.Name}\""));

			return fieldDefinition;
		}

		public static void ExtractFieldInformation(string arg, out string typeName, out string typeModifiers, out string fieldName) {
			if (ParsingSequences.FieldAccess.TryParse(arg) is not IResult<string> { WasSuccessful: true } result)
				throw ChipsCompiler.ErrorAndThrow(new InvalidDataException($"Field indicator \"{arg.DesanitizeString()}\" is malformed"));

			if (!fieldParseResultRegex.IsMatch(result.Value))
				throw ChipsCompiler.ErrorAndThrow(new InvalidDataException($"Field indicator \"{arg.DesanitizeString()}\" is malformed"));

			// Field name split happens on the last comma
			var match = fieldParseResultRegex.Match(result.Value);
			typeName = match.Groups[1].Value;
			typeModifiers = match.Groups[2].Value;
			fieldName = match.Groups[3].Value;
		}

		public static void ExtractFieldInformation(string arg, out string typeFullDefinition, out string fieldName) {
			if (ParsingSequences.FieldAccess.TryParse(arg) is not IResult<string> { WasSuccessful: true } result)
				throw ChipsCompiler.ErrorAndThrow(new InvalidDataException($"Field indicator \"{arg.DesanitizeString()}\" is malformed"));

			if (!fieldParseResultRegex.IsMatch(result.Value))
				throw ChipsCompiler.ErrorAndThrow(new InvalidDataException($"Field indicator \"{arg.DesanitizeString()}\" is malformed"));

			// Data is ignored, just split on "::"
			int index = arg.LastIndexOf("::");
			typeFullDefinition = arg[..index];
			fieldName = arg[(index + 2)..];
		}

		public static MethodDefinition ParseMethodIdentifierArgument(CompilationContext context, string arg) {
			if (ParsingSequences.MethodAccess.TryParse(arg) is not IResult<ParsedMethodReference> { WasSuccessful: true } result)
				throw ChipsCompiler.ErrorAndThrow(new InvalidDataException($"Method indicator \"{arg.DesanitizeString()}\" is malformed"));

			var method = result.Value;

			ExtractTypeInformation(method.type, true, out string typeName, out string modifiers);

			if (!context.resolver.Resolve(typeName, out _, out TypeDefinition? typeDefinition))
				throw ChipsCompiler.ErrorAndThrow(new InvalidDataException($"Type \"{typeName.DesanitizeString()}\" could not be resolved"));

			if (modifiers is { Length: >0 })
				typeDefinition = typeDefinition.AdjustTypeBasedOnSuffix(modifiers);

			// Local capturing
			CompilationContext c = context;
			string methodName = method.name;

			TypeDefinition[] methodParameters = method.parameterTypes.Select(s => ParseTypeIdentifierArgument(c, s, true)).ToArray();

			bool DoesMethodSignatureMatch(MethodDefinition search) {
				if (search.Name?.ToString() != methodName)
					return false;

				var searchParams = search.ParameterDefinitions;

				if (searchParams.Count != methodParameters.Length)
					return false;

				for (int i = 0; i < searchParams.Count; i++) {
					if (searchParams[i].MetadataToken != methodParameters[i].MetadataToken)
						return false;
				}

				return true;
			}

			if (typeDefinition.Methods.FirstOrDefault(DoesMethodSignatureMatch) is not MethodDefinition methodDefinition)
				throw ChipsCompiler.ErrorAndThrow(new InvalidDataException($"Method \"{method.name.DesanitizeString()}\" does not exist in type \"{typeDefinition.Name}\" with the specified arguments"));

			return methodDefinition;
		}
	}
}
