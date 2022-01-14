using System.Runtime.CompilerServices;
using System.Text;

namespace Chips.Utility{
	internal static partial class Extensions{
		public static unsafe T* ToPointer<T>(this ref Span<T> span) where T : unmanaged
			=> (T*)Unsafe.AsPointer(ref span.GetPinnableReference());

		public static unsafe string DecodeSpan(this ref Span<byte> span)
			=> Encoding.UTF8.GetString((byte*)Unsafe.AsPointer(ref span.GetPinnableReference()), span.Length);

		public static ReadOnlySpan<int> ConvertSpan(this ReadOnlySpan<byte> span, int count){
			Span<int> result = stackalloc int[count];

			for(int i = 0; i < count; i++)
				result[i] = BitConverter.ToInt32(span[(i * 4)..((i + 1) * 4)]);

			return new ReadOnlySpan<int>(result.ToArray());
		}
	}
}
