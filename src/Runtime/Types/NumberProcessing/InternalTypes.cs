using Chips.Runtime.Meta;
using Chips.Runtime.Utility;
using System;

#pragma warning disable CS0162
namespace Chips.Runtime.Types.NumberProcessing {
	[TextTemplateGenerated]
	public partial struct SByte_T : IInteger<SByte>, INumberConstants<SByte_T> {
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

		public readonly INumber Cast(INumber number) {
			return ValueConverter.CastToSByte_T(number);
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
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Add(this);

			return new SByte_T(Arithmetic.Add(value, number.ToSByte()));
		}

		public readonly IInteger And(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).And(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.And(this);

			return new SByte_T(value & number.ToSByte());
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
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				upcast.Compare(number);
				return;
			}

			if (TypeTracking.ShouldUpcast(this, number)) {
				number.Upcast(this).Compare(number);
				return;
			}

			Registers.F.Negative = this.CompareLessThan(number);
			Registers.F.Zero = this.CompareEquals(number);
		}

		public readonly bool CompareEquals(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareEquals(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.CompareEquals(this);

			return value == number.ToSByte();
		}

		public readonly bool CompareGreaterThan(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareGreaterThan(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareGreaterThan(number);

			return value > number.ToSByte();
		}

		public readonly bool CompareLessThan(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareLessThan(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareLessThan(number);

			return value < number.ToSByte();
		}

		public readonly INumber Decrement()
			=> new SByte_T(unchecked(value - 1));

		public readonly INumber Divide(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Divide(number);

			return new SByte_T(value / number.ToSByte());
		}

		public readonly INumber Increment()
			=> new SByte_T(unchecked(value + 1));

		public readonly INumber Modulus(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Modulus(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Modulus(number);

			return new SByte_T(value % number.ToSByte());
		}

		public readonly INumber Multiply(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			return new SByte_T(unchecked(value * number.ToSByte()));
		}

		public readonly INumber Negate() {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Negate();
			}

			return new SByte_T(-value);
		}

		public readonly IInteger Not() {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Not();
			}

			return new SByte_T(~value);
		}

		public readonly IInteger Or(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Or(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Or(this);

			return new SByte_T(value | number.ToSByte());
		}

		public readonly INumber Repeat(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Repeat(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Repeat(number);

			return new SByte_T(Arithmetic.Repeat(value, number.ToSByte()));
		}

		public readonly INumber Subtract(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Subtract(number);

			return new SByte_T(Arithmetic.Subtract(value, number.ToSByte()));
		}

		public readonly SByte ToSByte() => (SByte)value;

		public readonly Int16 ToInt16() => (Int16)value;

		public readonly Int32 ToInt32() => (Int32)value;

		public readonly Int64 ToInt64() => (Int64)value;

		public readonly Byte ToByte() => (Byte)value;

		public readonly UInt16 ToUInt16() => (UInt16)value;

		public readonly UInt32 ToUInt32() => (UInt32)value;

		public readonly UInt64 ToUInt64() => (UInt64)value;

		public readonly IntPtr ToIntPtr() => (IntPtr)value;

		public readonly UIntPtr ToUIntPtr() => (UIntPtr)value;

		public readonly Single ToSingle() => (Single)value;

		public readonly Double ToDouble() => (Double)value;

		public readonly Decimal ToDecimal() => (Decimal)value;

		public readonly IInteger Xor(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(SByte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Xor(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Xor(this);

			return new SByte_T(value ^ number.ToSByte());
		}
	}

	[TextTemplateGenerated]
	public partial struct Int16_T : IInteger<Int16>, INumberConstants<Int16_T> {
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

		public readonly INumber Cast(INumber number) {
			return ValueConverter.CastToInt16_T(number);
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
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Add(this);

			return new Int16_T(Arithmetic.Add(value, number.ToInt16()));
		}

		public readonly IInteger And(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).And(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.And(this);

			return new Int16_T(value & number.ToInt16());
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
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				upcast.Compare(number);
				return;
			}

			if (TypeTracking.ShouldUpcast(this, number)) {
				number.Upcast(this).Compare(number);
				return;
			}

			Registers.F.Negative = this.CompareLessThan(number);
			Registers.F.Zero = this.CompareEquals(number);
		}

		public readonly bool CompareEquals(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareEquals(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.CompareEquals(this);

			return value == number.ToInt16();
		}

		public readonly bool CompareGreaterThan(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareGreaterThan(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareGreaterThan(number);

			return value > number.ToInt16();
		}

		public readonly bool CompareLessThan(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareLessThan(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareLessThan(number);

			return value < number.ToInt16();
		}

		public readonly INumber Decrement()
			=> new Int16_T(unchecked(value - 1));

		public readonly INumber Divide(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Divide(number);

			return new Int16_T(value / number.ToInt16());
		}

		public readonly INumber Increment()
			=> new Int16_T(unchecked(value + 1));

		public readonly INumber Modulus(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Modulus(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Modulus(number);

			return new Int16_T(value % number.ToInt16());
		}

		public readonly INumber Multiply(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			return new Int16_T(unchecked(value * number.ToInt16()));
		}

		public readonly INumber Negate() {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Negate();
			}

			return new Int16_T(-value);
		}

		public readonly IInteger Not() {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Not();
			}

			return new Int16_T(~value);
		}

		public readonly IInteger Or(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Or(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Or(this);

			return new Int16_T(value | number.ToInt16());
		}

		public readonly INumber Repeat(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Repeat(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Repeat(number);

			return new Int16_T(Arithmetic.Repeat(value, number.ToInt16()));
		}

		public readonly INumber Subtract(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Subtract(number);

			return new Int16_T(Arithmetic.Subtract(value, number.ToInt16()));
		}

		public readonly SByte ToSByte() => (SByte)value;

		public readonly Int16 ToInt16() => (Int16)value;

		public readonly Int32 ToInt32() => (Int32)value;

		public readonly Int64 ToInt64() => (Int64)value;

		public readonly Byte ToByte() => (Byte)value;

		public readonly UInt16 ToUInt16() => (UInt16)value;

		public readonly UInt32 ToUInt32() => (UInt32)value;

		public readonly UInt64 ToUInt64() => (UInt64)value;

		public readonly IntPtr ToIntPtr() => (IntPtr)value;

		public readonly UIntPtr ToUIntPtr() => (UIntPtr)value;

		public readonly Single ToSingle() => (Single)value;

		public readonly Double ToDouble() => (Double)value;

		public readonly Decimal ToDecimal() => (Decimal)value;

		public readonly IInteger Xor(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Xor(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Xor(this);

			return new Int16_T(value ^ number.ToInt16());
		}
	}

	[TextTemplateGenerated]
	public partial struct Int32_T : IInteger<Int32>, INumberConstants<Int32_T> {
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

		public readonly INumber Cast(INumber number) {
			return ValueConverter.CastToInt32_T(number);
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
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Add(this);

			return new Int32_T(Arithmetic.Add(value, number.ToInt32()));
		}

		public readonly IInteger And(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).And(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.And(this);

			return new Int32_T(value & number.ToInt32());
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
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				upcast.Compare(number);
				return;
			}

			if (TypeTracking.ShouldUpcast(this, number)) {
				number.Upcast(this).Compare(number);
				return;
			}

			Registers.F.Negative = this.CompareLessThan(number);
			Registers.F.Zero = this.CompareEquals(number);
		}

		public readonly bool CompareEquals(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareEquals(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.CompareEquals(this);

			return value == number.ToInt32();
		}

		public readonly bool CompareGreaterThan(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareGreaterThan(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareGreaterThan(number);

			return value > number.ToInt32();
		}

		public readonly bool CompareLessThan(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareLessThan(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareLessThan(number);

			return value < number.ToInt32();
		}

		public readonly INumber Decrement()
			=> new Int32_T(unchecked(value - 1));

		public readonly INumber Divide(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Divide(number);

			return new Int32_T(value / number.ToInt32());
		}

		public readonly INumber Increment()
			=> new Int32_T(unchecked(value + 1));

		public readonly INumber Modulus(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Modulus(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Modulus(number);

			return new Int32_T(value % number.ToInt32());
		}

		public readonly INumber Multiply(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			return new Int32_T(unchecked(value * number.ToInt32()));
		}

		public readonly INumber Negate() {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Negate();
			}

			return new Int32_T(-value);
		}

		public readonly IInteger Not() {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Not();
			}

			return new Int32_T(~value);
		}

		public readonly IInteger Or(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Or(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Or(this);

			return new Int32_T(value | number.ToInt32());
		}

		public readonly INumber Repeat(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Repeat(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Repeat(number);

			return new Int32_T(Arithmetic.Repeat(value, number.ToInt32()));
		}

		public readonly INumber Subtract(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Subtract(number);

			return new Int32_T(Arithmetic.Subtract(value, number.ToInt32()));
		}

		public readonly SByte ToSByte() => (SByte)value;

		public readonly Int16 ToInt16() => (Int16)value;

		public readonly Int32 ToInt32() => (Int32)value;

		public readonly Int64 ToInt64() => (Int64)value;

		public readonly Byte ToByte() => (Byte)value;

		public readonly UInt16 ToUInt16() => (UInt16)value;

		public readonly UInt32 ToUInt32() => (UInt32)value;

		public readonly UInt64 ToUInt64() => (UInt64)value;

		public readonly IntPtr ToIntPtr() => (IntPtr)value;

		public readonly UIntPtr ToUIntPtr() => (UIntPtr)value;

		public readonly Single ToSingle() => (Single)value;

		public readonly Double ToDouble() => (Double)value;

		public readonly Decimal ToDecimal() => (Decimal)value;

		public readonly IInteger Xor(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Xor(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Xor(this);

			return new Int32_T(value ^ number.ToInt32());
		}
	}

	[TextTemplateGenerated]
	public partial struct Int64_T : IInteger<Int64>, INumberConstants<Int64_T> {
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

		public readonly INumber Cast(INumber number) {
			return ValueConverter.CastToInt64_T(number);
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
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Add(this);

			return new Int64_T(Arithmetic.Add(value, number.ToInt64()));
		}

		public readonly IInteger And(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).And(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.And(this);

			return new Int64_T(value & number.ToInt64());
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
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				upcast.Compare(number);
				return;
			}

			if (TypeTracking.ShouldUpcast(this, number)) {
				number.Upcast(this).Compare(number);
				return;
			}

			Registers.F.Negative = this.CompareLessThan(number);
			Registers.F.Zero = this.CompareEquals(number);
		}

		public readonly bool CompareEquals(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareEquals(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.CompareEquals(this);

			return value == number.ToInt64();
		}

		public readonly bool CompareGreaterThan(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareGreaterThan(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareGreaterThan(number);

			return value > number.ToInt64();
		}

		public readonly bool CompareLessThan(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareLessThan(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareLessThan(number);

			return value < number.ToInt64();
		}

		public readonly INumber Decrement()
			=> new Int64_T(unchecked(value - 1));

		public readonly INumber Divide(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Divide(number);

			return new Int64_T(value / number.ToInt64());
		}

		public readonly INumber Increment()
			=> new Int64_T(unchecked(value + 1));

		public readonly INumber Modulus(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Modulus(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Modulus(number);

			return new Int64_T(value % number.ToInt64());
		}

		public readonly INumber Multiply(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			return new Int64_T(unchecked(value * number.ToInt64()));
		}

		public readonly INumber Negate() {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Negate();
			}

			return new Int64_T(-value);
		}

		public readonly IInteger Not() {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Not();
			}

			return new Int64_T(~value);
		}

		public readonly IInteger Or(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Or(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Or(this);

			return new Int64_T(value | number.ToInt64());
		}

		public readonly INumber Repeat(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Repeat(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Repeat(number);

			return new Int64_T(Arithmetic.Repeat(value, number.ToInt64()));
		}

		public readonly INumber Subtract(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Subtract(number);

			return new Int64_T(Arithmetic.Subtract(value, number.ToInt64()));
		}

		public readonly SByte ToSByte() => (SByte)value;

		public readonly Int16 ToInt16() => (Int16)value;

		public readonly Int32 ToInt32() => (Int32)value;

		public readonly Int64 ToInt64() => (Int64)value;

		public readonly Byte ToByte() => (Byte)value;

		public readonly UInt16 ToUInt16() => (UInt16)value;

		public readonly UInt32 ToUInt32() => (UInt32)value;

		public readonly UInt64 ToUInt64() => (UInt64)value;

		public readonly IntPtr ToIntPtr() => (IntPtr)value;

		public readonly UIntPtr ToUIntPtr() => (UIntPtr)value;

		public readonly Single ToSingle() => (Single)value;

		public readonly Double ToDouble() => (Double)value;

		public readonly Decimal ToDecimal() => (Decimal)value;

		public readonly IInteger Xor(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Int64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Xor(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Xor(this);

			return new Int64_T(value ^ number.ToInt64());
		}
	}

	[TextTemplateGenerated]
	public partial struct Byte_T : IUnsignedInteger<Byte>, INumberConstants<Byte_T> {
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

		public readonly INumber Cast(INumber number) {
			return ValueConverter.CastToByte_T(number);
		}

		public readonly INumber Upcast(INumber number) {
			return TypeTracking.ShouldUpcast(number, this) ? ValueConverter.CastToByte_T(number) : number;
		}

		public readonly INumber Abs()
			=> new Byte_T(value);

		public readonly INumber Add(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Add(this);

			return new Byte_T(Arithmetic.Add(value, number.ToByte()));
		}

		public readonly IInteger And(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).And(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.And(this);

			return new Byte_T(value & number.ToByte());
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
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				upcast.Compare(number);
				return;
			}

			if (TypeTracking.ShouldUpcast(this, number)) {
				number.Upcast(this).Compare(number);
				return;
			}

			Registers.F.Negative = this.CompareLessThan(number);
			Registers.F.Zero = this.CompareEquals(number);
		}

		public readonly bool CompareEquals(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareEquals(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.CompareEquals(this);

			return value == number.ToByte();
		}

		public readonly bool CompareGreaterThan(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareGreaterThan(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareGreaterThan(number);

			return value > number.ToByte();
		}

		public readonly bool CompareLessThan(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareLessThan(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareLessThan(number);

			return value < number.ToByte();
		}

		public readonly INumber Decrement()
			=> new Byte_T(unchecked(value - 1));

		public readonly INumber Divide(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Divide(number);

			return new Byte_T(value / number.ToByte());
		}

		public readonly INumber Increment()
			=> new Byte_T(unchecked(value + 1));

		public readonly INumber Modulus(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Modulus(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Modulus(number);

			return new Byte_T(value % number.ToByte());
		}

		public readonly INumber Multiply(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			return new Byte_T(unchecked(value * number.ToByte()));
		}

		public readonly INumber Negate() {
			throw new InvalidOperationException("Negation cannot be performed on unsigned integers");
		}

		public readonly IInteger Not() {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Not();
			}

			return new Byte_T(~value);
		}

		public readonly IInteger Or(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Or(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Or(this);

			return new Byte_T(value | number.ToByte());
		}

		public readonly INumber Repeat(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Repeat(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Repeat(number);

			return new Byte_T(Arithmetic.Repeat(value, number.ToByte()));
		}

		public readonly INumber Subtract(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Subtract(number);

			return new Byte_T(Arithmetic.Subtract(value, number.ToByte()));
		}

		public readonly SByte ToSByte() => (SByte)value;

		public readonly Int16 ToInt16() => (Int16)value;

		public readonly Int32 ToInt32() => (Int32)value;

		public readonly Int64 ToInt64() => (Int64)value;

		public readonly Byte ToByte() => (Byte)value;

		public readonly UInt16 ToUInt16() => (UInt16)value;

		public readonly UInt32 ToUInt32() => (UInt32)value;

		public readonly UInt64 ToUInt64() => (UInt64)value;

		public readonly IntPtr ToIntPtr() => (IntPtr)value;

		public readonly UIntPtr ToUIntPtr() => (UIntPtr)value;

		public readonly Single ToSingle() => (Single)value;

		public readonly Double ToDouble() => (Double)value;

		public readonly Decimal ToDecimal() => (Decimal)value;

		public readonly IInteger Xor(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(Byte) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Xor(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Xor(this);

			return new Byte_T(value ^ number.ToByte());
		}
	}

	[TextTemplateGenerated]
	public partial struct UInt16_T : IUnsignedInteger<UInt16>, INumberConstants<UInt16_T> {
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

		public readonly INumber Cast(INumber number) {
			return ValueConverter.CastToUInt16_T(number);
		}

		public readonly INumber Upcast(INumber number) {
			return TypeTracking.ShouldUpcast(number, this) ? ValueConverter.CastToUInt16_T(number) : number;
		}

		public readonly INumber Abs()
			=> new UInt16_T(value);

		public readonly INumber Add(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Add(this);

			return new UInt16_T(Arithmetic.Add(value, number.ToUInt16()));
		}

		public readonly IInteger And(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).And(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.And(this);

			return new UInt16_T(value & number.ToUInt16());
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
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				upcast.Compare(number);
				return;
			}

			if (TypeTracking.ShouldUpcast(this, number)) {
				number.Upcast(this).Compare(number);
				return;
			}

			Registers.F.Negative = this.CompareLessThan(number);
			Registers.F.Zero = this.CompareEquals(number);
		}

		public readonly bool CompareEquals(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareEquals(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.CompareEquals(this);

			return value == number.ToUInt16();
		}

		public readonly bool CompareGreaterThan(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareGreaterThan(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareGreaterThan(number);

			return value > number.ToUInt16();
		}

		public readonly bool CompareLessThan(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareLessThan(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareLessThan(number);

			return value < number.ToUInt16();
		}

		public readonly INumber Decrement()
			=> new UInt16_T(unchecked(value - 1));

		public readonly INumber Divide(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Divide(number);

			return new UInt16_T(value / number.ToUInt16());
		}

		public readonly INumber Increment()
			=> new UInt16_T(unchecked(value + 1));

		public readonly INumber Modulus(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Modulus(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Modulus(number);

			return new UInt16_T(value % number.ToUInt16());
		}

		public readonly INumber Multiply(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			return new UInt16_T(unchecked(value * number.ToUInt16()));
		}

		public readonly INumber Negate() {
			throw new InvalidOperationException("Negation cannot be performed on unsigned integers");
		}

		public readonly IInteger Not() {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Not();
			}

			return new UInt16_T(~value);
		}

		public readonly IInteger Or(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Or(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Or(this);

			return new UInt16_T(value | number.ToUInt16());
		}

		public readonly INumber Repeat(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Repeat(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Repeat(number);

			return new UInt16_T(Arithmetic.Repeat(value, number.ToUInt16()));
		}

		public readonly INumber Subtract(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Subtract(number);

			return new UInt16_T(Arithmetic.Subtract(value, number.ToUInt16()));
		}

		public readonly SByte ToSByte() => (SByte)value;

		public readonly Int16 ToInt16() => (Int16)value;

		public readonly Int32 ToInt32() => (Int32)value;

		public readonly Int64 ToInt64() => (Int64)value;

		public readonly Byte ToByte() => (Byte)value;

		public readonly UInt16 ToUInt16() => (UInt16)value;

		public readonly UInt32 ToUInt32() => (UInt32)value;

		public readonly UInt64 ToUInt64() => (UInt64)value;

		public readonly IntPtr ToIntPtr() => (IntPtr)value;

		public readonly UIntPtr ToUIntPtr() => (UIntPtr)value;

		public readonly Single ToSingle() => (Single)value;

		public readonly Double ToDouble() => (Double)value;

		public readonly Decimal ToDecimal() => (Decimal)value;

		public readonly IInteger Xor(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt16) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Xor(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Xor(this);

			return new UInt16_T(value ^ number.ToUInt16());
		}
	}

	[TextTemplateGenerated]
	public partial struct UInt32_T : IUnsignedInteger<UInt32>, INumberConstants<UInt32_T> {
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

		public readonly INumber Cast(INumber number) {
			return ValueConverter.CastToUInt32_T(number);
		}

		public readonly INumber Upcast(INumber number) {
			return TypeTracking.ShouldUpcast(number, this) ? ValueConverter.CastToUInt32_T(number) : number;
		}

		public readonly INumber Abs()
			=> new UInt32_T(value);

		public readonly INumber Add(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Add(this);

			return new UInt32_T(Arithmetic.Add(value, number.ToUInt32()));
		}

		public readonly IInteger And(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).And(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.And(this);

			return new UInt32_T(value & number.ToUInt32());
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
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				upcast.Compare(number);
				return;
			}

			if (TypeTracking.ShouldUpcast(this, number)) {
				number.Upcast(this).Compare(number);
				return;
			}

			Registers.F.Negative = this.CompareLessThan(number);
			Registers.F.Zero = this.CompareEquals(number);
		}

		public readonly bool CompareEquals(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareEquals(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.CompareEquals(this);

			return value == number.ToUInt32();
		}

		public readonly bool CompareGreaterThan(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareGreaterThan(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareGreaterThan(number);

			return value > number.ToUInt32();
		}

		public readonly bool CompareLessThan(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareLessThan(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareLessThan(number);

			return value < number.ToUInt32();
		}

		public readonly INumber Decrement()
			=> new UInt32_T(unchecked(value - 1));

		public readonly INumber Divide(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Divide(number);

			return new UInt32_T(value / number.ToUInt32());
		}

		public readonly INumber Increment()
			=> new UInt32_T(unchecked(value + 1));

		public readonly INumber Modulus(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Modulus(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Modulus(number);

			return new UInt32_T(value % number.ToUInt32());
		}

		public readonly INumber Multiply(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			return new UInt32_T(unchecked(value * number.ToUInt32()));
		}

		public readonly INumber Negate() {
			throw new InvalidOperationException("Negation cannot be performed on unsigned integers");
		}

		public readonly IInteger Not() {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Not();
			}

			return new UInt32_T(~value);
		}

		public readonly IInteger Or(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Or(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Or(this);

			return new UInt32_T(value | number.ToUInt32());
		}

		public readonly INumber Repeat(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Repeat(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Repeat(number);

			return new UInt32_T(Arithmetic.Repeat(value, number.ToUInt32()));
		}

		public readonly INumber Subtract(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Subtract(number);

			return new UInt32_T(Arithmetic.Subtract(value, number.ToUInt32()));
		}

		public readonly SByte ToSByte() => (SByte)value;

		public readonly Int16 ToInt16() => (Int16)value;

		public readonly Int32 ToInt32() => (Int32)value;

		public readonly Int64 ToInt64() => (Int64)value;

		public readonly Byte ToByte() => (Byte)value;

		public readonly UInt16 ToUInt16() => (UInt16)value;

		public readonly UInt32 ToUInt32() => (UInt32)value;

		public readonly UInt64 ToUInt64() => (UInt64)value;

		public readonly IntPtr ToIntPtr() => (IntPtr)value;

		public readonly UIntPtr ToUIntPtr() => (UIntPtr)value;

		public readonly Single ToSingle() => (Single)value;

		public readonly Double ToDouble() => (Double)value;

		public readonly Decimal ToDecimal() => (Decimal)value;

		public readonly IInteger Xor(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt32) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Xor(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Xor(this);

			return new UInt32_T(value ^ number.ToUInt32());
		}
	}

	[TextTemplateGenerated]
	public partial struct UInt64_T : IUnsignedInteger<UInt64>, INumberConstants<UInt64_T> {
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

		public readonly INumber Cast(INumber number) {
			return ValueConverter.CastToUInt64_T(number);
		}

		public readonly INumber Upcast(INumber number) {
			return TypeTracking.ShouldUpcast(number, this) ? ValueConverter.CastToUInt64_T(number) : number;
		}

		public readonly INumber Abs()
			=> new UInt64_T(value);

		public readonly INumber Add(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Add(this);

			return new UInt64_T(Arithmetic.Add(value, number.ToUInt64()));
		}

		public readonly IInteger And(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).And(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.And(this);

			return new UInt64_T(value & number.ToUInt64());
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
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				upcast.Compare(number);
				return;
			}

			if (TypeTracking.ShouldUpcast(this, number)) {
				number.Upcast(this).Compare(number);
				return;
			}

			Registers.F.Negative = this.CompareLessThan(number);
			Registers.F.Zero = this.CompareEquals(number);
		}

		public readonly bool CompareEquals(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareEquals(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.CompareEquals(this);

			return value == number.ToUInt64();
		}

		public readonly bool CompareGreaterThan(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareGreaterThan(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareGreaterThan(number);

			return value > number.ToUInt64();
		}

		public readonly bool CompareLessThan(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareLessThan(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareLessThan(number);

			return value < number.ToUInt64();
		}

		public readonly INumber Decrement()
			=> new UInt64_T(unchecked(value - 1));

		public readonly INumber Divide(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Divide(number);

			return new UInt64_T(value / number.ToUInt64());
		}

		public readonly INumber Increment()
			=> new UInt64_T(unchecked(value + 1));

		public readonly INumber Modulus(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Modulus(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Modulus(number);

			return new UInt64_T(value % number.ToUInt64());
		}

		public readonly INumber Multiply(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			return new UInt64_T(unchecked(value * number.ToUInt64()));
		}

		public readonly INumber Negate() {
			throw new InvalidOperationException("Negation cannot be performed on unsigned integers");
		}

		public readonly IInteger Not() {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Not();
			}

			return new UInt64_T(~value);
		}

		public readonly IInteger Or(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Or(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Or(this);

			return new UInt64_T(value | number.ToUInt64());
		}

		public readonly INumber Repeat(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Repeat(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Repeat(number);

			return new UInt64_T(Arithmetic.Repeat(value, number.ToUInt64()));
		}

		public readonly INumber Subtract(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Subtract(number);

			return new UInt64_T(Arithmetic.Subtract(value, number.ToUInt64()));
		}

		public readonly SByte ToSByte() => (SByte)value;

		public readonly Int16 ToInt16() => (Int16)value;

		public readonly Int32 ToInt32() => (Int32)value;

		public readonly Int64 ToInt64() => (Int64)value;

		public readonly Byte ToByte() => (Byte)value;

		public readonly UInt16 ToUInt16() => (UInt16)value;

		public readonly UInt32 ToUInt32() => (UInt32)value;

		public readonly UInt64 ToUInt64() => (UInt64)value;

		public readonly IntPtr ToIntPtr() => (IntPtr)value;

		public readonly UIntPtr ToUIntPtr() => (UIntPtr)value;

		public readonly Single ToSingle() => (Single)value;

		public readonly Double ToDouble() => (Double)value;

		public readonly Decimal ToDecimal() => (Decimal)value;

		public readonly IInteger Xor(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UInt64) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Xor(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Xor(this);

			return new UInt64_T(value ^ number.ToUInt64());
		}
	}

	[TextTemplateGenerated]
	public unsafe partial struct IntPtr_T : IInteger<IntPtr>, INumberConstants<IntPtr_T> {
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

		public readonly INumber Cast(INumber number) {
			return ValueConverter.CastToIntPtr_T(number);
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
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Add(this);

			return new IntPtr_T(Arithmetic.Add(value, number.ToIntPtr()));
		}

		public readonly IInteger And(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).And(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.And(this);

			return new IntPtr_T(value & number.ToIntPtr());
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
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				upcast.Compare(number);
				return;
			}

			if (TypeTracking.ShouldUpcast(this, number)) {
				number.Upcast(this).Compare(number);
				return;
			}

			Registers.F.Negative = this.CompareLessThan(number);
			Registers.F.Zero = this.CompareEquals(number);
		}

		public readonly bool CompareEquals(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareEquals(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.CompareEquals(this);

			return value == number.ToIntPtr();
		}

		public readonly bool CompareGreaterThan(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareGreaterThan(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareGreaterThan(number);

			return value > number.ToIntPtr();
		}

		public readonly bool CompareLessThan(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareLessThan(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareLessThan(number);

			return value < number.ToIntPtr();
		}

		public readonly INumber Decrement()
			=> new IntPtr_T(unchecked(value - 1));

		public readonly INumber Divide(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Divide(number);

			return new IntPtr_T(value / number.ToIntPtr());
		}

		public readonly INumber Increment()
			=> new IntPtr_T(unchecked(value + 1));

		public readonly INumber Modulus(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Modulus(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Modulus(number);

			return new IntPtr_T(value % number.ToIntPtr());
		}

		public readonly INumber Multiply(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			return new IntPtr_T(unchecked(value * number.ToIntPtr()));
		}

		public readonly INumber Negate() {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Negate();
			}

			return new IntPtr_T(-value);
		}

		public readonly IInteger Not() {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Not();
			}

			return new IntPtr_T(~value);
		}

		public readonly IInteger Or(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Or(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Or(this);

			return new IntPtr_T(value | number.ToIntPtr());
		}

		public readonly INumber Repeat(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Repeat(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Repeat(number);

			return new IntPtr_T(Arithmetic.Repeat(value, number.ToIntPtr()));
		}

		public readonly INumber Subtract(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Subtract(number);

			return new IntPtr_T(Arithmetic.Subtract(value, number.ToIntPtr()));
		}

		public readonly SByte ToSByte() => (SByte)value;

		public readonly Int16 ToInt16() => (Int16)value;

		public readonly Int32 ToInt32() => (Int32)value;

		public readonly Int64 ToInt64() => (Int64)value;

		public readonly Byte ToByte() => (Byte)value;

		public readonly UInt16 ToUInt16() => (UInt16)value;

		public readonly UInt32 ToUInt32() => (UInt32)value;

		public readonly UInt64 ToUInt64() => (UInt64)value;

		public readonly IntPtr ToIntPtr() => (IntPtr)value;

		public readonly UIntPtr ToUIntPtr() => (UIntPtr)value;

		public readonly Single ToSingle() => (Single)value;

		public readonly Double ToDouble() => (Double)value;

		public readonly Decimal ToDecimal() => (Decimal)value;

		public readonly IInteger Xor(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(IntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Xor(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Xor(this);

			return new IntPtr_T(value ^ number.ToIntPtr());
		}
	}

	[TextTemplateGenerated]
	public unsafe partial struct UIntPtr_T : IUnsignedInteger<UIntPtr>, INumberConstants<UIntPtr_T> {
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

		public readonly INumber Cast(INumber number) {
			return ValueConverter.CastToUIntPtr_T(number);
		}

		public readonly INumber Upcast(INumber number) {
			return TypeTracking.ShouldUpcast(number, this) ? ValueConverter.CastToUIntPtr_T(number) : number;
		}

		public readonly INumber Abs()
			=> new UIntPtr_T(value);

		public readonly INumber Add(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UIntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Add(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Add(this);

			return new UIntPtr_T(Arithmetic.Add(value, number.ToUIntPtr()));
		}

		public readonly IInteger And(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UIntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).And(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.And(this);

			return new UIntPtr_T(value & number.ToUIntPtr());
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
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UIntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				upcast.Compare(number);
				return;
			}

			if (TypeTracking.ShouldUpcast(this, number)) {
				number.Upcast(this).Compare(number);
				return;
			}

			Registers.F.Negative = this.CompareLessThan(number);
			Registers.F.Zero = this.CompareEquals(number);
		}

		public readonly bool CompareEquals(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UIntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareEquals(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.CompareEquals(this);

			return value == number.ToUIntPtr();
		}

		public readonly bool CompareGreaterThan(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UIntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareGreaterThan(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareGreaterThan(number);

			return value > number.ToUIntPtr();
		}

		public readonly bool CompareLessThan(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UIntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.CompareLessThan(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareLessThan(number);

			return value < number.ToUIntPtr();
		}

		public readonly INumber Decrement()
			=> new UIntPtr_T(unchecked(value - 1));

		public readonly INumber Divide(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UIntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Divide(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Divide(number);

			return new UIntPtr_T(value / number.ToUIntPtr());
		}

		public readonly INumber Increment()
			=> new UIntPtr_T(unchecked(value + 1));

		public readonly INumber Modulus(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UIntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Modulus(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Modulus(number);

			return new UIntPtr_T(value % number.ToUIntPtr());
		}

		public readonly INumber Multiply(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UIntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Multiply(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			return new UIntPtr_T(unchecked(value * number.ToUIntPtr()));
		}

		public readonly INumber Negate() {
			throw new InvalidOperationException("Negation cannot be performed on unsigned integers");
		}

		public readonly IInteger Not() {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UIntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Not();
			}

			return new UIntPtr_T(~value);
		}

		public readonly IInteger Or(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UIntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Or(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Or(this);

			return new UIntPtr_T(value | number.ToUIntPtr());
		}

		public readonly INumber Repeat(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UIntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Repeat(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Repeat(number);

			return new UIntPtr_T(Arithmetic.Repeat(value, number.ToUIntPtr()));
		}

		public readonly INumber Subtract(INumber number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UIntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return upcast.Subtract(number);
			}

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Subtract(number);

			return new UIntPtr_T(Arithmetic.Subtract(value, number.ToUIntPtr()));
		}

		public readonly SByte ToSByte() => (SByte)value;

		public readonly Int16 ToInt16() => (Int16)value;

		public readonly Int32 ToInt32() => (Int32)value;

		public readonly Int64 ToInt64() => (Int64)value;

		public readonly Byte ToByte() => (Byte)value;

		public readonly UInt16 ToUInt16() => (UInt16)value;

		public readonly UInt32 ToUInt32() => (UInt32)value;

		public readonly UInt64 ToUInt64() => (UInt64)value;

		public readonly IntPtr ToIntPtr() => (IntPtr)value;

		public readonly UIntPtr ToUIntPtr() => (UIntPtr)value;

		public readonly Single ToSingle() => (Single)value;

		public readonly Double ToDouble() => (Double)value;

		public readonly Decimal ToDecimal() => (Decimal)value;

		public readonly IInteger Xor(IInteger number) {
			// For sizes larger than int, this block should be removed by the compiler
			if (sizeof(UIntPtr) < sizeof(int)) {
				INumber upcast = ValueConverter.UpcastToAtLeastInt32(this);
				return ((IInteger)upcast!).Xor(number);
			}

			if (number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			if (TypeTracking.ShouldUpcast(this, number))
				return number.Xor(this);

			return new UIntPtr_T(value ^ number.ToUIntPtr());
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

		public readonly INumber Cast(INumber number) {
			return ValueConverter.CastToSingle_T(number);
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

			return new Single_T(Arithmetic.Add(value, number.ToSingle()));
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

			return new Single_T(MathF.Atan2(value, divisor.ToSingle()));
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

			Registers.F.Negative = this.CompareLessThan(number);
			Registers.F.Zero = this.CompareEquals(number);
		}

		public readonly bool CompareEquals(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.CompareEquals(this);

			return value == number.ToSingle();
		}

		public readonly bool CompareGreaterThan(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareGreaterThan(number);

			return value > number.ToSingle();
		}

		public readonly bool CompareLessThan(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareLessThan(number);

			return value < number.ToSingle();
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

			return new Single_T(value / number.ToSingle());
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

			return new Single_T(value % number.ToSingle());
		}

		public readonly INumber Multiply(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			return new Single_T(value * number.ToSingle());
		}

		public readonly INumber Negate()
			=> new Single_T(-value);

		public readonly IFloat Pow(IFloat exponent) {
			if (TypeTracking.ShouldUpcast(this, exponent))
				return ((IFloat)exponent.Upcast(this)).Pow(exponent);

			return new Single_T(MathF.Pow(value, exponent.ToSingle()));
		}

		public readonly INumber Repeat(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Repeat(number);

			return new Single_T(Arithmetic.Repeat(value, number.ToSingle()));
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

			return new Single_T(Arithmetic.Subtract(value, number.ToSingle()));
		}

		public readonly IFloat Tan()
			=> new Single_T(MathF.Tan(value));

		public readonly IFloat Tanh()
			=> new Single_T(MathF.Tanh(value));

		public readonly SByte ToSByte() => (SByte)value;

		public readonly Int16 ToInt16() => (Int16)value;

		public readonly Int32 ToInt32() => (Int32)value;

		public readonly Int64 ToInt64() => (Int64)value;

		public readonly Byte ToByte() => (Byte)value;

		public readonly UInt16 ToUInt16() => (UInt16)value;

		public readonly UInt32 ToUInt32() => (UInt32)value;

		public readonly UInt64 ToUInt64() => (UInt64)value;

		public readonly IntPtr ToIntPtr() => (IntPtr)value;

		public readonly UIntPtr ToUIntPtr() => (UIntPtr)value;

		public readonly Single ToSingle() => (Single)value;

		public readonly Double ToDouble() => (Double)value;

		public readonly Decimal ToDecimal() => (Decimal)value;

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

		public readonly INumber Cast(INumber number) {
			return ValueConverter.CastToDouble_T(number);
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

			return new Double_T(Arithmetic.Add(value, number.ToDouble()));
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

			return new Double_T(Math .Atan2(value, divisor.ToDouble()));
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

			Registers.F.Negative = this.CompareLessThan(number);
			Registers.F.Zero = this.CompareEquals(number);
		}

		public readonly bool CompareEquals(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.CompareEquals(this);

			return value == number.ToDouble();
		}

		public readonly bool CompareGreaterThan(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareGreaterThan(number);

			return value > number.ToDouble();
		}

		public readonly bool CompareLessThan(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareLessThan(number);

			return value < number.ToDouble();
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

			return new Double_T(value / number.ToDouble());
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

			return new Double_T(value % number.ToDouble());
		}

		public readonly INumber Multiply(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			return new Double_T(value * number.ToDouble());
		}

		public readonly INumber Negate()
			=> new Double_T(-value);

		public readonly IFloat Pow(IFloat exponent) {
			if (TypeTracking.ShouldUpcast(this, exponent))
				return ((IFloat)exponent.Upcast(this)).Pow(exponent);

			return new Double_T(Math.Pow(value, exponent.ToDouble()));
		}

		public readonly INumber Repeat(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Repeat(number);

			return new Double_T(Arithmetic.Repeat(value, number.ToDouble()));
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

			return new Double_T(Arithmetic.Subtract(value, number.ToDouble()));
		}

		public readonly IFloat Tan()
			=> new Double_T(Math.Tan(value));

		public readonly IFloat Tanh()
			=> new Double_T(Math.Tanh(value));

		public readonly SByte ToSByte() => (SByte)value;

		public readonly Int16 ToInt16() => (Int16)value;

		public readonly Int32 ToInt32() => (Int32)value;

		public readonly Int64 ToInt64() => (Int64)value;

		public readonly Byte ToByte() => (Byte)value;

		public readonly UInt16 ToUInt16() => (UInt16)value;

		public readonly UInt32 ToUInt32() => (UInt32)value;

		public readonly UInt64 ToUInt64() => (UInt64)value;

		public readonly IntPtr ToIntPtr() => (IntPtr)value;

		public readonly UIntPtr ToUIntPtr() => (UIntPtr)value;

		public readonly Single ToSingle() => (Single)value;

		public readonly Double ToDouble() => (Double)value;

		public readonly Decimal ToDecimal() => (Decimal)value;

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

		public readonly INumber Cast(INumber number) {
			return ValueConverter.CastToDecimal_T(number);
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

			return new Decimal_T(Arithmetic.Add(value, number.ToDecimal()));
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

			return new Decimal_T(DecimalMath.DecimalEx.ATan2(value, divisor.ToDecimal()));
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

			Registers.F.Negative = this.CompareLessThan(number);
			Registers.F.Zero = this.CompareEquals(number);
		}

		public readonly bool CompareEquals(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.CompareEquals(this);

			return value == number.ToDecimal();
		}

		public readonly bool CompareGreaterThan(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareGreaterThan(number);

			return value > number.ToDecimal();
		}

		public readonly bool CompareLessThan(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).CompareLessThan(number);

			return value < number.ToDecimal();
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

			return new Decimal_T(value / number.ToDecimal());
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

			return new Decimal_T(value % number.ToDecimal());
		}

		public readonly INumber Multiply(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Multiply(this);

			return new Decimal_T(value * number.ToDecimal());
		}

		public readonly INumber Negate()
			=> new Decimal_T(-value);

		public readonly IFloat Pow(IFloat exponent) {
			if (TypeTracking.ShouldUpcast(this, exponent))
				return ((IFloat)exponent.Upcast(this)).Pow(exponent);

			return new Decimal_T(DecimalMath.DecimalEx.Pow(value, exponent.ToDecimal()));
		}

		public readonly INumber Repeat(INumber number) {
			if (TypeTracking.ShouldUpcast(this, number))
				return number.Upcast(this).Repeat(number);

			return new Decimal_T(Arithmetic.Repeat(value, number.ToDecimal()));
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

			return new Decimal_T(Arithmetic.Subtract(value, number.ToDecimal()));
		}

		public readonly IFloat Tan()
			=> new Decimal_T(DecimalMath.DecimalEx.Tan(value));

		public readonly IFloat Tanh()
			=> throw new InvalidOperationException("Performing \"tanh\" on decimal values is not supported");

		public readonly SByte ToSByte() => (SByte)value;

		public readonly Int16 ToInt16() => (Int16)value;

		public readonly Int32 ToInt32() => (Int32)value;

		public readonly Int64 ToInt64() => (Int64)value;

		public readonly Byte ToByte() => (Byte)value;

		public readonly UInt16 ToUInt16() => (UInt16)value;

		public readonly UInt32 ToUInt32() => (UInt32)value;

		public readonly UInt64 ToUInt64() => (UInt64)value;

		public readonly IntPtr ToIntPtr() => (IntPtr)value;

		public readonly UIntPtr ToUIntPtr() => (UIntPtr)value;

		public readonly Single ToSingle() => (Single)value;

		public readonly Double ToDouble() => (Double)value;

		public readonly Decimal ToDecimal() => (Decimal)value;

	}

}