using Chips.Runtime.Utility;
using System;

namespace Chips.Runtime.Types {
	public class Stack {
		private readonly object?[] stack;

		public int SP { get; private set; }

		private readonly int capacity;

		public Stack() {
			capacity = 1000;
			stack = new object?[1000];
			SP = 0;
		}

		public Stack(int capacity) {
			if (capacity <= 0)
				throw new ArgumentException("Size was too small. Expected a value greater than zero");

			this.capacity = capacity;
			stack = new object[capacity];
			SP = 0;
		}

		public void Push(object? obj) {
			int old = SP;

			if (SP >= capacity) {
				if (!Sandbox.AllowStackOverflow)
					throw new Exception("Stack overflow detected. Cannot push more objects to the stack.");

				old = capacity - 1;
				SP = -1;
			}

			stack[old] = obj;
			SP++;
		}

		public object? Pop() {
			int old = SP - 1;

			if (SP <= 0) {
				if (!Sandbox.AllowStackOverflow)
					throw new Exception("Stack underflow detected. Cannot pop more objects from the stack.");

				old = 0;
				SP = capacity;
			}

			object? obj = stack[old];
			stack[old] = null;
			SP--;

			return obj;
		}

		public void SetIndirect(object? obj, int offset) {
			int sp = SP;
			if (sp + offset < 0) {
				if (!Sandbox.AllowStackOverflow)
					throw new Exception("Stack underflow detected.  Indirect set went below index 0");

				offset += sp;
				sp = capacity + offset;
			} else if (sp + offset >= capacity) {
				if (!Sandbox.AllowStackOverflow)
					throw new Exception("Stack overflow detected.  Indirect set went above Stack capacity");

				offset -= capacity - 1 - sp;
				sp = offset;
			}

			stack[sp] = obj;
		}

		public object? GetIndirect(int offset) {
			int sp = SP;
			if (sp + offset < 0) {
				if (!Sandbox.AllowStackOverflow)
					throw new Exception("Stack underflow detected.  Indirect get went below index 0");

				offset += sp;
				sp = capacity + offset;
			} else if (sp + offset >= capacity) {
				if (!Sandbox.AllowStackOverflow)
					throw new Exception("Stack overflow detected.  Indirect get went above Stack capacity");

				offset -= capacity - 1 - sp;
				sp = offset;
			}

			return stack[sp];
		}

		public object? Peek() => SP == 0 ? throw new Exception("Stack does not contain any values") : stack[SP - 1];

		public override string ToString()
			=> SP == 0
				? "[ <empty> ]"
				: Formatting.FormatArray(stack[0..SP]);
	}
}
