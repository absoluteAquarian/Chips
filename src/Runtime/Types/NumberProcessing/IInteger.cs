namespace Chips.Runtime.Types.NumberProcessing {
	public interface IInteger : INumber {
		IInteger And(IInteger number);

		IInteger ArithmeticShiftLeft();

		IInteger ArithmeticShiftRight();

		IInteger ArithmeticRotateLeft();

		IInteger ArithmeticRotateRight();

		IInteger Not();

		IInteger Or(IInteger number);

		IInteger Xor(IInteger number);
	}
}
