using Chips.Common.DataStructures;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Chips.Runtime.Meta {
	partial class RegisterTable {
		private const int LARGE_POINTER = sizeof(ulong);  // Ensure 8 bytes in the event of a 32-bit platform

		[StructLayout(LayoutKind.Explicit, Size = sizeof(ulong))]
		private struct _IntegerData {
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

		[StructLayout(LayoutKind.Explicit, Size = LARGE_POINTER)]
		private struct _AddressData {
			[FieldOffset(0)] public nint IntPtr;
			[FieldOffset(0)] public nuint UIntPtr;
		}

		[StructLayout(LayoutKind.Explicit, Size = sizeof(decimal))]
		private struct _FloatData {
			[FieldOffset(0)] public float Single;
			[FieldOffset(0)] public double Double;
			[FieldOffset(0)] public decimal Decimal;
		}

		[StructLayout(LayoutKind.Explicit, Size = _VectorData.SIZE)]  // Ensure largest possible size
		private struct _VariantObjectData {
			[StructLayout(LayoutKind.Explicit, Size = sizeof(ulong))]
			public struct _Intrinsic { }

			[FieldOffset(0)] public _IntegerData Integer;
			[FieldOffset(0)] public _AddressData Address;
			[FieldOffset(0)] public _FloatData Float;
			[FieldOffset(0)] private _Intrinsic Object;
			[FieldOffset(0)] public _VectorData Vector;
			[FieldOffset(0)] public _MetadataTokenData Token;

			public static ref T GetObject<T>(ref _VariantObjectData data) where T : class => ref Unsafe.As<_Intrinsic, T>(ref data.Object);
			public static ref Ref<T> GetStruct<T>(ref _VariantObjectData data) where T : struct => ref Unsafe.As<_Intrinsic, Ref<T>>(ref data.Object);
		}

		[StructLayout(LayoutKind.Explicit, Size = SIZE)]
		private struct _VectorData {
			public const int SIZE = sizeof(ulong) * 2;  // Vector<T> has two ulong fields

			[StructLayout(LayoutKind.Explicit, Size = SIZE)]
			public struct _Intrinsic { }

			[FieldOffset(0)] public Vector2 Vector2;
			[FieldOffset(0)] public Vector3 Vector3;
			[FieldOffset(0)] public Vector4 Vector4;
			[FieldOffset(0)] private _Intrinsic VectorT;

			public static ref Vector<T> GetVector<T>(ref _VectorData data) where T : struct => ref Unsafe.As<_Intrinsic, Vector<T>>(ref data.VectorT);
		}

		[StructLayout(LayoutKind.Explicit, Size = LARGE_POINTER)]  // Force 8 bytes in the event of a 32-bit platform
		private struct _MetadataTokenData {
			[FieldOffset(0)] public RuntimeTypeHandle TypeHandle;
			[FieldOffset(0)] public RuntimeMethodHandle MethodHandle;
			[FieldOffset(0)] public RuntimeFieldHandle FieldHandle;
		}
	}
}
