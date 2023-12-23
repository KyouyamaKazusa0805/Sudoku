namespace Sudoku.Linq;

/// <summary>
/// Represents a list of methods for iteration on <see cref="IBitStatusMap{TSelf, TElement}"/>.
/// </summary>
/// <seealso cref="IBitStatusMap{TSelf, TElement}"/>
public static class BitStatusMapEnumerable
{
	/// <summary>
	/// Projects each cell (of type <see cref="Cell"/>) of a <see cref="CellMap"/> to a mask (of type <see cref="Mask"/>),
	/// flattens the resulting sequence into one sequence, and invokes a result selector function on each element therein.
	/// </summary>
	/// <typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam>
	/// <param name="this">A sequence of values to project.</param>
	/// <param name="collectionSelector">A transform function to apply to each element of the input <see cref="CellMap"/>.</param>
	/// <param name="resultSelector">A transform function to apply to each element of the intermediate mask (of type <see cref="Mask"/>).</param>
	/// <returns>
	/// A <see cref="ReadOnlySpan{T}"/> of <typeparamref name="TResult"/> whose elements are the result
	/// of invoking the one-to-many transform function <paramref name="collectionSelector"/> on each element of <paramref name="this"/>
	/// and then mapping each of those sequences and their corresponding source element to a result element.
	/// </returns>
	public static ReadOnlySpan<TResult> SelectMany<TResult>(
		this scoped ref readonly CellMap @this,
		Func<Cell, Mask> collectionSelector,
		Func<Cell, Digit, TResult> resultSelector
	)
	{
		var result = new List<TResult>(@this.Count << 1);
		foreach (var cell in @this)
		{
			foreach (var digit in collectionSelector(cell))
			{
				result.Add(resultSelector(cell, digit));
			}
		}

		return result.ToArray();
	}

	/// <summary>
	/// Projects each candidate (of type <see cref="Candidate"/>) of a <see cref="CandidateMap"/> to a mask (of type <see cref="Mask"/>),
	/// flattens the resulting sequence into one sequence, and invokes a result selector function on each element therein.
	/// </summary>
	/// <typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam>
	/// <param name="this">A sequence of values to project.</param>
	/// <param name="collectionSelector">A transform function to apply to each element of the input <see cref="CandidateMap"/>.</param>
	/// <param name="resultSelector">A transform function to apply to each element of the intermediate mask (of type <see cref="Mask"/>).</param>
	/// <returns>
	/// A <see cref="ReadOnlySpan{T}"/> of <typeparamref name="TResult"/> whose elements are the result
	/// of invoking the one-to-many transform function <paramref name="collectionSelector"/> on each element of <paramref name="this"/>
	/// and then mapping each of those sequences and their corresponding source element to a result element.
	/// </returns>
	public static ReadOnlySpan<TResult> SelectMany<TResult>(
		this scoped ref readonly CandidateMap @this,
		Func<Candidate, Mask> collectionSelector,
		Func<Candidate, Digit, TResult> resultSelector
	)
	{
		var result = new List<TResult>(@this.Count << 1);
		foreach (var cell in @this)
		{
			foreach (var digit in collectionSelector(cell))
			{
				result.Add(resultSelector(cell, digit));
			}
		}

		return result.ToArray();
	}
}
