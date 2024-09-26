using System;
using System.Collections;
using System.Collections.Generic;

namespace Chips.Compiler.Utility {
	public class ConsoleMessage : IEnumerable<IConsoleText> {
		private readonly List<IConsoleText> _messages = [];
		private bool _finalized = false;

		public ConsoleMessage Add(string? text, ConsoleColorState? colors = null) {
			if (colors is ConsoleColorState colorsValue)
				UpdateMessageList(new ChangeConsoleColors(colorsValue));

			if (text is null)
				return this;

			if (text.Contains('\n')) {
				foreach (string line in text.Split('\n')) {
					UpdateMessageList(new TextWithColor(line.Trim('\r')));
					UpdateMessageList(default(ConsoleTextLineBreak));
				}
			} else {
				// The text is a single line
				UpdateMessageList(new TextWithColor(text));
			}

			return this;
		}

		public ConsoleMessage Add(object? obj, ConsoleColorState? colors = null) => Add(obj?.ToString(), colors);

		public ConsoleMessage AddLine() {
			UpdateMessageList(default(ConsoleTextLineBreak));
			return this;
		}

		public ConsoleMessage AddLine(string text, ConsoleColorState? colors = null) {
			Add(text, colors);
			UpdateMessageList(default(ConsoleTextLineBreak));
			return this;
		}

		public ConsoleMessage Offset(int offset, ConsoleColorState? colors = null) {
			if (offset < 0)
				throw new ArgumentOutOfRangeException(nameof(offset), "Offset must be non-negative");

			if (colors is ConsoleColorState colorsValue)
				UpdateMessageList(new ChangeConsoleColors(colorsValue));

			UpdateMessageList(new TextWithColor(new string(' ', offset)));
			return this;
		}

		public ConsoleMessage ChangeColors(ConsoleColor foreground, ConsoleColor background) {
			UpdateMessageList(new ChangeConsoleColors(new ConsoleColorState(foreground, background)));
			return this;
		}

		public ConsoleMessage IndentRight() {
			UpdateMessageList(new ConsoleIndent(true));
			return this;
		}

		public ConsoleMessage IndentLeft() {
			UpdateMessageList(new ConsoleIndent(false));
			return this;
		}

		private void UpdateMessageList(IConsoleText msg) {
			if (_finalized)
				throw new InvalidOperationException("Cannot add messages to a finalized ConsoleMessage");

			_messages.Add(msg);
		}

		public ConsoleMessage Finalize() {
			if (_finalized)
				throw new InvalidOperationException("ConsoleMessage is already finalized");

			_finalized = true;
			return this;
		}

		public void Print() {
			if (!_finalized)
				throw new InvalidOperationException("Cannot print a non-finalized ConsoleMessage");

			ConsolePrintingState state = ConsolePrintingState.Create();

			foreach (IConsoleText message in _messages)
				message.Print(ref state);

			// Reset the console's colors
			state.ResetColorsToOriginal();
		}

		public IEnumerator<IConsoleText> GetEnumerator() => _messages.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	public struct ConsolePrintingState {
		private int _indent;
		private bool _hasIndentedThisLine;
		private bool _hasWrittenTextThisLine;

		public readonly ConsoleColorState originalColors;

		private ConsolePrintingState(ConsoleColor foreground, ConsoleColor background) {
			_indent = 0;
			originalColors = new ConsoleColorState(foreground, background);
		}

		public static ConsolePrintingState Create() => new ConsolePrintingState(Console.ForegroundColor, Console.BackgroundColor);

		public void Print(string text) {
			if (!_hasIndentedThisLine)
				PrintIndents();

			Console.Write(text);
			_hasWrittenTextThisLine = true;
		}

		public void NewLine() {
			Console.WriteLine();
			_hasWrittenTextThisLine = false;
			_hasIndentedThisLine = false;
		}

		public void IncrementIndent() {
			if (_hasWrittenTextThisLine)
				throw new InvalidOperationException("Indent operation must be the first operation on a new line");
			if (!_hasIndentedThisLine)
				throw new InvalidOperationException("Indent can only be modified after the initial indent has been printed");

			_indent++;
			Console.CursorLeft += _indent * 2;
		}

		public void DecrementIndent() {
			if (_hasWrittenTextThisLine)
				throw new InvalidOperationException("Indent operation must be the first operation on a new line");
			if (!_hasIndentedThisLine)
				throw new InvalidOperationException("Indent can only be modified after the initial indent has been printed");

			if (_indent == 0)
				throw new InvalidOperationException("Indent level cannot be negative");

			_indent--;
			Console.CursorLeft -= _indent * 2;
		}

		public void ResetIndent() => _indent = 0;

		public void PrintIndents() {
			if (_hasIndentedThisLine)
				throw new InvalidOperationException("Cannot print indents more than once on a single line");

			Console.CursorLeft = _indent * 2;
			_hasIndentedThisLine = true;
		}

		public static void SetColors(ConsoleColorState colors) {
			Console.ForegroundColor = colors.foreground;
			Console.BackgroundColor = colors.background;
		}

		public readonly void ResetColorsToOriginal() => SetColors(originalColors);
	}

	public interface IConsoleText {
		void Print(ref ConsolePrintingState state);
	}

	internal readonly struct TextWithColor(string text) : IConsoleText {
		public readonly string text = text;

		public void Print(ref ConsolePrintingState state) => state.Print(text);
	}

	internal readonly struct ChangeConsoleColors(ConsoleColorState colors) : IConsoleText {
		public readonly ConsoleColorState colors = colors;

		public void Print(ref ConsolePrintingState state) => ConsolePrintingState.SetColors(colors);
	}

	public readonly struct ConsoleColorState(ConsoleColor foreground, ConsoleColor background) {
		public readonly ConsoleColor foreground = foreground;
		public readonly ConsoleColor background = background;
	}

	internal readonly struct ConsoleTextLineBreak : IConsoleText {
		public void Print(ref ConsolePrintingState state) => state.NewLine();
	}

	internal readonly struct ConsoleIndent(bool positive) : IConsoleText {
		public readonly bool positive = positive;

		public void Print(ref ConsolePrintingState state) {
			if (positive)
				state.IncrementIndent();
			else
				state.DecrementIndent();
		}
	}

	internal readonly struct ConsolePrintIndents : IConsoleText {
		public void Print(ref ConsolePrintingState state) => state.PrintIndents();
	}

	internal readonly struct ConsoleResetIndent : IConsoleText {
		public void Print(ref ConsolePrintingState state) => state.ResetIndent();
	}
}
