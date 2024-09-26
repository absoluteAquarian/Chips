using Chips.Compiler.Utility;
using System;

namespace Chips.Compiler.ErrorHandling {
	internal class ErrorMessage(string? sourceFile, int lineNumber, ErrorID id, params object[] arguments) : CompilationMessage<ErrorID>(sourceFile, lineNumber, id, arguments), IMessageFormatProvider<ErrorID> {
		protected override IMessageFormatProvider<ErrorID> GetFormatter() => this;

		public string FormatMessageCode(ErrorID id) {
			int maxDigits = ((int)ErrorID.Count).ToString().Length;
			return id.ToString().PadLeft(maxDigits, '0');
		}

		void IMessageFormatProvider<ErrorID>.ResolveMessageFormat(ErrorID id, ConsoleMessage message) {
			message.ChangeColors(IsError(id) ? ConsoleColor.Red : ConsoleColor.Yellow, ConsoleColor.Black)
				.Add("CHP").Add(FormatMessageCode(id)).Add(": ");

			_ = id switch {
				ErrorID.NewlineInStringLiteral => message.AddLine("Newline in string literal"),
				_ => throw new ArgumentOutOfRangeException(nameof(id), id, "Invalid ID"),
			};
		}

		void IMessageFormatProvider<ErrorID>.GetBaseColors(ErrorID id, out ConsoleColor fg, out ConsoleColor bg) {
			fg = IsError(id) ? ConsoleColor.Red : ConsoleColor.Yellow;
			bg = ConsoleColor.Black;
		}

		public static bool IsError(ErrorID id) => id switch {
			ErrorID.Count => false,
			_ => true,
		};
	}
}
