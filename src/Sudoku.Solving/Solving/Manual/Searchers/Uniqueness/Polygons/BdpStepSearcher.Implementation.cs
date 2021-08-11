using System;
using System.Collections.Generic;
using System.Numerics;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Models;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	partial class BdpStepSearcher
	{
		/// <summary>
		/// Check type 1.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="pattern">The pattern.</param>
		/// <param name="cornerMask1">The corner mask 1.</param>
		/// <param name="cornerMask2">The corner mask 2.</param>
		/// <param name="centerMask">The center mask.</param>
		/// <param name="map">The map.</param>
		partial void CheckType1(
			IList<StepInfo> accumulator, in SudokuGrid grid, in Pattern pattern, short cornerMask1,
			short cornerMask2, short centerMask, in Cells map)
		{
			short orMask = (short)((short)(cornerMask1 | cornerMask2) | centerMask);
			if (PopCount((uint)orMask) != (pattern.IsHeptagon ? 4 : 5))
			{
				return;
			}

			// Iterate on each combination.
			foreach (int[] digits in orMask.GetAllSets().GetSubsets(pattern.IsHeptagon ? 3 : 4))
			{
				short tempMask = 0;
				foreach (int digit in digits)
				{
					tempMask |= (short)(1 << digit);
				}

				int otherDigit = TrailingZeroCount(orMask & ~tempMask);
				var mapContainingThatDigit = map & CandMaps[otherDigit];
				if (mapContainingThatDigit.Count != 1)
				{
					continue;
				}

				int elimCell = mapContainingThatDigit[0];
				short elimMask = (short)(grid.GetCandidates(elimCell) & tempMask);
				if (elimMask == 0)
				{
					continue;
				}

				var conclusions = new List<Conclusion>();
				foreach (int digit in elimMask)
				{
					conclusions.Add(new(ConclusionType.Elimination, elimCell, digit));
				}

				var candidateOffsets = new List<DrawingInfo>();
				foreach (int cell in map)
				{
					if (mapContainingThatDigit.Contains(cell))
					{
						continue;
					}

					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(0, cell * 9 + digit));
					}
				}

				accumulator.Add(
					new BdpType1StepInfo(
						conclusions,
						new View[] { new() { Candidates = candidateOffsets } },
						map,
						tempMask
					)
				);
			}
		}

		/// <summary>
		/// Check type 2.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="pattern">The pattern.</param>
		/// <param name="cornerMask1">The corner mask 1.</param>
		/// <param name="cornerMask2">The corner mask 2.</param>
		/// <param name="centerMask">The center mask.</param>
		/// <param name="map">The map.</param>
		partial void CheckType2(
			IList<StepInfo> accumulator, in SudokuGrid grid, in Pattern pattern, short cornerMask1,
			short cornerMask2, short centerMask, in Cells map)
		{
			short orMask = (short)((short)(cornerMask1 | cornerMask2) | centerMask);
			if (PopCount((uint)orMask) != (pattern.IsHeptagon ? 4 : 5))
			{
				return;
			}

			// Iterate on each combination.
			foreach (int[] digits in orMask.GetAllSets().ToArray().GetSubsets(pattern.IsHeptagon ? 3 : 4))
			{
				short tempMask = 0;
				foreach (int digit in digits)
				{
					tempMask |= (short)(1 << digit);
				}

				int otherDigit = TrailingZeroCount(orMask & ~tempMask);
				var mapContainingThatDigit = map & CandMaps[otherDigit];
				var elimMap = (mapContainingThatDigit.PeerIntersection - map) & CandMaps[otherDigit];
				if (elimMap.IsEmpty)
				{
					continue;
				}

				var conclusions = new List<Conclusion>();
				foreach (int cell in elimMap)
				{
					conclusions.Add(new(ConclusionType.Elimination, cell, otherDigit));
				}

				var candidateOffsets = new List<DrawingInfo>();
				foreach (int cell in map)
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(digit == otherDigit ? 1 : 0, cell * 9 + digit));
					}
				}

				accumulator.Add(
					new BdpType2StepInfo(
						conclusions,
						new View[] { new() { Candidates = candidateOffsets } },
						map,
						tempMask,
						otherDigit
					)
				);
			}
		}

		/// <summary>
		/// Check type 3.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="pattern">The pattern.</param>
		/// <param name="cornerMask1">The corner mask 1.</param>
		/// <param name="cornerMask2">The corner mask 2.</param>
		/// <param name="centerMask">The center mask.</param>
		/// <param name="map">The map.</param>
		partial void CheckType3(
			IList<StepInfo> accumulator, in SudokuGrid grid, in Pattern pattern,
			short cornerMask1, short cornerMask2, short centerMask, in Cells map)
		{
			short orMask = (short)((short)(cornerMask1 | cornerMask2) | centerMask);
			foreach (int region in map.Regions)
			{
				Cells currentMap = RegionMaps[region] & map, otherCellsMap = map - currentMap;
				short otherMask = 0;
				foreach (int cell in otherCellsMap)
				{
					otherMask |= grid.GetCandidates(cell);
				}

				foreach (int[] digits in orMask.GetAllSets().GetSubsets(pattern.IsHeptagon ? 3 : 4))
				{
					short tempMask = 0;
					foreach (int digit in digits)
					{
						tempMask |= (short)(1 << digit);
					}
					if (otherMask != tempMask)
					{
						continue;
					}

					// Iterate on the cells by the specified size.
					var iterationCellsMap = (RegionMaps[region] - currentMap) & EmptyMap;
					int[] iterationCells = iterationCellsMap.ToArray();
					short otherDigitsMask = (short)(orMask & ~tempMask);
					for (
						int size = PopCount((uint)otherDigitsMask) - 1, count = iterationCellsMap.Count;
						size < count;
						size++
					)
					{
						foreach (int[] combination in iterationCells.GetSubsets(size))
						{
							short comparer = 0;
							foreach (int cell in combination)
							{
								comparer |= grid.GetCandidates(cell);
							}
							if ((tempMask & comparer) != 0 || PopCount((uint)tempMask) - 1 != size
								|| (tempMask & otherDigitsMask) != otherDigitsMask)
							{
								continue;
							}

							// Type 3 found.
							// Now check eliminations.
							var conclusions = new List<Conclusion>();
							foreach (int digit in comparer)
							{
								var cells = iterationCellsMap & CandMaps[digit];
								if (cells.IsEmpty)
								{
									continue;
								}

								foreach (int cell in cells)
								{
									conclusions.Add(new(ConclusionType.Elimination, cell, digit));
								}
							}

							if (conclusions.Count == 0)
							{
								continue;
							}

							var candidateOffsets = new List<DrawingInfo>();
							foreach (int cell in currentMap)
							{
								foreach (int digit in grid.GetCandidates(cell))
								{
									candidateOffsets.Add(
										new((tempMask >> digit & 1) != 0 ? 1 : 0, cell * 9 + digit)
									);
								}
							}
							foreach (int cell in otherCellsMap)
							{
								foreach (int digit in grid.GetCandidates(cell))
								{
									candidateOffsets.Add(new(0, cell * 9 + digit));
								}
							}
							foreach (int cell in combination)
							{
								foreach (int digit in grid.GetCandidates(cell))
								{
									candidateOffsets.Add(new(1, cell * 9 + digit));
								}
							}

							accumulator.Add(
								new BdpType3StepInfo(
									conclusions,
									new View[]
									{
										new()
										{
											Candidates = candidateOffsets,
											Regions = new DrawingInfo[] { new(0, region) }
										}
									},
									map,
									tempMask,
									combination,
									otherDigitsMask
								)
							);
						}
					}
				}
			}
		}

		/// <summary>
		/// Check type 4.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="pattern">The pattern.</param>
		/// <param name="cornerMask1">The corner mask 1.</param>
		/// <param name="cornerMask2">The corner mask 2.</param>
		/// <param name="centerMask">The center mask.</param>
		/// <param name="map">The map.</param>
		partial void CheckType4(
			IList<StepInfo> accumulator, in SudokuGrid grid, in Pattern pattern,
			short cornerMask1, short cornerMask2, short centerMask, in Cells map)
		{
			// The type 4 may be complex and terrible to process.
			// All regions that the pattern lies in should be checked.
			short orMask = (short)((short)(cornerMask1 | cornerMask2) | centerMask);
			foreach (int region in map.Regions)
			{
				Cells currentMap = RegionMaps[region] & map, otherCellsMap = map - currentMap;
				short otherMask = 0;
				foreach (int cell in otherCellsMap)
				{
					otherMask |= grid.GetCandidates(cell);
				}

				// Iterate on each possible digit combination.
				// For example, if values are { 1, 2, 3 }, then all combinations taken 2 values
				// are { 1, 2 }, { 2, 3 } and { 1, 3 }.
				foreach (int[] digits in orMask.GetAllSets().GetSubsets(pattern.IsHeptagon ? 3 : 4))
				{
					short tempMask = 0;
					foreach (int digit in digits)
					{
						tempMask |= (short)(1 << digit);
					}
					if (otherMask != tempMask)
					{
						continue;
					}

					// Iterate on each combination.
					// Only one digit should be eliminated, and other digits should form a "conjugate region".
					// In a so-called conjugate region, the digits can only appear in these cells in this region.
					foreach (int[] combination in
						(tempMask & orMask).GetAllSets().GetSubsets(currentMap.Count - 1))
					{
						short combinationMask = 0;
						var combinationMap = Cells.Empty;
						bool flag = false;
						foreach (int digit in combination)
						{
							if (!(ValueMaps[digit] & RegionMaps[region]).IsEmpty)
							{
								flag = true;
								break;
							}

							combinationMask |= (short)(1 << digit);
							combinationMap |= CandMaps[digit] & RegionMaps[region];
						}
						if (flag)
						{
							// The region contains digit value, which is not a normal pattern.
							continue;
						}

						if (combinationMap != currentMap)
						{
							// If not equal, the map may contains other digits in this region.
							// Therefore the conjugate region can't form.
							continue;
						}

						// Type 4 forms. Now check eliminations.
						int finalDigit = TrailingZeroCount(tempMask & ~combinationMask);
						var elimMap = combinationMap & CandMaps[finalDigit];
						if (elimMap.IsEmpty)
						{
							continue;
						}

						var conclusions = new List<Conclusion>();
						foreach (int cell in elimMap)
						{
							conclusions.Add(new(ConclusionType.Elimination, cell, finalDigit));
						}

						var candidateOffsets = new List<DrawingInfo>();
						foreach (int cell in currentMap)
						{
							foreach (int digit in grid.GetCandidates(cell) & combinationMask)
							{
								candidateOffsets.Add(new(1, cell * 9 + digit));
							}
						}
						foreach (int cell in otherCellsMap)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(new(0, cell * 9 + digit));
							}
						}

						accumulator.Add(
							new BdpType4StepInfo(
								conclusions,
								new View[]
								{
									new()
									{
										Candidates = candidateOffsets,
										Regions = new DrawingInfo[] { new(0, region) }
									}
								},
								map,
								otherMask,
								currentMap,
								combinationMask
							)
						);
					}
				}
			}
		}
	}
}
