using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Solving.Collections;
using Sudoku.Solving.Manual.Steps;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.Buffer.FastProperties;

namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with an <b>Almost Locked Sets XZ</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Extended Subset Principle</item>
/// <item>Singly-linked Almost Locked Sets XZ</item>
/// <item>Doubly-linked Almost Locked Sets XZ</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe class AlmostLockedSetsXzStepSearcher : IAlmostLockedSetsXzStepSearcher
{
	/// <inheritdoc/>
	public bool AllowCollision { get; set; }

	/// <inheritdoc/>
	public bool AllowLoopedPatterns { get; set; }

	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(23, DisplayingLevel.B);


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		int* house = stackalloc int[2];
		var alses = AlmostLockedSet.Gather(grid);

		for (int i = 0, length = alses.Length, iterationLengthOuter = length - 1; i < iterationLengthOuter; i++)
		{
			ref readonly var als1 = ref alses[i];
			_ = als1 is { Region: var region1, DigitsMask: var mask1, Map: var map1, PossibleEliminationSet: var possibleElimMap1 };
			for (int j = i + 1; j < length; j++)
			{
				ref readonly var als2 = ref alses[j];
				_ = als2 is { Region: var region2, DigitsMask: var mask2, Map: var map2, PossibleEliminationSet: var possibleElimMap2 };
				short xzMask = (short)(mask1 & mask2);
				var map = map1 | map2;
				var overlapMap = map1 & map2;
				if (!AllowCollision && overlapMap.Count != 0)
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
				if (PopCount((uint)xzMask) < 2 || map.AllSetsAreInOneRegion(out int region))
				{
					continue;
				}

				short rccMask = 0, z = 0;
				int nh = 0;
				foreach (int digit in xzMask)
				{
					if ((map & CandMaps[digit]).AllSetsAreInOneRegion(out region))
					{
						// 'digit' is the RCC digit.
						rccMask |= (short)(1 << digit);
						house[nh++] = region;
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
					if ((map % CandMaps[elimDigit]) is not [_, ..] elimMap)
					{
						continue;
					}

					foreach (int cell in elimMap)
					{
						conclusions.Add(new(ConclusionType.Elimination, cell, elimDigit));
					}

					finalZ |= (short)(1 << elimDigit);
				}

				if (AllowLoopedPatterns && PopCount((uint)rccMask) == 2)
				{
					// Doubly linked ALS-XZ.
					isDoublyLinked = true;
					foreach (int elimDigit in z & ~rccMask)
					{
						if ((CandMaps[elimDigit] & map1) is not [_, ..] zMap)
						{
							continue;
						}

						if ((!zMap & CandMaps[elimDigit] & map2) is not [_, ..] elimMap)
						{
							continue;
						}

						finalZ |= (short)(1 << elimDigit);
					}

					// RCC digit 2 eliminations.
					int k = 0;
					foreach (int digit in rccMask)
					{
						foreach (int cell in (RegionMaps[house[k]] & CandMaps[digit]) - map)
						{
							conclusions.Add(new(ConclusionType.Elimination, cell, digit));
						}

						k++;
					}

					// Possible eliminations.
					var tempMap = map;
					tempMap = CandMaps[TrailingZeroCount(mask1)];
					foreach (int digit in mask1.SkipSetBit(1))
					{
						tempMap |= CandMaps[digit];
					}
					tempMap &= possibleElimMap1;
					foreach (int cell in tempMap)
					{
						foreach (int digit in grid.GetCandidates(cell) & (mask1 & ~rccMask))
						{
							conclusions.Add(new(ConclusionType.Elimination, cell, digit));
						}
					}
					tempMap = CandMaps[TrailingZeroCount(mask2)];
					foreach (int digit in mask2.SkipSetBit(1))
					{
						tempMap |= CandMaps[digit];
					}
					tempMap &= possibleElimMap2;
					foreach (int cell in tempMap)
					{
						foreach (int digit in grid.GetCandidates(cell) & (mask2 & ~rccMask))
						{
							conclusions.Add(new(ConclusionType.Elimination, cell, digit));
						}
					}
				}

				if (conclusions.Count == 0)
				{
					continue;
				}

				// Now record highlight elements.
				bool isEsp = als1.IsBivalueCell || als2.IsBivalueCell;
				var candidateOffsets = new List<(int, ColorIdentifier)>();
				if (isEsp)
				{
					foreach (int cell in map)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)(finalZ >> digit & 1)));
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
							candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)101));
						}
						foreach (int digit in targetDigitsMask)
						{
							candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)2));
						}
						foreach (int digit in rccDigitsMask)
						{
							candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)1));
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
							candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)102));
						}
						foreach (int digit in targetDigitsMask)
						{
							candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)2));
						}
						foreach (int digit in rccDigitsMask)
						{
							candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)1));
						}
					}
				}

				var step = new AlmostLockedSetsXzStep(
					conclusions.ToImmutableArray(),
					ImmutableArray.Create(new PresentationData
					{
						Candidates = candidateOffsets,
						Regions = isEsp ? null : new[]
						{
							(region1, (ColorIdentifier)0),
							(region2, (ColorIdentifier)1)
						}
					}),
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
