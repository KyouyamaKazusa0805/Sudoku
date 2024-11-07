namespace Sudoku.Analytics.Intercepting.Generated;

/// <summary>
/// Represents for the driver that will be used for searching for almost locked sets.
/// </summary>
internal static class AlmostLockedSetsPattern_InterceptorGenerated
{
	/// <summary>
	/// Try to collect all possible ALSes in the specified grid.
	/// </summary>
	/// <param name="grid">The grid to be used.</param>
	/// <returns>A list of ALSes.</returns>
	//[InterceptsLocation(@"C:\Users\admin\source\repos\SunnieShine\Sudoku\src\Sudoku.Analytics\Analytics\Caching\ChainingRules\CachedAlmostLockedSetsChainingRule.cs", 21, 46)]
	//[InterceptsLocation(@"C:\Users\admin\source\repos\SunnieShine\Sudoku\src\Sudoku.Analytics\Analytics\StepSearchers\AlmostLockedSets\AlmostLockedSetsWWingStepSearcher.cs", 26, 38)]
	//[InterceptsLocation(@"C:\Users\admin\source\repos\SunnieShine\Sudoku\src\Sudoku.Analytics\Analytics\StepSearchers\AlmostLockedSets\AlmostLockedSetsXyWingStepSearcher.cs", 26, 38)]
	//[InterceptsLocation(@"C:\Users\admin\source\repos\SunnieShine\Sudoku\src\Sudoku.Analytics\Analytics\StepSearchers\AlmostLockedSets\AlmostLockedSetsXzStepSearcher.cs", 46, 38)]
	//[InterceptsLocation(@"C:\Users\admin\source\repos\SunnieShine\Sudoku\src\Sudoku.Analytics\Analytics\StepSearchers\AlmostLockedSets\DeathBlossomStepSearcher.cs", 59, 38)]
	//[InterceptsLocation(@"C:\Users\admin\source\repos\SunnieShine\Sudoku\src\Sudoku.Analytics\Analytics\StepSearchers\DeadlyPatterns\UniqueRectangleStepSearcher.cs", 212, 38)]
	public static ReadOnlySpan<AlmostLockedSetPattern> CollectAlmostLockedSets(ref readonly Grid grid)
	{
		// Get all bi-value-cell ALSes.
		var result = new List<AlmostLockedSetPattern>();
		foreach (var cell in BivalueCells)
		{
			var eliminationMap = new CellMap[9];
			foreach (var digit in grid.GetCandidates(cell))
			{
				eliminationMap[digit] = PeersMap[cell] & CandidatesMap[digit];
			}
			result.Add(new(grid.GetCandidates(cell), in cell.AsCellMap(), PeersMap[cell] & EmptyCells, eliminationMap));
		}

		// Get all non-bi-value-cell ALSes.
		for (var houseIndex = 0; houseIndex < 27; houseIndex++)
		{
			if ((HousesMap[houseIndex] & EmptyCells) is not { Count: >= 3 } tempMap)
			{
				continue;
			}

			for (var size = 2; size <= tempMap.Count - 1; size++)
			{
				foreach (ref readonly var map in tempMap & size)
				{
					var blockMask = map.BlockMask;
					if (Mask.IsPow2(blockMask) && houseIndex >= 9)
					{
						// All ALS cells lying on a box-row or a box-column will be processed as a block ALS.
						continue;
					}

					// Get all candidates in these cells.
					var digitsMask = (Mask)0;
					foreach (var cell in map)
					{
						digitsMask |= grid.GetCandidates(cell);
					}
					if (Mask.PopCount(digitsMask) - 1 != size)
					{
						continue;
					}

					var eliminationMap = new CellMap[9];
					foreach (var digit in digitsMask)
					{
						eliminationMap[digit] = map % CandidatesMap[digit];
					}

					var coveredLine = map.SharedLine;
					result.Add(
						new(
							digitsMask,
							in map,
							houseIndex < 9 && coveredLine is >= 9 and not 32
								? (HousesMap[houseIndex] | HousesMap[coveredLine]) & EmptyCells & ~map
								: tempMap & ~map,
							eliminationMap
						)
					);
				}
			}
		}
		return result.AsSpan();
	}
}
