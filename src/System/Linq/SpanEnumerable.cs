namespace System.Linq;

/// <summary>
/// Provides LINQ-based extension methods on <see cref="Span{T}"/> and <see cref="ReadOnlySpan{T}"/>.
/// </summary>
/// <seealso cref="Span{T}"/>
/// <seealso cref="ReadOnlySpan{T}"/>
public static class SpanEnumerable
{
	/// <summary>
	/// The select method used in <see langword="from"/>-<see langword="in"/>-<see langword="select"/>
	/// clause.
	/// </summary>
	/// <typeparam name="T">The element type.</typeparam>
	/// <typeparam name="TResult">The result type.</typeparam>
	/// <param name="this">The list.</param>
	/// <param name="selector">The selector that is used for conversion.</param>
	/// <returns>The array of target result elements.</returns>
	public static TResult[] Select<T, TResult>(this in Span<T> @this, Func<T, TResult> selector)
	{
		var result = new TResult[@this.Length];
		int i = 0;
		foreach (var element in @this)
		{
			result[i++] = selector(element);
		}

		return result;
	}

	/// <summary>
	/// The select method used in <see langword="from"/>-<see langword="in"/>-<see langword="select"/>
	/// clause.
	/// </summary>
	/// <typeparam name="T">The element type.</typeparam>
	/// <typeparam name="TResult">The result type.</typeparam>
	/// <param name="this">The list.</param>
	/// <param name="selector">The selector that is used for conversion.</param>
	/// <returns>The array of target result elements.</returns>
	public static TResult[] Select<T, TResult>(this in ReadOnlySpan<T> @this, Func<T, TResult> selector)
	{
		var result = new TResult[@this.Length];
		int i = 0;
		foreach (var element in @this)
		{
			result[i++] = selector(element);
		}

		return result;
	}
}
