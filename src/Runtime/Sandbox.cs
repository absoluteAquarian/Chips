using Chips.Runtime.Meta;
using System;

namespace Chips.Runtime {
	public static unsafe class Sandbox {
		public const string Version = "1.0";

		public static bool AllowStackOverflow;

		public static int Execute(string[] args, int stackSize, bool allowStackOverflow, delegate*<void> entryPoint) {
			//Invoke the static ctors
			_ = Metadata.Registers.A;
			_ = Metadata.Flags.Carry;
			_ = Metadata.stack;

			AllowStackOverflow = allowStackOverflow;
			Metadata.stack = new(stackSize);
			Metadata.programArgs = args;

			try {
				entryPoint();
				return 0;
			} catch (Exception ex) {
				Console.WriteLine($"{ex.GetType().Name} thrown: {ex.Message}\nCompiled stacktrace:\n{ex.StackTrace}");
				return -1;
			}
		}
	}
}
