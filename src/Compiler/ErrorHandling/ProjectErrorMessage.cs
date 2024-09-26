using Chips.Compiler.Utility;
using System;

namespace Chips.Compiler.ErrorHandling {
	internal class ProjectErrorMessage(string? sourceFile, int lineNumber, ProjectErrorID id, params object[] arguments) : CompilationMessage<ProjectErrorID>(sourceFile, lineNumber, id, arguments), IMessageFormatProvider<ProjectErrorID> {
		protected override IMessageFormatProvider<ProjectErrorID> GetFormatter() => this;

		public string FormatMessageCode(ProjectErrorID id) {
			int maxDigits = ((int)ProjectErrorID.Count).ToString().Length;
			return id.ToString().PadLeft(maxDigits, '0');
		}

		void IMessageFormatProvider<ProjectErrorID>.ResolveMessageFormat(ProjectErrorID id, ConsoleMessage message) {
			message.Add("PROJ").Add(FormatMessageCode(id)).Add(": ");

			_ = id switch {
				ProjectErrorID.NewlineInStringLiteral => message.AddLine("Newline in string literal"),
				ProjectErrorID.UnknownDirective => message.AddLine("Unknown directive \"{0}\"", Arguments),
				ProjectErrorID.ExcessCharactersAfterDirective => message.AddLine("Excess characters after \"{0}\" directive", Arguments),
				ProjectErrorID.NoSourceFilesFromProjectFile => message.AddLine("No source files were specified in the project file."),
				ProjectErrorID.NoSourceFilesFromCommandLine => message.AddLine("No source files were found."),
				ProjectErrorID.UnknownSourceScope => message.AddLine("Unknown source scope \"{0}\", expected \"include\" or \"exclude\"", Arguments),
				_ => throw new ArgumentOutOfRangeException(nameof(id), id, "Invalid ID"),
			};
		}

		void IMessageFormatProvider<ProjectErrorID>.GetBaseColors(ProjectErrorID id, out ConsoleColor fg, out ConsoleColor bg) {
			fg = IsError(id) ? ConsoleColor.Red : ConsoleColor.Yellow;
			bg = ConsoleColor.Black;
		}

		public static bool IsError(ProjectErrorID id) => id switch {
			ProjectErrorID.Count => false,
			_ => true,
		};
	}
}
