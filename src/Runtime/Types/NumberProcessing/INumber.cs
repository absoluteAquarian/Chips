namespace Chips.Runtime.Types.NumberProcessing {
	/// <summary>
	/// The base interface for all numeric types that Chips supports
	/// </summary>
	public interface INumber {
		object Value { get; }

		bool IsZero { get; }

		bool IsNegative { get; }

		int NumericSize { get; }

		INumber Upcast(INumber number);

		INumber Abs();

		INumber Add(INumber number);

		void Compare(INumber number);

		INumber Decrement();

		INumber Divide(INumber number);

		INumber Increment();

		INumber Modulus(INumber number);

		INumber Multiply(INumber number);

		INumber Negate();

		INumber Subtract(INumber number);
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
