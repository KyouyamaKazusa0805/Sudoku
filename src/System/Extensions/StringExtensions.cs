namespace System;

/// <summary>
/// Provides extension methods on <see cref="string"/>.
/// </summary>
/// <seealso cref="string"/>
public static unsafe class StringExtensions
{
	/// <summary>
	/// Indicates the regular expression to match all null lines and header spaces in their lines.
	/// </summary>
	[IsRegex]
	private const string NullLinesOrHeaderSpaces = @"(^\s*|(?<=\r\n)\s+)";


	/// <summary>
	/// Indicates the time span that is used for matching.
	/// </summary>
	private static readonly TimeSpan MatchingTimeSpan = TimeSpan.FromSeconds(5);

	/// <summary>
	/// Indicates the exception that will be thrown when a certain regular expression is invalid.
	/// </summary>
	private static readonly InvalidOperationException InvalidOperation =
		new("The specified regular expression pattern is invalid.");


	/// <summary>
	/// Count how many specified characters are in the current string.
	/// </summary>
	/// <param name="this">The current string.</param>
	/// <param name="character">The character to count.</param>
	/// <returns>The number of characters found.</returns>
	public static int CountOf(this string @this, char character)
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
	/// <exception cref="InvalidOperationException">
	/// Throws when the specified <paramref name="pattern"/> is not a valid regular
	/// expression pattern.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool SatisfyPattern(this string @this, [NotNullWhen(true)] string? pattern) =>
		pattern?.IsRegexPattern() ?? false ? @this.Match(pattern) == @this : throw InvalidOperation;

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
	/// <exception cref="InvalidOperationException">
	/// Throws when the specified <paramref name="pattern"/> is not a valid regular
	/// expression pattern.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsMatch(this string @this, string pattern) =>
		pattern.IsRegexPattern()
			? Regex.IsMatch(@this, pattern, RegexOptions.ExplicitCapture, MatchingTimeSpan)
			: throw InvalidOperation;

	/// <summary>
	/// Replace the character at the specified index with the new value.
	/// </summary>
	/// <param name="this">The current string.</param>
	/// <param name="index">The index.</param>
	/// <param name="charToInsert">The string to insert.</param>
	/// <returns>The result string.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ReplaceAt(this string @this, int index, char charToInsert)
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
	/// <exception cref="InvalidOperationException">
	/// Throws when the specified <paramref name="pattern"/> is not a valid regular
	/// expression pattern.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? Match(this string @this, string pattern) =>
		pattern.IsRegexPattern() ? @this.Match(pattern, RegexOptions.None) : throw InvalidOperation;

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
	/// <exception cref="InvalidOperationException">
	/// Throws when the specified <paramref name="pattern"/> is not a valid regular
	/// expression pattern.
	/// </exception>
	/// <seealso cref="Regex.Match(string, string, RegexOptions)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? Match(this string @this, string pattern, RegexOptions regexOption) =>
		pattern.IsRegexPattern()
			? Regex.Match(@this, pattern, regexOption, MatchingTimeSpan) is { Success: true, Value: var value }
				? value
				: null
			: throw InvalidOperation;

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
	/// <exception cref="InvalidOperationException">
	/// Throws when the specified <paramref name="pattern"/> is not a valid regular
	/// expression pattern.
	/// </exception>
	/// <seealso cref="Regex.Matches(string, string)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string[] MatchAll(this string @this, string pattern) =>
		pattern.IsRegexPattern() ? @this.MatchAll(pattern, RegexOptions.None) : throw InvalidOperation;

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
	/// <exception cref="InvalidOperationException">
	/// Throws when the specified <paramref name="pattern"/> is not a valid regular
	/// expression pattern.
	/// </exception>
	/// <seealso cref="Regex.Matches(string, string, RegexOptions)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string[] MatchAll(this string @this, string pattern, RegexOptions regexOption) =>
		pattern.IsRegexPattern() ? (
			from Match m in Regex.Matches(@this, pattern, regexOption, MatchingTimeSpan)
			select m.Value
		).ToArray() : throw InvalidOperation;

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
	/// will only contain the letters, digits or the underscore character '<c>_</c>'
	/// (i.e. <c>"Helloworld"</c> as the result of this example).
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	/// Throws when the <paramref name="reservePattern"/> is invalid.
	/// All possible patterns are shown in the tip for the parameter <paramref name="reservePattern"/>.
	/// </exception>
	public static string Reserve(this string @this, string reservePattern)
	{
		delegate*<char, bool> predicate = reservePattern switch
		{
			@"\d" => &char.IsDigit,
			@"\t" => &isTab,
			@"\w" => &isLetterDigitOrUnderscore,
			_ => throw InvalidOperation
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

		return new(ptr, 0, count);


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
}
