namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Subset</b> step searcher.
/// </summary>
/// <param name="priority">
/// <inheritdoc cref="StepSearcher(int, int, StepSearcherRunningArea)" path="/param[@name='priority']"/>
/// </param>
/// <param name="level">
/// <inheritdoc cref="StepSearcher(int, int, StepSearcherRunningArea)" path="/param[@name='level']"/>
/// </param>
/// <param name="runningArea">
/// <inheritdoc cref="StepSearcher(int, int, StepSearcherRunningArea)" path="/param[@name='runningArea']"/>
/// </param>
public abstract partial class SubsetStepSearcher(
	int priority,
	int level,
	StepSearcherRunningArea runningArea = StepSearcherRunningArea.Searching | StepSearcherRunningArea.Gathering
) : StepSearcher(priority, level, runningArea)
{
	/// <summary>
	/// Indicates whether the step searcher only searches for locked subsets.
	/// </summary>
	public abstract bool OnlySearchingForLocked { get; }


	/// <inheritdoc/>
	protected internal sealed override Step? Collect(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		for (var size = 2; size <= (OnlySearchingForLocked ? 3 : 4); size++)
		{
			// Naked subsets.
			for (var house = 0; house < 27; house++)
			{
				if ((HousesMap[house] & EmptyCells) is not { Count: >= 2 } currentEmptyMap)
				{
					continue;
				}

				// Iterate on each combination.
				foreach (var cells in currentEmptyMap & size)
				{
					var digitsMask = grid.GetDigitsUnion(cells);
					if (PopCount((uint)digitsMask) != size)
					{
						continue;
					}

					// Naked subset found. Now check eliminations.
					var (lockedDigitsMask, conclusions) = ((Mask)0, new List<Conclusion>(18));
					foreach (var digit in digitsMask)
					{
						var map = cells % CandidatesMap[digit];
						lockedDigitsMask |= (Mask)(map.InOneHouse ? 0 : 1 << digit);

						foreach (var cell in map)
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
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
							candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
						}
					}

					var isLocked = lockedDigitsMask == digitsMask ? true : lockedDigitsMask != 0 ? false : default(bool?);
					if (!OnlySearchingForLocked || isLocked is not null && OnlySearchingForLocked)
					{
						var step = new NakedSubsetStep(
							conclusions.ToArray(),
							new[] { View.Empty | candidateOffsets | new HouseViewNode(WellKnownColorIdentifier.Normal, house) },
							house,
							cells,
							digitsMask,
							OnlySearchingForLocked ? true : isLocked
						);

						if (context.OnlyFindOne)
						{
							return step;
						}

						context.Accumulator.Add(step);
					}
				}
			}

			// Hidden subsets.
			for (var house = 0; house < 27; house++)
			{
				scoped ref readonly var currentHouseCells = ref HousesMap[house];
				var traversingMap = currentHouseCells - EmptyCells;
				if (traversingMap.Count >= 8)
				{
					// No available digit (Or hidden single).
					continue;
				}

				var mask = Grid.MaxCandidatesMask;
				foreach (var cell in traversingMap)
				{
					mask &= (Mask)~(1 << grid[cell]);
				}
				foreach (var digits in mask.GetAllSets().GetSubsets(size))
				{
					var (tempMask, digitsMask, map) = (mask, (Mask)0, CellMap.Empty);
					foreach (var digit in digits)
					{
						tempMask &= (Mask)~(1 << digit);
						digitsMask |= (Mask)(1 << digit);
						map |= currentHouseCells & CandidatesMap[digit];
					}
					if (map.Count != size)
					{
						continue;
					}

					// Gather eliminations.
					var conclusions = new List<Conclusion>();
					foreach (var digit in tempMask)
					{
						foreach (var cell in map & CandidatesMap[digit])
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					// Gather highlight candidates.
					var (cellOffsets, candidateOffsets) = (new List<CellViewNode>(), new List<CandidateViewNode>());
					foreach (var digit in digits)
					{
						foreach (var cell in map & CandidatesMap[digit])
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
						}

						cellOffsets.AddRange(GetCrosshatchBaseCells(grid, digit, house, map));
					}

					var isLocked = map.IsInIntersection;
					if (!OnlySearchingForLocked || isLocked && OnlySearchingForLocked)
					{
						var containsExtraEliminations = false;
						if (isLocked)
						{
							// A potential locked hidden subset found. Extra eliminations should be checked.
							// Please note that here a hidden subset may not be a locked one because eliminations aren't validated.
							var eliminatingHouse = TrailingZeroCount(map.CoveredHouses & ~(1 << house));
							foreach (var cell in (HousesMap[eliminatingHouse] & EmptyCells) - map)
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

						if (OnlySearchingForLocked && isLocked && !containsExtraEliminations
							|| !OnlySearchingForLocked && isLocked && containsExtraEliminations)
						{
							// This is a locked hidden subset. We cannot handle this as a normal hidden subset.
							continue;
						}

						var step = new HiddenSubsetStep(
							conclusions.ToArray(),
							new[]
							{
								View.Empty
									| candidateOffsets
									| new HouseViewNode(WellKnownColorIdentifier.Normal, house)
									| cellOffsets
							},
							house,
							map,
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
		}

		return null;
	}

	/// <summary>
	/// Try to create a list of <see cref="CellViewNode"/>s indicating the crosshatching base cells.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="digit">The digit.</param>
	/// <param name="house">The house.</param>
	/// <param name="cells">The cells.</param>
	/// <returns>A list of <see cref="CellViewNode"/> instances.</returns>
	private CellViewNode[] GetCrosshatchBaseCells(scoped in Grid grid, Digit digit, House house, scoped in CellMap cells)
	{
		var info = Crosshatching.GetCrosshatchingInfo(grid, digit, house, cells);
		if (info is not ({ } combination, var emptyCellsShouldBeCovered, var emptyCellsNotNeedToBeCovered))
		{
			return Array.Empty<CellViewNode>();
		}

		var result = new List<CellViewNode>();
		foreach (var c in combination)
		{
			result.Add(new(WellKnownColorIdentifier.Normal, c) { RenderingMode = RenderingMode.DirectModeOnly });
		}
		foreach (var c in emptyCellsShouldBeCovered)
		{
			var p = emptyCellsNotNeedToBeCovered.Contains(c) ? WellKnownColorIdentifier.Auxiliary2 : WellKnownColorIdentifier.Auxiliary1;
			result.Add(new(p, c) { RenderingMode = RenderingMode.DirectModeOnly });
		}

		return result.ToArray();
	}
}
