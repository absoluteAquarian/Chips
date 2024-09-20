using System;
using System.Collections.Generic;

namespace Chips.Common.Utility {
	partial class Extensions {
		public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> enumerator, bool resetOnCompletion = false) {
			if (enumerator is null)
				throw new ArgumentNullException(nameof(enumerator));

			while (enumerator.MoveNext())
				yield return enumerator.Current;

			if (resetOnCompletion)
				enumerator.Reset();
		}
	}
}
