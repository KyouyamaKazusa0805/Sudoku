using System;
using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Models;
using Sudoku.Techniques;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Encapsulates an <b>almost locked set XZ rule</b> (ALS-XZ) technique searcher.
	/// </summary>
	public sealed class AlsXzStepSearcher : AlsStepSearcher
	{
		/// <inheritdoc/>
		public override SearchingOptions Options { get; set; } = new(23, DisplayingLevel.B);

		/// <summary>
		/// Indicates the searcher properties.
		/// </summary>
		/// <remarks>
		/// Please note that all technique searches should contain
		/// this static property in order to display on settings window. If the searcher doesn't contain,
		/// when we open the settings window, it'll throw an exception to report about this.
		/// </remarks>
		[Obsolete("Please use the property '" + nameof(Options) + "' instead.", false)]
		public static TechniqueProperties Properties { get; } = new(23, nameof(Technique.SinglyLinkedAlsXz))
		{
			DisplayLevel = 2
		};


		/// <inheritdoc/>
		public override unsafe void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			int* house = stackalloc int[2];
			var alses = Als.GetAllAlses(grid);
			for (int i = 0, length = alses.Length, iterationLength = length - 1; i < iterationLength; i++)
			{
				var als1 = alses[i];
				var (_, region1, mask1, map1, possibleElimMap1, _) = als1;
				for (int j = i + 1; j < length; j++)
				{
					var als2 = alses[j];
					var (_, region2, mask2, map2, possibleElimMap2, _) = als2;
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
						var elimMap = map % CandMaps[elimDigit];
						if (elimMap.IsEmpty)
						{
							continue;
						}

						foreach (int cell in elimMap)
						{
							conclusions.Add(new(ConclusionType.Elimination, cell, elimDigit));
						}

						finalZ |= (short)(1 << elimDigit);
					}

					if (AllowAlsCycles && PopCount((uint)rccMask) == 2)
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
							isEsp ? null : isDoublyLinked
						)
					);
				}
			}
		}
	}
}
