namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with an <b>Almost Locked Sets XZ Rule</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Extended Subset Principle</item>
/// <item>Singly-linked Almost Locked Sets XZ Rule</item>
/// <item>Doubly-linked Almost Locked Sets XZ Rule</item>
/// </list>
/// </summary>
[StepSearcher(Technique.SinglyLinkedAlmostLockedSetsXzRule, Technique.DoublyLinkedAlmostLockedSetsXzRule, Technique.ExtendedSubsetPrinciple)]
public sealed partial class AlmostLockedSetsXzStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates whether two ALSes make an collision, which means they share the some same cells. 
	/// </summary>
	[RuntimeIdentifier(RuntimeIdentifier.AllowCollisionOnAlmostLockedSetXzRule)]
	public bool AllowCollision { get; set; }

	/// <summary>
	/// Indicates whether the searcher will enhance the searching to find all possible eliminations
	/// for looped-ALS eliminations.
	/// </summary>
	[RuntimeIdentifier(RuntimeIdentifier.AllowLoopedPatternsOnAlmostLockedSetXzRule)]
	public bool AllowLoopedPatterns { get; set; }


	/// <inheritdoc/>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="/g/developer-notes" />
	/// <para>
	/// This algorithm uses a concept called Restricted Common Candidate (abbr. RCC) to limit the implementation.
	/// If you don't know that is an RCC,
	/// <see href="https://sunnieshine.github.io/Sudoku/terms/restricted-common-candidate">this link</see>
	/// will tell you what is it.
	/// </para>
	/// </remarks>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		var house = (stackalloc House[2]);
		var alses = grid.GatherAlmostLockedSets();
		for (var (i, length) = (0, alses.Length); i < length - 1; i++)
		{
			var als1 = alses[i];
			var house1 = als1.House;
			var mask1 = als1.DigitsMask;
			var map1 = als1.Cells;
			var possibleElimMap1 = als1.PossibleEliminationMap;
			for (var j = i + 1; j < length; j++)
			{
				var als2 = alses[j];
				var house2 = als2.House;
				var mask2 = als2.DigitsMask;
				var map2 = als2.Cells;
				var possibleElimMap2 = als2.PossibleEliminationMap;
				var xzMask = (Mask)(mask1 & mask2);
				var map = map1 | map2;
				var overlapMap = map1 & map2;
				if (!AllowCollision && overlapMap is not [])
				{
					continue;
				}

				// Remove all digits to satisfy that the RCC digit can't appear
				// in the intersection of two ALSes.
				foreach (var cell in overlapMap)
				{
					xzMask &= (Mask)~grid.GetCandidates(cell);
				}

				// If the number of digits that both two ALSes contain is only one (or zero),
				// the ALS-XZ won't be formed.
				if (PopCount((uint)xzMask) < 2 || map.AllSetsAreInOneHouse(out var houseIndex))
				{
					continue;
				}

				var rccMask = (Mask)0;
				var z = (Mask)0;
				var nh = 0;
				foreach (var digit in xzMask)
				{
					if ((map & CandidatesMap[digit]).AllSetsAreInOneHouse(out houseIndex))
					{
						// 'digit' is the RCC digit.
						rccMask |= (Mask)(1 << digit);
						house[nh++] = houseIndex;
					}
					else
					{
						// 'digit' is the eliminating digit.
						z |= (Mask)(1 << digit);
					}
				}

				if (rccMask == 0 || (rccMask & rccMask - 1) == 0 && z == 0)
				{
					continue;
				}

				// Check basic eliminations.
				var isDoublyLinked = (bool?)false;
				var finalZ = (Mask)0;
				var conclusions = new List<Conclusion>();
				foreach (var elimDigit in z)
				{
					if (map % CandidatesMap[elimDigit] is not (var elimMap and not []))
					{
						continue;
					}

					foreach (var cell in elimMap)
					{
						conclusions.Add(new(Elimination, cell, elimDigit));
					}

					finalZ |= (Mask)(1 << elimDigit);
				}

				if (AllowLoopedPatterns && PopCount((uint)rccMask) == 2)
				{
					// Doubly linked ALS-XZ.
					isDoublyLinked = true;
					foreach (var elimDigit in (Mask)(z & ~rccMask))
					{
						if ((CandidatesMap[elimDigit] & map1) is not (var zMap and not []))
						{
							continue;
						}

						if ((zMap.PeerIntersection & CandidatesMap[elimDigit] & map2) is not (var elimMap and not []))
						{
							continue;
						}

						finalZ |= (Mask)(1 << elimDigit);
					}

					// RCC digit 2 eliminations.
					var k = 0;
					foreach (var digit in rccMask)
					{
						foreach (var cell in (HousesMap[house[k]] & CandidatesMap[digit]) - map)
						{
							conclusions.Add(new(Elimination, cell, digit));
						}

						k++;
					}

					// Possible eliminations.
					var tempMap = map;
					tempMap = CandidatesMap[TrailingZeroCount(mask1)];
					foreach (var digit in mask1.SkipSetBit(1))
					{
						tempMap |= CandidatesMap[digit];
					}
					tempMap &= possibleElimMap1;
					foreach (var cell in tempMap)
					{
						foreach (var digit in (Mask)(grid.GetCandidates(cell) & (mask1 & ~rccMask)))
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
					tempMap = CandidatesMap[TrailingZeroCount(mask2)];
					foreach (var digit in mask2.SkipSetBit(1))
					{
						tempMap |= CandidatesMap[digit];
					}
					tempMap &= possibleElimMap2;
					foreach (var cell in tempMap)
					{
						foreach (var digit in (Mask)(grid.GetCandidates(cell) & (mask2 & ~rccMask)))
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
				}

				if (conclusions.Count == 0)
				{
					continue;
				}

				// Now record highlight elements.
				var isEsp = als1.IsBivalueCell || als2.IsBivalueCell;
				var candidateOffsets = new List<CandidateViewNode>();
				if (isEsp)
				{
					foreach (var cell in map)
					{
						foreach (var digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new((WellKnownColorIdentifierKind)(finalZ >> digit & 1), cell * 9 + digit));
						}
					}
				}
				else
				{
					foreach (var cell in map1)
					{
						var mask = grid.GetCandidates(cell);
						var alsDigitsMask = (Mask)(mask & ~(finalZ | rccMask));
						var targetDigitsMask = (Mask)(mask & finalZ);
						var rccDigitsMask = (Mask)(mask & rccMask);
						foreach (var digit in alsDigitsMask)
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.AlmostLockedSet1, cell * 9 + digit));
						}
						foreach (var digit in targetDigitsMask)
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, cell * 9 + digit));
						}
						foreach (var digit in rccDigitsMask)
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, cell * 9 + digit));
						}
					}
					foreach (var cell in map2)
					{
						var mask = grid.GetCandidates(cell);
						var alsDigitsMask = (Mask)(mask & ~(finalZ | rccMask));
						var targetDigitsMask = (Mask)(mask & finalZ);
						var rccDigitsMask = (Mask)(mask & rccMask);
						foreach (var digit in alsDigitsMask)
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.AlmostLockedSet2, cell * 9 + digit));
						}
						foreach (var digit in targetDigitsMask)
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, cell * 9 + digit));
						}
						foreach (var digit in rccDigitsMask)
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, cell * 9 + digit));
						}
					}
				}

				var step = new AlmostLockedSetsXzStep(
					conclusions.ToArray(),
					new[]
					{
						View.Empty
							| candidateOffsets
							| (isEsp ? null : new HouseViewNode[] { new(WellKnownColorIdentifier.Normal, house1), new(WellKnownColorIdentifier.Auxiliary1, house2) })
					},
					als1,
					als2,
					rccMask,
					finalZ,
					isEsp ? null : isDoublyLinked
				);
				if (context.OnlyFindOne)
				{
					return step;
				}

				context.Accumulator.Add(step);
			}

			i++;
		}

		return null;
	}
}
