using System;
using System.Collections;
using System.Collections.Generic;

namespace Chips.Runtime.Specifications {
	public sealed class OpcodeArgumentCollection : IEnumerable<object?> {
		private object?[] args = Array.Empty<object?>();
		private int _count;
		private int _version;

		public ref object? this[int index] => ref args[index];

		public int Count => _count;

		public OpcodeArgumentCollection Add(object? argument) {
			EnsureCapacity(_count + 1);
			args[_count++] = argument;
			_version++;
			return this;
		}

		public OpcodeArgumentCollection AddRange(params object?[] arguments) {
			EnsureCapacity(_count + arguments.Length);
			foreach (var argument in arguments)
				args[_count++] = argument;
			_version++;
			return this;
		}

		public void Clear() {
			_count = 0;
			_version++;
		}

		private void EnsureCapacity(int capacity) {
			if (args.Length >= capacity)
				return;

			int length = Math.Max(1, args.Length);
			while (length < capacity)
				length *= 2;

			Array.Resize(ref args, length);
		}

		public IEnumerator<object?> GetEnumerator() => new Enumerator(this);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public struct Enumerator : IEnumerator<object?> {
			private OpcodeArgumentCollection _collection;
			private object? _current;
			private int _version;
			private int _index;

			public object? Current => _current;

			internal Enumerator(OpcodeArgumentCollection collection) {
				_collection = collection;
				_version = collection._version;
				_index = 0;
				_current = null!;
			}

			public bool MoveNext() {
				if (_version != _collection._version)
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");

				if (_index < _collection._count) {
					_current = _collection.args[_index];
					_index++;
					return true;
				}

				_current = null!;
				return false;
			}

			public void Reset() {
				if (_version != _collection._version)
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");

				_index = 0;
				_current = null!;
			}

			public void Dispose() {
				_collection = null!;
				_current = null!;
				_index = -1;
			}
		}
	}
}
