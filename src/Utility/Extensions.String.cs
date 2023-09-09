﻿using System;
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
	}
}