using Chips.Runtime.Meta;
using Chips.Runtime.Utility;
using System;

#pragma warning disable CS0162
namespace Chips.Runtime.Types.NumberProcessing {
	[TextTemplateGenerated]
	public struct SByte_T : IInteger<SByte>, INumberConstants<SByte_T> {
		private SByte value;

		public readonly object Value => value;

		public readonly SByte ActualValue => value;

		public readonly bool IsZero => value == 0;

		public readonly bool IsNegative => value < 0;

		public readonly int NumericSize => sizeof(SByte);

		public static SByte_T Zero => new SByte_T((SByte)0);

		public static SByte_T One => new SByte_T((SByte)1);

		public SByte_T(SByte value) {
			this.value = value;
		}

		public SByte_T(Int32 value) {
			this.value = (SByte)value;
		}

		public readonly INumber Upcast(INumber number) {
			return TypeTracking.ShouldUpcast(number, this) ? ValueConverter.CastToSByte_T(number) : number;
		}

		public readonly INumber Abs() {
			if (value == SByte.MinValue) {
				Registers.F.Overflow = true;
				return new SByte_T(SByte.MaxValue);
			}

			return new SByte_T(Math.Abs(value));
		}

		public readonly INumber Add(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Add(this);

			SByte_T convert = ValueConverter.CastToSByte_T(number);

			if ((value < 0 && convert.value < 0 && value + convert.value > 0) || (value > 0 && convert.value > 0 && value + convert.value < 0)) {
				Registers.F.Overflow = true;
				return new SByte_T(value < 0 ? SByte.MinValue : SByte.MaxValue);
			}

			return new SByte_T(unchecked(value + convert.value));
		}

		public readonly IInteger And(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).And(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.And(this);

			SByte_T convert = ValueConverter.CastToSByte_T(number);
			return new SByte_T(value & convert.value);
		}

		public readonly IInteger ArithmeticRotateLeft() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = unchecked(value & (SByte)((SByte)1 << (8 * sizeof(SByte) - 1))) != 0;
			
			var i = new SByte_T(value << 1);
			if (carry)
				i.value |= 1;

			return i;
		}

		public readonly IInteger ArithmeticRotateRight() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = (value & 1) != 0;
			
			var i = new SByte_T(value >> 1);
			if (carry)
				i.value = (SByte)unchecked(i.value | ((SByte)1 << (8 * sizeof(SByte) - 1)));

			return i;
		}

		public readonly IInteger ArithmeticShiftLeft()
			=> new SByte_T(value << 1);

		public readonly IInteger ArithmeticShiftRight()
			=> new SByte_T(value >> 1);

		public readonly void Compare(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				upcast.Compare(number);
				return;
			}

			if (TypeTracking.ShouldUpcast(this, number)) {
				number.Upcast(this).Compare(number);
				return;
			}

			INumber sub = this.Subtract(number);

			Registers.F.Negative = sub.IsNegative;
			Registers.F.Zero = sub.IsZero;
		}

		public readonly INumber Decrement()
			=> new SByte_T(unchecked(value - 1));

		public readonly INumber Divide(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Divide(number);

			SByte_T convert = ValueConverter.CastToSByte_T(number);
			return new SByte_T(value / convert.value);
		}

		public readonly INumber Increment()
			=> new SByte_T(unchecked(value + 1));

		public readonly INumber Modulus(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Modulus(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Modulus(number);

			SByte_T convert = ValueConverter.CastToSByte_T(number);
			return new SByte_T(value % convert.value);
		}

		public readonly INumber Multiply(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			SByte_T convert = ValueConverter.CastToSByte_T(number);
			return new SByte_T(unchecked(value * convert.value));
		}

		public readonly INumber Negate() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Negate();
			}

			return new SByte_T(-value);
		}

		public readonly IInteger Not() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Not();
			}

			return new SByte_T(~value);
		}

		public readonly IInteger Or(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Or(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Or(this);

			SByte_T convert = ValueConverter.CastToSByte_T(number);
			return new SByte_T(value | convert.value);
		}

		public readonly INumber Subtract(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Subtract(number);

			SByte_T convert = ValueConverter.CastToSByte_T(number);
			return new SByte_T(unchecked(value - convert.value));
		}

		public readonly IInteger Xor(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Xor(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Xor(this);

			SByte_T convert = ValueConverter.CastToSByte_T(number);
			return new SByte_T(value ^ convert.value);
		}
	}

	[TextTemplateGenerated]
	public struct Int16_T : IInteger<Int16>, INumberConstants<Int16_T> {
		private Int16 value;

		public readonly object Value => value;

		public readonly Int16 ActualValue => value;

		public readonly bool IsZero => value == 0;

		public readonly bool IsNegative => value < 0;

		public readonly int NumericSize => sizeof(Int16);

		public static Int16_T Zero => new Int16_T((Int16)0);

		public static Int16_T One => new Int16_T((Int16)1);

		public Int16_T(Int16 value) {
			this.value = value;
		}

		public Int16_T(Int32 value) {
			this.value = (Int16)value;
		}

		public readonly INumber Upcast(INumber number) {
			return TypeTracking.ShouldUpcast(number, this) ? ValueConverter.CastToInt16_T(number) : number;
		}

		public readonly INumber Abs() {
			if (value == Int16.MinValue) {
				Registers.F.Overflow = true;
				return new Int16_T(Int16.MaxValue);
			}

			return new Int16_T(Math.Abs(value));
		}

		public readonly INumber Add(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Add(this);

			Int16_T convert = ValueConverter.CastToInt16_T(number);

			if ((value < 0 && convert.value < 0 && value + convert.value > 0) || (value > 0 && convert.value > 0 && value + convert.value < 0)) {
				Registers.F.Overflow = true;
				return new Int16_T(value < 0 ? Int16.MinValue : Int16.MaxValue);
			}

			return new Int16_T(unchecked(value + convert.value));
		}

		public readonly IInteger And(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).And(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.And(this);

			Int16_T convert = ValueConverter.CastToInt16_T(number);
			return new Int16_T(value & convert.value);
		}

		public readonly IInteger ArithmeticRotateLeft() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = unchecked(value & (Int16)((Int16)1 << (8 * sizeof(Int16) - 1))) != 0;
			
			var i = new Int16_T(value << 1);
			if (carry)
				i.value |= 1;

			return i;
		}

		public readonly IInteger ArithmeticRotateRight() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = (value & 1) != 0;
			
			var i = new Int16_T(value >> 1);
			if (carry)
				i.value = (Int16)unchecked(i.value | ((Int16)1 << (8 * sizeof(Int16) - 1)));

			return i;
		}

		public readonly IInteger ArithmeticShiftLeft()
			=> new Int16_T(value << 1);

		public readonly IInteger ArithmeticShiftRight()
			=> new Int16_T(value >> 1);

		public readonly void Compare(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				upcast.Compare(number);
				return;
			}

			if (TypeTracking.ShouldUpcast(this, number)) {
				number.Upcast(this).Compare(number);
				return;
			}

			INumber sub = this.Subtract(number);

			Registers.F.Negative = sub.IsNegative;
			Registers.F.Zero = sub.IsZero;
		}

		public readonly INumber Decrement()
			=> new Int16_T(unchecked(value - 1));

		public readonly INumber Divide(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Divide(number);

			Int16_T convert = ValueConverter.CastToInt16_T(number);
			return new Int16_T(value / convert.value);
		}

		public readonly INumber Increment()
			=> new Int16_T(unchecked(value + 1));

		public readonly INumber Modulus(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Modulus(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Modulus(number);

			Int16_T convert = ValueConverter.CastToInt16_T(number);
			return new Int16_T(value % convert.value);
		}

		public readonly INumber Multiply(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			Int16_T convert = ValueConverter.CastToInt16_T(number);
			return new Int16_T(unchecked(value * convert.value));
		}

		public readonly INumber Negate() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Negate();
			}

			return new Int16_T(-value);
		}

		public readonly IInteger Not() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Not();
			}

			return new Int16_T(~value);
		}

		public readonly IInteger Or(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Or(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Or(this);

			Int16_T convert = ValueConverter.CastToInt16_T(number);
			return new Int16_T(value | convert.value);
		}

		public readonly INumber Subtract(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Subtract(number);

			Int16_T convert = ValueConverter.CastToInt16_T(number);
			return new Int16_T(unchecked(value - convert.value));
		}

		public readonly IInteger Xor(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Xor(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Xor(this);

			Int16_T convert = ValueConverter.CastToInt16_T(number);
			return new Int16_T(value ^ convert.value);
		}
	}

	[TextTemplateGenerated]
	public struct Int32_T : IInteger<Int32>, INumberConstants<Int32_T> {
		private Int32 value;

		public readonly object Value => value;

		public readonly Int32 ActualValue => value;

		public readonly bool IsZero => value == 0;

		public readonly bool IsNegative => value < 0;

		public readonly int NumericSize => sizeof(Int32);

		public static Int32_T Zero => new Int32_T((Int32)0);

		public static Int32_T One => new Int32_T((Int32)1);

		public Int32_T(Int32 value) {
			this.value = value;
		}


		public readonly INumber Upcast(INumber number) {
			return TypeTracking.ShouldUpcast(number, this) ? ValueConverter.CastToInt32_T(number) : number;
		}

		public readonly INumber Abs() {
			if (value == Int32.MinValue) {
				Registers.F.Overflow = true;
				return new Int32_T(Int32.MaxValue);
			}

			return new Int32_T(Math.Abs(value));
		}

		public readonly INumber Add(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Add(this);

			Int32_T convert = ValueConverter.CastToInt32_T(number);

			if ((value < 0 && convert.value < 0 && value + convert.value > 0) || (value > 0 && convert.value > 0 && value + convert.value < 0)) {
				Registers.F.Overflow = true;
				return new Int32_T(value < 0 ? Int32.MinValue : Int32.MaxValue);
			}

			return new Int32_T(unchecked(value + convert.value));
		}

		public readonly IInteger And(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).And(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.And(this);

			Int32_T convert = ValueConverter.CastToInt32_T(number);
			return new Int32_T(value & convert.value);
		}

		public readonly IInteger ArithmeticRotateLeft() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = unchecked(value & (Int32)((Int32)1 << (8 * sizeof(Int32) - 1))) != 0;
			
			var i = new Int32_T(value << 1);
			if (carry)
				i.value |= 1;

			return i;
		}

		public readonly IInteger ArithmeticRotateRight() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = (value & 1) != 0;
			
			var i = new Int32_T(value >> 1);
			if (carry)
				i.value = (Int32)unchecked(i.value | ((Int32)1 << (8 * sizeof(Int32) - 1)));

			return i;
		}

		public readonly IInteger ArithmeticShiftLeft()
			=> new Int32_T(value << 1);

		public readonly IInteger ArithmeticShiftRight()
			=> new Int32_T(value >> 1);

		public readonly void Compare(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				upcast.Compare(number);
				return;
			}

			if (TypeTracking.ShouldUpcast(this, number)) {
				number.Upcast(this).Compare(number);
				return;
			}

			INumber sub = this.Subtract(number);

			Registers.F.Negative = sub.IsNegative;
			Registers.F.Zero = sub.IsZero;
		}

		public readonly INumber Decrement()
			=> new Int32_T(unchecked(value - 1));

		public readonly INumber Divide(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Divide(number);

			Int32_T convert = ValueConverter.CastToInt32_T(number);
			return new Int32_T(value / convert.value);
		}

		public readonly INumber Increment()
			=> new Int32_T(unchecked(value + 1));

		public readonly INumber Modulus(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Modulus(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Modulus(number);

			Int32_T convert = ValueConverter.CastToInt32_T(number);
			return new Int32_T(value % convert.value);
		}

		public readonly INumber Multiply(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			Int32_T convert = ValueConverter.CastToInt32_T(number);
			return new Int32_T(unchecked(value * convert.value));
		}

		public readonly INumber Negate() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Negate();
			}

			return new Int32_T(-value);
		}

		public readonly IInteger Not() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Not();
			}

			return new Int32_T(~value);
		}

		public readonly IInteger Or(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Or(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Or(this);

			Int32_T convert = ValueConverter.CastToInt32_T(number);
			return new Int32_T(value | convert.value);
		}

		public readonly INumber Subtract(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Subtract(number);

			Int32_T convert = ValueConverter.CastToInt32_T(number);
			return new Int32_T(unchecked(value - convert.value));
		}

		public readonly IInteger Xor(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Xor(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Xor(this);

			Int32_T convert = ValueConverter.CastToInt32_T(number);
			return new Int32_T(value ^ convert.value);
		}
	}

	[TextTemplateGenerated]
	public struct Int64_T : IInteger<Int64>, INumberConstants<Int64_T> {
		private Int64 value;

		public readonly object Value => value;

		public readonly Int64 ActualValue => value;

		public readonly bool IsZero => value == 0;

		public readonly bool IsNegative => value < 0;

		public readonly int NumericSize => sizeof(Int64);

		public static Int64_T Zero => new Int64_T((Int64)0);

		public static Int64_T One => new Int64_T((Int64)1);

		public Int64_T(Int64 value) {
			this.value = value;
		}

		public Int64_T(Int32 value) {
			this.value = (Int64)value;
		}

		public readonly INumber Upcast(INumber number) {
			return TypeTracking.ShouldUpcast(number, this) ? ValueConverter.CastToInt64_T(number) : number;
		}

		public readonly INumber Abs() {
			if (value == Int64.MinValue) {
				Registers.F.Overflow = true;
				return new Int64_T(Int64.MaxValue);
			}

			return new Int64_T(Math.Abs(value));
		}

		public readonly INumber Add(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Add(this);

			Int64_T convert = ValueConverter.CastToInt64_T(number);

			if ((value < 0 && convert.value < 0 && value + convert.value > 0) || (value > 0 && convert.value > 0 && value + convert.value < 0)) {
				Registers.F.Overflow = true;
				return new Int64_T(value < 0 ? Int64.MinValue : Int64.MaxValue);
			}

			return new Int64_T(unchecked(value + convert.value));
		}

		public readonly IInteger And(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).And(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.And(this);

			Int64_T convert = ValueConverter.CastToInt64_T(number);
			return new Int64_T(value & convert.value);
		}

		public readonly IInteger ArithmeticRotateLeft() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = unchecked(value & (Int64)((Int64)1 << (8 * sizeof(Int64) - 1))) != 0;
			
			var i = new Int64_T(value << 1);
			if (carry)
				i.value |= 1;

			return i;
		}

		public readonly IInteger ArithmeticRotateRight() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = (value & 1) != 0;
			
			var i = new Int64_T(value >> 1);
			if (carry)
				i.value = (Int64)unchecked(i.value | ((Int64)1 << (8 * sizeof(Int64) - 1)));

			return i;
		}

		public readonly IInteger ArithmeticShiftLeft()
			=> new Int64_T(value << 1);

		public readonly IInteger ArithmeticShiftRight()
			=> new Int64_T(value >> 1);

		public readonly void Compare(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				upcast.Compare(number);
				return;
			}

			if (TypeTracking.ShouldUpcast(this, number)) {
				number.Upcast(this).Compare(number);
				return;
			}

			INumber sub = this.Subtract(number);

			Registers.F.Negative = sub.IsNegative;
			Registers.F.Zero = sub.IsZero;
		}

		public readonly INumber Decrement()
			=> new Int64_T(unchecked(value - 1));

		public readonly INumber Divide(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Divide(number);

			Int64_T convert = ValueConverter.CastToInt64_T(number);
			return new Int64_T(value / convert.value);
		}

		public readonly INumber Increment()
			=> new Int64_T(unchecked(value + 1));

		public readonly INumber Modulus(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Modulus(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Modulus(number);

			Int64_T convert = ValueConverter.CastToInt64_T(number);
			return new Int64_T(value % convert.value);
		}

		public readonly INumber Multiply(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			Int64_T convert = ValueConverter.CastToInt64_T(number);
			return new Int64_T(unchecked(value * convert.value));
		}

		public readonly INumber Negate() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Negate();
			}

			return new Int64_T(-value);
		}

		public readonly IInteger Not() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Not();
			}

			return new Int64_T(~value);
		}

		public readonly IInteger Or(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Or(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Or(this);

			Int64_T convert = ValueConverter.CastToInt64_T(number);
			return new Int64_T(value | convert.value);
		}

		public readonly INumber Subtract(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Subtract(number);

			Int64_T convert = ValueConverter.CastToInt64_T(number);
			return new Int64_T(unchecked(value - convert.value));
		}

		public readonly IInteger Xor(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Xor(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Xor(this);

			Int64_T convert = ValueConverter.CastToInt64_T(number);
			return new Int64_T(value ^ convert.value);
		}
	}

	[TextTemplateGenerated]
	public struct Byte_T : IUnsignedInteger<Byte>, INumberConstants<Byte_T> {
		private Byte value;

		public readonly object Value => value;

		public readonly Byte ActualValue => value;

		public readonly bool IsZero => value == 0;

		public readonly bool IsNegative => false;

		public readonly int NumericSize => sizeof(Byte);

		public static Byte_T Zero => new Byte_T((Byte)0);

		public static Byte_T One => new Byte_T((Byte)1);

		public Byte_T(Byte value) {
			this.value = value;
		}

		public Byte_T(Int32 value) {
			this.value = (Byte)value;
		}

		public readonly INumber Upcast(INumber number) {
			return TypeTracking.ShouldUpcast(number, this) ? ValueConverter.CastToByte_T(number) : number;
		}

		public readonly INumber Abs()
			=> new Byte_T(value);

		public readonly INumber Add(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Add(this);

			Byte_T convert = ValueConverter.CastToByte_T(number);

			if (value + convert.value < value) {
				Registers.F.Overflow = true;
				return new Byte_T(Byte.MaxValue);
			}

			return new Byte_T(unchecked(value + convert.value));
		}

		public readonly IInteger And(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).And(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.And(this);

			Byte_T convert = ValueConverter.CastToByte_T(number);
			return new Byte_T(value & convert.value);
		}

		public readonly IInteger ArithmeticRotateLeft() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = unchecked(value & (Byte)((Byte)1 << (8 * sizeof(Byte) - 1))) != 0;
			
			var i = new Byte_T(value << 1);
			if (carry)
				i.value |= 1;

			return i;
		}

		public readonly IInteger ArithmeticRotateRight() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = (value & 1) != 0;
			
			var i = new Byte_T(value >> 1);
			if (carry)
				i.value = (Byte)unchecked(i.value | ((Byte)1 << (8 * sizeof(Byte) - 1)));

			return i;
		}

		public readonly IInteger ArithmeticShiftLeft()
			=> new Byte_T(value << 1);

		public readonly IInteger ArithmeticShiftRight()
			=> new Byte_T(value >> 1);

		public readonly void Compare(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				upcast.Compare(number);
				return;
			}

			if (TypeTracking.ShouldUpcast(this, number)) {
				number.Upcast(this).Compare(number);
				return;
			}

			INumber sub = this.Subtract(number);

			Registers.F.Negative = sub.Divide(this).IsZero;  // If (this - number) is less than this, then (sub / this) will be 0 due to integer division
			Registers.F.Zero = sub.IsZero;
		}

		public readonly INumber Decrement()
			=> new Byte_T(unchecked(value - 1));

		public readonly INumber Divide(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Divide(number);

			Byte_T convert = ValueConverter.CastToByte_T(number);
			return new Byte_T(value / convert.value);
		}

		public readonly INumber Increment()
			=> new Byte_T(unchecked(value + 1));

		public readonly INumber Modulus(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Modulus(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Modulus(number);

			Byte_T convert = ValueConverter.CastToByte_T(number);
			return new Byte_T(value % convert.value);
		}

		public readonly INumber Multiply(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			Byte_T convert = ValueConverter.CastToByte_T(number);
			return new Byte_T(unchecked(value * convert.value));
		}

		public readonly INumber Negate() {
			throw new InvalidOperationException("Negation cannot be performed on unsigned integers");
		}

		public readonly IInteger Not() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Not();
			}

			return new Byte_T(~value);
		}

		public readonly IInteger Or(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Or(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Or(this);

			Byte_T convert = ValueConverter.CastToByte_T(number);
			return new Byte_T(value | convert.value);
		}

		public readonly INumber Subtract(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Subtract(number);

			Byte_T convert = ValueConverter.CastToByte_T(number);
			return new Byte_T(unchecked(value - convert.value));
		}

		public readonly IInteger Xor(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Xor(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Xor(this);

			Byte_T convert = ValueConverter.CastToByte_T(number);
			return new Byte_T(value ^ convert.value);
		}
	}

	[TextTemplateGenerated]
	public struct UInt16_T : IUnsignedInteger<UInt16>, INumberConstants<UInt16_T> {
		private UInt16 value;

		public readonly object Value => value;

		public readonly UInt16 ActualValue => value;

		public readonly bool IsZero => value == 0;

		public readonly bool IsNegative => false;

		public readonly int NumericSize => sizeof(UInt16);

		public static UInt16_T Zero => new UInt16_T((UInt16)0);

		public static UInt16_T One => new UInt16_T((UInt16)1);

		public UInt16_T(UInt16 value) {
			this.value = value;
		}

		public UInt16_T(Int32 value) {
			this.value = (UInt16)value;
		}

		public readonly INumber Upcast(INumber number) {
			return TypeTracking.ShouldUpcast(number, this) ? ValueConverter.CastToUInt16_T(number) : number;
		}

		public readonly INumber Abs()
			=> new UInt16_T(value);

		public readonly INumber Add(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Add(this);

			UInt16_T convert = ValueConverter.CastToUInt16_T(number);

			if (value + convert.value < value) {
				Registers.F.Overflow = true;
				return new UInt16_T(UInt16.MaxValue);
			}

			return new UInt16_T(unchecked(value + convert.value));
		}

		public readonly IInteger And(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).And(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.And(this);

			UInt16_T convert = ValueConverter.CastToUInt16_T(number);
			return new UInt16_T(value & convert.value);
		}

		public readonly IInteger ArithmeticRotateLeft() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = unchecked(value & (UInt16)((UInt16)1 << (8 * sizeof(UInt16) - 1))) != 0;
			
			var i = new UInt16_T(value << 1);
			if (carry)
				i.value |= 1;

			return i;
		}

		public readonly IInteger ArithmeticRotateRight() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = (value & 1) != 0;
			
			var i = new UInt16_T(value >> 1);
			if (carry)
				i.value = (UInt16)unchecked(i.value | ((UInt16)1 << (8 * sizeof(UInt16) - 1)));

			return i;
		}

		public readonly IInteger ArithmeticShiftLeft()
			=> new UInt16_T(value << 1);

		public readonly IInteger ArithmeticShiftRight()
			=> new UInt16_T(value >> 1);

		public readonly void Compare(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				upcast.Compare(number);
				return;
			}

			if (TypeTracking.ShouldUpcast(this, number)) {
				number.Upcast(this).Compare(number);
				return;
			}

			INumber sub = this.Subtract(number);

			Registers.F.Negative = sub.Divide(this).IsZero;  // If (this - number) is less than this, then (sub / this) will be 0 due to integer division
			Registers.F.Zero = sub.IsZero;
		}

		public readonly INumber Decrement()
			=> new UInt16_T(unchecked(value - 1));

		public readonly INumber Divide(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Divide(number);

			UInt16_T convert = ValueConverter.CastToUInt16_T(number);
			return new UInt16_T(value / convert.value);
		}

		public readonly INumber Increment()
			=> new UInt16_T(unchecked(value + 1));

		public readonly INumber Modulus(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Modulus(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Modulus(number);

			UInt16_T convert = ValueConverter.CastToUInt16_T(number);
			return new UInt16_T(value % convert.value);
		}

		public readonly INumber Multiply(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			UInt16_T convert = ValueConverter.CastToUInt16_T(number);
			return new UInt16_T(unchecked(value * convert.value));
		}

		public readonly INumber Negate() {
			throw new InvalidOperationException("Negation cannot be performed on unsigned integers");
		}

		public readonly IInteger Not() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Not();
			}

			return new UInt16_T(~value);
		}

		public readonly IInteger Or(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Or(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Or(this);

			UInt16_T convert = ValueConverter.CastToUInt16_T(number);
			return new UInt16_T(value | convert.value);
		}

		public readonly INumber Subtract(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Subtract(number);

			UInt16_T convert = ValueConverter.CastToUInt16_T(number);
			return new UInt16_T(unchecked(value - convert.value));
		}

		public readonly IInteger Xor(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Xor(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Xor(this);

			UInt16_T convert = ValueConverter.CastToUInt16_T(number);
			return new UInt16_T(value ^ convert.value);
		}
	}

	[TextTemplateGenerated]
	public struct UInt32_T : IUnsignedInteger<UInt32>, INumberConstants<UInt32_T> {
		private UInt32 value;

		public readonly object Value => value;

		public readonly UInt32 ActualValue => value;

		public readonly bool IsZero => value == 0;

		public readonly bool IsNegative => false;

		public readonly int NumericSize => sizeof(UInt32);

		public static UInt32_T Zero => new UInt32_T((UInt32)0);

		public static UInt32_T One => new UInt32_T((UInt32)1);

		public UInt32_T(UInt32 value) {
			this.value = value;
		}

		public UInt32_T(Int32 value) {
			this.value = (UInt32)value;
		}

		public readonly INumber Upcast(INumber number) {
			return TypeTracking.ShouldUpcast(number, this) ? ValueConverter.CastToUInt32_T(number) : number;
		}

		public readonly INumber Abs()
			=> new UInt32_T(value);

		public readonly INumber Add(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Add(this);

			UInt32_T convert = ValueConverter.CastToUInt32_T(number);

			if (value + convert.value < value) {
				Registers.F.Overflow = true;
				return new UInt32_T(UInt32.MaxValue);
			}

			return new UInt32_T(unchecked(value + convert.value));
		}

		public readonly IInteger And(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).And(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.And(this);

			UInt32_T convert = ValueConverter.CastToUInt32_T(number);
			return new UInt32_T(value & convert.value);
		}

		public readonly IInteger ArithmeticRotateLeft() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = unchecked(value & (UInt32)((UInt32)1 << (8 * sizeof(UInt32) - 1))) != 0;
			
			var i = new UInt32_T(value << 1);
			if (carry)
				i.value |= 1;

			return i;
		}

		public readonly IInteger ArithmeticRotateRight() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = (value & 1) != 0;
			
			var i = new UInt32_T(value >> 1);
			if (carry)
				i.value = (UInt32)unchecked(i.value | ((UInt32)1 << (8 * sizeof(UInt32) - 1)));

			return i;
		}

		public readonly IInteger ArithmeticShiftLeft()
			=> new UInt32_T(value << 1);

		public readonly IInteger ArithmeticShiftRight()
			=> new UInt32_T(value >> 1);

		public readonly void Compare(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				upcast.Compare(number);
				return;
			}

			if (TypeTracking.ShouldUpcast(this, number)) {
				number.Upcast(this).Compare(number);
				return;
			}

			INumber sub = this.Subtract(number);

			Registers.F.Negative = sub.Divide(this).IsZero;  // If (this - number) is less than this, then (sub / this) will be 0 due to integer division
			Registers.F.Zero = sub.IsZero;
		}

		public readonly INumber Decrement()
			=> new UInt32_T(unchecked(value - 1));

		public readonly INumber Divide(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Divide(number);

			UInt32_T convert = ValueConverter.CastToUInt32_T(number);
			return new UInt32_T(value / convert.value);
		}

		public readonly INumber Increment()
			=> new UInt32_T(unchecked(value + 1));

		public readonly INumber Modulus(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Modulus(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Modulus(number);

			UInt32_T convert = ValueConverter.CastToUInt32_T(number);
			return new UInt32_T(value % convert.value);
		}

		public readonly INumber Multiply(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			UInt32_T convert = ValueConverter.CastToUInt32_T(number);
			return new UInt32_T(unchecked(value * convert.value));
		}

		public readonly INumber Negate() {
			throw new InvalidOperationException("Negation cannot be performed on unsigned integers");
		}

		public readonly IInteger Not() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Not();
			}

			return new UInt32_T(~value);
		}

		public readonly IInteger Or(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Or(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Or(this);

			UInt32_T convert = ValueConverter.CastToUInt32_T(number);
			return new UInt32_T(value | convert.value);
		}

		public readonly INumber Subtract(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Subtract(number);

			UInt32_T convert = ValueConverter.CastToUInt32_T(number);
			return new UInt32_T(unchecked(value - convert.value));
		}

		public readonly IInteger Xor(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Xor(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Xor(this);

			UInt32_T convert = ValueConverter.CastToUInt32_T(number);
			return new UInt32_T(value ^ convert.value);
		}
	}

	[TextTemplateGenerated]
	public struct UInt64_T : IUnsignedInteger<UInt64>, INumberConstants<UInt64_T> {
		private UInt64 value;

		public readonly object Value => value;

		public readonly UInt64 ActualValue => value;

		public readonly bool IsZero => value == 0;

		public readonly bool IsNegative => false;

		public readonly int NumericSize => sizeof(UInt64);

		public static UInt64_T Zero => new UInt64_T((UInt64)0);

		public static UInt64_T One => new UInt64_T((UInt64)1);

		public UInt64_T(UInt64 value) {
			this.value = value;
		}

		public UInt64_T(Int32 value) {
			this.value = (UInt64)value;
		}

		public readonly INumber Upcast(INumber number) {
			return TypeTracking.ShouldUpcast(number, this) ? ValueConverter.CastToUInt64_T(number) : number;
		}

		public readonly INumber Abs()
			=> new UInt64_T(value);

		public readonly INumber Add(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Add(this);

			UInt64_T convert = ValueConverter.CastToUInt64_T(number);

			if (value + convert.value < value) {
				Registers.F.Overflow = true;
				return new UInt64_T(UInt64.MaxValue);
			}

			return new UInt64_T(unchecked(value + convert.value));
		}

		public readonly IInteger And(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).And(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.And(this);

			UInt64_T convert = ValueConverter.CastToUInt64_T(number);
			return new UInt64_T(value & convert.value);
		}

		public readonly IInteger ArithmeticRotateLeft() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = unchecked(value & (UInt64)((UInt64)1 << (8 * sizeof(UInt64) - 1))) != 0;
			
			var i = new UInt64_T(value << 1);
			if (carry)
				i.value |= 1;

			return i;
		}

		public readonly IInteger ArithmeticRotateRight() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = (value & 1) != 0;
			
			var i = new UInt64_T(value >> 1);
			if (carry)
				i.value = (UInt64)unchecked(i.value | ((UInt64)1 << (8 * sizeof(UInt64) - 1)));

			return i;
		}

		public readonly IInteger ArithmeticShiftLeft()
			=> new UInt64_T(value << 1);

		public readonly IInteger ArithmeticShiftRight()
			=> new UInt64_T(value >> 1);

		public readonly void Compare(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				upcast.Compare(number);
				return;
			}

			if (TypeTracking.ShouldUpcast(this, number)) {
				number.Upcast(this).Compare(number);
				return;
			}

			INumber sub = this.Subtract(number);

			Registers.F.Negative = sub.Divide(this).IsZero;  // If (this - number) is less than this, then (sub / this) will be 0 due to integer division
			Registers.F.Zero = sub.IsZero;
		}

		public readonly INumber Decrement()
			=> new UInt64_T(unchecked(value - 1));

		public readonly INumber Divide(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Divide(number);

			UInt64_T convert = ValueConverter.CastToUInt64_T(number);
			return new UInt64_T(value / convert.value);
		}

		public readonly INumber Increment()
			=> new UInt64_T(unchecked(value + 1));

		public readonly INumber Modulus(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Modulus(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Modulus(number);

			UInt64_T convert = ValueConverter.CastToUInt64_T(number);
			return new UInt64_T(value % convert.value);
		}

		public readonly INumber Multiply(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			UInt64_T convert = ValueConverter.CastToUInt64_T(number);
			return new UInt64_T(unchecked(value * convert.value));
		}

		public readonly INumber Negate() {
			throw new InvalidOperationException("Negation cannot be performed on unsigned integers");
		}

		public readonly IInteger Not() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Not();
			}

			return new UInt64_T(~value);
		}

		public readonly IInteger Or(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Or(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Or(this);

			UInt64_T convert = ValueConverter.CastToUInt64_T(number);
			return new UInt64_T(value | convert.value);
		}

		public readonly INumber Subtract(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Subtract(number);

			UInt64_T convert = ValueConverter.CastToUInt64_T(number);
			return new UInt64_T(unchecked(value - convert.value));
		}

		public readonly IInteger Xor(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Xor(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Xor(this);

			UInt64_T convert = ValueConverter.CastToUInt64_T(number);
			return new UInt64_T(value ^ convert.value);
		}
	}

	[TextTemplateGenerated]
	public unsafe struct IntPtr_T : IInteger<IntPtr>, INumberConstants<IntPtr_T> {
		private IntPtr value;

		public readonly object Value => value;

		public readonly IntPtr ActualValue => value;

		public readonly bool IsZero => value == 0;

		public readonly bool IsNegative => value < 0;

		public readonly int NumericSize => sizeof(IntPtr);

		public static IntPtr_T Zero => new IntPtr_T((IntPtr)0);

		public static IntPtr_T One => new IntPtr_T((IntPtr)1);

		public IntPtr_T(IntPtr value) {
			this.value = value;
		}

		public IntPtr_T(Int32 value) {
			this.value = (IntPtr)value;
		}

		public readonly INumber Upcast(INumber number) {
			return TypeTracking.ShouldUpcast(number, this) ? ValueConverter.CastToIntPtr_T(number) : number;
		}

		public readonly INumber Abs() {
			if (value == IntPtr.MinValue) {
				Registers.F.Overflow = true;
				return new IntPtr_T(IntPtr.MaxValue);
			}

			return new IntPtr_T(Math.Abs(value));
		}

		public readonly INumber Add(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Add(this);

			IntPtr_T convert = ValueConverter.CastToIntPtr_T(number);

			if ((value < 0 && convert.value < 0 && value + convert.value > 0) || (value > 0 && convert.value > 0 && value + convert.value < 0)) {
				Registers.F.Overflow = true;
				return new IntPtr_T(value < 0 ? IntPtr.MinValue : IntPtr.MaxValue);
			}

			return new IntPtr_T(unchecked(value + convert.value));
		}

		public readonly IInteger And(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).And(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.And(this);

			IntPtr_T convert = ValueConverter.CastToIntPtr_T(number);
			return new IntPtr_T(value & convert.value);
		}

		public readonly IInteger ArithmeticRotateLeft() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = unchecked(value & (IntPtr)((IntPtr)1 << (8 * sizeof(IntPtr) - 1))) != 0;
			
			var i = new IntPtr_T(value << 1);
			if (carry)
				i.value |= 1;

			return i;
		}

		public readonly IInteger ArithmeticRotateRight() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = (value & 1) != 0;
			
			var i = new IntPtr_T(value >> 1);
			if (carry)
				i.value = (IntPtr)unchecked(i.value | ((IntPtr)1 << (8 * sizeof(IntPtr) - 1)));

			return i;
		}

		public readonly IInteger ArithmeticShiftLeft()
			=> new IntPtr_T(value << 1);

		public readonly IInteger ArithmeticShiftRight()
			=> new IntPtr_T(value >> 1);

		public readonly void Compare(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				upcast.Compare(number);
				return;
			}

			if (TypeTracking.ShouldUpcast(this, number)) {
				number.Upcast(this).Compare(number);
				return;
			}

			INumber sub = this.Subtract(number);

			Registers.F.Negative = sub.IsNegative;
			Registers.F.Zero = sub.IsZero;
		}

		public readonly INumber Decrement()
			=> new IntPtr_T(unchecked(value - 1));

		public readonly INumber Divide(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Divide(number);

			IntPtr_T convert = ValueConverter.CastToIntPtr_T(number);
			return new IntPtr_T(value / convert.value);
		}

		public readonly INumber Increment()
			=> new IntPtr_T(unchecked(value + 1));

		public readonly INumber Modulus(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Modulus(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Modulus(number);

			IntPtr_T convert = ValueConverter.CastToIntPtr_T(number);
			return new IntPtr_T(value % convert.value);
		}

		public readonly INumber Multiply(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			IntPtr_T convert = ValueConverter.CastToIntPtr_T(number);
			return new IntPtr_T(unchecked(value * convert.value));
		}

		public readonly INumber Negate() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Negate();
			}

			return new IntPtr_T(-value);
		}

		public readonly IInteger Not() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Not();
			}

			return new IntPtr_T(~value);
		}

		public readonly IInteger Or(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Or(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Or(this);

			IntPtr_T convert = ValueConverter.CastToIntPtr_T(number);
			return new IntPtr_T(value | convert.value);
		}

		public readonly INumber Subtract(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Subtract(number);

			IntPtr_T convert = ValueConverter.CastToIntPtr_T(number);
			return new IntPtr_T(unchecked(value - convert.value));
		}

		public readonly IInteger Xor(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Xor(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Xor(this);

			IntPtr_T convert = ValueConverter.CastToIntPtr_T(number);
			return new IntPtr_T(value ^ convert.value);
		}
	}

	[TextTemplateGenerated]
	public unsafe struct UIntPtr_T : IUnsignedInteger<UIntPtr>, INumberConstants<UIntPtr_T> {
		private UIntPtr value;

		public readonly object Value => value;

		public readonly UIntPtr ActualValue => value;

		public readonly bool IsZero => value == 0;

		public readonly bool IsNegative => false;

		public readonly int NumericSize => sizeof(UIntPtr);

		public static UIntPtr_T Zero => new UIntPtr_T((UIntPtr)0);

		public static UIntPtr_T One => new UIntPtr_T((UIntPtr)1);

		public UIntPtr_T(UIntPtr value) {
			this.value = value;
		}

		public UIntPtr_T(Int32 value) {
			this.value = (UIntPtr)value;
		}

		public readonly INumber Upcast(INumber number) {
			return TypeTracking.ShouldUpcast(number, this) ? ValueConverter.CastToUIntPtr_T(number) : number;
		}

		public readonly INumber Abs()
			=> new UIntPtr_T(value);

		public readonly INumber Add(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UIntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Add(this);

			UIntPtr_T convert = ValueConverter.CastToUIntPtr_T(number);

			if (value + convert.value < value) {
				Registers.F.Overflow = true;
				return new UIntPtr_T(UIntPtr.MaxValue);
			}

			return new UIntPtr_T(unchecked(value + convert.value));
		}

		public readonly IInteger And(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UIntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).And(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.And(this);

			UIntPtr_T convert = ValueConverter.CastToUIntPtr_T(number);
			return new UIntPtr_T(value & convert.value);
		}

		public readonly IInteger ArithmeticRotateLeft() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = unchecked(value & (UIntPtr)((UIntPtr)1 << (8 * sizeof(UIntPtr) - 1))) != 0;
			
			var i = new UIntPtr_T(value << 1);
			if (carry)
				i.value |= 1;

			return i;
		}

		public readonly IInteger ArithmeticRotateRight() {
			bool carry = Registers.F.Carry;
			Registers.F.Carry = (value & 1) != 0;
			
			var i = new UIntPtr_T(value >> 1);
			if (carry)
				i.value = (UIntPtr)unchecked(i.value | ((UIntPtr)1 << (8 * sizeof(UIntPtr) - 1)));

			return i;
		}

		public readonly IInteger ArithmeticShiftLeft()
			=> new UIntPtr_T(value << 1);

		public readonly IInteger ArithmeticShiftRight()
			=> new UIntPtr_T(value >> 1);

		public readonly void Compare(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UIntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				upcast.Compare(number);
				return;
			}

			if (TypeTracking.ShouldUpcast(this, number)) {
				number.Upcast(this).Compare(number);
				return;
			}

			INumber sub = this.Subtract(number);

			Registers.F.Negative = sub.Divide(this).IsZero;  // If (this - number) is less than this, then (sub / this) will be 0 due to integer division
			Registers.F.Zero = sub.IsZero;
		}

		public readonly INumber Decrement()
			=> new UIntPtr_T(unchecked(value - 1));

		public readonly INumber Divide(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UIntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Divide(number);

			UIntPtr_T convert = ValueConverter.CastToUIntPtr_T(number);
			return new UIntPtr_T(value / convert.value);
		}

		public readonly INumber Increment()
			=> new UIntPtr_T(unchecked(value + 1));

		public readonly INumber Modulus(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UIntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Modulus(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Modulus(number);

			UIntPtr_T convert = ValueConverter.CastToUIntPtr_T(number);
			return new UIntPtr_T(value % convert.value);
		}

		public readonly INumber Multiply(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UIntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			UIntPtr_T convert = ValueConverter.CastToUIntPtr_T(number);
			return new UIntPtr_T(unchecked(value * convert.value));
		}

		public readonly INumber Negate() {
			throw new InvalidOperationException("Negation cannot be performed on unsigned integers");
		}

		public readonly IInteger Not() {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UIntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Not();
			}

			return new UIntPtr_T(~value);
		}

		public readonly IInteger Or(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UIntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Or(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Or(this);

			UIntPtr_T convert = ValueConverter.CastToUIntPtr_T(number);
			return new UIntPtr_T(value | convert.value);
		}

		public readonly INumber Subtract(INumber number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UIntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Subtract(number);

			UIntPtr_T convert = ValueConverter.CastToUIntPtr_T(number);
			return new UIntPtr_T(unchecked(value - convert.value));
		}

		public readonly IInteger Xor(IInteger number) {
			//For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UIntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Xor(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Xor(this);

			UIntPtr_T convert = ValueConverter.CastToUIntPtr_T(number);
			return new UIntPtr_T(value ^ convert.value);
		}
	}

	[TextTemplateGenerated]
	public struct Single_T : IFloat<Single>, IFloatConstants<Single_T> {
		private Single value;

		public readonly object Value => value;

		public readonly Single ActualValue => value;

		public readonly bool IsZero => value == 0;

		public readonly bool IsNegative => value < 0;

		public readonly int NumericSize => sizeof(Single);

		public static Single_T Zero => new Single_T((Single)0);

		public static Single_T One => new Single_T((Single)1);

		public static Single_T E => new Single_T(MathF.E);

		public readonly bool IsNaN => Single.IsNaN(value);

		public readonly bool IsInfinity => Single.IsInfinity(value);

		public Single_T(Single value) {
			this.value = value;
		}

		public readonly INumber Upcast(INumber number) {
			return TypeTracking.ShouldUpcast(number, this) ? ValueConverter.CastToSingle_T(number) : number;
		}

		public readonly INumber Abs()
			=> new Single_T(MathF.Abs(value));

		public readonly IFloat Acos()
			=> new Single_T(MathF.Acos(value));

		public readonly IFloat Acosh()
			=> new Single_T(MathF.Acosh(value));

		public readonly INumber Add(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Add(this);

			Single_T convert = ValueConverter.CastToSingle_T(number);
			return new Single_T(value + convert.value);
		}

		public readonly IFloat Asin()
			=> new Single_T(MathF.Asin(value));

		public readonly IFloat Asinh()
			=> new Single_T(MathF.Asinh(value));

		public readonly IFloat Atan()
			=> new Single_T(MathF.Atan(value));

		public readonly IFloat Atan2(IFloat divisor) {
			if (TypeTracking.ShouldUpcast(this, divisor))
				return ((IFloat)divisor.Upcast(this)).Atan2(divisor);

			Single_T convert = ValueConverter.CastToSingle_T(divisor);
			return new Single_T(MathF.Atan2(value, convert.value));
		}

		public readonly IFloat Atanh()
			=> new Single_T(MathF.Asinh(value));

		public readonly INumber Ceiling()
			=> new Single_T(MathF.Ceiling(value));

		public readonly void Compare(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number)) {
				number.Upcast(this).Compare(number);
				return;
			}

			INumber sub = this.Subtract(number);

			Registers.F.Negative = sub.IsNegative;
			Registers.F.Zero = sub.IsZero;
		}

		public readonly IFloat Cos()
			=> new Single_T(MathF.Cos(value));

		public readonly IFloat Cosh()
			=> new Single_T(MathF.Cosh(value));
		
		public readonly INumber Decrement()
			=> new Single_T(value + 1);

		public readonly INumber Divide(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Divide(number);

			Single_T convert = ValueConverter.CastToSingle_T(number);
			return new Single_T(value / convert.value);
		}

		public readonly IFloat Exp()
			=> new Single_T(MathF.Exp(value));

		public readonly INumber Floor()
			=> new Single_T(MathF.Floor(value));

		public readonly IInteger GetBits()
			=> ValueConverter.RetrieveFloatingPointBits(this);

		public readonly INumber Increment()
			=> new Single_T(value + 1);

		public readonly IFloat Inverse()
			=> (IFloat)One.Divide(this);

		public readonly IFloat Ln()
			=> new Single_T(MathF.Log(value));

		public readonly IFloat Log10()
			=> new Single_T(MathF.Log10(value));

		public readonly IFloat Log2()
			=> new Single_T(MathF.Log2(value));

		public readonly INumber Modulus(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Modulus(number);

			Single_T convert = ValueConverter.CastToSingle_T(number);
			return new Single_T(value % convert.value);
		}

		public readonly INumber Multiply(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			Single_T convert = ValueConverter.CastToSingle_T(number);
			return new Single_T(value * convert.value);
		}

		public readonly INumber Negate()
			=> new Single_T(-value);

		public readonly IFloat Pow(IFloat exponent) {
			if (TypeTracking.ShouldUpcast(this, exponent))
				return ((IFloat)exponent.Upcast(this)).Pow(exponent);

			Single_T convert = ValueConverter.CastToSingle_T(exponent);
			return new Single_T(MathF.Pow(value, convert.value));
		}

		public readonly IFloat Root(IFloat root)
			=> Pow(root.Inverse());

		public readonly IFloat Sin()
			=> new Single_T(MathF.Sin(value));

		public readonly IFloat Sinh()
			=> new Single_T(MathF.Sinh(value));

		public readonly IFloat Sqrt()
			=> new Single_T(MathF.Sqrt(value));

		public readonly INumber Subtract(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Subtract(number);

			Single_T convert = ValueConverter.CastToSingle_T(number);
			return new Single_T(value - convert.value);
		}

		public readonly IFloat Tan()
			=> new Single_T(MathF.Tan(value));

		public readonly IFloat Tanh()
			=> new Single_T(MathF.Tanh(value));
	}
	
	[TextTemplateGenerated]
	public struct Double_T : IFloat<Double>, IFloatConstants<Double_T> {
		private Double value;

		public readonly object Value => value;

		public readonly Double ActualValue => value;

		public readonly bool IsZero => value == 0;

		public readonly bool IsNegative => value < 0;

		public readonly int NumericSize => sizeof(Double);

		public static Double_T Zero => new Double_T((Double)0);

		public static Double_T One => new Double_T((Double)1);

		public static Double_T E => new Double_T(Math.E);

		public readonly bool IsNaN => Double.IsNaN(value);

		public readonly bool IsInfinity => Double.IsInfinity(value);

		public Double_T(Double value) {
			this.value = value;
		}

		public Double_T(Single value) {
			this.value = (Double)value;
		}

		public readonly INumber Upcast(INumber number) {
			return TypeTracking.ShouldUpcast(number, this) ? ValueConverter.CastToDouble_T(number) : number;
		}

		public readonly INumber Abs()
			=> new Double_T(Math.Abs(value));

		public readonly IFloat Acos()
			=> new Double_T(Math.Acos(value));

		public readonly IFloat Acosh()
			=> new Double_T(Math.Acosh(value));

		public readonly INumber Add(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Add(this);

			Double_T convert = ValueConverter.CastToDouble_T(number);
			return new Double_T(value + convert.value);
		}

		public readonly IFloat Asin()
			=> new Double_T(Math.Asin(value));

		public readonly IFloat Asinh()
			=> new Double_T(Math.Asinh(value));

		public readonly IFloat Atan()
			=> new Double_T(Math.Atan(value));

		public readonly IFloat Atan2(IFloat divisor) {
			if (TypeTracking.ShouldUpcast(this, divisor))
				return ((IFloat)divisor.Upcast(this)).Atan2(divisor);

			Double_T convert = ValueConverter.CastToDouble_T(divisor);
			return new Double_T(Math .Atan2(value, convert.value));
		}

		public readonly IFloat Atanh()
			=> new Double_T(Math.Asinh(value));

		public readonly INumber Ceiling()
			=> new Double_T(Math.Ceiling(value));

		public readonly void Compare(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number)) {
				number.Upcast(this).Compare(number);
				return;
			}

			INumber sub = this.Subtract(number);

			Registers.F.Negative = sub.IsNegative;
			Registers.F.Zero = sub.IsZero;
		}

		public readonly IFloat Cos()
			=> new Double_T(Math.Cos(value));

		public readonly IFloat Cosh()
			=> new Double_T(Math.Cosh(value));
		
		public readonly INumber Decrement()
			=> new Double_T(value + 1);

		public readonly INumber Divide(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Divide(number);

			Double_T convert = ValueConverter.CastToDouble_T(number);
			return new Double_T(value / convert.value);
		}

		public readonly IFloat Exp()
			=> new Double_T(Math.Exp(value));

		public readonly INumber Floor()
			=> new Double_T(Math.Floor(value));

		public readonly IInteger GetBits()
			=> ValueConverter.RetrieveFloatingPointBits(this);

		public readonly INumber Increment()
			=> new Double_T(value + 1);

		public readonly IFloat Inverse()
			=> (IFloat)One.Divide(this);

		public readonly IFloat Ln()
			=> new Double_T(Math.Log(value));

		public readonly IFloat Log10()
			=> new Double_T(Math.Log10(value));

		public readonly IFloat Log2()
			=> new Double_T(Math.Log2(value));

		public readonly INumber Modulus(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Modulus(number);

			Double_T convert = ValueConverter.CastToDouble_T(number);
			return new Double_T(value % convert.value);
		}

		public readonly INumber Multiply(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			Double_T convert = ValueConverter.CastToDouble_T(number);
			return new Double_T(value * convert.value);
		}

		public readonly INumber Negate()
			=> new Double_T(-value);

		public readonly IFloat Pow(IFloat exponent) {
			if (TypeTracking.ShouldUpcast(this, exponent))
				return ((IFloat)exponent.Upcast(this)).Pow(exponent);

			Double_T convert = ValueConverter.CastToDouble_T(exponent);
			return new Double_T(Math.Pow(value, convert.value));
		}

		public readonly IFloat Root(IFloat root)
			=> Pow(root.Inverse());

		public readonly IFloat Sin()
			=> new Double_T(Math.Sin(value));

		public readonly IFloat Sinh()
			=> new Double_T(Math.Sinh(value));

		public readonly IFloat Sqrt()
			=> new Double_T(Math.Sqrt(value));

		public readonly INumber Subtract(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Subtract(number);

			Double_T convert = ValueConverter.CastToDouble_T(number);
			return new Double_T(value - convert.value);
		}

		public readonly IFloat Tan()
			=> new Double_T(Math.Tan(value));

		public readonly IFloat Tanh()
			=> new Double_T(Math.Tanh(value));
	}
	
	[TextTemplateGenerated]
	public struct Decimal_T : IFloat<Decimal>, IFloatConstants<Decimal_T> {
		private Decimal value;

		public readonly object Value => value;

		public readonly Decimal ActualValue => value;

		public readonly bool IsZero => value == 0;

		public readonly bool IsNegative => value < 0;

		public readonly int NumericSize => sizeof(Decimal);

		public static Decimal_T Zero => new Decimal_T((Decimal)0);

		public static Decimal_T One => new Decimal_T((Decimal)1);

		public static Decimal_T E => new Decimal_T(DecimalMath.DecimalEx.E);

		public readonly bool IsNaN => false;

		public readonly bool IsInfinity => false;

		public Decimal_T(Decimal value) {
			this.value = value;
		}

		public Decimal_T(Single value) {
			this.value = (Decimal)value;
		}

		public readonly INumber Upcast(INumber number) {
			return TypeTracking.ShouldUpcast(number, this) ? ValueConverter.CastToDecimal_T(number) : number;
		}

		public readonly INumber Abs()
			=> new Decimal_T(Math.Abs(value));

		public readonly IFloat Acos()
			=> new Decimal_T(DecimalMath.DecimalEx.ACos(value));

		public readonly IFloat Acosh()
			=> throw new InvalidOperationException("Performing \"acosh\" on decimal values is not supported");

		public readonly INumber Add(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Add(this);

			Decimal_T convert = ValueConverter.CastToDecimal_T(number);
			return new Decimal_T(value + convert.value);
		}

		public readonly IFloat Asin()
			=> new Decimal_T(DecimalMath.DecimalEx.ASin(value));

		public readonly IFloat Asinh()
			=> throw new InvalidOperationException("Performing \"asinh\" on decimal values is not supported");

		public readonly IFloat Atan()
			=> new Decimal_T(DecimalMath.DecimalEx.ATan(value));

		public readonly IFloat Atan2(IFloat divisor) {
			if (TypeTracking.ShouldUpcast(this, divisor))
				return ((IFloat)divisor.Upcast(this)).Atan2(divisor);

			Decimal_T convert = ValueConverter.CastToDecimal_T(divisor);
			return new Decimal_T(DecimalMath.DecimalEx.ATan2(value, convert.value));
		}

		public readonly IFloat Atanh()
			=> throw new InvalidOperationException("Performing \"atanh\" on decimal values is not supported");

		public readonly INumber Ceiling()
			=> new Decimal_T(Math.Ceiling(value));

		public readonly void Compare(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number)) {
				number.Upcast(this).Compare(number);
				return;
			}

			INumber sub = this.Subtract(number);

			Registers.F.Negative = sub.IsNegative;
			Registers.F.Zero = sub.IsZero;
		}

		public readonly IFloat Cos()
			=> new Decimal_T(DecimalMath.DecimalEx.Cos(value));

		public readonly IFloat Cosh()
			=> throw new InvalidOperationException("Performing \"cosh\" on decimal values is not supported");
		
		public readonly INumber Decrement()
			=> new Decimal_T(value + 1);

		public readonly INumber Divide(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Divide(number);

			Decimal_T convert = ValueConverter.CastToDecimal_T(number);
			return new Decimal_T(value / convert.value);
		}

		public readonly IFloat Exp()
			=> new Decimal_T(DecimalMath.DecimalEx.Exp(value));

		public readonly INumber Floor()
			=> new Decimal_T(Math.Floor(value));

		public readonly IInteger GetBits()
			=> throw new InvalidOperationException("Retrieving the bits on an decimal instance is not supported");

		public readonly INumber Increment()
			=> new Decimal_T(value + 1);

		public readonly IFloat Inverse()
			=> (IFloat)One.Divide(this);

		public readonly IFloat Ln()
			=> new Decimal_T(DecimalMath.DecimalEx.Log(value));

		public readonly IFloat Log10()
			=> new Decimal_T(DecimalMath.DecimalEx.Log10(value));

		public readonly IFloat Log2()
			=> new Decimal_T(DecimalMath.DecimalEx.Log2(value));

		public readonly INumber Modulus(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Modulus(number);

			Decimal_T convert = ValueConverter.CastToDecimal_T(number);
			return new Decimal_T(value % convert.value);
		}

		public readonly INumber Multiply(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			Decimal_T convert = ValueConverter.CastToDecimal_T(number);
			return new Decimal_T(value * convert.value);
		}

		public readonly INumber Negate()
			=> new Decimal_T(-value);

		public readonly IFloat Pow(IFloat exponent) {
			if (TypeTracking.ShouldUpcast(this, exponent))
				return ((IFloat)exponent.Upcast(this)).Pow(exponent);

			Decimal_T convert = ValueConverter.CastToDecimal_T(exponent);
			return new Decimal_T(DecimalMath.DecimalEx.Pow(value, convert.value));
		}

		public readonly IFloat Root(IFloat root)
			=> Pow(root.Inverse());

		public readonly IFloat Sin()
			=> new Decimal_T(DecimalMath.DecimalEx.Sin(value));

		public readonly IFloat Sinh()
			=> throw new InvalidOperationException("Performing \"sinh\" on decimal values is not supported");

		public readonly IFloat Sqrt()
			=> new Decimal_T(DecimalMath.DecimalEx.Sqrt(value));

		public readonly INumber Subtract(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Subtract(number);

			Decimal_T convert = ValueConverter.CastToDecimal_T(number);
			return new Decimal_T(value - convert.value);
		}

		public readonly IFloat Tan()
			=> new Decimal_T(DecimalMath.DecimalEx.Tan(value));

		public readonly IFloat Tanh()
			=> throw new InvalidOperationException("Performing \"tanh\" on decimal values is not supported");
	}
	
}