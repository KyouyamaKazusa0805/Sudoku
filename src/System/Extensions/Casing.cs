namespace System;

/// <summary>
/// Provides a set of methods that handles for a <see cref="string"/>, to convert the value
/// into the specified case.
/// </summary>
public static class Casing
{
	/// <summary>
	/// Defines a regular expression pattern that matches for an identifier based on UTF-8 format.
	/// </summary>
	private static readonly Regex Utf8IdentifierPattern =
		new(
			@"[A-Za-z_]\w*",
			RegexOptions.Compiled,
			TimeSpan.FromSeconds(5)
		);

	/// <summary>
	/// Checks whether the specified string value is a valid identifier name.
	/// </summary>
	/// <param name="str">The string to check.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsValidIdentifier(string str) => Utf8IdentifierPattern.IsMatch(str);

	/// <summary>
	/// Converts the current string identifier into the camel-casing (like <c>camelCasingVariable</c>).
	/// If the first character is the underscore, the return value will remove the underscore,
	/// and treat the last characters as the identifier and converts into the camel-casing.
	/// </summary>
	/// <param name="str">The string to convert.</param>
	/// <returns>The result string.</returns>
	/// <exception cref="ArgumentException">
	/// Throws when the argument <paramref name="str"/> is not an identifier.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToCamelCase(string str) =>
		!IsValidIdentifier(str)
			? throw new ArgumentException("The specified string is not an identifier.", nameof(str))
			: str switch
			{
				[>= 'a' and <= 'z', ..] => str,
				[var firstChar and >= 'A' and <= 'Z', .. var otherChars] => $"{firstChar}{otherChars}",
				['_', .. var otherChars] => ToCamelCase(otherChars)
			};

	/// <summary>
	/// Converts the current string identifier into the pascal-casing (like <c>PascalCasingVariable</c>).
	/// If the first character is the underscore, the return value will remove the underscore,
	/// and treat the last characters as the identifier and converts into the pascal-casing.
	/// </summary>
	/// <param name="str">The string to convert.</param>
	/// <returns>The result string.</returns>
	/// <exception cref="ArgumentException">
	/// Throws when the argument <paramref name="str"/> is not an identifier.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToPascalCase(string str) =>
		!IsValidIdentifier(str)
			? throw new ArgumentException("The specified string is not an identifier.", nameof(str))
			: str switch
			{
				[var firstChar and >= 'a' and <= 'z', .. var otherChars] => $"{firstChar}{otherChars}",
				[>= 'A' and <= 'Z', ..] => str,
				['_', .. var otherChars] => ToCamelCase(otherChars)
			};
}
