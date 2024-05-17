namespace System.Text.RegularExpressions;

/// <summary>
/// Provides LINQ-based extension methods on <see cref="MatchCollection"/>.
/// </summary>
/// <seealso cref="MatchCollection"/>
public static class MatchCollectionEnumerable
{
	/// <inheritdoc cref="SpanEnumerable.Select{T, TResult}(ReadOnlySpan{T}, Func{T, TResult})"/>
	public static ReadOnlySpan<TResult> Select<TResult>(this MatchCollection @this, Func<Match, TResult> selector)
	{
		var result = new TResult[@this.Count];
		var i = 0;
		foreach (Match element in @this)
		{
			result[i++] = selector(element);
		}
		return result;
	}
}
