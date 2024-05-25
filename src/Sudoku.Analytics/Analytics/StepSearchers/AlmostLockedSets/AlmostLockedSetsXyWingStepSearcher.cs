namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with an <b>Almost Locked Sets XY-Wing</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Almost Locked Sets XY-Wing</item>
/// </list>
/// </summary>
[StepSearcher("StepSearcherName_AlmostLockedSetsXyWingStepSearcher", Technique.AlmostLockedSetsXyWing)]
public sealed partial class AlmostLockedSetsXyWingStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates whether two ALSes make an collision, which means they share the some same cells. 
	/// </summary>
	[SettingItemName(SettingItemNames.AllowCollisionOnAlmostLockedSetXyWing)]
	public bool AllowCollision { get; set; }


	/// <inheritdoc/>
	protected internal override Step? Collect(ref AnalysisContext context)
	{
		ref readonly var grid = ref context.Grid;
		var alses = AlmostLockedSetsModule.CollectAlmostLockedSets(in grid);

		// Gather all RCCs.
		var rccList = new List<(AlmostLockedSet Left, AlmostLockedSet Right, Mask Mask)>();
		for (var (i, length) = (0, alses.Length); i < length - 1; i++)
		{
			var als1 = alses[i];
			var (map1, mask1) = (als1.Cells, als1.DigitsMask);
			for (var j = i + 1; j < length; j++)
			{
				var als2 = alses[j];
				var (map2, mask2) = (als2.Cells, als2.DigitsMask);

				var map = map1 | map2;
				if (map.InOneHouse(out _))
				{
					continue;
				}

				if (!AllowCollision && !!(map1 && map2))
				{
					continue;
				}

				if ((Mask)(mask1 & mask2) is var mask and not 0)
				{
					var rccMask = (Mask)0;
					foreach (var digit in mask)
					{
						if ((map & CandidatesMap[digit]).InOneHouse(out _))
						{
							rccMask |= (Mask)(1 << digit);
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
		for (var (i, count) = (0, rccList.Count); i < count - 1; i++)
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

				var (aMask, bMask) = (a.DigitsMask, b.DigitsMask);
				var (aMap, bMap, cMap) = (a.Cells, b.Cells, c.Cells);
				var map = aMap | bMap;
				if (map == aMap || map == bMap)
				{
					continue;
				}

				if (!AllowCollision && !!(aMap && bMap || aMap && cMap || bMap && cMap))
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

						var (finalX, finalY) = ((Mask)(1 << digit1), (Mask)(1 << digit2));
						if ((Mask)(aMask & bMask & ~(finalX | finalY)) is not (var digitsMask and not 0))
						{
							continue;
						}

						// Gather eliminations.
						var (finalZ, conclusions) = ((Mask)0, new List<Conclusion>());
						foreach (var digit in digitsMask)
						{
							if ((aMap | bMap) % (CandidatesMap[digit] & ~(aMap | bMap | cMap)) is not (var elimMap and not []))
							{
								continue;
							}

							finalZ |= (Mask)(1 << digit);
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
						var cellOffsets = new List<CellViewNode>();
						var candidateOffsets = new List<CandidateViewNode>();
						foreach (var cell in aMap)
						{
							cellOffsets.Add(new(ColorIdentifier.AlmostLockedSet1, cell));

							foreach (var digit in grid.GetCandidates(cell))
							{
								var colorIdentifier = (finalZ >> digit & 1) != 0
									? ColorIdentifier.Auxiliary2
									: ((finalX | finalY) >> digit & 1) != 0
										? ColorIdentifier.Auxiliary1
										: ColorIdentifier.AlmostLockedSet1;
								candidateOffsets.Add(new(colorIdentifier, cell * 9 + digit));
							}
						}
						foreach (var cell in bMap)
						{
							cellOffsets.Add(new(ColorIdentifier.AlmostLockedSet2, cell));

							foreach (var digit in grid.GetCandidates(cell))
							{
								var colorIdentifier = (finalZ >> digit & 1) != 0
									? ColorIdentifier.Auxiliary2
									: ((finalX | finalY) >> digit & 1) != 0
										? ColorIdentifier.Auxiliary1
										: ColorIdentifier.AlmostLockedSet2;
								candidateOffsets.Add(new(colorIdentifier, cell * 9 + digit));
							}
						}
						foreach (var cell in cMap)
						{
							cellOffsets.Add(new(ColorIdentifier.AlmostLockedSet3, cell));

							foreach (var digit in grid.GetCandidates(cell))
							{
								var colorIdentifier = ((finalX | finalY) >> digit & 1) != 0
									? ColorIdentifier.Auxiliary1
									: ColorIdentifier.AlmostLockedSet3;
								candidateOffsets.Add(new(colorIdentifier, cell * 9 + digit));
							}
						}

						var step = new AlmostLockedSetsXyWingStep(
							[.. conclusions],
							[[.. cellOffsets, .. candidateOffsets]],
							context.Options,
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
