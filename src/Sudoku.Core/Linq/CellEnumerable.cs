namespace Sudoku.Linq;

/// <summary>
/// Provides with extension methods on <see cref="Cells"/> type, around LINQ.
/// </summary>
/// <seealso cref="Cells"/>
public static class CellEnumerable
{
	/// <summary>
	/// Projects each element of a sequence into a new form.
	/// </summary>
	/// <typeparam name="TResult">
	/// The type of the value returned by <paramref name="selector"/>.
	/// This type must be an <see langword="unmanaged"/> type in order to make optimization
	/// in the future release of C# versions.
	/// </typeparam>
	/// <param name="source">A sequence of values to invoke a transform function on.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <returns>
	/// A <see cref="ReadOnlySpan{T}"/> of <typeparamref name="TResult"/> whose elements are the result
	/// of invoking the transform function on each element of <paramref name="source"/>.
	/// </returns>
	public static ImmutableArray<TResult> Select<TResult>(this scoped in Cells source, Func<int, TResult> selector)
		where TResult : unmanaged
	{
		var result = new TResult[source.Count];
		int i = 0;
		foreach (int cell in source)
		{
			result[i++] = selector(cell);
		}

		return ImmutableArray.Create(result);
	}
}
