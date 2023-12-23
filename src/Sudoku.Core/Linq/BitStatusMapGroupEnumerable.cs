namespace Sudoku.Linq;

/// <summary>
/// Represents a list of LINQ methods used by <see cref="BitStatusMapGroup{TMap, TElement, TKey}"/> instances.
/// </summary>
/// <seealso cref="BitStatusMapGroup{TMap, TElement, TKey}"/>
public static class BitStatusMapGroupEnumerable
{
	/// <summary>
	/// Projects a list of <see cref="BitStatusMapGroup{TMap, TElement, TKey}"/> of types <see cref="CellMap"/>, <see cref="Cell"/>
	/// and <typeparamref name="TKey"/>, into a <see cref="Cell"/> value; collect converted results and merge
	/// into a <see cref="CellMap"/> instance.
	/// </summary>
	/// <typeparam name="TKey">The type of the grouping.</typeparam>
	/// <param name="this">The list to be checked.</param>
	/// <param name="selector">The transform method to apply to each element.</param>
	/// <returns>The result.</returns>
	public static CellMap Select<TKey>(
		this scoped ReadOnlySpan<BitStatusMapGroup<CellMap, Cell, TKey>> @this,
		Func<BitStatusMapGroup<CellMap, Cell, TKey>, Cell> selector
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
	/// Projects a list of <see cref="BitStatusMapGroup{TMap, TElement, TKey}"/> of types <see cref="CandidateMap"/>, <see cref="Candidate"/>
	/// and <typeparamref name="TKey"/>, into a <see cref="Candidate"/> value; collect converted results and merge
	/// into a <see cref="CandidateMap"/> instance.
	/// </summary>
	/// <typeparam name="TKey">The type of the grouping.</typeparam>
	/// <param name="this">The list to be checked.</param>
	/// <param name="selector">The transform method to apply to each element.</param>
	/// <returns>The result.</returns>
	public static CandidateMap Select<TKey>(
		this scoped ReadOnlySpan<BitStatusMapGroup<CandidateMap, Candidate, TKey>> @this,
		Func<BitStatusMapGroup<CandidateMap, Candidate, TKey>, Candidate> selector
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
