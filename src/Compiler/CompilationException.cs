namespace Chips {
	public class CompilationException {
		public readonly string? SourceFile;
		public readonly int SourceLine;
		public readonly string Reason;

		public CompilationException(string reason) {
			Reason = reason;
			SourceFile = ChipsCompiler.CompilingSourceFile;
			SourceLine = ChipsCompiler.CompilingSourceLineOverride ?? ChipsCompiler.CompilingSourceLine;
		}
	}
}
