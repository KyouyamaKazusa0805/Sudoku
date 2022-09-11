namespace Sudoku.Solving.Implementations.Searchers;

[StepSearcher]
internal sealed unsafe partial class AlmostLockedSetsXyWingStepSearcher : IAlmostLockedSetsXyWingStepSearcher
{
	/// <inheritdoc/>
	[StepSearcherProperty]
	public bool AllowCollision { get; set; }


	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		var rccs = new List<(AlmostLockedSet Left, AlmostLockedSet Right, short Mask)>();
		var alses = IAlmostLockedSetsStepSearcher.Gather(grid);

		// Gather all RCCs.
		for (int i = 0, length = alses.Length, iterationLengthOuter = length - 1; i < iterationLengthOuter; i++)
		{
			var als1 = alses[i];
			var map1 = als1.Map;
			var mask1 = als1.DigitsMask;
			for (var j = i + 1; j < length; j++)
			{
				var als2 = alses[j];
				var map2 = als2.Map;
				var mask2 = als2.DigitsMask;
				var map = map1 | map2;
				if (map.InOneHouse || (map1 & map2) is not [])
				{
					continue;
				}

				if ((mask1 & mask2) is var mask and not 0)
				{
					short rccMask = 0;
					foreach (var digit in mask)
					{
						if ((map & CandidatesMap[digit]).InOneHouse)
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
			for (var j = i + 1; j < count; j++)
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

				int aHouse = a.House, bHouse = b.House, cHouse = c.House;
				short aMask = a.DigitsMask, bMask = b.DigitsMask;
				CellMap aMap = a.Map, bMap = b.Map, cMap = c.Map;
				var map = aMap | bMap;
				if (map == aMap || map == bMap)
				{
					continue;
				}

				if (!AllowCollision && (aMap && bMap || aMap && cMap || bMap && cMap) is not [])
				{
					continue;
				}

				foreach (var digit1 in mask1)
				{
					foreach (var digit2 in mask2)
					{
						if (digit1 == digit2)
						{
							continue;
						}

						short finalX = (short)(1 << digit1), finalY = (short)(1 << digit2);
						var digitsMask = (short)(aMask & bMask & ~(finalX | finalY));
						if (digitsMask == 0)
						{
							continue;
						}

						// Gather eliminations.
						short finalZ = 0;
						var conclusions = new List<Conclusion>();
						foreach (var digit in digitsMask)
						{
							var elimMap = (aMap | bMap) % CandidatesMap[digit] - (aMap | bMap | cMap);
							if (!elimMap)
							{
								continue;
							}

							finalZ |= (short)(1 << digit);
							foreach (var cell in elimMap)
							{
								conclusions.Add(new(Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						// Record highlight candidates and cells.
						var candidateOffsets = new List<CandidateViewNode>();
						foreach (var cell in aMap)
						{
							var mask = grid.GetCandidates(cell);
							var alsDigitsMask = (short)(mask & ~(finalX | finalZ));
							var xDigitsMask = (short)(mask & finalX);
							var zDigitsMask = (short)(mask & finalZ);
							foreach (var digit in alsDigitsMask)
							{
								candidateOffsets.Add(new(DisplayColorKind.AlmostLockedSet1, cell * 9 + digit));
							}
							foreach (var digit in xDigitsMask)
							{
								candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
							}
							foreach (var digit in zDigitsMask)
							{
								candidateOffsets.Add(new(DisplayColorKind.Auxiliary2, cell * 9 + digit));
							}
						}
						foreach (var cell in bMap)
						{
							var mask = grid.GetCandidates(cell);
							var alsDigitsMask = (short)(mask & ~(finalY | finalZ));
							var yDigitsMask = (short)(mask & finalY);
							var zDigitsMask = (short)(mask & finalZ);
							foreach (var digit in alsDigitsMask)
							{
								candidateOffsets.Add(new(DisplayColorKind.AlmostLockedSet1, cell * 9 + digit));
							}
							foreach (var digit in yDigitsMask)
							{
								candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
							}
							foreach (var digit in zDigitsMask)
							{
								candidateOffsets.Add(new(DisplayColorKind.AlmostLockedSet2, cell * 9 + digit));
							}
						}
						foreach (var cell in cMap)
						{
							var mask = grid.GetCandidates(cell);
							var alsDigitsMask = (short)(mask & ~(finalX | finalY));
							var xyDigitsMask = (short)(mask & (finalX | finalY));
							foreach (var digit in alsDigitsMask)
							{
								candidateOffsets.Add(new(DisplayColorKind.AlmostLockedSet1, cell * 9 + digit));
							}
							foreach (var digit in xyDigitsMask)
							{
								candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
							}
						}

						var step = new AlmostLockedSetsXyWingStep(
							ImmutableArray.CreateRange(conclusions),
							ImmutableArray.Create(
								View.Empty
									| candidateOffsets
									| new HouseViewNode[]
									{
										new(DisplayColorKind.AlmostLockedSet1, aHouse),
										new(DisplayColorKind.AlmostLockedSet2, bHouse),
										new(DisplayColorKind.AlmostLockedSet3, cHouse)
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
