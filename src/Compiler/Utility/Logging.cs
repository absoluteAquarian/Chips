using System;

namespace Chips.Compiler.Utility {
	internal static class Logging {
		public static void Debug(string message) {
			Console.WriteLine(message);
		}

		public static void Debug(object obj) {
			Console.WriteLine(obj);
		}

		public static void Warn(string message) {
			PrintWithColor(message, ConsoleColor.Yellow);
		}

		public static void Warn(object obj) {
			PrintWithColor(obj?.ToString(), ConsoleColor.Yellow);
		}

		public static void Error(string message) {
			PrintWithColor(message, ConsoleColor.Red);
		}

		public static void Error(object obj) {
			PrintWithColor(obj?.ToString(), ConsoleColor.Red);
		}

		public static void PrintWithColor(string? message, ConsoleColor color) {
			ConsoleColor fg = Console.ForegroundColor;
			ConsoleColor bg = Console.BackgroundColor;

			Console.ForegroundColor = color;
			Console.BackgroundColor = ConsoleColor.Black;

			Console.WriteLine(message);

			Console.ForegroundColor = fg;
			Console.BackgroundColor = bg;
		}
	}
}
