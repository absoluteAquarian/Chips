using System.Runtime.CompilerServices;
using System;
using System.Runtime.InteropServices;

namespace Chips.Runtime.Meta {
	internal struct _Address : IChipsObject, IMutableInteger , ICreateObject<_Address>, IConvertToAddress<_Address> {
		public _AddressData data;
		public _NativeInt type;

		[MethodImpl(MethodImplOptions.AggressiveInlining)] static ref _Address IConvertToAddress<_Address>.AsAddress(in _Address value, Span<_Address> stackAlloc1) => ref Unsafe.AsRef(in value);

		static _Address ICreateObject<_Address>.Create<U>(in U value) {
			if (typeof(U) == typeof(_Address))
				return Unsafe.As<U, _Address>(ref Unsafe.AsRef(in value));

			_Address obj = default;
			obj.type = TypeMetrics.TypeToNativeInt<U>();
			Unsafe.As<_Address, U>(ref obj) = value;
			return obj;
		}
	}

	[StructLayout(LayoutKind.Explicit, Size = TypeMetrics.LARGE_POINTER)]
	internal struct _AddressData {
		[FieldOffset(0)] public nint IntPtr;
		[FieldOffset(0)] public nuint UIntPtr;
	}

	internal enum _NativeInt {
		NInt,
		NUInt
	}
}
