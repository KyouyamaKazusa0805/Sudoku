namespace Sudoku.Linq;

/// <summary>
/// Represents a list of LINQ methods to be used for type <see cref="List{T}"/>.
/// </summary>
/// <seealso cref="List{T}"/>
public static class ListEnumerable
{
	/// <summary>
	/// Projects each element in a <see cref="List{T}"/> to a <see cref="CellMap"/>,
	/// flattens the resulting sequence into one sequence, and invokes a result selector function on each element therein.
	/// </summary>
	/// <typeparam name="TSource">The type of the elements in sourece.</typeparam>
	/// <typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam>
	/// <param name="this">A sequence of values to project.</param>
	/// <param name="collectionSelector">A transform function to apply to each element of the input <see cref="List{T}"/>.</param>
	/// <param name="resultSelector">A transform function to apply to each element of the intermediate <see cref="CellMap"/>.</param>
	/// <returns>
	/// A <see cref="ReadOnlySpan{T}"/> of <typeparamref name="TResult"/> whose elements are the result
	/// of invoking the one-to-many transform function <paramref name="collectionSelector"/> on each element of <paramref name="this"/>
	/// and then mapping each of those sequences and their corresponding source element to a result element.
	/// </returns>
	public static ReadOnlySpan<TResult> SelectMany<TSource, TResult>(
		this List<TSource> @this,
		Func<TSource, CellMap> collectionSelector,
		Func<TSource, Cell, TResult> resultSelector
	)
	{
		var result = new List<TResult>(@this.Count << 1);
		foreach (var element in @this)
		{
			foreach (var cell in collectionSelector(element))
			{
				result.Add(resultSelector(element, cell));
			}
		}

		return CollectionsMarshal.AsSpan(result);
	}

	/// <summary>
	/// Projects each element in a <see cref="List{T}"/> to a <see cref="CandidateMap"/>,
	/// flattens the resulting sequence into one sequence, and invokes a result selector function on each element therein.
	/// </summary>
	/// <typeparam name="TSource">The type of the elements in sourece.</typeparam>
	/// <typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam>
	/// <param name="this">A sequence of values to project.</param>
	/// <param name="collectionSelector">A transform function to apply to each element of the input <see cref="List{T}"/>.</param>
	/// <param name="resultSelector">A transform function to apply to each element of the intermediate <see cref="CandidateMap"/>.</param>
	/// <returns>
	/// A <see cref="ReadOnlySpan{T}"/> of <typeparamref name="TResult"/> whose elements are the result
	/// of invoking the one-to-many transform function <paramref name="collectionSelector"/> on each element of <paramref name="this"/>
	/// and then mapping each of those sequences and their corresponding source element to a result element.
	/// </returns>
	public static ReadOnlySpan<TResult> SelectMany<TSource, TResult>(
		this List<TSource> @this,
		Func<TSource, CandidateMap> collectionSelector,
		Func<TSource, Candidate, TResult> resultSelector
	)
	{
		var result = new List<TResult>(@this.Count << 1);
		foreach (var element in @this)
		{
			foreach (var cell in collectionSelector(element))
			{
				result.Add(resultSelector(element, cell));
			}
		}

		return CollectionsMarshal.AsSpan(result);
	}
}
