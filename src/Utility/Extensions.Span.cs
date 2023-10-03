using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Chips.Utility {
	partial class Extensions {
		public static unsafe T* ToPointer<T>(this ref Span<T> span) where T : unmanaged
			=> (T*)Unsafe.AsPointer(ref span.GetPinnableReference());

		public static unsafe string DecodeSpan(this ref Span<byte> span)
			=> Encoding.UTF8.GetString((byte*)Unsafe.AsPointer(ref span.GetPinnableReference()), span.Length);

		public static int Extract7BitEncodedInt(this ReadOnlySpan<byte> span, int offset, out int bytesRead) {
			int result = 0;
			int shift = 0;
			bytesRead = 0;

			byte read;
			do {
				// Read the byte
				read = span[offset];
				result |= (read & 0x7F) << shift;

				// Prepare for the next byte
				shift += 7;
				bytesRead++;
				offset++;
			} while ((read & 0x80) != 0 && bytesRead < 5 && offset < span.Length);

			if ((read & 0x80) != 0)
				throw new FormatException("Invalid 7-bit encoded integer");

			return result;
		}
	}
}
