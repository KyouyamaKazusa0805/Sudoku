namespace Sudoku.Solving.Manual.Searchers;

[StepSearcher]
internal sealed unsafe partial class RegularWingStepSearcher : IRegularWingStepSearcher
{
	/// <inheritdoc/>
	public int MaxSize { get; set; }


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		// Iterate on the size.
		// Note that the greatest size is determined by two factors: the size that you specified
		// and the number of bi-value cells in the grid.
		for (int size = 3, count = Min(MaxSize, BivalueCells.Count); size <= count; size++)
		{
			// Iterate on each pivot cell.
			foreach (int pivot in EmptyCells)
			{
				short mask = grid.GetCandidates(pivot);
				int candsCount = PopCount((uint)mask);
				if (candsCount != size && candsCount != size - 1)
				{
					// Candidates are not enough.
					continue;
				}

				var map = PeerMaps[pivot] & BivalueCells;
				if (map.Count < size - 1)
				{
					// Bi-value cells are not enough.
					continue;
				}

				// Iterate on each cell combination.
				foreach (var cells in map & size - 1)
				{
					// Check duplicate.
					// If two cells contain same candidates, the wing can't be formed.
					bool flag = false;
					for (int i = 0, length = cells.Count, outerLength = length - 1; i < outerLength; i++)
					{
						for (int j = i + 1; j < length; j++)
						{
							if (grid.GetMask(cells[i]) == grid.GetMask(cells[j]))
							{
								flag = true;
								goto CheckWhetherTwoCellsContainSameCandidateKind;
							}
						}
					}

				CheckWhetherTwoCellsContainSameCandidateKind:
					if (flag)
					{
						continue;
					}

					short union = mask, inter = (short)(Grid.MaxCandidatesMask & mask);
					foreach (int cell in cells)
					{
						short m = grid.GetCandidates(cell);
						union |= m;
						inter &= m;
					}

					if (PopCount((uint)union) != size || inter != 0 && !IsPow2(inter))
					{
						continue;
					}

					// Get the Z digit (The digit to be removed).
					bool isIncomplete = inter == 0;
					short interWithoutPivot = (short)(union & ~grid.GetCandidates(pivot));
					short maskToCheck = isIncomplete ? interWithoutPivot : inter;
					if (!IsPow2(maskToCheck))
					{
						continue;
					}

					// The pattern should be "az, bz, cz, dz, ... , abcd(z)".
					int zDigit = TrailingZeroCount(maskToCheck);
					var petals = cells;
					if ((petals + pivot & CandidatesMap[zDigit]).Count != (isIncomplete ? size - 1 : size))
					{
						continue;
					}

					// Check elimination map.
					var elimMap = !petals;
					if (!isIncomplete)
					{
						elimMap &= PeerMaps[pivot];
					}
					elimMap &= CandidatesMap[zDigit];
					if (elimMap is [])
					{
						continue;
					}

					// Gather highlight candidates.
					var candidateOffsets = new List<CandidateViewNode>(6);
					foreach (int cell in cells)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(
								new(
									digit == zDigit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
									cell * 9 + digit
								)
							);
						}
					}
					foreach (int digit in grid.GetCandidates(pivot))
					{
						candidateOffsets.Add(
							new(
								digit == zDigit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
								pivot * 9 + digit
							)
						);
					}

					var step = new RegularWingStep(
						ImmutableArray.Create(Conclusion.ToConclusions(elimMap, zDigit, ConclusionType.Elimination)),
						ImmutableArray.Create(View.Empty | candidateOffsets),
						pivot,
						PopCount((uint)mask),
						union,
						petals
					);

					if (onlyFindOne)
					{
						return step;
					}

					accumulator.Add(step);
				}
			}
		}

		return null;
	}
}
