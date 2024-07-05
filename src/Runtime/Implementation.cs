using Chips.Runtime.Types.NumberProcessing;
using Chips.Runtime.Utility;
using Chips.Utility;
using System;
using System.Runtime.CompilerServices;

namespace Chips.Runtime {
	public static class Implementation {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Compare(object value2, object value1) => ValueConverter.CheckedBoxToUnderlyingType(value1).Compare(ValueConverter.CheckedBoxToUnderlyingType(value2));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Kbrdy() => Console.KeyAvailable;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Kbkey() => Registers.S.Value = Console.ReadKey(Registers.F.Zero).KeyChar.ToString();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Kbline() => Registers.S.Value = Console.ReadLine()!;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToString(object value) => Registers.S.Value = value?.ToString()!;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ToStringFormat(object value, string format) => Registers.S.Value = string.Format($"{{0:{format}}}", value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe void StoreIndirect<T>(T value) => Unsafe.AsRef<T>((void*)Registers.A.Value.ToIntPtr()) = value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AssignArithmeticResult(INumber number) {
			if (number is IInteger numInteger)
				Registers.A.Value = numInteger;
			else if (number is IFloat numFloat)
				Registers.I.Value = numFloat;
			else
				throw new ArgumentException("Unknown number type: " + number.GetType().GetFullGenericTypeName(), nameof(number));
		}
	}
}
