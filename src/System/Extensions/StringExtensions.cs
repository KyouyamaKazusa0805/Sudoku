namespace System;

/// <summary>
/// Provides extension methods on <see cref="string"/>.
/// </summary>
/// <seealso cref="string"/>
public static partial class StringExtensions
{
	/// <summary>
	/// Indicates the time span that is used for matching.
	/// </summary>
	private static readonly TimeSpan MatchingTimeSpan = TimeSpan.FromSeconds(5);

	/// <summary>
	/// Indicates the exception that will be thrown when a certain regular expression is invalid.
	/// </summary>
	private static readonly InvalidOperationException InvalidOperation = new("The specified regular expression pattern is invalid.");


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
	public static bool SatisfyPattern(this string @this, [StringSyntax(StringSyntaxAttribute.Regex), NotNullWhen(true)] string? pattern)
		=> pattern?.IsRegexPattern() ?? false ? @this.Match(pattern) == @this : throw InvalidOperation;

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
	public static bool IsMatch(this string @this, [StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
		=> pattern.IsRegexPattern() ? Regex.IsMatch(@this, pattern, RegexOptions.ExplicitCapture, MatchingTimeSpan) : throw InvalidOperation;

	/// <summary>
	/// Try to get the reference to the first character from a string, and immutable by default.
	/// </summary>
	/// <param name="this">The string.</param>
	/// <returns>A read-only reference that points to the first character of the specified string.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref readonly char Ref(this string @this) => ref @this.AsSpan()[0];

	/// <summary>
	/// Try to get the reference to the first character of a string.
	/// </summary>
	/// <param name="this">The string.</param>
	/// <returns>The reference to the first element.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref char MutableRef(this string @this) => ref System.R.AsMutableRef(in @this.Ref());

	/// <summary>
	/// Removes all specified characters.
	/// </summary>
	/// <param name="this">The string value.</param>
	/// <param name="character">The character to be removed from the base string.</param>
	/// <returns>The result string value after removal.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string RemoveAll(this string @this, char character) => @this.Replace(character.ToString(), string.Empty);

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
	public static string? Match(this string @this, [StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
		=> pattern.IsRegexPattern() ? @this.Match(pattern, RegexOptions.None) : throw InvalidOperation;

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
	public static string? Match(this string @this, [StringSyntax(StringSyntaxAttribute.Regex, nameof(regexOption))] string pattern, RegexOptions regexOption)
		=> pattern.IsRegexPattern()
			? Regex.Match(@this, pattern, regexOption, MatchingTimeSpan) is { Success: true, Value: var value } ? value : null
			: throw InvalidOperation;

	/// <summary>
	/// Gets a new <see cref="string"/>[] result, with each element (a <see cref="string"/> with a single character)
	/// from the specified <see cref="string"/>.
	/// </summary>
	/// <param name="this">The current <see cref="string"/> instance.</param>
	/// <returns>An array of <see cref="string"/> elements.</returns>
	public static string[] ExpandCharacters(this string @this) => [.. from c in (ReadOnlySpan<char>)@this select c.ToString()];

	/// <summary>
	/// Cut the array to multiple part, making them are all of length <paramref name="length"/>.
	/// </summary>
	/// <param name="this">The string text.</param>
	/// <param name="length">The desired length.</param>
	/// <returns>A list of <see cref="string"/> values.</returns>
	public static string[] CutOfLength(this string @this, int length)
	{
		var result = new string[@this.Length % length == 0 ? @this.Length / length : @this.Length / length + 1];
		for (var i = 0; i < @this.Length / length; i++)
		{
			result[i] = @this[(i * length)..((i + 1) * length)];
		}

		return result;
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
	/// <exception cref="InvalidOperationException">
	/// Throws when the specified <paramref name="pattern"/> is not a valid regular
	/// expression pattern.
	/// </exception>
	/// <seealso cref="Regex.Matches(string, string)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string[] MatchAll(this string @this, [StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
		=> pattern.IsRegexPattern() ? @this.MatchAll(pattern, RegexOptions.None) : throw InvalidOperation;

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
	public static string[] MatchAll(this string @this, [StringSyntax(StringSyntaxAttribute.Regex, nameof(regexOption))] string pattern, RegexOptions regexOption)
		=> pattern.IsRegexPattern()
			? [.. from m in Regex.Matches(@this, pattern, regexOption, MatchingTimeSpan) select m.Value]
			: throw InvalidOperation;

	/// <inheritdoc cref="string.Split(char[], StringSplitOptions)"/>
	/// <param name="this">The array itself.</param>
	/// <param name="separator">The separator characters.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string[] SplitBy(this string @this, params char[] separator)
		=> @this.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

	/// <summary>
	/// Remove all new-line sequences in the current string.
	/// </summary>
	/// <param name="this">The current string.</param>
	/// <returns>
	/// A string whose contents match the current string, but with all new-line sequences replaced with <see cref="string.Empty"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string RemoveLineEndings(this string @this) => @this.ReplaceLineEndings(string.Empty);

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
	public static unsafe string Reserve(this string @this, [StringSyntax(StringSyntaxAttribute.Regex), ConstantExpected] string reservePattern)
	{
		static bool isTab(char c) => c == '\t';
		static bool isLetterDigitOrUnderscore(char c) => c == '_' || char.IsLetterOrDigit(c);

		//lang = regex
		var predicate = reservePattern switch
		{
			@"\d" => &char.IsDigit,
			@"\t" => &isTab,
			@"\w" => (delegate*<char, bool>)(&isLetterDigitOrUnderscore),
			_ => throw InvalidOperation
		};

		var length = @this.Length;
		var buffer = (stackalloc char[length]);
		var count = 0;
		ref var pointer = ref @this.MutableRef();
		for (var i = 0; i < length; i++, pointer = Unsafe.Add(ref pointer, 1))
		{
			if (predicate(pointer))
			{
				buffer[count++] = pointer;
			}
		}

		return new(buffer[..count]);
	}

	/// <summary>
	/// To check if the current string value is a valid regular
	/// expression pattern or not.
	/// </summary>
	/// <param name="this">The value to check.</param>
	/// <returns>A <see cref="bool"/> indicating that.</returns>
	public static bool IsRegexPattern([StringSyntax(StringSyntaxAttribute.Regex)] this string @this)
	{
		try
		{
			Regex.Match(string.Empty, @this, RegexOptions.ExplicitCapture, MatchingTimeSpan);
			return true;
		}
		catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
		{
			return false;
		}
	}

	[GeneratedRegex("""(^\s*|(?<=\r\n)\s+)""", RegexOptions.Compiled, 5000)]
	internal static partial Regex NullLinesOrHeaderSpacesPattern();
}
