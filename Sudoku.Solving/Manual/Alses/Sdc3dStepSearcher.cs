using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Solving.Annotations;
using static System.Algorithms;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Encapsulates a <b>3-dimension sue de coq</b> technique.
	/// </summary>
	public sealed class Sdc3dStepSearcher : AlsStepSearcher
	{
		/// <inheritdoc/>
		public Sdc3dStepSearcher(bool allowOverlapping, bool alsShowRegions, bool allowAlsCycles)
			: base(allowOverlapping, alsShowRegions, allowAlsCycles)
		{
		}


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(55, nameof(TechniqueCode.Sdc3d))
		{
			DisplayLevel = 2
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			List<Cells> rbList = new(3), cbList = new(3);
			foreach (int pivot in EmptyMap)
			{
				int r = RegionLabel.Row.GetRegion(pivot);
				int c = RegionLabel.Column.GetRegion(pivot);
				int b = RegionLabel.Block.GetRegion(pivot);
				Cells rbMap = RegionMaps[r] & RegionMaps[b], cbMap = RegionMaps[c] & RegionMaps[b];
				Cells rbEmptyMap = rbMap & EmptyMap, cbEmptyMap = cbMap & EmptyMap;
				if ((rbEmptyMap.Count, cbEmptyMap.Count) is not ( >= 2, >= 2))
				{
					// The intersection needs at least two cells.
					continue;
				}

				rbList.Clear();
				cbList.Clear();

				static void a(IList<Cells> list, in Cells emptyMap)
				{
					int[] offsets = emptyMap.Offsets;
					switch (emptyMap.Count)
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
							list.Add(new() { i, k });
							list.Add(new() { j, k });

							break;
						}
					}
				}

				a(rbList, rbEmptyMap);
				a(cbList, cbEmptyMap);

				foreach (var rbCurrentMap in rbList)
				{
					short rbSelectedInterMask = 0;
					foreach (int cell in rbCurrentMap)
					{
						rbSelectedInterMask |= grid.GetCandidateMask(cell);
					}
					if (rbSelectedInterMask.PopCount() <= rbCurrentMap.Count + 1)
					{
						continue;
					}

					foreach (var cbCurrentMap in cbList)
					{
						short cbSelectedInterMask = 0;
						foreach (int cell in cbCurrentMap)
						{
							cbSelectedInterMask |= grid.GetCandidateMask(cell);
						}
						if (cbSelectedInterMask.PopCount() <= cbCurrentMap.Count + 1)
						{
							continue;
						}

						if ((cbCurrentMap & rbCurrentMap).Count != 1)
						{
							continue;
						}

						// Get all maps to use later.
						var blockMap = RegionMaps[b] - rbCurrentMap - cbCurrentMap & EmptyMap;
						var rowMap = RegionMaps[r] - RegionMaps[b] & EmptyMap;
						var columnMap = RegionMaps[c] - RegionMaps[b] & EmptyMap;

						// Iterate on the number of the cells that should be selected in block.
						for (int i = 0; i < blockMap.Count; i++)
						{
							foreach (int[] selectedBlockCells in blockMap.ToArray().GetSubsets(i))
							{
								short blockMask = 0;
								var currentBlockMap = new Cells(selectedBlockCells);
								var elimMapBlock = Cells.Empty;

								// Get the links of the block.
								foreach (int cell in selectedBlockCells)
								{
									blockMask |= grid.GetCandidateMask(cell);
								}

								// Get the elimination map in the block.
								foreach (int digit in blockMask)
								{
									elimMapBlock |= CandMaps[digit];
								}
								elimMapBlock &= blockMap - currentBlockMap;

								for (int j = 1; j < Min(9 - i - currentBlockMap.Count, rowMap.Count, columnMap.Count); j++)
								{
									foreach (int[] selectedRowCells in rowMap.ToArray().GetSubsets(j))
									{
										short rowMask = 0;
										var currentRowMap = new Cells(selectedRowCells);
										var elimMapRow = Cells.Empty;

										foreach (int cell in selectedRowCells)
										{
											rowMask |= grid.GetCandidateMask(cell);
										}

										foreach (int digit in rowMask)
										{
											elimMapRow |= CandMaps[digit];
										}
										elimMapRow &= RegionMaps[r] - rbCurrentMap - currentRowMap;

										for (
											int k = 1;
											k <= Min(
												9 - i - j - currentBlockMap.Count - currentRowMap.Count,
												rowMap.Count, columnMap.Count);
											k++)
										{
											foreach (int[] selectedColumnCells in columnMap.ToArray().GetSubsets(k))
											{
												short columnMask = 0;
												var currentColumnMap = new Cells(selectedColumnCells);
												var elimMapColumn = Cells.Empty;

												foreach (int cell in selectedColumnCells)
												{
													columnMask |= grid.GetCandidateMask(cell);
												}

												foreach (int digit in columnMask)
												{
													elimMapColumn |= CandMaps[digit];
												}
												elimMapColumn &= RegionMaps[c] - cbCurrentMap - currentColumnMap;

												if ((blockMask & rowMask) != 0 && (rowMask & columnMask) != 0
													&& (blockMask & columnMask) != 0)
												{
													continue;
												}

												var fullMap =
													rbCurrentMap | cbCurrentMap
													| currentRowMap | currentColumnMap | currentBlockMap;
												var otherMap_row = fullMap - (currentRowMap | rbCurrentMap);
												var otherMap_column = fullMap - (currentColumnMap | cbCurrentMap);
												short mask = 0;
												foreach (int cell in otherMap_row)
												{
													mask |= grid.GetCandidateMask(cell);
												}
												if ((mask & rowMask) != 0)
												{
													// At least one digit spanned two regions.
													continue;
												}
												mask = 0;
												foreach (int cell in otherMap_column)
												{
													mask |= grid.GetCandidateMask(cell);
												}
												if ((mask & columnMask) != 0)
												{
													continue;
												}

												mask = (short)((short)(blockMask | rowMask) | columnMask);
												short rbMaskOnlyInInter = (short)(rbSelectedInterMask & ~mask);
												short cbMaskOnlyInInter = (short)(cbSelectedInterMask & ~mask);
												if (cbCurrentMap.Count + rbCurrentMap.Count + i + j + k - 1 ==
													blockMask.PopCount() + rowMask.PopCount() + columnMask.PopCount()
													+ rbMaskOnlyInInter.PopCount() + cbMaskOnlyInInter.PopCount()
													&& (!elimMapRow.IsEmpty || !elimMapColumn.IsEmpty
														|| !elimMapBlock.IsEmpty))
												{
													// Check eliminations.
													var conclusions = new List<Conclusion>();
													foreach (int digit in blockMask)
													{
														foreach (int cell in elimMapBlock & CandMaps[digit])
														{
															conclusions.Add(new(Elimination, cell, digit));
														}
													}
													foreach (int digit in rowMask)
													{
														foreach (int cell in elimMapRow & CandMaps[digit])
														{
															conclusions.Add(new(Elimination, cell, digit));
														}
													}
													foreach (int digit in columnMask)
													{
														foreach (int cell in elimMapColumn & CandMaps[digit])
														{
															conclusions.Add(new(Elimination, cell, digit));
														}
													}
													if (conclusions.Count == 0)
													{
														continue;
													}

													var cellOffsets = new List<DrawingInfo>();
													cellOffsets.AddRange(
														from cell in currentRowMap | rbCurrentMap
														select new DrawingInfo(0, cell));
													cellOffsets.AddRange(
														from cell in currentColumnMap | cbCurrentMap
														select new DrawingInfo(1, cell));
													cellOffsets.AddRange(
														from cell in currentBlockMap
														select new DrawingInfo(2, cell));

													var candidateOffsets = new List<DrawingInfo>();
													foreach (int digit in rowMask)
													{
														foreach (int cell in
															(currentRowMap | rbCurrentMap) & CandMaps[digit])
														{
															candidateOffsets.Add(new(0, cell * 9 + digit));
														}
													}
													foreach (int digit in columnMask)
													{
														foreach (int cell in
															(currentColumnMap | cbCurrentMap) & CandMaps[digit])
														{
															candidateOffsets.Add(new(1, cell * 9 + digit));
														}
													}
													foreach (int digit in blockMask)
													{
														foreach (int cell in
															(currentBlockMap | rbCurrentMap | cbCurrentMap) &
															CandMaps[digit])
														{
															candidateOffsets.Add(new(2, cell * 9 + digit));
														}
													}

													accumulator.Add(
														new Sdc3dStepInfo(
															conclusions,
															new View[]
															{
																new()
																{
																	Cells = AlsShowRegions ? null : cellOffsets,
																	Candidates =
																		AlsShowRegions ? candidateOffsets : null,
																	Regions = AlsShowRegions
																	? new DrawingInfo[]
																	{
																		new(0, r), new(1, c), new(2, b)
																	}
																	: null
																}
															},
															rowMask,
															columnMask,
															blockMask,
															currentRowMap | rbCurrentMap,
															currentColumnMap | cbCurrentMap,
															currentBlockMap | rbCurrentMap | cbCurrentMap));
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
	}
}
