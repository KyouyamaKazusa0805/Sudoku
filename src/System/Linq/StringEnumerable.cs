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
		for (var index = 0; index < @this.Length; index += maxLength)
		{
			yield return @this.Substring(index, Min(maxLength, @this.Length - index));
		}
	}

	/// <summary>
	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})" path="/summary"/>
	/// </summary>
	/// <param name="this">The string instance.</param>
	/// <param name="selector">
	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})" path="/param[@name='selector']"/>
	/// </param>
	/// <returns>
	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})" path="/returns"/>
	/// </returns>
	public static ReadOnlySpan<TResult> Select<TResult>(this string @this, Func<char, TResult> selector)
	{
		var result = new TResult[@this.Length];
		var i = 0;
		foreach (var element in @this)
		{
			result[i++] = selector(element);
		}

		return result;
	}
}
