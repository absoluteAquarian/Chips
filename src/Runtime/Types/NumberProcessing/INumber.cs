﻿namespace Chips.Runtime.Types.NumberProcessing {
	/// <summary>
	/// The base interface for all numeric types that Chips supports
	/// </summary>
	public interface INumber {
		object Value { get; }

		bool IsZero { get; }

		bool IsNegative { get; }

		int NumericSize { get; }

		INumber Cast(INumber number);

		INumber Upcast(INumber number);

		INumber Abs();

		INumber Add(INumber number);

		void Compare(INumber number);

		bool CompareEquals(INumber number);

		bool CompareGreaterThan(INumber number);

		bool CompareLessThan(INumber number);

		INumber Decrement();

		INumber Divide(INumber number);

		INumber Increment();

		INumber Modulus(INumber number);

		INumber Multiply(INumber number);

		INumber Negate();

		INumber Repeat(INumber number);

		INumber Subtract(INumber number);

		sbyte ToSByte();

		byte ToByte();

		short ToInt16();

		ushort ToUInt16();
		
		int ToInt32();

		uint ToUInt32();

		long ToInt64();

		ulong ToUInt64();

		nint ToIntPtr();

		nuint ToUIntPtr();

		float ToSingle();

		double ToDouble();

		decimal ToDecimal();
	}

	/// <summary>
	/// An extension of <see cref="INumber"/> for types that have a definite underlying value type
	/// </summary>
	/// <typeparam name="T">The type of the underlying value</typeparam>
	public interface INumber<T> : INumber {
		T ActualValue { get; }
	}

	/// <summary>
	/// An interface containing constants for a given numeric type
	/// </summary>
	public interface INumberConstants<T> where T : INumber, INumberConstants<T> {
		static abstract T Zero { get; }

		static abstract T One { get; }
	}

	/// <summary>
	/// An interface containing constants for a given floating-point type
	/// </summary>
	public interface IFloatConstants<T> : INumberConstants<T> where T : INumber, IFloatConstants<T> {
		static abstract T E { get; }
	}
}
