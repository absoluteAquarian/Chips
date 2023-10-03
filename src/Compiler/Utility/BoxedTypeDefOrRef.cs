using AsmResolver;
using AsmResolver.DotNet;
using AsmResolver.DotNet.Signatures.Types;
using AsmResolver.PE.DotNet.Metadata.Tables;
using System.Collections.Generic;

namespace Chips.Compiler.Utility {
	internal class BoxedTypeDefOrRef : ITypeDefOrRef {
		public readonly ITypeDefOrRef boxedType;

        public BoxedTypeDefOrRef(ITypeDefOrRef typeToBox) {
            boxedType = typeToBox;
        }

		public Utf8String? Name => boxedType.Name;

		public Utf8String? Namespace => boxedType.Namespace;

		public ITypeDefOrRef? DeclaringType => boxedType.DeclaringType;

		public IResolutionScope? Scope => boxedType.Scope;

		public bool IsValueType => boxedType.IsValueType;

		public string FullName => boxedType.FullName;

		public ModuleDefinition? Module => boxedType.Module;

		public IList<CustomAttribute> CustomAttributes => boxedType.CustomAttributes;

		public MetadataToken MetadataToken => boxedType.MetadataToken;

		string? ITypeDescriptor.Namespace => ((ITypeDescriptor)boxedType).Namespace;

		ITypeDescriptor? IMemberDescriptor.DeclaringType => ((IMemberDescriptor)boxedType).DeclaringType;

		string? INameProvider.Name => ((INameProvider)boxedType).Name;

		public ITypeDefOrRef ImportWith(ReferenceImporter importer) => boxedType.ImportWith(importer);

		public bool IsImportedInModule(ModuleDefinition module) => boxedType.IsImportedInModule(module);

		public TypeDefinition? Resolve() => boxedType.Resolve();

		public ITypeDefOrRef ToTypeDefOrRef() => boxedType.ToTypeDefOrRef();

		public TypeSignature ToTypeSignature(bool isValueType) => boxedType.ToTypeSignature(isValueType);

		public TypeSignature ToTypeSignature() => boxedType.ToTypeSignature();

		IImportable IImportable.ImportWith(ReferenceImporter importer) => ((IImportable)boxedType).ImportWith(importer);

		IMemberDefinition? IMemberDescriptor.Resolve() => ((IMemberDescriptor)boxedType).Resolve();
	}
}
