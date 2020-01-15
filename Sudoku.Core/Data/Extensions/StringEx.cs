using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using Sudoku.Diagnostics.CodeAnalysis;

namespace Sudoku.Data.Extensions
{
	public static class StringEx
	{
		public static bool SatisfyPattern(this string @this, [Pattern] string pattern)
		{
			Contract.Assume(pattern.IsRegexPattern());

			string? match = @this.Match(pattern);
			return !(match is null) && match == @this;
		}

		public static bool IsMatch(this string @this, [Pattern] string pattern)
		{
			Contract.Assume(pattern.IsRegexPattern());

			return Regex.IsMatch(@this, pattern);
		}

		public static string? Match(this string @this, [Pattern] string pattern) =>
			@this.Match(pattern, RegexOptions.None);

		public static string? Match(
			this string @this, [Pattern] string pattern, RegexOptions regexOption)
		{
			Contract.Assume(pattern.IsRegexPattern());

			var match = Regex.Match(@this, pattern, regexOption);
			return match.Success ? match.Value : null;
		}

		public static string[] MatchAll(this string @this, [Pattern] string pattern) =>
			@this.MatchAll(pattern, RegexOptions.None);

		public static string[] MatchAll(
			this string @this, [Pattern] string pattern, RegexOptions regexOption)
		{
			Contract.Assume(pattern.IsRegexPattern());

			var matches = Regex.Matches(@this, pattern, regexOption);
			var result = new List<string>();
			foreach (Match? match in matches) // Do not use 'var' ('var' is 'object?').
			{
				if (!(match is null))
				{
					result.Add(match.Value);
				}
			}

			return result.ToArray();
		}

		private static bool IsRegexPattern(this string @this)
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
