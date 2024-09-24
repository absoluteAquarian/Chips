using System;
using System.Runtime.CompilerServices;

namespace Chips.Runtime.Meta {
	internal interface IConvertToStatus<T> where T : struct, IConvertToStatus<T> {
		[MethodImpl(MethodImplOptions.AggressiveInlining)] abstract static ref _StatusObject AsStatus(in T value, Span<_StatusObject> stackAlloc1);
	}
}
