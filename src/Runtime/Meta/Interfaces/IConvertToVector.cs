using System;
using System.Runtime.CompilerServices;

namespace Chips.Runtime.Meta {
	interface IConvertToVector<T> where T : struct, IConvertToVector<T> {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		abstract static ref _VectorObject AsVector(in T value, Span<_VectorObject> stackAlloc1);
	}
}
