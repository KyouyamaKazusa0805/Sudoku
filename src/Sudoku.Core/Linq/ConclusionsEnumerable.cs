namespace Sudoku.Linq;

/// <summary>
/// Provides with a list of LINQ methods used by <see cref="Conclusion"/>.
/// </summary>
/// <seealso cref="Conclusion"/>
public static class ConclusionsEnumerable
{
	/// <summary>
	/// Projects a list of <see cref="Conclusion"/> instances, converted each instances into a <see cref="Cell"/> value,
	/// and merge them into a <see cref="CellMap"/> and return it.
	/// </summary>
	/// <param name="this">A list of <see cref="Conclusion"/> instances.</param>
	/// <param name="selector">The selector to project the values.</param>
	/// <returns>A <see cref="CellMap"/> result.</returns>
	public static CellMap Select(this ReadOnlySpan<Conclusion> @this, Func<Conclusion, Cell> selector)
	{
		var result = CellMap.Empty;
		foreach (var element in @this)
		{
			result.Add(selector(element));
		}
		return result;
	}
}
