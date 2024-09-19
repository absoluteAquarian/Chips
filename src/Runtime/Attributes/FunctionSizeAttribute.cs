using System;

namespace Chips.Runtime.Attributes {
	[AttributeUsage(AttributeTargets.Method)]
	public class FunctionSizeAttribute : Attribute {
		public readonly int Size;

		public FunctionSizeAttribute(int size) {
			Size = size;
		}
	}
}
