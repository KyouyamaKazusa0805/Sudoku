namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with an <b>Almost Locked Sets W-Wing</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Almost Locked Sets W-Wing</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed partial class AlmostLockedSetsWWingStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? GetAll(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		var alses = grid.GatherAlmostLockedSets();

		// Gather all conjugate pairs.
		var conjugatePairs = Cached.Gather();

		// Iterate on each ALS.
		for (int i = 0, length = alses.Length; i < length - 1; i++)
		{
			var als1 = alses[i];
			var map1 = als1.Map;
			var house1 = als1.House;
			var mask1 = als1.DigitsMask;
			for (var j = i + 1; j < length; j++)
			{
				var als2 = alses[j];
				var map2 = als2.Map;
				var house2 = als2.House;
				var mask2 = als2.DigitsMask;

				// Now we have got two ALSes to check.
				// Firstly, we should check whether two ALSes overlap with each other.
				if (map1 && map2)
				{
					// If overlap (or in a same house), just skip it.
					continue;
				}

				if ((map1 | map2).InOneHouse)
				{
					continue;
				}

				// Then merge masks from two ALSes' into one using the operator &.
				var mask = (Mask)(mask1 & mask2);
				if (PopCount((uint)mask) < 2)
				{
					// If we can't find any digit that both two ALSes holds, the ALS-W-Wing won't form.
					// Just skip it.
					continue;
				}

				// Iterate on each digit that two ALSes both holds.
				foreach (var x in mask)
				{
					if (conjugatePairs[x] is not { Count: not 0 })
					{
						// If the digit 'x' doesn't contain any conjugate pairs,
						// we won't find any ALS-W-Wings, So just skip it.
						continue;
					}

					var p1 = map1 % CandidatesMap[x];
					var p2 = map2 % CandidatesMap[x];
					if (!p1 || !p2)
					{
						// At least one of two ALSes can't see the node of the conjugate pair.
						continue;
					}

					if (conjugatePairs[x] is { } conjPairs)
					{
						var wDigitsMask = (Mask)0;
						var conclusions = new List<Conclusion>();

						// Iterate on each conjugate pair.
						foreach (var conjugatePair in conjPairs)
						{
							var cpMap = conjugatePair.Map;
							if (cpMap && map1 || cpMap && map2)
							{
								// Conjugate pair can't overlap with the ALS structure.
								continue;
							}

							if ((cpMap & p1).Count != 1 || (cpMap & p2).Count != 1 || ((p1 | p2) & cpMap).Count != 2)
							{
								// If so, the structure may be a grouped ALS-W-Wing,
								// but I may not implement this one at present, so just skip it.
								continue;
							}

							// Iterate on each digit as the digit 'w'.
							foreach (var w in (Mask)(mask & ~(1 << x)))
							{
								if ((map1 | map2) % CandidatesMap[w] is not (var tempMap and not []))
								{
									continue;
								}

								wDigitsMask |= (Mask)(1 << w);
								foreach (var cell in tempMap)
								{
									conclusions.Add(new(Elimination, cell, w));
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
							foreach (var cell in map1)
							{
								foreach (var digit in grid.GetCandidates(cell))
								{
									candidateOffsets.Add(
										new(
											(digit == x, (wDigitsMask >> digit & 1) != 0) switch
											{
												(true, _) => DisplayColorKind.Auxiliary1,
												(_, true) => DisplayColorKind.Auxiliary2,
												_ => DisplayColorKind.AlmostLockedSet1
											},
											cell * 9 + digit
										)
									);
								}
							}
							foreach (var cell in map2)
							{
								foreach (var digit in grid.GetCandidates(cell))
								{
									candidateOffsets.Add(
										new(
											(digit == x, (wDigitsMask >> digit & 1) != 0) switch
											{
												(true, _) => DisplayColorKind.Auxiliary1,
												(_, true) => DisplayColorKind.Auxiliary2,
												_ => DisplayColorKind.AlmostLockedSet2
											},
											cell * 9 + digit
										)
									);
								}
							}

							var step = new AlmostLockedSetsWWingStep(
								conclusions.ToArray(),
								new[]
								{
									View.Empty
										| candidateOffsets
										| new HouseViewNode[]
										{
											new(DisplayColorKind.AlmostLockedSet1, house1),
											new(DisplayColorKind.AlmostLockedSet2, house2),
											new(DisplayColorKind.Normal, TrailingZeroCount(conjugatePair.Houses))
										}
								},
								als1,
								als2,
								conjugatePair,
								wDigitsMask,
								x
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
		}

		return null;
	}
}

/// <summary>
/// Represents a cached gathering operation set.
/// </summary>
file static class Cached
{
	/// <summary>
	/// Gathers possible conjugate pairs grouped by digit.
	/// </summary>
	/// <returns>The conjugate pairs found, grouped by digit.</returns>
	internal static ICollection<Conjugate>?[] Gather()
	{
		var conjugatePairs = new ICollection<Conjugate>?[9];
		for (var digit = 0; digit < 9; digit++)
		{
			for (var houseIndex = 0; houseIndex < 27; houseIndex++)
			{
				if ((HousesMap[houseIndex] & CandidatesMap[digit]) is { Count: 2 } temp)
				{
					(conjugatePairs[digit] ??= new List<Conjugate>()).Add(new(temp, digit));
				}
			}
		}

		return conjugatePairs;
	}
}
