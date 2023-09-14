using System;
using System.IO;

namespace Chips.Compiler.Utility {
	/// <summary>
	/// An object representing a heap of strings
	/// </summary>
	public sealed class StringHeap {
		private char[] _heap = Array.Empty<char>();

		/// <summary>
		/// Adds a string to the heap if it isn't already present, then returns its index in the heap
		/// </summary>
		public StringMetadata GetOrAdd(string str) {
			// Check if the string is already in the heap
			ReadOnlySpan<char> incoming = str;
			ReadOnlySpan<char> heap = _heap;

			int index = heap.IndexOf(incoming, StringComparison.Ordinal);
			if (index < 0) {
				// Check if part of the end of the heap is the same as the beginning of the string
				// If so, we can just add the rest of the string to the heap
				int overlap = 0;
				for (int i = 1; i <= heap.Length; i++) {
					if (incoming.StartsWith(heap[^i..]))
						overlap = i;
				}

				Span<char> resizedHeap;
				if (overlap > 0) {
					// Add the remaning part of the string to the heap
					index = heap.Length - overlap;
					Array.Resize(ref _heap, index + incoming.Length);

					// Copy the string to the heap using Span
					resizedHeap = _heap;
					incoming[overlap..].CopyTo(resizedHeap[overlap..]);
				} else {
					// Add the full string to the heap
					index = heap.Length;
					Array.Resize(ref _heap, index + incoming.Length);

					// Copy the string to the heap using Span
					resizedHeap = _heap;
					incoming.CopyTo(resizedHeap[index..]);
				}
			}

			// Return the index of the string in the heap
			return new StringMetadata((uint)index, str.Length);
		}

		public string GetString(StringMetadata token) {
			// Throw if out of bounds
			if (token.Offset + token.Length > _heap.Length)
				throw new ArgumentOutOfRangeException();

			return new(_heap, (int)token.Offset, token.Length);
		}

		public void Serialize(BinaryWriter writer) {
			writer.Write7BitEncodedInt(_heap.Length);
			writer.Write(_heap);
		}

		public void Deserialize(BinaryReader reader) {
			int length = reader.Read7BitEncodedInt();
			_heap = reader.ReadChars(length);
		}

		public void WriteString(BinaryWriter writer, string str) {
			StringMetadata token = GetOrAdd(str);
			token.Serialize(writer);
		}

		public string ReadString(BinaryReader reader) {
			StringMetadata token = StringMetadata.Deserialize(reader);
			return GetString(token);
		}
	}

	public readonly struct StringMetadata {
		public readonly uint Offset;
		public readonly int Length;

		public StringMetadata(uint offset, int length) {
			Offset = offset;
			Length = length;
		}

		public void Serialize(BinaryWriter writer) {
			writer.Write7BitEncodedInt((int)Offset);
			writer.Write7BitEncodedInt(Length);
		}

		public static StringMetadata Deserialize(BinaryReader reader) {
			uint offset = (uint)reader.Read7BitEncodedInt();
			int length = reader.Read7BitEncodedInt();
			return new(offset, length);
		}
	}
}
