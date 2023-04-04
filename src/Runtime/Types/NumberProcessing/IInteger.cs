namespace Chips.Runtime.Types.NumberProcessing {
	public interface IInteger {
		IInteger And(IInteger number);

		IInteger ArithmeticShiftLeft();

		IInteger ArithmeticShiftRight();

		IInteger ArithmeticRotateLeft();

		IInteger ArithmeticRotateRight();

		string BinaryRepresentation(bool leadingZeroes);

		IInteger GetBit(IInteger bit);

		IInteger Not();

		IInteger Or(IInteger number);

		IInteger Xor(IInteger number);
	}
}
