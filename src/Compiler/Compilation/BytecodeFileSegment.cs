using AsmResolver.DotNet;
using AsmResolver.PE.DotNet.Metadata.Tables.Rows;
using Chips.Compiler.Utility;
using Chips.Utility;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Chips.Compiler.Compilation {
	internal abstract class BytecodeFileSegment {
		public readonly BytecodeMember memberType;

		public BytecodeFileSegment(BytecodeMember memberType) {
			this.memberType = memberType;
		}

		public abstract void WriteMember(CompilationContext context, BinaryWriter writer);

		public void WriteSegmentIdentifier(BinaryWriter writer) => writer.Write((byte)memberType);

		public static BytecodeMember ReadSegmentIdentifier(BinaryReader reader) => (BytecodeMember)reader.ReadByte();
	}

	internal abstract class BytecodeTopLevelSegment : BytecodeFileSegment {
		protected BytecodeTopLevelSegment(BytecodeMember memberType) : base(memberType) { }
	}

	internal abstract class BytecodeTypeMemberSegment : BytecodeFileSegment {
		protected BytecodeTypeMemberSegment(BytecodeMember memberType) : base(memberType) { }
	}

	internal sealed class BytecodeNamespaceSegment : BytecodeTopLevelSegment {
		public readonly string name;

		private readonly List<BytecodeFileSegment> members = new();

		public BytecodeNamespaceSegment(string name) : base(BytecodeMember.Namespace) {
			this.name = name;
		}

		public void AddNamespace(BytecodeNamespaceSegment ns) => members.Add(ns);

		public void AddType(BytecodeTypeSegment type) => members.Add(type);

		public static BytecodeNamespaceSegment ReadMember(CompilationContext context, BinaryReader reader, object? state) {
			string name = context.heap.ReadString(reader);
			string fullNamespace = state is string s ? $"{s}.{name}" : name;

			BytecodeNamespaceSegment segment = new(fullNamespace);

			int typeCount = reader.Read7BitEncodedInt();
			for (int i = 0; i < typeCount; i++) {
				BytecodeMember memberType = ReadSegmentIdentifier(reader);

				switch (memberType) {
					case BytecodeMember.Namespace:
						segment.AddNamespace(ReadMember(context, reader, fullNamespace));
						break;
					case BytecodeMember.Type:
						segment.AddType(BytecodeTypeSegment.ReadMember(context, reader, segment));
						break;
					default:
						throw new InvalidDataException($"Invalid member type {memberType} in namespace \"{name}\"");
				}
			}

			return segment;
		}

		public override void WriteMember(CompilationContext context, BinaryWriter writer) {
			context.heap.WriteString(writer, name);
			writer.Write7BitEncodedInt(members.Count);
			foreach (var member in members) {
				member.WriteSegmentIdentifier(writer);
				member.WriteMember(context, writer);
			}
		}
	}

	internal sealed class BytecodeTypeSegment : BytecodeTypeMemberSegment {
		public readonly BytecodeNamespaceSegment enclosingNamespace;
		public readonly BytecodeTypeSegment? enclosingType;
		public readonly string name;
		public readonly TypeAttributes attributes;

		public readonly ITypeDefOrRef baseType;

		public string FullName { get; }

		private readonly List<BytecodeTypeMemberSegment> definedMembers = new();

		public BytecodeTypeSegment(BytecodeNamespaceSegment enclosingNamespace, BytecodeTypeSegment? enclosingType, string name, TypeAttributes attributes, ITypeDefOrRef baseType) : base(BytecodeMember.Type) {
			this.enclosingNamespace = enclosingNamespace;
			this.enclosingType = enclosingType;
			this.name = name;
			this.attributes = attributes;
			this.baseType = baseType;

			FullName = ResolveFullName();
		}

		public void AddMember(BytecodeTypeMemberSegment member) => definedMembers.Add(member);

		public static BytecodeTypeSegment ReadMember(CompilationContext context, BinaryReader reader, object? state) {
			string name = context.heap.ReadString(reader);
			TypeAttributes attributes = (TypeAttributes)reader.ReadUInt32();
			DelayedTypeResolver baseType = reader.ReadTypeDefinition(context.resolver, context.heap);
			ChipsCompiler.AddDelayedResolver(baseType);

			BytecodeNamespaceSegment enclosingNamespace = state switch {
				BytecodeNamespaceSegment ns => ns,
				BytecodeTypeSegment type => type.enclosingNamespace,
				_ => throw new InvalidDataException("Invalid state for type segment")
			};

			BytecodeTypeSegment segment = new(enclosingNamespace, state as BytecodeTypeSegment, name, attributes, baseType);

			int memberCount = reader.Read7BitEncodedInt();
			for (int i = 0; i < memberCount; i++) {
				BytecodeMember memberType = ReadSegmentIdentifier(reader);

				switch (memberType) {
					case BytecodeMember.Type:
						segment.AddMember(ReadMember(context, reader, segment));
						break;
					case BytecodeMember.Field:
						segment.AddMember(BytecodeFieldSegment.ReadMember(context, reader, segment));
						break;
					case BytecodeMember.Method:
						segment.AddMember(BytecodeMethodSegment.ReadMember(context, reader, segment));
						break;
					default:
						throw new InvalidDataException($"Invalid member type {memberType} in type \"{segment.FullName}\"");
				}
			}

			return segment;
		}

		public override void WriteMember(CompilationContext context, BinaryWriter writer) {
			context.heap.WriteString(writer, name);
			writer.Write((uint)attributes);
			writer.Write(baseType, context.heap);
			writer.Write7BitEncodedInt(definedMembers.Count);
			foreach (var member in definedMembers) {
				member.WriteSegmentIdentifier(writer);
				member.WriteMember(context, writer);
			}
		}

		private string ResolveFullName() {
			StringBuilder sb = new StringBuilder(enclosingNamespace.name);
			sb.Append('.');
			
			Stack<BytecodeTypeSegment> typeStack = new();
			typeStack.Push(this);

			BytecodeTypeSegment? current = this;
			while (current.enclosingType is not null) {
				typeStack.Push(current.enclosingType);
				current = current.enclosingType;
			}

			while (typeStack.TryPop(out current)) {
				sb.Append(current.name);

				if (typeStack.Count > 0)
					sb.Append('+');
			}

			return sb.ToString();
		}
	}

	internal sealed class BytecodeFieldSegment : BytecodeTypeMemberSegment {
		public readonly BytecodeTypeSegment declaringType;
		public readonly string name;
		public readonly ITypeDefOrRef type;
		public readonly FieldAttributes attributes;

		public BytecodeFieldSegment(BytecodeTypeSegment declaringType, string name, ITypeDefOrRef type, FieldAttributes attributes) : base(BytecodeMember.Field) {
			this.declaringType = declaringType;
			this.name = name;
			this.type = type;
			this.attributes = attributes;
		}

		public static BytecodeFieldSegment ReadMember(CompilationContext context, BinaryReader reader, BytecodeTypeSegment declaringType) {
			string name = context.heap.ReadString(reader);
			DelayedTypeResolver type = reader.ReadTypeDefinition(context.resolver, context.heap);
			ChipsCompiler.AddDelayedResolver(type);
			FieldAttributes attributes = (FieldAttributes)reader.ReadUInt32();

			return new BytecodeFieldSegment(declaringType, name, type, attributes);
		}

		public override void WriteMember(CompilationContext context, BinaryWriter writer) {
			context.heap.WriteString(writer, name);
			writer.Write(type, context.heap);
			writer.Write((uint)attributes);
		}
	}

	internal sealed class BytecodeMethodSegment : BytecodeTypeMemberSegment {
		public readonly BytecodeTypeSegment declaringType;
		public readonly string name;
		public readonly MethodAttributes attributes;
		public readonly ITypeDefOrRef returnType;

		public bool IsEntryPoint => object.ReferenceEquals(ChipsCompiler.FoundEntryPoint, this);

		public readonly List<BytecodeVariableSegment> parameters = new();
		public readonly List<BytecodeVariableSegment> locals = new();
		public readonly BytecodeMethodBody body;

		public BytecodeMethodSegment(BytecodeTypeSegment declaringType, string name, MethodAttributes attributes, ITypeDefOrRef returnType) : base(BytecodeMember.Method) {
			this.declaringType = declaringType;
			this.name = name;
			this.attributes = attributes;
			this.returnType = returnType;
			body = new(this);
		}

		public static BytecodeMethodSegment ReadMember(CompilationContext context, BinaryReader reader, BytecodeTypeSegment declaringType) {
			string name = context.heap.ReadString(reader);
			MethodAttributes attributes = (MethodAttributes)reader.ReadUInt32();
			DelayedTypeResolver returnType = reader.ReadTypeDefinition(context.resolver, context.heap);
			ChipsCompiler.AddDelayedResolver(returnType);

			BytecodeMethodSegment segment = new(declaringType, name, attributes, returnType);

			int parameterCount = reader.Read7BitEncodedInt();
			for (int i = 0; i < parameterCount; i++)
				segment.parameters.Add(BytecodeVariableSegment.ReadMember(context, reader));

			// Attempt to set the entry point
			var factory = ChipsCompiler.ManifestModule.CorLibTypeFactory;
			var returnTypeName = (returnType as IFullNameProvider).FullName;
			if (!ChipsCompiler.NoEntryPoint && name == "Main" && (returnTypeName == factory.Void.FullName || returnTypeName == "void" || returnTypeName == factory.Int32.FullName || returnTypeName == "int") && (parameterCount == 0 || (parameterCount == 1 && segment.parameters[0].type.FullName == factory.String.MakeArrayType().FullName))) {
				if (ChipsCompiler.FoundEntryPoint is not null)
					throw new InvalidDataException("Multiple entry points found");

				ChipsCompiler.FoundEntryPoint = segment;
			}

			int localCount = reader.Read7BitEncodedInt();
			for (int i = 0; i < localCount; i++)
				segment.locals.Add(BytecodeVariableSegment.ReadMember(context, reader));

			int labelCount = reader.Read7BitEncodedInt();
			for (int i = 0; i < labelCount; i++) {
				var label = ChipsLabel.ReadMember(reader);
				label.Index = i;
				segment.body.Labels.Add(label);
			}

			int instructionCount = reader.Read7BitEncodedInt();
			for (int i = 0; i < instructionCount; i++)
				segment.body.Instructions.Add(ChipsInstruction.Read(reader, context.resolver, context.heap));

			return segment;
		}

		public override void WriteMember(CompilationContext context, BinaryWriter writer) {
			context.heap.WriteString(writer, name);
			writer.Write((uint)attributes);
			writer.Write(returnType, context.heap);

			writer.Write7BitEncodedInt(parameters.Count);
			foreach (var parameter in parameters)
				parameter.WriteMember(context, writer);

			writer.Write7BitEncodedInt(locals.Count);
			foreach (var local in locals)
				local.WriteMember(context, writer);
			
			writer.Write7BitEncodedInt(body.Labels.Count);
			foreach (var label in body.Labels)
				label.WriteMember(writer);

			writer.Write7BitEncodedInt(body.Instructions.Count);
			foreach (var instruction in body.Instructions)
				instruction.Write(writer, context.resolver, context.heap);
		}
	}

	internal sealed class BytecodeVariableSegment : BytecodeFileSegment {
		public readonly string name;
		public readonly ITypeDefOrRef type;

		public BytecodeVariableSegment(string name, ITypeDefOrRef type) : base(BytecodeMember.Variable) {
			this.name = name;
			this.type = type;
		}

		public static BytecodeVariableSegment ReadMember(CompilationContext context, BinaryReader reader) {
			string name = context.heap.ReadString(reader);
			DelayedTypeResolver type = reader.ReadTypeDefinition(context.resolver, context.heap);
			ChipsCompiler.AddDelayedResolver(type);

			return new BytecodeVariableSegment(name, type);
		}

		public override void WriteMember(CompilationContext context, BinaryWriter writer) {
			context.heap.WriteString(writer, name);
			writer.Write(type, context.heap);
		}
	}

	internal sealed class BytecodeTypeAliasSegment : BytecodeTopLevelSegment {
		public readonly string alias;
		public readonly ITypeDefOrRef resolvedAlias;

		public BytecodeTypeAliasSegment(string alias, ITypeDefOrRef resolvedAlias) : base(BytecodeMember.TypeAlias) {
			this.alias = alias;
			this.resolvedAlias = resolvedAlias;
		}

		public static BytecodeTypeAliasSegment ReadMember(CompilationContext context, BinaryReader reader) {
			string alias = context.heap.ReadString(reader);
			DelayedTypeResolver resolvedAlias = reader.ReadTypeDefinition(context.resolver, context.heap);
			ChipsCompiler.AddDelayedResolver(resolvedAlias);

			return new BytecodeTypeAliasSegment(alias, resolvedAlias);
		}

		public override void WriteMember(CompilationContext context, BinaryWriter writer) {
			context.heap.WriteString(writer, alias);
			writer.Write(resolvedAlias, context.heap);
		}
	}

	internal sealed class BytecodeNamespaceImportSegment : BytecodeTopLevelSegment {
		public readonly string importedNamespace;

		public BytecodeNamespaceImportSegment(string importedNamespace) : base(BytecodeMember.ExternNamespace) {
			this.importedNamespace = importedNamespace;
		}

		public static BytecodeNamespaceImportSegment ReadMember(CompilationContext context, BinaryReader reader) {
			string importedNamespace = context.heap.ReadString(reader);

			return new BytecodeNamespaceImportSegment(importedNamespace);
		}

		public override void WriteMember(CompilationContext context, BinaryWriter writer) {
			context.heap.WriteString(writer, importedNamespace);
		}
	}
}
