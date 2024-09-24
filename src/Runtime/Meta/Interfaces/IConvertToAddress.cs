using System;
using System.Runtime.CompilerServices;

namespace Chips.Runtime.Meta {
	interface IConvertToAddress<T> where T : struct, IConvertToAddress<T> {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		abstract static ref _Address AsAddress(in T value, Span<_Address> stackAlloc1);
	}
}
