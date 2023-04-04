using Chips.Runtime.Types;
using System;
using System.Numerics;
using System.Text;

namespace Chips.Runtime.Utility {
	public static class Formatting {
		public static string FormatArray(Array? a) {
			if (a is null)
				return "null";

			if (a.Length == 0)
				return "[ <empty> ]";

			if (a.Length == 1)
				return $"[ {FormatObject(a.GetValue(0))} ]";

			StringBuilder ret = new("[ ");

			for (int i = 0; i < a.Length; i++) {
				var elem = a.GetValue(i);

				if (i > 0)
					ret.Append(", ");

				ret.Append(FormatObject(elem));
			}
			ret.Append(" ]");

			return ret.ToString();
		}

		public static string FormatObject(object? o)
			=> o switch {
				string s => $"\"{s}\"",
				char c => $"'{c}'",
				Array a => FormatArray(a),
				Register r => $"&{r.name}",
				Complex c => double.IsNaN(c.Real) || double.IsNaN(c.Imaginary)
					? "NaN"
					: (double.IsInfinity(c.Real) || double.IsInfinity(c.Imaginary)
						? "Infinity"
						: $"{c.Real} + {c.Imaginary}i"),
				null => "null",
				_ => o.ToString()
			} ?? "null";  //Null-coalescing operator needed to prevent a warning
	}
}
