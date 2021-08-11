using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace System
{
	/// <summary>
	/// Provides extension methods on <see cref="string"/>.
	/// </summary>
	/// <seealso cref="string"/>
	public static class StringExtensions
	{
		/// <summary>
		/// Indicates the regular expression to match all null lines and header spaces in their lines.
		/// </summary>
		[Regex]
		private const string NullLinesOrHeaderSpaces = @"(^\s*|(?<=\r\n)\s+)";


		/// <summary>
		/// Indicates the time span that is used for matching.
		/// </summary>
		private static readonly TimeSpan MatchingTimeSpan = TimeSpan.FromSeconds(5);


		/// <summary>
		/// Count how many specified characters are in the current string.
		/// </summary>
		/// <param name="this">The current string.</param>
		/// <param name="character">The character to count.</param>
		/// <returns>The number of characters found.</returns>
		public static unsafe int CountOf(this string @this, char character)
		{
			int count = 0;
			fixed (char* pThis = @this)
			{
				for (char* p = pThis; *p != '\0'; p++)
				{
					if (*p == character)
					{
						count++;
					}
				}
			}

			return count;
		}

		/// <summary>
		/// Check whether the specified string instance is satisfied
		/// the specified regular expression pattern or not.
		/// </summary>
		/// <param name="this">The value to check.</param>
		/// <param name="pattern">
		/// The regular expression pattern. If the value is <see langword="null"/>,
		/// the return value is always <see langword="false"/>.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		/// <exception cref="InvalidRegexStringException">
		/// Throws when the specified <paramref name="pattern"/> is not an valid regular
		/// expression pattern.
		/// </exception>
		public static bool SatisfyPattern(this string @this, [NotNullWhen(true)] string? pattern) =>
			pattern is not null && (
				pattern.IsRegexPattern()
				? @this.Match(pattern) == @this
				: throw new InvalidRegexStringException { WrongRegexString = pattern }
			);

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
		public static bool IsMatch(this string @this, string pattern) => pattern.IsRegexPattern()
			? Regex.IsMatch(@this, pattern, RegexOptions.ExplicitCapture, MatchingTimeSpan)
			: throw new InvalidRegexStringException { WrongRegexString = pattern };

		/// <summary>
		/// Replace the character at the specified index with the new value.
		/// </summary>
		/// <param name="this">The current string.</param>
		/// <param name="index">The index.</param>
		/// <param name="charToInsert">The string to insert.</param>
		/// <returns>The result string.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe string ReplaceAt(this string @this, int index, char charToInsert)
		{
			char* resultPtr = stackalloc char[@this.Length + 1];
			resultPtr[@this.Length] = '\0';
			fixed (char* pThis = @this)
			{
				Unsafe.CopyBlock(resultPtr, pThis, (uint)(sizeof(char) * @this.Length));
			}

			resultPtr[index] = charToInsert;

			return new(resultPtr);
		}

		/// <summary>
		/// Slices the current <see cref="string"/> instance, via the specified character.
		/// If the string contains that character, we'll slice the string to get the segment.
		/// </summary>
		/// <remarks>
		/// Please note that even if the character exists in the string, we won't get the segment
		/// containing that character.
		/// </remarks>
		/// <param name="this">The string.</param>
		/// <param name="ch">The character to check.</param>
		/// <returns>
		/// The slice result. If the character doesn't exist in the string,
		/// <see langword="null"/> will be returned.
		/// </returns>
		public static string? SliceViaCharacter(this string @this, char ch) =>
			@this.IndexOf(ch) is var i and not -1 ? @this[..i] : null;

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
		public static string? Match(this string @this, string pattern) => pattern.IsRegexPattern()
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
		public static string? Match(this string @this, string pattern, RegexOptions regexOption) =>
			pattern.IsRegexPattern()
			? Regex.Match(@this, pattern, regexOption, MatchingTimeSpan) is { Success: true, Value: var value }
			? value
			: null
			: throw new InvalidRegexStringException { WrongRegexString = pattern };

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
		public static string[] MatchAll(this string @this, string pattern) => pattern.IsRegexPattern()
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

			int length = @this.Length;
			char* ptr = stackalloc char[length];
			int count = 0;
			fixed (char* p = @this)
			{
				char* q = p;
				for (int i = 0; i < length; i++, q++)
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
		public static string TrimVerbatim(this string @this) => Regex.Replace(
			@this, NullLinesOrHeaderSpaces,
			string.Empty, RegexOptions.ExplicitCapture, MatchingTimeSpan
		);

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
