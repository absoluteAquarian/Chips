using Chips.Compiler;
using System.Globalization;
using System;
using Chips.Parsing;
using Sprache;
using System.IO;
using AsmResolver.DotNet;
using Chips.Utility;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics.CodeAnalysis;

namespace Chips.Runtime.Specifications {
	partial class Opcode {
		protected static object? ParseIntegerArgument(CompilationContext context, string arg) {
			if (arg.Length == 0)
				throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new FormatException("Argument cannot be empty"));

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
							throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new FormatException($"Argument \"{arg}\" could not be parsed as a hexadecimal UInt64 number"));

						return result;
					} else {
						if (!long.TryParse(span, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out long result))
							throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new FormatException($"Argument \"{arg}\" could not be parsed as a hexadecimal Int64 number"));

						return result;
					}
				} else {
					if (numberUnsigned) {
						if (!uint.TryParse(span, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint result))
							throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new FormatException($"Argument \"{arg}\" could not be parsed as a hexadecimal UInt32 number"));

						return result;
					} else {
						if (!int.TryParse(span, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int result))
							throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new FormatException($"Argument \"{arg}\" could not be parsed as a hexadecimal Int32 number"));

						return result;
					}
				}
			} else if (binaryNumber) {
				// Force the number to be large if it's longer than 32 bits
				if (span.Length > 32)
					numberLarge = true;

				if (numberLarge) {
					if (span.IndexOfAnyExcept("01") != -1)
						throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new FormatException($"Argument \"{arg}\" could not be parsed as a binary Int64 number"));

					return numberUnsigned ? Convert.ToUInt64(span.ToString(), 2) : Convert.ToInt64(span.ToString(), 2);
				} else {
					if (span.IndexOfAnyExcept("01") != -1)
						throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new FormatException($"Argument \"{arg}\" could not be parsed as a binary Int32 number"));

					return numberUnsigned ? Convert.ToUInt32(span.ToString(), 2) : Convert.ToInt32(span.ToString(), 2);
				}
			} else {
				if (numberLarge) {
					if (numberUnsigned) {
						if (!ulong.TryParse(span, out ulong result))
							throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new FormatException($"Argument \"{arg}\" could not be parsed as a UInt64 number"));

						return result;
					} else {
						if (!long.TryParse(span, out long result))
							throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new FormatException($"Argument \"{arg}\" could not be parsed as an Int64 number"));

						return result;
					}
				} else {
					if (numberUnsigned) {
						if (!uint.TryParse(span, out uint result))
							throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new FormatException($"Argument \"{arg}\" could not be parsed as a UInt32 number"));

						return result;
					} else {
						if (!int.TryParse(span, out int result))
							throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new FormatException($"Argument \"{arg}\" could not be parsed as an Int32 number"));

						return result;
					}
				}
			}
		}

		protected static object? ParseFloatArgument(CompilationContext context, string arg) {
			if (arg.Length == 0)
				return ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new FormatException("Argument cannot be empty"));

			ReadOnlySpan<char> span = arg;

			if (span[^1] == 'f') {
				// Single constant
				if (!float.TryParse(span[..^1], NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
					throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new FormatException($"Argument \"{arg}\" could not be parsed as a Single number"));
					
				return result;
			} else if (span[^1] == 'm') {
				// Decimal constant
				if (!decimal.TryParse(span[..^1], NumberStyles.Float, CultureInfo.InvariantCulture, out decimal result))
					throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new FormatException($"Argument \"{arg}\" could not be parsed as a Decimal number"));

				return result;
			} else {
				// Default to double constant
				if (span[^1] == 'd')
					span = span[..^1];

				if (!double.TryParse(span, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
					throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new FormatException($"Argument \"{arg}\" could not be parsed as a Double number"));

				return result;
			}
		}

		[StringSyntax("regex")]
		private const string TYPE_IDENTIFIER_REGEX = "([a-zA-Z_][a-zA-Z_\\d.+]*){((?:\\[\\]d\\d+|\\?|,)*)}";

		[GeneratedRegex(TYPE_IDENTIFIER_REGEX, RegexOptions.Compiled)]
		private static partial Regex TypeRegex();
		private static readonly Regex typeParseResultRegex = TypeRegex();

		[StringSyntax("regex")]
		private const string FIELD_IDENTIFIER_REGEX = ",((?:[a-zA-z_][a-zA-Z_\\d]*)*)";

		[GeneratedRegex(TYPE_IDENTIFIER_REGEX + FIELD_IDENTIFIER_REGEX, RegexOptions.Compiled)]
		private static partial Regex FieldRegex();
		private static readonly Regex fieldParseResultRegex = FieldRegex();

		protected static object? ParseTypeIdentifierArgument(CompilationContext context, string arg) {
			if (ParsingSequences.VariableType.TryParse(arg) is not IResult<string> { WasSuccessful: true } result)
				throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new InvalidDataException($"Type indicator \"{arg}\" is malformed"));

			if (!typeParseResultRegex.IsMatch(result.Value))
				throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new InvalidDataException($"Type indicator \"{arg}\" is malformed"));

			var match = typeParseResultRegex.Match(result.Value);
			string typeName = match.Groups[1].Value;
			string modifiers = match.Groups[2].Value;

			if (!context.resolver.Resolve(typeName, out _, out TypeDefinition? typeDefinition))
				throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new InvalidDataException($"Type \"{typeName}\" could not be resolved"));

			if (modifiers is not null)
				typeDefinition = typeDefinition.AdjustTypeBasedOnSuffix(modifiers);

			return typeDefinition;
		}

		protected static object? ParseFieldIdentifierArgument(CompilationContext context, string arg) {
			if (ParsingSequences.FieldAccess.TryParse(arg) is not IResult<string> { WasSuccessful: true } result)
				throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new InvalidDataException($"Field indicator \"{arg}\" is malformed"));

			if (!fieldParseResultRegex.IsMatch(result.Value))
				throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new InvalidDataException($"Field indicator \"{arg}\" is malformed"));

			// Field name split happens on the last comma
			var match = fieldParseResultRegex.Match(result.Value);
			string typeName = match.Groups[1].Value;
			string modifiers = match.Groups[2].Value;
			string fieldName = match.Groups[3].Value;

			if (!context.resolver.Resolve(typeName, out _, out TypeDefinition? typeDefinition))
				throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new InvalidDataException($"Type \"{typeName}\" could not be resolved"));

			if (modifiers is not null)
				typeDefinition = typeDefinition.AdjustTypeBasedOnSuffix(modifiers);

			if (typeDefinition.Fields.FirstOrDefault(f => f.Name?.ToString() == fieldName) is not FieldDefinition fieldDefinition)
				throw ChipsCompiler.ErrorAndThrow(context.resolver.activeSourceFile, new InvalidDataException($"Field \"{fieldName}\" does not exist in type \"{typeDefinition.Name}\""));

			return fieldDefinition;
		}
	}
}
