using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Common.Abstractions.Dates
{
	public static class DatesRegexPatterns
	{
		public static Regex DateRegex { get; } = new Regex(@"(?<y>\d{4})-(?<m>\d{2})-(?<d>\d{2})", RegexOptions.Compiled);
		public static Regex TimeRegex { get; } = new Regex(@"(?<h>\d{2})-(?<m>\d{2})-(?<s>\d{2})(.(?<micro>\d{1,6})){0,1}", RegexOptions.Compiled);
		public static Regex DateTimeRegex { get; } = new Regex($"(?<date>{DatesRegexPatterns.DateRegex})T(?<time>{DatesRegexPatterns.TimeRegex})", RegexOptions.Compiled);
	}
}
