using System;
using System.Numerics;

namespace Chips.Runtime.Meta {
	partial class RegisterTable {
		private enum _Integer {
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

		private static _Integer TypeToInteger<T>() {
			if (typeof(T) == typeof(byte))
				return _Integer.Byte;
			if (typeof(T) == typeof(sbyte))
				return _Integer.SByte;
			if (typeof(T) == typeof(short))
				return _Integer.Int16;
			if (typeof(T) == typeof(ushort))
				return _Integer.UInt16;
			if (typeof(T) == typeof(int))
				return _Integer.Int32;
			if (typeof(T) == typeof(uint))
				return _Integer.UInt32;
			if (typeof(T) == typeof(long))
				return _Integer.Int64;
			if (typeof(T) == typeof(ulong))
				return _Integer.UInt64;
			if (typeof(T) == typeof(char))
				return _Integer.Char;

			throw new InvalidTypeToRegisterEnumException<_Integer, T>();
		}

		private enum _NativeInt {
			NInt,
			NUInt
		}

		private static _NativeInt TypeToNativeInt<T>() {
			if (typeof(T) == typeof(nint))
				return _NativeInt.NInt;
			if (typeof(T) == typeof(nuint))
				return _NativeInt.NUInt;

			throw new InvalidTypeToRegisterEnumException<_NativeInt, T>();
		}

		private enum _Float {
			Single,
			Double,
			Decimal
		}

		private static _Float TypeToFloat<T>() {
			if (typeof(T) == typeof(float))
				return _Float.Single;
			if (typeof(T) == typeof(double))
				return _Float.Double;
			if (typeof(T) == typeof(decimal))
				return _Float.Decimal;

			throw new InvalidTypeToRegisterEnumException<_Float, T>();
		}

		private enum _String {
			String,
			StringBuilder
		}

		private static _String TypeToStringObject<T>() {
			if (typeof(T) == typeof(string))
				return _String.String;
			if (typeof(T) == typeof(System.Text.StringBuilder))
				return _String.StringBuilder;

			throw new InvalidTypeToRegisterEnumException<_String, T>();
		}

		private enum _Variant {
			Integer,
			Address,
			Float,
			String,
			Object,
			Vector,
			Token
		}

		private enum _Vector {
			Vector2,
			Vector3,
			Vector4,
			VectorX
		}

		private static _Vector TypeToVector<T>() {
			if (typeof(T) == typeof(Vector2))
				return _Vector.Vector2;
			if (typeof(T) == typeof(Vector3))
				return _Vector.Vector3;
			if (typeof(T) == typeof(Vector4))
				return _Vector.Vector4;
			if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Vector<>))
				return _Vector.VectorX;

			throw new InvalidTypeToRegisterEnumException<_Vector, T>();
		}

		private enum _MetadataToken {
			Type,
			Method,
			Field
		}

		private static _MetadataToken TypeToMetadataToken<T>() {
			if (typeof(T) == typeof(RuntimeTypeHandle))
				return _MetadataToken.Type;
			if (typeof(T) == typeof(RuntimeMethodHandle))
				return _MetadataToken.Method;
			if (typeof(T) == typeof(RuntimeFieldHandle))
				return _MetadataToken.Field;

			throw new InvalidTypeToRegisterEnumException<_MetadataToken, T>();
		}
	}
}
