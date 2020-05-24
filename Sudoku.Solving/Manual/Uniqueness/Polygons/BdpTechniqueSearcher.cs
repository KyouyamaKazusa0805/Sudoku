using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static System.Algorithms;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;
using static Sudoku.Data.GridMap.InitializeOption;

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

				short cornerMask1 = GetMask(grid, pattern.Pair1Map);
				short cornerMask2 = GetMask(grid, pattern.Pair2Map);
				short centerMask = GetMask(grid, pattern.CenterCellsMap);
				CheckType1(accumulator, grid, pattern, cornerMask1, cornerMask2, centerMask);
				CheckType2(accumulator, grid, pattern, cornerMask1, cornerMask2, centerMask);
				CheckType4(accumulator, grid, pattern, cornerMask1, cornerMask2, centerMask);
			}
		}

		private void CheckType1(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, Pattern pattern, short cornerMask1,
			short cornerMask2, short centerMask)
		{
			short orMask = (short)((short)(cornerMask1 | cornerMask2) | centerMask);
			if (orMask.CountSet() != (pattern.IsHeptagon ? 4 : 5))
			{
				return;
			}

			// Iterate on each combination.
			var map = pattern.Map;
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
			short cornerMask2, short centerMask)
		{
			short orMask = (short)((short)(cornerMask1 | cornerMask2) | centerMask);
			if (orMask.CountSet() != (pattern.IsHeptagon ? 4 : 5))
			{
				return;
			}

			// Iterate on each combination.
			var map = pattern.Map;
			foreach (int[] digits in GetCombinationsOfArray(orMask.GetAllSets().ToArray(), pattern.IsHeptagon ? 3 : 4))
			{
				short tempMask = 0;
				foreach (int digit in digits)
				{
					tempMask |= (short)(1 << digit);
				}

				int otherDigit = (orMask & ~tempMask).FindFirstSet();
				var mapContainingThatDigit = map & CandMaps[otherDigit];
				var elimMap =
					(new GridMap(mapContainingThatDigit, ProcessPeersWithoutItself) - map) & CandMaps[otherDigit];
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

		[SuppressMessage("", "IDE0004:Remove redundant cast")]
		private void CheckType4(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, Pattern pattern, short cornerMask1,
			short cornerMask2, short centerMask)
		{
			// The type 4 may be complex and terrible to process.
			// All regions that the pattern lies on should be checked.
			var map = pattern.Map;
			short orMask = (short)((short)(cornerMask1 | cornerMask2) | centerMask);
			foreach (int region in map.Regions)
			{
				var currentMap = RegionMaps[region] & map;
				var otherCellsMap = map - currentMap;
				short currentMask = GetMask(grid, currentMap);
				short otherMask = GetMask(grid, otherCellsMap);

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
						foreach (int digit in combination)
						{
							combinationMask |= (short)(1 << digit);
							combinationMap |= CandMaps[digit] & RegionMaps[region];
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


		/// <summary>
		/// Get the mask.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="map">The map.</param>
		/// <returns>The mask.</returns>
		private static short GetMask(IReadOnlyGrid grid, GridMap map)
		{
			short mask = 0;
			foreach (int cell in map.Offsets)
			{
				mask |= grid.GetCandidatesReversal(cell);
			}

			return mask;
		}
	}
}
