namespace Sudoku.Analytics.Linq;

/// <summary>
/// Provides with LINQ-based extension methods on <see cref="CellMap"/>.
/// </summary>
/// <seealso cref="CellMap"/>
public static class CellMapEnumerable
{
	/// <summary>
	/// Projects each element in <see cref="CellMap"/> instance into the target-typed <typeparamref name="TResult"/> array,
	/// using the specified function to convert.
	/// </summary>
	/// <typeparam name="TResult">The type of target value.</typeparam>
	/// <param name="this">The <see cref="CellMap"/> instance.</param>
	/// <param name="selector">The selector.</param>
	/// <returns>An array of <typeparamref name="TResult"/> elements.</returns>
	public static TResult[] Select<TResult>(this scoped in CellMap @this, Func<int, TResult> selector)
	{
		var result = new TResult[@this.Count];
		var i = 0;
		foreach (var element in @this)
		{
			result[i++] = selector(element);
		}

		return result;
	}
}
