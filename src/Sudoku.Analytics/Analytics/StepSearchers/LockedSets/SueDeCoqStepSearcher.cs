namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Sue de Coq</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Sue de Coq</item>
/// <item>Sue de Coq with Isolated Digit</item>
/// <item>Cannibalistic Sue de Coq</item>
/// </list>
/// </summary>
[StepSearcher("StepSearcherName_SueDeCoqStepSearcher", Technique.SueDeCoq, Technique.SueDeCoqIsolated, Technique.SueDeCoqCannibalism)]
public sealed partial class SueDeCoqStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(ref AnalysisContext context)
	{
		// A valid SdC needs at least 4 cells like:
		//
		//     abcd abcd | ab
		//     cd        |
		if (EmptyCells.Count < 4)
		{
			return null;
		}

		ref readonly var grid = ref context.Grid;
		var list = new List<CellMap>(4);
		foreach (var cannibalMode in (false, true))
		{
			foreach (var ((baseSet, coverSet), (a, b, c, _)) in Intersection.IntersectionMaps)
			{
				var emptyCellsInInterMap = c & EmptyCells;
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
						list.AddRef(in emptyCellsInInterMap);
						break;
					}
					case [var i, var j, var k]:
					{
						list.AddRef([i, j]);
						list.AddRef([j, k]);
						list.AddRef([i, k]);
						list.AddRef(in emptyCellsInInterMap);
						break;
					}
				}

				// Iterate on each intersection combination.
				foreach (ref readonly var currentInterMap in list.AsReadOnlySpan())
				{
					var selectedInterMask = grid[in currentInterMap];
					if (PopCount((uint)selectedInterMask) <= currentInterMap.Count + 1)
					{
						// The intersection combination is an ALS or a normal subset, which is invalid in SdCs.
						continue;
					}

					var blockMap = (b | c - currentInterMap) & EmptyCells;
					var lineMap = a & EmptyCells;

					// Iterate on the number of the cells that should be selected in block.
					for (var i = 1; i < blockMap.Count; i++)
					{
						// Iterate on each combination in block.
						foreach (ref readonly var currentBlockMap in blockMap.GetSubsets(i))
						{
							var blockMask = grid[in currentBlockMap];
							var elimMapBlock = CellMap.Empty;

							// Get the elimination map in the block.
							foreach (var digit in blockMask)
							{
								elimMapBlock |= CandidatesMap[digit];
							}
							elimMapBlock &= blockMap - currentBlockMap;

							// Iterate on the number of the cells that should be selected in line.
							for (var j = 1; j <= 9 - i - currentInterMap.Count && j <= lineMap.Count; j++)
							{
								// Iterate on each combination in line.
								foreach (ref readonly var currentLineMap in lineMap.GetSubsets(j))
								{
									var lineMask = grid[in currentLineMap];
									var elimMapLine = CellMap.Empty;

									// Get the elimination map in the line.
									foreach (var digit in lineMask)
									{
										elimMapLine |= CandidatesMap[digit];
									}
									elimMapLine &= lineMap - currentLineMap;

									var maskIsolated = (Mask)(
										cannibalMode
											? lineMask & blockMask & selectedInterMask
											: selectedInterMask & (Mask)~(blockMask | lineMask)
									);
									var maskOnlyInInter = (Mask)(selectedInterMask & (Mask)~(blockMask | lineMask));
									if (!cannibalMode
										&& ((blockMask & lineMask) != 0 || maskIsolated != 0 && !IsPow2(maskIsolated))
										|| cannibalMode && !IsPow2(maskIsolated))
									{
										continue;
									}

									var elimMapIsolated = CellMap.Empty;
									var digitIsolated = TrailingZeroCount(maskIsolated);
									if (digitIsolated != TrailingZeroCountFallback)
									{
										elimMapIsolated = (cannibalMode ? (currentBlockMap | currentLineMap) : currentInterMap)
											% CandidatesMap[digitIsolated]
											& EmptyCells;
									}

									var p = PopCount((uint)blockMask) + PopCount((uint)lineMask) + PopCount((uint)maskOnlyInInter);
									if (currentInterMap.Count + i + j != p || !(elimMapBlock | elimMapLine | elimMapIsolated))
									{
										// Invalid or no elimination.
										continue;
									}

									// Check eliminations.
									var conclusions = new List<Conclusion>();
									foreach (var cell in elimMapBlock)
									{
										foreach (var digit in grid.GetCandidates(cell))
										{
											if ((blockMask >> digit & 1) != 0)
											{
												conclusions.Add(new(Elimination, cell, digit));
											}
										}
									}
									foreach (var cell in elimMapLine)
									{
										foreach (var digit in grid.GetCandidates(cell))
										{
											if ((lineMask >> digit & 1) != 0)
											{
												conclusions.Add(new(Elimination, cell, digit));
											}
										}
									}
									foreach (var cell in elimMapIsolated)
									{
										conclusions.Add(new(Elimination, cell, digitIsolated));
									}
									if (conclusions.Count == 0)
									{
										continue;
									}

									var candidateOffsets = new List<CandidateViewNode>();
									foreach (var cell in currentBlockMap)
									{
										foreach (var digit in grid.GetCandidates(cell))
										{
											candidateOffsets.Add(
												new(
													!cannibalMode && digit == digitIsolated ? ColorIdentifier.Auxiliary2 : ColorIdentifier.Normal,
													cell * 9 + digit
												)
											);
										}
									}
									foreach (var cell in currentLineMap)
									{
										foreach (var digit in grid.GetCandidates(cell))
										{
											candidateOffsets.Add(
												new(
													!cannibalMode && digit == digitIsolated ? ColorIdentifier.Auxiliary2 : ColorIdentifier.Auxiliary1,
													cell * 9 + digit
												)
											);
										}
									}
									foreach (var cell in currentInterMap)
									{
										foreach (var digit in grid.GetCandidates(cell))
										{
											candidateOffsets.Add(
												new(
													digitIsolated == digit
														? ColorIdentifier.Auxiliary2
														: (blockMask >> digit & 1) != 0 ? ColorIdentifier.Normal : ColorIdentifier.Auxiliary1,
													cell * 9 + digit
												)
											);
										}
									}

									var step = new SueDeCoqStep(
										[.. conclusions],
										[
											[
												.. candidateOffsets,
												new HouseViewNode(ColorIdentifier.Normal, coverSet),
												new HouseViewNode(ColorIdentifier.Auxiliary2, baseSet)
											]
										],
										context.Options,
										coverSet,
										baseSet,
										blockMask,
										lineMask,
										selectedInterMask,
										cannibalMode,
										maskIsolated,
										in currentBlockMap,
										in currentLineMap,
										in currentInterMap
									);
									if (context.OnlyFindOne)
									{
										return step;
									}

									context.Accumulator.Add(step);
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
