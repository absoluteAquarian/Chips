using System;

namespace Chips.Runtime.Types {
	public class List {
		private object?[] values;

		public int Capacity {
			get => values.Length;
			set {
				if (value < 0)
					throw new ArgumentOutOfRangeException(nameof(value), "New capacity was negative");

				if (value != values.Length) {
					if (value == 0)
						values = Array.Empty<object>();
					else
						Array.Resize(ref values, value);

					ResetCount();
				}
			}
		}

		public int Count { get; private set; }

		public List() {
			values = Array.Empty<object>();
			Count = 0;
		}

		public List(int capacity) {
			values = new object[capacity];

			ResetCount();
		}

		public List(object[] values) {
			this.values = (object[])values.Clone();

			ResetCount();
		}

		public List(Array array) {
			values = new object[array.Length];

			Array.Copy(array, values, array.Length);

			ResetCount();
		}

		private void ResetCount() {
			Count = 0;

			for (int i = 0; i < values.Length; i++)
				if (values[i] is not null)
					Count++;
		}

		public object? this[int index] {
			get => values[index];
			set {
				if (index < 0)
					throw new ArgumentOutOfRangeException(nameof(index), "Index was negative");

				if (index >= values.Length)
					throw new ArgumentOutOfRangeException(nameof(index), $"Index ({index}) was equal to or exceeded list capacity ({Capacity})");

				bool valueExists = values[index] is not null;
				values[index] = value;

				if (!valueExists && value is not null)
					Count++;
				else if (valueExists && value is null)
					Count--;
			}
		}

		public object[] ToArray() => (object[])values.Clone();

		public int IndexOf(object? obj)
			=> Array.IndexOf(values, obj);
	}
}
