using Chips.Runtime.Types.NumberProcessing;
using System;
using System.Numerics;

namespace Chips.Runtime.Utility {
	public static partial class ValueConverter {
		public static class Constants {
			public static readonly float E_Single = 2.7182818284590451f;
			public static readonly double E_Double = Math.E;
			public static readonly decimal E_Decimal = DecimalMath.DecimalEx.E;
			public static readonly Half E_Half = (Half)E_Single;

			public static IFloat GetConst_E(Type t) {
				if (t == typeof(float))
					return new Single_T(E_Single);
				else if (t == typeof(double))
					return new Double_T(E_Double);
				else if (t == typeof(decimal))
					return new Decimal_T(E_Decimal);
				else if (t == typeof(Half))
					return new Half_T(E_Half);

				throw new InvalidOperationException("Function expects a floating-point type");
			}
		}

		public static long? AsSignedInteger(object? obj)
			=> obj as sbyte? ?? obj as short? ?? obj as int? ?? obj as long?;

		public static ulong? AsUnsignedInteger(object? obj)
			=> obj as byte? ?? obj as ushort? ?? obj as uint? ?? obj as ulong?;

		public static double? AsFloatingPoint(object? obj)
			=> obj as float? ?? obj as double?;

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
				_ => throw new Exception($"Cannot upcast <{(TypeTracking.GetChipsType(number.Value, throwOnNotFound: false) ?? "unknown")}> to an integer")
			};

		// TODO: decimal support
		internal static unsafe INumber RetrieveFloatingPointBits(INumber number)
			=> number.Value switch {
				float f => new Int32_T(*(int*)&f),
				double d => new Int64_T(*(long*)&d),
				decimal dm => throw new Exception("Operation is not supported on <f128> instances"),
				Half h => new Int16_T(*(short*)&h),
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
				Half h => new Half_T(h),
				Complex c => new Complex_T(c),
				_ => null  //Unsuccessful boxing should just result in "null", as most checks are usually to see if the result is an IInteger or IFloat
			};

		internal static INumber? EnsureObjectCanBeCastToIConvertable(INumber num)
			=> num switch {
				BigInteger_T b => new Double_T((double)(BigInteger)b.Value),
				Complex_T _ => null,  //Conversion not allowed
				_ => num
			};

		internal static object?[] AsObjectArray(object? obj) {
			if (obj is null)
				return null!;

			if (obj is not Array array)
				return new object?[] { obj };

			object?[] arr = new object[array.Length];
			for (int i = 0; i < array.Length; i++)
				arr[i] = array.GetValue(i);

			return arr;
		}
	}
}
