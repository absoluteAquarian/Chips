namespace Chips.Compiler.ErrorHandling {
	internal enum ProjectErrorID {
		NewlineInStringLiteral,
		UnknownDirective,
		ExcessCharactersAfterDirective,
		NoSourceFilesFromProjectFile,
		NoSourceFilesFromCommandLine,
		UnknownSourceScope,
		Count
	}
}
