using Chips.Utility;
using System;
using System.IO;
using System.Text;

namespace Chips.Compiler.Utility {
	/// <summary>
	/// An object representing a heap of strings
	/// </summary>
	public sealed class StringHeap {
		private byte[] _heap = Array.Empty<byte>();

		public StringHeap() {
			// Empty string is always first in the heap
			_ = GetOrAdd("");
		}

		/// <summary>
		/// Adds a string to the heap if it isn't already present, then returns its index in the heap
		/// </summary>
		public StringMetadata GetOrAdd(string str) {
			int size = str.GetHeapSize();
			Span<byte> bytes = size < 1024 * 4 ? stackalloc byte[size] : new byte[size];
			str.EncodeToHeap(bytes);

			// Check if the string is already in the heap
			ReadOnlySpan<byte> incoming = bytes;
			ReadOnlySpan<byte> heap = _heap;

			int index = heap.IndexOf(incoming);
			if (index < 0) {
				// Add the full string to the heap
				index = heap.Length;
				Array.Resize(ref _heap, index + incoming.Length);

				// Copy the string to the heap using Span
				Span<byte> resizedHeap = _heap;
				incoming.CopyTo(resizedHeap[index..]);
			}

			// Return the index of the string in the heap
			return new StringMetadata(index);
		}

		public string GetString(StringMetadata token) {
			// Throw if out of bounds
			if (token.Offset >= _heap.Length)
				throw new ArgumentOutOfRangeException();

			// Get the length of the string at the offset as a 7-bit encoded int
			ReadOnlySpan<byte> heap = _heap;
			int length = heap.Extract7BitEncodedInt(token.Offset, out int bytesRead);

			// Extract the string from the heap
			ReadOnlySpan<byte> heapedStr = heap.Slice(token.Offset + bytesRead, length);

			// Check if the next byte is the null terminator
			int terminatorIndex = token.Offset + bytesRead + length;
			if (terminatorIndex >= heap.Length || heap[terminatorIndex] != 0)
				throw new InvalidOperationException("String heap is corrupted");

			// Decode the string from the heap
			return Encoding.UTF8.GetString(heapedStr);
		}

		public void Clear() {
			_heap = Array.Empty<byte>();
		}

		public void Serialize(BinaryWriter writer) {
			writer.Write7BitEncodedInt(_heap.Length);
			writer.Write(_heap);
		}

		public void Deserialize(BinaryReader reader) {
			int length = reader.Read7BitEncodedInt();
			_heap = reader.ReadBytes(length);
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
		public readonly int Offset;

		public StringMetadata(int offset) {
			Offset = offset;
		}

		public void Serialize(BinaryWriter writer) {
			writer.Write(Offset);
		}

		public static StringMetadata Deserialize(BinaryReader reader) {
			int offset = reader.ReadInt32();
			return new(offset);
		}
	}
}
