using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace System.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="string"/>.
	/// </summary>
	/// <seealso cref="string"/>
	public static class StringEx
	{
		/// <summary>
		/// Indicates the regular expression to match all null lines and header spaces in their lines.
		/// </summary>
		private const string NullLinesOrHeaderSpaces = @"(^\s*|(?<=\r\n)\s+)";


		/// <summary>
		/// Indicates the time span that is used for matching.
		/// </summary>
		private static readonly TimeSpan MatchingTimeSpan = TimeSpan.FromSeconds(5);


		/// <summary>
		/// Check whether the specified string instance is satisfied
		/// the specified regular expression pattern or not.
		/// </summary>
		/// <param name="this">The value to check.</param>
		/// <param name="pattern">The regular expression pattern.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		/// <exception cref="InvalidRegexStringException">
		/// Throws when the specified <paramref name="pattern"/> is not an valid regular
		/// expression pattern.
		/// </exception>
		public static bool SatisfyPattern(this string @this, string pattern) =>
			pattern.IsRegexPattern()
			? @this.Match(pattern) == @this
			: throw new InvalidRegexStringException { WrongRegexString = pattern };

		/// <summary>
		/// Check whether the specified string instance can match the value
		/// using the specified regular expression pattern or not.
		/// </summary>
		/// <param name="this">The value to match.</param>
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
			? Regex.IsMatch(@this, pattern, RegexOptions.ExplicitCapture, MatchingTimeSpan)
			: throw new InvalidRegexStringException { WrongRegexString = pattern };

		/// <summary>
		/// Searches the specified input string for the first occurrence of
		/// the specified regular expression pattern.
		/// </summary>
		/// <param name="this">The value to match.</param>
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
			: throw new InvalidRegexStringException { WrongRegexString = pattern };

		/// <summary>
		/// Searches the input string for the first occurrence of the specified regular
		/// expression, using the specified matching options.
		/// </summary>
		/// <param name="this">The value to match.</param>
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
				throw new InvalidRegexStringException { WrongRegexString = pattern };
			}

			var match = Regex.Match(@this, pattern, regexOption, MatchingTimeSpan);
			return match.Success ? match.Value : null;
		}

		/// <summary>
		/// Searches the specified input string for all occurrences of a
		/// specified regular expression pattern.
		/// </summary>
		/// <param name="this">The value to match.</param>
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
			: throw new InvalidRegexStringException { WrongRegexString = pattern };

		/// <summary>
		/// Searches the specified input string for all occurrences of a
		/// specified regular expression pattern, using the specified matching
		/// options.
		/// </summary>
		/// <param name="this">The value to match.</param>
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
				throw new InvalidRegexStringException { WrongRegexString = pattern };
			}

			var result = new List<string>();

			// Do not use 'var' ('var' is 'object?').
			foreach (Match match in Regex.Matches(@this, pattern, regexOption, MatchingTimeSpan))
			{
				result.Add(match.Value);
			}

			return result.ToArray();
		}

		/// <summary>
		/// Reserve all characters that satisfy the specified pattern.
		/// </summary>
		/// <param name="this">The string.</param>
		/// <param name="reservePattern">
		/// The pattern that reserved characters satisfied. All supported patterns are:
		/// <list type="table">
		/// <item>
		/// <term><c>@"\d"</c></term>
		/// <description>To match a digit.</description>
		/// </item>
		/// <item>
		/// <term><c>@"\t"</c></term>
		/// <description>To match a tab.</description>
		/// </item>
		/// <item>
		/// <term><c>@"\w"</c></term>
		/// <description>To match a letter, digit or underscore character <c>'_'</c>.</description>
		/// </item>
		/// </list>
		/// </param>
		/// <returns>The result string.</returns>
		/// <remarks>
		/// For example, if code is <c>"Hello, world!".Reserve(@"\w")</c>, the return value
		/// won't contain any punctuation marks (i.e. <c>"Helloworld"</c>).
		/// </remarks>
		/// <exception cref="InvalidRegexStringException">
		/// Throws when the <paramref name="reservePattern"/> is invalid (Please expand the description
		/// of the parameter <paramref name="reservePattern"/> to learn about all valid patterns).
		/// </exception>
		public static unsafe string Reserve(this string @this, string reservePattern)
		{
			delegate*<char, bool> predicate = reservePattern switch
			{
				@"\d" => &char.IsDigit,
				@"\t" => &isTab,
				@"\w" => &isLetterDigitOrUnderscore,
				_ => throw new InvalidRegexStringException("The current reserved pattern is invalid.")
			};

			char* ptr = stackalloc char[@this.Length];
			int count = 0;
			fixed (char* p = @this)
			{
				char* q = p;
				for (int i = 0; i < @this.Length; i++, q++)
				{
					if (predicate(*q))
					{
						ptr[count++] = *q;
					}
				}
			}

			return new string(ptr, 0, count);

			static bool isTab(char c) => c == '\t';
			static bool isLetterDigitOrUnderscore(char c) => c == '_' || char.IsLetterOrDigit(c);
		}

		/// <summary>
		/// To check if the current string value is a valid regular
		/// expression pattern or not.
		/// </summary>
		/// <param name="this">The value to check.</param>
		/// <returns>A <see cref="bool"/> indicating that.</returns>
		public static bool IsRegexPattern(this string @this)
		{
			try
			{
				Regex.Match(string.Empty, @this, RegexOptions.ExplicitCapture, MatchingTimeSpan);
				return true;
			}
			catch (ArgumentException)
			{
				return false;
			}
		}

		/// <summary>
		/// Trim all spaces when they started a new line, or null lines.
		/// </summary>
		/// <param name="this">The string.</param>
		/// <returns>The trimmed result.</returns>
		/// <remarks>
		/// Note that all null lines and header spaces are removed.
		/// </remarks>
		public static string TrimVerbatim(this string @this) =>
			Regex.Replace(
				@this, NullLinesOrHeaderSpaces,
				string.Empty, RegexOptions.ExplicitCapture, MatchingTimeSpan);

		/// <summary>
		/// Trim new-line characters from the tail of the string.
		/// </summary>
		/// <param name="this">The string.</param>
		/// <returns>The result.</returns>
		public static string TrimEndNewLine(this string @this) => @this.TrimEnd(new[] { '\r', '\n' });

		/// <summary>
		/// Split the string with the fixed characters (new line).
		/// </summary>
		/// <param name="this">The string.</param>
		/// <returns>The result.</returns>
		public static string[] SplitByNewLine(this string @this) =>
			@this.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
	}
}
