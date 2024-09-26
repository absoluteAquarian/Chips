using Chips.Compiler.Utility;
using System;

namespace Chips.Compiler.ErrorHandling {
	public interface IMessageFormatProvider<T> where T : struct, Enum {
		void ResolveMessageFormat(T id, ConsoleMessage message);

		string FormatMessageCode(T id);

		void GetBaseColors(T id, out ConsoleColor fg, out ConsoleColor bg);
	}
}
