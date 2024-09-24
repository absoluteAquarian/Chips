using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Chips.Runtime.Meta {
	internal struct _TokenHandle : IChipsObject, ICreateObject<_TokenHandle>, IConvertToToken<_TokenHandle> {
		public _TokenData data;
		public _Token type;

		[MethodImpl(MethodImplOptions.AggressiveInlining)] static ref _TokenHandle IConvertToToken<_TokenHandle>.AsToken(in _TokenHandle value, Span<_TokenHandle> stackAlloc1) => ref Unsafe.AsRef(in value);

		static _TokenHandle ICreateObject<_TokenHandle>.Create<U>(in U value) {
			if (typeof(U) == typeof(_TokenHandle))
				return Unsafe.As<U, _TokenHandle>(ref Unsafe.AsRef(in value));

			_TokenHandle obj = default;
			obj.type = TypeMetrics.TypeToMetadataToken<U>();
			Unsafe.As<_TokenHandle, U>(ref obj) = value;
			return obj;
		}
	}

	[StructLayout(LayoutKind.Explicit, Size = TypeMetrics.LARGE_POINTER)]  // Force 8 bytes in the event of a 32-bit platform
	internal struct _TokenData {
		[FieldOffset(0)] public RuntimeTypeHandle TypeHandle;
		[FieldOffset(0)] public RuntimeMethodHandle MethodHandle;
		[FieldOffset(0)] public RuntimeFieldHandle FieldHandle;
	}

	internal enum _Token {
		Type,
		Method,
		Field
	}
}
