using Chips.Runtime.Types.NumberProcessing;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Chips.Runtime.Utility {
	public static partial class ValueConverter {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BigInteger_T CastToBigInteger_T<T>(this T number) where T : INumber
			=> new(Convert.ToUInt64(EnsureObjectCanBeCastToIConvertable(number)?.Value
				?? throw new ArgumentException($"Cannot cast a <~cplx> value to <{TypeTracking.GetChipsType(typeof(UInt64_T))}>")));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BigInteger_T CastToBigInteger_T(this BigInteger number)
			=> new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Half_T CastToHalf_T<T>(this T number) where T : INumber
			=> new((Half)Convert.ToSingle(EnsureObjectCanBeCastToIConvertable(number)?.Value
				?? throw new ArgumentException($"Cannot cast a <~cplx> value to <{TypeTracking.GetChipsType(typeof(Single_T))}>")));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Half_T CastToHalf_T(this Half number)
			=> new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Complex_T CastToComplex_T<T>(this T number) where T : INumber
			=> new(new Complex(Convert.ToDouble(EnsureObjectCanBeCastToIConvertable(number)?.Value
				?? throw new ArgumentException($"Cannot cast a <~cplx> value to <{TypeTracking.GetChipsType(typeof(Double_T))}>")),
				0));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Complex_T CastToComplex_T(this Complex number)
			=> new(number);
	}
}
