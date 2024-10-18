namespace System.Text.RegularExpressions;

/// <summary>
/// Provides with extension methods on <see cref="string"/> instances, with regular expression handling.
/// </summary>
public static class StringRegexExtensions
{
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
			? Regex.IsMatch(@this, pattern, RegexOptions.ExplicitCapture)
			: throw new InvalidOperationException(SR.ExceptionMessage("StringIsInvalidRegex"));

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
			? Regex.Match(@this, pattern, regexOption) is { Success: true, Value: var value } ? value : null
			: throw new InvalidOperationException(SR.ExceptionMessage("StringIsInvalidRegex"));

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
			? (from m in Regex.Matches(@this, pattern, regexOption) select m.Value).ToArray()
			: throw new InvalidOperationException(SR.ExceptionMessage("StringIsInvalidRegex"));

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
			Regex.Match(string.Empty, @this, RegexOptions.ExplicitCapture);
			return true;
		}
		catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
		{
			return false;
		}
	}
}
