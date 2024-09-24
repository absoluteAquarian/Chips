using System;
using System.Runtime.CompilerServices;

namespace Chips.Runtime.Meta {
	interface IConvertToInteger<T> where T : struct, IConvertToInteger<T> {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		abstract static ref _NumberInteger AsInteger(in T value, Span<_NumberInteger> stackAlloc1);
	}
}
