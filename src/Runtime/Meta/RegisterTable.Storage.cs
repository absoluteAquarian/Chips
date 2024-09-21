using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Chips.Runtime.Meta {
	partial class RegisterTable {
		private const int LARGE_POINTER = sizeof(ulong);  // Ensure 8 bytes in the event of a 32-bit platform

		[StructLayout(LayoutKind.Explicit, Size = sizeof(ulong))]
		private readonly struct _IntegerData {
			[FieldOffset(0)] public readonly sbyte SByte;
			[FieldOffset(0)] public readonly byte Byte;
			[FieldOffset(0)] public readonly short Int16;
			[FieldOffset(0)] public readonly ushort UInt16;
			[FieldOffset(0)] public readonly int Int32;
			[FieldOffset(0)] public readonly uint UInt32;
			[FieldOffset(0)] public readonly long Int64;
			[FieldOffset(0)] public readonly ulong UInt64;
			[FieldOffset(0)] public readonly char Char;
		}

		[StructLayout(LayoutKind.Explicit, Size = LARGE_POINTER)]
		private readonly struct _AddressData {
			[FieldOffset(0)] public readonly nint IntPtr;
			[FieldOffset(0)] public readonly nuint UIntPtr;
		}

		[StructLayout(LayoutKind.Explicit, Size = sizeof(decimal))]
		private readonly struct _FloatData {
			[FieldOffset(0)] public readonly float Single;
			[FieldOffset(0)] public readonly double Double;
			[FieldOffset(0)] public readonly decimal Decimal;
		}

		[StructLayout(LayoutKind.Explicit, Size = sizeof(decimal))]  // Decimal and Vector<T> are both 128-bit types
		private readonly struct _VariantObjectData {
			[StructLayout(LayoutKind.Explicit, Size = sizeof(ulong))]
			private readonly struct _Intrinsic { }

			[FieldOffset(0)] public readonly _IntegerData Integer;
			[FieldOffset(0)] public readonly _AddressData Address;
			[FieldOffset(0)] public readonly _FloatData Float;
			[FieldOffset(0)] private readonly _Intrinsic Object;
			[FieldOffset(0)] public readonly _VectorData Vector;
			[FieldOffset(0)] public readonly _MetadataTokenData Token;

			// "object" cannot be at the same FieldOffset as non-reference types, hence the redirection
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static ref readonly object GetObject(ref _VariantObjectData data) => ref Unsafe.As<_VariantObjectData, object>(ref data);
		}

		[StructLayout(LayoutKind.Explicit, Size = SIZE)]
		private readonly struct _VectorData {
			private const int SIZE = sizeof(ulong) * 2;  // Vector<T> has two ulong fields

			[StructLayout(LayoutKind.Explicit, Size = SIZE)]
			private readonly struct _Intrinsic { }

			[FieldOffset(0)] public readonly Vector2 Vector2;
			[FieldOffset(0)] public readonly Vector3 Vector3;
			[FieldOffset(0)] public readonly Vector4 Vector4;
			[FieldOffset(0)] private readonly _Intrinsic VectorT;

			// This struct needs to not have a generic type parameter, so a redirection is necessary
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static ref readonly Vector<T> GetVector<T>(ref _VectorData data) where T : struct => ref Unsafe.As<_VectorData, Vector<T>>(ref data);
		}

		[StructLayout(LayoutKind.Explicit, Size = LARGE_POINTER)]  // Force 8 bytes in the event of a 32-bit platform
		private readonly struct _MetadataTokenData {
			[FieldOffset(0)] public readonly RuntimeTypeHandle TypeHandle;
			[FieldOffset(0)] public readonly RuntimeMethodHandle MethodHandle;
			[FieldOffset(0)] public readonly RuntimeFieldHandle FieldHandle;
		}
	}
}
