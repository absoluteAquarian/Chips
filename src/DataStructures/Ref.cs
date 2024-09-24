namespace Chips.Common.DataStructures {
	public abstract class __Ref {
		public abstract object Copy();
	}

	public sealed class Ref<T>(T value) : __Ref {
		public T value = value;

		public override object Copy() => new Ref<T>(value);  // Since the Chips.Runtime assembly won't be using Ref<T> directly except for creation, this redirection is needed to "copy" valuetype instances.

		public override string? ToString() => value?.ToString();

		public static implicit operator T(Ref<T> reference) => reference.value;
		public static implicit operator Ref<T>(T value) => new(value);
	}
}
