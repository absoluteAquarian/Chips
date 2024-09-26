using Chips.Compiler.Utility;
using System;

namespace Chips.Compiler.ErrorHandling {
	public abstract class CompilationMessage(string? sourceFile, int lineNumber, params object[] arguments) {
		public readonly string? sourceFile = sourceFile;
		public readonly int lineNumber = lineNumber;
		private readonly object[] arguments = arguments;
		public Span<object> Arguments => (arguments ?? []).AsSpan();

		protected abstract void Resolve(ConsoleMessage message);

		protected abstract void GetBaseColors(out ConsoleColor fg, out ConsoleColor bg);

		public void Print() {
			GetBaseColors(out ConsoleColor fg, out ConsoleColor bg);

			ConsoleMessage message = new ConsoleMessage().ChangeColors(fg, bg);

			if (sourceFile is not null){
				message.Add("[").Add(sourceFile).Add("]");
				
				if (lineNumber > 0)
					message.Add(" on line ").Add(lineNumber).Add(",");

				message.Add(" ");
			}

			Resolve(message);
			message.Print();
		}
	}

	public abstract class CompilationMessage<T>(string? sourceFile, int lineNumber, T id, params object[] arguments) : CompilationMessage(sourceFile, lineNumber, arguments) where T : struct, Enum {
		public readonly T messageID = id;

		protected sealed override void Resolve(ConsoleMessage message) => GetFormatter().ResolveMessageFormat(messageID, message);

		protected sealed override void GetBaseColors(out ConsoleColor fg, out ConsoleColor bg) => GetFormatter().GetBaseColors(messageID, out fg, out bg);

		protected abstract IMessageFormatProvider<T> GetFormatter();
	}
}
