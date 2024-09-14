namespace Sudoku.Analytics.Patterning.Searching;

/// <summary>
/// Represents a pattern searcher on XYZ-Wings.
/// </summary>
internal sealed class XyzWingPatternSearcher : IPatternSearcher<XyzWing>
{
	/// <inheritdoc/>
	public static ReadOnlySpan<XyzWing> Search(ref readonly Grid grid)
	{
		// Collect for tri-value cells.
		var trivalueCells = CellMap.Empty;
		foreach (var cell in EmptyCells & ~BivalueCells)
		{
			if (Mask.PopCount(grid.GetCandidates(cell)) == 3)
			{
				trivalueCells.Add(cell);
			}
		}

		// Iterate on each pivot cell to get all possible results.
		var result = new List<XyzWing>();
		foreach (var pivot in trivalueCells)
		{
			var digitsMaskPivot = grid.GetCandidates(pivot);

			// Fetch for two cells from two different houses.
			foreach (var housePair in HouseTypes.AsReadOnlySpan().GetSubsets(2))
			{
				var house1 = pivot.ToHouse(housePair[0]);
				var house2 = pivot.ToHouse(housePair[1]);
				var bivalueCellsFromHouse1 = BivalueCells & HousesMap[house1];
				var bivalueCellsFromHouse2 = BivalueCells & HousesMap[house2];
				if (!bivalueCellsFromHouse1 || !bivalueCellsFromHouse2)
				{
					continue;
				}

				foreach (var leafCell1 in bivalueCellsFromHouse1)
				{
					var digitsMask1 = grid.GetCandidates(leafCell1);
					foreach (var leafCell2 in bivalueCellsFromHouse2)
					{
						var pattern = pivot.AsCellMap() + leafCell1 + leafCell2;
						if (pattern.InOneHouse(out _))
						{
							continue;
						}

						var digitsMask2 = grid.GetCandidates(leafCell2);

						// Check whether 3 cells intersected by one common digit, and contains 3 different digits.
						var unionedDigitsMask = (Mask)((Mask)(digitsMaskPivot | digitsMask1) | digitsMask2);
						if (Mask.PopCount(unionedDigitsMask) != 3
							|| unionedDigitsMask != digitsMaskPivot
							|| !Mask.IsPow2((Mask)(digitsMaskPivot & digitsMask1 & digitsMask2)))
						{
							continue;
						}

						var intersectedDigit = Mask.Log2((Mask)(digitsMaskPivot & digitsMask1 & digitsMask2));
						result.Add(new(pivot, leafCell1, leafCell2, house1, house2, unionedDigitsMask, intersectedDigit));
					}
				}
			}
		}
		return result.AsReadOnlySpan();
	}
}
