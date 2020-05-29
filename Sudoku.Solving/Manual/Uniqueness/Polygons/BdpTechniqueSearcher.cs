using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static System.Algorithms;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;
using static Sudoku.Solving.Constants.Processings;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Encapsulates a <b>Borescoper's deadly pattern</b> technique searcher.
	/// </summary>
	[TechniqueDisplay("Borescoper's Deadly Pattern")]
	public sealed partial class BdpTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <summary>
		/// All different patterns.
		/// </summary>
		/// <remarks>
		/// All possible heptagons and octagons are in here.
		/// </remarks>
		private static readonly Pattern[] Patterns = new Pattern[14580];


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 53;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			if (EmptyMap.Count < 7)
			{
				return;
			}

			for (int i = 0, end = EmptyMap.Count == 7 ? 14580 : 11664; i < end; i++)
			{
				var pattern = Patterns[i];
				if ((EmptyMap | pattern.Map) != EmptyMap)
				{
					// The pattern contains non-empty cells.
					continue;
				}

				short cornerMask1 = BitwiseOrMasks(grid, pattern.Pair1Map);
				short cornerMask2 = BitwiseOrMasks(grid, pattern.Pair2Map);
				short centerMask = BitwiseOrMasks(grid, pattern.CenterCellsMap);
				var map = pattern.Map;
				CheckType1(accumulator, grid, pattern, cornerMask1, cornerMask2, centerMask, map);
				CheckType2(accumulator, grid, pattern, cornerMask1, cornerMask2, centerMask, map);
				CheckType3(accumulator, grid, pattern, cornerMask1, cornerMask2, centerMask, map);
				CheckType4(accumulator, grid, pattern, cornerMask1, cornerMask2, centerMask, map);
			}
		}

		private void CheckType1(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, Pattern pattern, short cornerMask1,
			short cornerMask2, short centerMask, GridMap map)
		{
			short orMask = (short)((short)(cornerMask1 | cornerMask2) | centerMask);
			if (orMask.CountSet() != (pattern.IsHeptagon ? 4 : 5))
			{
				return;
			}

			// Iterate on each combination.
			foreach (int[] digits in GetCombinationsOfArray(orMask.GetAllSets().ToArray(), pattern.IsHeptagon ? 3 : 4))
			{
				short tempMask = 0;
				foreach (int digit in digits)
				{
					tempMask |= (short)(1 << digit);
				}

				int otherDigit = (orMask & ~tempMask).FindFirstSet();
				var mapContainingThatDigit = map & CandMaps[otherDigit];
				if (mapContainingThatDigit.Count != 1)
				{
					continue;
				}

				int elimCell = mapContainingThatDigit.SetAt(0);
				short elimMask = (short)(grid.GetCandidatesReversal(elimCell) & tempMask);
				if (elimMask == 0)
				{
					continue;
				}

				var conclusions = new List<Conclusion>();
				foreach (int digit in elimMask.GetAllSets())
				{
					conclusions.Add(new Conclusion(Elimination, elimCell, digit));
				}

				var candidateOffsets = new List<(int, int)>();
				foreach (int cell in map.Offsets)
				{
					if (mapContainingThatDigit[cell])
					{
						continue;
					}

					foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
					{
						candidateOffsets.Add((0, cell * 9 + digit));
					}
				}

				accumulator.Add(
					new BdpType1TechniqueInfo(
						conclusions,
						views: new[]
						{
							new View(
								cellOffsets: null,
								candidateOffsets,
								regionOffsets: null,
								links: null)
						},
						digitsMask: tempMask,
						map));
			}
		}

		private void CheckType2(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, Pattern pattern, short cornerMask1,
			short cornerMask2, short centerMask, GridMap map)
		{
			short orMask = (short)((short)(cornerMask1 | cornerMask2) | centerMask);
			if (orMask.CountSet() != (pattern.IsHeptagon ? 4 : 5))
			{
				return;
			}

			// Iterate on each combination.
			foreach (int[] digits in GetCombinationsOfArray(orMask.GetAllSets().ToArray(), pattern.IsHeptagon ? 3 : 4))
			{
				short tempMask = 0;
				foreach (int digit in digits)
				{
					tempMask |= (short)(1 << digit);
				}

				int otherDigit = (orMask & ~tempMask).FindFirstSet();
				var mapContainingThatDigit = map & CandMaps[otherDigit];
				var elimMap = (mapContainingThatDigit.PeerIntersection - map) & CandMaps[otherDigit];
				if (elimMap.IsEmpty)
				{
					continue;
				}

				var conclusions = new List<Conclusion>();
				foreach (int cell in elimMap.Offsets)
				{
					conclusions.Add(new Conclusion(Elimination, cell, otherDigit));
				}

				var candidateOffsets = new List<(int, int)>();
				foreach (int cell in map.Offsets)
				{
					foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
					{
						candidateOffsets.Add((digit == otherDigit ? 1 : 0, cell * 9 + digit));
					}
				}

				accumulator.Add(
					new BdpType2TechniqueInfo(
						conclusions,
						views: new[]
						{
							new View(
								cellOffsets: null,
								candidateOffsets,
								regionOffsets: null,
								links: null)
						},
						digitsMask: tempMask,
						map,
						extraDigit: otherDigit));
			}
		}

		private void CheckType3(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, Pattern pattern, short cornerMask1,
			short cornerMask2, short centerMask, GridMap map)
		{
			short orMask = (short)((short)(cornerMask1 | cornerMask2) | centerMask);
			foreach (int region in map.Regions)
			{
				var currentMap = RegionMaps[region] & map;
				var otherCellsMap = map - currentMap;
				short currentMask = BitwiseOrMasks(grid, currentMap);
				short otherMask = BitwiseOrMasks(grid, otherCellsMap);

				foreach (int[] digits in
					GetCombinationsOfArray(orMask.GetAllSets().ToArray(), pattern.IsHeptagon ? 3 : 4))
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
					for (int size = otherDigitsMask.CountSet() - 1; size < iterationCellsMap.Count; size++)
					{
						foreach (int[] combination in GetCombinationsOfArray(iterationCells, size))
						{
							short comparer = 0;
							foreach (int cell in combination)
							{
								comparer |= grid.GetCandidatesReversal(cell);
							}
							if ((tempMask & comparer) != 0 || tempMask.CountSet() - 1 != size
								|| (tempMask & otherDigitsMask) != otherDigitsMask)
							{
								continue;
							}

							// Type 3 found.
							// Now check eliminations.
							var conclusions = new List<Conclusion>();
							foreach (int digit in comparer.GetAllSets())
							{
								var cells = iterationCellsMap & CandMaps[digit];
								if (cells.IsEmpty)
								{
									continue;
								}

								foreach (int cell in cells.Offsets)
								{
									conclusions.Add(new Conclusion(Elimination, cell, digit));
								}
							}

							if (conclusions.Count == 0)
							{
								continue;
							}

							var candidateOffsets = new List<(int, int)>();
							foreach (int cell in currentMap.Offsets)
							{
								foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
								{
									candidateOffsets.Add(((tempMask >> digit & 1) != 0 ? 1 : 0, cell * 9 + digit));
								}
							}
							foreach (int cell in otherCellsMap.Offsets)
							{
								foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
								{
									candidateOffsets.Add((0, cell * 9 + digit));
								}
							}
							foreach (int cell in combination)
							{
								foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
								{
									candidateOffsets.Add((1, cell * 9 + digit));
								}
							}

							accumulator.Add(
								new BdpType3TechniqueInfo(
									conclusions,
									views: new[]
									{
									new View(
										cellOffsets: null,
										candidateOffsets,
										regionOffsets: new[] { (0, region) },
										links: null)
									},
									digitsMask: tempMask,
									map,
									extraCellsMap: new GridMap(combination),
									extraDigitsMask: otherDigitsMask));
						}
					}
				}
			}
		}

		[SuppressMessage("", "IDE0004:Remove redundant cast")]
		private void CheckType4(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, Pattern pattern, short cornerMask1,
			short cornerMask2, short centerMask, GridMap map)
		{
			// The type 4 may be complex and terrible to process.
			// All regions that the pattern lies on should be checked.
			short orMask = (short)((short)(cornerMask1 | cornerMask2) | centerMask);
			foreach (int region in map.Regions)
			{
				var currentMap = RegionMaps[region] & map;
				var otherCellsMap = map - currentMap;
				short currentMask = BitwiseOrMasks(grid, currentMap);
				short otherMask = BitwiseOrMasks(grid, otherCellsMap);

				// Iterate on each possible digit combination.
				// For example, if values are { 1, 2, 3 }, then all combinations taken 2 values
				// are { 1, 2 }, { 2, 3 } and { 1, 3 }.
				foreach (int[] digits in
					GetCombinationsOfArray(orMask.GetAllSets().ToArray(), pattern.IsHeptagon ? 3 : 4))
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
						GetCombinationsOfArray((tempMask & orMask).GetAllSets().ToArray(), currentMap.Count - 1))
					{
						short combinationMask = 0;
						var combinationMap = GridMap.Empty;
						bool flag = false;
						foreach (int digit in combination)
						{
							if (grid.HasDigitValue(digit, region))
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
							// Therefore the conjugate region cannot form.
							continue;
						}

						// Type 4 forms. Now check eliminations.
						int finalDigit = (tempMask & ~combinationMask).FindFirstSet();
						var elimMap = combinationMap & CandMaps[finalDigit];
						if (elimMap.IsEmpty)
						{
							continue;
						}

						var conclusions = new List<Conclusion>();
						foreach (int cell in elimMap.Offsets)
						{
							conclusions.Add(new Conclusion(Elimination, cell, finalDigit));
						}

						var candidateOffsets = new List<(int, int)>();
						foreach (int cell in currentMap.Offsets)
						{
							foreach (int digit in (grid.GetCandidatesReversal(cell) & combinationMask).GetAllSets())
							{
								candidateOffsets.Add((1, cell * 9 + digit));
							}
						}
						foreach (int cell in otherCellsMap.Offsets)
						{
							foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
							{
								candidateOffsets.Add((0, cell * 9 + digit));
							}
						}

						accumulator.Add(
							new BdpType4TechniqueInfo(
								conclusions,
								views: new[]
								{
									new View(
										cellOffsets: null,
										candidateOffsets,
										regionOffsets: new[] { (0, region) },
										links: null)
								},
								digitsMask: otherMask,
								map,
								conjugateRegion: currentMap,
								extraMask: combinationMask));
					}
				}
			}
		}
	}
}
