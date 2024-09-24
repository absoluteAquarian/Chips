using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Chips.Runtime.Meta {
	internal struct _StringObject : IChipsObject, ICreateObject<_StringObject>, IConvertToString<_StringObject> {
		public _StringData data;
		public _String type;

		[MethodImpl(MethodImplOptions.AggressiveInlining)] static ref _StringObject IConvertToString<_StringObject>.AsString(in _StringObject value, Span<_StringObject> stackAlloc1) => ref Unsafe.AsRef(in value);

		static _StringObject ICreateObject<_StringObject>.Create<U>(in U value) {
			if (typeof(U) == typeof(_StringObject))
				return Unsafe.As<U, _StringObject>(ref Unsafe.AsRef(in value));

			_StringObject obj = default;
			obj.type = TypeMetrics.TypeToStringObject<U>();
			Unsafe.As<_StringObject, U>(ref obj) = value;
			return obj;
		}
	}

	[StructLayout(LayoutKind.Explicit, Size = TypeMetrics.LARGE_POINTER)]
	internal struct _StringData {
		[StructLayout(LayoutKind.Explicit, Size = TypeMetrics.LARGE_POINTER)]
		private struct _Intrinsic { }

		[FieldOffset(0)] private _Intrinsic Object;

		// "object" cannot be at the same FieldOffset as non-reference types, hence the redirection
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static ref string? AsString(ref _StringData data) => ref Unsafe.As<_Intrinsic, string?>(ref data.Object);

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static ref StringBuilder AsStringBuilder(ref _StringData data) => ref Unsafe.As<_Intrinsic, StringBuilder?>(ref data.Object)!;

		// Only used to check if the instance is null
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static ref object? AsObject(ref _StringData data) => ref Unsafe.As<_Intrinsic, object?>(ref data.Object);
	}

	internal enum _String {
		String,
		StringBuilder
	}
}
