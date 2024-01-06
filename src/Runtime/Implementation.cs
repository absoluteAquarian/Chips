using Chips.Runtime.Utility;
using System;
using System.Runtime.CompilerServices;

namespace Chips.Runtime {
	public static class Implementation {
		public static void Compare(object a, object b) {
			ValueConverter.CheckedBoxToUnderlyingType(a).Compare(ValueConverter.CheckedBoxToUnderlyingType(b));
		}

		public static bool Kbrdy() => Console.KeyAvailable;

		public static void Kbkey() => Registers.S.Value = Console.ReadKey(Registers.F.Zero).KeyChar.ToString();

		public static void Kbline() => Registers.S.Value = Console.ReadLine()!;

		public static void ToString(object value) {
			Registers.S.Value = value?.ToString()!;
		}

		public static void ToStringFormat(object value, string format) {
			Registers.S.Value = string.Format($"{{0:{format}}}", value);
		}

		public static unsafe void StoreIndirect<T>(T value) {
			ref T ptr = ref Unsafe.AsRef<T>((void*)Registers.A.Value.ToIntPtr());
			ptr = value;
		}
	}
}
