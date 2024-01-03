using Chips.Runtime.Types;
using Chips.Runtime.Types.NumberProcessing;
using System;

namespace Chips.Runtime {
	public static class Registers {
		public static readonly IntegerRegister A = new() { ID = 0 };
		public static readonly FloatRegister I = new() { ID = 1 };
		public static readonly StringRegister S = new() { ID = 2 };
		public static readonly ExceptionRegister E = new() { ID = 3 };
		public static readonly FlagsRegister F = new() { ID = 4 };
		public static readonly IntegerRegister X = new() { ID = 5 };
		public static readonly IntegerRegister Y = new() { ID = 6 };

		static Registers() {
			A.Value = new Int32_T(0);
			I.Value = new Single_T(0);
			S.Value = null!;
			E.Value = null!;
			F.Value = 0;
			X.Value = new Int32_T(0);
			Y.Value = new Int32_T(0);
		}

		public static Register GetRegisterFromID(int id) {
			if (id == A.ID)
				return A;
			if (id == I.ID)
				return I;
			if (id == S.ID)
				return S;
			if (id == E.ID)
				return E;
			if (id == F.ID)
				return F;
			if (id == X.ID)
				return X;
			if (id == Y.ID)
				return Y;

			throw new ArgumentException("Unknown register ID: " + id, nameof(id));
		}

		public static string GetRegisterNameFromID(int id) {
			if (id == A.ID)
				return "A";
			if (id == I.ID)
				return "I";
			if (id == S.ID)
				return "S";
			if (id == E.ID)
				return "E";
			if (id == F.ID)
				return "F";
			if (id == X.ID)
				return "X";
			if (id == Y.ID)
				return "Y";

			throw new ArgumentException("Unknown register ID: " + id, nameof(id));
		}
	}
}
