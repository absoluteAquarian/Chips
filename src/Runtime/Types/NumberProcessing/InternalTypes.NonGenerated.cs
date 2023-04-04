using Chips.Runtime.Meta;
using Chips.Runtime.Utility;
using System;
using System.Numerics;
using System.Text;

namespace Chips.Runtime.Types.NumberProcessing{
	public struct BigInteger_T : INumber, IInteger{
		private BigInteger value;

		public object Value => value;

		public BigInteger_T(BigInteger value){
			this.value = value;
		}

		public BigInteger_T(int value){
			this.value = value;
		}


		public INumber Abs()
			=> new BigInteger_T(value > 0 ? value : -value);

		public INumber Add(INumber number){
			BigInteger_T convert = ValueConverter.CastToBigInteger_T(number);
			return new BigInteger_T(value + convert.value);
		}

		public IInteger And(IInteger number){
			if(number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if(number is IFloat)
				throw new Exception("Cannot perform bitwise-AND operations with non-integer values");

			BigInteger_T convert = ValueConverter.CastToBigInteger_T(iNum);
			return new BigInteger_T(value & convert.value);
		}

		public IInteger ArithmeticRotateLeft(){
			bool carry = Metadata.Flags.Carry;
			Metadata.Flags.Carry = value.Sign < 0;
			
			var i = new BigInteger_T((value / 2) << 1);  //Keep the bit amount the same
			if(carry)
				i.value |= 1;

			return i;
		}

		public IInteger ArithmeticRotateRight(){
			bool carry = Metadata.Flags.Carry;
			Metadata.Flags.Carry = !value.IsEven;
			
			var i = new BigInteger_T(value >> 1);
			if(carry)
				i.value |= (BigInteger)1 << (int)value.GetBitLength();

			return i;
		}

		public IInteger ArithmeticShiftLeft()
			=> new BigInteger_T(value << 1);

		public IInteger ArithmeticShiftRight()
			=> new BigInteger_T(value >> 1);

		public string BinaryRepresentation(bool leadingZeroes){
			byte[] bytes = value.ToByteArray();

			StringBuilder sb = new(bytes.Length * 2 + 1);
		
			int i = bytes.Length - 1;
			string first = Convert.ToString(bytes[i], 2);

			if(first[0] != '0' && value.Sign > 0)
				sb.Append('0');
		
			sb.Append(first);

			//Go from most significant byte to least significant byte
			for(i--; i >= 0; i--)
				sb.Append(Convert.ToString(bytes[i], 2).PadLeft(8, '0'));

			return sb.ToString();
		}

		public INumber Decrement()
			=> new BigInteger_T(value - 1);

		public INumber Divide(INumber number, bool inverseLogic = false){
			BigInteger_T convert = ValueConverter.CastToBigInteger_T(number);
			return new BigInteger_T(!inverseLogic ? value / convert.value : convert.value / value);
		}

		public IInteger GetBit(IInteger bit){
			if(bit is not INumber number)
				throw new ArgumentException("Internal Chips Error -- Value was an integer, but not a number");
			
			if(ArithmeticSet.Number.Create(number.Value).CompareTo(ushort.MaxValue) >= 0)
				return new BigInteger_T(0);

			ushort shift = (ushort)ValueConverter.CastToUInt16_T(number).Value;
			if(shift >= value.GetBitLength())
				return new BigInteger_T(0);
			
			BigInteger mask = 1 << shift;
			return new BigInteger_T(value & mask);
		}

		public INumber Increment()
			=> new BigInteger_T(value - 1);

		public INumber Multiply(INumber number){
			BigInteger_T convert = ValueConverter.CastToBigInteger_T(number);
			return new BigInteger_T(value * convert.value);
		}

		public INumber Negate()
			=> new BigInteger_T(-value);

		public IInteger Not()
			=> new BigInteger_T(~value);

		public IInteger Or(IInteger number){
			if(number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if(number is IFloat)
				throw new Exception("Cannot perform bitwise-OR operations with non-integer values");

			BigInteger_T convert = ValueConverter.CastToBigInteger_T(iNum);
			return new BigInteger_T(value | convert.value);
		}

		public INumber Remainder(INumber number){
			BigInteger_T convert = ValueConverter.CastToBigInteger_T(number);
			return new BigInteger_T(value % convert.value);
		}

		public INumber Subtract(INumber number){
			BigInteger_T convert = ValueConverter.CastToBigInteger_T(number);
			return new BigInteger_T(unchecked(value - convert.value));
		}

		public IInteger Xor(IInteger number){
			if(number is not INumber iNum)
				throw new Exception("Internal Chips Error: IInteger instance was not an INumber");

			if(number is IFloat)
				throw new Exception("Cannot perform bitwise-XOR operations with non-integer values");

			BigInteger_T convert = ValueConverter.CastToBigInteger_T(iNum);
			return new BigInteger_T(value ^ convert.value);
		}
	}

	public struct Half_T : INumber, IFloat{
		private Half value;
		public object Value => value;

		private float ValueF => (float)value;

		public Half_T(Half value)
			=> this.value = value;

		public Half_T(float value)
			=> this.value = (Half)value;

		public INumber Abs()
			=> new Half_T((float)Math.Abs(ValueF));

		public IFloat Acos()
			=> new Half_T((float)Math.Acos(ValueF));

		public IFloat Acosh()
			=> new Half_T((float)Math.Acosh(ValueF));

		public INumber Add(INumber number){
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if(number is IInteger)
				number = ValueConverter.CastToHalf_T(number);
			else if(targetSize > TypeTracking.GetSizeFromNumericType(typeof(Half)))
				return number.Add(this);

			Half_T convert = ValueConverter.CastToHalf_T(number);
			return new Half_T(ValueF + convert.ValueF);
		}

		public IFloat Asin()
			=> new Half_T((float)Math.Asin(ValueF));

		public IFloat Asinh()
			=> new Half_T((float)Math.Asinh(ValueF));

		public IFloat Atan()
			=> new Half_T((float)Math.Atan(ValueF));

		public IFloat Atan2(IFloat divisor, bool inverseLogic = false){
			if(divisor is not INumber number)
				throw new InvalidOperationException("Number was not a floating-point value");

			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if(number is IInteger)
				number = ValueConverter.CastToHalf_T(number);
			else if(targetSize > TypeTracking.GetSizeFromNumericType(typeof(Half)))
				return divisor.Atan2(this, true);

			Half_T convert = ValueConverter.CastToHalf_T(number);
			return new Half_T((float)Math.Atan2(!inverseLogic ? ValueF : convert.ValueF, !inverseLogic ? convert.ValueF : ValueF));
		}

		public IFloat Atanh()
			=> new Half_T((float)Math.Asinh(ValueF));

		public INumber Ceiling()
			=> new Half_T((float)Math.Ceiling(ValueF));

		public IFloat Cos()
			=> new Half_T((float)Math.Cos(ValueF));

		public IFloat Cosh()
			=> new Half_T((float)Math.Cosh(ValueF));

		public INumber Decrement()
			=> new Half_T(ValueF - 1);

		public INumber Divide(INumber number, bool inverseLogic = false){
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if(number is IInteger)
				number = ValueConverter.CastToHalf_T(number);
			else if(targetSize > TypeTracking.GetSizeFromNumericType(typeof(Half)))
				return number.Divide(this, true);

			Half_T convert = ValueConverter.CastToHalf_T(number);
			return new Half_T(!inverseLogic ? ValueF / convert.ValueF : convert.ValueF / ValueF);
		}

		public IFloat Exp()
			=> new Half_T((float)Math.Exp(ValueF));

		public INumber Floor()
			=> new Half_T((float)Math.Floor(ValueF));

		public IInteger GetBits()
			=> (ValueConverter.RetrieveFloatingPointBits(this) as IInteger)!;

		public INumber Increment()
			=> new Single_T(ValueF + 1);

		public IFloat Inverse()
			=> (new Half_T(1f).Divide(this) as IFloat)!;

		public IFloat Ln()
			=> new Half_T((float)Math.Log(ValueF));

		public IFloat Log10()
			=> new Half_T((float)Math.Log10(ValueF));

		public IFloat Log2()
			=> new Half_T((float)Math.Log2(ValueF));

		public INumber Multiply(INumber number){
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if(number is IInteger)
				number = ValueConverter.CastToHalf_T(number);
			else if(targetSize > TypeTracking.GetSizeFromNumericType(typeof(Half)))
				return number.Multiply(this);

			Half_T convert = ValueConverter.CastToHalf_T(number);
			return new Half_T(ValueF * convert.ValueF);
		}

		public INumber Negate()
			=> new Half_T(-ValueF);

		public IFloat Pow(IFloat exponent){
			if(exponent is not INumber number)
				throw new InvalidOperationException("Number was not a floating-point value");
			
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if(number is IInteger)
				number = ValueConverter.CastToHalf_T(number);
			else if(targetSize > TypeTracking.GetSizeFromNumericType(typeof(Half)))
				return ValueConverter.Constants.GetConst_E(number.Value.GetType()).Pow((number.Multiply((Ln() as INumber)!) as IFloat)!); //x^y = e^(y * ln(x))

			Half_T convert = ValueConverter.CastToHalf_T(number);
			return new Half_T((float)Math.Pow(ValueF, convert.ValueF));
		}

		public INumber Remainder(INumber number){
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if(number is IInteger)
				number = ValueConverter.CastToHalf_T(number);
			else if(targetSize > TypeTracking.GetSizeFromNumericType(typeof(Half)))
				return number.Remainder(this);

			Half_T convert = ValueConverter.CastToHalf_T(number);
			return new Half_T(ValueF % convert.ValueF);
		}

		public IFloat Root(IFloat root)
			=> Pow(root.Inverse());

		public IFloat Sin()
			=> new Half_T((float)Math.Sin(ValueF));

		public IFloat Sinh()
			=> new Half_T((float)Math.Sinh(ValueF));

		public IFloat Sqrt()
			=> new Half_T((float)Math.Sqrt(ValueF));

		public INumber Subtract(INumber number){
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if(number is IInteger)
				number = ValueConverter.CastToHalf_T(number);
			else if(targetSize > TypeTracking.GetSizeFromNumericType(typeof(Half)))
				return number.Negate().Subtract(this.Negate());

			Half_T convert = ValueConverter.CastToHalf_T(number);
			return new Half_T(ValueF - convert.ValueF);
		}

		public IFloat Tan()
			=> new Half_T((float)Math.Tan(ValueF));

		public IFloat Tanh()
			=> new Half_T((float)Math.Tanh(ValueF));
	}

	public struct Complex_T : INumber, IFloat{
		private Complex value;
		public object Value => value;

		public Complex_T(Complex value)
			=> this.value = value;

		public Complex_T(float value)
			=> this.value = new Complex(value, 0);

		public INumber Abs()
			=> new Complex_T(Complex.Abs(value));

		public IFloat Acos()
			=> new Complex_T(Complex.Acos(value));

		public IFloat Acosh()
			=> throw new InvalidOperationException("Operation \"acosh\" is not supported on complex numbers");

		public INumber Add(INumber number){
			Complex_T convert = ValueConverter.CastToComplex_T(number);
			return new Complex_T(value + convert.value);
		}

		public IFloat Asin()
			=> new Complex_T(Complex.Asin(value));

		public IFloat Asinh()
			=> throw new InvalidOperationException("Operation \"asinh\" is not supported on complex numbers");

		public IFloat Atan()
			=> new Complex_T(Complex.Atan(value));

		public IFloat Atan2(IFloat divisor, bool inverseLogic = false)
			=> throw new InvalidOperationException("Operation \"atan2\" is not supported on complex numbers");

		public IFloat Atanh()
			=> throw new InvalidOperationException("Operation \"atanh\" is not supported on complex numbers");

		public INumber Ceiling()
			=> new Complex_T(new Complex(Math.Ceiling(value.Real), Math.Ceiling(value.Imaginary)));

		public IFloat Cos()
			=> new Complex_T(Complex.Cos(value));

		public IFloat Cosh()
			=> throw new InvalidOperationException("Operation \"cosh\" is not supported on complex numbers");

		public INumber Decrement()
			=> throw new InvalidOperationException("Operation \"decrement\" is not supported on complex numbers");

		public INumber Divide(INumber number, bool inverseLogic = false){
			Complex_T convert = ValueConverter.CastToComplex_T(number);
			return new Complex_T(!inverseLogic ? value / convert.value : convert.value / value);
		}

		public IFloat Exp()
			=> new Complex_T(Complex.Exp(value));

		public INumber Floor()
			=> new Complex_T(new Complex(Math.Floor(value.Real), Math.Floor(value.Imaginary)));

		public IInteger GetBits()
			=> throw new InvalidOperationException("Operation \"bits\" is not supported on complex numbers");

		public INumber Increment()
			=> throw new InvalidOperationException("Operation \"increment\" is not supported on complex numbers");

		public IFloat Inverse()
			=> new Complex_T(Complex.Reciprocal(value));

		public IFloat Ln()
			=> new Complex_T(Complex.Log(value));

		public IFloat Log10()
			=> new Complex_T(Complex.Log10(value));

		public IFloat Log2()
			=> new Complex_T(Complex.Log(value, 2));

		public INumber Multiply(INumber number){
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if(number is IInteger)
				number = ValueConverter.CastToComplex_T(number);
			else if(targetSize > TypeTracking.GetSizeFromNumericType(typeof(Half)))
				return number.Multiply(this);

			Complex_T convert = ValueConverter.CastToComplex_T(number);
			return new Complex_T(value * convert.value);
		}

		public INumber Negate()
			=> new Complex_T(-value);

		public IFloat Pow(IFloat exponent){
			if(exponent is not INumber number)
				throw new InvalidOperationException("Number was not a floating-point value");
			
			Complex_T convert = ValueConverter.CastToComplex_T(number);
			return new Complex_T(Complex.Pow(value, convert.value));
		}

		public INumber Remainder(INumber number)
			=> throw new InvalidOperationException("Operation \"remainder\" is not supported on complex numbers");

		public IFloat Root(IFloat root)
			=> Pow(root.Inverse());

		public IFloat Sin()
			=> new Complex_T(Complex.Sin(value));

		public IFloat Sinh()
			=> throw new InvalidOperationException("Operation \"sinh\" is not supported on complex numbers");

		public IFloat Sqrt()
			=> new Complex_T(Complex.Sqrt(value));

		public INumber Subtract(INumber number){
			int targetSize = TypeTracking.GetSizeFromNumericType(number.Value.GetType());

			if(number is IInteger)
				number = ValueConverter.CastToComplex_T(number);
			else if(targetSize > TypeTracking.GetSizeFromNumericType(typeof(Half)))
				return number.Negate().Subtract(this.Negate());

			Complex_T convert = ValueConverter.CastToComplex_T(number);
			return new Complex_T(value - convert.value);
		}

		public IFloat Tan()
			=> new Complex_T(Complex.Tan(value));

		public IFloat Tanh()
			=> throw new InvalidOperationException("Operation \"tanh\" is not supported on complex numbers");
	}
}
