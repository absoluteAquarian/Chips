using System;

namespace Chips.Runtime.Meta {
	public partial struct Status {
		public void Update(SByte value) {
			Carry = false;
			Overflow = false;	
			Negative = value < 0;
			Zero = value == 0;
		}

		public void Update(Int16 value) {
			Carry = false;
			Overflow = false;	
			Negative = value < 0;
			Zero = value == 0;
		}

		public void Update(Int32 value) {
			Carry = false;
			Overflow = false;	
			Negative = value < 0;
			Zero = value == 0;
		}

		public void Update(Int64 value) {
			Carry = false;
			Overflow = false;	
			Negative = value < 0;
			Zero = value == 0;
		}

		public void Update(Byte value) {
			Carry = false;
			Overflow = false;	
			Negative = false;
			Zero = value == 0;
		}

		public void Update(UInt16 value) {
			Carry = false;
			Overflow = false;	
			Negative = false;
			Zero = value == 0;
		}

		public void Update(UInt32 value) {
			Carry = false;
			Overflow = false;	
			Negative = false;
			Zero = value == 0;
		}

		public void Update(UInt64 value) {
			Carry = false;
			Overflow = false;	
			Negative = false;
			Zero = value == 0;
		}

		public void Update(Single value) {
			Carry = false;
			Overflow = Single.IsInfinity(value) || Single.IsNaN(value);
			Negative = value < 0;
			Zero = value == 0 || Single.IsNaN(value);
		}

		public void Update(Double value) {
			Carry = false;
			Overflow = Double.IsInfinity(value) || Double.IsNaN(value);
			Negative = value < 0;
			Zero = value == 0 || Double.IsNaN(value);
		}

		public void Update(Decimal value) {
			Carry = false;
			Overflow = false;
			Negative = value < 0;
			Zero = value == 0;
		}

	}
}