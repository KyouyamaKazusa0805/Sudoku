using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sudoku.Data.Extensions
{
	public static class StringEx
	{
		public static string? Match(this string @this, string pattern)
		{
			if (!pattern.IsRegexPattern())
				return null;

			var match = Regex.Match(@this, pattern);
			return match.Success ? match.Value : null;
		}

		public static string[] MatchAll(this string @this, string pattern)
		{
			if (!pattern.IsRegexPattern())
				return Array.Empty<string>();

			var matches = Regex.Matches(@this, pattern);
			var result = new List<string>();
			foreach (Match? match in matches) // Do not use 'var' ('var' is 'object?').
			{
				if (match is not null)
				{
					result.Add(match.Value);
				}
			}

			return result.ToArray();
		}

		public static bool IsRegexPattern(this string @this)
		{
			try
			{
				Regex.Match(string.Empty, @this);
				return true;
			}
			catch (ArgumentException)
			{
				return false;
			}
		}
	}
}
