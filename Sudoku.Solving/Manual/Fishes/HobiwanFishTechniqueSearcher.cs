using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Solving.Extensions;
using Sudoku.Solving.Manual.LastResorts;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Fishes
{
	/// <summary>
	/// Encapsulates a Hobiwan's fish technique searcher.
	/// </summary>
	public sealed class HobiwanFishTechniqueSearcher : FishTechniqueSearcher
	{
		/// <summary>
		/// Indicates the maximum number of exo-fins will be found.
		/// </summary>
		private readonly int _exofinCount;

		/// <summary>
		/// Indicates the maximum number of endo-fins will be found.
		/// </summary>
		private readonly int _endofinCount;

		/// <summary>
		/// Indicates the maximum size will be found. The maximum value supporting is 7.
		/// </summary>
		private readonly int _size;

		/// <summary>
		/// Indicates whether the puzzle will check templates first.
		/// If so and the digit does not have any eliminations, this digit
		/// will be skipped rather than do empty and useless loops.
		/// </summary>
		private readonly bool _checkTemplates;

		/// <summary>
		/// Indicates region maps.
		/// </summary>
		private readonly GridMap[] _regionMaps;


		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="size">The size.</param>
		/// <param name="exofinCount">The maximum number of exo-fins.</param>
		/// <param name="endofinCount">The maximum number of endo-fins.</param>
		/// <param name="checkTemplates">
		/// Indicates whether the puzzle will check templates first.
		/// </param>
		/// <param name="regionMaps">The region maps.</param>
		public HobiwanFishTechniqueSearcher(
			int size, int exofinCount, int endofinCount, bool checkTemplates,
			GridMap[] regionMaps) =>
			(_size, _exofinCount, _endofinCount, _checkTemplates, _regionMaps) = (size, exofinCount, endofinCount, checkTemplates, regionMaps);


		/// <inheritdoc/>
		public override int Priority { get; set; } = 80;


		/// <inheritdoc/>
		public override void AccumulateAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			(_, _, var digitDistributions) = grid;

			var elimMaps = new GridMap[9];
			if (_checkTemplates)
			{
				// Check templates.
				// Now sum up all possible eliminations.
				var searcher = new PatternOverlayMethodTechniqueSearcher();
				var bag = new Bag<TechniqueInfo>();
				for (int digit = 0; digit < 9; digit++)
				{
					searcher.AccumulateAll(bag, grid);

					// Store all eliminations.
					ref var map = ref elimMaps[digit];
					if (bag.Any())
					{
						foreach (var info in bag)
						{
							foreach (var conclusion in info.Conclusions)
							{
								map[conclusion.CellOffset] = true;
							}
						}
					}

					bag.Clear();
				}
			}

			// Now search fishes.
			for (int size = 2; size <= _size; size++)
			{
				for (int digit = 0; digit < 9; digit++)
				{
					AccumulateAllBySize(accumulator, grid, digit, size, elimMaps, digitDistributions);
				}
			}
		}

		/// <summary>
		/// Accumulate all results by the size of the fish.
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="grid">The grid to check.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="size">The size.</param>
		/// <param name="elimMaps">The possible elimination cells map.</param>
		/// <param name="digitDistributions">The all distributions for each digit.</param>
		private void AccumulateAllBySize(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, int digit, int size,
			GridMap[] elimMaps, GridMap[] digitDistributions)
		{
			if (_checkTemplates && elimMaps[digit].IsEmpty)
			{
				// No possible eliminations use.
				return;
			}

			for (int bs1 = 0; bs1 < 28 - size; bs1++)
			{
				if (grid.HasDigitValue(digit, bs1))
				{
					continue;
				}

				for (int bs2 = bs1 + 1; bs2 < 29 - size; bs2++)
				{
					if (grid.HasDigitValue(digit, bs2))
					{
						continue;
					}

					if (size == 2)
					{
						// Check X-Wings.
						// Record all cells which contains that digit.
						int[] baseSets = new[] { bs1, bs2 };

						// Iterate on each combination of cover sets.
						// If we have checked the template, the region will also
						// contain the elimination.
						var bodyMap = CreateMap(baseSets) & digitDistributions[digit];
						var coverSetsList = new List<IEnumerable<int>>(GetAllProperCoverSets(grid, digit, size, baseSets, elimMaps, bodyMap));
						foreach (var coverSets in coverSetsList)
						{
							// Now search for exo-fins and endo-fins.
							var tempBodyMap = bodyMap;
							var elimMap = CreateMap(coverSets) & digitDistributions[digit];
							var exofinsMap = GetExofinMap(tempBodyMap, elimMap);
							var endofinsMap = GetEndofinMap(baseSets) & digitDistributions[digit];
							var finMap = exofinsMap | endofinsMap;

							if (exofinsMap.Count > _exofinCount || endofinsMap.Count > _endofinCount)
							{
								continue;
							}

							// Reduct body map and elimination map.
							// Then body map will contain only body cells (redundant cells
							// will be cleared).
							tempBodyMap &= elimMap;
							elimMap -= tempBodyMap;

							// Check whether a candidate of that digit exists
							// on any fin cells.
							// If so, we should check elimination and fin cells' intersection;
							// otherwise, all cells in the elimination map will be the result.
							foreach (int cell in finMap.Offsets)
							{
								if (grid.CandidateExists(cell, digit))
								{
									elimMap &= new GridMap(cell);
								}
							}
							if (elimMap.IsEmpty)
							{
								continue;
							}

							// Now check eliminations.
							var conclusions = new List<Conclusion>();
							foreach (int cell in elimMap.Offsets)
							{
								if (grid.CandidateExists(cell, digit))
								{
									conclusions.Add(new Conclusion(ConclusionType.Elimination, cell, digit));
								}
							}
							if (conclusions.Count == 0)
							{
								continue;
							}

							// Record all highlight candidates.
							var candidateOffsets = new List<(int, int)>();
							RecordHighlightCandidates(grid, digit, tempBodyMap, exofinsMap, endofinsMap, candidateOffsets);

							// Record all highlight regions.
							var regionOffsets = new List<(int, int)>();
							RecordHighlightRegions(baseSets, coverSets, regionOffsets);

							accumulator.Add(
								new HobiwanFishTechniqueInfo(
									conclusions,
									views: new[]
									{
										new View(
											cellOffsets: null,
											candidateOffsets,
											regionOffsets,
											linkMasks: null)
									},
									digit,
									baseSets: baseSets.ToArray(),
									coverSets: coverSets.ToArray(),
									exofins: exofinsMap.IsEmpty ? null : exofinsMap.ToArray(),
									endofins: endofinsMap.IsEmpty ? null : endofinsMap.ToArray(),
									// TODO: Check sashimi.
									isSashimi: finMap.IsEmpty ? (bool?)null : false));
						}
					}
					else // size > 2
					{
						// TODO: Check Swordfishes.
					} // end if size == 2
				} // end for bs2
			} // end for bs1
		}

		/// <summary>
		/// Record all highlight regions.
		/// </summary>
		/// <param name="baseSets">The base sets.</param>
		/// <param name="coverSets">The cover sets.</param>
		/// <param name="regionOffsets">The list of all highlight regions.</param>
		private static void RecordHighlightRegions(
			int[] baseSets, IEnumerable<int> coverSets, IList<(int, int)> regionOffsets)
		{
			foreach (int region in baseSets)
			{
				regionOffsets.Add((0, region));
			}
			foreach (int region in coverSets)
			{
				regionOffsets.Add((1, region));
			}
		}

		/// <summary>
		/// Record all highlight candidates.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="bodyMap">The body map.</param>
		/// <param name="exofinsMap">The exo-fins map.</param>
		/// <param name="endofinsMap">The endo-fins map.</param>
		/// <param name="candidateOffsets">The list of highlight candidates.</param>
		private static void RecordHighlightCandidates(
			IReadOnlyGrid grid, int digit, GridMap bodyMap, GridMap exofinsMap,
			GridMap endofinsMap, IList<(int, int)> candidateOffsets)
		{
			bool candExists(int cell, int digit) => grid.CandidateExists(cell, digit);
			foreach (int cell in bodyMap.Offsets)
			{
				if (candExists(cell, digit))
				{
					candidateOffsets.Add((0, cell * 9 + digit));
				}
			}
			foreach (int cell in exofinsMap.Offsets)
			{
				if (candExists(cell, digit))
				{
					candidateOffsets.Add((1, cell * 9 + digit));
				}
			}
			foreach (int cell in endofinsMap.Offsets)
			{
				if (candExists(cell, digit))
				{
					candidateOffsets.Add((2, cell * 9 + digit));
				}
			}
		}

		/// <summary>
		/// Get all proper cover sets combinations.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="size">The size.</param>
		/// <param name="baseSets">The base sets.</param>
		/// <param name="elimMaps">All elimination maps.</param>
		/// <param name="bodyMap">The body map.</param>
		/// <returns>All cover sets combinations.</returns>
		private IEnumerable<IEnumerable<int>> GetAllProperCoverSets(
			IReadOnlyGrid grid, int digit, int size, int[] baseSets, GridMap[] elimMaps,
			GridMap bodyMap)
		{
			foreach (int regionsMask in new BitCombinationGenerator(27, size))
			{
				if (!RegionsAreWorth(grid, digit, regionsMask, baseSets, elimMaps, bodyMap))
				{
					continue;
				}

				yield return regionsMask.GetAllSets();
			}
		}

		/// <summary>
		/// To check whether the specified iterated regions satisfy
		/// the following conditions:
		/// <list type="number">
		/// <item>The region <b>cannot</b> contain the digit value.</item>
		/// <item>
		/// The specified regions <b>must</b> contain any one of eliminations
		/// when the elimination map is not empty.
		/// </item>
		/// <item><b>None</b> of all regions is a part of base sets.</item>
		/// <item>The region <b>must</b> contain at least one body cell.</item>
		/// </list>
		/// <b>Any one of</b> all conditions is false,
		/// the method will return <see langword="false"/>.
		/// </summary>
		/// <param name="grid">The grid to check.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="regionsMask">
		/// The region mask (got by <see cref="BitCombinationGenerator"/>).
		/// </param>
		/// <param name="baseSets">Base sets.</param>
		/// <param name="elimMaps">The elimination maps.</param>
		/// <param name="bodyMap">The body map.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		private bool RegionsAreWorth(
			IReadOnlyGrid grid, int digit, int regionsMask, int[] baseSets, GridMap[] elimMaps,
			GridMap bodyMap)
		{
			var regions = regionsMask.GetAllSets();
			if (regions.Any(region => grid.HasDigitValue(digit, region)))
			{
				return false;
			}

			if (regions.Any(region => baseSets.Contains(region)))
			{
				return false;
			}

			if (regions.Any(region => (bodyMap & _regionMaps[region]).IsEmpty))
			{
				return false;
			}

			var elimMap = elimMaps[digit];
			if (elimMap.IsNotEmpty)
			{
				// If the elimination map is not empty,
				// we will check whether the region iterated contains
				// any one of the eliminations.
				// If not, the combination is useless.
				bool checker = false;
				foreach (int region in regions)
				{
					if ((_regionMaps[region] & elimMap).IsEmpty)
					{
						checker = true;
						break;
					}
				}

				if (checker)
				{
					return false;
				}
			}

			return _checkTemplates && elimMap.IsNotEmpty || !_checkTemplates;
		}

		/// <summary>
		/// Get all cells from specified regions.
		/// </summary>
		/// <param name="regions">All regions.</param>
		/// <returns>The instance containing all cells.</returns>
		private GridMap CreateMap(IEnumerable<int> regions)
		{
			var result = GridMap.Empty;
			foreach (int region in regions)
			{
				result |= _regionMaps[region];
			}

			return result;
		}

		/// <summary>
		/// Get the endo-fins map.
		/// </summary>
		/// <param name="baseSets">All base sets.</param>
		/// <returns>Endo-fin map.</returns>
		private GridMap GetEndofinMap(int[] baseSets)
		{
			var result = GridMap.Empty;
			foreach (int value in new BitCombinationGenerator(baseSets.Length, 2))
			{
				int p1 = value.FindFirstSet();

				// Endo-fins are cells that lie on two base sets at the same time.
				result |= _regionMaps[baseSets[p1]] & _regionMaps[baseSets[value.GetNextSetBit(p1)]];
			}

			return result;
		}


		/// <summary>
		/// Get the exo-fins map.
		/// </summary>
		/// <param name="bodyMap">The map of body cells.</param>
		/// <param name="elimMap">The map of elimination maps (cover sets).</param>
		/// <returns>Exo-fin map.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static GridMap GetExofinMap(GridMap bodyMap, GridMap elimMap)
		{
			// Exo-fins are cells that lie on base sets but not in body,
			// where the body is expressed with 'base & cover'
			// (i.e. exo-fins: 'base - body').
			return bodyMap - (bodyMap & elimMap);
		}
	}
}
