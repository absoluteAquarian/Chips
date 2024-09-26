namespace Chips.Compiler.Utility {
	public interface ISourceFileInfoProvider {
		string SourceFile { get; }

		int LineNumber { get; }
	}
}
