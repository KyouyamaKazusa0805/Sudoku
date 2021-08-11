using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Models;
using Sudoku.Solving.Manual.LastResorts;
using Sudoku.Techniques;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.Constants;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.Fishes
{
	/// <summary>
	/// Encapsulates a <b>Hobiwan's fish</b> technique searcher.
	/// </summary>
	public sealed class ComplexFishStepSearcher : FishStepSearcher
	{
		/// <summary>
		/// Indicates the max size to search.
		/// </summary>
		public int MaxSize { get; set; }

		/// <inheritdoc/>
		public override SearchingOptions Options { get; set; } = new(32, DisplayingLevel.B);


		/// <summary>
		/// Indicates the searcher properties.
		/// </summary>
		/// <remarks>
		/// Please note that all technique searches should contain
		/// this static property in order to display on settings window. If the searcher doesn't contain,
		/// when we open the settings window, it'll throw an exception to report about this.
		/// </remarks>
		[Obsolete("Please use the property '" + nameof(Options) + "' instead.", false)]
		public static TechniqueProperties Properties { get; } = new(32, nameof(Technique.FrankenSwordfish))
		{
			DisplayLevel = 2
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			// Gather the POM eliminations to get all possible fish eliminations.
			var pomElims = GetPomEliminationsFirstly(grid);

			// Other variables.
			var tempList = new List<StepInfo>();
			var tempGrid = grid;

			// Sum up how many digits exist complex fish.
			int count = 0;
			for (int digit = 0; digit < 9; digit++)
			{
				if (pomElims[digit] is not null)
				{
					count++;
				}
			}

			var tasksForSearchingForComplexFishes = new Task[count];
			count = 0;
			for (int digit = 0; digit < 9; digit++)
			{
				if (pomElims[digit] is null)
				{
					continue;
				}

				// Note the iteration variable can't be used directly.
				// We should add a copy in order to use it.
				int currentDigit = digit;

				// Create a background thread to work on searching for fishes of this digit.
				// Here we use the local function because all captured objects will be encapsulated
				// by a struct instead of a class.
				tasksForSearchingForComplexFishes[count++] = Task.Run(innerProcess);

				void innerProcess() => GetAll(tempList, tempGrid, pomElims, currentDigit);
			}

			// Synchronize the tasks.
			Task.WaitAll(tasksForSearchingForComplexFishes);

			// Remove duplicate items.
			accumulator.AddRange(tempList.RemoveDuplicateItems());
		}


		/// <summary>
		/// Get all possible fish steps.
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="pomElims">The possible eliminations to check, specified as a dictionary.</param>
		/// <param name="digit">The current digit used.</param>
		private unsafe void GetAll(
			IList<StepInfo> accumulator, in SudokuGrid grid, IList<Conclusion>?[] pomElims, int digit)
		{
			const RegionLabel bothLines = (RegionLabel)3;

			int* currentCoverSets = stackalloc int[MaxSize];

			// Iterate on each size.
			for (int size = 2; size <= MaxSize; size++)
			{
				// Iterate on different cases on whether searcher finds mutant fishes.
				// If false, search for franken fishes.
				foreach (bool searchForMutant in stackalloc[] { false, true })
				{
					// Try to check the POM eliminations.
					// If the digit as a key doesn't contain any list in that dictionary,
					// just skip this loop.
					var pomElimsOfThisDigit = pomElims[digit];
					if (pomElimsOfThisDigit is null)
					{
						continue;
					}

					// Get the eliminations of this digit.
					int[] elims = new int[pomElimsOfThisDigit.Count];
					int index = 0;
					foreach (var conclusion in pomElimsOfThisDigit)
					{
						elims[index++] = conclusion.Cell;
					}

					// Then iterate on each elimination.
					foreach (int cell in elims)
					{
						// Try to assume the digit is true in the current cell,
						// and we can get a map of all possible cells that can be filled with the digit.
						var possibleMap = CandMaps[digit] - new Cells(cell);

						// Get the table of all possible regions that contains that digit.
						var baseTable = possibleMap.Regions.GetAllSets();

						// If the 'table.Length' property is lower than '2 * size',
						// we can't find any possible complex fish now. Just skip it.
						if (baseTable.Length < size << 1)
						{
							continue;
						}

						// Iterate on each base set combinations.
						foreach (int[] baseSets in baseTable.GetSubsets(size))
						{
							// Get the mask representing the base sets used.
							int baseSetsMask = 0;
							foreach (int baseSet in baseSets)
							{
								baseSetsMask |= 1 << baseSet;
							}

							// Franken fishes doesn't contain both row and column two region types.
							if (!searchForMutant
								&& (baseSetsMask & AllRowsMask) != 0 && (baseSetsMask & AllColumnsMask) != 0)
							{
								continue;
							}

							// Get the primary map of endo-fins.
							Cells tempMap = Cells.Empty, endofins = Cells.Empty;
							for (int i = 0, length = baseSets.Length; i < length; i++)
							{
								int baseSet = baseSets[i];
								if (i != 0)
								{
									endofins |= RegionMaps[baseSet] & tempMap;
								}

								tempMap |= RegionMaps[baseSet];
							}
							endofins &= possibleMap;

							// We can't hold any endo-fins at present. The algorithm limits
							// the whole technique structure can't contain endo-fins now.
							// We just assume the possible elimination is true, then the last incomplete
							// structure can't contain any fins; otherwise, kraken fishes.
							// Do you know why we only check endo-fins instead of checking both exo-fins
							// and endo-fins? Because here the incomplete structure don't contain
							// any exo-fins at all.
							if (!endofins.IsEmpty)
							{
								continue;
							}

							// Get the mask for checking for mutant fish.
							RegionLabel baseRegionTypes;
							if (searchForMutant)
							{
								baseRegionTypes = 0;
								if ((baseSetsMask & AllRowsMask) != 0)
								{
									baseRegionTypes |= RegionLabel.Row;
								}
								if ((baseSetsMask & AllColumnsMask) != 0)
								{
									baseRegionTypes |= RegionLabel.Column;
								}
							}

							// Get all used base set list.
							int usedInBaseSets = 0;
							var baseMap = Cells.Empty;
							foreach (int baseSet in baseSets)
							{
								baseMap |= RegionMaps[baseSet];
								usedInBaseSets |= 1 << baseSet;
							}

							// Remove the cells in both peers of 'cell' and the fish body.
							var actualBaseMap = baseMap;
							baseMap &= possibleMap;

							// Now check the possible cover sets to iterate.
							int z = baseMap.Regions & ~usedInBaseSets & AllRegionsMask;
							if (z == 0)
							{
								continue;
							}

							// If the count is lower than size, we can't find any possible fish.
							int popCount = PopCount((uint)z);
							if (popCount < size)
							{
								continue;
							}

							// Now record the cover sets into the cover table.
							var coverTable = z.GetAllSets();

							// Iterate on each cover sets combination.
							foreach (int[] coverSets in coverTable.GetSubsets(size - 1))
							{
								// Now get the cover sets map.
								var coverMap = Cells.Empty;
								foreach (int coverSet in coverSets)
								{
									coverMap |= RegionMaps[coverSet];
								}

								// All cells in base sets should lie in cover sets.
								if (baseMap > coverMap)
								{
									continue;
								}

								// Now check the current cover sets.
								int usedInCoverSets = 0, i = 0;
								foreach (int coverSet in coverSets)
								{
									currentCoverSets[i++] = coverSet;
									usedInCoverSets |= 1 << coverSet;
								}
								actualBaseMap &= CandMaps[digit];

								// Now iterate on three different region types, to check the final region
								// that is the elimination lies in.
								for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
								{
									int region = cell.ToRegion(label);

									// Check whether the region is both used in base sets and cover sets.
									if ((usedInBaseSets >> region & 1) != 0
										|| (usedInCoverSets >> region & 1) != 0)
									{
										continue;
									}

									// Add the region into the cover sets, and check the cover region types.
									usedInCoverSets |= 1 << region;
									RegionLabel coverRegionTypes;
									if (searchForMutant)
									{
										coverRegionTypes = 0;
										if ((usedInCoverSets & AllRowsMask) != 0)
										{
											coverRegionTypes |= RegionLabel.Row;
										}
										if ((usedInCoverSets & AllColumnsMask) != 0)
										{
											coverRegionTypes |= RegionLabel.Column;
										}
									}

									// Now check whether the current fish is normal ones,
									// or neither base sets nor cover sets of the mutant fish
									// contain both row and column regions.
									if ((usedInBaseSets & AllRowsMask) == usedInBaseSets
										&& (usedInCoverSets & AllColumnsMask) == usedInCoverSets
										|| (usedInBaseSets & AllColumnsMask) == usedInBaseSets
										&& (usedInCoverSets & AllRowsMask) == usedInCoverSets)
									{
										// Normal fish.
										goto BacktrackValue;
									}

									if (!searchForMutant
										&& (usedInCoverSets & AllRowsMask) != 0
										&& (usedInCoverSets & AllColumnsMask) != 0)
									{
										// Mutant fish but checking Franken now.
										goto BacktrackValue;
									}

									if (searchForMutant
										&& *&baseRegionTypes != bothLines && *&coverRegionTypes != bothLines)
									{
										// Not Mutant fish.
										goto BacktrackValue;
									}

									// Now actual base sets must overlap the current region.
									if ((actualBaseMap & RegionMaps[region]).IsEmpty)
									{
										goto BacktrackValue;
									}

									// Verify passed.
									// Re-initializes endo-fins.
									endofins = Cells.Empty;

									// Insert into the current cover set list, in order to keep
									// all cover sets are in order (i.e. Sort the cover sets).
									int j = size - 2;
									for (; j >= 0; j--)
									{
										if (currentCoverSets[j] >= region)
										{
											currentCoverSets[j + 1] = currentCoverSets[j];
										}
										else
										{
											currentCoverSets[j + 1] = region;
											break;
										}
									}
									if (j < 0)
									{
										currentCoverSets[0] = region;
									}

									// Collect all endo-fins firstly.
									for (j = 0; j < size; j++)
									{
										if (j > 0)
										{
											endofins |= RegionMaps[baseSets[j]] & tempMap;
										}

										if (j == 0)
										{
											tempMap = RegionMaps[baseSets[j]];
										}
										else
										{
											tempMap |= RegionMaps[baseSets[j]];
										}
									}
									endofins &= CandMaps[digit];

									// Add the new region into the cover sets map.
									var nowCoverMap = coverMap | RegionMaps[region];

									// Collect all exo-fins, in order to get all eliminations.
									var exofins = actualBaseMap - nowCoverMap - endofins;
									var elimMap = nowCoverMap - actualBaseMap & CandMaps[digit];
									var fins = exofins | endofins;
									if (!fins.IsEmpty)
									{
										elimMap &= fins.PeerIntersection & CandMaps[digit];
									}

									// Check whether the elimination exists.
									if (elimMap.IsEmpty)
									{
										goto BacktrackValue;
									}

									// Gather eliminations, views and step information.
									var conclusions = new List<Conclusion>();
									foreach (int elimCell in elimMap)
									{
										conclusions.Add(new(ConclusionType.Elimination, elimCell, digit));
									}

									// Collect highlighting candidates.
									List<DrawingInfo> candidateOffsets = new(), regionOffsets = new();
									foreach (int body in actualBaseMap)
									{
										candidateOffsets.Add(new(0, body * 9 + digit));
									}
									foreach (int exofin in exofins)
									{
										candidateOffsets.Add(new(1, exofin * 9 + digit));
									}
									foreach (int endofin in endofins)
									{
										candidateOffsets.Add(new(3, endofin * 9 + digit));
									}

									// Don't forget the extra cover set.
									// Add it into the list now.
									int[] actualCoverSets = new int[size];
									for (int p = 0, iterationSize = size - 1; p < iterationSize; p++)
									{
										actualCoverSets[p] = coverSets[p];
									}
									actualCoverSets[^1] = region;

									// Collect highlighting regions.
									foreach (int baseSet in baseSets)
									{
										regionOffsets.Add(new(0, baseSet));
									}
									foreach (int coverSet in actualCoverSets)
									{
										regionOffsets.Add(new(2, coverSet));
									}

									// Add into the 'accumulator'.
									accumulator.Add(
										new ComplexFishStepInfo(
											conclusions,
											new View[]
											{
												new()
												{
													Candidates = candidateOffsets,
													Regions = regionOffsets
												}
											},
											digit,
											baseSets,
											actualCoverSets, // Don't use 'coverSets'.
											exofins,
											endofins,
											!searchForMutant,
											IsSashimi(baseSets, fins, digit)
										)
									);

								// Backtracking.
								BacktrackValue:
									usedInCoverSets &= ~(1 << region);
								}
							}
						}
					}
				}
			}
		}


		/// <summary>
		/// Get POM technique eliminations at first.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>The dictionary that contains all eliminations grouped by digit used.</returns>
		private static IList<Conclusion>?[] GetPomEliminationsFirstly(in SudokuGrid grid)
		{
			var tempList = new List<StepInfo>();
			new PomStepSearcher().GetAll(tempList, grid);

			var result = new IList<Conclusion>?[9];
			foreach (PomStepInfo info in tempList)
			{
				int digit = info.Digit;
				ref var currentList = ref result[digit];
				if (currentList is null)
				{
					currentList = new List<Conclusion>(info.Conclusions);
				}
				else
				{
					currentList.AddRange(info.Conclusions);
				}
			}

			return result;
		}
	}
}
