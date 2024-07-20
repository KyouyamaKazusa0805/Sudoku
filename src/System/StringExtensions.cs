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



	[GeneratedRegex("""(^\s*|(?<=\r\n)\s+)""", RegexOptions.Compiled, 5000)]
	internal static partial Regex NullLinesOrHeaderSpacesPattern { get; }


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
		=> pattern?.IsRegexPattern() ?? false
			? @this.Match(pattern) == @this
			: throw new InvalidOperationException(SR.ExceptionMessage("StringIsInvalidRegex"));

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
		=> pattern.IsRegexPattern()
			? Regex.IsMatch(@this, pattern, RegexOptions.ExplicitCapture, MatchingTimeSpan)
			: throw new InvalidOperationException(SR.ExceptionMessage("StringIsInvalidRegex"));

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
	public static ref char MutableRef(this string @this) => ref @ref.AsMutableRef(in @this.Ref());

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
		=> pattern.IsRegexPattern()
			? @this.Match(pattern, RegexOptions.None)
			: throw new InvalidOperationException(SR.ExceptionMessage("StringIsInvalidRegex"));

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
			: throw new InvalidOperationException(SR.ExceptionMessage("StringIsInvalidRegex"));

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
	public static string[] Chunk(this string @this, int length)
	{
		var result = new string[@this.Length % length == 0 ? @this.Length / length : @this.Length / length + 1];
		for (var i = 0; i < @this.Length / length; i++)
		{
			result[i] = @this.AsSpan().Slice(i * length, length).ToString();
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
		=> pattern.IsRegexPattern()
			? @this.MatchAll(pattern, RegexOptions.None)
			: throw new InvalidOperationException(SR.ExceptionMessage("StringIsInvalidRegex"));

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
			: throw new InvalidOperationException(SR.ExceptionMessage("StringIsInvalidRegex"));

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
	/// Replace the string by using the specified regular expression.
	/// </summary>
	/// <param name="this">The base string.</param>
	/// <param name="pattern">A valid regular expression pattern to be used.</param>
	/// <param name="evaluator">
	/// The evaluator method that replace all strings inside <paramref name="this"/>
	/// satisfying the specified <paramref name="pattern"/>, with the target string which this method will return.
	/// </param>
	/// <returns>The final replaced string.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ReplaceRegex(this string @this, [StringSyntax(StringSyntaxAttribute.Regex)] string pattern, MatchEvaluator evaluator)
		=> Regex.Replace(@this, pattern, evaluator);

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
}
