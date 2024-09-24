using System.Runtime.CompilerServices;
using System;
using System.Runtime.InteropServices;

namespace Chips.Runtime.Meta {
	internal struct _NumberInteger : IChipsObject, IMutableInteger, ICreateObject<_NumberInteger>, IConvertToInteger<_NumberInteger> {
		public _IntegerData data;
		public _Integer type;

		[MethodImpl(MethodImplOptions.AggressiveInlining)] static ref _NumberInteger IConvertToInteger<_NumberInteger>.AsInteger(in _NumberInteger value, Span<_NumberInteger> stackAlloc1) => ref Unsafe.AsRef(in value);

		static _NumberInteger ICreateObject<_NumberInteger>.Create<U>(in U value) {
			if (typeof(U) == typeof(_NumberInteger))
				return Unsafe.As<U, _NumberInteger>(ref Unsafe.AsRef(in value));

			_NumberInteger obj = default;
			obj.type = TypeMetrics.TypeToInteger<U>();
			Unsafe.As<_NumberInteger, U>(ref obj) = value;
			return obj;
		}
	}

	[StructLayout(LayoutKind.Explicit, Size = sizeof(ulong))]
	internal struct _IntegerData {
		[FieldOffset(0)] public sbyte SByte;
		[FieldOffset(0)] public byte Byte;
		[FieldOffset(0)] public short Int16;
		[FieldOffset(0)] public ushort UInt16;
		[FieldOffset(0)] public int Int32;
		[FieldOffset(0)] public uint UInt32;
		[FieldOffset(0)] public long Int64;
		[FieldOffset(0)] public ulong UInt64;
		[FieldOffset(0)] public char Char;
	}

	internal enum _Integer {
		Int32,  // Int unordered to make it the default value
		Byte,
		SByte,
		Int16,
		UInt16,
		UInt32,
		Int64,
		UInt64,
		Char
	}
}
