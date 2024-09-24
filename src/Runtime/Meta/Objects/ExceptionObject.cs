using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Chips.Runtime.Meta {
	internal struct _ExceptionObject : IChipsObject, ICreateObject<_ExceptionObject>, IConvertToException<_ExceptionObject> {
		public _ExceptionData data;

		static ref _ExceptionObject IConvertToException<_ExceptionObject>.AsException(in _ExceptionObject value, Span<_ExceptionObject> stackAlloc1) => ref Unsafe.AsRef(in value);

		static _ExceptionObject ICreateObject<_ExceptionObject>.Create<U>(in U value) {
			if (typeof(U) == typeof(_ExceptionObject))
				return Unsafe.As<U, _ExceptionObject>(ref Unsafe.AsRef(in value));
			else if (!typeof(Exception).IsAssignableFrom(typeof(U)))
				throw new InvalidConversionException<U>("ExceptionObject");

			_ExceptionObject obj = default;
			_ExceptionData.Get(ref obj.data) = Unsafe.As<U, Exception>(ref Unsafe.AsRef(in value));
			return obj;
		}
	}

	[StructLayout(LayoutKind.Explicit, Size = TypeMetrics.LARGE_POINTER)]
	internal struct _ExceptionData {
		[StructLayout(LayoutKind.Explicit, Size = TypeMetrics.LARGE_POINTER)]
		private struct _Intrinsic { }

		[FieldOffset(0)] private _Intrinsic Object;

		// "object" cannot be at the same FieldOffset as non-reference types, hence the redirection
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static ref Exception? Get(ref _ExceptionData data) => ref Unsafe.As<_Intrinsic, Exception?>(ref data.Object);
	}
}
