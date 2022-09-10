namespace Sudoku.Linq;

/// <summary>
/// Provides with extension methods on <see cref="Grid"/> type, around LINQ.
/// </summary>
/// <seealso cref="Grid"/>
public static class GridEnumerable
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
	/// A <see cref="ImmutableArray{T}"/> of <typeparamref name="TResult"/> whose elements are the result
	/// of invoking the transform function on each element of <paramref name="source"/>.
	/// </returns>
	public static ImmutableArray<TResult> Select<TResult>(this scoped in Grid source, Func<int, TResult> selector)
	{
		var result = new TResult[81];
		int i = 0;
		foreach (int candidate in source)
		{
			result[i++] = selector(candidate);
		}

		return ImmutableArray.Create(result);
	}
}
