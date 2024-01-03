using Chips.Runtime.Types.NumberProcessing;
using System;
using System.Runtime.CompilerServices;

namespace Chips.Runtime.Utility {
	partial class ValueConverter {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SByte_T CastToSByte_T<T>(this T number) where T : INumber => new(number.ToSByte());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SByte_T CastToSByte_T(this SByte number) => new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SByte CastToSByte(INumber number) => number.ToSByte();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Int16_T CastToInt16_T<T>(this T number) where T : INumber => new(number.ToInt16());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Int16_T CastToInt16_T(this Int16 number) => new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Int16 CastToInt16(INumber number) => number.ToInt16();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Int32_T CastToInt32_T<T>(this T number) where T : INumber => new(number.ToInt32());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Int32_T CastToInt32_T(this Int32 number) => new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Int32 CastToInt32(INumber number) => number.ToInt32();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Int64_T CastToInt64_T<T>(this T number) where T : INumber => new(number.ToInt64());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Int64_T CastToInt64_T(this Int64 number) => new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Int64 CastToInt64(INumber number) => number.ToInt64();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Byte_T CastToByte_T<T>(this T number) where T : INumber => new(number.ToByte());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Byte_T CastToByte_T(this Byte number) => new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Byte CastToByte(INumber number) => number.ToByte();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt16_T CastToUInt16_T<T>(this T number) where T : INumber => new(number.ToUInt16());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt16_T CastToUInt16_T(this UInt16 number) => new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt16 CastToUInt16(INumber number) => number.ToUInt16();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt32_T CastToUInt32_T<T>(this T number) where T : INumber => new(number.ToUInt32());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt32_T CastToUInt32_T(this UInt32 number) => new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt32 CastToUInt32(INumber number) => number.ToUInt32();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt64_T CastToUInt64_T<T>(this T number) where T : INumber => new(number.ToUInt64());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt64_T CastToUInt64_T(this UInt64 number) => new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt64 CastToUInt64(INumber number) => number.ToUInt64();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Single_T CastToSingle_T<T>(this T number) where T : INumber => new(number.ToSingle());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Single_T CastToSingle_T(this Single number) => new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Single CastToSingle(INumber number) => number.ToSingle();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Double_T CastToDouble_T<T>(this T number) where T : INumber => new(number.ToDouble());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Double_T CastToDouble_T(this Double number) => new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Double CastToDouble(INumber number) => number.ToDouble();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Decimal_T CastToDecimal_T<T>(this T number) where T : INumber => new(number.ToDecimal());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Decimal_T CastToDecimal_T(this Decimal number) => new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Decimal CastToDecimal(INumber number) => number.ToDecimal();

	}
}
