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
		return
			from pair in simplifyCoordinates(from cell in cells select (cell / 9, cell % 9))
			let rows = pair.Item1
			let columns = pair.Item2
			select rows switch
			{
				RowIndex r => new CoordinateSplit([r], [.. columns]),
				SortedSet<RowIndex> r => new CoordinateSplit([.. r], [.. columns]),
				_ => throw new InvalidOperationException()
			};


		static ReadOnlySpan<(object, SortedSet<ColumnIndex>)> simplifyCoordinates(ReadOnlySpan<(RowIndex, ColumnIndex)> coordinates)
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

			var (simplifiedRows, simplifiedColumns) = (rowGroups.ToDictionary(), colGroups.ToDictionary());
			var final = new List<(object RowOrRows, object ColumnOrColumns)>();
			foreach (var (x, yList) in simplifiedRows)
			{
				foreach (var y in yList)
				{
					final.Add(simplifiedColumns.TryGetValue(y, out var xList) && !xList.SequenceEqual([x]) ? (xList, y) : (x, y));
				}
			}

			var finalDic = new Dictionary<SortedSet<RowIndex>, SortedSet<ColumnIndex>>(SortedSet<RowIndex>.CreateSetComparer());
			foreach (var (rowOrRows, columnOrColumns) in final)
			{
				switch (rowOrRows)
				{
					case SortedSet<RowIndex> xList:
					{
						if (!finalDic.TryAdd(xList, [(ColumnIndex)columnOrColumns]))
						{
							finalDic[xList].Add((ColumnIndex)columnOrColumns);
						}
						break;
					}
					default:
					{
						var key = (SortedSet<RowIndex>)[(RowIndex)rowOrRows];
						if (!finalDic.TryAdd(key, [(ColumnIndex)columnOrColumns]))
						{
							finalDic[key].Add((ColumnIndex)columnOrColumns);
						}
						break;
					}
				}
			}

			return
				from kvp in finalDic.ToArray()
				let keySet = kvp.Key
				let valueSet = kvp.Value
				select keySet.Count > 1 ? (keySet, valueSet) : ((object)keySet.Min, valueSet);
		}
	}
}
