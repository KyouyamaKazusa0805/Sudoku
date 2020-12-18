using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Manual.LastResorts;
using static Sudoku.Constants.Processings;

namespace Sudoku.Solving.Manual.Fishes
{
	/// <summary>
	/// Encapsulates a <b>Hobiwan's fish</b> technique searcher.
	/// </summary>
	public sealed class ComplexFishStepSearcher : FishStepSearcher
	{
		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(80, nameof(TechniqueCode.FrankenSwordfish))
		{
			DisplayLevel = 3
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			// Gather the POM eliminations to get all possible fish eliminations.
			var dictionary = GetPomEliminationsFirstly(grid);

			// Enumerate all possible digit.
			for (int digit = 0; digit < 9; digit++)
			{
				// Check whether the digit contains possible eliminations in POM eliminations dictionary.
				// If not, this digit doesn't contain any eliminations in fish, so we just skip the loop.
				if (!dictionary.ContainsKey(digit))
				{
					continue;
				}

				// Enumerate all possible eliminations of the current digit.
				foreach (var conclusion in dictionary[digit])
				{
					// Get all possible regions to iterate.
					// Here we should iterate on regions by the size.
					int possibleCell = conclusion.Cell;
					int[] regionsToIterateOn = CandMaps[digit].Regions.GetAllSets().ToArray();
					for (int size = 2; size <= 4; size++)
					{
						foreach (int[] baseSets in regionsToIterateOn.GetSubsets(size))
						{
							// Assume 'possibleCell' is filled with that digit,
							// and eliminate peer cells, in order to confirm the last cells
							// in base sets. They'll be cover sets list.
							var baseMap = Cells.Empty;
							foreach (int baseSet in baseSets)
							{
								baseMap |= RegionMaps[baseSet] & CandMaps[digit];
							}
							baseMap -= new Cells(possibleCell);

							// Then we should remove base sets, in order to avoid appearing duplicate
							// regions in enumerating.
							int possibleCoverRegions = baseMap.Regions;
							foreach (int baseSet in baseSets)
							{
								possibleCoverRegions &= ~(1 << baseSet);
							}

							// Now enumerate cover sets combinations.
							// Because we have removed the peers, the number of cover sets should
							// be enumerated should be 'size - 1' instead of 'size'.
							int[] possibleCoverSets = possibleCoverRegions.GetAllSets().ToArray();
							foreach (int[] coverSets in possibleCoverSets.GetSubsets(size - 1))
							{
								// Now we check the coverage.
								var coverMap = Cells.Empty;
								foreach (int coverSet in coverSets)
								{
									coverMap |= RegionMaps[coverSet] & baseMap;
								}

								// If now 'map' contains more cells than 'coverMap',
								// the checking will be failed.
								if (coverMap != baseMap)
								{
									continue;
								}

								// Try to iterate on three regions, and confirm the absolute cover sets
								// and eliminations.
								for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
								{
									int extraRegion = label.ToRegion(possibleCell);
									if (baseSets.Contains(extraRegion))
									{
										continue;
									}

									baseMap |= RegionMaps[extraRegion] & baseMap;
									coverMap |= RegionMaps[extraRegion] & CandMaps[digit];

									if (baseMap != coverMap)
									{
										continue;
									}

									// Check exo- and endo-fins.
									Cells exofins = Cells.Empty, endofins = Cells.Empty, lastMap = Cells.Empty;
									for (int i = 0, length = baseSets.Length; i < length; i++)
									{
										int region = baseSets[i];
										exofins |= RegionMaps[region] & CandMaps[digit];

										if (i != 0)
										{
											endofins |= RegionMaps[region] & lastMap;
										}

										lastMap |= exofins;
									}
									exofins -= coverMap;

									var elimMap = coverMap;
									if (!exofins.IsEmpty) elimMap &= exofins.PeerIntersection;
									if (!endofins.IsEmpty) elimMap &= endofins.PeerIntersection;
									elimMap &= CandMaps[digit];

									if (elimMap.IsEmpty)
									{
										continue;
									}

									// Gather the eliminations and cells or regions to highlight.
									var conclusions = new List<Conclusion>();
									foreach (int cell in elimMap)
									{
										conclusions.Add(new(ConclusionType.Elimination, cell, digit));
									}

									var candidateOffsets = new List<DrawingInfo>();
									foreach (int cell in baseMap)
									{
										candidateOffsets.Add(new(0, cell * 9 + digit));
									}
									foreach (int cell in exofins)
									{
										candidateOffsets.Add(new(1, cell * 9 + digit));
									}
									foreach (int cell in endofins)
									{
										candidateOffsets.Add(new(3, cell * 9 + digit));
									}

									var regionOffsets = new List<DrawingInfo>();
									foreach (int baseSet in baseSets)
									{
										regionOffsets.Add(new(0, baseSet));
									}
									foreach (int coverSet in new List<int>(coverSets) { extraRegion })
									{
										regionOffsets.Add(new(2, coverSet));
									}

									accumulator.Add(
										new HobiwanFishStepInfo(
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
											coverSets,
											exofins,
											endofins,
											exofins.IsEmpty && endofins.IsEmpty ? null : false));
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
		private static IReadOnlyDictionary<int, IList<Conclusion>> GetPomEliminationsFirstly(in SudokuGrid grid)
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
