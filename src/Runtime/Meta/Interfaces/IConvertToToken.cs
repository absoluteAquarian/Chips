using System;
using System.Runtime.CompilerServices;

namespace Chips.Runtime.Meta {
	interface IConvertToToken<T> where T : struct, IConvertToToken<T> {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		abstract static ref _TokenHandle AsToken(in T value, Span<_TokenHandle> stackAlloc1);
	}
}
