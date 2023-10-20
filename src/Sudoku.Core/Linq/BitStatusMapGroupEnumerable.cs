using Sudoku.Concepts;

namespace Sudoku.Linq;

/// <summary>
/// Represents a list of LINQ methods used by <see cref="BitStatusMapGroup{TMap, TElement, TKey}"/> instances.
/// </summary>
/// <seealso cref="BitStatusMapGroup{TMap, TElement, TKey}"/>
public static class BitStatusMapGroupEnumerable
{
	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})"/>
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

	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})"/>
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
