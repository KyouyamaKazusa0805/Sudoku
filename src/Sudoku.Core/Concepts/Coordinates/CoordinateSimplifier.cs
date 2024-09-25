namespace Sudoku.Concepts.Coordinates;

/// <summary>
/// Represents a simplifier type that can simplify the coordinates.
/// </summary>
public static class CoordinateSimplifier
{
	/// <summary>
	/// Try to simplify parts of cells, by combining same rows and columns.
	/// </summary>
	/// <param name="cells">The cells to be simplified.</param>
	/// <returns>A list of parts grouped by rows and its matched columns.</returns>
	public static ReadOnlySpan<CoordinateSplit> Simplify(ref readonly CellMap cells)
	{
		return (CoordinateSplit[])[
			..
			from pair in simplifyCoordinates([.. from cell in cells select (cell / 9, cell % 9)])
			let rows = pair.Item1
			let columns = pair.Item2
			select rows switch
			{
				RowIndex r => new CoordinateSplit([r], [.. columns]),
				SortedSet<RowIndex> r => new CoordinateSplit([.. r], [.. columns]),
				_ => throw new InvalidOperationException()
			}
		];


		static List<(object, SortedSet<ColumnIndex>)> simplifyCoordinates(List<(RowIndex, ColumnIndex)> coordinates)
		{
			var rowGroups = new Dictionary<RowIndex, SortedSet<ColumnIndex>>();
			var colGroups = new Dictionary<ColumnIndex, SortedSet<RowIndex>>();
			foreach (var (x, y) in coordinates)
			{
				if (!rowGroups.TryAdd(x, [y]))
				{
					rowGroups[x].Add(y);
				}
				if (!colGroups.TryAdd(y, [x]))
				{
					colGroups[y].Add(x);
				}
			}

			var (simplifiedRows, simplifiedCols) = (rowGroups.ToDictionary(), colGroups.ToDictionary());
			var finalSimplified = new List<(object RowOrRows, object ColumnOrColumns)>();
			foreach (var (x, yList) in simplifiedRows)
			{
				foreach (var y in yList)
				{
					finalSimplified.Add(
						simplifiedCols.TryGetValue(y, out var xList)
							? xList.SequenceEqual([x]) ? (x, y) : (xList, y)
							: (x, y)
					);
				}
			}

			var finalDict = new Dictionary<SortedSet<RowIndex>, SortedSet<ColumnIndex>>(SortedSet<RowIndex>.CreateSetComparer());
			foreach (var (rowOrRows, columnOrColumns) in finalSimplified)
			{
				if (rowOrRows is SortedSet<RowIndex> xList)
				{
					if (!finalDict.TryAdd(xList, [(ColumnIndex)columnOrColumns]))
					{
						finalDict[xList].Add((ColumnIndex)columnOrColumns);
					}
				}
				else
				{
					var key = (SortedSet<RowIndex>)[(RowIndex)rowOrRows];
					if (!finalDict.TryAdd(key, [(ColumnIndex)columnOrColumns]))
					{
						finalDict[key].Add((ColumnIndex)columnOrColumns);
					}
				}
			}

			return [
				..
				from kvp in finalDict.ToArray()
				let keySet = kvp.Key
				let valueSet = kvp.Value
				select (keySet.Count > 1 ? (keySet, valueSet) : ((object)keySet.Min, valueSet))
			];
		}
	}
}
