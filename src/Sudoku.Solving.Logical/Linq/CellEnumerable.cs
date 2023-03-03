﻿namespace Sudoku.Linq;

/// <summary>
/// Provides with extension methods on <see cref="CellMap"/> type, around LINQ.
/// </summary>
/// <seealso cref="CellMap"/>
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
	/// A <see cref="ImmutableArray{T}"/> of <typeparamref name="TResult"/> whose elements are the result
	/// of invoking the transform function on each element of <paramref name="source"/>.
	/// </returns>
	public static TResult[] Select<TResult>(this scoped in CellMap source, Func<int, TResult> selector) where TResult : unmanaged
	{
		var result = new TResult[source.Count];
		var i = 0;
		foreach (var cell in source)
		{
			result[i++] = selector(cell);
		}

		return result;
	}
}
