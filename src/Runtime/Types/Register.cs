using Chips.Runtime.Types.NumberProcessing;
using Chips.Runtime.Utility;
using Chips.Utility;
using System;

namespace Chips.Runtime.Types {
	public abstract class Register {
		public int ID { get; init; }

		public abstract bool AcceptsValue(object? value);

		public abstract void SetValue(object? value);
	}

	public abstract class Register<T> : Register {
		private T _value;

		public T Value {
			get => _value;
			set {
				if (HasValueChanged(_value, value)) {
					OnValueChanged(value);
					_value = value;
				}
			}
		}

		protected abstract bool HasValueChanged(T oldValue, T newValue);

		protected abstract void OnValueChanged(T newValue);

		public override void SetValue(object? value) {
			if (AcceptsValue(value))
				Value = (T)value!;
			else
				throw new ArgumentException($"Register {Registers.GetRegisterNameFromID(ID)} cannot accept values of type \"{typeof(T).GetSimplifiedGenericTypeName()}\"");
		}
	}

	public sealed class IntegerRegister : Register<IInteger> {
		protected override bool HasValueChanged(IInteger oldValue, IInteger newValue) {
			// Automatically handles type changes as well as value equivalence
			return !oldValue.Value.Equals(newValue.Value);
		}

		protected override void OnValueChanged(IInteger newValue) {
			Registers.F.Zero = newValue.IsZero;
			Registers.F.Negative = newValue.IsNegative;
		}

		public override bool AcceptsValue(object? value) {
			return TypeTracking.IsInteger(value);
		}

		public void Set(sbyte value) => Value = value.CastToSByte_T();
		public void Set(byte value) => Value = value.CastToByte_T();
		public void Set(short value) => Value = value.CastToInt16_T();
		public void Set(ushort value) => Value = value.CastToUInt16_T();
		public void Set(int value) => Value = value.CastToInt32_T();
		public void Set(uint value) => Value = value.CastToUInt32_T();
		public void Set(long value) => Value = value.CastToInt64_T();
		public void Set(ulong value) => Value = value.CastToUInt64_T();

		public override void SetValue(object? value) {
			value = ValueConverter.CheckedBoxToUnderlyingType(value);
			base.SetValue(value);
		}
	}

	public sealed class FloatRegister : Register<IFloat> {
		protected override bool HasValueChanged(IFloat oldValue, IFloat newValue) {
			return !oldValue.Value.Equals(newValue.Value);
		}

		protected override void OnValueChanged(IFloat newValue) {
			Registers.F.Zero = newValue.IsZero;
			Registers.F.Negative = newValue.IsNegative;
			Registers.F.InvalidFloat = newValue.IsNaN;
		}

		public override bool AcceptsValue(object? value) {
			return TypeTracking.IsFloatingPoint(value);
		}

		public void Set(float value) => Value = value.CastToSingle_T();
		public void Set(double value) => Value = value.CastToDouble_T();
		public void Set(decimal value) => Value = value.CastToDecimal_T();

		public override void SetValue(object? value) {
			value = ValueConverter.CheckedBoxToUnderlyingType(value);
			base.SetValue(value);
		}
	}

	public sealed class StringRegister : Register<string> {
		protected override bool HasValueChanged(string oldValue, string newValue) {
			return oldValue != newValue;
		}

		protected override void OnValueChanged(string newValue) {
			Registers.F.Zero = newValue is null;
		}

		public override bool AcceptsValue(object? value) {
			return value is string;
		}
	}

	public sealed class ExceptionRegister : Register<Exception> {
		protected override bool HasValueChanged(Exception oldValue, Exception newValue) {
			return !object.ReferenceEquals(oldValue, newValue);
		}

		protected override void OnValueChanged(Exception newValue) { }

		public override bool AcceptsValue(object? value) {
			return value is Exception;
		}
	}

	public sealed class FlagsRegister : Register<byte> {
		public const byte Mask_Carry = 0x80;
		public bool Carry {
			get => (Value & Mask_Carry) != 0;
			set => Value = (byte)(value ? Value | Mask_Carry : Value & ~Mask_Carry);
		}

		public const byte Mask_InvalidFloat = 0x10;
		public bool InvalidFloat {
			get => (Value & Mask_InvalidFloat) != 0;
			set => Value = (byte)(value ? Value | Mask_InvalidFloat : Value & ~Mask_InvalidFloat);
		}

		public const byte Mask_Conversion = 0x08;
		public bool Conversion {
			get => (Value & Mask_Conversion) != 0;
			set => Value = (byte)(value ? Value | Mask_Conversion : Value & ~Mask_Conversion);
		}

		public const byte Mask_Zero = 0x04;
		public bool Zero {
			get => (Value & Mask_Zero) != 0;
			set => Value = (byte)(value ? Value | Mask_Zero : Value & ~Mask_Zero);
		}

		public const byte Mask_Negative = 0x02;
		public bool Negative {
			get => (Value & Mask_Negative) != 0;
			set => Value = (byte)(value ? Value | Mask_Negative : Value & ~Mask_Negative);
		}

		public const byte Mask_Overflow = 0x01;
		public bool Overflow {
			get => (Value & Mask_Overflow) != 0;
			set => Value = (byte)(value ? Value | Mask_Overflow : Value & ~Mask_Overflow);
		}

		protected override bool HasValueChanged(byte oldValue, byte newValue) {
			return oldValue != newValue;
		}

		protected override void OnValueChanged(byte newValue) { }

		public override bool AcceptsValue(object? value) {
			throw new NotSupportedException("Flags register does not support writing to it");
		}

		public override void SetValue(object? value) {
			throw new NotSupportedException("Flags register does not support writing to it");
		}
	}
}
