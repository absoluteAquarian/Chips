using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Chips.Runtime.Meta {
	internal static class ObjectConverter {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TObjectTo Convert<TObjectFrom, TIntermediaryFrom, TIntermediaryTo, TObjectTo>(this ref TObjectFrom source)
		where TObjectFrom : struct, ICreateObject<TObjectFrom>
		where TIntermediaryFrom : struct, INumberBase<TIntermediaryFrom>
		where TIntermediaryTo : struct, INumberBase<TIntermediaryTo>
		where TObjectTo : struct, ICreateObject<TObjectTo>
			=> TObjectTo.Create(TIntermediaryTo.CreateChecked(Unsafe.As<TObjectFrom, TIntermediaryFrom>(ref source)));

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static TTo Wrap<TFrom, TTo>(in TFrom? value) where TTo : struct, IChipsObject, ICreateObject<TTo> => TTo.Create(value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TypedRegister<_Address> AsAddress<T>(this in TypedRegister<T> register, Span<_Address> stackAlloc1) where T : struct, IConvertToAddress<T> => new TypedRegister<_Address>(register.register, ref T.AsAddress(register.value, stackAlloc1));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TypedRegister<_NumberInteger> AsInteger<T>(this in TypedRegister<T> register, Span<_NumberInteger> stackAlloc1) where T : struct, IConvertToInteger<T> => new TypedRegister<_NumberInteger>(register.register, ref T.AsInteger(register.value, stackAlloc1));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TypedRegister<_NumberFloat> AsFloat<T>(this in TypedRegister<T> register, Span<_NumberFloat> stackAlloc1) where T : struct, IConvertToFloat<T> => new TypedRegister<_NumberFloat>(register.register, ref T.AsFloat(register.value, stackAlloc1));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TypedRegister<_StringObject> AsString<T>(this in TypedRegister<T> register, Span<_StringObject> stackAlloc1) where T : struct, IConvertToString<T> => new TypedRegister<_StringObject>(register.register, ref T.AsString(register.value, stackAlloc1));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TypedRegister<_Object> AsObject<T>(this in TypedRegister<T> register, Span<_Object> stackAlloc1) where T : struct, IConvertToObject<T> => new TypedRegister<_Object>(register.register, ref T.AsObject(register.value, stackAlloc1));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TypedRegister<_VectorObject> AsVector<T>(this in TypedRegister<T> register, Span<_VectorObject> stackAlloc1) where T : struct, IConvertToVector<T> => new TypedRegister<_VectorObject>(register.register, ref T.AsVector(register.value, stackAlloc1));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TypedRegister<_TokenHandle> AsToken<T>(this in TypedRegister<T> register, Span<_TokenHandle> stackAlloc1) where T : struct, IConvertToToken<T> => new TypedRegister<_TokenHandle>(register.register, ref T.AsToken(register.value, stackAlloc1));

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _Address UnwrapAddress<T>(this T value) where T : struct, IChipsObject, IConvertToAddress<T> => T.AsAddress(in value, stackalloc _Address[1]);

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _NumberInteger UnwrapInteger<T>(this T value) where T : struct, IChipsObject, IConvertToInteger<T> => T.AsInteger(in value, stackalloc _NumberInteger[1]);

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _NumberFloat UnwrapFloat<T>(this T value) where T : struct, IChipsObject, IConvertToFloat<T> => T.AsFloat(in value, stackalloc _NumberFloat[1]);

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _StringObject UnwrapString<T>(this T value) where T : struct, IChipsObject, IConvertToString<T> => T.AsString(in value, stackalloc _StringObject[1]);

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _Object UnwrapObject<T>(this T value) where T : struct, IChipsObject, IConvertToObject<T> => T.AsObject(in value, stackalloc _Object[1]);

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VectorObject UnwrapVector<T>(this T value) where T : struct, IChipsObject, IConvertToVector<T> => T.AsVector(in value, stackalloc _VectorObject[1]);

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _TokenHandle UnwrapToken<T>(this T value) where T : struct, IChipsObject, IConvertToToken<T> => T.AsToken(in value, stackalloc _TokenHandle[1]);

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _ExceptionObject UnwrapException<T>(this T value) where T : struct, IChipsObject, IConvertToException<T> => T.AsException(in value, stackalloc _ExceptionObject[1]);

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _StatusObject UnwrapStatus<T>(this T value) where T : struct, IChipsObject, IConvertToStatus<T> => T.AsStatus(in value, stackalloc _StatusObject[1]);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _Address AsAddress<T>(this ref T value) where T : struct {
			if (typeof(T) == typeof(nint) || typeof(T) == typeof(nuint))
				return Wrap<T, _Address>(value);
			else if (typeof(T) == typeof(_VariantObject))
				return Unsafe.As<T, _VariantObject>(ref value).UnwrapAddress();

			throw new InvalidConversionException<T>("Address");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _NumberInteger AsInteger<T>(this ref T value) where T : struct {
			if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(byte) || typeof(T) == typeof(short) || typeof(T) == typeof(ushort)
			|| typeof(T) == typeof(int) || typeof(T) == typeof(uint) || typeof(T) == typeof(long) || typeof(T) == typeof(ulong)
			|| typeof(T) == typeof(char))
				return Wrap<T, _NumberInteger>(value);
			else if (typeof(T) == typeof(_VariantObject))
				return Unsafe.As<T, _VariantObject>(ref value).UnwrapInteger();

			throw new InvalidConversionException<T>("Integer");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _NumberFloat AsFloat<T>(this ref T value) where T : struct {
			if (typeof(T) == typeof(float) || typeof(T) == typeof(double) || typeof(T) == typeof(decimal) || typeof(T) == typeof(Half))
				return Wrap<T, _NumberFloat>(value);
			else if (typeof(T) == typeof(_VariantObject))
				return Unsafe.As<T, _VariantObject>(ref value).UnwrapFloat();

			throw new InvalidConversionException<T>("Float");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _StringObject AsString<T>(this ref T value) where T : struct {
			if (typeof(T) == typeof(_VariantObject))
				return Unsafe.As<T, _VariantObject>(ref value).UnwrapString();

			throw new InvalidConversionException<T>("StringObject");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _StringObject AsString<T>(this T? value) where T : class {
			if (typeof(T) == typeof(string) || typeof(T) == typeof(StringBuilder))
				return Wrap<T, _StringObject>(value);

			throw new InvalidConversionException<T>("StringObject");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _Object AsObject<T>(this ref T value) where T : struct => Wrap<T, _Object>(value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _Object AsObject<T>(this T? value) where T : class => Wrap<T, _Object>(value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _VectorObject AsVector<T>(this ref T value) where T : struct {
			if (typeof(T) == typeof(Vector2) || typeof(T) == typeof(Vector3) || typeof(T) == typeof(Vector4)
			|| typeof(T) == typeof(Vector<byte>) || typeof(T) == typeof(Vector<double>) || typeof(T) == typeof(Vector<short>)
			|| typeof(T) == typeof(Vector<int>) || typeof(T) == typeof(Vector<long>) || typeof(T) == typeof(Vector<nint>)
			|| typeof(T) == typeof(Vector<nuint>) || typeof(T) == typeof(Vector<sbyte>) || typeof(T) == typeof(Vector<float>)
			|| typeof(T) == typeof(Vector<ushort>) || typeof(T) == typeof(Vector<uint>) || typeof(T) == typeof(Vector<ulong>))
				return Wrap<T, _VectorObject>(value);
			else if (typeof(T) == typeof(_VariantObject))
				return Unsafe.As<T, _VariantObject>(ref value).UnwrapVector();

			throw new InvalidConversionException<T>("VectorObject");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _TokenHandle AsToken<T>(this ref T value) where T : struct {
			if (typeof(T) == typeof(RuntimeTypeHandle) || typeof(T) == typeof(RuntimeMethodHandle) || typeof(T) == typeof(RuntimeFieldHandle))
				return Wrap<T, _TokenHandle>(value);
			else if (typeof(T) == typeof(_VariantObject))
				return Unsafe.As<T, _VariantObject>(ref value).UnwrapToken();

			throw new InvalidConversionException<T>("MetadataTokenHandle");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _VariantObject AsVariant<T>(this T value) => Wrap<T, _VariantObject>(value);  // VariantObject has no restrictions, and any special cases are handled by separate code paths

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _ExceptionObject AsException<T>(this ref T value) where T : struct {
			if (typeof(T) == typeof(_VariantObject))
				return Unsafe.As<T, _VariantObject>(ref value).UnwrapException();

			throw new InvalidConversionException<T>("ExceptionObject");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _ExceptionObject AsException<T>(this T? value) where T : class {
			if (typeof(Exception).IsAssignableFrom(typeof(T)))
				return Wrap<T, _ExceptionObject>(value);

			throw new InvalidConversionException<T>("ExceptionObject");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static _StatusObject AsStatus<T>(this ref T value) where T : struct {
			if (typeof(T) == typeof(Status))
				return Wrap<T, _StatusObject>(value);
			else if (typeof(T) == typeof(_VariantObject))
				return Unsafe.As<T, _VariantObject>(ref value).UnwrapStatus();

			throw new InvalidConversionException<T>("StatusObject");
		}
	}
}
