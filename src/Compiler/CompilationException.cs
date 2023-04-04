namespace Chips {
	public class CompilationException {
		public readonly string? SourceFile;
		public readonly string Reason;

		public CompilationException(string? sourceFile, string reason) {
			SourceFile = sourceFile;
			Reason = reason;
		}
	}
}
