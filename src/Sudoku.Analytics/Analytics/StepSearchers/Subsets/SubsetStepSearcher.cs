namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Subset</b> step searcher. The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Locked Subsets:
/// <list type="bullet">
/// <item>Locked Pair</item>
/// <item>Locked Triple</item>
/// </list>
/// </item>
/// <item>
/// Semi-Locked Subsets:
/// <list type="bullet">
/// <item>Naked Pair (+)</item>
/// <item>Naked Triple (+)</item>
/// <item>Naked Quadruple (+)</item>
/// </list>
/// </item>
/// <item>
/// Normal Naked Subsets:
/// <list type="bullet">
/// <item>Naked Pair</item>
/// <item>Naked Triple</item>
/// <item>Naked Quadruple</item>
/// </list>
/// </item>
/// <item>
/// Hidden Subsets:
/// <list type="bullet">
/// <item>Hidden Pair</item>
/// <item>Hidden Triple</item>
/// <item>Hidden Quadruple</item>
/// </list>
/// </item>
/// </list>
/// </summary>
[StepSearcher]
public sealed partial class SubsetStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		for (var size = 2; size <= 4; size++)
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
					var mask = grid.GetDigitsUnion(cells);
					if (PopCount((uint)mask) != size)
					{
						continue;
					}

					// Naked subset found. Now check eliminations.
					var (flagMask, conclusions) = ((Mask)0, new List<Conclusion>(18));
					foreach (var digit in mask)
					{
						var map = cells % CandidatesMap[digit];
						flagMask |= (Mask)(map.InOneHouse ? 0 : 1 << digit);

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

					var step = new NakedSubsetStep(
						conclusions.ToArray(),
						new[] { View.Empty | candidateOffsets | new HouseViewNode(WellKnownColorIdentifier.Normal, house) },
						house,
						cells,
						mask,
						flagMask == mask ? true : flagMask != 0 ? false : default(bool?)
					);

					if (context.OnlyFindOne)
					{
						return step;
					}

					context.Accumulator.Add(step);
				}
			}

			// Hidden subsets.
			for (var house = 0; house < 27; house++)
			{
				var traversingMap = HousesMap[house] - EmptyCells;
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
						map |= HousesMap[house] & CandidatesMap[digit];
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
						var subsetCellsForThisDigit = map & CandidatesMap[digit];
						foreach (var cell in subsetCellsForThisDigit)
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
						}

						cellOffsets.AddRange(from node in GetCrosshatchBaseCells(grid, digit, house, subsetCellsForThisDigit) select node);
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
						digitsMask
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
