using Chips.Runtime.Types.NumberProcessing;
using System.Runtime.CompilerServices;

namespace Chips.Runtime.Utility {
	partial class ValueConverter {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IntPtr_T CastToIntPtr_T<T>(this T number) where T : INumber => new(number.ToIntPtr());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IntPtr_T CastToIntPtr_T(this nint number) => new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static nint CastToIntPtr(INumber number) => number.ToIntPtr();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UIntPtr_T CastToUIntPtr_T<T>(this T number) where T : INumber => new(number.ToUIntPtr());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UIntPtr_T CastToUIntPtr_T(this nuint number) => new(number);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static nuint CastToUIntPtr(INumber number) => number.ToUIntPtr();
	}
}
