using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Manual.LastResorts;
using static Sudoku.Constants.Processings;
using EliminationList = System.Collections.Generic.IReadOnlyDictionary<
	int,
	System.Collections.Generic.IList<Sudoku.Data.Conclusion>
>;
using Steps = System.Collections.Generic.IList<Sudoku.Solving.Manual.StepInfo>;

namespace Sudoku.Solving.Manual.Fishes
{
	/// <summary>
	/// Encapsulates a <b>Hobiwan's fish</b> technique searcher.
	/// </summary>
	public sealed class ComplexFishStepSearcher : FishStepSearcher
	{
		/// <summary>
		/// Indicates the mask that means all rows.
		/// </summary>
		private const int AllRowsMask = 0b111_111_111__000_000_000;

		/// <summary>
		/// Indicates the mask that means all columns.
		/// </summary>
		private const int AllColumnsMask = 0b111_111_111__000_000_000__000_000_000;

		/// <summary>
		/// Indictes the mask that means all regions.
		/// </summary>
		private const int AllRegions = 0b111_111_111__111_111_111__111_111_111;


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(80, nameof(TechniqueCode.FrankenSwordfish))
		{
			DisplayLevel = 3
		};


		/// <inheritdoc/>
		public override void GetAll(Steps accumulator, in SudokuGrid grid)
		{
			// Gather the POM eliminations to get all possible fish eliminations.
			var pomElims = GetPomEliminationsFirstly(grid);

			var tempList = new List<StepInfo>();
			for (int size = 2; size <= 5; size++)
			{
				GetAll(tempList, grid, size, pomElims);
			}

			accumulator.AddRange(tempList);
		}


		/// <summary>
		/// Get all possible fish steps.
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="size">The size to check.</param>
		/// <param name="pomElims">The possible eliminations to check, specified as a dictionary.</param>
		[SkipLocalsInit]
		private static void GetAll(Steps accumulator, in SudokuGrid grid, int size, EliminationList pomElims)
		{
			unsafe
			{
				// Iterate on different cases on whether searcher finds mutant fishes.
				// If false, search for franken fishes.
				foreach (bool searchForMutant in stackalloc[] { false, true })
				{
					// Iterate on each digit.
					for (int digit = 0; digit < 9; digit++)
					{
						// Try to check the POM eliminations.
						// If the digit as a key doesn't contain any list in that dictionary,
						// just skip this loop.
						if (!pomElims.ContainsKey(digit))
						{
							continue;
						}

						// Get the eliminations and convert it to an array.
						int[] elims = (from conclusion in pomElims[digit] select conclusion.Cell).ToArray();

						// Then iterate on each elimination.
						foreach (int cell in elims)
						{
							// Try to assume the digit is true in the current cell,
							// and we can get a map of all possible cells that can be filled with the digit.
							var possibleMap = CandMaps[digit] - new Cells(cell);

							// Get the table of all possible regions that contains that digit.
							int[] baseTable = possibleMap.Regions.GetAllSets().ToArray();

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

								// Get the mask for checking simple fish if searching for mutant ones.
								byte baseMaskForCheckingSimpleFishIfSearchingForMutant;
								if (searchForMutant)
								{
									baseMaskForCheckingSimpleFishIfSearchingForMutant = 0;
									if ((baseSetsMask & AllRowsMask) != 0)
									{
										baseMaskForCheckingSimpleFishIfSearchingForMutant |= 1;
									}
									if ((baseSetsMask & AllColumnsMask) != 0)
									{
										baseMaskForCheckingSimpleFishIfSearchingForMutant |= 2;
									}
								}

								// Get the primary map of endo-fins.
								Cells tempMap = Cells.Empty, endofins = Cells.Empty;
								for (int i = 0; i < baseSets.Length; i++)
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
								// We just assume the possible elimination is true, then the last imcomplete
								// structure can't contain any fins; otherwise, kraken fishes.
								// Do you know why we only check endo-fins instead of checking both exo-fins
								// and endo-fins? Because here the incomplete structure don't contain
								// any exo-fins at all.
								if (!endofins.IsEmpty)
								{
									continue;
								}

								int usedInBaseSets = 0;
								var baseMap = Cells.Empty;
								foreach (int baseSet in baseSets)
								{
									baseMap |= RegionMaps[baseSet];
									usedInBaseSets |= 1 << baseSet;
								}

								var actualBaseMap = baseMap;
								baseMap &= possibleMap;

								int z = baseMap.Regions & ~usedInBaseSets & AllRegions, count = 0;
								int[] coverTable = new int[z.PopCount()];
								foreach (int region in z)
								{
									coverTable[count++] = region;
								}

								if (count < size)
								{
									continue;
								}

								foreach (int[] coverSets in coverTable.GetSubsets(size - 1))
								{
									var coverMap = Cells.Empty;
									int coverSetMask = 0;
									for (int i = 0, length = coverSets.Length - 1; i < length; i++)
									{
										int coverSet = coverSets[i];
										coverMap = RegionMaps[coverSet];
										coverSetMask |= 1 << coverSet;
									}

									if (baseMap.Overlaps(coverMap))
									{
										continue;
									}

									int[] currentCoverSets = new int[size - 1];
									int usedInCoverSets = 0;
									for (int i = 0, length = coverSets.Length - 1; i < length; i++)
									{
										currentCoverSets[i] = coverSets[i];
										usedInCoverSets |= 1 << coverSets[i];
									}
									actualBaseMap &= CandMaps[digit];

									int region;
									bool flag = false;
									for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
									{
										region = label.ToRegion(cell);
										if ((usedInBaseSets >> region & 1) != 0
											|| (usedInCoverSets >> region & 1) != 0)
										{
											continue;
										}

										usedInCoverSets |= 1 << region;
										byte coverMaskForCheckingSimpleFishIfSearchingForMutant;
										if (searchForMutant)
										{
											coverMaskForCheckingSimpleFishIfSearchingForMutant = 0;
											if ((usedInCoverSets & AllRowsMask) != 0)
											{
												coverMaskForCheckingSimpleFishIfSearchingForMutant |= 1;
											}
											if ((usedInCoverSets & AllColumnsMask) != 0)
											{
												coverMaskForCheckingSimpleFishIfSearchingForMutant |= 2;
											}
										}

										if ((usedInBaseSets & AllRowsMask) == usedInBaseSets
											&& (usedInCoverSets & AllColumnsMask) == usedInCoverSets
											|| (usedInBaseSets & AllColumnsMask) == usedInBaseSets
											&& (usedInCoverSets & AllRowsMask) == usedInCoverSets
											|| searchForMutant
											&& *&baseMaskForCheckingSimpleFishIfSearchingForMutant != 3
											&& *&coverMaskForCheckingSimpleFishIfSearchingForMutant != 3)
										{
											// Normal fish.
											usedInCoverSets &= ~(1 << region);
											continue;
										}

										if (!actualBaseMap.Overlaps(RegionMaps[region]))
										{
											continue;
										}

										flag = true;
										break;
									}
									if (!flag)
									{
										continue;
									}

									endofins = Cells.Empty;
									unsafe
									{
										coverMap |= RegionMaps[*&region];
									}

									// Insert into the current cover set list, in order to keep
									// all cover sets are in order.
									int j = size - 2;
									for (; j >= 0; j--)
									{
										if (currentCoverSets[j] >= *&region)
										{
											currentCoverSets[j + 1] = currentCoverSets[j];
										}
										else
										{
											currentCoverSets[j + 1] = *&region;
											break;
										}
									}
									if (j < 0)
									{
										currentCoverSets[0] = *&region;
									}

									for (j = 0; j < size; j++)
									{
										if (j > 0)
										{
											endofins |= RegionMaps[currentCoverSets[j]] & tempMap;
										}

										if (j == 0)
										{
											tempMap = RegionMaps[currentCoverSets[j]];
										}
										else
										{
											tempMap &= RegionMaps[currentCoverSets[j]];
										}
									}
									endofins &= CandMaps[digit];

									var exofins = actualBaseMap - coverMap - endofins;
									var elimMap = coverMap - actualBaseMap & CandMaps[digit];
									var fins = exofins | endofins;
									if (!fins.IsEmpty)
									{
										elimMap &= fins.PeerIntersection & CandMaps[digit];
									}

									// TODO: Gather eliminations, views and step information.
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
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <returns>The dictionary that contains all eliminations grouped by digit used.</returns>
		private static EliminationList GetPomEliminationsFirstly(in SudokuGrid grid)
		{
			var tempList = new List<StepInfo>();
			new PomStepSearcher().GetAll(tempList, grid);

			var result = new Dictionary<int, IList<Conclusion>>();
			foreach (PomStepInfo info in tempList)
			{
				int key = info.Digit;
				if (!result.ContainsKey(key))
				{
					result.Add(key, new List<Conclusion>(info.Conclusions));
				}
				else
				{
					result[key].AddRange(info.Conclusions);
				}
			}

			return result;
		}
	}
}
