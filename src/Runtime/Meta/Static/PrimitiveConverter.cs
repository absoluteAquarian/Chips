using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Chips.Runtime.Meta {
	internal static class PrimitiveConverter {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _Address CastToAddress<T>(in _NumberInteger value) where T : struct, INumberBase<T> {
			if (typeof(T) == typeof(nint) || typeof(T) == typeof(nuint)) {
				return value.type switch {
					_Integer.Int32 => Unsafe.AsRef(in value).Convert<_NumberInteger, int, T, _Address>(),
					_Integer.Byte => Unsafe.AsRef(in value).Convert<_NumberInteger, byte, T, _Address>(),
					_Integer.SByte => Unsafe.AsRef(in value).Convert<_NumberInteger, sbyte, T, _Address>(),
					_Integer.Int16 => Unsafe.AsRef(in value).Convert<_NumberInteger, short, T, _Address>(),
					_Integer.UInt16 => Unsafe.AsRef(in value).Convert<_NumberInteger, ushort, T, _Address>(),
					_Integer.UInt32 => Unsafe.AsRef(in value).Convert<_NumberInteger, uint, T, _Address>(),
					_Integer.Int64 => Unsafe.AsRef(in value).Convert<_NumberInteger, long, T, _Address>(),
					_Integer.UInt64 => Unsafe.AsRef(in value).Convert<_NumberInteger, ulong, T, _Address>(),
					_Integer.Char => throw new InvalidNumberOperandException<char>(),
					_ => throw new InvalidObjectStateException(nameof(value))
				};
			}

			throw new InvalidConversionException<T>("Address");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _Address CastToAddress<T>(in _TokenHandle value) where T : struct, INumberBase<T> {
			if (typeof(T) == typeof(nint) || typeof(T) == typeof(nuint)) {
				return value.type switch {
					_Token.Type => ObjectConverter.Wrap<nint, _Address>(value.data.TypeHandle.Value),
					_Token.Method => ObjectConverter.Wrap<nint, _Address>(value.data.MethodHandle.Value),
					_Token.Field => ObjectConverter.Wrap<nint, _Address>(value.data.FieldHandle.Value),
					_ => throw new InvalidObjectStateException(nameof(value))
				};
			}

			throw new InvalidConversionException<T>("Address");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _NumberInteger CastToInteger<T>(in _Address value) where T : struct, INumberBase<T> {
			if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(byte) || typeof(T) == typeof(short) || typeof(T) == typeof(ushort)
			|| typeof(T) == typeof(int) || typeof(T) == typeof(uint) || typeof(T) == typeof(long) || typeof(T) == typeof(ulong)) {
				return value.type switch {
					_NativeInt.NInt => Unsafe.AsRef(in value).Convert<_Address, nint, T, _NumberInteger>(),
					_NativeInt.NUInt => Unsafe.AsRef(in value).Convert<_Address, nuint, T, _NumberInteger>(),
					_ => throw new InvalidObjectStateException(nameof(value))
				};
			} else if (typeof(T) == typeof(char))
				throw new CannotConvertTypeException<char>("Address");

			throw new InvalidConversionException<T>("Integer");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _NumberInteger CastToInteger<T>(in _NumberFloat value) where T : struct, INumberBase<T> {
			if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(byte) || typeof(T) == typeof(short) || typeof(T) == typeof(ushort)
			|| typeof(T) == typeof(int) || typeof(T) == typeof(uint) || typeof(T) == typeof(long) || typeof(T) == typeof(ulong)) {
				return value.type switch {
					_Float.Single => Unsafe.AsRef(in value).Convert<_NumberFloat, float, T, _NumberInteger>(),
					_Float.Double => Unsafe.AsRef(in value).Convert<_NumberFloat, double, T, _NumberInteger>(),
					_Float.Decimal => Unsafe.AsRef(in value).Convert<_NumberFloat, decimal, T, _NumberInteger>(),
					_Float.Half => Unsafe.AsRef(in value).Convert<_NumberFloat, Half, T, _NumberInteger>(),
					_ => throw new InvalidObjectStateException(nameof(value))
				};
			} else if (typeof(T) == typeof(char))
				throw new CannotConvertTypeException<char>("Float");

			throw new InvalidConversionException<T>("Integer");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _NumberFloat CastToFloat<T>(in _NumberInteger value) where T : struct, INumberBase<T> {
			if (typeof(T) == typeof(float) || typeof(T) == typeof(double) || typeof(T) == typeof(decimal) || typeof(T) == typeof(Half)) {
				return value.type switch {
					_Integer.Int32 => Unsafe.AsRef(in value).Convert<_NumberInteger, int, T, _NumberFloat>(),
					_Integer.Byte => Unsafe.AsRef(in value).Convert<_NumberInteger, byte, T, _NumberFloat>(),
					_Integer.SByte => Unsafe.AsRef(in value).Convert<_NumberInteger, sbyte, T, _NumberFloat>(),
					_Integer.Int16 => Unsafe.AsRef(in value).Convert<_NumberInteger, short, T, _NumberFloat>(),
					_Integer.UInt16 => Unsafe.AsRef(in value).Convert<_NumberInteger, ushort, T, _NumberFloat>(),
					_Integer.UInt32 => Unsafe.AsRef(in value).Convert<_NumberInteger, uint, T, _NumberFloat>(),
					_Integer.Int64 => Unsafe.AsRef(in value).Convert<_NumberInteger, long, T, _NumberFloat>(),
					_Integer.UInt64 => Unsafe.AsRef(in value).Convert<_NumberInteger, ulong, T, _NumberFloat>(),
					_Integer.Char => throw new InvalidNumberOperandException<char>(),
					_ => throw new InvalidObjectStateException(nameof(value))
				};
			}

			throw new InvalidConversionException<T>("Float");
		}
	}
}
