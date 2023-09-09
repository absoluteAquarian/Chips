using System;
using System.IO;

namespace Chips.Compiler.Utility {
	/// <summary>
	/// An object representing a heap of strings
	/// </summary>
	public sealed class StringHeap {
		// Using StringBuilder to reduce string copying
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
				// Add the string to the heap
				index = heap.Length;
				Array.Resize(ref _heap, index + incoming.Length);

				// Copy the string to the heap using Span
				Span<char> resizedHeap = _heap;
				incoming.CopyTo(resizedHeap[index..]);
			}

			// Return the index of the string in the heap
			return new StringMetadata((uint)index, str.Length);
		}

		public string GetString(StringMetadata token) {
			// Throw if out of bounds
			if (token.Offset + token.Length > _heap.Length)
				throw new ArgumentOutOfRangeException();

			return new(_heap, (int)token.Offset, (int)token.Length);
		}

		public void Serialize(BinaryWriter writer) {
			writer.Write(_heap.Length);
			writer.Write(_heap);
		}

		public void Deserialize(BinaryReader reader) {
			int length = reader.ReadInt32();
			_heap = reader.ReadChars(length);
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
