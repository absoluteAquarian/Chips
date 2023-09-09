using Chips.Runtime.Types.NumberProcessing;
using Chips.Utility;
using System;

namespace Chips.Runtime.Utility {
	public static partial class ValueConverter {
		public static class Constants {
			public static readonly float E_Single = 2.7182818284590451f;
			public static readonly double E_Double = Math.E;
			public static readonly decimal E_Decimal = DecimalMath.DecimalEx.E;

			public static IFloat GetConst_E(Type t) {
				if (t == typeof(float))
					return new Single_T(E_Single);
				else if (t == typeof(double))
					return new Double_T(E_Double);
				else if (t == typeof(decimal))
					return new Decimal_T(E_Decimal);

				throw new InvalidOperationException("Function expects a floating-point type");
			}
		}

		internal static INumber UpcastToAtLeastInt32(INumber number)
			=> number switch {
				SByte_T sb => new Int32_T((sbyte)sb.Value),
				Byte_T b => new Int32_T((byte)b.Value),
				Int16_T s => new Int32_T((short)s.Value),
				UInt16_T u => new Int32_T((ushort)u.Value),
				Int32_T _ => number,
				UInt32_T _ => number,
				Int64_T _ => number,
				UInt64_T _ => number,
				_ => throw new Exception($"Cannot upcast {(number?.Value?.GetType().GetSimplifiedGenericTypeName() ?? "unknown")} to an integer")
			};

		// TODO: decimal support
		internal static unsafe INumber RetrieveFloatingPointBits(INumber number)
			=> number.Value switch {
				float f => new Int32_T(BitConverter.SingleToInt32Bits(f)),
				double d => new Int64_T(BitConverter.DoubleToInt64Bits(d)),
				decimal _ => throw new Exception("Operation is not supported on decimal instances"),
				_ => throw new Exception("Operation can only be performed on floating-point types")
			};

		public static INumber? BoxToUnderlyingType(object? o)
			=> o switch {
				int i => new Int32_T(i),
				sbyte s => new SByte_T(s),
				short sh => new Int16_T(sh),
				long l => new Int64_T(l),
				uint u => new UInt32_T(u),
				byte b => new Byte_T(b),
				ushort us => new UInt16_T(us),
				ulong ul => new UInt64_T(ul),
				float f => new Single_T(f),
				double d => new Double_T(d),
				decimal dm => new Decimal_T(dm),
				_ => null  //Unsuccessful boxing should just result in "null", as most checks are usually to see if the result is an IInteger or IFloat
			};
	}
}
