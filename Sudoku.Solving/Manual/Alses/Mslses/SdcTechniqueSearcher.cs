using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Constants.Values;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Alses.Mslses
{
	/// <summary>
	/// Encapsulates a <b>sue de coq</b> (SdC) technique searcher.
	/// Cannibalistic SdCs can be found also.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.Sdc))]
	public sealed class SdcTechniqueSearcher : AlsTechniqueSearcher
	{
		/// <inheritdoc/>
		/// <remarks>
		/// The fields <see cref="AlsTechniqueSearcher._allowAlsCycles"/> and
		/// <see cref="AlsTechniqueSearcher._allowOverlapping"/> won't be used here.
		/// </remarks>
		public SdcTechniqueSearcher(bool allowOverlapping, bool alsShowRegions, bool allowAlsCycles)
			: base(allowOverlapping, alsShowRegions, allowAlsCycles)
		{
		}


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(50);


		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, in SudokuGrid grid)
		{
			if (EmptyMap.Count < 4)
			{
				// SdC needs at least 4 cells like:
				// abcd abcd | ab
				// cd        |
				return;
			}

			var list = new List<GridMap>(4);
			foreach (bool cannibalMode in BooleanValues)
			{
				foreach (var ((baseSet, coverSet), (a, b, c)) in IntersectionMaps)
				{
					var emptyCellsInInterMap = c & EmptyMap;
					if (emptyCellsInInterMap.Count < 2)
					{
						// The intersection needs at least two cells.
						continue;
					}

					list.Clear();
					switch (emptyCellsInInterMap.Count)
					{
						case 2:
						{
							list.Add(new() { emptyCellsInInterMap.First, emptyCellsInInterMap.SetAt(1) });

							break;
						}
						case 3:
						{
							var (i, j, k) = (
								emptyCellsInInterMap.First, emptyCellsInInterMap.SetAt(1),
								emptyCellsInInterMap.SetAt(2));
							list.Add(new() { i, j });
							list.Add(new() { j, k });
							list.Add(new() { i, k });
							list.Add(new() { i, j, k });

							break;
						}
					}

					// Iterate on each intersection combination.
					foreach (var currentInterMap in list)
					{
						short selectedInterMask = 0;
						foreach (int cell in currentInterMap)
						{
							selectedInterMask |= grid.GetCandidateMask(cell);
						}
						if (selectedInterMask.PopCount() <= currentInterMap.Count + 1)
						{
							// The intersection combination is an ALS or a normal subset,
							// which is invalid in SdCs.
							continue;
						}

						var blockMap = (b | c - currentInterMap) & EmptyMap;
						var lineMap = a & EmptyMap;

						// Iterate on the number of the cells that should be selected in block.
						for (int i = 1; i < blockMap.Count; i++)
						{
							// Iterate on each combination in block.
							foreach (int[] selectedCellsInBlock in blockMap.ToArray().GetSubsets(i))
							{
								short blockMask = 0;
								var currentBlockMap = new GridMap(selectedCellsInBlock);
								var elimMapBlock = GridMap.Empty;

								// Get the links of the block.
								foreach (int cell in selectedCellsInBlock)
								{
									blockMask |= grid.GetCandidateMask(cell);
								}

								// Get the elimination map in the block.
								foreach (int digit in blockMask)
								{
									elimMapBlock |= CandMaps[digit];
								}
								elimMapBlock &= blockMap - currentBlockMap;

								// Iterate on the number of the cells that should be selected in line.
								for (int j = 1; j <= 9 - i - currentInterMap.Count && j <= lineMap.Count; j++)
								{
									// Iterate on each combination in line.
									foreach (int[] selectedCellsInLine in lineMap.ToArray().GetSubsets(j))
									{
										short lineMask = 0;
										var currentLineMap = new GridMap(selectedCellsInLine);
										var elimMapLine = GridMap.Empty;

										// Get the links of the line.
										foreach (int cell in selectedCellsInLine)
										{
											lineMask |= grid.GetCandidateMask(cell);
										}

										// Get the elimination map in the line.
										foreach (int digit in lineMask)
										{
											elimMapLine |= CandMaps[digit];
										}
										elimMapLine &= lineMap - currentLineMap;

										short maskIsolated = (short)(
											cannibalMode
												? (lineMask & blockMask & selectedInterMask)
												: (selectedInterMask & ~(blockMask | lineMask)));
										short maskOnlyInInter = (short)(selectedInterMask & ~(blockMask | lineMask));
										if (!cannibalMode && (
											(blockMask & lineMask) != 0
											|| maskIsolated != 0 && !maskIsolated.IsPowerOfTwo())
											|| cannibalMode && !maskIsolated.IsPowerOfTwo())
										{
											continue;
										}

										var elimMapIsolated = GridMap.Empty;
										int digitIsolated = maskIsolated.FindFirstSet();
										if (digitIsolated != -1)
										{
											elimMapIsolated =
											(
												cannibalMode
													? (currentBlockMap | currentLineMap) & CandMaps[digitIsolated]
													: currentInterMap & CandMaps[digitIsolated]
											).PeerIntersection & CandMaps[digitIsolated] & EmptyMap;
										}

										if (currentInterMap.Count + i + j ==
											blockMask.PopCount() + lineMask.PopCount() + maskOnlyInInter.PopCount()
											&& (elimMapBlock.IsNotEmpty || elimMapLine.IsNotEmpty
												|| elimMapIsolated.IsNotEmpty))
										{
											// Check eliminations.
											var conclusions = new List<Conclusion>();
											foreach (int cell in elimMapBlock)
											{
												foreach (int digit in grid.GetCandidates(cell))
												{
													if ((blockMask >> digit & 1) != 0)
													{
														conclusions.Add(new(Elimination, cell, digit));
													}
												}
											}
											foreach (int cell in elimMapLine)
											{
												foreach (int digit in grid.GetCandidates(cell))
												{
													if ((lineMask >> digit & 1) != 0)
													{
														conclusions.Add(new(Elimination, cell, digit));
													}
												}
											}
											foreach (int cell in elimMapIsolated)
											{
												conclusions.Add(new(Elimination, cell, digitIsolated));
											}
											if (conclusions.Count == 0)
											{
												continue;
											}

											// Record highlight candidates and cells.
											var cellOffsets = new List<DrawingInfo>();
											cellOffsets.AddRange(
												from cell in currentBlockMap select new DrawingInfo(0, cell));
											cellOffsets.AddRange(
												from cell in currentLineMap select new DrawingInfo(1, cell));
											cellOffsets.AddRange(
												from cell in currentInterMap select new DrawingInfo(2, cell));

											var candidateOffsets = new List<DrawingInfo>();
											foreach (int cell in currentBlockMap)
											{
												foreach (int digit in grid.GetCandidates(cell))
												{
													candidateOffsets.Add(
														new(
															!cannibalMode && digit == digitIsolated ? 2 : 0,
															cell * 9 + digit));
												}
											}
											foreach (int cell in currentLineMap)
											{
												foreach (int digit in grid.GetCandidates(cell))
												{
													candidateOffsets.Add(
														new(
															!cannibalMode && digit == digitIsolated ? 2 : 1,
															cell * 9 + digit));
												}
											}
											foreach (int cell in currentInterMap)
											{
												foreach (int digit in grid.GetCandidates(cell))
												{
													candidateOffsets.Add(
														new(
															true switch
															{
																_ when digitIsolated == digit => 2,
																_ when (blockMask >> digit & 1) != 0 => 0,
																_ => 1
															}, cell * 9 + digit));
												}
											}

											accumulator.Add(
												new SdcTechniqueInfo(
													conclusions,
													new View[]
													{
														new(
															_alsShowRegions ? null : cellOffsets,
															_alsShowRegions ? candidateOffsets : null,
															_alsShowRegions switch
															{
																true => new DrawingInfo[]
																{
																	new(0, coverSet), new(1, baseSet)
																},
																_ => null
															},
															null)
													},
													coverSet,
													baseSet,
													blockMask,
													lineMask,
													selectedInterMask,
													cannibalMode,
													maskIsolated,
													currentBlockMap,
													currentLineMap,
													currentInterMap));
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
