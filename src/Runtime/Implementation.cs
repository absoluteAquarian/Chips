using System;

namespace Chips.Runtime {
	public static class Implementation {
		public static bool Kbrdy() => Console.KeyAvailable;

		public static void Kbkey() => Registers.S.Value = Console.ReadKey(Registers.F.Zero).KeyChar.ToString();
	}
}
