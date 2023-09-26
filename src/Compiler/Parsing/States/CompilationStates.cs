using AsmResolver.DotNet;
using AsmResolver.PE.DotNet.Metadata.Tables.Rows;
using Chips.Compiler.Compilation;
using Chips.Compiler.IO;
using Chips.Compiler.IO.PDB;
using Chips.Compiler.Utility;
using Chips.Runtime.Specifications;
using Chips.Utility;
using Sprache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Chips.Compiler.Parsing.States {
	internal abstract class BaseState {
		public BaseState? Previous { get; internal set; }

		public abstract bool ParseNext(StreamReader reader, CompilationContext context, BytecodeFile code, out BaseState? next);
	}

	internal abstract class MultipleStates : BaseState {
		protected abstract IEnumerable<BaseState> EnumeratePossibleStates();

		public override bool ParseNext(StreamReader reader, CompilationContext context, BytecodeFile code, out BaseState? next) {
			long pos = reader.BaseStream.Position;

			foreach (var state in EnumeratePossibleStates()) {
				state.Previous = Previous;

				if (state.ParseNext(reader, context, code, out next))
					return true;

				reader.BaseStream.Position = pos;
			}

			next = null;
			return false;
		}
	}

	internal sealed class Comment : BaseState {
		public override bool ParseNext(StreamReader reader, CompilationContext context, BytecodeFile code, out BaseState? next) {
			ChipsCompiler.CompilingSourceLineOverride = null;

			reader.ReadUntilNonWhitespace();

			if (reader.Peek() == ';') {
				reader.ReadUntilNewline();
				next = Previous;
				return true;
			}

			next = null;
			return false;
		}
	}

	internal sealed class Empty : BaseState {
		public override bool ParseNext(StreamReader reader, CompilationContext context, BytecodeFile code, out BaseState? next) {
			ChipsCompiler.CompilingSourceLineOverride = null;

			reader.ReadUntilNonWhitespace();
			next = Previous;
			return true;
		}
	}

	internal sealed class FileScope : MultipleStates {
		protected override IEnumerable<BaseState> EnumeratePossibleStates() {
			yield return new Comment();
			yield return new NamespaceImport();
			yield return new TypeAlias();
			yield return new Namespace();
			yield return new Empty();
		}
	}

	internal sealed class Scope : BaseState {
		public readonly BaseState body;

		private Scope(BaseState body) {
			ArgumentNullException.ThrowIfNull(body);
			this.body = body;
		}

		public static Scope Create(BaseState parent, BaseState body) {
			return new Scope(body) { Previous = parent.Previous };
		}

		public override bool ParseNext(StreamReader reader, CompilationContext context, BytecodeFile code, out BaseState? next) {
			// Parse the scope opening
			if (reader.ReadFirstNonWhitespaceChar() != '{') {
				next = null;
				return false;
			}

			bool ret = true;
			try {
				long pos = reader.BaseStream.Position;
				body.Previous = Previous;
				if (!body.ParseNext(reader, context, code, out next)) {
					// Allow the body parsing to fail gracefully, since it can be empty
					reader.BaseStream.Position = pos;
				}
			} catch {
				// Exceptions mean the body parsing failed
				ret = false;
				next = null;
			}

			// Parse the scope closing
			// In case any characters appeared between the current location and the closing bracket, consume them
			if (!reader.ReadUntil('}', alwaysConsume: true).EndsWith('}')) {
				next = null;
				return false;
			}

			// Parse was successful
			return ret;
		}
	}

	internal sealed class NamespaceImport : BaseState {
		public override bool ParseNext(StreamReader reader, CompilationContext context, BytecodeFile code, out BaseState? next) {
			ChipsCompiler.CompilingSourceLineOverride = ChipsCompiler.CompilingSourceLine;

			if (reader.ReadWord() != ".import") {
				next = null;
				return false;
			}

			ChipsCompiler.CompilingSourceLineOverride = ChipsCompiler.CompilingSourceLine;

			string ns = reader.ReadWord();
			context.resolver.AddNamespaceImport(ns);

			code.AddSegment(new BytecodeNamespaceImportSegment(ns));

			if (Previous is not FileScope)
				throw ChipsCompiler.ErrorAndThrow(new ParsingException("Namespace imports must be declared outside of any scope"));

			next = Previous;
			return true;
		}
	}

	internal sealed class TypeAlias : BaseState {
		public override bool ParseNext(StreamReader reader, CompilationContext context, BytecodeFile code, out BaseState? next) {
			ChipsCompiler.CompilingSourceLineOverride = ChipsCompiler.CompilingSourceLine;

			if (reader.ReadWord() != ".alias") {
				next = null;
				return false;
			}

			ChipsCompiler.CompilingSourceLineOverride = ChipsCompiler.CompilingSourceLine;

			string alias = reader.ReadWord();

			if (reader.ReadWord() != "=") {
				next = null;
				return false;
			}

			ChipsCompiler.CompilingSourceLineOverride = ChipsCompiler.CompilingSourceLine;

			string type = reader.ReadUntil(';', alwaysConsume: true);
			context.resolver.AddTypeAlias(alias, type);

			if (!context.resolver.Resolve(alias, out _, out TypeDefinition? typeDef, muteErrors: true))
				throw ChipsCompiler.ErrorAndThrow(new ParsingException($"Could not resolve type alias \"{alias.DesanitizeString()}\" to a known type"));

			code.AddSegment(new BytecodeTypeAliasSegment(alias, typeDef));

			if (Previous is not FileScope)
				throw ChipsCompiler.ErrorAndThrow(new ParsingException("Type aliases must be declared outside of any scope"));

			next = Previous;
			return true;
		}
	}

	internal class Namespace : BaseState {
		public string Name { get; private set; }

		public BytecodeNamespaceSegment Segment { get; private set; }

		public CPDBFileNamespaceSegment NamespaceSymbol { get; private set; }

		public override bool ParseNext(StreamReader reader, CompilationContext context, BytecodeFile code, out BaseState? next) {
			ChipsCompiler.CompilingSourceLineOverride = ChipsCompiler.CompilingSourceLine;

			if (reader.ReadWord() != ".namespace") {
				next = null;
				return false;
			}

			string ns = reader.ReadWord();
			Name = ns;

			string full = GetFullName();
			context.resolver.EnterNamespaceScope(full);

			Segment = new BytecodeNamespaceSegment(full);

			if (Previous is Namespace nsScope)
				nsScope.Segment.AddNamespace(Segment);
			else
				code.AddSegment(Segment);

			NamespaceSymbol = code.CPDBMembers.AddNamespace(full);

			next = Scope.Create(this, new NamespaceBody());
			return true;
		}

		public string GetFullName() => Previous is Namespace ns ? ns.GetFullName() + "." + Name : Name;
	}

	internal class NamespaceBody : MultipleStates {
		protected override IEnumerable<BaseState> EnumeratePossibleStates() {
			yield return new Comment();
			yield return new TypeIdentifier();
		}

		public override bool ParseNext(StreamReader reader, CompilationContext context, BytecodeFile code, out BaseState? next) {
			bool success = base.ParseNext(reader, context, code, out next);

			if (!success) {
				// Go to the previous state
				next = Previous;
			}

			return true;
		}

		public Namespace GetNamespace() => (Namespace)Previous!;

		public string GetFullName() => ((Namespace)Previous!).GetFullName();
	}

	internal sealed class TypeIdentifier : BaseState {
		public string Name { get; private set; }

		public BytecodeTypeSegment Segment { get; private set; }

		public CPDBFileTypeSegment TypeSymbol { get; private set; }

		public override bool ParseNext(StreamReader reader, CompilationContext context, BytecodeFile code, out BaseState? next) {
			ChipsCompiler.CompilingSourceLineOverride = ChipsCompiler.CompilingSourceLine;

			if (reader.ReadWord() != ".type") {
				next = null;
				return false;
			}

			ChipsCompiler.CompilingSourceLineOverride = ChipsCompiler.CompilingSourceLine;

			string typeName = reader.ReadWord();

			if (ParsingSequences.IdentifierString.TryParse(typeName) is not IResult<string> { WasSuccessful: true } result)
				throw ChipsCompiler.ErrorAndThrow(new ParsingException($"Invalid type name string: {typeName.DesanitizeString()}"));

			Name = result.Value;

			ChipsCompiler.CompilingSourceLineOverride = ChipsCompiler.CompilingSourceLine;

			TypeAttributes attributes = TypeAttributes.NotPublic;
			bool isClass = true;
			string word = reader.ReadWord();
			ITypeDefOrRef? baseType = null;
			switch (word) {
				case ":":
					// Accessibility modifiers were specified
					// Max modifier length: public static
					// Max modifier length (nested): derived assembly static
					string access = reader.ReadWordsUntil(Previous is TypeIdentifier ? 3 : 2, true, "->", "{", "{}");

					var parser = Previous is TypeIdentifier ? ParsingSequences.NestedTypeClassificationAndAttributes : ParsingSequences.TypeClassificationAndAttributes;
					if (parser.TryParse(access) is not IResult<ParsedType> { WasSuccessful: true } accessResult)
						throw ChipsCompiler.ErrorAndThrow(new ParsingException($"Invalid type classification or access modifiers detected: {access.DesanitizeString()}"));

					ParsedType parsed = accessResult.Value;
					attributes = parsed.attributes;
					isClass = parsed.isClass;

					word = reader.PeekWord();
					if (word == "->") {
						ChipsCompiler.CompilingSourceLineOverride = ChipsCompiler.CompilingSourceLine;

						// Consume the characters
						reader.ReadWord();
						goto case "->";
					} else if (word == "{" || word == "{}") {
						if (word == "{}")
							next = Scope.Create(this, new Empty());  // Type definition is empty
						else
							next = Scope.Create(this, new TypeBody());
					} else {
						ChipsCompiler.CompilingSourceLineOverride = ChipsCompiler.CompilingSourceLine;

						// Consume the characters, then error
						reader.ReadWord();
						goto default;
					}

					break;
				case "->":
					// Type inheritance was specified
					if (!isClass) {
						// Structs cannot inherit from anything
						throw ChipsCompiler.ErrorAndThrow(new ParsingException("Structs cannot inherit from anything"));
					}

					// TODO: implement this
					throw ChipsCompiler.ErrorAndThrow(new ParsingException("Type inheritance parsing has not been implemented yet"));
				default:
					throw ChipsCompiler.ErrorAndThrow(new ParsingException($"Token \"->\" or \":\" was expected for type definition, found \"{word.DesanitizeString()}\" instead"));
			}

			baseType ??= isClass
				? ChipsCompiler.ManifestModule.CorLibTypeFactory.Object.Resolve()!
				: ChipsCompiler.ValueTypeDefinition;

			TypeDefinition baseTypeDef = baseType.Resolve() ?? throw ChipsCompiler.ErrorAndThrow(new ParsingException("Base type could not be resolved"));

			var ns = GetNamespace();
			Segment = new BytecodeTypeSegment(ns.Segment, (Previous as TypeIdentifier)?.Segment, Name, attributes, baseTypeDef);
			ns.Segment.AddType(Segment);

			TypeSymbol = ns.NamespaceSymbol.AddType(GetFullNestedName());

			return true;
		}

		public string GetFullNestedName() => Previous is TypeIdentifier type ? type.GetFullNestedName() + "+" + Name : Name;

		public Namespace GetNamespace()
			=> Previous switch {
				NamespaceBody ns => ns.GetNamespace(),
				TypeIdentifier type => type.GetNamespace(),
				_ => throw ChipsCompiler.ErrorAndThrow(new Exception($"Invalid previous state for {nameof(TypeIdentifier)}: {Previous?.GetType().Name ?? "null"}"))
			};
	}

	internal sealed class TypeBody : MultipleStates {
		protected override IEnumerable<BaseState> EnumeratePossibleStates() {
			yield return new Comment();
			yield return new Field();
			yield return new MethodIdentifier();
			yield return new TypeIdentifier();
			yield return new Empty();
		}

		public override bool ParseNext(StreamReader reader, CompilationContext context, BytecodeFile code, out BaseState? next) {
			bool success = base.ParseNext(reader, context, code, out _);

			if (success) {
				// Attempt to parse more things in this state
				next = this;
			} else {
				// Go to the previous state
				next = Previous;
			}

			return true;
		}

		public TypeIdentifier GetScope() => (TypeIdentifier)Previous!;
	}

	internal sealed class Field : BaseState {
		public string Name { get; private set; }

		public BytecodeFieldSegment Segment { get; private set; }

		public override bool ParseNext(StreamReader reader, CompilationContext context, BytecodeFile code, out BaseState? next) {
			ChipsCompiler.CompilingSourceLineOverride = ChipsCompiler.CompilingSourceLine;

			if (reader.ReadWord() != ".field") {
				next = null;
				return false;
			}

			ChipsCompiler.CompilingSourceLineOverride = ChipsCompiler.CompilingSourceLine;

			// Max modifier length:  derived assembly static
			string modifiers = reader.ReadWordsUntil(3, true, ":");

			// Last "modifier" was actually the field name
			int index = modifiers.LastIndexOf(' ');
			string fieldName;
			FieldAttributes attributes;

			if (index > 0) {
				fieldName = modifiers[(index + 1)..];
				modifiers = modifiers[..index];

				if (ParsingSequences.FieldAccessModifiers.TryParse(modifiers) is not IResult<FieldAttributes> { WasSuccessful: true } attributesResult)
					throw ChipsCompiler.ErrorAndThrow(new ParsingException($"Invalid access modifiers detected: {modifiers.DesanitizeString()}"));

				attributes = attributesResult.Value;
			} else {
				// No modifiers specified
				modifiers = "";
				fieldName = modifiers;

				attributes = FieldAttributes.Private;
			}

			if (ParsingSequences.IdentifierString.TryParse(fieldName) is not IResult<string> { WasSuccessful: true } result)
				throw ChipsCompiler.ErrorAndThrow(new ParsingException($"Invalid field name string: {fieldName.DesanitizeString()}"));

			Name = result.Value;

			ChipsCompiler.CompilingSourceLineOverride = ChipsCompiler.CompilingSourceLine;

			string separator = reader.ReadWord();
			if (separator != ":")
				throw ChipsCompiler.ErrorAndThrow(new ParsingException($"Token \":\" was expected for field definition, found \"{separator.DesanitizeString()}\" instead"));

			ChipsCompiler.CompilingSourceLineOverride = ChipsCompiler.CompilingSourceLine;

			string typeName = reader.ReadUntilNewline();

			TypeDefinition fieldType = StringSerialization.ParseTypeIdentifierArgument(context, typeName, false);

			Segment = new BytecodeFieldSegment(Name, fieldType, attributes);
			((TypeBody)Previous!).GetScope().Segment.AddMember(Segment);

			// Go to the previous state
			next = Previous;
			return true;
		}
	}

	internal sealed class MethodIdentifier : BaseState {
		public bool hasDefinedLocals;

		public string Name { get; private set; }

		public BytecodeMethodSegment Segment { get; private set; }

		public CPDBFileMethodSegment MethodSymbol { get; private set; }

		public override bool ParseNext(StreamReader reader, CompilationContext context, BytecodeFile code, out BaseState? next) {
			ChipsCompiler.CompilingSourceLineOverride = ChipsCompiler.CompilingSourceLine;

			if (reader.ReadWord() != ".method") {
				next = null;
				return false;
			}

			ChipsCompiler.CompilingSourceLineOverride = ChipsCompiler.CompilingSourceLine;

			string methodName = reader.ReadWord();

			if (ParsingSequences.IdentifierString.TryParse(methodName) is not IResult<string> { WasSuccessful: true } result)
				throw ChipsCompiler.ErrorAndThrow(new ParsingException($"Invalid method name string: {methodName.DesanitizeString()}"));

			Name = result.Value;

			ChipsCompiler.CompilingSourceLineOverride = ChipsCompiler.CompilingSourceLine;

			string args = reader.ReadWordsUntil(0, true, "->");

			int index = args.LastIndexOf(')');
			if (index < 0)
				throw ChipsCompiler.ErrorAndThrow(new ParsingException($"Function arguments were malformed, could not parse: {args.DesanitizeString()}"));

			string argTypes = args[..index];
			string methodAttributes = args[(index + 1)..];

			if (ParsingSequences.FunctionArguments.TryParse(argTypes) is not IResult<ParsedMethodVariable[]> { WasSuccessful: true } argsResult)
				throw ChipsCompiler.ErrorAndThrow(new ParsingException($"Function arguments were malformed, could not parse: {args.DesanitizeString()}"));

			ParsedMethodVariable[] parsedArgs = argsResult.Value;

			if (ParsingSequences.MethodAccessModifiers.TryParse(methodAttributes) is not IResult<MethodAttributes> { WasSuccessful: true } attributesResult)
				throw ChipsCompiler.ErrorAndThrow(new ParsingException(methodAttributes.Length > 0 ? $"Invalid access modifiers detected: {methodAttributes.DesanitizeString()}" : "No method attributes were specified"));

			MethodAttributes attributes = attributesResult.Value;

			ChipsCompiler.CompilingSourceLineOverride = ChipsCompiler.CompilingSourceLine;

			string returnSpecifier = reader.ReadWord();
			if (returnSpecifier != "->")
				throw ChipsCompiler.ErrorAndThrow(new ParsingException($"Token \"->\" was expected for method definition, found \"{returnSpecifier.DesanitizeString()}\" instead"));

			ChipsCompiler.CompilingSourceLineOverride = ChipsCompiler.CompilingSourceLine;

			string returnType = reader.ReadUntil('{');
			if (ParsingSequences.VariableType.TryParse(returnType) is not IResult<string> { WasSuccessful: true } returnTypeResult)
				throw ChipsCompiler.ErrorAndThrow(new ParsingException($"Invalid return type: {returnType.DesanitizeString()}"));

			TypeDefinition returnTypeDefinition = StringSerialization.ParseTypeIdentifierArgument(context, returnTypeResult.Value, false);

			Segment = new BytecodeMethodSegment(Name, attributes, returnTypeDefinition);

			foreach (ParsedMethodVariable methodArg in parsedArgs) {
				TypeDefinition argType = StringSerialization.ParseTypeIdentifierArgument(context, methodArg.type, true);

				Segment.parameters.Add(new BytecodeVariableSegment(methodArg.name, argType));
			}

			var typeScope = ((TypeBody)Previous!).GetScope();

			typeScope.Segment.AddMember(Segment);

			MethodSymbol = typeScope.TypeSymbol.AddMethod(Name);

			next = Scope.Create(this, new MethodBody());
			return true;
		}
	}

	internal class MethodBody : MultipleStates {
		public bool activeLabel;

		protected override IEnumerable<BaseState> EnumeratePossibleStates() {
			var method = GetMethod();

			if (!method.hasDefinedLocals) {
				yield return new Comment();
				yield return new LocalsListIdentifier();
				yield return new LabelIdentifier();
				yield return new InstructionIdentifier();
				yield return new Empty();
			} else {
				yield return new Comment();
				yield return new LabelIdentifier();
				yield return new InstructionIdentifier();
				yield return new Empty();
			}
		}

		public override bool ParseNext(StreamReader reader, CompilationContext context, BytecodeFile code, out BaseState? next) {
			bool success = base.ParseNext(reader, context, code, out _);

			if (success) {
				// Attempt to parse more things in this state
				next = this;
			} else {
				if (activeLabel)
					throw ChipsCompiler.ErrorAndThrow(new ParsingException("Method body ended with a label identifier, but no instruction was found after it"));

				// Go to the previous state
				next = Previous;
			}

			return true;
		}

		public MethodIdentifier GetMethod() => (MethodIdentifier)Previous!;
	}

	internal class LocalsListIdentifier : BaseState {
		public override bool ParseNext(StreamReader reader, CompilationContext context, BytecodeFile code, out BaseState? next) {
			ChipsCompiler.CompilingSourceLineOverride = ChipsCompiler.CompilingSourceLine;

			if (reader.ReadWord() != ".locals") {
				next = null;
				return false;
			}

			next = Scope.Create(this, new LocalsList());
			return true;
		}

		public MethodIdentifier GetMethod() => ((MethodBody)Previous!).GetMethod();
	}

	internal class LocalsList : MultipleStates {
		public int definedLocals;
		public bool previousLocalHadComma;

		protected override IEnumerable<BaseState> EnumeratePossibleStates() {
			yield return new Comment();
			yield return new LocalIdentifier();
			yield return new Empty();
		}

		public override bool ParseNext(StreamReader reader, CompilationContext context, BytecodeFile code, out BaseState? next) {
			var method = GetMethod();

			if (method.hasDefinedLocals)
				throw ChipsCompiler.ErrorAndThrow(new ParsingException("Local variables were already defined for this method"));

			if (method.Segment.body.Instructions.Count > 0)
				throw ChipsCompiler.ErrorAndThrow(new ParsingException("Local variables must be defined before any instructions"));

			if (method.Segment.body.Labels.Count > 0)
				throw ChipsCompiler.ErrorAndThrow(new ParsingException("Local variables must be defined before any labels"));

			return base.ParseNext(reader, context, code, out next);
		}

		public MethodIdentifier GetMethod() => ((LocalsListIdentifier)Previous!).GetMethod();
	}

	internal class LocalIdentifier : BaseState {
		public override bool ParseNext(StreamReader reader, CompilationContext context, BytecodeFile code, out BaseState? next) {
			ChipsCompiler.CompilingSourceLineOverride = ChipsCompiler.CompilingSourceLine;

			// Read words until a comma or the end of the locals list is reached
			// Max word length:  name : type,
			string words = reader.ReadUntilMany(new char[] { ',', '}' }, alwaysConsume: false);

			var list = GetList();

			// Only consume the comma if it exists
			if (reader.Peek() == ',') {
				reader.Read();
				list.previousLocalHadComma = true;
			} else
				list.previousLocalHadComma = false;

			if (ParsingSequences.FunctionLocal.TryParse(words) is not IResult<ParsedMethodVariable> { WasSuccessful: true } result)
				throw ChipsCompiler.ErrorAndThrow(new ParsingException($"Invalid local variable definition: {words.DesanitizeString()}"));

			if (list.definedLocals > 0 && !list.previousLocalHadComma)
				throw ChipsCompiler.ErrorAndThrow(new ParsingException("Local variables must be separated by commas"));

			ParsedMethodVariable parsedVariable = result.Value;

			TypeDefinition localType = StringSerialization.ParseTypeIdentifierArgument(context, parsedVariable.type, true);

			var method = GetMethod();
			method.Segment.locals.Add(new BytecodeVariableSegment(parsedVariable.name, localType));

			list.definedLocals++;

			next = Previous;
			return true;
		}

		public LocalsList GetList() => (LocalsList)Previous!;

		public MethodIdentifier GetMethod() => ((LocalsList)Previous!).GetMethod();
	}

	internal class LabelIdentifier : BaseState {
		public override bool ParseNext(StreamReader reader, CompilationContext context, BytecodeFile code, out BaseState? next) {
			string identifier = reader.ReadUntil(':', alwaysConsume: true);

			if (ParsingSequences.IdentifierString.TryParse(identifier) is not IResult<string> { WasSuccessful: true } result)
				throw ChipsCompiler.ErrorAndThrow(new ParsingException($"Invalid label identifier: {identifier.DesanitizeString()}"));

			string name = result.Value;

			var method = GetMethod();
			method.activeLabel = true;

			var scope = method.GetMethod();

			var label = scope.Segment.body.ReserveLabel(name);
			label.OpcodeOffset = scope.Segment.body.Instructions.Count;

			scope.MethodSymbol.AddLabel(scope.Name, label);

			next = Previous;
			return true;
		}

		public MethodBody GetMethod() => (MethodBody)Previous!;
	}

	internal class InstructionIdentifier : BaseState {
		public override bool ParseNext(StreamReader reader, CompilationContext context, BytecodeFile code, out BaseState? next) {
			string opcode = reader.ReadWord();

			if (ParsingSequences.Opcode.TryParse(opcode) is not IResult<string> { WasSuccessful: true } result)
				throw ChipsCompiler.ErrorAndThrow(new ParsingException($"Invalid opcode identifier: {opcode.DesanitizeString()}"));

			opcode = result.Value;

			var possibleOpcodes = ChipsCompiler.FindOpcode(opcode);

			if (!possibleOpcodes.Any())
				throw ChipsCompiler.ErrorAndThrow(new ParsingException($"Unknown opcode: {opcode.DesanitizeString()}"));

			string[] arguments = reader.ReadManyWordsOrQuotedStrings(preprocessEscapedQuotes: true, terminateOnComment: true)
				.Select(static p => p.wasQuoted ? $"\"{p.text}\"" : p.text)
				.ToArray();

			var method = GetMethod();
			var scope = method.GetMethod();
			var body = scope.Segment.body.Instructions;

			bool anySuccess = false;
			foreach (Opcode possibleOpcode in possibleOpcodes) {
				int count = ChipsCompiler.ErrorCount;
				try {
					var args = possibleOpcode.ParseArguments(context, arguments) ?? new();

					ChipsInstruction instr = new ChipsInstruction(possibleOpcode.Code, args);
					body.Add(instr);
					anySuccess = true;
					break;
				} catch {
					// Consume the error
					ChipsCompiler.RestoreExceptionState(count);
				}
			}

			if (!anySuccess)
				throw ChipsCompiler.ErrorAndThrow(new ParsingException($"No definition of opcode \"{opcode}\" could parse the provided arguments"));

			next = Previous;
			return true;
		}

		public MethodBody GetMethod() => (MethodBody)Previous!;
	}
}
