namespace Chips.Runtime.Specifications {
	public enum ArithmeticOperation {
		/// <summary>
		/// a + b = c
		/// </summary>
		Addition = 0,
		/// <summary>
		/// a - b = c
		/// </summary>
		Subtraction,
		/// <summary>
		/// a * b = c
		/// </summary>
		Multiplication,
		/// <summary>
		/// a / b = c
		/// </summary>
		Division,
		/// <summary>
		/// a % b = c<br/>
		/// result is cyclical from 0 to b and is affected by the sign of a<br/>
		/// E.g. -2.5 % 10 = -2.5
		/// </summary>
		Modulo,
		/// <summary>
		/// a % b = c<br/>
		/// result is cyclical from 0 to b<br/>
		/// E.g. -2.5 % 10 = 7.5
		/// </summary>
		Repeat,
		/// <summary>
		/// a = a + 1
		/// </summary>
		Increment,
		/// <summary>
		/// a = a - 1
		/// </summary>
		Decrement,
		/// <summary>
		/// a = -1 * a
		/// </summary>
		Negation
	}
}
