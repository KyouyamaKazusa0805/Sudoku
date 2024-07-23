namespace Sudoku.Concepts;

/// <summary>
/// Provides with extension methods on <see cref="CellMap"/> instances.
/// </summary>
/// <seealso cref="CellMap"/>
public static class CellMapExtensions
{
	/// <summary>
	/// Try to group up with target cells, separating into multiple parts, grouped by its containing row or column.
	/// </summary>
	/// <param name="this">The target cells to be split.</param>
	/// <param name="houses">The mask value holding a list of houses to be matched.</param>
	/// <returns>
	/// A list of <see cref="CellMap"/> grouped,
	/// representing as a <see cref="CellMapOrCandidateMapGrouping{TMap, TElement, TEnumerator, TKey}"/>.
	/// </returns>
	/// <seealso cref="CellMapOrCandidateMapGrouping{TMap, TElement, TEnumerator, TKey}"/>
	public static ReadOnlySpan<CellMapOrCandidateMapGrouping<CellMap, Cell, CellMap.Enumerator, House>> GroupTargets(
		this scoped in CellMap @this,
		HouseMask houses
	)
	{
		var (result, i) = (new CellMapOrCandidateMapGrouping<CellMap, Cell, CellMap.Enumerator, House>[PopCount((uint)houses)], 0);
		foreach (var house in houses)
		{
			if ((@this & HousesMap[house]) is var map and not [])
			{
				result[i++] = new(house, in map);
			}
		}
		return result.AsReadOnlySpan()[..i];
	}
}
