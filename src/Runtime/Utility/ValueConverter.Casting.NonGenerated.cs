using Chips.Runtime.Types.NumberProcessing;
using System.Runtime.CompilerServices;
using System;

namespace Chips.Runtime.Utility {
	partial class ValueConverter {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IntPtr_T CastToIntPtr_T<T>(this T number) where T : INumber => new((nint)Convert.ToInt64(number?.Value ?? throw new ArgumentNullException(nameof(number))));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IntPtr_T CastToIntPtr_T(this nint number) => new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UIntPtr_T CastToUIntPtr_T<T>(this T number) where T : INumber => new((nuint)Convert.ToInt64(number?.Value ?? throw new ArgumentNullException(nameof(number))));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UIntPtr_T CastToUIntPtr_T(this nuint number) => new(number);
	}
}
