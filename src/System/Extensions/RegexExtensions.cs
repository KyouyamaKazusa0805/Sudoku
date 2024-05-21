namespace System.Text.RegularExpressions;

/// <summary>
/// Provides with extension methods on <see cref="Regex"/>.
/// </summary>
/// <seealso cref="Regex"/>
public static class RegexExtensions
{
	/// <inheritdoc cref="Regex.Match(string)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<char> ValueMatch(this Regex @this, string input)
	{
		var enumerator = @this.ValueMatches(input);
		return enumerator.MoveNext() ? enumerator.Current : default;
	}

	/// <inheritdoc cref="Regex.Matches(string)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ValueMatchEnumerator ValueMatches(this Regex @this, string input) => new(input, @this.EnumerateMatches(input));
}
