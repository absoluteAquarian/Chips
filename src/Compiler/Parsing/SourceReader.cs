using Chips.Runtime.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Chips.Compiler.Parsing {
	/// <summary>
	/// A wrapper class over <see cref="StreamReader"/> that provides additional functionality for compiler errors
	/// </summary>
	internal class SourceReader : IDisposable {
		private StreamReader _reader;

		public StreamReader BaseReader => _reader;

		public bool NotConnectedToCompiler { get; init; }

		public SourceReader(StreamReader reader) {
			_reader = reader;
		}

		public int GetActualPosition() => typeof(StreamReader).RetrieveField<int>("_charPos", _reader);

		public void SetActualPosition(int position) => typeof(StreamReader).AssignField("_charPos", _reader, position);

		public string ReadWord(bool terminateOnComment = false) {
			// Read until we find a non-whitespace character
			ReadUntilNonWhitespace();

			// Read until the next whitespace character
			StringBuilder word = new();

			while (TryReadExceptWhitespace(out char read) && (!terminateOnComment || read != ';'))
				word.Append(read);

			return word.ToString();
		}

		public string PeekWord(bool terminateOnComment = false) {
			int pos = GetActualPosition();
			string word = ReadWord(terminateOnComment);

			SetActualPosition(pos);
			return word;
		}

		public char ReadFirstNonWhitespaceChar() {
			ReadUntilNonWhitespace();
			return (char)_reader.Read();
		}

		public char PeekFirstNonWhitespaceChar() {
			int pos = GetActualPosition();
			char read = ReadFirstNonWhitespaceChar();

			SetActualPosition(pos);
			return read;
		}

		public string ReadWordOrQuotedString(out bool wasQuoted, bool preprocessEscapedQuotes = false, bool terminateOnComment = false) {
			// Read until we find a non-whitespace character
			ReadUntilNonWhitespace();

			// If the next character is a quote, read until the next quote
			char read;
			if (_reader.Peek() == '"') {
				_reader.Read();
				
				StringBuilder quotedWord = new();
				bool escaped = false;
				while (TryReadExcept('"', out read, alwaysConsume: true)) {
					if (read == '\n' && !NotConnectedToCompiler) {
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

			while (TryReadExceptWhitespace(out read)) {
				if (terminateOnComment && read == ';')
					break;

				word.Append(read);
			}

			wasQuoted = false;

			return word.ToString();
		}

		public IEnumerable<ParsedPossibleQuotedString> ReadManyWordsOrQuotedStrings(bool preprocessEscapedQuotes = false, bool terminateOnComment = false) {
			List<ParsedPossibleQuotedString> words = new();

			string afterArg;
			do {
				string word = ReadWordOrQuotedString(out bool wasQuoted, preprocessEscapedQuotes, terminateOnComment);
				words.Add(new(word, wasQuoted));

				afterArg = PeekUntilMany(new char[] { ',', '\n' }, alwaysConsume: true);
			} while (afterArg.Length > 0 && !afterArg.EndsWith('\r'));

			return words;
		}

		public string ReadUntil(char except, bool alwaysConsume = false) {
			StringBuilder sb = new();
			while (TryReadExcept(except, out char read, alwaysConsume))
				sb.Append(read);
			return sb.ToString();
		}

		public string ReadUntilMany(char[] except, bool alwaysConsume = false) {
			StringBuilder sb = new();
			while (TryReadExceptMany(except, out char read, alwaysConsume))
				sb.Append(read);
			return sb.ToString();
		}

		public string PeekUntilMany(char[] except, bool alwaysConsume = false) {
			int pos = GetActualPosition();
			string read = ReadUntilMany(except, alwaysConsume);

			SetActualPosition(pos);
			return read;
		}

		public string ReadUntilNonWhitespace(bool alwaysConsume = false) {
			StringBuilder sb = new();
			while (TryReadWhitespace(out char read, alwaysConsume))
				sb.Append(read);
			return sb.ToString();
		}

		public string ReadUntilNewline() {
			StringBuilder sb = new();
			while (TryReadExcept('\n', out char read, alwaysConsume: true))
				sb.Append(read);
			return sb.ToString();
		}

		public string PeekUntilNewline() {
			int pos = GetActualPosition();
			string read = ReadUntilNewline();

			SetActualPosition(pos);
			return read;
		}

		public string ReadWordsUntil(int maxWords, bool terminateOnComment, params string[] except) {
			StringBuilder sb = new();
			int words = 0;

			while (Array.IndexOf(except, PeekWord(terminateOnComment)) == -1) {
				if (sb.Length > 0)
					sb.Append(' ');

				sb.Append(ReadWord(terminateOnComment));

				if (maxWords > 0 && ++words >= maxWords)
					break;
			}

			return sb.ToString();
		}

		public bool TryReadExcept(char except, out char read, bool alwaysConsume = false) {
			int peek = _reader.Peek();
			if (peek >= 0 && peek != except) {
				read = (char)_reader.Read();

				if (read == '\n' && !NotConnectedToCompiler)
					ChipsCompiler.CompilingSourceLine++;

				return true;
			}

			if (alwaysConsume && peek >= 0) {
				read = (char)_reader.Read();

				if (read == '\n' && !NotConnectedToCompiler)
					ChipsCompiler.CompilingSourceLine++;
			} else
				read = default;

			return false;
		}

		public bool TryReadExceptMany(char[] except, out char read, bool alwaysConsume = false) {
			int peek = _reader.Peek();
			if (peek >= 0 && Array.IndexOf(except, (char)peek) == -1) {
				read = (char)_reader.Read();

				if (read == '\n' && !NotConnectedToCompiler)
					ChipsCompiler.CompilingSourceLine++;

				return true;
			}

			if (alwaysConsume && peek >= 0) {
				read = (char)_reader.Read();

				if (read == '\n' && !NotConnectedToCompiler)
					ChipsCompiler.CompilingSourceLine++;
			} else
				read = default;

			return false;
		}

		public bool TryReadWhitespace(out char read, bool alwaysConsume = false) {
			int peek = _reader.Peek();
			if (peek >= 0 && char.IsWhiteSpace((char)peek)) {
				read = (char)_reader.Read();

				if (read == '\n' && !NotConnectedToCompiler)
					ChipsCompiler.CompilingSourceLine++;

				return true;
			}

			if (alwaysConsume && peek >= 0)
				read = (char)_reader.Read();
			else 
				read = default;

			return false;
		}

		public bool TryReadExceptWhitespace(out char read, bool alwaysConsume = false) {
			int peek = _reader.Peek();
			if (peek >= 0 && !char.IsWhiteSpace((char)peek)) {
				read = (char)_reader.Read();
				return true;
			}

			if (alwaysConsume && peek >= 0) {
				read = (char)_reader.Read();

				if (read == '\n' && !NotConnectedToCompiler)
					ChipsCompiler.CompilingSourceLine++;
			} else
				read = default;

			return false;
		}

		#region Implement IDisposable
		private bool disposed;

		private void Dispose(bool disposing) {
			if (!disposed) {
				if (disposing)
					_reader.Dispose();

				_reader = null!;
				disposed = true;
			}
		}

		~SourceReader() => Dispose(disposing: false);

		public void Dispose() {
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
