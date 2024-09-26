namespace Chips.Compiler.ErrorHandling {
	public enum ErrorID {
		NewlineInStringLiteral,
		TypeAliasAlreadyDefined,
		NamespaceAlreadyImported,
		CannotResolveType_NoAssemblies,
		CannotResolveType_NotFound,
		CannotResolveType_Ambiguous,
		CannotResolveType_Ambiguous_MultipleAssemblies,
		Count
	}
}
