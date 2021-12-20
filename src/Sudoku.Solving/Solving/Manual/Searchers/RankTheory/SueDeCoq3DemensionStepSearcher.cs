namespace Sudoku.Solving.Manual.Searchers.RankTheory;

/// <summary>
/// Provides with a <b>3-demensional Sue de Coq</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>3-demensional Sue de Coq</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe class SueDeCoq3DemensionStepSearcher : ISueDeCoq3DemensionStepSearcher
{
	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(22, DisplayingLevel.B);


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		List<Cells> rbList = new(3), cbList = new(3);
		foreach (int pivot in EmptyMap)
		{
			int r = pivot.ToRegion(RegionLabel.Row);
			int c = pivot.ToRegion(RegionLabel.Column);
			int b = pivot.ToRegion(RegionLabel.Block);
			Cells rbMap = RegionMaps[r] & RegionMaps[b], cbMap = RegionMaps[c] & RegionMaps[b];
			Cells rbEmptyMap = rbMap & EmptyMap, cbEmptyMap = cbMap & EmptyMap;
			if (
				(RowBlock: rbEmptyMap.Count, ColumnBlock: cbEmptyMap.Count) is not (
					RowBlock: >= 2,
					ColumnBlock: >= 2
				)
			)
			{
				// The intersection needs at least two cells.
				continue;
			}

			rbList.Clear();
			cbList.Clear();


			static void a(IList<Cells> list, in Cells emptyMap)
			{
				switch (emptyMap.Count)
				{
					case 2:
					{
						list.Add(new() { emptyMap[0], emptyMap[1] });

						break;
					}
					case 3:
					{
						int i = emptyMap[0], j = emptyMap[1], k = emptyMap[2];
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
					rbSelectedInterMask |= grid.GetCandidates(cell);
				}
				if (PopCount((uint)rbSelectedInterMask) <= rbCurrentMap.Count + 1)
				{
					continue;
				}

				foreach (var cbCurrentMap in cbList)
				{
					short cbSelectedInterMask = 0;
					foreach (int cell in cbCurrentMap)
					{
						cbSelectedInterMask |= grid.GetCandidates(cell);
					}
					if (PopCount((uint)cbSelectedInterMask) <= cbCurrentMap.Count + 1)
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
					for (int i = 0, count = blockMap.Count; i < count; i++)
					{
						foreach (int[] selectedBlockCells in blockMap.ToArray().GetSubsets(i))
						{
							short blockMask = 0;
							var currentBlockMap = new Cells(selectedBlockCells);
							var elimMapBlock = Cells.Empty;

							// Get the links of the block.
							foreach (int cell in selectedBlockCells)
							{
								blockMask |= grid.GetCandidates(cell);
							}

							// Get the elimination map in the block.
							foreach (int digit in blockMask)
							{
								elimMapBlock |= CandMaps[digit];
							}
							elimMapBlock &= blockMap - currentBlockMap;

							for (
								int
									j = 1,
									limit = MathExtensions.Min(
										9 - i - currentBlockMap.Count, rowMap.Count, columnMap.Count
									);
								j < limit;
								j++
							)
							{
								foreach (int[] selectedRowCells in rowMap.ToArray().GetSubsets(j))
								{
									short rowMask = 0;
									var currentRowMap = new Cells(selectedRowCells);
									var elimMapRow = Cells.Empty;

									foreach (int cell in selectedRowCells)
									{
										rowMask |= grid.GetCandidates(cell);
									}

									foreach (int digit in rowMask)
									{
										elimMapRow |= CandMaps[digit];
									}
									elimMapRow &= RegionMaps[r] - rbCurrentMap - currentRowMap;

									for (
										int k = 1;
										k <= MathExtensions.Min(
											9 - i - j - currentBlockMap.Count - currentRowMap.Count,
											rowMap.Count, columnMap.Count
										);
										k++
									)
									{
										foreach (int[] selectedColumnCells in columnMap.ToArray().GetSubsets(k))
										{
											short columnMask = 0;
											var currentColumnMap = new Cells(selectedColumnCells);
											var elimMapColumn = Cells.Empty;

											foreach (int cell in selectedColumnCells)
											{
												columnMask |= grid.GetCandidates(cell);
											}

											foreach (int digit in columnMask)
											{
												elimMapColumn |= CandMaps[digit];
											}
											elimMapColumn &= RegionMaps[c] - cbCurrentMap - currentColumnMap;

											if ((blockMask & rowMask) != 0
												&& (rowMask & columnMask) != 0
												&& (columnMask & blockMask) != 0)
											{
												continue;
											}

											var fullMap = rbCurrentMap | cbCurrentMap | currentRowMap | currentColumnMap | currentBlockMap;
											var otherMap_row = fullMap - (currentRowMap | rbCurrentMap);
											var otherMap_column = fullMap - (currentColumnMap | cbCurrentMap);
											short mask = 0;
											foreach (int cell in otherMap_row)
											{
												mask |= grid.GetCandidates(cell);
											}
											if ((mask & rowMask) != 0)
											{
												// At least one digit spanned two regions.
												continue;
											}

											mask = 0;
											foreach (int cell in otherMap_column)
											{
												mask |= grid.GetCandidates(cell);
											}
											if ((mask & columnMask) != 0)
											{
												continue;
											}

											mask = (short)((short)(blockMask | rowMask) | columnMask);
											short rbMaskOnlyInInter = (short)(rbSelectedInterMask & ~mask);
											short cbMaskOnlyInInter = (short)(cbSelectedInterMask & ~mask);

											int bCount = PopCount((uint)blockMask);
											int rCount = PopCount((uint)rowMask);
											int cCount = PopCount((uint)columnMask);
											int rbCount = PopCount((uint)rbMaskOnlyInInter);
											int cbCount = PopCount((uint)cbMaskOnlyInInter);
											if (cbCurrentMap.Count + rbCurrentMap.Count + i + j + k - 1 == bCount + rCount + cCount + rbCount + cbCount
												&& !(elimMapRow.IsEmpty && elimMapColumn.IsEmpty && elimMapBlock.IsEmpty))
											{
												// Check eliminations.
												var conclusions = new List<Conclusion>();
												foreach (int digit in blockMask)
												{
													foreach (int cell in elimMapBlock & CandMaps[digit])
													{
														conclusions.Add(new(ConclusionType.Elimination, cell, digit));
													}
												}
												foreach (int digit in rowMask)
												{
													foreach (int cell in elimMapRow & CandMaps[digit])
													{
														conclusions.Add(new(ConclusionType.Elimination, cell, digit));
													}
												}
												foreach (int digit in columnMask)
												{
													foreach (int cell in elimMapColumn & CandMaps[digit])
													{
														conclusions.Add(new(ConclusionType.Elimination, cell, digit));
													}
												}
												if (conclusions.Count == 0)
												{
													continue;
												}

#if false
												var cellOffsets = new List<(int, ColorIdentifier)>();
												foreach (int cell in currentRowMap | rbCurrentMap)
												{
													cellOffsets.Add((cell, (ColorIdentifier)0));
												}
												foreach (int cell in currentColumnMap | cbCurrentMap)
												{
													cellOffsets.Add((cell, (ColorIdentifier)1));
												}
												foreach (int cell in currentBlockMap)
												{
													cellOffsets.Add((cell, (ColorIdentifier)2));
												}
#endif

												var candidateOffsets = new List<(int, ColorIdentifier)>();
												foreach (int digit in rowMask)
												{
													foreach (int cell in (currentRowMap | rbCurrentMap) & CandMaps[digit])
													{
														candidateOffsets.Add(
															(
																cell * 9 + digit,
																(ColorIdentifier)0
															)
														);
													}
												}
												foreach (int digit in columnMask)
												{
													foreach (int cell in (currentColumnMap | cbCurrentMap) & CandMaps[digit])
													{
														candidateOffsets.Add(
															(
																cell * 9 + digit,
																(ColorIdentifier)1
															)
														);
													}
												}
												foreach (int digit in blockMask)
												{
													foreach (int cell in (currentBlockMap | rbCurrentMap | cbCurrentMap) & CandMaps[digit])
													{
														candidateOffsets.Add(
															(
																cell * 9 + digit,
																(ColorIdentifier)2
															)
														);
													}
												}

												var step = new SueDeCoq3DemensionStep(
													conclusions.ToImmutableArray(),
													ImmutableArray.Create(new PresentationData
													{
														Candidates = candidateOffsets,
														Regions = new[]
														{
															(r, (ColorIdentifier)0),
															(c, (ColorIdentifier)2),
															(b, (ColorIdentifier)3)
														}
													}),
													rowMask,
													columnMask,
													blockMask,
													currentRowMap | rbCurrentMap,
													currentColumnMap | cbCurrentMap,
													currentBlockMap | rbCurrentMap | cbCurrentMap
												);
												if (onlyFindOne)
												{
													return step;
												}

												accumulator.Add(step);
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

		return null;
	}
}
