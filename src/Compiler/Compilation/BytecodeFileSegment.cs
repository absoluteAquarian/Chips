using AsmResolver.DotNet;
using AsmResolver.PE.DotNet.Metadata.Tables.Rows;
using Chips.Utility;
using System.Collections.Generic;
using System.IO;

namespace Chips.Compiler.Compilation {
	internal abstract class BytecodeFileSegment {
		public readonly BytecodeMember memberType;

		public BytecodeFileSegment(BytecodeMember memberType) {
			this.memberType = memberType;
		}

		public abstract void WriteMember(CompilationContext context, BinaryWriter writer);

		public void WriteSegmentIdentifier(BinaryWriter writer) => writer.Write((byte)memberType);
	}

	internal abstract class BytecodeTypeMemberSegment : BytecodeFileSegment {
		protected BytecodeTypeMemberSegment(BytecodeMember memberType) : base(memberType) { }
	}

	internal sealed class BytecodeNamespaceSegment : BytecodeFileSegment {
		public readonly string name;

		private readonly List<BytecodeTypeSegment> definedTypes = new();

		public BytecodeNamespaceSegment(string name) : base(BytecodeMember.Namespace) {
			this.name = name;
		}

		public void AddType(BytecodeTypeSegment type) => definedTypes.Add(type);

		public override void WriteMember(CompilationContext context, BinaryWriter writer) {
			writer.Write(name);
			writer.Write7BitEncodedInt(definedTypes.Count);
			foreach (var type in definedTypes) {
				type.WriteSegmentIdentifier(writer);
				type.WriteMember(context, writer);
			}
		}
	}

	internal sealed class BytecodeTypeSegment : BytecodeTypeMemberSegment {
		public readonly BytecodeTypeSegment enclosingType;
		public readonly string name;
		public readonly TypeAttributes attributes;

		private readonly List<BytecodeTypeMemberSegment> definedMembers = new();

		public BytecodeTypeSegment(BytecodeTypeSegment enclosingType, string name, TypeAttributes attributes) : base(BytecodeMember.Type) {
			this.enclosingType = enclosingType;
			this.name = name;
			this.attributes = attributes;
		}

		public void AddMember(BytecodeTypeMemberSegment member) => definedMembers.Add(member);

		public override void WriteMember(CompilationContext context, BinaryWriter writer) {
			writer.Write(name);
			writer.Write((uint)attributes);
			writer.Write7BitEncodedInt(definedMembers.Count);
			foreach (var member in definedMembers) {
				member.WriteSegmentIdentifier(writer);
				member.WriteMember(context, writer);
			}
		}
	}

	internal sealed class BytecodeFieldSegment : BytecodeTypeMemberSegment {
		public readonly BytecodeTypeSegment declaringType;
		public readonly string name;
		public readonly TypeDefinition type;
		public readonly TypeAttributes attributes;

		public BytecodeFieldSegment(string name, TypeDefinition type, TypeAttributes attributes) : base(BytecodeMember.Field) {
			this.name = name;
			this.type = type;
			this.attributes = attributes;
		}

		public override void WriteMember(CompilationContext context, BinaryWriter writer) {
			writer.Write(name);
			writer.Write(type, context.resolver);
			writer.Write((uint)attributes);
		}
	}

	internal sealed class BytecodeMethodSegment : BytecodeTypeMemberSegment {
		public readonly string name;
		public readonly TypeAttributes attributes;
		public readonly TypeDefinition returnType;

		public readonly List<BytecodeVariableSegment> parameters = new();
		public readonly List<BytecodeVariableSegment> locals = new();
		public readonly BytecodeMethodBody body;

		public BytecodeMethodSegment(string name, TypeAttributes attributes, TypeDefinition returnType) : base(BytecodeMember.Method) {
			this.name = name;
			this.attributes = attributes;
			this.returnType = returnType;
			body = new(this);
		}

		public override void WriteMember(CompilationContext context, BinaryWriter writer) {
			writer.Write(name);
			writer.Write((uint)attributes);
			writer.Write(returnType, context.resolver);

			writer.Write7BitEncodedInt(parameters.Count);
			foreach (var parameter in parameters)
				parameter.WriteMember(context, writer);

			writer.Write7BitEncodedInt(locals.Count);
			foreach (var local in locals)
				local.WriteMember(context, writer);
			
			writer.Write7BitEncodedInt(body.Labels.Count);
			foreach (var label in body.Labels)
				label.WriteMember(context, writer);
		}
	}

	internal sealed class BytecodeVariableSegment : BytecodeFileSegment {
		public readonly string name;
		public readonly TypeDefinition type;

		public BytecodeVariableSegment(string name, TypeDefinition type) : base(BytecodeMember.Variable) {
			this.name = name;
			this.type = type;
		}

		public override void WriteMember(CompilationContext context, BinaryWriter writer) {
			writer.Write(name);
			writer.Write(type, context.resolver);
		}
	}

	internal sealed class BytecodeTypeAliasSegment : BytecodeFileSegment {
		public readonly string alias;
		public readonly TypeDefinition resolvedAlias;

		public BytecodeTypeAliasSegment(string alias, TypeDefinition resolvedAlias) : base(BytecodeMember.TypeAlias) {
			this.alias = alias;
			this.resolvedAlias = resolvedAlias;
		}

		public override void WriteMember(CompilationContext context, BinaryWriter writer) {
			writer.Write(alias);
			writer.Write(resolvedAlias, context.resolver);
		}
	}

	internal sealed class BytecodeNamespaceImportSegment : BytecodeFileSegment {
		public readonly string importedNamespace;
		public readonly string assemblyName;
		public readonly AssemblyDefinition sourceAssembly;

		public BytecodeNamespaceImportSegment(string importedNamespace, string assemblyName, AssemblyDefinition sourceAssembly) : base(BytecodeMember.ExternNamespace) {
			this.importedNamespace = importedNamespace;
			this.assemblyName = assemblyName;
			this.sourceAssembly = sourceAssembly;
		}

		public override void WriteMember(CompilationContext context, BinaryWriter writer) {
			writer.Write(importedNamespace);
			writer.Write(assemblyName);
		}
	}
}
