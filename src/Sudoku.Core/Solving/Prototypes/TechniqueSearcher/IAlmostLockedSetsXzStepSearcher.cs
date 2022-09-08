namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with an <b>Almost Locked Sets XZ Rule</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Extended Subset Principle</item>
/// <item>Singly-linked Almost Locked Sets XZ Rule</item>
/// <item>Doubly-linked Almost Locked Sets XZ Rule</item>
/// </list>
/// </summary>
public interface IAlmostLockedSetsXzStepSearcher : IAlmostLockedSetsStepSearcher
{
	/// <summary>
	/// Indicates whether two ALSes make an collision, which means they share the some same cells. 
	/// </summary>
	public abstract bool AllowCollision { get; set; }

	/// <summary>
	/// Indicates whether the searcher will enhance the searching to find all possible eliminations
	/// for looped-ALS eliminations.
	/// </summary>
	public abstract bool AllowLoopedPatterns { get; set; }
}

[StepSearcher]
internal sealed unsafe partial class AlmostLockedSetsXzStepSearcher : IAlmostLockedSetsXzStepSearcher
{
	/// <inheritdoc/>
	[StepSearcherProperty]
	public bool AllowCollision { get; set; }

	/// <inheritdoc/>
	[StepSearcherProperty]
	public bool AllowLoopedPatterns { get; set; }


	/// <inheritdoc/>
	/// <remarks>
	/// <para><b>Developer notes</b></para>
	/// <para>
	/// This algorithm uses a concept called Restricted Common Candidate (abbr. RCC) to limit the implementation.
	/// If you don't know that is an RCC,
	/// <see href="https://sunnieshine.github.io/Sudoku/terms/restricted-common-candidate">this link</see>
	/// will tell you what is it.
	/// </para>
	/// </remarks>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		int* house = stackalloc int[2];
		var alses = IAlmostLockedSetsStepSearcher.Gather(grid);

		for (int i = 0, length = alses.Length, iterationLengthOuter = length - 1; i < iterationLengthOuter; i++)
		{
			var als1 = alses[i];
			int house1 = als1.House;
			short mask1 = als1.DigitsMask;
			var map1 = als1.Map;
			var possibleElimMap1 = als1.PossibleEliminationMap;
			for (int j = i + 1; j < length; j++)
			{
				var als2 = alses[j];
				int house2 = als2.House;
				short mask2 = als2.DigitsMask;
				var map2 = als2.Map;
				var possibleElimMap2 = als2.PossibleEliminationMap;
				short xzMask = (short)(mask1 & mask2);
				var map = map1 | map2;
				var overlapMap = map1 & map2;
				if (!AllowCollision && overlapMap is not [])
				{
					continue;
				}

				// Remove all digits to satisfy that the RCC digit can't appear
				// in the intersection of two ALSes.
				foreach (int cell in overlapMap)
				{
					xzMask &= (short)~grid.GetCandidates(cell);
				}

				// If the number of digits that both two ALSes contain is only one (or zero),
				// the ALS-XZ won't be formed.
				if (PopCount((uint)xzMask) < 2 || map.AllSetsAreInOneHouse(out int houseIndex))
				{
					continue;
				}

				short rccMask = 0, z = 0;
				int nh = 0;
				foreach (int digit in xzMask)
				{
					if ((map & CandidatesMap[digit]).AllSetsAreInOneHouse(out houseIndex))
					{
						// 'digit' is the RCC digit.
						rccMask |= (short)(1 << digit);
						house[nh++] = houseIndex;
					}
					else
					{
						// 'digit' is the eliminating digit.
						z |= (short)(1 << digit);
					}
				}

				if (rccMask == 0 || (rccMask & rccMask - 1) == 0 && z == 0)
				{
					continue;
				}

				// Check basic eliminations.
				bool? isDoublyLinked = false;
				short finalZ = 0;
				var conclusions = new List<Conclusion>();
				foreach (int elimDigit in z)
				{
					if (map % CandidatesMap[elimDigit] is not (var elimMap and not []))
					{
						continue;
					}

					foreach (int cell in elimMap)
					{
						conclusions.Add(new(Elimination, cell, elimDigit));
					}

					finalZ |= (short)(1 << elimDigit);
				}

				if (AllowLoopedPatterns && PopCount((uint)rccMask) == 2)
				{
					// Doubly linked ALS-XZ.
					isDoublyLinked = true;
					foreach (int elimDigit in z & ~rccMask)
					{
						if ((CandidatesMap[elimDigit] & map1) is not (var zMap and not []))
						{
							continue;
						}

						if ((+zMap & CandidatesMap[elimDigit] & map2) is not (var elimMap and not []))
						{
							continue;
						}

						finalZ |= (short)(1 << elimDigit);
					}

					// RCC digit 2 eliminations.
					int k = 0;
					foreach (int digit in rccMask)
					{
						foreach (int cell in (HousesMap[house[k]] & CandidatesMap[digit]) - map)
						{
							conclusions.Add(new(Elimination, cell, digit));
						}

						k++;
					}

					// Possible eliminations.
					var tempMap = map;
					tempMap = CandidatesMap[TrailingZeroCount(mask1)];
					foreach (int digit in mask1.SkipSetBit(1))
					{
						tempMap |= CandidatesMap[digit];
					}
					tempMap &= possibleElimMap1;
					foreach (int cell in tempMap)
					{
						foreach (int digit in grid.GetCandidates(cell) & (mask1 & ~rccMask))
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
					tempMap = CandidatesMap[TrailingZeroCount(mask2)];
					foreach (int digit in mask2.SkipSetBit(1))
					{
						tempMap |= CandidatesMap[digit];
					}
					tempMap &= possibleElimMap2;
					foreach (int cell in tempMap)
					{
						foreach (int digit in grid.GetCandidates(cell) & (mask2 & ~rccMask))
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
				bool isEsp = als1.IsBivalueCell || als2.IsBivalueCell;
				var candidateOffsets = new List<CandidateViewNode>();
				if (isEsp)
				{
					foreach (int cell in map)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new((DisplayColorKind)(finalZ >> digit & 1), cell * 9 + digit));
						}
					}
				}
				else
				{
					foreach (int cell in map1)
					{
						short mask = grid.GetCandidates(cell);
						short alsDigitsMask = (short)(mask & ~(finalZ | rccMask));
						short targetDigitsMask = (short)(mask & finalZ);
						short rccDigitsMask = (short)(mask & rccMask);
						foreach (int digit in alsDigitsMask)
						{
							candidateOffsets.Add(new(DisplayColorKind.AlmostLockedSet1, cell * 9 + digit));
						}
						foreach (int digit in targetDigitsMask)
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary2, cell * 9 + digit));
						}
						foreach (int digit in rccDigitsMask)
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
						}
					}
					foreach (int cell in map2)
					{
						short mask = grid.GetCandidates(cell);
						short alsDigitsMask = (short)(mask & ~(finalZ | rccMask));
						short targetDigitsMask = (short)(mask & finalZ);
						short rccDigitsMask = (short)(mask & rccMask);
						foreach (int digit in alsDigitsMask)
						{
							candidateOffsets.Add(new(DisplayColorKind.AlmostLockedSet2, cell * 9 + digit));
						}
						foreach (int digit in targetDigitsMask)
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary2, cell * 9 + digit));
						}
						foreach (int digit in rccDigitsMask)
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
						}
					}
				}

				var step = new AlmostLockedSetsXzStep(
					ImmutableArray.CreateRange(conclusions),
					ImmutableArray.Create(
						View.Empty
							| candidateOffsets
							| (
								isEsp ? null : new HouseViewNode[]
								{
									new(DisplayColorKind.Normal, house1),
									new(DisplayColorKind.Auxiliary1, house2)
								}
							)
					),
					als1,
					als2,
					rccMask,
					finalZ,
					isEsp ? null : isDoublyLinked
				);
				if (onlyFindOne)
				{
					return step;
				}

				accumulator.Add(step);
			}

			i++;
		}

		return null;
	}
}
