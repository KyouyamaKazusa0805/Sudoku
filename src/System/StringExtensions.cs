namespace System;

/// <summary>
/// Provides extension methods on <see cref="string"/>.
/// </summary>
/// <seealso cref="string"/>
public static partial class StringExtensions
{
	/// <summary>
	/// Removes all specified characters.
	/// </summary>
	/// <param name="this">The string value.</param>
	/// <param name="character">The character to be removed from the base string.</param>
	/// <returns>The result string value after removal.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string RemoveAll(this string @this, char character) => @this.Replace(character.ToString(), string.Empty);

	/// <summary>
	/// Gets a new <see cref="string"/>[] result, with each element (a <see cref="string"/> with a single character)
	/// from the specified <see cref="string"/>.
	/// </summary>
	/// <param name="this">The current <see cref="string"/> instance.</param>
	/// <returns>An array of <see cref="string"/> elements.</returns>
	public static ReadOnlySpan<string> ExpandCharacters(this string @this) => from c in @this.AsSpan() select c.ToString();

	/// <summary>
	/// Cut the array to multiple part, making them are all of length <paramref name="length"/>.
	/// </summary>
	/// <param name="this">The string text.</param>
	/// <param name="length">The desired length.</param>
	/// <returns>A list of <see cref="string"/> values.</returns>
	public static ReadOnlySpan<string> Chunk(this string @this, int length)
	{
		var result = new string[@this.Length % length == 0 ? @this.Length / length : @this.Length / length + 1];
		for (var i = 0; i < @this.Length / length; i++)
		{
			result[i] = @this.AsSpan().Slice(i * length, length).ToString();
		}
		return result;
	}

	/// <inheritdoc cref="string.Split(char[], StringSplitOptions)"/>
	/// <param name="this">The array itself.</param>
	/// <param name="separator">The separator characters.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<string> SplitBy(this string @this, params char[] separator)
		=> @this.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
