namespace System.Linq;

/// <summary>
/// Provides with LINQ methods on a <see cref="string"/> value.
/// </summary>
public static class StringEnumerable
{
	/// <summary>
	/// Splits the specified string value into multiple parts, with each part being a same length.
	/// </summary>
	/// <param name="this">The string to be split.</param>
	/// <param name="maxLength">The length to be split.</param>
	/// <returns>An <see cref="IEnumerable{T}"/> instance with multiple parts of the string.</returns>
	public static IEnumerable<string> SplitByLength(this string @this, int maxLength)
	{
		for (int index = 0; index < @this.Length; index += maxLength)
		{
			yield return @this.Substring(index, Min(maxLength, @this.Length - index));
		}
	}
}
