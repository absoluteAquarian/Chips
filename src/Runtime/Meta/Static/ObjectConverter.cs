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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TypedRegister<_ExceptionObject> AsException<T>(this in TypedRegister<T> register, Span<_ExceptionObject> stackAlloc1) where T : struct, IConvertToException<T> => new TypedRegister<_ExceptionObject>(register.register, ref T.AsException(register.value, stackAlloc1));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TypedRegister<_StatusObject> AsStatus<T>(this in TypedRegister<T> register, Span<_StatusObject> stackAlloc1) where T : struct, IConvertToStatus<T> => new TypedRegister<_StatusObject>(register.register, ref T.AsStatus(register.value, stackAlloc1));

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _Address UnwrapAddress<T>(this T value) where T : struct, IChipsObject, IConvertToAddress<T> => T.AsAddress(in value, stackalloc _Address[1]);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _NumberInteger UnwrapInteger<T>(this T value) where T : struct, IChipsObject, IConvertToInteger<T> => T.AsInteger(in value, stackalloc _NumberInteger[1]);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _NumberFloat UnwrapFloat<T>(this T value) where T : struct, IChipsObject, IConvertToFloat<T> => T.AsFloat(in value, stackalloc _NumberFloat[1]);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _StringObject UnwrapString<T>(this T value) where T : struct, IChipsObject, IConvertToString<T> => T.AsString(in value, stackalloc _StringObject[1]);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _Object UnwrapObject<T>(this T value) where T : struct, IChipsObject, IConvertToObject<T> => T.AsObject(in value, stackalloc _Object[1]);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VectorObject UnwrapVector<T>(this T value) where T : struct, IChipsObject, IConvertToVector<T> => T.AsVector(in value, stackalloc _VectorObject[1]);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _TokenHandle UnwrapToken<T>(this T value) where T : struct, IChipsObject, IConvertToToken<T> => T.AsToken(in value, stackalloc _TokenHandle[1]);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _ExceptionObject UnwrapException<T>(this T value) where T : struct, IChipsObject, IConvertToException<T> => T.AsException(in value, stackalloc _ExceptionObject[1]);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _StatusObject UnwrapStatus<T>(this T value) where T : struct, IChipsObject, IConvertToStatus<T> => T.AsStatus(in value, stackalloc _StatusObject[1]);

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _Address AsAddress(this in nint value) => Wrap<nint, _Address>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _Address AsAddress(this in nuint value) => Wrap<nuint, _Address>(value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _NumberInteger AsInteger(this in int value) => Wrap<int, _NumberInteger>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _NumberInteger AsInteger(this in byte value) => Wrap<byte, _NumberInteger>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _NumberInteger AsInteger(this in sbyte value) => Wrap<sbyte, _NumberInteger>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _NumberInteger AsInteger(this in short value) => Wrap<short, _NumberInteger>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _NumberInteger AsInteger(this in ushort value) => Wrap<ushort, _NumberInteger>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _NumberInteger AsInteger(this in uint value) => Wrap<uint, _NumberInteger>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _NumberInteger AsInteger(this in long value) => Wrap<long, _NumberInteger>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _NumberInteger AsInteger(this in ulong value) => Wrap<ulong, _NumberInteger>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _NumberInteger AsInteger(this in char value) => Wrap<char, _NumberInteger>(value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _NumberFloat AsFloat(this in float value) => Wrap<float, _NumberFloat>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _NumberFloat AsFloat(this in double value) => Wrap<double, _NumberFloat>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _NumberFloat AsFloat(this in decimal value) => Wrap<decimal, _NumberFloat>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _NumberFloat AsFloat(this in Half value) => Wrap<Half, _NumberFloat>(value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _StringObject AsString(this string? value) => Wrap<string, _StringObject>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _StringObject AsString(this StringBuilder? value) => Wrap<StringBuilder, _StringObject>(value);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _Object AsObjectFromObject<T>(this T? value) where T : class => Wrap<T, _Object>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _Object AsObjectFromStruct<T>(this T value) where T : struct => Wrap<T, _Object>(value);  // JIT doesn't allow "this in" for generic struct arguments

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VectorObject AsVector(this in Vector2 value) => Wrap<Vector2, _VectorObject>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VectorObject AsVector(this in Vector3 value) => Wrap<Vector3, _VectorObject>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VectorObject AsVector(this in Vector4 value) => Wrap<Vector4, _VectorObject>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VectorObject AsVector(this in Vector<byte> value) => Wrap<Vector<byte>, _VectorObject>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VectorObject AsVector(this in Vector<double> value) => Wrap<Vector<double>, _VectorObject>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VectorObject AsVector(this in Vector<short> value) => Wrap<Vector<short>, _VectorObject>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VectorObject AsVector(this in Vector<int> value) => Wrap<Vector<int>, _VectorObject>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VectorObject AsVector(this in Vector<long> value) => Wrap<Vector<long>, _VectorObject>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VectorObject AsVector(this in Vector<nint> value) => Wrap<Vector<nint>, _VectorObject>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VectorObject AsVector(this in Vector<nuint> value) => Wrap<Vector<nuint>, _VectorObject>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VectorObject AsVector(this in Vector<sbyte> value) => Wrap<Vector<sbyte>, _VectorObject>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VectorObject AsVector(this in Vector<float> value) => Wrap<Vector<float>, _VectorObject>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VectorObject AsVector(this in Vector<ushort> value) => Wrap<Vector<ushort>, _VectorObject>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VectorObject AsVector(this in Vector<uint> value) => Wrap<Vector<uint>, _VectorObject>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VectorObject AsVector(this in Vector<ulong> value) => Wrap<Vector<ulong>, _VectorObject>(value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _TokenHandle AsToken(this in RuntimeTypeHandle value) => Wrap<RuntimeTypeHandle, _TokenHandle>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _TokenHandle AsToken(this in RuntimeMethodHandle value) => Wrap<RuntimeMethodHandle, _TokenHandle>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _TokenHandle AsToken(this in RuntimeFieldHandle value) => Wrap<RuntimeFieldHandle, _TokenHandle>(value);

		// VariantObject has no restrictions, and any special cases are handled by separate code paths
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VariantObject AsVariantFromObject<T>(this T? value) where T : class => Wrap<T, _VariantObject>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VariantObject AsVariantFromStruct<T>(this T value) where T : struct => Wrap<T, _VariantObject>(value);  // JIT doesn't allow "this in" for generic struct arguments
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VariantObject AsVariant(this in _Address value) => Wrap<_Address, _VariantObject>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VariantObject AsVariant(this in _NumberInteger value) => Wrap<_NumberInteger, _VariantObject>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VariantObject AsVariant(this in _NumberFloat value) => Wrap<_NumberFloat, _VariantObject>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VariantObject AsVariant(this in _StringObject value) => Wrap<_StringObject, _VariantObject>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VariantObject AsVariant(this in _Object value) => Wrap<_Object, _VariantObject>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VariantObject AsVariant(this in _VectorObject value) => Wrap<_VectorObject, _VariantObject>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VariantObject AsVariant(this in _TokenHandle value) => Wrap<_TokenHandle, _VariantObject>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VariantObject AsVariant(this in _ExceptionObject value) => Wrap<_ExceptionObject, _VariantObject>(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _VariantObject AsVariant(this in _StatusObject value) => Wrap<_StatusObject, _VariantObject>(value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _ExceptionObject AsException<T>(this Exception value) => Wrap<Exception, _ExceptionObject>(value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static _StatusObject AsStatus(this in Status value) => Wrap<Status, _StatusObject>(value);
	}
}
