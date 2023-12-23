namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides with extension methods on <see cref="string"/>.
/// </summary>
/// <seealso cref="string"/>
internal static class StringExtensions
{
	/// <summary>
	/// Converts the specified string value as the camel-casing.
	/// </summary>
	/// <param name="this">The string.</param>
	/// <returns>The converted string value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToCamelCase(this string @this)
		=> @this switch
		{
			[] => string.Empty,
			[var c and >= 'A' and <= 'Z', .. var otherChars] => $"{(char)(c + ' ')}{otherChars}",
			[>= 'a' and <= 'z', ..] => @this,
			[var c, .. var otherChars] => $"{c}{otherChars.ToCamelCase()}"
		};

	/// <inheritdoc cref="SyntaxFacts.IsKeywordKind(SyntaxKind)"/>
	/// <param name="this">The string.</param>
	public static bool IsKeyword(this string @this)
	{
		try
		{
			return SyntaxFacts.IsKeywordKind((SyntaxKind)Enum.Parse(typeof(SyntaxKind), $"{@this}Keyword", true)!);
		}
		catch (ArgumentException)
		{
			// Parsing failed.
			return false;
		}
	}
}
