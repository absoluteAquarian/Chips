using System.IO;
using System.Text;

namespace Chips.Utility {
	partial class Extensions {
		public static string ReadWord(this StreamReader reader) {
			// Read until we find a non-whitespace character
			reader.ReadUntilNonWhitespace();

			// Read until the next whitespace character
			StringBuilder word = new();

			while (reader.TryReadExceptWhitespace(out char read))
				word.Append(read);

			return word.ToString();
		}

		public static string ReadWordOrQuotedString(this StreamReader reader) {
			// Read until we find a non-whitespace character
			reader.ReadUntilNonWhitespace();

			// If the next character is a quote, read until the next quote
			char read;
			if (reader.Peek() == '"') {
				reader.Read();
				
				StringBuilder quotedWord = new();
				while (reader.TryReadExcept('"', out read, alwaysConsume: true))
					quotedWord.Append(read);

				return quotedWord.ToString();
			}

			// Otherwise, read until the next whitespace character
			StringBuilder word = new();

			while (reader.TryReadExceptWhitespace(out read))
				word.Append(read);

			return word.ToString();
		}

		public static void ReadUntilNonWhitespace(this StreamReader reader) {
			while (reader.TryReadWhitespace(out _));
		}

		public static bool TryReadExcept(this StreamReader reader, char except, out char read, bool alwaysConsume = false) {
			if (reader.Peek() is int a and > 0 && a != except) {
				read = (char)reader.Read();
				return true;
			}

			if (alwaysConsume)
				reader.Read();

			read = default;
			return false;
		}

		public static bool TryReadWhitespace(this StreamReader reader, out char read, bool alwaysConsume = false) {
			if (reader.Peek() is int a and > 0 && char.IsWhiteSpace((char)a)) {
				read = (char)reader.Read();
				return true;
			}

			if (alwaysConsume)
				reader.Read();

			read = default;
			return false;
		}

		public static bool TryReadExceptWhitespace(this StreamReader reader, out char read, bool alwaysConsume = false) {
			if (reader.Peek() is int a and > 0 && !char.IsWhiteSpace((char)a)) {
				read = (char)reader.Read();
				return true;
			}

			if (alwaysConsume)
				reader.Read();

			read = default;
			return false;
		}
	}
}
