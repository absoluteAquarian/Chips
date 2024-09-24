using System;

namespace Chips.Runtime.Meta {
	partial class RegisterTable {
		private ref struct _RegisterTypePopulator {
			private int _currentRegister;

			public _RegisterType Current { readonly get; private set; }

			public bool MoveNext() {
				if (_currentRegister > (int)Register.ANS) {
					Current = default;
					return false;
				}

				if (_currentRegister <= (int)Register.PTR_F)
					Current = _RegisterType.Address;
				else if (_currentRegister <= (int)Register.Z_1F)
					Current = _RegisterType.Integer;
				else if (_currentRegister <= (int)Register.FL_1F)
					Current = _RegisterType.Float;
				else if (_currentRegister <= (int)Register.S_F)
					Current = _RegisterType.String;
				else if (_currentRegister <= (int)Register.OBJ_3F)
					Current = _RegisterType.Object;
				else if (_currentRegister <= (int)Register.V_F)
					Current = _RegisterType.Vector;
				else if (_currentRegister <= (int)Register.T_F)
					Current = _RegisterType.Token;
				else if (_currentRegister < (int)Register.R_0)
					Current = _RegisterType.InvalidRegister;
				else if (_currentRegister <= (int)Register.R_2)
					Current = _RegisterType.Variant;
				else if (_currentRegister == (int)Register.EX)
					Current = _RegisterType.Exception;
				else if (_currentRegister == (int)Register.PS)
					Current = _RegisterType.Status;
				else if (_currentRegister == (int)Register.ANS)
					Current = _RegisterType.Variant;

				_currentRegister++;
				return true;
			}

			public void Reset() {
				_currentRegister = default;
				Current = default;
			}

			public readonly _RegisterTypePopulator GetEnumerator() => this;
		}

		private enum _RegisterType {
			InvalidRegister,
			Address,
			Integer,
			Float,
			String,
			Object,
			Vector,
			Token,
			Variant,
			Exception,
			Status
		}

		private static readonly _RegisterType[] _registerToClassification = [ .. new _RegisterTypePopulator() ];

		private static _RegisterType RegisterToClassification(Register register) {
			_RegisterType type = _registerToClassification[(int)register];

			if (type == _RegisterType.InvalidRegister)
				throw new ArgumentOutOfRangeException(nameof(register), $"Index does not refer to a valid register: {(int)register}");

			return type;
		}
	}
}
