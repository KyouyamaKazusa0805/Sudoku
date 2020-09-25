using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="string"/>.
	/// </summary>
	/// <seealso cref="string"/>
	public static class StringEx
	{
		/// <summary>
		/// Check whether the specified string instance is satisfied
		/// the specified regular expression pattern or not.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value to check.</param>
		/// <param name="pattern">The regular expression pattern.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		/// <exception cref="InvalidRegexStringException">
		/// Throws when the specified <paramref name="pattern"/> is not an valid regular
		/// expression pattern.
		/// </exception>
		public static bool SatisfyPattern(this string @this, string pattern) =>
			pattern.IsRegexPattern()
				? @this.Match(pattern) == @this
				: throw new InvalidRegexStringException();

		/// <summary>
		/// Check whether the specified string instance can match the value
		/// using the specified regular expression pattern or not.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value to match.</param>
		/// <param name="pattern">The regular expression pattern.</param>
		/// <returns>A <see cref="bool"/> indicating that.</returns>
		/// <remarks>
		/// This method is a syntactic sugar of the calling
		/// method <see cref="Regex.IsMatch(string, string)"/>.
		/// </remarks>
		/// <seealso cref="Regex.IsMatch(string, string)"/>
		/// <exception cref="InvalidRegexStringException">
		/// Throws when the specified <paramref name="pattern"/> is not an valid regular
		/// expression pattern.
		/// </exception>
		public static bool IsMatch(this string @this, string pattern) =>
			pattern.IsRegexPattern()
				? Regex.IsMatch(@this, pattern)
				: throw new InvalidRegexStringException();

		/// <summary>
		/// Searches the specified input string for the first occurrence of
		/// the specified regular expression pattern.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value to match.</param>
		/// <param name="pattern">The regular expression pattern.</param>
		/// <returns>
		/// The value after matching. If failed to match,
		/// the value will be <see langword="null"/>.
		/// </returns>
		/// <remarks>
		/// This method is a syntactic sugar of the calling
		/// method <see cref="Regex.Match(string, string)"/>.
		/// </remarks>
		/// <seealso cref="Regex.Match(string, string)"/>
		/// <exception cref="InvalidRegexStringException">
		/// Throws when the specified <paramref name="pattern"/> is not an valid regular
		/// expression pattern.
		/// </exception>
		public static string? Match(this string @this, string pattern) =>
			pattern.IsRegexPattern()
				? @this.Match(pattern, RegexOptions.None)
				: throw new InvalidRegexStringException();

		/// <summary>
		/// Searches the input string for the first occurrence of the specified regular
		/// expression, using the specified matching options.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value to match.</param>
		/// <param name="pattern">The regular expression pattern.</param>
		/// <param name="regexOption">The matching options.</param>
		/// <returns>
		/// The matched string value. If failed to match,
		/// the value will be <see langword="null"/>.
		/// </returns>
		/// <remarks>
		/// This method is a syntactic sugar of the calling
		/// method <see cref="Regex.Match(string, string, RegexOptions)"/>.
		/// </remarks>
		/// <exception cref="InvalidRegexStringException">
		/// Throws when the specified <paramref name="pattern"/> is not an valid regular
		/// expression pattern.
		/// </exception>
		/// <seealso cref="Regex.Match(string, string, RegexOptions)"/>
		public static string? Match(this string @this, string pattern, RegexOptions regexOption)
		{
			if (!pattern.IsRegexPattern())
			{
				throw new InvalidRegexStringException();
			}

			var match = Regex.Match(@this, pattern, regexOption);
			return match.Success ? match.Value : null;
		}

		/// <summary>
		/// Searches the specified input string for all occurrences of a
		/// specified regular expression pattern.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value to match.</param>
		/// <param name="pattern">The regular expression pattern.</param>
		/// <returns>
		/// The result after matching. If failed to match,
		/// the returning array will be an empty string array (has no elements).
		/// </returns>
		/// <remarks>
		/// This method is a syntactic sugar of the calling
		/// method <see cref="Regex.Matches(string, string)"/>.
		/// </remarks>
		/// <exception cref="InvalidRegexStringException">
		/// Throws when the specified <paramref name="pattern"/> is not an valid regular
		/// expression pattern.
		/// </exception>
		/// <seealso cref="Regex.Matches(string, string)"/>
		public static string[] MatchAll(this string @this, string pattern) =>
			pattern.IsRegexPattern()
				? @this.MatchAll(pattern, RegexOptions.None)
				: throw new InvalidRegexStringException();

		/// <summary>
		/// Searches the specified input string for all occurrences of a
		/// specified regular expression pattern, using the specified matching
		/// options.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value to match.</param>
		/// <param name="pattern">The regular expression pattern.</param>
		/// <param name="regexOption">The matching options.</param>
		/// <returns>
		/// The result after matching. If failed to match,
		/// the returning array will be an empty string array (has no elements).
		/// </returns>
		/// <remarks>
		/// This method is a syntactic sugar of the calling
		/// method <see cref="Regex.Matches(string, string, RegexOptions)"/>.
		/// </remarks>
		/// <exception cref="InvalidRegexStringException">
		/// Throws when the specified <paramref name="pattern"/> is not an valid regular
		/// expression pattern.
		/// </exception>
		/// <seealso cref="Regex.Matches(string, string, RegexOptions)"/>
		public static string[] MatchAll(this string @this, string pattern, RegexOptions regexOption)
		{
			if (!pattern.IsRegexPattern())
			{
				throw new InvalidRegexStringException();
			}

			var result = new List<string>();
			foreach (Match? match in Regex.Matches(@this, pattern, regexOption)) // Do not use 'var' ('var' is 'object?').
			{
				if (match is not null)
				{
					result.Add(match.Value);
				}
			}

			return result.ToArray();
		}

		/// <summary>
		/// Reserve all characters that satisfy the specified pattern.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The string.</param>
		/// <param name="reservePattern">The pattern that reserved characters satisfied.</param>
		/// <returns>The result string.</returns>
		public static string Reserve(this string @this, string reservePattern)
		{
			var span = (stackalloc char[1]);

			var sb = new StringBuilder();
			foreach (char c in @this)
			{
				span[0] = c;
				if (span.ToString().SatisfyPattern(reservePattern))
				{
					sb.Append(c);
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// To check if the current string value is a valid regular
		/// expression pattern or not.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value to check.</param>
		/// <returns>A <see cref="bool"/> indicating that.</returns>
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
