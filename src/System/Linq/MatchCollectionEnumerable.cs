namespace System.Text.RegularExpressions;

/// <summary>
/// Provides LINQ-based extension methods on <see cref="MatchCollection"/>.
/// </summary>
/// <seealso cref="MatchCollection"/>
public static class MatchCollectionEnumerable
{
	/// <summary>
	/// The select method used in <see langword="from"/>-<see langword="in"/>-<see langword="select"/>
	/// clause.
	/// </summary>
	/// <typeparam name="TResult">The result type.</typeparam>
	/// <param name="this">The list.</param>
	/// <param name="selector">The selector that is used for conversion.</param>
	/// <returns>The array of target result elements.</returns>
	public static TResult[] Select<TResult>(this MatchCollection @this, Func<Match, TResult> selector)
	{
		var result = new TResult[@this.Count];
		int i = 0;
		foreach (Match element in @this)
		{
			result[i++] = selector(element);
		}

		return result;
	}
}
