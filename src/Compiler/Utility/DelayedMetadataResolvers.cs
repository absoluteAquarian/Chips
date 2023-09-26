using AsmResolver;
using AsmResolver.DotNet;
using AsmResolver.DotNet.Signatures;
using AsmResolver.DotNet.Signatures.Types;
using AsmResolver.PE.DotNet.Metadata.Tables;
using Chips.Utility;
using System;
using System.Collections.Generic;
using System.IO;

namespace Chips.Compiler.Utility {
	public interface IDelayedResolver {
		void Resolve(CompilationContext context);
	}

	public abstract class BaseDelayedMetadataResolver<T> : IDelayedResolver where T : class, IMetadataMember {
		private readonly TypeResolverSnapshot snapshot;
		public readonly string metadata;

		public readonly int sourceLine;
		public readonly string sourceFile;

		private T _resolvedMember;
		public T Member {
			get {
				return _resolvedMember ?? throw new Exception("Member was not resolved");
			}
		}

		protected BaseDelayedMetadataResolver(T member) {
			snapshot = default;
			metadata = null!;
			sourceLine = -1;
			sourceFile = null!;
			_resolvedMember = member;
		}

		protected BaseDelayedMetadataResolver(TypeResolver resolver, string metadata) {
			snapshot = resolver.GetSnapshot();
			this.metadata = metadata;
			sourceLine = ChipsCompiler.CompilingSourceLineOverride ?? ChipsCompiler.CompilingSourceLine;
			sourceFile = ChipsCompiler.CompilingSourceFile;
		}

		protected abstract T ResolveMember(CompilationContext context);

		public void Resolve(CompilationContext context) {
			if (!snapshot.IsValid) {
				if (_resolvedMember is null)
					throw ChipsCompiler.ErrorAndThrow(new NullReferenceException("Assigned member was null"));
			} else if (_resolvedMember is null) {
				int? line = ChipsCompiler.CompilingSourceLineOverride;
				string file = ChipsCompiler.CompilingSourceFile;
				ChipsCompiler.CompilingSourceLineOverride = sourceLine;
				ChipsCompiler.CompilingSourceFile = sourceFile;

				context.resolver.RestoreSnapshot(snapshot);

				_resolvedMember ??= ResolveMember(context) ?? throw ChipsCompiler.ErrorAndThrow(new InvalidDataException($"Metadata for \"{metadata.DesanitizeString()}\" could not be resolved"));

				ChipsCompiler.CompilingSourceLineOverride = line;
				ChipsCompiler.CompilingSourceFile = file;
			}
		}
	}

	public sealed class DelayedTypeResolver : BaseDelayedMetadataResolver<TypeDefinition>, ITypeDefOrRef {
		private DelayedTypeResolver(TypeDefinition def) : base(def) { }

		public DelayedTypeResolver(TypeResolver resolver, string metadata) : base(resolver, metadata) { }

		public static DelayedTypeResolver FromKnownDefinition(TypeDefinition def) {
			return new(def);
		}

		protected override TypeDefinition ResolveMember(CompilationContext context) {
			return StringSerialization.ParseTypeIdentifierArgument(context, metadata, false);
		}

		#region ITypeDefOrRef
		Utf8String? ITypeDefOrRef.Name => (Member as ITypeDefOrRef).Name;
		string? INameProvider.Name => (Member as INameProvider).Name;
		Utf8String? ITypeDefOrRef.Namespace => (Member as ITypeDefOrRef).Namespace;
		string? ITypeDescriptor.Namespace => (Member as ITypeDescriptor).Namespace;
		ITypeDefOrRef? ITypeDefOrRef.DeclaringType => (Member as ITypeDefOrRef).DeclaringType;
		ITypeDescriptor? IMemberDescriptor.DeclaringType => (Member as IMemberDescriptor).DeclaringType;
		IResolutionScope? ITypeDescriptor.Scope => (Member as ITypeDescriptor).Scope;
		bool ITypeDescriptor.IsValueType => (Member as ITypeDescriptor).IsValueType;
		string IFullNameProvider.FullName => (Member as IFullNameProvider)?.FullName ?? metadata;  // Null case included due to Binary I/O reading FullName
		ModuleDefinition? IModuleProvider.Module => (Member as IModuleProvider).Module;
		IList<CustomAttribute> IHasCustomAttribute.CustomAttributes => (Member as IHasCustomAttribute).CustomAttributes;
		MetadataToken IMetadataMember.MetadataToken => (Member as IMetadataMember).MetadataToken;

		ITypeDefOrRef ITypeDefOrRef.ImportWith(ReferenceImporter importer) {
			return (Member as ITypeDefOrRef).ImportWith(importer);
		}

		IImportable IImportable.ImportWith(ReferenceImporter importer) {
			return (Member as IImportable).ImportWith(importer);
		}

		bool IImportable.IsImportedInModule(ModuleDefinition module) {
			return (Member as IImportable).IsImportedInModule(module);
		}

		TypeDefinition? ITypeDescriptor.Resolve() {
			return (Member as ITypeDescriptor).Resolve();
		}

		IMemberDefinition? IMemberDescriptor.Resolve() {
			return (Member as IMemberDescriptor).Resolve();
		}

		ITypeDefOrRef ITypeDescriptor.ToTypeDefOrRef() {
			return (Member as ITypeDescriptor).ToTypeDefOrRef();
		}

		TypeSignature ITypeDefOrRef.ToTypeSignature(bool isValueType) {
			return (Member as ITypeDefOrRef).ToTypeSignature(isValueType);
		}

		TypeSignature ITypeDescriptor.ToTypeSignature() {
			return (Member as ITypeDescriptor).ToTypeSignature();
		}
		#endregion
	}

	public sealed class DelayedMethodResolver : BaseDelayedMetadataResolver<MethodDefinition>, IMethodDescriptor {
		private DelayedMethodResolver(MethodDefinition def) : base(def) { }

		public DelayedMethodResolver(TypeResolver resolver, string metadata) : base(resolver, metadata) { }

		public static DelayedMethodResolver FromKnownDefinition(MethodDefinition def) {
			return new(def);
		}

		protected override MethodDefinition ResolveMember(CompilationContext context) {
			return StringSerialization.ParseMethodIdentifierArgument(context, metadata);
		}

		#region IMethodDescriptor
		ITypeDescriptor? IMemberDescriptor.DeclaringType => (Member as IMemberDescriptor).DeclaringType;
		Utf8String? IMethodDescriptor.Name => (Member as IMethodDescriptor).Name;
		string? INameProvider.Name => (Member as INameProvider).Name;
		MethodSignature? IMethodDescriptor.Signature => (Member as IMethodDescriptor).Signature;
		string IFullNameProvider.FullName => (Member as IFullNameProvider).FullName;
		ModuleDefinition? IModuleProvider.Module => (Member as IModuleProvider).Module;
		MetadataToken IMetadataMember.MetadataToken => (Member as IMetadataMember).MetadataToken;

		IImportable IImportable.ImportWith(ReferenceImporter importer) {
			return (Member as IImportable).ImportWith(importer);
		}

		bool IImportable.IsImportedInModule(ModuleDefinition module) {
			return (Member as IImportable).IsImportedInModule(module);
		}

		MethodDefinition? IMethodDescriptor.Resolve() {
			return (Member as IMethodDescriptor).Resolve();
		}

		IMemberDefinition? IMemberDescriptor.Resolve() {
			return (Member as IMemberDescriptor).Resolve();
		}
		#endregion
	}

	public sealed class DelayedFieldResolver : BaseDelayedMetadataResolver<FieldDefinition>, IFieldDescriptor {
		public readonly string typeMetadata;
		public readonly string fieldMetadata;

		private DelayedFieldResolver(FieldDefinition def) : base(def) {
			if (def.DeclaringType is null)
				throw new ArgumentException("Field must have a declaring type", nameof(def));
			if (def.Name is null)
				throw new ArgumentException("Field must have a name", nameof(def));

			typeMetadata = def.DeclaringType.FullName;
			fieldMetadata = def.Name;
		}

		public DelayedFieldResolver(TypeResolver resolver, string metadata) : base(resolver, metadata) {
			StringSerialization.ExtractFieldInformation(metadata, out typeMetadata, out fieldMetadata);
		}

		public static DelayedFieldResolver FromKnownDefinition(FieldDefinition def) {
			return new(def);
		}

		protected override FieldDefinition ResolveMember(CompilationContext context) {
			return StringSerialization.ParseFieldIdentifierArgument(context, metadata);
		}

		#region IFieldDescriptor
		Utf8String? IFieldDescriptor.Name => (Member as IFieldDescriptor).Name;
		FieldSignature? IFieldDescriptor.Signature => (Member as IFieldDescriptor).Signature;
		ITypeDescriptor? IMemberDescriptor.DeclaringType => (Member as IMemberDescriptor).DeclaringType;
		string IFullNameProvider.FullName => (Member as IFullNameProvider).FullName;
		string? INameProvider.Name => (Member as INameProvider).Name;
		ModuleDefinition? IModuleProvider.Module => (Member as IModuleProvider).Module;
		MetadataToken IMetadataMember.MetadataToken => (Member as IMetadataMember).MetadataToken;

		FieldDefinition? IFieldDescriptor.Resolve() {
			return (Member as IFieldDescriptor).Resolve();
		}

		IMemberDefinition? IMemberDescriptor.Resolve() {
			return (Member as IMemberDescriptor).Resolve();
		}

		bool IImportable.IsImportedInModule(ModuleDefinition module) {
			return (Member as IImportable).IsImportedInModule(module);
		}

		IImportable IImportable.ImportWith(ReferenceImporter importer) {
			return (Member as IImportable).ImportWith(importer);
		}
		#endregion
	}

	internal static class DelayedResolverExtensions {
		public static void WriteFullName(this IFieldDescriptor field, BinaryWriter writer, StringHeap heap) {
			if (field is DelayedFieldResolver resolver) {
				heap.WriteString(writer, resolver.typeMetadata);
				heap.WriteString(writer, resolver.fieldMetadata);
			} else {
				if (field.Module?.Assembly is null)
					throw new ArgumentException("Field must be defined in an assembly", nameof(field));
				if (field.DeclaringType is null)
					throw new ArgumentException("Field must have a declaring type", nameof(field));
				if (field.Name is null)
					throw new ArgumentException("Field must have a name", nameof(field));

				heap.WriteString(writer, field.DeclaringType.FullName);
				heap.WriteString(writer, field.Name);
			}
		}
	}
}
