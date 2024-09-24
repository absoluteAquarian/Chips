using System;
using System.Runtime.CompilerServices;

namespace Chips.Runtime.Meta {
	interface IConvertToObject<T> where T : struct, IConvertToObject<T> {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		abstract static ref _Object AsObject(in T value, Span<_Object> stackAlloc1);
	}
}
