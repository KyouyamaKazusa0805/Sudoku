namespace Sudoku.Solving.Manual.Searchers.RankTheory;

/// <summary>
/// Provides with a <b>Sue de Coq</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Sue de Coq Basic Type</item>
/// <item>Sue de Coq Isolated</item>
/// <item>Cannibalistic Sue de Coq</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe class SueDeCoqStepSearcher : ISueDeCoqStepSearcher
{
	/// <inheritdoc/>
	public SearchingOptions Options { get; set; } = new(15, DisplayingLevel.C);


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		// A vaild SdC needs at least 4 cells like:
		//
		//     abcd abcd | ab
		//     cd        |
		if (EmptyMap.Count < 4)
		{
			return null;
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
				switch (emptyCellsInInterMap)
				{
					case [var i, var j]:
					{
						list.Add(new() { i, i });

						break;
					}
					case [var i, var j, var k]:
					{
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
					if (PopCount((uint)selectedInterMask) <= currentInterMap.Count + 1)
					{
						// The intersection combination is an ALS or a normal subset,
						// which is invalid in SdCs.
						continue;
					}

					var blockMap = (b | c - currentInterMap) & EmptyMap;
					var lineMap = a & EmptyMap;

					// Iterate on the number of the cells that should be selected in block.
					for (int i = 1, count = blockMap.Count; i < count; i++)
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
											? lineMask & blockMask & selectedInterMask
											: selectedInterMask & ~(blockMask | lineMask)
									);
									short maskOnlyInInter = (short)(selectedInterMask & ~(blockMask | lineMask));
									if (
										!cannibalMode && (
											(blockMask & lineMask) != 0
											|| maskIsolated != 0 && !IsPow2(maskIsolated)
										) || cannibalMode && !IsPow2(maskIsolated)
									)
									{
										continue;
									}

									var elimMapIsolated = Cells.Empty;
									int digitIsolated = TrailingZeroCount(maskIsolated);
									if (digitIsolated != InvalidFirstSet)
									{
										elimMapIsolated = (
											cannibalMode
												? (currentBlockMap | currentLineMap)
												: currentInterMap
										) % CandMaps[digitIsolated] & EmptyMap;
									}

									if (
										currentInterMap.Count + i + j
										== PopCount((uint)blockMask) + PopCount((uint)lineMask)
											+ PopCount((uint)maskOnlyInInter)
										&& (
											!elimMapBlock.IsEmpty || !elimMapLine.IsEmpty
											|| !elimMapIsolated.IsEmpty
										)
									)
									{
										// Check eliminations.
										var conclusions = new List<Conclusion>();
										foreach (int cell in elimMapBlock)
										{
											foreach (int digit in grid.GetCandidates(cell))
											{
												if ((blockMask >> digit & 1) != 0)
												{
													conclusions.Add(
														new(ConclusionType.Elimination, cell, digit)
													);
												}
											}
										}
										foreach (int cell in elimMapLine)
										{
											foreach (int digit in grid.GetCandidates(cell))
											{
												if ((lineMask >> digit & 1) != 0)
												{
													conclusions.Add(
														new(ConclusionType.Elimination, cell, digit)
													);
												}
											}
										}
										foreach (int cell in elimMapIsolated)
										{
											conclusions.Add(
												new(ConclusionType.Elimination, cell, digitIsolated)
											);
										}
										if (conclusions.Count == 0)
										{
											continue;
										}

#if false
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
#endif

										var candidateOffsets = new List<(int, ColorIdentifier)>();
										foreach (int cell in currentBlockMap)
										{
											foreach (int digit in grid.GetCandidates(cell))
											{
												candidateOffsets.Add(
													(
														cell * 9 + digit,
														(ColorIdentifier)(
															!cannibalMode && digit == digitIsolated ? 2 : 0
														)
													)
												);
											}
										}
										foreach (int cell in currentLineMap)
										{
											foreach (int digit in grid.GetCandidates(cell))
											{
												candidateOffsets.Add(
													(
														cell * 9 + digit,
														(ColorIdentifier)(
															!cannibalMode && digit == digitIsolated ? 2 : 1
														)
													)
												);
											}
										}
										foreach (int cell in currentInterMap)
										{
											foreach (int digit in grid.GetCandidates(cell))
											{
												candidateOffsets.Add(
													(
														cell * 9 + digit,
														(ColorIdentifier)(
															digitIsolated == digit
																? 2
																: (blockMask >> digit & 1) != 0 ? 0 : 1
														)
													)
												);
											}
										}

										var step = new SueDeCoqStep(
											conclusions.ToImmutableArray(),
											ImmutableArray.Create(new PresentationData
											{
												Candidates = candidateOffsets,
												Regions = new (int, ColorIdentifier)[]
												{
													(coverSet, (ColorIdentifier)0),
													(baseSet, (ColorIdentifier)2)
												}
											}),
											coverSet,
											baseSet,
											blockMask,
											lineMask,
											selectedInterMask,
											cannibalMode,
											maskIsolated,
											currentBlockMap,
											currentLineMap,
											currentInterMap
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

		return null;
	}
}
