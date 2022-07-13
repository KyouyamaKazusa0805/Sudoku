namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with an <b>Almost Locked Sets W-Wing</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Almost Locked Sets W-Wing</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe partial class AlmostLockedSetsWWingStepSearcher : IAlmostLockedSetsWWingStepSearcher
{
	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		var alses = IAlmostLockedSetsStepSearcher.Gather(grid);

		// Gather all conjugate pairs.
		var conjugatePairs = new ICollection<Conjugate>?[9];
		for (int digit = 0; digit < 9; digit++)
		{
			for (int houseIndex = 0; houseIndex < 27; houseIndex++)
			{
				if ((HouseMaps[houseIndex] & CandidatesMap[digit]) is { Count: 2 } temp)
				{
					(conjugatePairs[digit] ??= new List<Conjugate>()).Add(new(temp, digit));
				}
			}
		}

		// Iterate on each ALS.
		for (int i = 0, length = alses.Length, iterationLength = length - 1; i < iterationLength; i++)
		{
			var als1 = alses[i];
			var map1 = als1.Map;
			int house1 = als1.House;
			short mask1 = als1.DigitsMask;
			for (int j = i + 1; j < length; j++)
			{
				var als2 = alses[j];
				var map2 = als2.Map;
				int house2 = als2.House;
				short mask2 = als2.DigitsMask;

				// Now we have got two ALSes to check.
				// Firstly, we should check whether two ALSes overlap with each other.
				if ((map1 & map2) is not [] || (map1 | map2).InOneHouse)
				{
					// If overlap (or in a same house), just skip it.
					continue;
				}

				// Then merge masks from two ALSes' into one using the operator &.
				short mask = (short)(mask1 & mask2);
				if (PopCount((uint)mask) < 2)
				{
					// If we can't find any digit that both two ALSes holds, the ALS-W-Wing won't form.
					// Just skip it.
					continue;
				}

				// Iterate on each digit that two ALSes both holds.
				foreach (int x in mask)
				{
					if (conjugatePairs[x] is not { Count: not 0 })
					{
						// If the digit 'x' doesn't contain any conjugate pairs,
						// we won't find any ALS-W-Wings, So just skip it.
						continue;
					}

					Cells p1 = map1 % CandidatesMap[x], p2 = map2 % CandidatesMap[x];
					if (p1 is [] || p2 is [])
					{
						// At least one of two ALSes can't see the node of the conjugate pair.
						continue;
					}

					if (conjugatePairs[x] is { } conjPairs)
					{
						short wDigitsMask = 0;
						var conclusions = new List<Conclusion>();

						// Iterate on each conjugate pair.
						foreach (var conjugatePair in conjPairs)
						{
							var cpMap = conjugatePair.Map;
							if ((cpMap & map1) is not [] || (cpMap & map2) is not [])
							{
								// Conjugate pair can't overlap with the ALS structure.
								continue;
							}

							if ((cpMap & p1).Count != 1 || (cpMap & p2).Count != 1 || ((p1 | p2) & cpMap).Count != 2)
							{
								// If so, the structure may be a grouped ALS-W-Wing,
								// but I don't implement this one, so just skip it.
								continue;
							}

							// Iterate on each digit as the digit 'w'.
							foreach (int w in mask & ~(1 << x))
							{
								if ((map1 | map2) % CandidatesMap[w] is not { Count: not 0 } tempMap)
								{
									continue;
								}

								wDigitsMask |= (short)(1 << w);
								foreach (int cell in tempMap)
								{
									conclusions.Add(new(ConclusionType.Elimination, cell, w));
								}
							}

							// Check the existence of the eliminations.
							if (conclusions.Count == 0)
							{
								continue;
							}

							// Gather highlight cells and candidates.
							var candidateOffsets = new List<CandidateViewNode>
							{
								new(DisplayColorKind.Normal, cpMap[0] * 9 + x),
								new(DisplayColorKind.Normal, cpMap[1] * 9 + x)
							};
							foreach (int cell in map1)
							{
								foreach (int digit in grid.GetCandidates(cell))
								{
									candidateOffsets.Add(
										new(
											digit == x
												? DisplayColorKind.Auxiliary1
												: (wDigitsMask >> digit & 1) != 0
													? DisplayColorKind.Auxiliary2
													: DisplayColorKind.AlmostLockedSet1,
											cell * 9 + digit
										)
									);
								}
							}
							foreach (int cell in map2)
							{
								foreach (int digit in grid.GetCandidates(cell))
								{
									candidateOffsets.Add(
										new(
											digit == x
												? DisplayColorKind.Auxiliary1
												: (wDigitsMask >> digit & 1) != 0
													? DisplayColorKind.Auxiliary2
													: DisplayColorKind.AlmostLockedSet2,
											cell * 9 + digit
										)
									);
								}
							}

							accumulator.Add(
								new AlmostLockedSetsWWingStep(
									ImmutableArray.CreateRange(conclusions),
									ImmutableArray.Create(
										View.Empty
											| candidateOffsets
											| new HouseViewNode[]
											{
												new(DisplayColorKind.AlmostLockedSet1, house1),
												new(DisplayColorKind.AlmostLockedSet2, house2),
												new(DisplayColorKind.Normal, TrailingZeroCount(conjugatePair.Houses))
											}
									),
									als1,
									als2,
									conjugatePair,
									wDigitsMask,
									x
								)
							);
						}
					}
				}
			}
		}

		return null;
	}
}
