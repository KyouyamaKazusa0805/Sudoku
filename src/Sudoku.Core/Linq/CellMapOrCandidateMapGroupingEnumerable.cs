namespace Sudoku.Linq;

/// <summary>
/// Represents a list of LINQ methods used by <see cref="CellMapOrCandidateMapGrouping{TMap, TElement, TEnumerator, TKey}"/> instances.
/// </summary>
/// <seealso cref="CellMapOrCandidateMapGrouping{TMap, TElement, TEnumerator, TKey}"/>
public static class CellMapOrCandidateMapGroupingEnumerable
{
	/// <summary>
	/// Filters a sequence of values based on a predicate.
	/// </summary>
	/// <typeparam name="TMap">
	/// The type of the map that stores the <see cref="CellMapOrCandidateMapGrouping{TMap, TElement, TEnumerator, TKey}.Values"/>.
	/// </typeparam>
	/// <typeparam name="TElement">
	/// The type of elements stored in <see cref="CellMapOrCandidateMapGrouping{TMap, TElement, TEnumerator, TKey}.Values"/>.
	/// </typeparam>
	/// <typeparam name="TEnumerator">The type of enumerator.</typeparam>
	/// <typeparam name="TKey">The type of the key in the group.</typeparam>
	/// <param name="this">The instance to be checked.</param>
	/// <param name="predicate">A function to test each element for a condition.</param>
	/// <returns>
	/// An array of <typeparamref name="TElement"/> instances
	/// that contains elements from the input sequence that satisfy the condition.
	/// </returns>
	public static ReadOnlySpan<TElement> Where<TMap, TElement, TEnumerator, TKey>(
		this CellMapOrCandidateMapGrouping<TMap, TElement, TEnumerator, TKey> @this,
		Func<TElement, bool> predicate
	)
		where TMap : unmanaged, ICellMapOrCandidateMap<TMap, TElement, TEnumerator>
		where TElement : unmanaged, IBinaryInteger<TElement>
		where TEnumerator : struct, IEnumerator<TElement>, allows ref struct
		where TKey : notnull
	{
		var result = new TElement[@this.Values.Count];
		var i = 0;
		foreach (var element in @this.Values)
		{
			if (predicate(element))
			{
				result[i++] = element;
			}
		}
		return result.AsReadOnlySpan()[..i];
	}

	/// <summary>
	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})" path="/summary"/>
	/// </summary>
	/// <typeparam name="TMap">
	/// <inheritdoc cref="CellMapOrCandidateMapGrouping{TMap, TElement, TEnumerator, TKey}" path="/typeparam[@name='TMap']"/>
	/// </typeparam>
	/// <typeparam name="TElement">
	/// <inheritdoc cref="CellMapOrCandidateMapGrouping{TMap, TElement, TEnumerator, TKey}" path="/typeparam[@name='TElement']"/>
	/// </typeparam>
	/// <typeparam name="TKey">
	/// <inheritdoc cref="CellMapOrCandidateMapGrouping{TMap, TElement, TEnumerator, TKey}" path="/typeparam[@name='TKey']"/>
	/// </typeparam>
	/// <typeparam name="TEnumerator">
	/// The type of enumerator.
	/// </typeparam>
	/// <typeparam name="TResult">
	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})" path="/typeparam[@name='TResult']"/>
	/// </typeparam>
	/// <param name="this">The instance to be checked.</param>
	/// <param name="selector">
	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})" path="/param[@name='selector']"/>
	/// </param>
	/// <returns>
	/// A <see cref="ReadOnlySpan{T}"/> of <typeparamref name="TResult"/> instances
	/// whose elements are the result of invoking the transform function on each element of the current instance.
	/// </returns>
	public static ReadOnlySpan<TResult> Select<TMap, TElement, TEnumerator, TKey, TResult>(
		this CellMapOrCandidateMapGrouping<TMap, TElement, TEnumerator, TKey> @this,
		Func<TElement, TResult> selector
	)
		where TMap : unmanaged, ICellMapOrCandidateMap<TMap, TElement, TEnumerator>
		where TElement : unmanaged, IBinaryInteger<TElement>
		where TEnumerator : struct, IEnumerator<TElement>, allows ref struct
		where TKey : notnull
	{
		var result = new TResult[@this.Values.Count];
		var i = 0;
		foreach (var element in @this.Values)
		{
			result[i++] = selector(element);
		}
		return result;
	}

	/// <summary>
	/// Projects a list of <see cref="CellMapOrCandidateMapGrouping{TMap, TElement, TEnumerator, TKey}"/> of types <see cref="CellMap"/>,
	/// <see cref="Cell"/> and <typeparamref name="TKey"/>, into a <see cref="Cell"/> value; collect converted results and merge
	/// into a <see cref="CellMap"/> instance.
	/// </summary>
	/// <typeparam name="TKey">The type of the grouping.</typeparam>
	/// <param name="this">The list to be checked.</param>
	/// <param name="selector">The transform method to apply to each element.</param>
	/// <returns>The result.</returns>
	public static CellMap Select<TKey>(
		this ReadOnlySpan<CellMapOrCandidateMapGrouping<CellMap, Cell, CellMap.Enumerator, TKey>> @this,
		Func<CellMapOrCandidateMapGrouping<CellMap, Cell, CellMap.Enumerator, TKey>, Cell> selector
	) where TKey : notnull
	{
		var result = CellMap.Empty;
		foreach (var group in @this)
		{
			result.Add(selector(group));
		}
		return result;
	}

	/// <summary>
	/// Projects a list of <see cref="CellMapOrCandidateMapGrouping{TMap, TElement, TEnumerator, TKey}"/> of types <see cref="CandidateMap"/>,
	/// <see cref="Candidate"/> and <typeparamref name="TKey"/>, into a <see cref="Candidate"/> value; collect converted results
	/// and merge into a <see cref="CandidateMap"/> instance.
	/// </summary>
	/// <typeparam name="TKey">The type of the grouping.</typeparam>
	/// <param name="this">The list to be checked.</param>
	/// <param name="selector">The transform method to apply to each element.</param>
	/// <returns>The result.</returns>
	public static CandidateMap Select<TKey>(
		this ReadOnlySpan<CellMapOrCandidateMapGrouping<CandidateMap, Candidate, CandidateMap.Enumerator, TKey>> @this,
		Func<CellMapOrCandidateMapGrouping<CandidateMap, Candidate, CandidateMap.Enumerator, TKey>, Candidate> selector
	) where TKey : notnull
	{
		var result = CandidateMap.Empty;
		foreach (var group in @this)
		{
			result.Add(selector(group));
		}
		return result;
	}
}
