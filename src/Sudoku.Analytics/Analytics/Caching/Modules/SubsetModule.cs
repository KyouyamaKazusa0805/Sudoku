namespace Sudoku.Analytics.Caching.Modules;

using unsafe SubsetModuleSearcherFuncPtr = delegate*<ref StepAnalysisContext, ref readonly Grid, ref readonly CellMap, ReadOnlySpan<CellMap>, int, bool, Step?>;

/// <summary>
/// Represents a subset module.
/// </summary>
internal static class SubsetModule
{
	/// <summary>
	/// The internal method to create subset steps.
	/// </summary>
	/// <param name="searchingForLocked">Indicates whether the module only searches for locked subsets.</param>
	/// <param name="context">The context.</param>
	/// <returns>The collected steps.</returns>
	public static unsafe Step? CollectCore(bool searchingForLocked, ref StepAnalysisContext context)
	{
		var p = stackalloc SubsetModuleSearcherFuncPtr[] { &HiddenSubset, &NakedSubset };
		var q = stackalloc SubsetModuleSearcherFuncPtr[] { &NakedSubset, &HiddenSubset };
		var searchers = context.Options is { IsDirectMode: true } ? p : q;

		ref readonly var grid = ref context.Grid;
		var emptyCellsForGrid = grid.EmptyCells;
		var candidatesMapForGrid = grid.CandidatesMap;
		var nakedSingleCells = CellMap.Empty;
		foreach (var cell in emptyCellsForGrid)
		{
			if (Mask.IsPow2(grid.GetCandidates(cell)))
			{
				nakedSingleCells.Add(cell);
			}
		}
		emptyCellsForGrid &= ~nakedSingleCells;

		for (var size = 2; size <= (searchingForLocked ? 3 : 4); size++)
		{
			for (var i = 0; i < 2; i++)
			{
				if (searchers[i](ref context, in grid, in emptyCellsForGrid, candidatesMapForGrid, size, searchingForLocked) is { } step)
				{
					return step;
				}
			}
		}

		return null;
	}

	/// <summary>
	/// Search for hidden subsets.
	/// </summary>
	private static HiddenSubsetStep? HiddenSubset(
		ref StepAnalysisContext context,
		ref readonly Grid grid,
		ref readonly CellMap emptyCellsForGrid,
		ReadOnlySpan<CellMap> candidatesMapForGrid,
		int size,
		bool searchingForLocked
	)
	{
		for (var house = 0; house < 27; house++)
		{
			ref readonly var currentHouseCells = ref HousesMap[house];
			var traversingMap = currentHouseCells & emptyCellsForGrid;
			var mask = grid[in traversingMap];
			foreach (var digits in mask.GetAllSets().GetSubsets(size))
			{
				var (tempMask, digitsMask, cells) = (mask, (Mask)0, CellMap.Empty);
				foreach (var digit in digits)
				{
					tempMask &= (Mask)~(1 << digit);
					digitsMask |= (Mask)(1 << digit);
					cells |= currentHouseCells & candidatesMapForGrid[digit];
				}
				if (cells.Count != size)
				{
					continue;
				}

				// Gather eliminations.
				var conclusions = new List<Conclusion>();
				foreach (var digit in tempMask)
				{
					foreach (var cell in cells & candidatesMapForGrid[digit])
					{
						conclusions.Add(new(Elimination, cell, digit));
					}
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				// Gather highlight candidates.
				var (cellOffsets, candidateOffsets) = (new List<IconViewNode>(), new List<CandidateViewNode>());
				foreach (var digit in digits)
				{
					foreach (var cell in cells & candidatesMapForGrid[digit])
					{
						candidateOffsets.Add(new(ColorIdentifier.Normal, cell * 9 + digit));
					}

					cellOffsets.AddRange(Excluder.GetSubsetExcluders(in grid, digit, house, in cells));
				}

				var isLocked = cells.IsInIntersection;
				if (!searchingForLocked || isLocked && searchingForLocked)
				{
					var containsExtraEliminations = false;
					if (isLocked)
					{
						// A potential locked hidden subset found. Extra eliminations should be checked.
						// Please note that here a hidden subset may not be a locked one because eliminations aren't validated.
						var eliminatingHouse = HouseMask.TrailingZeroCount(cells.SharedHouses & ~(1 << house));
						foreach (var cell in HousesMap[eliminatingHouse] & emptyCellsForGrid & ~cells)
						{
							foreach (var digit in digitsMask)
							{
								if ((grid.GetCandidates(cell) >> digit & 1) != 0)
								{
									conclusions.Add(new(Elimination, cell, digit));
									containsExtraEliminations = true;
								}
							}
						}
					}

					if (searchingForLocked && isLocked && !containsExtraEliminations
						|| !searchingForLocked && isLocked && containsExtraEliminations)
					{
						// This is a locked hidden subset. We cannot handle this as a normal hidden subset.
						continue;
					}

					var step = new HiddenSubsetStep(
						conclusions.AsReadOnlyMemory(),
						[[.. candidateOffsets, new HouseViewNode(ColorIdentifier.Normal, house), .. cellOffsets]],
						context.Options,
						house,
						in cells,
						digitsMask,
						isLocked && containsExtraEliminations
					);
					if (context.OnlyFindOne)
					{
						return step;
					}

					context.Accumulator.Add(step);
				}
			}
		}

		return null;
	}

	/// <summary>
	/// Search for naked subsets.
	/// </summary>
	private static NakedSubsetStep? NakedSubset(
		ref StepAnalysisContext context,
		ref readonly Grid grid,
		ref readonly CellMap emptyCellsForGrid,
		ReadOnlySpan<CellMap> candidatesMapForGrid,
		int size,
		bool searchingForLocked
	)
	{
		for (var house = 0; house < 27; house++)
		{
			if ((HousesMap[house] & emptyCellsForGrid) is not { Count: >= 2 } currentEmptyMap)
			{
				continue;
			}

			// Remove cells that only contain 1 candidate (Naked Singles).
			foreach (var cell in HousesMap[house] & emptyCellsForGrid)
			{
				if (Mask.IsPow2(grid.GetCandidates(cell)))
				{
					currentEmptyMap.Remove(cell);
				}
			}

			// Iterate on each combination.
			foreach (ref readonly var cells in currentEmptyMap & size)
			{
				var digitsMask = grid[in cells];
				if (Mask.PopCount(digitsMask) != size)
				{
					continue;
				}

				// Naked subset found. Now check eliminations.
				var (lockedDigitsMask, conclusions) = ((Mask)0, new List<Conclusion>(18));
				foreach (var digit in digitsMask)
				{
					var map = cells % candidatesMapForGrid[digit];
					lockedDigitsMask |= (Mask)(map.FirstSharedHouse != 32 ? 0 : 1 << digit);

					conclusions.AddRange(from cell in map select new Conclusion(Elimination, cell, digit));
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>(16);
				foreach (var cell in cells)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(ColorIdentifier.Normal, cell * 9 + digit));
					}
				}

				var isLocked = cells.IsInIntersection
					? true
					: lockedDigitsMask == digitsMask && size != 4
						? true
						: lockedDigitsMask != 0 ? false : default(bool?);
				if ((isLocked, searchingForLocked) is not ((true, true) or (not true, false)))
				{
					continue;
				}

				var step = new NakedSubsetStep(
					conclusions.AsReadOnlyMemory(),
					[[.. candidateOffsets, new HouseViewNode(ColorIdentifier.Normal, house)]],
					context.Options,
					house,
					in cells,
					digitsMask,
					isLocked
				);
				if (context.OnlyFindOne)
				{
					return step;
				}

				context.Accumulator.Add(step);
			}
		}

		return null;
	}
}
