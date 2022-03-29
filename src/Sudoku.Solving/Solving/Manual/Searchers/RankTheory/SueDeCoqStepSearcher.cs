namespace Sudoku.Solving.Manual.Searchers;

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
public sealed unsafe partial class SueDeCoqStepSearcher : ISueDeCoqStepSearcher
{
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

		bool* cannibalModeCases = stackalloc[] { false, true };

		using var list = new ValueList<Cells>(4);
		for (int caseIndex = 0; caseIndex < 2; caseIndex++)
		{
			bool cannibalMode = cannibalModeCases[caseIndex];
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
					case { Count: 2 }:
					{
						list.Add(emptyCellsInInterMap);

						break;
					}
					case [var i, var j, var k]:
					{
						list.Add(Cells.Empty + (i + j));
						list.Add(Cells.Empty + (j + k));
						list.Add(Cells.Empty + (i + k));
						list.Add(emptyCellsInInterMap);

						break;
					}
				}

				// Iterate on each intersection combination.
				foreach (var currentInterMap in list)
				{
					short selectedInterMask = grid.GetDigitsUnion(currentInterMap);
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
						foreach (var currentBlockMap in blockMap & i)
						{
							short blockMask = grid.GetDigitsUnion(currentBlockMap);
							var elimMapBlock = Cells.Empty;

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
								foreach (var currentLineMap in lineMap & j)
								{
									short lineMask = grid.GetDigitsUnion(currentLineMap);
									var elimMapLine = Cells.Empty;

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

									if (currentInterMap.Count + i + j == PopCount((uint)blockMask) + PopCount((uint)lineMask) + PopCount((uint)maskOnlyInInter)
										&& (elimMapBlock | elimMapLine | elimMapIsolated) is not [])
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

										var candidateOffsets = new List<CandidateViewNode>();
										foreach (int cell in currentBlockMap)
										{
											foreach (int digit in grid.GetCandidates(cell))
											{
												candidateOffsets.Add(
													new(
														!cannibalMode && digit == digitIsolated ? 2 : 0,
														cell * 9 + digit
													)
												);
											}
										}
										foreach (int cell in currentLineMap)
										{
											foreach (int digit in grid.GetCandidates(cell))
											{
												candidateOffsets.Add(
													new(
														!cannibalMode && digit == digitIsolated ? 2 : 1,
														cell * 9 + digit
													)
												);
											}
										}
										foreach (int cell in currentInterMap)
										{
											foreach (int digit in grid.GetCandidates(cell))
											{
												candidateOffsets.Add(
													new(
														digitIsolated == digit
															? 2
															: (blockMask >> digit & 1) != 0 ? 0 : 1,
														cell * 9 + digit
													)
												);
											}
										}

										var step = new SueDeCoqStep(
											conclusions.ToImmutableArray(),
											ImmutableArray.Create(
												View.Empty
													+ candidateOffsets
													+ new RegionViewNode[] { new(0, coverSet), new(2, baseSet) }
											),
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
