namespace Sudoku.Solving.Logical.StepSearchers;

[StepSearcher]
internal sealed unsafe partial class MultisectorLockedSetsStepSearcher : IMultisectorLockedSetsStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(scoped ref LogicalAnalysisContext context)
	{
		var linkForEachHouse = stackalloc short[27];
		var linkForEachDigit = stackalloc CellMap[9];
		scoped ref readonly var grid = ref context.Grid;
		foreach (var pattern in IMultisectorLockedSetsStepSearcher.Patterns)
		{
			var map = EmptyCells & pattern;
			if (pattern.Count < 12 && pattern.Count - map.Count > 1 || pattern.Count - map.Count > 2)
			{
				continue;
			}

			int n = 0, count = map.Count;
			for (var digit = 0; digit < 9; digit++)
			{
				var pMap = linkForEachDigit + digit;
				*pMap = CandidatesMap[digit] & map;
				n += MathExtensions.Min(
					PopCount((uint)pMap->RowMask),
					PopCount((uint)pMap->ColumnMask),
					PopCount((uint)pMap->BlockMask)
				);
			}

			if (n == count)
			{
				var canL = new CellMap[9];
				var conclusions = new List<Conclusion>();
				var candidateOffsets = new List<CandidateViewNode>();
				for (var digit = 0; digit < 9; digit++)
				{
					var q = (short)(1 << digit);
					var currentMap = linkForEachDigit[digit];
					uint
						rMask = (uint)currentMap.RowMask,
						cMask = (uint)currentMap.ColumnMask,
						bMask = (uint)currentMap.BlockMask;
					var temp = MathExtensions.Min(PopCount(rMask), PopCount(cMask), PopCount(bMask));
					var elimMap = CellMap.Empty;
					var check = 0;
					if (PopCount(rMask) == temp)
					{
						check++;
						foreach (var i in rMask)
						{
							var house = i + 9;
							linkForEachHouse[house] |= q;
							elimMap |= (CandidatesMap[digit] & HousesMap[house] & map).PeerIntersection;
						}
					}
					if (PopCount(cMask) == temp)
					{
						check++;
						foreach (var i in cMask)
						{
							var house = i + 18;
							linkForEachHouse[house] |= q;
							elimMap |= (CandidatesMap[digit] & HousesMap[house] & map).PeerIntersection;
						}
					}
					if (PopCount(bMask) == temp)
					{
						check++;
						foreach (var i in bMask)
						{
							linkForEachHouse[i] |= q;
							elimMap |= (CandidatesMap[digit] & HousesMap[i] & map).PeerIntersection;
						}
					}

					elimMap &= CandidatesMap[digit];
					if (!elimMap)
					{
						continue;
					}

					foreach (var cell in elimMap)
					{
						if (map.Contains(cell))
						{
							canL[digit].Add(cell);
						}

						conclusions.Add(new(Elimination, cell, digit));
					}
				}

				if (conclusions.Count == 0)
				{
					continue;
				}

				for (var house = 0; house < 27; house++)
				{
					var linkMask = linkForEachHouse[house];
					if (linkMask == 0)
					{
						continue;
					}

					foreach (var cell in map & HousesMap[house])
					{
						var cands = (short)(grid.GetCandidates(cell) & linkMask);
						if (cands == 0)
						{
							continue;
						}

						foreach (var cand in cands)
						{
							if (!canL[cand].Contains(cell))
							{
								candidateOffsets.Add(
									new(
										house switch
										{
											< 9 => DisplayColorKind.Auxiliary2,
											< 18 => DisplayColorKind.Normal,
											_ => DisplayColorKind.Auxiliary1
										},
										cell * 9 + cand
									)
								);
							}
						}
					}
				}

				var step = new MultisectorLockedSetsStep(
					conclusions.ToArray(),
					new[] { View.Empty | candidateOffsets },
					map
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
