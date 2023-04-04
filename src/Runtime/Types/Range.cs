using System;

namespace Chips.Runtime.Types {
	public struct Range {
		public readonly int start, end;

		public Range(int start, int end) {
			this.start = start;
			this.end = end;

			if (start > end)
				throw new ArgumentException("Range start must be less than or equal to range end", nameof(start));
		}

		public static unsafe explicit operator int[](Range range) {
			int stride = range.end - range.start;
			if (stride == 0)
				return Array.Empty<int>();

			int[] ret = new int[stride];
			fixed (int* ptr = ret) {
				int* nfPtr = ptr;
				for (int i = 0; i < stride; i++, nfPtr++)
					*nfPtr = range.start + i;
			}

			return ret;
		}

		public override string ToString() => $"[{start}..{end}]";

		public static bool TryParse(string? str, out Range range) {
			if (str is null) {
				range = default;
				return false;
			}

			ReadOnlySpan<char> span = str.AsSpan();
			//Length of >= 6 accounts for the punctuation in "[X..Y]"
			if (span.Length >= 6 && span[0] == '[' && span[^1] == ']') {
				int rangeIndex = span.IndexOf("..", StringComparison.CurrentCulture);
				int rangeIndexEnd = rangeIndex + 2;
				if (rangeIndex > 2 && int.TryParse(span[1..rangeIndex], out int start) && int.TryParse(span[rangeIndexEnd..^1], out int end)) {
					//The string is valid
					range = new(start, end);
					return true;
				}
			}

			//The string is not valid
			range = default;
			return false;
		}
	}
}
