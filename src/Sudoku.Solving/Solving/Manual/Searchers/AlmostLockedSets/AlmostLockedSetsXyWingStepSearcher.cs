using Sudoku.Collections;
using Sudoku.Data;
using Sudoku.Presentation;
using Sudoku.Solving.Collections;
using Sudoku.Solving.Manual.Steps.AlmostLockedSets;
using static System.Numerics.BitOperations;
using static Sudoku.Solving.Manual.Buffer.FastProperties;

namespace Sudoku.Solving.Manual.Searchers.AlmostLockedSets;

/// <summary>
/// Provides with an <b>Almost Locked Sets XY-Wing</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Almost Locked Sets XY-Wing</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe class AlmostLockedSetsXyWingStepSearcher : IAlmostLockedSetsXyWingStepSearcher
{
	/// <inheritdoc/>
	public bool AllowCollision { get; set; }

	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(27, DisplayingLevel.B);


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		var rccs = new List<(AlmostLockedSet Left, AlmostLockedSet Right, short Mask)>();
		var alses = AlmostLockedSet.Gather(grid);

		// Gather all RCCs.
		for (int i = 0, length = alses.Length, iterationLengthOuter = length - 1; i < iterationLengthOuter; i++)
		{
			ref readonly var als1 = ref alses[i];
			_ = als1 is { DigitsMask: var mask1, Map: var map1 };
			for (int j = i + 1; j < length; j++)
			{
				ref readonly var als2 = ref alses[j];
				_ = als2 is { DigitsMask: var mask2, Map: var map2 };
				var map = map1 | map2;
				if (map.InOneRegion || !(map1 & map2).IsEmpty)
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
						: als12 == als21
							? (als11, als22, als12)
							: (als11, als21, als12);

				_ = a is { Region: var aRegion, DigitsMask: var aMask, Map: var aMap };
				_ = b is { Region: var bRegion, DigitsMask: var bMask, Map: var bMap };
				_ = c is { Region: var cRegion, Map: var cMap };
				var map = aMap | bMap;
				if (map == aMap || map == bMap)
				{
					continue;
				}

				if (!AllowCollision
					&& (!(aMap & bMap).IsEmpty || !(aMap & cMap).IsEmpty || !(bMap & cMap).IsEmpty))
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

						short finalX = (short)(1 << digit1);
						short finalY = (short)(1 << digit2);
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
							if (elimMap.IsEmpty)
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
						var candidateOffsets = new List<(int, ColorIdentifier)>();
						foreach (int cell in aMap)
						{
							short mask = grid.GetCandidates(cell);
							short alsDigitsMask = (short)(mask & ~(finalX | finalZ));
							short xDigitsMask = (short)(mask & (finalX));
							short zDigitsMask = (short)(mask & finalZ);
							foreach (int digit in alsDigitsMask)
							{
								candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)101));
							}
							foreach (int digit in xDigitsMask)
							{
								candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)1));
							}
							foreach (int digit in zDigitsMask)
							{
								candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)2));
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
								candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)101));
							}
							foreach (int digit in yDigitsMask)
							{
								candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)1));
							}
							foreach (int digit in zDigitsMask)
							{
								candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)102));
							}
						}
						foreach (int cell in cMap)
						{
							short mask = grid.GetCandidates(cell);
							short alsDigitsMask = (short)(mask & ~(finalX | finalY));
							short xyDigitsMask = (short)(mask & (finalX | finalY));
							foreach (int digit in alsDigitsMask)
							{
								candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)103));
							}
							foreach (int digit in xyDigitsMask)
							{
								candidateOffsets.Add((cell * 9 + digit, (ColorIdentifier)1));
							}
						}

						var step = new AlmostLockedSetsXyWingStep(
							conclusions.ToImmutableArray(),
							ImmutableArray.Create(new PresentationData
							{
								Candidates = candidateOffsets,
								Regions = new[]
								{
									(aRegion, (ColorIdentifier)101),
									(bRegion, (ColorIdentifier)102),
									(cRegion, (ColorIdentifier)103)
								}
							}),
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
