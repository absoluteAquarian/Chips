using Chips.Runtime.Types;

namespace Chips.Runtime {
	public static class Registers {
		public static readonly IntegerRegister A = new() { ID = 0 };
		public static readonly FloatRegister I = new() { ID = 1 };
		public static readonly StringRegister S = new() { ID = 2 };
		public static readonly ExceptionRegister E = new() { ID = 3 };
		public static readonly FlagsRegister F = new() { ID = 4 };
		public static readonly IntegerRegister X = new() { ID = 5 };
		public static readonly IntegerRegister Y = new() { ID = 6 };
	}
}
