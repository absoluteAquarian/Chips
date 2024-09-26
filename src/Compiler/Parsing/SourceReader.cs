using Chips.Compiler.ErrorHandling;
using Chips.Compiler.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Chips.Compiler.Parsing {
	/// <summary>
	/// A wrapper class over <see cref="StreamReader"/> that provides additional functionality for compiler errors
	/// </summary>
	internal class SourceReader(string file) : IDisposable, ISourceFileInfoProvider {
		private class FakeStreamReader {
			// Field order matches the order in StreamReader.  This may have to be updated if StreamReader changes.
			public readonly Stream _stream;
			public readonly Encoding _encoding;
			public readonly Decoder _decoder;
			public readonly byte[] _byteBuffer;
			public readonly char[] _charBuffer;
			public int _charPos;  // This is the only field that matters
			public readonly int _charLen;
			public readonly int _byteLen;
			public readonly int _bytePos;
			public readonly int _maxCharsPerBuffer;
			public readonly bool _disposed;
			public readonly bool _detectEncoding;
			public readonly bool _checkPreamble;
			public readonly bool _isBlocked;
			public readonly bool _closable;
			public readonly Task _asyncReadTask;
		}

		private readonly ref struct ReaderState(SourceReader self) {
			public readonly int position = self.GetActualPosition();
			public readonly int line = self.LineNumber;
			public readonly SourceReader self = self;

			public void Dispose() {
				self.SetActualPosition(position);
				self.LineNumber = line;
			}
		}

		public const char COMMENT_INDICATOR = ';';

		private StreamReader _reader = new StreamReader(File.OpenRead(file));
		private string _file = file;

		public StreamReader BaseReader => _reader;
		public string SourceFile => _file;

		public int LineNumber { get; private set; } = 1;

		public int GetActualPosition() => Unsafe.As<FakeStreamReader>(_reader)._charPos;

		public void SetActualPosition(int position) => Unsafe.As<FakeStreamReader>(_reader)._charPos = position;

		private int ReadAndAdvanceLines() {
			int read = _reader.Read();

			if (read == '\n')
				LineNumber++;

			return read;
		}

		public string ReadWord(bool terminateOnComment = false) {
			// Read until we find a non-whitespace character
			ReadUntilNonWhitespace();

			// Read until the next whitespace character
			StringBuilder word = new();

			while (TryReadExceptWhitespace(out char read) && (!terminateOnComment || read != COMMENT_INDICATOR))
				word.Append(read);

			return word.ToString();
		}

		public string PeekWord(bool terminateOnComment = false) {
			using ReaderState state = new(this);
			return ReadWord(terminateOnComment);
		}

		public char ReadFirstNonWhitespaceChar() {
			ReadUntilNonWhitespace();
			return (char)ReadAndAdvanceLines();
		}

		public char PeekFirstNonWhitespaceChar() {
			using ReaderState state = new(this);
			return ReadFirstNonWhitespaceChar();
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
					if (read == '\n')
						ChipsCompiler.Results.Error(_file, LineNumber, ErrorID.NewlineInStringLiteral);

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
				if (terminateOnComment && read == COMMENT_INDICATOR)
					break;

				word.Append(read);
			}

			wasQuoted = false;

			return word.ToString();
		}

		public IEnumerable<ParsedPossibleQuotedString> ReadManyWordsOrQuotedStrings(bool preprocessEscapedQuotes = false, bool terminateOnComment = false) {
			List<ParsedPossibleQuotedString> words = [];

			string afterArg;
			do {
				string word = ReadWordOrQuotedString(out bool wasQuoted, preprocessEscapedQuotes, terminateOnComment);
				words.Add(new(word, wasQuoted));

				afterArg = PeekUntilMany([ ',', '\n' ], alwaysConsume: true);
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
			using ReaderState state = new(this);
			return ReadUntilMany(except, alwaysConsume);
		}

		public void ReadUntilNonWhitespace() {
			while (TryReadWhitespace(out _, false));
		}

		public string ReadUntilNewline() {
			StringBuilder sb = new();
			while (TryReadExcept('\n', out char read, alwaysConsume: true))
				sb.Append(read);
			return sb.ToString();
		}

		public string PeekUntilNewline() {
			using ReaderState state = new(this);
			return ReadUntilNewline();
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

		public bool TryReadExcept(char except, out char read, bool alwaysConsume = false) => FilteredRead(except, alwaysConsume, out read, static (except, peek) => peek != except);

		public bool TryReadExceptMany(char[] except, out char read, bool alwaysConsume = false) => FilteredRead(except, alwaysConsume, out read, static (except, peek) => Array.IndexOf(except, peek) == -1);

		public bool TryReadWhitespace(out char read, bool alwaysConsume = false) => FilteredRead(alwaysConsume, out read, char.IsWhiteSpace);

		public bool TryReadExceptWhitespace(out char read, bool alwaysConsume = false) => FilteredRead(alwaysConsume, out read, static peek => !char.IsWhiteSpace(peek));

		private bool FilteredRead<T>(T value, bool alwaysConsume, out char read, Func<T, char, bool> checkPeekFunc) {
			int peek = _reader.Peek();
			if (peek >= 0) {
				read = alwaysConsume || checkPeekFunc(value, (char)peek) ? (char)ReadAndAdvanceLines() : default;
				return true;
			}

			read = default;
			return false;
		}

		private bool FilteredRead(bool alwaysConsume, out char read, Func<char, bool> checkPeekFunc) {
			int peek = _reader.Peek();
			if (peek >= 0) {
				read = alwaysConsume || checkPeekFunc((char)peek) ? (char)ReadAndAdvanceLines() : default;
				return true;
			}

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
				_file = null!;
				LineNumber = -1;
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
