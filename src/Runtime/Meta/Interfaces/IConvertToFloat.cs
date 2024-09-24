using System;
using System.Runtime.CompilerServices;

namespace Chips.Runtime.Meta {
	interface IConvertToFloat<T> where T : struct, IConvertToFloat<T> {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		abstract static ref _NumberFloat AsFloat(in T value, Span<_NumberFloat> stackAlloc1);
	}
}
