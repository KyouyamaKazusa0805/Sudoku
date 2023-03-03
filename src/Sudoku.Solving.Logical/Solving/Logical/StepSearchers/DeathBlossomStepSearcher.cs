﻿namespace Sudoku.Solving.Logical.StepSearchers;

[StepSearcher]
internal sealed partial class DeathBlossomStepSearcher : IDeathBlossomStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(scoped ref LogicalAnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		var accumulator = context.Accumulator!;
		var dic = IDeathBlossomStepSearcher.GatherGroupedByCell(grid);

		// Cell Type.
		foreach (var hubCell in EmptyCells)
		{
			if (IsPow2(grid.GetCandidates(hubCell)))
			{
				// Optimization: If the cell only contains one candidate, it will be a naked single. We should ignore it.
				continue;
			}

			if (!dic.TryGetValue(hubCell, out var alsGroupedByCell))
			{
				// The current cell does not contain any possible related ALS structures.
				continue;
			}

			var digitsMask = grid.GetCandidates(hubCell);
			var alsesGroupedByDigit = new List<AlmostLockedSet[]>(PopCount((uint)digitsMask));
			var containsDigitDoesNotContainRelatedAlses = false;
			foreach (var digit in digitsMask)
			{
				if (!alsGroupedByCell.TryGetValue(digit, out var list)/* || list.Count == 0*/)
				{
					containsDigitDoesNotContainRelatedAlses = true;
					break;
				}

				alsesGroupedByDigit.Add(list.ToArray());
			}
			if (containsDigitDoesNotContainRelatedAlses)
			{
				continue;
			}

			foreach (var combination in Combinatorics.GetExtractedCombinations(alsesGroupedByDigit.ToArray()))
			{
				var isDuplicated = false;
				for (var i = 0; i < combination.Length - 1; i++)
				{
					var a = combination[i];
					for (var j = i + 1; j < combination.Length; j++)
					{
						var b = combination[j];
						switch (a, b)
						{
							case ({ Map: [var aa] }, { Map: [var bb] })
							when aa.ToHouseIndex(HouseType.Block) == bb.ToHouseIndex(HouseType.Block)
								|| aa.ToHouseIndex(HouseType.Row) == bb.ToHouseIndex(HouseType.Row)
								|| aa.ToHouseIndex(HouseType.Column) == bb.ToHouseIndex(HouseType.Column):
							case ({ House: var ah }, { House: var bh }) when ah == bh:
							case (_, _) when a == b:
							{
								isDuplicated = true;
								goto CheckDuplicate;
							}
						}
					}
				}
			CheckDuplicate:
				if (isDuplicated)
				{
					continue;
				}

				var elimDigitsMask = Grid.MaxCandidatesMask;
				foreach (var als in combination)
				{
					elimDigitsMask &= als.DigitsMask;
				}
				elimDigitsMask &= (short)~grid.GetCandidates(hubCell);

				if (elimDigitsMask == 0)
				{
					// No digits can be eliminated.
					continue;
				}

				var conclusions = new List<Conclusion>();
				var candidateOffsets = new List<CandidateViewNode>();
				foreach (var elimDigit in elimDigitsMask)
				{
					var cellsHavingElimDigit = CellMap.Empty;
					foreach (var als in combination)
					{
						cellsHavingElimDigit |= als.Map & CandidatesMap[elimDigit];
					}

					if ((CandidatesMap[elimDigit] & cellsHavingElimDigit.ExpandedPeers) - hubCell is not (var elimMap and not []))
					{
						// No eliminations found.
						continue;
					}

					foreach (var elimCell in elimMap)
					{
						conclusions.Add(new(Elimination, elimCell, elimDigit));
					}
					foreach (var cellHavingElimDigit in cellsHavingElimDigit)
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, cellHavingElimDigit * 9 + elimDigit));
					}
				}
				if (conclusions.Count == 0)
				{
					// No eliminations found.
					continue;
				}

				byte alsIdentifier = 0;
				foreach (var als in combination)
				{
					foreach (var alsCell in als.Map)
					{
						foreach (var digit in (short)(grid.GetCandidates(alsCell) & ~elimDigitsMask))
						{
							candidateOffsets.Add(new((DisplayColorKind)alsIdentifier, alsCell * 9 + digit));
						}
					}

					alsIdentifier = (byte)(DisplayColorKind.AlmostLockedSet1 + (byte)((alsIdentifier + 1) % 5));
				}

				var step = new DeathBlossomCellTypeStep(
					conclusions.ToArray(),
					new[] { View.Empty | candidateOffsets | new CellViewNode(DisplayColorKind.Normal, hubCell) },
					hubCell,
					elimDigitsMask,
					combination
				);
				if (context.OnlyFindOne)
				{
					return step;
				}

				accumulator.Add(step);
			}
		}

		return null;
	}
}
