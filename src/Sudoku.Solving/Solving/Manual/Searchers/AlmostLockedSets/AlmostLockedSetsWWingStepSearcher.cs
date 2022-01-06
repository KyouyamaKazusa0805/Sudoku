namespace Sudoku.Solving.Manual.Searchers.AlmostLockedSets;

/// <summary>
/// Provides with an <b>Almost Locked Sets W-Wing</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Almost Locked Sets W-Wing</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe class AlmostLockedSetsWWingStepSearcher : IAlmostLockedSetsWWingStepSearcher
{
	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(28, DisplayingLevel.B);


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		var alses = AlmostLockedSet.Gather(grid);

		// Gather all conjugate pairs.
		var conjugatePairs = new ICollection<ConjugatePair>?[9];
		for (int digit = 0; digit < 9; digit++)
		{
			for (int region = 0; region < 27; region++)
			{
				if ((RegionMaps[region] & CandMaps[digit]) is [_, _] temp)
				{
					(conjugatePairs[digit] ??= new List<ConjugatePair>()).Add(new(temp, digit));
				}
			}
		}

		// Iterate on each ALS.
		for (int i = 0, length = alses.Length, iterationLength = length - 1; i < iterationLength; i++)
		{
			ref readonly var als1 = ref alses[i];
			var (_, region1, mask1, map1, _, _) = als1;
			for (int j = i + 1; j < length; j++)
			{
				ref readonly var als2 = ref alses[j];
				var (_, region2, mask2, map2, _, _) = als2;

				// Now we have got two ALSes to check.
				// Firstly, we should check whether two ALSes overlap with each other.
				if (!(map1 & map2).IsEmpty || (map1 | map2).InOneRegion)
				{
					// If overlap (or in a same region), just skip it.
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

					Cells p1 = map1 % CandMaps[x], p2 = map2 % CandMaps[x];
					if (p1.IsEmpty || p2.IsEmpty)
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
							if (!(cpMap & map1).IsEmpty || !(cpMap & map2).IsEmpty)
							{
								// Conjugate pair can't overlap with the ALS structure.
								continue;
							}

							if ((cpMap & p1).Count != 1 || (cpMap & p2).Count != 1
								|| ((p1 | p2) & cpMap).Count != 2)
							{
								// If so, the structure may be a grouped ALS-W-Wing,
								// but I don't implement this one, so just skip it.
								continue;
							}

							// Iterate on each digit as the digit 'w'.
							foreach (int w in mask & ~(1 << x))
							{
								var tempMap = (map1 | map2) % CandMaps[w];
								if (tempMap.IsEmpty)
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
							var candidateOffsets = new List<(int, ColorIdentifier)>
							{
								(cpMap[0] * 9 + x, (ColorIdentifier)0),
								(cpMap[1] * 9 + x, (ColorIdentifier)0)
							};
							foreach (int cell in map1)
							{
								foreach (int digit in grid.GetCandidates(cell))
								{
									candidateOffsets.Add(
										(
											cell * 9 + digit,
											(ColorIdentifier)(
												digit == x ? 1 : (wDigitsMask >> digit & 1) != 0 ? 2 : 101
											)
										)
									);
								}
							}
							foreach (int cell in map2)
							{
								foreach (int digit in grid.GetCandidates(cell))
								{
									candidateOffsets.Add(
										(
											cell * 9 + digit,
											(ColorIdentifier)(
												digit == x ? 1 : (wDigitsMask >> digit & 1) != 0 ? 2 : 102
											)
										)
									);
								}
							}

							accumulator.Add(
								new AlmostLockedSetsWWingStep(
									conclusions.ToImmutableArray(),
									ImmutableArray.Create(new PresentationData
									{
										Candidates = candidateOffsets,
										Regions = new[]
										{
											(region1, (ColorIdentifier)101),
											(region2, (ColorIdentifier)102),
											(TrailingZeroCount(conjugatePair.Regions), (ColorIdentifier)0)
										}
									}),
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
