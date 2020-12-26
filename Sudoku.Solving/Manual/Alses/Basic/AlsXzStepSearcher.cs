using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Alses.Basic
{
	/// <summary>
	/// Encapsulates an <b>almost locked set XZ rule</b> (ALS-XZ) technique searcher.
	/// </summary>
	public sealed class AlsXzStepSearcher : AlsStepSearcher
	{
		/// <inheritdoc/>
		public AlsXzStepSearcher(bool allowOverlapping, bool alsShowRegions, bool allowAlsCycles)
			: base(allowOverlapping, alsShowRegions, allowAlsCycles)
		{
		}


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(55, nameof(TechniqueCode.SinglyLinkedAlsXz)) { DisplayLevel = 2 };


		/// <inheritdoc/>
		[SkipLocalsInit]
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			var house = (stackalloc int[2]);
			var alses = Als.GetAllAlses(grid).ToArray();
			for (int i = 0, length = alses.Length; i < length - 1; i++)
			{
				var als1 = alses[i];
				var (isBivalue1, region1, mask1, map1, possibleElimMap1, _) = als1;
				for (int j = i + 1; j < length; j++)
				{
					var als2 = alses[j];
					var (isBivalue2, region2, mask2, map2, possibleElimMap2, _) = als2;
					short xzMask = (short)(mask1 & mask2);
					var map = map1 | map2;
					var overlapMap = map1 & map2;
					if (!AllowOverlapping && !overlapMap.IsEmpty)
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
					if (xzMask.PopCount() < 2 || map.AllSetsAreInOneRegion(out int region))
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

					if (rccMask == 0 || rccMask.IsPowerOfTwo() && z == 0)
					{
						continue;
					}

					// Check basic eliminations.
					bool? isDoublyLinked = false;
					short finalZ = 0;
					var conclusions = new List<Conclusion>();
					foreach (int elimDigit in z)
					{
						var elimMap = (CandMaps[elimDigit] & map).PeerIntersection & CandMaps[elimDigit];
						if (elimMap.IsEmpty)
						{
							continue;
						}

						foreach (int cell in elimMap)
						{
							conclusions.Add(new(Elimination, cell, elimDigit));
						}

						finalZ |= (short)(1 << elimDigit);
					}

					if (AllowAlsCycles && rccMask.PopCount() == 2)
					{
						// Doubly linked ALS-XZ.
						isDoublyLinked = true;
						foreach (int elimDigit in z & ~rccMask)
						{
							var zMap = CandMaps[elimDigit] & map1;
							if (zMap.IsEmpty)
							{
								continue;
							}

							var elimMap = zMap.PeerIntersection & CandMaps[elimDigit] & map2;
							if (elimMap.IsEmpty)
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
								conclusions.Add(new(Elimination, cell, digit));
							}

							k++;
						}

						// Possible eliminations.
						var tempMap = map;
						tempMap = CandMaps[mask1.FindFirstSet()];
						for (k = 1; k < mask1.PopCount(); k++)
						{
							tempMap |= CandMaps[mask1.SetAt(k)];
						}
						tempMap &= possibleElimMap1;
						foreach (int cell in tempMap)
						{
							foreach (int digit in grid.GetCandidates(cell) & (mask1 & ~rccMask))
							{
								conclusions.Add(new(Elimination, cell, digit));
							}
						}
						tempMap = CandMaps[mask2.FindFirstSet()];
						for (k = 1; k < mask2.PopCount(); k++)
						{
							tempMap |= CandMaps[mask2.SetAt(k)];
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
					var candidateOffsets = new List<DrawingInfo>();
					var cellOffsets = new List<DrawingInfo>();
					if (isEsp)
					{
						foreach (int cell in map)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(new(finalZ >> digit & 1, cell * 9 + digit));
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
								candidateOffsets.Add(new(-1, cell * 9 + digit));
							}
							foreach (int digit in targetDigitsMask)
							{
								candidateOffsets.Add(new(2, cell * 9 + digit));
							}
							foreach (int digit in rccDigitsMask)
							{
								candidateOffsets.Add(new(1, cell * 9 + digit));
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
								candidateOffsets.Add(new(-2, cell * 9 + digit));
							}
							foreach (int digit in targetDigitsMask)
							{
								candidateOffsets.Add(new(2, cell * 9 + digit));
							}
							foreach (int digit in rccDigitsMask)
							{
								candidateOffsets.Add(new(1, cell * 9 + digit));
							}
						}
					}

					accumulator.Add(
						new AlsXzStepInfo(
							conclusions,
							new View[]
							{
								new()
								{
									Cells = AlsShowRegions ? null : cellOffsets,
									Candidates = AlsShowRegions ? candidateOffsets : null,
									Regions = AlsShowRegions
									? isEsp ? null : new DrawingInfo[] { new(0, region1), new(1, region2) }
									: null
								}
							},
							als1,
							als2,
							rccMask,
							finalZ,
							isEsp ? null : isDoublyLinked));
				}
			}
		}
	}
}
