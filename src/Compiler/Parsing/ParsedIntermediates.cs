using AsmResolver.PE.DotNet.Metadata.Tables.Rows;
using Chips.Common.Utility;
using System;

namespace Chips.Compiler.Parsing {
	public readonly struct ParsedType(bool isClass, TypeAttributes attributes) {
		public readonly bool isClass = isClass;
		public readonly TypeAttributes attributes = attributes;
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
				throw new ArgumentException("Invalid method argument");
			name = parsedString[..index];
			type = parsedString[(index + 1)..];
		}
	}

	public readonly struct ParsedPossibleQuotedString(string text, bool wasQuoted) {
		public readonly string text = text;
		public readonly bool wasQuoted = wasQuoted;
	}

	public readonly struct ParsedMethodReference(string type, string name, string[] parameterTypes) {
		public readonly string type = type;
		public readonly string name = name;
		public readonly string[] parameterTypes = parameterTypes;
	}

	public readonly struct ParsedTypeAndModifiers(string type, string modifiers) {
		public readonly string type = type;
		public readonly string modifiers = modifiers;

		public string AttemptCoreTypeAlias() => type.AttemptCoreTypeAlias() + modifiers;
	}
}
