using System.Numerics;
using Sudoku.Analytics.Steps;
using Sudoku.Analytics.StepSearchers;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Rendering.Nodes;
using static System.Numerics.BitOperations;
using static Sudoku.Analytics.CachedFields;
using static Sudoku.Analytics.ConclusionType;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Analytics.StepSearcherModules;

/// <summary>
/// Represents a subset module.
/// </summary>
internal sealed class SubsetModule : IStepSearcherModule<SubsetModule>
{
	/// <inheritdoc/>
	static Type[] IStepSearcherModule<SubsetModule>.SupportedTypes => [typeof(LockedSubsetStepSearcher), typeof(NormalSubsetStepSearcher)];


	/// <summary>
	/// The internal method to create subset steps.
	/// </summary>
	/// <param name="searchingForLocked">Indicates whether the module only searches for locked subsets.</param>
	/// <param name="context">The context.</param>
	/// <returns>The collected steps.</returns>
	public static Step? CollectCore(bool searchingForLocked, scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		for (var size = 2; size <= (searchingForLocked ? 3 : 4); size++)
		{
			// Naked subsets.
			for (var house = 0; house < 27; house++)
			{
				if ((HousesMap[house] & EmptyCells) is not { Count: >= 2 } currentEmptyMap)
				{
					continue;
				}

				// Remove cells that only contain 1 candidate (Naked Singles).
				foreach (var cell in HousesMap[house] & EmptyCells)
				{
					if (IsPow2(grid.GetCandidates(cell)))
					{
						currentEmptyMap.Remove(cell);
					}
				}

				// Iterate on each combination.
				foreach (ref readonly var cells in currentEmptyMap.GetSubsets(size))
				{
					var digitsMask = grid[in cells];
					if (PopCount((uint)digitsMask) != size)
					{
						continue;
					}

					// Naked subset found. Now check eliminations.
					var (lockedDigitsMask, conclusions) = ((Mask)0, new List<Conclusion>(18));
					foreach (var digit in digitsMask)
					{
						var map = cells % CandidatesMap[digit];
						lockedDigitsMask |= (Mask)(map.InOneHouse(out _) ? 0 : 1 << digit);

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
							candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
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
						[.. conclusions],
						[[.. candidateOffsets, new HouseViewNode(WellKnownColorIdentifier.Normal, house)]],
						context.PredefinedOptions,
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

			// Hidden subsets.
			for (var house = 0; house < 27; house++)
			{
				scoped ref readonly var currentHouseCells = ref HousesMap[house];
				var traversingMap = currentHouseCells & EmptyCells;
				var mask = grid[in traversingMap];
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

						cellOffsets.AddRange(GetCrosshatchBaseCells(in grid, digit, house, in map));
					}

					var isLocked = map.IsInIntersection;
					if (!searchingForLocked || isLocked && searchingForLocked)
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

						if (searchingForLocked && isLocked && !containsExtraEliminations
							|| !searchingForLocked && isLocked && containsExtraEliminations)
						{
							// This is a locked hidden subset. We cannot handle this as a normal hidden subset.
							continue;
						}

						var step = new HiddenSubsetStep(
							[.. conclusions],
							[[.. candidateOffsets, new HouseViewNode(WellKnownColorIdentifier.Normal, house), .. cellOffsets]],
							context.PredefinedOptions,
							house,
							in map,
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
	private static CellViewNode[] GetCrosshatchBaseCells(scoped ref readonly Grid grid, Digit digit, House house, scoped ref readonly CellMap cells)
	{
		var info = Crosshatching.GetCrosshatchingInfo(in grid, digit, house, in cells);
		if (info is not var (combination, emptyCellsShouldBeCovered, emptyCellsNotNeedToBeCovered))
		{
			return [];
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

		return [.. result];
	}
}