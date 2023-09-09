using Chips.Runtime.Meta;
using Chips.Runtime.Utility;
using System;

#pragma warning disable CS0162
namespace Chips.Runtime.Types.NumberProcessing {
	[TextTemplateGenerated]
	public struct SByte_T : IInteger {
		private SByte value;

		public object Value => value;

		public SByte ActualValue => value;

		public bool IsZero => value == 0;

		public bool IsNegative => value < 0;

		public SByte_T(SByte value) {
			this.value = value;
		}

		public SByte_T(Int32 value) {
			this.value = (SByte)value;
		}

		public INumber Abs() {
			if (value == SByte.MinValue) {
				Registers.F.Overflow = true;
				return new SByte_T(SByte.MaxValue);
			}

			return new SByte_T(Math.Abs(value));
		}

		public INumber Add(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(SByte))
				return number.Add(this);

			SByte_T convert = ValueConverter.CastToSByte_T(number);

			if ((value < 0 && convert.value < 0 && value + convert.value > 0) || (value > 0 && convert.value > 0 && value + convert.value < 0)) {
				Registers.F.Overflow = true;
				return new SByte_T(value < 0 ? SByte.MinValue : SByte.MaxValue);
			}

			return new SByte_T(unchecked(value + convert.value));
		}

		public IInteger And(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.And(number);
			}

			if (number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.GetSizeFromNumericType(iNum.Value.GetType()) > sizeof(SByte))
				return number.And(this);

			SByte_T convert = ValueConverter.CastToSByte_T(iNum);
			return new SByte_T(value & convert.value);
		}

		public IInteger ArithmeticRotateLeft() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = unchecked(value & (SByte)((SByte)1 << (8 * sizeof(SByte) - 1))) != 0;
			
			var i = new SByte_T(value << 1);
			if (carry)
				i.value |= 1;

			return i;
		}

		public IInteger ArithmeticRotateRight() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = (value & 1) != 0;
			
			var i = new SByte_T(value >> 1);
			if (carry)
				i.value = (SByte)unchecked(i.value | ((SByte)1 << (8 * sizeof(SByte) - 1)));

			return i;
		}

		public IInteger ArithmeticShiftLeft()
			=> new SByte_T(value << 1);

		public IInteger ArithmeticShiftRight()
			=> new SByte_T(value >> 1);

		public string BinaryRepresentation(bool leadingZeroes) {
			string bin = Convert.ToString(value, 2);
			if (leadingZeroes)
				bin = bin.PadLeft(sizeof(SByte) * 8, '0');
			return bin;
		}

		public INumber Decrement()
			=> new SByte_T(unchecked(value - 1));

		public INumber Divide(INumber number, bool inverseLogic = false) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(SByte))
				return number.Divide(this, true);

			SByte_T convert = ValueConverter.CastToSByte_T(number);
			return new SByte_T(!inverseLogic ? value / convert.value : convert.value / value);
		}

		public INumber Increment()
			=> new SByte_T(unchecked(value + 1));

		public INumber Multiply(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(SByte))
				return number.Multiply(this);

			SByte_T convert = ValueConverter.CastToSByte_T(number);
			return new SByte_T(unchecked(value * convert.value));
		}

		public INumber Negate() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Negate();
			}

			return new SByte_T(-value);
		}

		public IInteger Not() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.Not();
			}

			return new SByte_T(~value);
		}

		public IInteger Or(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.Or(number);
			}

			if (number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.GetSizeFromNumericType(iNum.Value.GetType()) > sizeof(SByte))
				return number.Or(this);

			SByte_T convert = ValueConverter.CastToSByte_T(iNum);
			return new SByte_T(value | convert.value);
		}

		public INumber Remainder(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Remainder(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(SByte))
				return number.Remainder(this);

			SByte_T convert = ValueConverter.CastToSByte_T(number);
			return new SByte_T(value % convert.value);
		}

		public INumber Subtract(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(SByte))
				return number.Negate().Subtract(this.Negate());

			SByte_T convert = ValueConverter.CastToSByte_T(number);
			return new SByte_T(unchecked(value - convert.value));
		}

		public IInteger Xor(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.And(number);
			}

			if (number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.GetSizeFromNumericType(iNum.Value.GetType()) > sizeof(SByte))
				return number.Xor(this);

			SByte_T convert = ValueConverter.CastToSByte_T(iNum);
			return new SByte_T(value ^ convert.value);
		}
	}

	[TextTemplateGenerated]
	public struct Int16_T : IInteger {
		private Int16 value;

		public object Value => value;

		public Int16 ActualValue => value;

		public bool IsZero => value == 0;

		public bool IsNegative => value < 0;

		public Int16_T(Int16 value) {
			this.value = value;
		}

		public Int16_T(Int32 value) {
			this.value = (Int16)value;
		}

		public INumber Abs() {
			if (value == Int16.MinValue) {
				Registers.F.Overflow = true;
				return new Int16_T(Int16.MaxValue);
			}

			return new Int16_T(Math.Abs(value));
		}

		public INumber Add(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(Int16))
				return number.Add(this);

			Int16_T convert = ValueConverter.CastToInt16_T(number);

			if ((value < 0 && convert.value < 0 && value + convert.value > 0) || (value > 0 && convert.value > 0 && value + convert.value < 0)) {
				Registers.F.Overflow = true;
				return new Int16_T(value < 0 ? Int16.MinValue : Int16.MaxValue);
			}

			return new Int16_T(unchecked(value + convert.value));
		}

		public IInteger And(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.And(number);
			}

			if (number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.GetSizeFromNumericType(iNum.Value.GetType()) > sizeof(Int16))
				return number.And(this);

			Int16_T convert = ValueConverter.CastToInt16_T(iNum);
			return new Int16_T(value & convert.value);
		}

		public IInteger ArithmeticRotateLeft() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = unchecked(value & (Int16)((Int16)1 << (8 * sizeof(Int16) - 1))) != 0;
			
			var i = new Int16_T(value << 1);
			if (carry)
				i.value |= 1;

			return i;
		}

		public IInteger ArithmeticRotateRight() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = (value & 1) != 0;
			
			var i = new Int16_T(value >> 1);
			if (carry)
				i.value = (Int16)unchecked(i.value | ((Int16)1 << (8 * sizeof(Int16) - 1)));

			return i;
		}

		public IInteger ArithmeticShiftLeft()
			=> new Int16_T(value << 1);

		public IInteger ArithmeticShiftRight()
			=> new Int16_T(value >> 1);

		public string BinaryRepresentation(bool leadingZeroes) {
			string bin = Convert.ToString(value, 2);
			if (leadingZeroes)
				bin = bin.PadLeft(sizeof(Int16) * 8, '0');
			return bin;
		}

		public INumber Decrement()
			=> new Int16_T(unchecked(value - 1));

		public INumber Divide(INumber number, bool inverseLogic = false) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(Int16))
				return number.Divide(this, true);

			Int16_T convert = ValueConverter.CastToInt16_T(number);
			return new Int16_T(!inverseLogic ? value / convert.value : convert.value / value);
		}

		public INumber Increment()
			=> new Int16_T(unchecked(value + 1));

		public INumber Multiply(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(Int16))
				return number.Multiply(this);

			Int16_T convert = ValueConverter.CastToInt16_T(number);
			return new Int16_T(unchecked(value * convert.value));
		}

		public INumber Negate() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Negate();
			}

			return new Int16_T(-value);
		}

		public IInteger Not() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.Not();
			}

			return new Int16_T(~value);
		}

		public IInteger Or(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.Or(number);
			}

			if (number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.GetSizeFromNumericType(iNum.Value.GetType()) > sizeof(Int16))
				return number.Or(this);

			Int16_T convert = ValueConverter.CastToInt16_T(iNum);
			return new Int16_T(value | convert.value);
		}

		public INumber Remainder(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Remainder(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(Int16))
				return number.Remainder(this);

			Int16_T convert = ValueConverter.CastToInt16_T(number);
			return new Int16_T(value % convert.value);
		}

		public INumber Subtract(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(Int16))
				return number.Negate().Subtract(this.Negate());

			Int16_T convert = ValueConverter.CastToInt16_T(number);
			return new Int16_T(unchecked(value - convert.value));
		}

		public IInteger Xor(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.And(number);
			}

			if (number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.GetSizeFromNumericType(iNum.Value.GetType()) > sizeof(Int16))
				return number.Xor(this);

			Int16_T convert = ValueConverter.CastToInt16_T(iNum);
			return new Int16_T(value ^ convert.value);
		}
	}

	[TextTemplateGenerated]
	public struct Int32_T : IInteger {
		private Int32 value;

		public object Value => value;

		public Int32 ActualValue => value;

		public bool IsZero => value == 0;

		public bool IsNegative => value < 0;

		public Int32_T(Int32 value) {
			this.value = value;
		}


		public INumber Abs() {
			if (value == Int32.MinValue) {
				Registers.F.Overflow = true;
				return new Int32_T(Int32.MaxValue);
			}

			return new Int32_T(Math.Abs(value));
		}

		public INumber Add(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(Int32))
				return number.Add(this);

			Int32_T convert = ValueConverter.CastToInt32_T(number);

			if ((value < 0 && convert.value < 0 && value + convert.value > 0) || (value > 0 && convert.value > 0 && value + convert.value < 0)) {
				Registers.F.Overflow = true;
				return new Int32_T(value < 0 ? Int32.MinValue : Int32.MaxValue);
			}

			return new Int32_T(unchecked(value + convert.value));
		}

		public IInteger And(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.And(number);
			}

			if (number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.GetSizeFromNumericType(iNum.Value.GetType()) > sizeof(Int32))
				return number.And(this);

			Int32_T convert = ValueConverter.CastToInt32_T(iNum);
			return new Int32_T(value & convert.value);
		}

		public IInteger ArithmeticRotateLeft() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = unchecked(value & (Int32)((Int32)1 << (8 * sizeof(Int32) - 1))) != 0;
			
			var i = new Int32_T(value << 1);
			if (carry)
				i.value |= 1;

			return i;
		}

		public IInteger ArithmeticRotateRight() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = (value & 1) != 0;
			
			var i = new Int32_T(value >> 1);
			if (carry)
				i.value = (Int32)unchecked(i.value | ((Int32)1 << (8 * sizeof(Int32) - 1)));

			return i;
		}

		public IInteger ArithmeticShiftLeft()
			=> new Int32_T(value << 1);

		public IInteger ArithmeticShiftRight()
			=> new Int32_T(value >> 1);

		public string BinaryRepresentation(bool leadingZeroes) {
			string bin = Convert.ToString(value, 2);
			if (leadingZeroes)
				bin = bin.PadLeft(sizeof(Int32) * 8, '0');
			return bin;
		}

		public INumber Decrement()
			=> new Int32_T(unchecked(value - 1));

		public INumber Divide(INumber number, bool inverseLogic = false) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(Int32))
				return number.Divide(this, true);

			Int32_T convert = ValueConverter.CastToInt32_T(number);
			return new Int32_T(!inverseLogic ? value / convert.value : convert.value / value);
		}

		public INumber Increment()
			=> new Int32_T(unchecked(value + 1));

		public INumber Multiply(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(Int32))
				return number.Multiply(this);

			Int32_T convert = ValueConverter.CastToInt32_T(number);
			return new Int32_T(unchecked(value * convert.value));
		}

		public INumber Negate() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Negate();
			}

			return new Int32_T(-value);
		}

		public IInteger Not() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.Not();
			}

			return new Int32_T(~value);
		}

		public IInteger Or(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.Or(number);
			}

			if (number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.GetSizeFromNumericType(iNum.Value.GetType()) > sizeof(Int32))
				return number.Or(this);

			Int32_T convert = ValueConverter.CastToInt32_T(iNum);
			return new Int32_T(value | convert.value);
		}

		public INumber Remainder(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Remainder(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(Int32))
				return number.Remainder(this);

			Int32_T convert = ValueConverter.CastToInt32_T(number);
			return new Int32_T(value % convert.value);
		}

		public INumber Subtract(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(Int32))
				return number.Negate().Subtract(this.Negate());

			Int32_T convert = ValueConverter.CastToInt32_T(number);
			return new Int32_T(unchecked(value - convert.value));
		}

		public IInteger Xor(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.And(number);
			}

			if (number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.GetSizeFromNumericType(iNum.Value.GetType()) > sizeof(Int32))
				return number.Xor(this);

			Int32_T convert = ValueConverter.CastToInt32_T(iNum);
			return new Int32_T(value ^ convert.value);
		}
	}

	[TextTemplateGenerated]
	public struct Int64_T : IInteger {
		private Int64 value;

		public object Value => value;

		public Int64 ActualValue => value;

		public bool IsZero => value == 0;

		public bool IsNegative => value < 0;

		public Int64_T(Int64 value) {
			this.value = value;
		}

		public Int64_T(Int32 value) {
			this.value = (Int64)value;
		}

		public INumber Abs() {
			if (value == Int64.MinValue) {
				Registers.F.Overflow = true;
				return new Int64_T(Int64.MaxValue);
			}

			return new Int64_T(Math.Abs(value));
		}

		public INumber Add(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(Int64))
				return number.Add(this);

			Int64_T convert = ValueConverter.CastToInt64_T(number);

			if ((value < 0 && convert.value < 0 && value + convert.value > 0) || (value > 0 && convert.value > 0 && value + convert.value < 0)) {
				Registers.F.Overflow = true;
				return new Int64_T(value < 0 ? Int64.MinValue : Int64.MaxValue);
			}

			return new Int64_T(unchecked(value + convert.value));
		}

		public IInteger And(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.And(number);
			}

			if (number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.GetSizeFromNumericType(iNum.Value.GetType()) > sizeof(Int64))
				return number.And(this);

			Int64_T convert = ValueConverter.CastToInt64_T(iNum);
			return new Int64_T(value & convert.value);
		}

		public IInteger ArithmeticRotateLeft() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = unchecked(value & (Int64)((Int64)1 << (8 * sizeof(Int64) - 1))) != 0;
			
			var i = new Int64_T(value << 1);
			if (carry)
				i.value |= 1;

			return i;
		}

		public IInteger ArithmeticRotateRight() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = (value & 1) != 0;
			
			var i = new Int64_T(value >> 1);
			if (carry)
				i.value = (Int64)unchecked(i.value | ((Int64)1 << (8 * sizeof(Int64) - 1)));

			return i;
		}

		public IInteger ArithmeticShiftLeft()
			=> new Int64_T(value << 1);

		public IInteger ArithmeticShiftRight()
			=> new Int64_T(value >> 1);

		public string BinaryRepresentation(bool leadingZeroes) {
			string bin = Convert.ToString(value, 2);
			if (leadingZeroes)
				bin = bin.PadLeft(sizeof(Int64) * 8, '0');
			return bin;
		}

		public INumber Decrement()
			=> new Int64_T(unchecked(value - 1));

		public INumber Divide(INumber number, bool inverseLogic = false) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(Int64))
				return number.Divide(this, true);

			Int64_T convert = ValueConverter.CastToInt64_T(number);
			return new Int64_T(!inverseLogic ? value / convert.value : convert.value / value);
		}

		public INumber Increment()
			=> new Int64_T(unchecked(value + 1));

		public INumber Multiply(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(Int64))
				return number.Multiply(this);

			Int64_T convert = ValueConverter.CastToInt64_T(number);
			return new Int64_T(unchecked(value * convert.value));
		}

		public INumber Negate() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Negate();
			}

			return new Int64_T(-value);
		}

		public IInteger Not() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.Not();
			}

			return new Int64_T(~value);
		}

		public IInteger Or(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.Or(number);
			}

			if (number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.GetSizeFromNumericType(iNum.Value.GetType()) > sizeof(Int64))
				return number.Or(this);

			Int64_T convert = ValueConverter.CastToInt64_T(iNum);
			return new Int64_T(value | convert.value);
		}

		public INumber Remainder(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Remainder(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(Int64))
				return number.Remainder(this);

			Int64_T convert = ValueConverter.CastToInt64_T(number);
			return new Int64_T(value % convert.value);
		}

		public INumber Subtract(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(Int64))
				return number.Negate().Subtract(this.Negate());

			Int64_T convert = ValueConverter.CastToInt64_T(number);
			return new Int64_T(unchecked(value - convert.value));
		}

		public IInteger Xor(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.And(number);
			}

			if (number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.GetSizeFromNumericType(iNum.Value.GetType()) > sizeof(Int64))
				return number.Xor(this);

			Int64_T convert = ValueConverter.CastToInt64_T(iNum);
			return new Int64_T(value ^ convert.value);
		}
	}

	[TextTemplateGenerated]
	public struct Byte_T : IInteger {
		private Byte value;

		public object Value => value;

		public Byte ActualValue => value;

		public bool IsZero => value == 0;

		public bool IsNegative => value < 0;

		public Byte_T(Byte value) {
			this.value = value;
		}

		public Byte_T(Int32 value) {
			this.value = (Byte)value;
		}

		public INumber Abs() {
			if (value == Byte.MinValue) {
				Registers.F.Overflow = true;
				return new Byte_T(Byte.MaxValue);
			}

			return new Byte_T(Math.Abs(value));
		}

		public INumber Add(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(Byte))
				return number.Add(this);

			Byte_T convert = ValueConverter.CastToByte_T(number);

			if ((value < 0 && convert.value < 0 && value + convert.value > 0) || (value > 0 && convert.value > 0 && value + convert.value < 0)) {
				Registers.F.Overflow = true;
				return new Byte_T(value < 0 ? Byte.MinValue : Byte.MaxValue);
			}

			return new Byte_T(unchecked(value + convert.value));
		}

		public IInteger And(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.And(number);
			}

			if (number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.GetSizeFromNumericType(iNum.Value.GetType()) > sizeof(Byte))
				return number.And(this);

			Byte_T convert = ValueConverter.CastToByte_T(iNum);
			return new Byte_T(value & convert.value);
		}

		public IInteger ArithmeticRotateLeft() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = unchecked(value & (Byte)((Byte)1 << (8 * sizeof(Byte) - 1))) != 0;
			
			var i = new Byte_T(value << 1);
			if (carry)
				i.value |= 1;

			return i;
		}

		public IInteger ArithmeticRotateRight() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = (value & 1) != 0;
			
			var i = new Byte_T(value >> 1);
			if (carry)
				i.value = (Byte)unchecked(i.value | ((Byte)1 << (8 * sizeof(Byte) - 1)));

			return i;
		}

		public IInteger ArithmeticShiftLeft()
			=> new Byte_T(value << 1);

		public IInteger ArithmeticShiftRight()
			=> new Byte_T(value >> 1);

		public string BinaryRepresentation(bool leadingZeroes) {
			string bin = Convert.ToString(value, 2);
			if (leadingZeroes)
				bin = bin.PadLeft(sizeof(Byte) * 8, '0');
			return bin;
		}

		public INumber Decrement()
			=> new Byte_T(unchecked(value - 1));

		public INumber Divide(INumber number, bool inverseLogic = false) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(Byte))
				return number.Divide(this, true);

			Byte_T convert = ValueConverter.CastToByte_T(number);
			return new Byte_T(!inverseLogic ? value / convert.value : convert.value / value);
		}

		public INumber Increment()
			=> new Byte_T(unchecked(value + 1));

		public INumber Multiply(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(Byte))
				return number.Multiply(this);

			Byte_T convert = ValueConverter.CastToByte_T(number);
			return new Byte_T(unchecked(value * convert.value));
		}

		public INumber Negate() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Negate();
			}

			return new Byte_T(-value);
		}

		public IInteger Not() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.Not();
			}

			return new Byte_T(~value);
		}

		public IInteger Or(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.Or(number);
			}

			if (number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.GetSizeFromNumericType(iNum.Value.GetType()) > sizeof(Byte))
				return number.Or(this);

			Byte_T convert = ValueConverter.CastToByte_T(iNum);
			return new Byte_T(value | convert.value);
		}

		public INumber Remainder(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Remainder(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(Byte))
				return number.Remainder(this);

			Byte_T convert = ValueConverter.CastToByte_T(number);
			return new Byte_T(value % convert.value);
		}

		public INumber Subtract(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(Byte))
				return number.Negate().Subtract(this.Negate());

			Byte_T convert = ValueConverter.CastToByte_T(number);
			return new Byte_T(unchecked(value - convert.value));
		}

		public IInteger Xor(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.And(number);
			}

			if (number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.GetSizeFromNumericType(iNum.Value.GetType()) > sizeof(Byte))
				return number.Xor(this);

			Byte_T convert = ValueConverter.CastToByte_T(iNum);
			return new Byte_T(value ^ convert.value);
		}
	}

	[TextTemplateGenerated]
	public struct UInt16_T : IInteger {
		private UInt16 value;

		public object Value => value;

		public UInt16 ActualValue => value;

		public bool IsZero => value == 0;

		public bool IsNegative => false;

		public UInt16_T(UInt16 value) {
			this.value = value;
		}

		public UInt16_T(Int32 value) {
			this.value = (UInt16)value;
		}

		public INumber Abs()
			=> new UInt16_T(value);

		public INumber Add(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(UInt16))
				return number.Add(this);

			UInt16_T convert = ValueConverter.CastToUInt16_T(number);

			if (value + convert.value < value) {
				Registers.F.Overflow = true;
				return new UInt16_T(UInt16.MaxValue);
			}

			return new UInt16_T(unchecked(value + convert.value));
		}

		public IInteger And(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.And(number);
			}

			if (number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.GetSizeFromNumericType(iNum.Value.GetType()) > sizeof(UInt16))
				return number.And(this);

			UInt16_T convert = ValueConverter.CastToUInt16_T(iNum);
			return new UInt16_T(value & convert.value);
		}

		public IInteger ArithmeticRotateLeft() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = unchecked(value & (UInt16)((UInt16)1 << (8 * sizeof(UInt16) - 1))) != 0;
			
			var i = new UInt16_T(value << 1);
			if (carry)
				i.value |= 1;

			return i;
		}

		public IInteger ArithmeticRotateRight() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = (value & 1) != 0;
			
			var i = new UInt16_T(value >> 1);
			if (carry)
				i.value = (UInt16)unchecked(i.value | ((UInt16)1 << (8 * sizeof(UInt16) - 1)));

			return i;
		}

		public IInteger ArithmeticShiftLeft()
			=> new UInt16_T(value << 1);

		public IInteger ArithmeticShiftRight()
			=> new UInt16_T(value >> 1);

		public string BinaryRepresentation(bool leadingZeroes) {
			string bin = Convert.ToString(value, 2);
			if (leadingZeroes)
				bin = bin.PadLeft(sizeof(UInt16) * 8, '0');
			return bin;
		}

		public INumber Decrement()
			=> new UInt16_T(unchecked(value - 1));

		public INumber Divide(INumber number, bool inverseLogic = false) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(UInt16))
				return number.Divide(this, true);

			UInt16_T convert = ValueConverter.CastToUInt16_T(number);
			return new UInt16_T(!inverseLogic ? value / convert.value : convert.value / value);
		}

		public INumber Increment()
			=> new UInt16_T(unchecked(value + 1));

		public INumber Multiply(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(UInt16))
				return number.Multiply(this);

			UInt16_T convert = ValueConverter.CastToUInt16_T(number);
			return new UInt16_T(unchecked(value * convert.value));
		}

		public INumber Negate() {
			throw new InvalidOperationException("Negation cannot be performed on unsigned integers");
		}

		public IInteger Not() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.Not();
			}

			return new UInt16_T(~value);
		}

		public IInteger Or(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.Or(number);
			}

			if (number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.GetSizeFromNumericType(iNum.Value.GetType()) > sizeof(UInt16))
				return number.Or(this);

			UInt16_T convert = ValueConverter.CastToUInt16_T(iNum);
			return new UInt16_T(value | convert.value);
		}

		public INumber Remainder(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Remainder(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(UInt16))
				return number.Remainder(this);

			UInt16_T convert = ValueConverter.CastToUInt16_T(number);
			return new UInt16_T(value % convert.value);
		}

		public INumber Subtract(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(UInt16))
				return number.Negate().Subtract(this.Negate());

			UInt16_T convert = ValueConverter.CastToUInt16_T(number);
			return new UInt16_T(unchecked(value - convert.value));
		}

		public IInteger Xor(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.And(number);
			}

			if (number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.GetSizeFromNumericType(iNum.Value.GetType()) > sizeof(UInt16))
				return number.Xor(this);

			UInt16_T convert = ValueConverter.CastToUInt16_T(iNum);
			return new UInt16_T(value ^ convert.value);
		}
	}

	[TextTemplateGenerated]
	public struct UInt32_T : IInteger {
		private UInt32 value;

		public object Value => value;

		public UInt32 ActualValue => value;

		public bool IsZero => value == 0;

		public bool IsNegative => false;

		public UInt32_T(UInt32 value) {
			this.value = value;
		}

		public UInt32_T(Int32 value) {
			this.value = (UInt32)value;
		}

		public INumber Abs()
			=> new UInt32_T(value);

		public INumber Add(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(UInt32))
				return number.Add(this);

			UInt32_T convert = ValueConverter.CastToUInt32_T(number);

			if (value + convert.value < value) {
				Registers.F.Overflow = true;
				return new UInt32_T(UInt32.MaxValue);
			}

			return new UInt32_T(unchecked(value + convert.value));
		}

		public IInteger And(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.And(number);
			}

			if (number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.GetSizeFromNumericType(iNum.Value.GetType()) > sizeof(UInt32))
				return number.And(this);

			UInt32_T convert = ValueConverter.CastToUInt32_T(iNum);
			return new UInt32_T(value & convert.value);
		}

		public IInteger ArithmeticRotateLeft() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = unchecked(value & (UInt32)((UInt32)1 << (8 * sizeof(UInt32) - 1))) != 0;
			
			var i = new UInt32_T(value << 1);
			if (carry)
				i.value |= 1;

			return i;
		}

		public IInteger ArithmeticRotateRight() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = (value & 1) != 0;
			
			var i = new UInt32_T(value >> 1);
			if (carry)
				i.value = (UInt32)unchecked(i.value | ((UInt32)1 << (8 * sizeof(UInt32) - 1)));

			return i;
		}

		public IInteger ArithmeticShiftLeft()
			=> new UInt32_T(value << 1);

		public IInteger ArithmeticShiftRight()
			=> new UInt32_T(value >> 1);

		public string BinaryRepresentation(bool leadingZeroes) {
			string bin = Convert.ToString(value, 2);
			if (leadingZeroes)
				bin = bin.PadLeft(sizeof(UInt32) * 8, '0');
			return bin;
		}

		public INumber Decrement()
			=> new UInt32_T(unchecked(value - 1));

		public INumber Divide(INumber number, bool inverseLogic = false) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(UInt32))
				return number.Divide(this, true);

			UInt32_T convert = ValueConverter.CastToUInt32_T(number);
			return new UInt32_T(!inverseLogic ? value / convert.value : convert.value / value);
		}

		public INumber Increment()
			=> new UInt32_T(unchecked(value + 1));

		public INumber Multiply(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(UInt32))
				return number.Multiply(this);

			UInt32_T convert = ValueConverter.CastToUInt32_T(number);
			return new UInt32_T(unchecked(value * convert.value));
		}

		public INumber Negate() {
			throw new InvalidOperationException("Negation cannot be performed on unsigned integers");
		}

		public IInteger Not() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.Not();
			}

			return new UInt32_T(~value);
		}

		public IInteger Or(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.Or(number);
			}

			if (number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.GetSizeFromNumericType(iNum.Value.GetType()) > sizeof(UInt32))
				return number.Or(this);

			UInt32_T convert = ValueConverter.CastToUInt32_T(iNum);
			return new UInt32_T(value | convert.value);
		}

		public INumber Remainder(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Remainder(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(UInt32))
				return number.Remainder(this);

			UInt32_T convert = ValueConverter.CastToUInt32_T(number);
			return new UInt32_T(value % convert.value);
		}

		public INumber Subtract(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(UInt32))
				return number.Negate().Subtract(this.Negate());

			UInt32_T convert = ValueConverter.CastToUInt32_T(number);
			return new UInt32_T(unchecked(value - convert.value));
		}

		public IInteger Xor(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.And(number);
			}

			if (number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.GetSizeFromNumericType(iNum.Value.GetType()) > sizeof(UInt32))
				return number.Xor(this);

			UInt32_T convert = ValueConverter.CastToUInt32_T(iNum);
			return new UInt32_T(value ^ convert.value);
		}
	}

	[TextTemplateGenerated]
	public struct UInt64_T : IInteger {
		private UInt64 value;

		public object Value => value;

		public UInt64 ActualValue => value;

		public bool IsZero => value == 0;

		public bool IsNegative => false;

		public UInt64_T(UInt64 value) {
			this.value = value;
		}

		public UInt64_T(Int32 value) {
			this.value = (UInt64)value;
		}

		public INumber Abs()
			=> new UInt64_T(value);

		public INumber Add(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(UInt64))
				return number.Add(this);

			UInt64_T convert = ValueConverter.CastToUInt64_T(number);

			if (value + convert.value < value) {
				Registers.F.Overflow = true;
				return new UInt64_T(UInt64.MaxValue);
			}

			return new UInt64_T(unchecked(value + convert.value));
		}

		public IInteger And(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.And(number);
			}

			if (number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.GetSizeFromNumericType(iNum.Value.GetType()) > sizeof(UInt64))
				return number.And(this);

			UInt64_T convert = ValueConverter.CastToUInt64_T(iNum);
			return new UInt64_T(value & convert.value);
		}

		public IInteger ArithmeticRotateLeft() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = unchecked(value & (UInt64)((UInt64)1 << (8 * sizeof(UInt64) - 1))) != 0;
			
			var i = new UInt64_T(value << 1);
			if (carry)
				i.value |= 1;

			return i;
		}

		public IInteger ArithmeticRotateRight() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = (value & 1) != 0;
			
			var i = new UInt64_T(value >> 1);
			if (carry)
				i.value = (UInt64)unchecked(i.value | ((UInt64)1 << (8 * sizeof(UInt64) - 1)));

			return i;
		}

		public IInteger ArithmeticShiftLeft()
			=> new UInt64_T(value << 1);

		public IInteger ArithmeticShiftRight()
			=> new UInt64_T(value >> 1);

		public string BinaryRepresentation(bool leadingZeroes) {
			string bin = Convert.ToString((long)value, 2);
			if (leadingZeroes)
				bin = bin.PadLeft(sizeof(UInt64) * 8, '0');
			return bin;
		}

		public INumber Decrement()
			=> new UInt64_T(unchecked(value - 1));

		public INumber Divide(INumber number, bool inverseLogic = false) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(UInt64))
				return number.Divide(this, true);

			UInt64_T convert = ValueConverter.CastToUInt64_T(number);
			return new UInt64_T(!inverseLogic ? value / convert.value : convert.value / value);
		}

		public INumber Increment()
			=> new UInt64_T(unchecked(value + 1));

		public INumber Multiply(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(UInt64))
				return number.Multiply(this);

			UInt64_T convert = ValueConverter.CastToUInt64_T(number);
			return new UInt64_T(unchecked(value * convert.value));
		}

		public INumber Negate() {
			throw new InvalidOperationException("Negation cannot be performed on unsigned integers");
		}

		public IInteger Not() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.Not();
			}

			return new UInt64_T(~value);
		}

		public IInteger Or(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.Or(number);
			}

			if (number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.GetSizeFromNumericType(iNum.Value.GetType()) > sizeof(UInt64))
				return number.Or(this);

			UInt64_T convert = ValueConverter.CastToUInt64_T(iNum);
			return new UInt64_T(value | convert.value);
		}

		public INumber Remainder(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Remainder(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(UInt64))
				return number.Remainder(this);

			UInt64_T convert = ValueConverter.CastToUInt64_T(number);
			return new UInt64_T(value % convert.value);
		}

		public INumber Subtract(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.GetSizeFromNumericType(number.Value.GetType()) > sizeof(UInt64))
				return number.Negate().Subtract(this.Negate());

			UInt64_T convert = ValueConverter.CastToUInt64_T(number);
			return new UInt64_T(unchecked(value - convert.value));
		}

		public IInteger Xor(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return (upcast as IInteger)!.And(number);
			}

			if (number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.GetSizeFromNumericType(iNum.Value.GetType()) > sizeof(UInt64))
				return number.Xor(this);

			UInt64_T convert = ValueConverter.CastToUInt64_T(iNum);
			return new UInt64_T(value ^ convert.value);
		}
	}

	[TextTemplateGenerated]
	public struct Single_T : IFloat {
		private Single value;

		public object Value => value;

		public Single ActualValue => value;

		public bool IsZero => value == 0;

		public bool IsNegative => value < 0;

		public bool IsNaN => Single.IsNaN(value);

		public bool IsInfinity => Single.IsInfinity(value);

		public Single_T(Single value) {
			this.value = value;
		}

		public INumber Abs()
			=> new Single_T((Single)Math.Abs(value));

		public IFloat Acos()
			=> new Single_T((Single)Math.Acos(value));

		public IFloat Acosh()
			=> new Single_T((Single)Math.Acosh(value));

		public INumber Add(INumber number) {
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if (number is IInteger || targetSize < sizeof(Single))
				number = ValueConverter.CastToSingle_T(number);
			else if (targetSize > sizeof(Single))
				return number.Add(this);

			Single_T convert = ValueConverter.CastToSingle_T(number);
			return new Single_T(value + convert.value);
		}

		public IFloat Asin()
			=> new Single_T((Single)Math.Asin(value));

		public IFloat Asinh()
			=> new Single_T((Single)Math.Asinh(value));

		public IFloat Atan()
			=> new Single_T((Single)Math.Atan(value));

		public IFloat Atan2(IFloat divisor, bool inverseLogic = false) {
			if (divisor is not INumber number)
				throw new InvalidOperationException("Number was not a floating-point value");

			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if (targetSize < sizeof(Single))
				number = ValueConverter.CastToSingle_T(number);
			else if (targetSize > sizeof(Single))
				return divisor.Atan2(this, true);

			Single_T convert = ValueConverter.CastToSingle_T(number);
			return new Single_T((Single)Math.Atan2(!inverseLogic ? value : convert.value, !inverseLogic ? convert.value : value));
		}

		public IFloat Atanh()
			=> new Single_T((Single)Math.Asinh(value));

		public INumber Ceiling()
			=> new Single_T((Single)Math.Ceiling(value));

		public IFloat Cos()
			=> new Single_T((Single)Math.Cos(value));

		public IFloat Cosh()
			=> new Single_T((Single)Math.Cosh(value));
		
		public INumber Decrement()
			=> new Single_T(value + 1);

		public INumber Divide(INumber number, bool inverseLogic = false) {
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if (number is IInteger || targetSize < sizeof(Single))
				number = ValueConverter.CastToSingle_T(number);
			else if (targetSize > sizeof(Single))
				return number.Divide(this, true);

			Single_T convert = ValueConverter.CastToSingle_T(number);
			return new Single_T(!inverseLogic ? value / convert.value : convert.value / value);
		}

		public IFloat Exp()
			=> new Single_T((Single)Math.Exp(value));

		public INumber Floor()
			=> new Single_T((Single)Math.Floor(value));

		public IInteger GetBits()
			=> (ValueConverter.RetrieveFloatingPointBits(this) as IInteger)!;

		public INumber Increment()
			=> new Single_T(value + 1);

		public IFloat Inverse()
			=> (new Single_T(1f).Divide(this) as IFloat)!;

		public IFloat Ln()
			=> new Single_T((Single)Math.Log(value));

		public IFloat Log10()
			=> new Single_T((Single)Math.Log10(value));

		public IFloat Log2()
			=> new Single_T((Single)Math.Log2(value));

		public INumber Multiply(INumber number) {
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if (number is IInteger || targetSize < sizeof(Single))
				number = ValueConverter.CastToSingle_T(number);
			else if (targetSize > sizeof(Single))
				return number.Multiply(this);

			Single_T convert = ValueConverter.CastToSingle_T(number);
			return new Single_T(value * convert.value);
		}

		public INumber Negate()
			=> new Single_T(-value);

		public IFloat Pow(IFloat exponent) {
			if (exponent is not INumber number)
				throw new InvalidOperationException("Number was not a floating-point value");
			
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if (number is IInteger || targetSize < sizeof(Single))
				number = ValueConverter.CastToSingle_T(number);
			else if (targetSize > sizeof(Single))
				return ValueConverter.Constants.GetConst_E(number.Value.GetType()).Pow((number.Multiply((Ln() as INumber)!) as IFloat)!); //x^y = e^(y * ln(x))

			Single_T convert = ValueConverter.CastToSingle_T(number);
			return new Single_T(value * convert.value);
		}

		public INumber Remainder(INumber number) {
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if (number is IInteger || targetSize < sizeof(Single))
				number = ValueConverter.CastToSingle_T(number);
			else if (targetSize > sizeof(Single))
				return number.Remainder(this);

			Single_T convert = ValueConverter.CastToSingle_T(number);
			return new Single_T(value % convert.value);
		}

		public IFloat Root(IFloat root)
			=> Pow(root.Inverse());

		public IFloat Sin()
			=> new Single_T((Single)Math.Sin(value));

		public IFloat Sinh()
			=> new Single_T((Single)Math.Sinh(value));

		public IFloat Sqrt()
			=> new Single_T((Single)Math.Sqrt(value));

		public INumber Subtract(INumber number) {
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if (number is IInteger || targetSize < sizeof(Single))
				number = ValueConverter.CastToSingle_T(number);
			else if (targetSize > sizeof(Single))
				return number.Negate().Subtract(this.Negate());

			Single_T convert = ValueConverter.CastToSingle_T(number);
			return new Single_T(value - convert.value);
		}

		public IFloat Tan()
			=> new Single_T((Single)Math.Tan(value));

		public IFloat Tanh()
			=> new Single_T((Single)Math.Tanh(value));
	}
	
	[TextTemplateGenerated]
	public struct Double_T : IFloat {
		private Double value;

		public object Value => value;

		public Double ActualValue => value;

		public bool IsZero => value == 0;

		public bool IsNegative => value < 0;

		public bool IsNaN => Double.IsNaN(value);

		public bool IsInfinity => Double.IsInfinity(value);

		public Double_T(Double value) {
			this.value = value;
		}

		public Double_T(Single value) {
			this.value = (Double)value;
		}

		public INumber Abs()
			=> new Double_T((Double)Math.Abs(value));

		public IFloat Acos()
			=> new Double_T((Double)Math.Acos(value));

		public IFloat Acosh()
			=> new Double_T((Double)Math.Acosh(value));

		public INumber Add(INumber number) {
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if (number is IInteger || targetSize < sizeof(Double))
				number = ValueConverter.CastToDouble_T(number);
			else if (targetSize > sizeof(Double))
				return number.Add(this);

			Double_T convert = ValueConverter.CastToDouble_T(number);
			return new Double_T(value + convert.value);
		}

		public IFloat Asin()
			=> new Double_T((Double)Math.Asin(value));

		public IFloat Asinh()
			=> new Double_T((Double)Math.Asinh(value));

		public IFloat Atan()
			=> new Double_T((Double)Math.Atan(value));

		public IFloat Atan2(IFloat divisor, bool inverseLogic = false) {
			if (divisor is not INumber number)
				throw new InvalidOperationException("Number was not a floating-point value");

			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if (targetSize < sizeof(Double))
				number = ValueConverter.CastToDouble_T(number);
			else if (targetSize > sizeof(Double))
				return divisor.Atan2(this, true);

			Double_T convert = ValueConverter.CastToDouble_T(number);
			return new Double_T((Double)Math.Atan2(!inverseLogic ? value : convert.value, !inverseLogic ? convert.value : value));
		}

		public IFloat Atanh()
			=> new Double_T((Double)Math.Asinh(value));

		public INumber Ceiling()
			=> new Double_T((Double)Math.Ceiling(value));

		public IFloat Cos()
			=> new Double_T((Double)Math.Cos(value));

		public IFloat Cosh()
			=> new Double_T((Double)Math.Cosh(value));
		
		public INumber Decrement()
			=> new Double_T(value + 1);

		public INumber Divide(INumber number, bool inverseLogic = false) {
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if (number is IInteger || targetSize < sizeof(Double))
				number = ValueConverter.CastToDouble_T(number);
			else if (targetSize > sizeof(Double))
				return number.Divide(this, true);

			Double_T convert = ValueConverter.CastToDouble_T(number);
			return new Double_T(!inverseLogic ? value / convert.value : convert.value / value);
		}

		public IFloat Exp()
			=> new Double_T((Double)Math.Exp(value));

		public INumber Floor()
			=> new Double_T((Double)Math.Floor(value));

		public IInteger GetBits()
			=> (ValueConverter.RetrieveFloatingPointBits(this) as IInteger)!;

		public INumber Increment()
			=> new Double_T(value + 1);

		public IFloat Inverse()
			=> (new Double_T(1f).Divide(this) as IFloat)!;

		public IFloat Ln()
			=> new Double_T((Double)Math.Log(value));

		public IFloat Log10()
			=> new Double_T((Double)Math.Log10(value));

		public IFloat Log2()
			=> new Double_T((Double)Math.Log2(value));

		public INumber Multiply(INumber number) {
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if (number is IInteger || targetSize < sizeof(Double))
				number = ValueConverter.CastToDouble_T(number);
			else if (targetSize > sizeof(Double))
				return number.Multiply(this);

			Double_T convert = ValueConverter.CastToDouble_T(number);
			return new Double_T(value * convert.value);
		}

		public INumber Negate()
			=> new Double_T(-value);

		public IFloat Pow(IFloat exponent) {
			if (exponent is not INumber number)
				throw new InvalidOperationException("Number was not a floating-point value");
			
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if (number is IInteger || targetSize < sizeof(Double))
				number = ValueConverter.CastToDouble_T(number);
			else if (targetSize > sizeof(Double))
				return ValueConverter.Constants.GetConst_E(number.Value.GetType()).Pow((number.Multiply((Ln() as INumber)!) as IFloat)!); //x^y = e^(y * ln(x))

			Double_T convert = ValueConverter.CastToDouble_T(number);
			return new Double_T(value * convert.value);
		}

		public INumber Remainder(INumber number) {
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if (number is IInteger || targetSize < sizeof(Double))
				number = ValueConverter.CastToDouble_T(number);
			else if (targetSize > sizeof(Double))
				return number.Remainder(this);

			Double_T convert = ValueConverter.CastToDouble_T(number);
			return new Double_T(value % convert.value);
		}

		public IFloat Root(IFloat root)
			=> Pow(root.Inverse());

		public IFloat Sin()
			=> new Double_T((Double)Math.Sin(value));

		public IFloat Sinh()
			=> new Double_T((Double)Math.Sinh(value));

		public IFloat Sqrt()
			=> new Double_T((Double)Math.Sqrt(value));

		public INumber Subtract(INumber number) {
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if (number is IInteger || targetSize < sizeof(Double))
				number = ValueConverter.CastToDouble_T(number);
			else if (targetSize > sizeof(Double))
				return number.Negate().Subtract(this.Negate());

			Double_T convert = ValueConverter.CastToDouble_T(number);
			return new Double_T(value - convert.value);
		}

		public IFloat Tan()
			=> new Double_T((Double)Math.Tan(value));

		public IFloat Tanh()
			=> new Double_T((Double)Math.Tanh(value));
	}
	
	[TextTemplateGenerated]
	public struct Decimal_T : IFloat {
		private Decimal value;

		public object Value => value;

		public Decimal ActualValue => value;

		public bool IsZero => value == 0;

		public bool IsNegative => value < 0;

		public bool IsNaN => false;

		public bool IsInfinity => false;

		public Decimal_T(Decimal value) {
			this.value = value;
		}

		public Decimal_T(Single value) {
			this.value = (Decimal)value;
		}

		public INumber Abs()
			=> new Decimal_T((Decimal)Math.Abs(value));

		public IFloat Acos()
			=> new Decimal_T(DecimalMath.DecimalEx.ACos(value));

		public IFloat Acosh()
			=> throw new InvalidOperationException("Performing \"acosh\" on <f128> values is not supported");

		public INumber Add(INumber number) {
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if (number is IInteger || targetSize < sizeof(Decimal))
				number = ValueConverter.CastToDecimal_T(number);
			else if (targetSize > sizeof(Decimal))
				return number.Add(this);

			Decimal_T convert = ValueConverter.CastToDecimal_T(number);
			return new Decimal_T(value + convert.value);
		}

		public IFloat Asin()
			=> new Decimal_T(DecimalMath.DecimalEx.ASin(value));

		public IFloat Asinh()
			=> throw new InvalidOperationException("Performing \"asinh\" on <f128> values is not supported");

		public IFloat Atan()
			=> new Decimal_T(DecimalMath.DecimalEx.ATan(value));

		public IFloat Atan2(IFloat divisor, bool inverseLogic = false) {
			if (divisor is not INumber number)
				throw new InvalidOperationException("Number was not a floating-point value");

			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if (targetSize < sizeof(Decimal))
				number = ValueConverter.CastToDecimal_T(number);
			else if (targetSize > sizeof(Decimal))
				return divisor.Atan2(this, true);

			Decimal_T convert = ValueConverter.CastToDecimal_T(number);
			return new Decimal_T(DecimalMath.DecimalEx.ATan2(value, convert.value));
		}

		public IFloat Atanh()
			=> throw new InvalidOperationException("Performing \"atanh\" on <f128> values is not supported");

		public INumber Ceiling()
			=> new Decimal_T((Decimal)Math.Ceiling(value));

		public IFloat Cos()
			=> new Decimal_T(DecimalMath.DecimalEx.Cos(value));

		public IFloat Cosh()
			=> throw new InvalidOperationException("Performing \"cosh\" on <f128> values is not supported");
		
		public INumber Decrement()
			=> new Decimal_T(value + 1);

		public INumber Divide(INumber number, bool inverseLogic = false) {
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if (number is IInteger || targetSize < sizeof(Decimal))
				number = ValueConverter.CastToDecimal_T(number);
			else if (targetSize > sizeof(Decimal))
				return number.Divide(this, true);

			Decimal_T convert = ValueConverter.CastToDecimal_T(number);
			return new Decimal_T(!inverseLogic ? value / convert.value : convert.value / value);
		}

		public IFloat Exp()
			=> new Decimal_T(DecimalMath.DecimalEx.Exp(value));

		public INumber Floor()
			=> new Decimal_T((Decimal)Math.Floor(value));

		public IInteger GetBits()
			=> throw new InvalidOperationException("Retrieving the bits on an <f128> instance is not supported");

		public INumber Increment()
			=> new Decimal_T(value + 1);

		public IFloat Inverse()
			=> (new Decimal_T(1f).Divide(this) as IFloat)!;

		public IFloat Ln()
			=> new Decimal_T(DecimalMath.DecimalEx.Log(value));

		public IFloat Log10()
			=> new Decimal_T(DecimalMath.DecimalEx.Log10(value));

		public IFloat Log2()
			=> new Decimal_T(DecimalMath.DecimalEx.Log2(value));

		public INumber Multiply(INumber number) {
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if (number is IInteger || targetSize < sizeof(Decimal))
				number = ValueConverter.CastToDecimal_T(number);
			else if (targetSize > sizeof(Decimal))
				return number.Multiply(this);

			Decimal_T convert = ValueConverter.CastToDecimal_T(number);
			return new Decimal_T(value * convert.value);
		}

		public INumber Negate()
			=> new Decimal_T(-value);

		public IFloat Pow(IFloat exponent) {
			if (exponent is not INumber number)
				throw new InvalidOperationException("Number was not a floating-point value");
			
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if (number is IInteger || targetSize < sizeof(Decimal))
				number = ValueConverter.CastToDecimal_T(number);
			else if (targetSize > sizeof(Decimal))
				return ValueConverter.Constants.GetConst_E(number.Value.GetType()).Pow((number.Multiply((Ln() as INumber)!) as IFloat)!); //x^y = e^(y * ln(x))

			Decimal_T convert = ValueConverter.CastToDecimal_T(number);
			return new Decimal_T(value * convert.value);
		}

		public INumber Remainder(INumber number) {
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if (number is IInteger || targetSize < sizeof(Decimal))
				number = ValueConverter.CastToDecimal_T(number);
			else if (targetSize > sizeof(Decimal))
				return number.Remainder(this);

			Decimal_T convert = ValueConverter.CastToDecimal_T(number);
			return new Decimal_T(value % convert.value);
		}

		public IFloat Root(IFloat root)
			=> Pow(root.Inverse());

		public IFloat Sin()
			=> new Decimal_T(DecimalMath.DecimalEx.Sin(value));

		public IFloat Sinh()
			=> throw new InvalidOperationException("Performing \"sinh\" on <f128> values is not supported");

		public IFloat Sqrt()
			=> new Decimal_T(DecimalMath.DecimalEx.Sqrt(value));

		public INumber Subtract(INumber number) {
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if (number is IInteger || targetSize < sizeof(Decimal))
				number = ValueConverter.CastToDecimal_T(number);
			else if (targetSize > sizeof(Decimal))
				return number.Negate().Subtract(this.Negate());

			Decimal_T convert = ValueConverter.CastToDecimal_T(number);
			return new Decimal_T(value - convert.value);
		}

		public IFloat Tan()
			=> new Decimal_T(DecimalMath.DecimalEx.Tan(value));

		public IFloat Tanh()
			=> throw new InvalidOperationException("Performing \"tanh\" on <f128> values is not supported");
	}
	
}