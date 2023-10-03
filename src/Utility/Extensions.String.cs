using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Chips.Utility {
	partial class Extensions {
		private static readonly Dictionary<char, string> _escapedCharacters = new() {
			['\\'] = "\\",
			['\''] = "'",
			['"'] = "\"",
			['0'] = "\0",
			['a'] = "\a",
			['b'] = "\b",
			['f'] = "\f",
			['n'] = "\n",
			['r'] = "\r",
			['t'] = "\t",
			['v'] = "\v",
		};

		private static readonly Dictionary<char, char> _unescapedCharacters = new() {
		//	['\\'] = '\\',
		//	['\''] = '\'',
		//	['\"'] = '"',
			['\0'] = '0',
			['\a'] = 'a',
			['\b'] = 'b',
			['\f'] = 'f',
			['\n'] = 'n',
			['\r'] = 'r',
			['\t'] = 't',
			['\v'] = 'v',
		};

		public static string SanitizeString(this string str) {
			if (string.IsNullOrWhiteSpace(str))
				return str;

			// Convert any escaped characters to their actual values
			char lastChar = '\0';
			StringBuilder sb = new();
			for (int i = 0; i < str.Length; i++) {
				char c = str[i];

				if (lastChar == '\\') {
					if (!_escapedCharacters.TryGetValue(c, out string? value))
						throw new ArgumentException("Invalid escape sequence: \\" + c);

					sb.Append(value);
					lastChar = '\0';
				} else {
					if (c != '\\') {
						// Append the character itself
						sb.Append(c);
					}

					lastChar = c;
				}
			}

			if (lastChar == '\\')
				throw new ArgumentException("Invalid escape sequence: \\");

			return sb.ToString();
		}

		public static string DesanitizeString(this string str) {
			if (string.IsNullOrWhiteSpace(str))
				return str;

			StringBuilder sb = new();
			foreach (char c in str) {
				if (_unescapedCharacters.TryGetValue(c, out char value))
					sb.Append('\\').Append(value);
				else
					sb.Append(c);
			}

			return sb.ToString();
		}

		public static byte[] EncodeToCPDB(this string str) {
			if (string.IsNullOrWhiteSpace(str))
				throw new ArgumentException("String cannot be null or whitespace", nameof(str));

			using MemoryStream ms = new();
			using BinaryWriter writer = new(ms);

			writer.Write7BitEncodedInt(str.Length);
			writer.Write(Encoding.UTF8.GetBytes(str));

			return ms.ToArray();
		}

		public static ReadOnlySpan<byte> EncodeASCIISpan(this string str) {
			if (string.IsNullOrWhiteSpace(str))
				throw new ArgumentException("String cannot be null or whitespace", nameof(str));

			byte[] bytes = new byte[str.Length];
			for (int i = 0; i < str.Length; i++) {
				ushort decode = str[i];
				if (decode > 0xFF)
					throw new ArgumentException("String contains non-ASCII characters", nameof(str));

				bytes[i] = (byte)decode;
			}

			return bytes;
		}

		public static int CountChars(this string str, char c) {
			int count = 0;
			foreach (char ch in str) {
				if (ch == c)
					count++;
			}
			return count;
		}

		public static int CountChars(this string str, char c1, char c2) {
			int count = 0;
			foreach (char ch in str) {
				if (ch == c1 || ch == c2)
					count++;
			}
			return count;
		}

		public static int CountChars(this string str, char c1, char c2, char c3) {
			int count = 0;
			foreach (char ch in str) {
				if (ch == c1 || ch == c2 || ch == c3)
					count++;
			}
			return count;
		}

		public static int CountChars(this string str, params char[] chars) {
			int count = 0;
			foreach (char ch in str) {
				foreach (char c in chars) {
					if (ch == c) {
						count++;
						break;
					}
				}
			}
			return count;
		}

		public static string AttemptCoreTypeAlias(this string type) {
			return type switch {
				"System.SByte" => "sbyte",
				"System.Byte" => "byte",
				"System.Int16" => "short",
				"System.UInt16" => "ushort",
				"System.Int32" => "int",
				"System.UInt32" => "uint",
				"System.Int64" => "long",
				"System.UInt64" => "ulong",
				"System.IntPtr" => "nint",
				"System.UIntPtr" => "nuint",
				"System.Single" => "float",
				"System.Double" => "double",
				"System.Boolean" => "bool",
				"System.Char" => "char",
				"System.String" => "string",
				"System.Object" => "object",
				"System.Void" => "void",
				"System.Decimal" => "decimal",
				_ => type
			};
		}

		public static int GetHeapSize(this string str) {
			// Calculate how large the 7-bit encoded int will be for the string's length
			// Magic numbers taken from:  https://stackoverflow.com/a/49780224
			int length = Encoding.UTF8.GetByteCount(str);

			int size;
			if (length < 128)
				size = 1;
			else if (length < 16384)
				size = 2;
			else if (length < 2097152)
				size = 3;
			else if (length < 268435456)
				size = 4;
			else
				size = 5;

			return size + length + 1;  // Encoded length + string bytes + null terminator
		}

		public static void EncodeToHeap(this string str, Span<byte> span) {
			int expectedSize = str.GetHeapSize();
			if (span.Length < expectedSize)
				throw new ArgumentException($"Span is too small to encode string. Expected at least {expectedSize} bytes, got {span.Length} bytes instead.", nameof(span));

			// Write the string's length as a 7-bit encoded int
			int length = Encoding.UTF8.GetByteCount(str);
			int i = 0;
			while (length >= 128) {
				span[i++] = (byte)(length | 0x80);
				length >>= 7;
			}
			span[i++] = (byte)length;

			// Write the string's bytes
			i += Encoding.UTF8.GetBytes(str, span.Slice(i));

			// Write the null terminator
			span[i] = 0;
		}
	}
}
