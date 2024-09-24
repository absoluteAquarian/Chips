using System;

namespace Chips.Runtime.Meta {
	internal interface IConvertToException<T> where T : struct, IConvertToException<T> {
		abstract static ref _ExceptionObject AsException(in T value, Span<_ExceptionObject> stackAlloc1);
	}
}
