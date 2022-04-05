namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with an <b>Almost Locked Sets XY-Wing</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Almost Locked Sets XY-Wing</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe partial class AlmostLockedSetsXyWingStepSearcher : IAlmostLockedSetsXyWingStepSearcher
{
	/// <inheritdoc/>
	public bool AllowCollision { get; set; }


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		var rccs = new List<(AlmostLockedSet Left, AlmostLockedSet Right, short Mask)>();
		var alses = IAlmostLockedSetsStepSearcher.Gather(grid);

		// Gather all RCCs.
		for (int i = 0, length = alses.Length, iterationLengthOuter = length - 1; i < iterationLengthOuter; i++)
		{
			var als1 = alses[i];
			var map1 = als1.Map;
			short mask1 = als1.DigitsMask;
			for (int j = i + 1; j < length; j++)
			{
				var als2 = alses[j];
				var map2 = als2.Map;
				short mask2 = als2.DigitsMask;
				var map = map1 | map2;
				if (map.InOneRegion || (map1 & map2) is not [])
				{
					continue;
				}

				if ((mask1 & mask2) is var mask and not 0)
				{
					short rccMask = 0;
					foreach (int digit in mask)
					{
						if ((map & CandMaps[digit]).InOneRegion)
						{
							rccMask |= (short)(1 << digit);
						}
					}
					if (rccMask == 0)
					{
						continue;
					}

					rccs.Add((als1, als2, rccMask));
				}
			}
		}

		// Now check them.
		for (int i = 0, count = rccs.Count, iterationCountOuter = count - 1; i < iterationCountOuter; i++)
		{
			var (als11, als12, mask1) = rccs[i];
			for (int j = i + 1; j < count; j++)
			{
				var (als21, als22, mask2) = rccs[j];
				if (mask1 == mask2 && IsPow2(mask1) && IsPow2(mask2))
				{
					// Cannot form a XY-Wing.
					continue;
				}

				if (!(als11 == als21 ^ als12 == als22 || als11 == als22 ^ als12 == als21))
				{
					continue;
				}

				// Get the logical order of three ALSes.
				var (a, b, c) = als11 == als21
					? (als12, als22, als11)
					: als11 == als22
						? (als12, als21, als11)
						: als12 == als21 ? (als11, als22, als12) : (als11, als21, als12);

				int aRegion = a.Region, bRegion = b.Region, cRegion = c.Region;
				short aMask = a.DigitsMask, bMask = b.DigitsMask;
				Cells aMap = a.Map, bMap = b.Map, cMap = c.Map;
				var map = aMap | bMap;
				if (map == aMap || map == bMap)
				{
					continue;
				}

				if (!AllowCollision
					&& ((aMap & bMap) is not [] || (aMap & cMap) is not [] || (bMap & cMap) is not []))
				{
					continue;
				}

				foreach (int digit1 in mask1)
				{
					foreach (int digit2 in mask2)
					{
						if (digit1 == digit2)
						{
							continue;
						}

						short finalX = (short)(1 << digit1), finalY = (short)(1 << digit2);
						short digitsMask = (short)(aMask & bMask & ~(finalX | finalY));
						if (digitsMask == 0)
						{
							continue;
						}

						// Gather eliminations.
						short finalZ = 0;
						var conclusions = new List<Conclusion>();
						foreach (int digit in digitsMask)
						{
							var elimMap = (aMap | bMap) % CandMaps[digit] - (aMap | bMap | cMap);
							if (elimMap is [])
							{
								continue;
							}

							finalZ |= (short)(1 << digit);
							foreach (int cell in elimMap)
							{
								conclusions.Add(new(ConclusionType.Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						// Record highlight candidates and cells.
						var candidateOffsets = new List<CandidateViewNode>();
						foreach (int cell in aMap)
						{
							short mask = grid.GetCandidates(cell);
							short alsDigitsMask = (short)(mask & ~(finalX | finalZ));
							short xDigitsMask = (short)(mask & (finalX));
							short zDigitsMask = (short)(mask & finalZ);
							foreach (int digit in alsDigitsMask)
							{
								candidateOffsets.Add(new(101, cell * 9 + digit));
							}
							foreach (int digit in xDigitsMask)
							{
								candidateOffsets.Add(new(1, cell * 9 + digit));
							}
							foreach (int digit in zDigitsMask)
							{
								candidateOffsets.Add(new(2, cell * 9 + digit));
							}
						}
						foreach (int cell in bMap)
						{
							short mask = grid.GetCandidates(cell);
							short alsDigitsMask = (short)(mask & ~(finalY | finalZ));
							short yDigitsMask = (short)(mask & finalY);
							short zDigitsMask = (short)(mask & finalZ);
							foreach (int digit in alsDigitsMask)
							{
								candidateOffsets.Add(new(101, cell * 9 + digit));
							}
							foreach (int digit in yDigitsMask)
							{
								candidateOffsets.Add(new(1, cell * 9 + digit));
							}
							foreach (int digit in zDigitsMask)
							{
								candidateOffsets.Add(new(102, cell * 9 + digit));
							}
						}
						foreach (int cell in cMap)
						{
							short mask = grid.GetCandidates(cell);
							short alsDigitsMask = (short)(mask & ~(finalX | finalY));
							short xyDigitsMask = (short)(mask & (finalX | finalY));
							foreach (int digit in alsDigitsMask)
							{
								candidateOffsets.Add(new(103, cell * 9 + digit));
							}
							foreach (int digit in xyDigitsMask)
							{
								candidateOffsets.Add(new(1, cell * 9 + digit));
							}
						}

						var step = new AlmostLockedSetsXyWingStep(
							conclusions.ToImmutableArray(),
							ImmutableArray.Create(
								View.Empty
									+ candidateOffsets
									+ new RegionViewNode[]
									{
										new(101, aRegion),
										new(102, bRegion),
										new(103, cRegion)
									}
							),
							a,
							b,
							c,
							finalX,
							finalY,
							finalZ
						);
						if (onlyFindOne)
						{
							return step;
						}

						accumulator.Add(step);
					}
				}
			}
		}

		return null;
	}
}
