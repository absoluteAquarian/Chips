namespace Chips.Common.DataStructures {
	public sealed class Ref<T>(T value) {
		public T value = value;

		public static implicit operator T(Ref<T> reference) => reference.value;
		public static implicit operator Ref<T>(T value) => new(value);
	}
}
