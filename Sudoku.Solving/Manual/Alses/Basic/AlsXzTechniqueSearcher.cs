using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;
using static Sudoku.Data.GridMap.InitializeOption;

namespace Sudoku.Solving.Manual.Alses.Basic
{
	/// <summary>
	/// Encapsulates an <b>almost locked set XZ rule</b> (ALS-XZ) technique searcher.
	/// </summary>
	[TechniqueDisplay("Almost Locked Sets XZ Rule")]
	public sealed class AlsXzTechniqueSearcher : AlsTechniqueSearcher
	{
		/// <summary>
		/// Initialize an instance with the specified information.
		/// </summary>
		/// <param name="allowOverlapping">
		/// Indicates whether the ALSes can be overlapped with each other.
		/// </param>
		/// <param name="alsShowRegions">
		/// Indicates whether all ALSes shows their regions rather than cells.
		/// </param>
		/// <param name="allowAlsCycles">Indicates whether the solver will check ALS cycles.</param>
		public AlsXzTechniqueSearcher(bool allowOverlapping, bool alsShowRegions, bool allowAlsCycles)
			: base(allowOverlapping, alsShowRegions, allowAlsCycles)
		{
		}


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 55;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			var candsMap = grid.GetCandidatesMap();

			var house = (Span<int>)stackalloc int[2];
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
					if (!_allowOverlapping && overlapMap.IsNotEmpty)
					{
						continue;
					}

					// Remove all digits to satisfy that the RCC digit cannot appear
					// in the intersection of two ALSes.
					if (overlapMap.IsNotEmpty)
					{
						foreach (int cell in overlapMap.Offsets)
						{
							xzMask &= (short)~grid.GetCandidatesReversal(cell);
						}
					}

					// If the number of digits that both two ALSes contain is only one (or zero),
					// the ALS-XZ will not be formed.
					if (xzMask.CountSet() < 2 || map.AllSetsAreInOneRegion(out int region))
					{
						continue;
					}

					short rccMask = 0, z = 0;
					int nh = 0;
					foreach (int digit in xzMask.GetAllSets())
					{
						if ((map & candsMap[digit]).AllSetsAreInOneRegion(out region))
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

					if (rccMask == 0 || (rccMask & (rccMask - 1)) == 0 && z == 0)
					{
						continue;
					}

					// Check basic eliminations.
					bool? isDoublyLinked = false;
					short finalZ = 0;
					var conclusions = new List<Conclusion>();
					foreach (int elimDigit in z.GetAllSets())
					{
						var elimMap = new GridMap(candsMap[elimDigit] & map, ProcessPeersWithoutItself)
							& candsMap[elimDigit];
						if (elimMap.IsEmpty)
						{
							continue;
						}

						foreach (int cell in elimMap.Offsets)
						{
							conclusions.Add(new Conclusion(Elimination, cell, elimDigit));
						}

						finalZ |= (short)(1 << elimDigit);
					}

					if (_allowAlsCycles && rccMask.CountSet() == 2)
					{
						// Doubly linked ALS-XZ.
						isDoublyLinked = true;
						foreach (int elimDigit in (z & ~rccMask).GetAllSets())
						{
							var zMap = candsMap[elimDigit] & map1;
							if (zMap.IsEmpty)
							{
								continue;
							}

							var elimMap = new GridMap(zMap, ProcessPeersWithoutItself) & candsMap[elimDigit] & map2;
							if (elimMap.IsEmpty)
							{
								continue;
							}

							finalZ |= (short)(1 << elimDigit);
						}

						// RCC digit 2 eliminations.
						int k = 0;
						foreach (int digit in rccMask.GetAllSets())
						{
							var elimMap = (RegionMaps[house[k]] & candsMap[digit]) - map;
							if (elimMap.IsNotEmpty)
							{
								foreach (int cell in elimMap.Offsets)
								{
									if (grid.Exists(cell, digit) is true)
									{
										conclusions.Add(new Conclusion(Elimination, cell, digit));
									}
								}
							}

							k++;
						}

						// Possible eliminations.
						var tempMap = map;
						tempMap = candsMap[mask1.FindFirstSet()];
						for (k = 1; k < mask1.CountSet(); k++)
						{
							tempMap |= candsMap[mask1.SetAt(k)];
						}
						tempMap &= possibleElimMap1;
						if (tempMap.IsNotEmpty)
						{
							foreach (int cell in tempMap.Offsets)
							{
								foreach (int digit in
									(grid.GetCandidatesReversal(cell) & (mask1 & ~rccMask)).GetAllSets())
								{
									conclusions.Add(new Conclusion(Elimination, cell, digit));
								}
							}
						}
						tempMap = candsMap[mask2.FindFirstSet()];
						for (k = 1; k < mask2.CountSet(); k++)
						{
							tempMap |= candsMap[mask2.SetAt(k)];
						}
						tempMap &= possibleElimMap2;
						if (tempMap.IsNotEmpty)
						{
							foreach (int cell in tempMap.Offsets)
							{
								foreach (int digit in
									(grid.GetCandidatesReversal(cell) & (mask2 & ~rccMask)).GetAllSets())
								{
									conclusions.Add(new Conclusion(Elimination, cell, digit));
								}
							}
						}
					}

					if (conclusions.Count == 0)
					{
						continue;
					}

					// Now record highlight elements.
					bool isEsp = als1.IsBivalueCell || als2.IsBivalueCell;
					var candidateOffsets = new List<(int, int)>();
					var cellOffsets = new List<(int, int)>();
					if (isEsp)
					{
						foreach (int cell in map.Offsets)
						{
							foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
							{
								candidateOffsets.Add((finalZ >> digit & 1, cell * 9 + digit));
							}
						}
					}
					else
					{
						foreach (int cell in map1.Offsets)
						{
							short mask = grid.GetCandidatesReversal(cell);
							short alsDigitsMask = (short)(mask & ~(finalZ | rccMask));
							short targetDigitsMask = (short)(mask & finalZ);
							short rccDigitsMask = (short)(mask & rccMask);
							foreach (int digit in alsDigitsMask.GetAllSets())
							{
								candidateOffsets.Add((-1, cell * 9 + digit));
							}
							foreach (int digit in targetDigitsMask.GetAllSets())
							{
								candidateOffsets.Add((2, cell * 9 + digit));
							}
							foreach (int digit in rccDigitsMask.GetAllSets())
							{
								candidateOffsets.Add((1, cell * 9 + digit));
							}
						}
						foreach (int cell in map2.Offsets)
						{
							short mask = grid.GetCandidatesReversal(cell);
							short alsDigitsMask = (short)(mask & ~(finalZ | rccMask));
							short targetDigitsMask = (short)(mask & finalZ);
							short rccDigitsMask = (short)(mask & rccMask);
							foreach (int digit in alsDigitsMask.GetAllSets())
							{
								candidateOffsets.Add((-2, cell * 9 + digit));
							}
							foreach (int digit in targetDigitsMask.GetAllSets())
							{
								candidateOffsets.Add((2, cell * 9 + digit));
							}
							foreach (int digit in rccDigitsMask.GetAllSets())
							{
								candidateOffsets.Add((1, cell * 9 + digit));
							}
						}
					}

					accumulator.Add(
						new AlsXzTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: _alsShowRegions ? null : cellOffsets,
									candidateOffsets: _alsShowRegions ? candidateOffsets : null,
									regionOffsets:
										_alsShowRegions
											? isEsp ? null : new[] { (0, region1), (1, region2) }
											: null,
									links: null)
							},
							als1,
							als2,
							xDigitsMask: rccMask,
							zDigitsMask: finalZ,
							isDoublyLinked: isEsp ? null : isDoublyLinked));
				}
			}
		}
	}
}
