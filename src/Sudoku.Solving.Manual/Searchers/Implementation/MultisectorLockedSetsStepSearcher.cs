namespace Sudoku.Solving.Manual.Searchers;

[StepSearcher]
internal sealed unsafe partial class MultisectorLockedSetsStepSearcher : IMultisectorLockedSetsStepSearcher
{
	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		short* linkForEachHouse = stackalloc short[27];
		var linkForEachDigit = stackalloc Cells[9];
		foreach (var pattern in IMultisectorLockedSetsStepSearcher.Patterns)
		{
			var map = EmptyCells & pattern;
			if (pattern.Count < 12 && pattern.Count - map.Count > 1 || pattern.Count - map.Count > 2)
			{
				continue;
			}

			int n = 0, count = map.Count;
			for (int digit = 0; digit < 9; digit++)
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
				var canL = new Cells[9];
				var conclusions = new List<Conclusion>();
				var candidateOffsets = new List<CandidateViewNode>();
				for (int digit = 0; digit < 9; digit++)
				{
					short q = (short)(1 << digit);
					var currentMap = linkForEachDigit[digit];
					uint
						rMask = (uint)currentMap.RowMask,
						cMask = (uint)currentMap.ColumnMask,
						bMask = (uint)currentMap.BlockMask;
					int temp = MathExtensions.Min(PopCount(rMask), PopCount(cMask), PopCount(bMask));
					var elimMap = Cells.Empty;
					int check = 0;
					if (PopCount(rMask) == temp)
					{
						check++;
						foreach (int i in rMask)
						{
							int house = i + 9;
							linkForEachHouse[house] |= q;
							elimMap |= !(CandidatesMap[digit] & HouseMaps[house] & map);
						}
					}
					if (PopCount(cMask) == temp)
					{
						check++;
						foreach (int i in cMask)
						{
							int house = i + 18;
							linkForEachHouse[house] |= q;
							elimMap |= !(CandidatesMap[digit] & HouseMaps[house] & map);
						}
					}
					if (PopCount(bMask) == temp)
					{
						check++;
						foreach (int i in bMask)
						{
							linkForEachHouse[i] |= q;
							elimMap |= !(CandidatesMap[digit] & HouseMaps[i] & map);
						}
					}

					elimMap &= CandidatesMap[digit];
					if (elimMap is [])
					{
						continue;
					}

					foreach (int cell in elimMap)
					{
						if (map.Contains(cell))
						{
							canL[digit].Add(cell);
						}

						conclusions.Add(new(ConclusionType.Elimination, cell, digit));
					}
				}

				if (conclusions.Count == 0)
				{
					continue;
				}

				for (int house = 0; house < 27; house++)
				{
					short linkMask = linkForEachHouse[house];
					if (linkMask == 0)
					{
						continue;
					}

					foreach (int cell in map & HouseMaps[house])
					{
						short cands = (short)(grid.GetCandidates(cell) & linkMask);
						if (cands == 0)
						{
							continue;
						}

						foreach (int cand in cands)
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
					ImmutableArray.CreateRange(conclusions),
					ImmutableArray.Create(View.Empty | candidateOffsets),
					map
				);
				if (onlyFindOne)
				{
					return step;
				}

				accumulator.Add(step);
			}
		}

		return null;
	}
}
