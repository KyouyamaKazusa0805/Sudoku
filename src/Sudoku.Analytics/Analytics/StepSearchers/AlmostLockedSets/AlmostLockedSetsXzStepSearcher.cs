namespace Sudoku.Analytics.StepSearchers.AlmostLockedSets;

/// <summary>
/// Provides with an <b>Almost Locked Sets XZ Rule</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Extended Subset Principle</item>
/// <item>Singly-linked Almost Locked Sets XZ Rule</item>
/// <item>Doubly-linked Almost Locked Sets XZ Rule</item>
/// </list>
/// </summary>
[StepSearcher(
	"StepSearcherName_AlmostLockedSetsXzStepSearcher",
	Technique.SinglyLinkedAlmostLockedSetsXzRule, Technique.DoublyLinkedAlmostLockedSetsXzRule)]
public sealed partial class AlmostLockedSetsXzStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates whether two ALSes make an collision, which means they share the some same cells. 
	/// </summary>
	[SettingItemName(SettingItemNames.AllowCollisionOnAlmostLockedSetsXzRule)]
	public bool AllowCollision { get; set; }

	/// <summary>
	/// Indicates whether the searcher will enhance the searching to find all possible eliminations
	/// for looped-ALS eliminations.
	/// </summary>
	[SettingItemName(SettingItemNames.AllowLoopedPatternsOnAlmostLockedSetsXzRule)]
	public bool AllowLoopedPatterns { get; set; }


	/// <inheritdoc/>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="/g/developer-notes" />
	/// <para>
	/// This algorithm uses a concept called Restricted Common Candidate (abbr. RCC) to limit the implementation.
	/// If you don't know that is an RCC,
	/// <see href="http://sudopedia.enjoysudoku.com/Restricted_common.html">this link</see>
	/// will tell you what is it.
	/// </para>
	/// </remarks>
	[InterceptorMethodCaller]
	protected internal override Step? Collect(ref StepAnalysisContext context)
	{
		ref readonly var grid = ref context.Grid;
		var alses = AlmostLockedSetPattern.Collect(in grid);

		var house = (stackalloc House[2]);
		for (var (i, length) = (0, alses.Length); i < length - 1; i++)
		{
			var als1 = alses[i];
			var mask1 = als1.DigitsMask;
			var map1 = als1.Cells;
			var possibleElimMap1 = als1.PossibleEliminationMap;
			for (var j = i + 1; j < length; j++)
			{
				var als2 = alses[j];
				var mask2 = als2.DigitsMask;
				var map2 = als2.Cells;
				var possibleElimMap2 = als2.PossibleEliminationMap;
				var xzMask = (Mask)(mask1 & mask2);
				var map = map1 | map2;
				var overlapMap = map1 & map2;
				if (!AllowCollision && !!overlapMap)
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
				if (Mask.PopCount(xzMask) < 2 || map.FirstSharedHouse != 32)
				{
					continue;
				}

				var rccMask = (Mask)0;
				var z = (Mask)0;
				var nh = 0;
				foreach (var digit in xzMask)
				{
					if ((map & CandidatesMap[digit]).FirstSharedHouse is var houseIndex and not 32)
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
				var isDoublyLinked = false;
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

				if (AllowLoopedPatterns && Mask.PopCount(rccMask) == 2)
				{
					// Doubly linked ALS-XZ.
					isDoublyLinked = true;
					foreach (var elimDigit in (Mask)(z & ~rccMask))
					{
						if ((CandidatesMap[elimDigit] & map1) is not (var zMap and not []))
						{
							continue;
						}

						if (!(zMap.PeerIntersection & CandidatesMap[elimDigit] & map2))
						{
							continue;
						}

						finalZ |= (Mask)(1 << elimDigit);
					}

					// RCC digit 2 eliminations.
					var k = 0;
					foreach (var digit in rccMask)
					{
						foreach (var cell in HousesMap[house[k]] & CandidatesMap[digit] & ~map)
						{
							conclusions.Add(new(Elimination, cell, digit));
						}

						k++;
					}

					// Possible eliminations.
					var tempMap = map;
					tempMap = CandidatesMap[Mask.TrailingZeroCount(mask1)];
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
					tempMap = CandidatesMap[Mask.TrailingZeroCount(mask2)];
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

				// Now collect highlight elements.
				var cellOffsets = new List<CellViewNode>();
				var candidateOffsets = new List<CandidateViewNode>();
				foreach (var cell in map1)
				{
					cellOffsets.Add(new(ColorIdentifier.AlmostLockedSet1, cell));

					var mask = grid.GetCandidates(cell);
					var alsDigitsMask = (Mask)(mask & ~(finalZ | rccMask));
					var targetDigitsMask = (Mask)(mask & finalZ);
					var rccDigitsMask = (Mask)(mask & rccMask);
					foreach (var digit in alsDigitsMask)
					{
						candidateOffsets.Add(new(ColorIdentifier.AlmostLockedSet1, cell * 9 + digit));
					}
					foreach (var digit in targetDigitsMask)
					{
						candidateOffsets.Add(new(ColorIdentifier.Auxiliary2, cell * 9 + digit));
					}
					foreach (var digit in rccDigitsMask)
					{
						candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, cell * 9 + digit));
					}
				}
				foreach (var cell in map2)
				{
					cellOffsets.Add(new(ColorIdentifier.AlmostLockedSet2, cell));

					var mask = grid.GetCandidates(cell);
					var alsDigitsMask = (Mask)(mask & ~(finalZ | rccMask));
					var targetDigitsMask = (Mask)(mask & finalZ);
					var rccDigitsMask = (Mask)(mask & rccMask);
					foreach (var digit in alsDigitsMask)
					{
						candidateOffsets.Add(new(ColorIdentifier.AlmostLockedSet2, cell * 9 + digit));
					}
					foreach (var digit in targetDigitsMask)
					{
						candidateOffsets.Add(new(ColorIdentifier.Auxiliary2, cell * 9 + digit));
					}
					foreach (var digit in rccDigitsMask)
					{
						candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, cell * 9 + digit));
					}
				}

				var step = new AlmostLockedSetsXzStep(
					conclusions.AsMemory(),
					[[.. cellOffsets, .. candidateOffsets]],
					context.Options,
					als1,
					als2,
					rccMask,
					finalZ,
					isDoublyLinked
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
