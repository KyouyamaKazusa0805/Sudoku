namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with an <b>Almost Locked Sets XY-Wing</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Almost Locked Sets XY-Wing</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed partial class AlmostLockedSetsXyWingStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates whether two ALSes make an collision, which means they share the some same cells. 
	/// </summary>
	public bool AllowCollision { get; set; }


	/// <inheritdoc/>
	protected internal override Step? GetAll(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		var rccList = new List<(AlmostLockedSet Left, AlmostLockedSet Right, short Mask)>();
		var alses = grid.GatherAlmostLockedSets();

		// Gather all RCCs.
		for (int i = 0, length = alses.Length; i < length - 1; i++)
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
					var rccMask = (short)0;
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

					rccList.Add((als1, als2, rccMask));
				}
			}
		}

		// Now check them.
		for (int i = 0, count = rccList.Count; i < count - 1; i++)
		{
			var (als11, als12, mask1) = rccList[i];
			for (var j = i + 1; j < count; j++)
			{
				var (als21, als22, mask2) = rccList[j];
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
				var (a, b, c) = (als11 == als21, als11 == als22, als12 == als21) switch
				{
					(true, _, _) => (als12, als22, als11),
					(_, true, _) => (als12, als21, als11),
					(_, _, true) => (als11, als22, als12),
					_ => (als11, als21, als12)
				};

				var aHouse = a.House;
				var bHouse = b.House;
				var cHouse = c.House;
				var aMask = a.DigitsMask;
				var bMask = b.DigitsMask;
				var aMap = a.Map;
				var bMap = b.Map;
				var cMap = c.Map;
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

						var finalX = (short)(1 << digit1);
						var finalY = (short)(1 << digit2);
						var digitsMask = (short)(aMask & bMask & ~(finalX | finalY));
						if (digitsMask == 0)
						{
							continue;
						}

						// Gather eliminations.
						var finalZ = (short)0;
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
							conclusions.ToArray(),
							new[]
							{
								View.Empty
									| candidateOffsets
									| new HouseViewNode[]
									{
										new(DisplayColorKind.AlmostLockedSet1, aHouse),
										new(DisplayColorKind.AlmostLockedSet2, bHouse),
										new(DisplayColorKind.AlmostLockedSet3, cHouse)
									}
							},
							a,
							b,
							c,
							finalX,
							finalY,
							finalZ
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

		return null;
	}
}
