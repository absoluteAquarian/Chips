using System;
using System.Runtime.CompilerServices;

namespace Chips.Runtime.Types {
	public struct Indexer {
		public readonly int value;

		public Indexer(int value) {
			this.value = value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int ConvertToInteger(int collectionSize)
			=> value >= 0 ? value : collectionSize + value;

		public override string ToString() => value >= 0 ? value.ToString() : $"^{-value}";

		public static bool TryParse(string? str, out Indexer indexer) {
			if (uint.TryParse(str, out uint typedResult) && typedResult <= int.MaxValue) {
				indexer = new((int)typedResult);
				return true;
			}

			ReadOnlySpan<char> span = str.AsSpan();

			if (str is not null && str.Length > 1 && span[0] == '^' && uint.TryParse(span[1..], out typedResult) && typedResult <= int.MaxValue - 1) {
				indexer = new(-(int)typedResult);
				return true;
			}

			indexer = default;
			return false;
		}
	}
}
