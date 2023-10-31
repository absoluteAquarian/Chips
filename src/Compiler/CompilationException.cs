using System.Text;

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

		public override string ToString() {
			StringBuilder sb = new(Reason);
			
			if (SourceFile is not null) {
				sb.Append(" at ");
				sb.Append(SourceFile);

				if (SourceLine >= 0) {
					sb.Append(" on line ");
					sb.Append(SourceLine + 1);
				}
			}

			return sb.ToString();
		}
	}
}
