namespace System;

/// <summary>
/// Provides extension methods on <see cref="string"/>.
/// </summary>
/// <seealso cref="string"/>
internal static class StringExtensions
{
	/// <summary>
	/// Converts the current string into the camel case.
	/// </summary>
	/// <param name="this">The string.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToCamelCase(this string @this)
	{
		@this = @this.TrimStart('_');
		return $"{char.ToLowerInvariant(@this[0])}{@this[1..]}";
	}
}
