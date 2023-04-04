using Chips.Runtime.Meta;

namespace Chips.Runtime.Types {
	/// <summary>
	/// A wrapper class around a <seealso cref="System.Text.RegularExpressions.Regex"/>
	/// </summary>
	public class Regex {
		public readonly string pattern;
		private readonly System.Text.RegularExpressions.Regex regex;

		//Optimize searching through the same string multiple times
		private string? lastPatternCheck;
		internal System.Text.RegularExpressions.MatchCollection? lastMatches;

		public Regex(string pattern) {
			this.pattern = pattern;
			regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.Compiled);
		}

		public void AttemptToMatch(string str) {
			if (str is null)
				return;

			if (str == lastPatternCheck)
				return;

			lastPatternCheck = str;

			//Perform the match
			lastMatches = regex.Matches(str);

			//Accessing the Count property will force the collection to update
			if (lastMatches.Count > 0)
				Metadata.Flags.RegexSuccess = true;
		}

		public string? GetMatchString(int index) {
			//null == invalid state or no matches found
			if (lastPatternCheck is null || lastMatches is null || lastMatches.Count == 0)
				return null;

			return lastMatches[index].Value;
		}

		public string ReplaceString(string original, string replace) {
			if (lastPatternCheck is null || lastMatches is null || lastMatches.Count == 0)
				return original;

			return regex.Replace(original, replace);
		}

		public override string ToString() => $"Regex: \"{pattern}\"";
	}
}
