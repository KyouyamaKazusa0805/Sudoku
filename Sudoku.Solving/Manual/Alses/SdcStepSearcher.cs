using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using static Sudoku.Constants.Processings;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Encapsulates a <b>sue de coq</b> (SdC) technique searcher.
	/// Cannibalistic SdCs can be found also.
	/// </summary>
	public sealed class SdcStepSearcher : AlsStepSearcher
	{
		/// <inheritdoc/>
		public SdcStepSearcher(bool allowOverlapping, bool alsShowRegions, bool allowAlsCycles)
			: base(allowOverlapping, alsShowRegions, allowAlsCycles)
		{
		}


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(50, nameof(TechniqueCode.Sdc))
		{
			DisplayLevel = 2
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			if (EmptyMap.Count < 4)
			{
				// SdC needs at least 4 cells like:
				// abcd abcd | ab
				// cd        |
				return;
			}

			var list = new List<Cells>(4);
			foreach (bool cannibalMode in stackalloc[] { false, true })
			{
				foreach (var ((baseSet, coverSet), (a, b, c, _)) in IntersectionMaps)
				{
					var emptyCellsInInterMap = c & EmptyMap;
					if (emptyCellsInInterMap.Count < 2)
					{
						// The intersection needs at least two empty cells.
						continue;
					}

					list.Clear();
					int[] offsets = emptyCellsInInterMap.ToArray();
					switch (emptyCellsInInterMap.Count)
					{
						case 2:
						{
							list.Add(new() { offsets[0], offsets[1] });

							break;
						}
						case 3:
						{
							int i = offsets[0], j = offsets[1], k = offsets[2];
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
							selectedInterMask |= grid.GetCandidates(cell);
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
								var currentBlockMap = new Cells(selectedCellsInBlock);
								var elimMapBlock = Cells.Empty;

								// Get the links of the block.
								foreach (int cell in selectedCellsInBlock)
								{
									blockMask |= grid.GetCandidates(cell);
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
										var currentLineMap = new Cells(selectedCellsInLine);
										var elimMapLine = Cells.Empty;

										// Get the links of the line.
										foreach (int cell in selectedCellsInLine)
										{
											lineMask |= grid.GetCandidates(cell);
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
										short maskOnlyInInter = (short)(
											selectedInterMask & ~(blockMask | lineMask));
										if (!cannibalMode && (
											blockMask.Overlaps(lineMask)
											|| maskIsolated != 0 && !maskIsolated.IsPowerOfTwo())
											|| cannibalMode && !maskIsolated.IsPowerOfTwo())
										{
											continue;
										}

										var elimMapIsolated = Cells.Empty;
										int digitIsolated = maskIsolated.FindFirstSet();
										if (digitIsolated != 32)
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
											&& (!elimMapBlock.IsEmpty || !elimMapLine.IsEmpty
												|| !elimMapIsolated.IsEmpty))
										{
											// Check eliminations.
											var conclusions = new List<Conclusion>();
											foreach (int cell in elimMapBlock)
											{
												foreach (int digit in grid.GetCandidates(cell))
												{
													if (blockMask.ContainsBit(digit))
													{
														conclusions.Add(
															new(ConclusionType.Elimination, cell, digit));
													}
												}
											}
											foreach (int cell in elimMapLine)
											{
												foreach (int digit in grid.GetCandidates(cell))
												{
													if (lineMask.ContainsBit(digit))
													{
														conclusions.Add(
															new(ConclusionType.Elimination, cell, digit));
													}
												}
											}
											foreach (int cell in elimMapIsolated)
											{
												conclusions.Add(
													new(ConclusionType.Elimination, cell, digitIsolated));
											}
											if (conclusions.Count == 0)
											{
												continue;
											}

											// Record highlight candidates and cells.
											var cellOffsets = new List<DrawingInfo>();
											foreach (int cell in currentBlockMap)
											{
												cellOffsets.Add(new(0, cell));
											}
											foreach (int cell in currentLineMap)
											{
												cellOffsets.Add(new(1, cell));
											}
											foreach (int cell in currentInterMap)
											{
												cellOffsets.Add(new(2, cell));
											}

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
													sbyte id;
													if (digitIsolated == digit)
													{
														id = 2;
													}
													else if (blockMask.ContainsBit(digit))
													{
														id = 0;
													}
													else
													{
														id = 1;
													}

													candidateOffsets.Add(new(id, cell * 9 + digit));
												}
											}

											accumulator.Add(
												new SdcStepInfo(
													conclusions,
													new View[]
													{
														new()
														{
															Cells = AlsShowRegions ? null : cellOffsets,
															Candidates = AlsShowRegions ? candidateOffsets : null,
															Regions = AlsShowRegions
															? new DrawingInfo[]
															{
																new(0, coverSet), new(1, baseSet)
															}
															: null
														}
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
