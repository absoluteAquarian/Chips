using Chips.Compiler.Parsing;
using Chips.Runtime.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Chips.Utility {
	partial class Extensions {
		internal static bool StringExtensionsLinkedToSourceLines { get; set; }

		public static int GetActualPosition(this StreamReader reader) => typeof(StreamReader).RetrieveField<int>("_charPos", reader);

		public static void SetActualPosition(this StreamReader reader, int position) => typeof(StreamReader).AssignField("_charPos", reader, position);

		public static string ReadWord(this StreamReader reader, bool terminateOnComment = false) {
			// Read until we find a non-whitespace character
			reader.ReadUntilNonWhitespace();

			// Read until the next whitespace character
			StringBuilder word = new();

			while (reader.TryReadExceptWhitespace(out char read) && (!terminateOnComment || read != ';'))
				word.Append(read);

			return word.ToString();
		}

		public static string PeekWord(this StreamReader reader, bool terminateOnComment = false) {
			int pos = reader.GetActualPosition();

			bool updateLine = StringExtensionsLinkedToSourceLines;
			StringExtensionsLinkedToSourceLines = false;

			string word = reader.ReadWord(terminateOnComment);

			StringExtensionsLinkedToSourceLines = updateLine;

			reader.SetActualPosition(pos);
			return word;
		}

		public static char ReadFirstNonWhitespaceChar(this StreamReader reader) {
			reader.ReadUntilNonWhitespace();
			return (char)reader.Read();
		}

		public static char PeekFirstNonWhitespaceChar(this StreamReader reader) {
			int pos = reader.GetActualPosition();

			bool updateLine = StringExtensionsLinkedToSourceLines;
			StringExtensionsLinkedToSourceLines = false;

			char read = reader.ReadFirstNonWhitespaceChar();

			StringExtensionsLinkedToSourceLines = updateLine;

			reader.SetActualPosition(pos);
			return read;
		}

		public static string ReadWordOrQuotedString(this StreamReader reader, out bool wasQuoted, bool preprocessEscapedQuotes = false, bool terminateOnComment = false) {
			// Read until we find a non-whitespace character
			reader.ReadUntilNonWhitespace();

			// If the next character is a quote, read until the next quote
			char read;
			if (reader.Peek() == '"') {
				reader.Read();
				
				StringBuilder quotedWord = new();
				bool escaped = false;
				while (reader.TryReadExcept('"', out read, alwaysConsume: true)) {
					if (read == '\n') {
						ChipsCompiler.CompilingSourceLine--;
						ChipsCompiler.Error("Unexpected newline in quoted string");
						ChipsCompiler.CompilingSourceLine++;
					}

					if (preprocessEscapedQuotes && escaped && read == '"') {
						quotedWord.Append("\\\"");
						escaped = false;
					} else if (escaped && read == '\\') {
						quotedWord.Append("\\\\");
						escaped = false;
					} else {
						if (read == '\\')
							escaped = !escaped;
						else
							quotedWord.Append(read);
					}
				}

				wasQuoted = true;

				return quotedWord.ToString();
			}

			// Otherwise, read until the next whitespace character
			StringBuilder word = new();

			while (reader.TryReadExceptWhitespace(out read)) {
				if (terminateOnComment && read == ';')
					break;

				word.Append(read);
			}

			wasQuoted = false;

			return word.ToString();
		}

		public static IEnumerable<ParsedPossibleQuotedString> ReadManyWordsOrQuotedStrings(this StreamReader reader, bool preprocessEscapedQuotes = false, bool terminateOnComment = false) {
			List<ParsedPossibleQuotedString> words = new();

			string afterArg;
			do {
				string word = reader.ReadWordOrQuotedString(out bool wasQuoted, preprocessEscapedQuotes, terminateOnComment);
				words.Add(new(word, wasQuoted));

				afterArg = reader.PeekUntilMany(new char[] { ',', '\n' }, alwaysConsume: true);
			} while (afterArg.Length > 0 && !afterArg.EndsWith('\r'));

			return words;
		}

		public static string ReadUntil(this StreamReader reader, char except, bool alwaysConsume = false) {
			StringBuilder sb = new();
			while (reader.TryReadExcept(except, out char read, alwaysConsume))
				sb.Append(read);
			return sb.ToString();
		}

		public static string ReadUntilMany(this StreamReader reader, char[] except, bool alwaysConsume = false) {
			StringBuilder sb = new();
			while (reader.TryReadExceptMany(except, out char read, alwaysConsume))
				sb.Append(read);
			return sb.ToString();
		}

		public static string PeekUntilMany(this StreamReader reader, char[] except, bool alwaysConsume = false) {
			int pos = reader.GetActualPosition();

			bool updateLine = StringExtensionsLinkedToSourceLines;
			StringExtensionsLinkedToSourceLines = false;

			string read = reader.ReadUntilMany(except, alwaysConsume);

			StringExtensionsLinkedToSourceLines = updateLine;

			reader.SetActualPosition(pos);
			return read;
		}

		public static string ReadUntilNonWhitespace(this StreamReader reader, bool alwaysConsume = false) {
			StringBuilder sb = new();
			while (reader.TryReadWhitespace(out char read, alwaysConsume))
				sb.Append(read);
			return sb.ToString();
		}

		public static string ReadUntilNewline(this StreamReader reader) {
			StringBuilder sb = new();
			while (reader.TryReadExcept('\n', out char read, alwaysConsume: true))
				sb.Append(read);
			return sb.ToString();
		}

		public static string PeekUntilNewline(this StreamReader reader) {
			int pos = reader.GetActualPosition();

			bool updateLine = StringExtensionsLinkedToSourceLines;
			StringExtensionsLinkedToSourceLines = false;

			string read = reader.ReadUntilNewline();

			StringExtensionsLinkedToSourceLines = updateLine;

			reader.SetActualPosition(pos);
			return read;
		}

		public static string ReadWordsUntil(this StreamReader reader, int maxWords, bool terminateOnComment, params string[] except) {
			StringBuilder sb = new();
			int words = 0;

			while (Array.IndexOf(except, reader.PeekWord(terminateOnComment)) == -1) {
				if (sb.Length > 0)
					sb.Append(' ');

				sb.Append(reader.ReadWord(terminateOnComment));

				if (maxWords > 0 && ++words >= maxWords)
					break;
			}

			return sb.ToString();
		}

		public static bool TryReadExcept(this StreamReader reader, char except, out char read, bool alwaysConsume = false) {
			int peek = reader.Peek();
			if (peek >= 0 && peek != except) {
				read = (char)reader.Read();

				if (StringExtensionsLinkedToSourceLines && read == '\n')
					ChipsCompiler.CompilingSourceLine++;

				return true;
			}

			if (alwaysConsume && peek >= 0) {
				read = (char)reader.Read();

				if (StringExtensionsLinkedToSourceLines && read == '\n')
					ChipsCompiler.CompilingSourceLine++;
			} else
				read = default;

			return false;
		}

		public static bool TryReadExceptMany(this StreamReader reader, char[] except, out char read, bool alwaysConsume = false) {
			int peek = reader.Peek();
			if (peek >= 0 && Array.IndexOf(except, (char)peek) == -1) {
				read = (char)reader.Read();

				if (StringExtensionsLinkedToSourceLines && read == '\n')
					ChipsCompiler.CompilingSourceLine++;

				return true;
			}

			if (alwaysConsume && peek >= 0) {
				read = (char)reader.Read();

				if (StringExtensionsLinkedToSourceLines && read == '\n')
					ChipsCompiler.CompilingSourceLine++;
			} else
				read = default;

			return false;
		}

		public static bool TryReadWhitespace(this StreamReader reader, out char read, bool alwaysConsume = false) {
			int peek = reader.Peek();
			if (peek >= 0 && char.IsWhiteSpace((char)peek)) {
				read = (char)reader.Read();

				if (StringExtensionsLinkedToSourceLines && read == '\n')
					ChipsCompiler.CompilingSourceLine++;

				return true;
			}

			if (alwaysConsume && peek >= 0)
				read = (char)reader.Read();
			else 
				read = default;

			return false;
		}

		public static bool TryReadExceptWhitespace(this StreamReader reader, out char read, bool alwaysConsume = false) {
			int peek = reader.Peek();
			if (peek >= 0 && !char.IsWhiteSpace((char)peek)) {
				read = (char)reader.Read();
				return true;
			}

			if (alwaysConsume && peek >= 0) {
				read = (char)reader.Read();

				if (StringExtensionsLinkedToSourceLines && read == '\n')
					ChipsCompiler.CompilingSourceLine++;
			} else
				read = default;

			return false;
		}
	}
}
