﻿namespace Sudoku.Solving.Logical.StepSearchers;

[StepSearcher]
internal sealed unsafe partial class SubsetStepSearcher : ISubsetStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(scoped ref LogicalAnalysisContext context)
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
					short flagMask = 0;
					var conclusions = new List<Conclusion>(18);
					foreach (var digit in mask)
					{
						var map = cells % CandidatesMap[digit];
						flagMask |= (short)(map.InOneHouse ? 0 : 1 << digit);

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
							candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
						}
					}

					var isLocked = flagMask == mask ? true : flagMask != 0 ? false : default(bool?);
					var step = new NakedSubsetStep(
						conclusions.ToArray(),
						new[] { View.Empty | candidateOffsets | new HouseViewNode(DisplayColorKind.Normal, house) },
						house,
						cells,
						mask,
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
				var traversingMap = HousesMap[house] - EmptyCells;
				if (traversingMap.Count >= 8)
				{
					// No available digit (Or hidden single).
					continue;
				}

				var mask = Grid.MaxCandidatesMask;
				foreach (var cell in traversingMap)
				{
					mask &= (short)~(1 << grid[cell]);
				}
				foreach (var digits in mask.GetAllSets().GetSubsets(size))
				{
					var tempMask = mask;
					short digitsMask = 0;
					var map = CellMap.Empty;
					foreach (var digit in digits)
					{
						tempMask &= (short)~(1 << digit);
						digitsMask |= (short)(1 << digit);
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
					var candidateOffsets = new List<CandidateViewNode>();
					foreach (var digit in digits)
					{
						foreach (var cell in map & CandidatesMap[digit])
						{
							candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
						}
					}

					var step = new HiddenSubsetStep(
						conclusions.ToArray(),
						new[] { View.Empty | candidateOffsets | new HouseViewNode(DisplayColorKind.Normal, house) },
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
}
