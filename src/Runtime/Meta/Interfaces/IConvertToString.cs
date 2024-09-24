using System;
using System.Runtime.CompilerServices;

namespace Chips.Runtime.Meta {
	interface IConvertToString<T> where T : IConvertToString<T> {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		abstract static ref _StringObject AsString(in T value, Span<_StringObject> stackAlloc1);
	}
}
