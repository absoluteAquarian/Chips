using AsmResolver.PE.DotNet.Metadata.Tables.Rows;
using Chips.Utility;

namespace Chips.Compiler.Parsing {
	public readonly struct ParsedType {
		public readonly bool isClass;
		public readonly TypeAttributes attributes;

		public ParsedType(bool isClass, TypeAttributes attributes) {
			this.isClass = isClass;
			this.attributes = attributes;
		}
	}

	public readonly struct ParsedMethodVariable {
		public readonly string name;
		public readonly string type;

		public ParsedMethodVariable(string name, string type) {
			this.name = name;
			this.type = type;
		}

		public ParsedMethodVariable(string parsedString) {
			int index = parsedString.IndexOf(':');
			if (index < 1)
				throw new ParsingException("Invalid method argument");
			name = parsedString[..index];
			type = parsedString[(index + 1)..];
		}
	}

	public readonly struct ParsedPossibleQuotedString {
		public readonly string text;
		public readonly bool wasQuoted;

		public ParsedPossibleQuotedString(string text, bool wasQuoted) {
			this.text = text;
			this.wasQuoted = wasQuoted;
		}
	}

	public readonly struct ParsedMethodReference {
		public readonly string type;
		public readonly string name;
		public readonly string[] parameterTypes;

		public ParsedMethodReference(string type, string name, string[] parameterTypes) {
			this.type = type;
			this.name = name;
			this.parameterTypes = parameterTypes;
		}
	}

	public readonly struct ParsedTypeAndModifiers {
		public readonly string type;
		public readonly string modifiers;

		public ParsedTypeAndModifiers(string type, string modifiers) {
			this.type = type;
			this.modifiers = modifiers;
		}

		public string AttemptCoreTypeAlias() => type.AttemptCoreTypeAlias() + modifiers;
	}
}
