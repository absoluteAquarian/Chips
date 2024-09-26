using Chips.Compiler.ErrorHandling;
using System.Collections;
using System.Collections.Generic;

namespace Chips.Compiler {
	internal class CompilationResults : IEnumerable<CompilationMessage> {
		private readonly List<CompilationMessage> messages = [];

		public bool HasErrors { get; private set; } = false;

		public void Error(string? sourceFile, int lineNumber, ErrorID code, params object[] arguments) {
			if (ErrorMessage.IsError(code))
				HasErrors = true;

			messages.Add(new ErrorMessage(sourceFile, lineNumber, code, arguments));
		}

		public void ProjectError(string? sourceFile, int lineNumber, ProjectErrorID code, params object[] arguments) {
			if (ProjectErrorMessage.IsError(code))
				HasErrors = true;

			messages.Add(new ProjectErrorMessage(sourceFile, lineNumber, code, arguments));
		}

		public IEnumerator<CompilationMessage> GetEnumerator() => messages.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
