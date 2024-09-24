using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Chips.Runtime.Meta {
	internal static class TypeMetrics {
		public const int LARGE_POINTER = sizeof(ulong);  // Ensure 8 bytes in the event of a 32-bit platform

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _Integer TypeToInteger<T>() {
			if (typeof(T) == typeof(byte)) return _Integer.Byte;
			if (typeof(T) == typeof(sbyte)) return _Integer.SByte;
			if (typeof(T) == typeof(short)) return _Integer.Int16;
			if (typeof(T) == typeof(ushort)) return _Integer.UInt16;
			if (typeof(T) == typeof(int)) return _Integer.Int32;
			if (typeof(T) == typeof(uint)) return _Integer.UInt32;
			if (typeof(T) == typeof(long)) return _Integer.Int64;
			if (typeof(T) == typeof(ulong)) return _Integer.UInt64;
			if (typeof(T) == typeof(char)) return _Integer.Char;

			throw new InvalidTypeToRegisterEnumException<_Integer, T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TypeToIntegerSubtype<T>() => (int)TypeToInteger<T>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _NativeInt TypeToNativeInt<T>() {
			if (typeof(T) == typeof(nint)) return _NativeInt.NInt;
			if (typeof(T) == typeof(nuint)) return _NativeInt.NUInt;

			throw new InvalidTypeToRegisterEnumException<_NativeInt, T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TypeToNativeIntSubtype<T>() => (int)TypeToNativeInt<T>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _Float TypeToFloat<T>() {
			if (typeof(T) == typeof(float)) return _Float.Single;
			if (typeof(T) == typeof(double)) return _Float.Double;
			if (typeof(T) == typeof(decimal)) return _Float.Decimal;
			if (typeof(T) == typeof(Half)) return _Float.Half;

			throw new InvalidTypeToRegisterEnumException<_Float, T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TypeToFloatSubtype<T>() => (int)TypeToFloat<T>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _String TypeToStringObject<T>() {
			if (typeof(T) == typeof(string)) return _String.String;
			if (typeof(T) == typeof(StringBuilder)) return _String.StringBuilder;

			throw new InvalidTypeToRegisterEnumException<_String, T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TypeToStringObjectSubtype<T>() => (int)TypeToStringObject<T>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _ObjectType TypeToObject<T>() => typeof(T).IsValueType ? _ObjectType.Valuetype : _ObjectType.Reference;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TypeToObjectSubtype<T>() => (int)TypeToObject<T>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _Vector TypeToVector<T>() {
			if (typeof(T) == typeof(Vector2)) return _Vector.Vector2;
			if (typeof(T) == typeof(Vector3)) return _Vector.Vector3;
			if (typeof(T) == typeof(Vector4)) return _Vector.Vector4;
			if (typeof(T) == typeof(Vector<byte>)) return _Vector.VectorT_Byte;
			if (typeof(T) == typeof(Vector<double>)) return _Vector.VectorT_Double;
			if (typeof(T) == typeof(Vector<short>)) return _Vector.VectorT_Int16;
			if (typeof(T) == typeof(Vector<int>)) return _Vector.VectorT_Int32;
			if (typeof(T) == typeof(Vector<long>)) return _Vector.VectorT_Int64;
			if (typeof(T) == typeof(Vector<nint>)) return _Vector.VectorT_IntPtr;
			if (typeof(T) == typeof(Vector<nuint>)) return _Vector.VectorT_UIntPtr;
			if (typeof(T) == typeof(Vector<sbyte>)) return _Vector.VectorT_SByte;
			if (typeof(T) == typeof(Vector<float>)) return _Vector.VectorT_Single;
			if (typeof(T) == typeof(Vector<ushort>)) return _Vector.VectorT_UInt16;
			if (typeof(T) == typeof(Vector<uint>)) return _Vector.VectorT_UInt32;
			if (typeof(T) == typeof(Vector<ulong>)) return _Vector.VectorT_UInt64;

			throw new InvalidTypeToRegisterEnumException<_Vector, T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TypeToVectorSubtype<T>() => (int)TypeToVector<T>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _Token TypeToMetadataToken<T>() {
			if (typeof(T) == typeof(RuntimeTypeHandle)) return _Token.Type;
			if (typeof(T) == typeof(RuntimeMethodHandle)) return _Token.Method;
			if (typeof(T) == typeof(RuntimeFieldHandle)) return _Token.Field;

			throw new InvalidTypeToRegisterEnumException<_Token, T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TypeToMetadataTokenSubtype<T>() => (int)TypeToMetadataToken<T>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _Variant TypeToVariant<T>() {
			if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(byte) || typeof(T) == typeof(short) || typeof(T) == typeof(ushort)
			|| typeof(T) == typeof(int) || typeof(T) == typeof(uint) || typeof(T) == typeof(long) || typeof(T) == typeof(ulong)
			|| typeof(T) == typeof(char))
				return _Variant.Integer;

			if (typeof(T) == typeof(nint) || typeof(T) == typeof(nuint))
				return _Variant.Address;

			if (typeof(T) == typeof(float) || typeof(T) == typeof(double) || typeof(T) == typeof(decimal) || typeof(T) == typeof(Half))
				return _Variant.Float;

			if (typeof(T) == typeof(string) || typeof(T) == typeof(StringBuilder))
				return _Variant.String;

			if (typeof(T) == typeof(RuntimeTypeHandle) || typeof(T) == typeof(RuntimeMethodHandle) || typeof(T) == typeof(RuntimeFieldHandle))
				return _Variant.Token;
			
			if (typeof(T) == typeof(Vector2) || typeof(T) == typeof(Vector3) || typeof(T) == typeof(Vector4)
			|| typeof(T) == typeof(Vector<byte>) || typeof(T) == typeof(Vector<double>) || typeof(T) == typeof(Vector<short>)
			|| typeof(T) == typeof(Vector<int>) || typeof(T) == typeof(Vector<long>) || typeof(T) == typeof(Vector<nint>)
			|| typeof(T) == typeof(Vector<nuint>) || typeof(T) == typeof(Vector<sbyte>) || typeof(T) == typeof(Vector<float>)
			|| typeof(T) == typeof(Vector<ushort>) || typeof(T) == typeof(Vector<uint>) || typeof(T) == typeof(Vector<ulong>))
				return _Variant.Vector;

			return _Variant.Object;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TypeToVariantSubtype<T>() {
			if (typeof(T) == typeof(byte) || typeof(T) == typeof(sbyte) || typeof(T) == typeof(short) || typeof(T) == typeof(ushort)
			|| typeof(T) == typeof(int) || typeof(T) == typeof(uint) || typeof(T) == typeof(long) || typeof(T) == typeof(ulong)
			|| typeof(T) == typeof(char))
				return TypeToIntegerSubtype<T>();

			if (typeof(T) == typeof(nint) || typeof(T) == typeof(nuint))
				return TypeToNativeIntSubtype<T>();

			if (typeof(T) == typeof(float) || typeof(T) == typeof(double) || typeof(T) == typeof(decimal) || typeof(T) == typeof(Half))
				return TypeToFloatSubtype<T>();

			if (typeof(T) == typeof(string) || typeof(T) == typeof(StringBuilder))
				return TypeToStringObjectSubtype<T>();

			if (typeof(T) == typeof(RuntimeTypeHandle) || typeof(T) == typeof(RuntimeMethodHandle) || typeof(T) == typeof(RuntimeFieldHandle))
				return TypeToMetadataTokenSubtype<T>();

			if (typeof(T) == typeof(Vector2) || typeof(T) == typeof(Vector3) || typeof(T) == typeof(Vector4)
			|| typeof(T) == typeof(Vector<byte>) || typeof(T) == typeof(Vector<double>) || typeof(T) == typeof(Vector<short>)
			|| typeof(T) == typeof(Vector<int>) || typeof(T) == typeof(Vector<long>) || typeof(T) == typeof(Vector<nint>)
			|| typeof(T) == typeof(Vector<nuint>) || typeof(T) == typeof(Vector<sbyte>) || typeof(T) == typeof(Vector<float>)
			|| typeof(T) == typeof(Vector<ushort>) || typeof(T) == typeof(Vector<uint>) || typeof(T) == typeof(Vector<ulong>))
				return TypeToVectorSubtype<T>();

			return TypeToObjectSubtype<T>();
		}
	}
}
