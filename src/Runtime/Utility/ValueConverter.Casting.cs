using Chips.Runtime.Types.NumberProcessing;
using System;
using System.Runtime.CompilerServices;

namespace Chips.Runtime.Utility {
	public static partial class ValueConverter {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SByte_T CastToSByte_T<T>(this T number) where T : INumber
			=> new(Convert.ToSByte(EnsureObjectCanBeCastToIConvertable(number)?.Value
				?? throw new ArgumentException($"Cannot cast a <~cplx> value to <{TypeTracking.GetChipsType(typeof(SByte_T))}>")));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SByte_T CastToSByte_T(this SByte number)
			=> new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Int16_T CastToInt16_T<T>(this T number) where T : INumber
			=> new(Convert.ToInt16(EnsureObjectCanBeCastToIConvertable(number)?.Value
				?? throw new ArgumentException($"Cannot cast a <~cplx> value to <{TypeTracking.GetChipsType(typeof(Int16_T))}>")));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Int16_T CastToInt16_T(this Int16 number)
			=> new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Int32_T CastToInt32_T<T>(this T number) where T : INumber
			=> new(Convert.ToInt32(EnsureObjectCanBeCastToIConvertable(number)?.Value
				?? throw new ArgumentException($"Cannot cast a <~cplx> value to <{TypeTracking.GetChipsType(typeof(Int32_T))}>")));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Int32_T CastToInt32_T(this Int32 number)
			=> new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Int64_T CastToInt64_T<T>(this T number) where T : INumber
			=> new(Convert.ToInt64(EnsureObjectCanBeCastToIConvertable(number)?.Value
				?? throw new ArgumentException($"Cannot cast a <~cplx> value to <{TypeTracking.GetChipsType(typeof(Int64_T))}>")));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Int64_T CastToInt64_T(this Int64 number)
			=> new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Byte_T CastToByte_T<T>(this T number) where T : INumber
			=> new(Convert.ToByte(EnsureObjectCanBeCastToIConvertable(number)?.Value
				?? throw new ArgumentException($"Cannot cast a <~cplx> value to <{TypeTracking.GetChipsType(typeof(Byte_T))}>")));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Byte_T CastToByte_T(this Byte number)
			=> new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt16_T CastToUInt16_T<T>(this T number) where T : INumber
			=> new(Convert.ToUInt16(EnsureObjectCanBeCastToIConvertable(number)?.Value
				?? throw new ArgumentException($"Cannot cast a <~cplx> value to <{TypeTracking.GetChipsType(typeof(UInt16_T))}>")));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt16_T CastToUInt16_T(this UInt16 number)
			=> new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt32_T CastToUInt32_T<T>(this T number) where T : INumber
			=> new(Convert.ToUInt32(EnsureObjectCanBeCastToIConvertable(number)?.Value
				?? throw new ArgumentException($"Cannot cast a <~cplx> value to <{TypeTracking.GetChipsType(typeof(UInt32_T))}>")));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt32_T CastToUInt32_T(this UInt32 number)
			=> new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt64_T CastToUInt64_T<T>(this T number) where T : INumber
			=> new(Convert.ToUInt64(EnsureObjectCanBeCastToIConvertable(number)?.Value
				?? throw new ArgumentException($"Cannot cast a <~cplx> value to <{TypeTracking.GetChipsType(typeof(UInt64_T))}>")));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt64_T CastToUInt64_T(this UInt64 number)
			=> new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Single_T CastToSingle_T<T>(this T number) where T : INumber
			=> new(Convert.ToSingle(EnsureObjectCanBeCastToIConvertable(number)?.Value
				?? throw new ArgumentException($"Cannot cast a <~cplx> value to <{TypeTracking.GetChipsType(typeof(Single_T))}>")));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Single_T CastToSingle_T(this Single number)
			=> new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Double_T CastToDouble_T<T>(this T number) where T : INumber
			=> new(Convert.ToDouble(EnsureObjectCanBeCastToIConvertable(number)?.Value
				?? throw new ArgumentException($"Cannot cast a <~cplx> value to <{TypeTracking.GetChipsType(typeof(Double_T))}>")));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Double_T CastToDouble_T(this Double number)
			=> new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Decimal_T CastToDecimal_T<T>(this T number) where T : INumber
			=> new(Convert.ToDecimal(EnsureObjectCanBeCastToIConvertable(number)?.Value
				?? throw new ArgumentException($"Cannot cast a <~cplx> value to <{TypeTracking.GetChipsType(typeof(Decimal_T))}>")));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Decimal_T CastToDecimal_T(this Decimal number)
			=> new(number);

	}
}