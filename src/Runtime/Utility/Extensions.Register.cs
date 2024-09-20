using Chips.Runtime.Meta;

namespace Chips.Runtime.Utility {
	partial class Extensions {
		public static string GetName(this Register register) {
			string name = register.ToString().Replace("_", string.Empty);
			return $"${name}";
		}
	}
}
