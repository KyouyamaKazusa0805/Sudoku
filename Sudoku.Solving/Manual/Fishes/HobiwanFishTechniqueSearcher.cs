using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using static Sudoku.Constants.Processings;
using Pair = System.ValueTuple<int, int>;
using Templates = Sudoku.Solving.Manual.LastResorts.PomTechniqueSearcher;

namespace Sudoku.Solving.Manual.Fishes
{
	/// <summary>
	/// Encapsulates a <b>Hobiwan's fish</b> technique searcher.
	/// </summary>
	[TechniqueDisplay("Hobiwan's Fish")]
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
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="size">The size.</param>
		/// <param name="exofinCount">The maximum number of exo-fins.</param>
		/// <param name="endofinCount">The maximum number of endo-fins.</param>
		/// <param name="checkTemplates">
		/// Indicates whether the puzzle will check templates first.
		/// </param>
		public HobiwanFishTechniqueSearcher(int size, int exofinCount, int endofinCount, bool checkTemplates) =>
			(_size, _exofinCount, _endofinCount, _checkTemplates) = (size, exofinCount, endofinCount, checkTemplates);


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 80;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = false;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			var candMaps = grid.GetCandidatesMap();

			// Now search fishes.
			if (_size <= 4)
			{
				for (int size = 2; size <= _size; size++)
				{
					for (int digit = 0; digit < 9; digit++)
					{
						AccumulateAllBySize(accumulator, grid, digit, size, new GridMap[9], candMaps);
					}
				}
			}
			else
			{
				for (int size = 2; size <= 3; size++)
				{
					for (int digit = 0; digit < 9; digit++)
					{
						AccumulateAllBySize(accumulator, grid, digit, size, new GridMap[9], candMaps);
					}
				}

				// Checking templates will be started at size greater than 4.
				var elimMaps = new GridMap[9];
				if (_checkTemplates)
				{
					// Check templates.
					// Now sum up all possible eliminations.
					var searcher = new Templates();
					var bag = new Bag<TechniqueInfo>();
					for (int digit = 0; digit < 9; digit++)
					{
						searcher.GetAll(bag, grid);

						// Store all eliminations.
						ref var map = ref elimMaps[digit];
						if (bag.Any())
						{
							foreach (var info in bag)
							{
								foreach (var conclusion in info.Conclusions)
								{
									map.Add(conclusion.CellOffset);
								}
							}
						}

						bag.Clear();
					}
				}

				for (int size = 4; size <= _size; size++)
				{
					for (int digit = 0; digit < 9; digit++)
					{
						AccumulateAllBySize(accumulator, grid, digit, size, elimMaps, candMaps);
					}
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
			if (size > 4 && _checkTemplates && elimMaps[digit].IsEmpty)
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

					var baseSets = new List<int> { bs1, bs2 };
					if (size == 2)
					{
						// Check X-Wings.
						// Iterate on each combination of cover sets.
						var bodyMap = CreateMap(baseSets) & digitDistributions[digit];
						CheckCoverSets(accumulator, grid, digit, size, elimMaps, digitDistributions, baseSets, bodyMap);

						baseSets.RemoveLastElement();
					}
					else // size > 2
					{
						for (int bs3 = bs2 + 1; bs3 < 30 - size; bs3++)
						{
							if (grid.HasDigitValue(digit, bs3)
								|| IsRedundantRegion(digit, digitDistributions, baseSets, bs3))
							{
								continue;
							}

							if (size == 3)
							{
								// Check swordfishes.
								baseSets.Add(bs3);

								// Iterate on each combination of cover sets.
								var bodyMap = CreateMap(baseSets) & digitDistributions[digit];
								CheckCoverSets(accumulator, grid, digit, size, elimMaps, digitDistributions, baseSets, bodyMap);

								baseSets.RemoveLastElement();
							}
							else // size > 3
							{
								for (int bs4 = bs3 + 1; bs4 < 31 - size; bs4++)
								{
									if (grid.HasDigitValue(digit, bs4)
										|| IsRedundantRegion(digit, digitDistributions, baseSets, bs4))
									{
										continue;
									}

									if (size == 4)
									{
										// Check jellyfishes.
										baseSets.Add(bs4);

										// Iterate on each combination of cover sets.
										var bodyMap = CreateMap(baseSets) & digitDistributions[digit];
										CheckCoverSets(accumulator, grid, digit, size, elimMaps, digitDistributions, baseSets, bodyMap);

										baseSets.RemoveLastElement();
									}
									else // size > 4
									{
										for (int bs5 = bs4 + 1; bs5 < 32 - size; bs5++)
										{
											if (grid.HasDigitValue(digit, bs5)
												|| IsRedundantRegion(digit, digitDistributions, baseSets, bs5))
											{
												continue;
											}

											if (size == 5)
											{
												// Check starfishes.
												baseSets.Add(bs5);

												// Iterate on each combination of cover sets.
												var bodyMap = CreateMap(baseSets) & digitDistributions[digit];
												CheckCoverSets(accumulator, grid, digit, size, elimMaps, digitDistributions, baseSets, bodyMap);

												baseSets.RemoveLastElement();
											}
											else // size > 5
											{
												for (int bs6 = bs5 + 1; bs6 < 33 - size; bs6++)
												{
													if (grid.HasDigitValue(digit, bs6)
														|| IsRedundantRegion(digit, digitDistributions, baseSets, bs6))
													{
														continue;
													}

													if (size == 6)
													{
														// Check whales.
														baseSets.Add(bs6);

														// Iterate on each combination of cover sets.
														var bodyMap = CreateMap(baseSets) & digitDistributions[digit];
														CheckCoverSets(accumulator, grid, digit, size, elimMaps, digitDistributions, baseSets, bodyMap);

														baseSets.RemoveLastElement();
													}
													else // size > 6
													{
														for (int bs7 = bs6 + 1; bs7 < 34 - size; bs7++)
														{
															if (grid.HasDigitValue(digit, bs7)
																|| IsRedundantRegion(digit, digitDistributions, baseSets, bs7))
															{
																continue;
															}

															if (size == 7)
															{
																// Check leviathans.
																baseSets.Add(bs7);

																// Iterate on each combination of cover sets.
																var bodyMap = CreateMap(baseSets) & digitDistributions[digit];
																CheckCoverSets(accumulator, grid, digit, size, elimMaps, digitDistributions, baseSets, bodyMap);

																baseSets.RemoveLastElement();
															}
															// TODO: Check higher-order fishes.
														} // end for bs7
													} // end if size == 6
												} // end for bs6
											} // end if size == 5
										} // end for bs5
									} // end if size == 4
								} // end for bs4
							} // end if size == 3
						} // end for bs3
					} // end if size == 2
				} // end for bs2
			} // end for bs1
		}

		/// <summary>
		/// Check whether the specified region to add is redundant.
		/// </summary>
		/// <param name="digit">The digit.</param>
		/// <param name="digitDistributions">The digit distributions.</param>
		/// <param name="baseSets">The base sets.</param>
		/// <param name="baseSetToAdd">The base set to add.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool IsRedundantRegion(
			int digit, GridMap[] digitDistributions, IReadOnlyList<int> baseSets, int baseSetToAdd) =>
			(CreateMap(baseSets) & digitDistributions[digit]) == (CreateMap(baseSets.Append(baseSetToAdd)) & digitDistributions[digit]);

		/// <summary>
		/// Check all cover sets combinations to verify whether the fish can be formed.
		/// </summary>
		/// <param name="accumulator">The result accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="size">The size.</param>
		/// <param name="elimMaps">The elimination maps.</param>
		/// <param name="digitDistributions">The digit distributions.</param>
		/// <param name="baseSets">The base sets.</param>
		/// <param name="bodyMap">The body map.</param>
		private void CheckCoverSets(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, int digit, int size,
			GridMap[] elimMaps, GridMap[] digitDistributions,
			IReadOnlyList<int> baseSets, GridMap bodyMap)
		{
			foreach (var coverSets in GetAllProperCoverSets(grid, digit, size, baseSets, elimMaps, bodyMap))
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
					if (!(grid.Exists(cell, digit) is true))
					{
						continue;
					}

					elimMap &= new GridMap(cell);
				}
				if (elimMap.IsEmpty)
				{
					continue;
				}

				// Now check eliminations.
				var conclusions = new List<Conclusion>();
				foreach (int cell in elimMap.Offsets)
				{
					if (!(grid.Exists(cell, digit) is true))
					{
						continue;
					}

					conclusions.Add(new Conclusion(ConclusionType.Elimination, cell, digit));
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

				AddTechnique(accumulator, digit, baseSets, bodyMap, coverSets, exofinsMap, endofinsMap, finMap, conclusions, candidateOffsets, regionOffsets);
			}
		}

		/// <summary>
		/// Add the technique to bag.
		/// </summary>
		/// <param name="accumulator">The bag.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="baseSets">The base sets.</param>
		/// <param name="bodyMap">The body map.</param>
		/// <param name="coverSets">The cover sets.</param>
		/// <param name="exofinsMap">The exo-fins map.</param>
		/// <param name="endofinsMap">The endo-fins map.</param>
		/// <param name="finMap">The fins map.</param>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="candidateOffsets">The highlight candidates.</param>
		/// <param name="regionOffsets">The highlight regions.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddTechnique(
			IBag<TechniqueInfo> accumulator, int digit, IReadOnlyList<int> baseSets,
			GridMap bodyMap, IEnumerable<int> coverSets, GridMap exofinsMap,
			GridMap endofinsMap, GridMap finMap, IReadOnlyList<Conclusion> conclusions,
			IReadOnlyList<Pair> candidateOffsets, IReadOnlyList<Pair> regionOffsets)
		{
			accumulator.Add(
				new HobiwanFishTechniqueInfo(
					conclusions,
					views: new[]
					{
						new View(
							cellOffsets: null,
							candidateOffsets,
							regionOffsets,
							links: null)
					},
					digit,
					baseSets: baseSets.ToArray(),
					coverSets: coverSets.ToArray(),
					exofins: exofinsMap.IsEmpty ? null : exofinsMap.ToArray(),
					endofins: endofinsMap.IsEmpty ? null : endofinsMap.ToArray(),
					isSashimi: CheckSashimi(baseSets, bodyMap, finMap)));
		}

		/// <summary>
		/// Check whether the fish is sashimi or not.
		/// </summary>
		/// <param name="baseSets">All base sets.</param>
		/// <param name="bodyMap">The map of body cells.</param>
		/// <param name="finMap">The map of fin cells.</param>
		/// <returns>A <see cref="bool"/>? result.</returns>
		private bool? CheckSashimi(IEnumerable<int> baseSets, GridMap bodyMap, GridMap finMap)
		{
			foreach (int baseSet in baseSets)
			{
				if ((bodyMap & RegionMaps[baseSet]).Count == 1)
				{
					return true;
				}
			}

			return finMap.IsEmpty ? (bool?)null : false;
		}

		/// <summary>
		/// Record all highlight regions.
		/// </summary>
		/// <param name="baseSets">The base sets.</param>
		/// <param name="coverSets">The cover sets.</param>
		/// <param name="regionOffsets">The list of all highlight regions.</param>
		private static void RecordHighlightRegions(
			IReadOnlyList<int> baseSets, IEnumerable<int> coverSets, IList<Pair> regionOffsets)
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
			GridMap endofinsMap, IList<Pair> candidateOffsets)
		{
			foreach (int cell in bodyMap.Offsets)
			{
				if (grid.Exists(cell, digit) is true)
				{
					candidateOffsets.Add((0, cell * 9 + digit));
				}
			}
			foreach (int cell in exofinsMap.Offsets)
			{
				if (grid.Exists(cell, digit) is true)
				{
					candidateOffsets.Add((1, cell * 9 + digit));
				}
			}
			foreach (int cell in endofinsMap.Offsets)
			{
				if (grid.Exists(cell, digit) is true)
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
			IReadOnlyGrid grid, int digit, int size, IReadOnlyList<int> baseSets, GridMap[] elimMaps,
			GridMap bodyMap)
		{
			foreach (int regionsMask in new BitCombinationGenerator(27, size))
			{
				if (!RegionsAreWorth(grid, digit, regionsMask, baseSets, elimMaps, bodyMap, size))
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
		/// <param name="size">The size.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		private bool RegionsAreWorth(
			IReadOnlyGrid grid, int digit, int regionsMask, IReadOnlyList<int> baseSets,
			GridMap[] elimMaps, GridMap bodyMap, int size)
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

			if (regions.Any(region => (bodyMap & RegionMaps[region]).IsEmpty))
			{
				return false;
			}

			static bool rowJudger(int region) => region / 9 == 1;
			static bool columnJudger(int region) => region / 9 == 2;
			if (baseSets.All(rowJudger) && regions.All(columnJudger)
				|| baseSets.All(columnJudger) && regions.All(rowJudger))
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
					if ((RegionMaps[region] & elimMap).IsEmpty)
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

			bool templatesChecked = size > 4 && _checkTemplates;
			return templatesChecked && elimMap.IsNotEmpty || !templatesChecked;
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
				result |= RegionMaps[region];
			}

			return result;
		}

		/// <summary>
		/// Get the endo-fins map.
		/// </summary>
		/// <param name="baseSets">All base sets.</param>
		/// <returns>Endo-fin map.</returns>
		private GridMap GetEndofinMap(IReadOnlyList<int> baseSets)
		{
			var result = GridMap.Empty;
			foreach (int value in new BitCombinationGenerator(baseSets.Count, 2))
			{
				int p1 = value.FindFirstSet();

				// Endo-fins are cells that lie on two base sets at the same time.
				result |= RegionMaps[baseSets[p1]] & RegionMaps[baseSets[value.GetNextSet(p1)]];
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
