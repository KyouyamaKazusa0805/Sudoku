using Sudoku.Concepts;

namespace Sudoku.Linq;

/// <summary>
/// Represents a list of LINQ methods used by <see cref="BitStatusMapGroup{TMap, TElement, TKey}"/> instances.
/// </summary>
/// <seealso cref="BitStatusMapGroup{TMap, TElement, TKey}"/>
public static class BitStatusMapGroupEnumerable
{
	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})"/>
	public static CellMap Select(this scoped ReadOnlySpan<BitStatusMapGroup<CellMap, Cell, Cell>> @this, Func<BitStatusMapGroup<CellMap, Cell, Cell>, Cell> selector)
	{
		var result = CellMap.Empty;
		foreach (var group in @this)
		{
			result.Add(selector(group));
		}

		return result;
	}
}
