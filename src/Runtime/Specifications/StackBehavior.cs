namespace Chips.Runtime.Specifications {
	public enum StackBehavior {
		None = 0x0000,

		PushOne = 0x0001,
		PushTwo = 0x0002,

		PopOne = 0x0100,
		PopTwo = 0x0200,
		PopThree = 0x0400,
		PopVar = 0x0800
	}
}
