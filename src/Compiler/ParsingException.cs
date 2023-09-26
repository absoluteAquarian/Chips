using System;

namespace Chips.Compiler {
	internal class ParsingException : Exception {
		public ParsingException(string message) : base(message) { }
	}
}
