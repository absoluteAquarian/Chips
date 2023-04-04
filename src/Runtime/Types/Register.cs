using Chips.Runtime.Meta;
using Chips.Runtime.Specifications;
using Chips.Runtime.Types.NumberProcessing;
using Chips.Runtime.Utility;

namespace Chips.Runtime.Types {
	public unsafe class Register {
		private object? data;
		public object? Data {
			get => getDataOverride is not null ? getDataOverride() : data;
			set {
				if (acceptValueFunc is not null && !acceptValueFunc(value))
					throw new RegisterAssignmentException($"{Formatting.FormatObject(this)} cannot accept values of type \"{TypeTracking.GetChipsType(value)}\"", globalContext);

				data = value;

				Metadata.Registers.CheckRegister(this);
			}
		}

		internal readonly delegate*<object?, bool> acceptValueFunc;

		internal delegate*<object?> getDataOverride;

		public readonly string name;

		internal static Opcode.FunctionContext globalContext;

		internal Register(string name, object? initialValue, delegate*<object?, bool> acceptValueFunc) {
			this.name = name;
			Data = initialValue;
			this.acceptValueFunc = acceptValueFunc;
		}

		public bool GetDataAsInt32(out int value) {
			if (ValueConverter.BoxToUnderlyingType(Data) is INumber num) {
				value = (int)ValueConverter.CastToInt32_T(num).Value;
				return true;
			}

			value = -1;
			return false;
		}

		public override string ToString() => $"&{name}";
	}
}
