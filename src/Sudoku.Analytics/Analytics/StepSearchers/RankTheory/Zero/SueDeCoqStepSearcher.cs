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
[StepSearcher(new[] { DifficultyLevel.Fiendish })]
public sealed partial class SueDeCoqStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override unsafe Step? Collect(scoped ref AnalysisContext context)
	{
		// A valid SdC needs at least 4 cells like:
		//
		//     abcd abcd | ab
		//     cd        |
		if (EmptyCells.Count < 4)
		{
			return null;
		}

		var cannibalModeCases = stackalloc[] { false, true };

		scoped ref readonly var grid = ref context.Grid;
		using scoped var list = new ValueList<CellMap>(4);
		for (var caseIndex = 0; caseIndex < 2; caseIndex++)
		{
			var cannibalMode = cannibalModeCases[caseIndex];
			foreach (var ((baseSet, coverSet), (a, b, c, _)) in IntersectionMaps)
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
						list.Add(emptyCellsInInterMap);

						break;
					}
					case [var i, var j, var k]:
					{
						list.Add(CellsMap[i] + j);
						list.Add(CellsMap[j] + k);
						list.Add(CellsMap[i] + k);
						list.Add(emptyCellsInInterMap);

						break;
					}
				}

				// Iterate on each intersection combination.
				foreach (var currentInterMap in list)
				{
					var selectedInterMask = grid.GetDigitsUnion(currentInterMap);
					if (PopCount((uint)selectedInterMask) <= currentInterMap.Count + 1)
					{
						// The intersection combination is an ALS or a normal subset,
						// which is invalid in SdCs.
						continue;
					}

					var blockMap = (b | c - currentInterMap) & EmptyCells;
					var lineMap = a & EmptyCells;

					// Iterate on the number of the cells that should be selected in block.
					for (var i = 1; i < blockMap.Count; i++)
					{
						// Iterate on each combination in block.
						foreach (var currentBlockMap in blockMap & i)
						{
							var blockMask = grid.GetDigitsUnion(currentBlockMap);
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
								foreach (var currentLineMap in lineMap & j)
								{
									var lineMask = grid.GetDigitsUnion(currentLineMap);
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
											: selectedInterMask & ~(blockMask | lineMask)
									);
									var maskOnlyInInter = (Mask)(selectedInterMask & ~(blockMask | lineMask));
									if (!cannibalMode
										&& ((blockMask & lineMask) != 0 || maskIsolated != 0 && !IsPow2(maskIsolated))
										|| cannibalMode && !IsPow2(maskIsolated))
									{
										continue;
									}

									var elimMapIsolated = CellMap.Empty;
									var digitIsolated = TrailingZeroCount(maskIsolated);
									if (digitIsolated != InvalidTrailingZeroCountMethodFallback)
									{
										elimMapIsolated = (
											cannibalMode
												? (currentBlockMap | currentLineMap)
												: currentInterMap
										) % CandidatesMap[digitIsolated] & EmptyCells;
									}

									if (currentInterMap.Count + i + j == PopCount((uint)blockMask) + PopCount((uint)lineMask) + PopCount((uint)maskOnlyInInter)
										&& (elimMapBlock | elimMapLine | elimMapIsolated) is not [])
									{
										// Check eliminations.
										var conclusions = new List<Conclusion>();
										foreach (var cell in elimMapBlock)
										{
											foreach (var digit in grid.GetCandidates(cell))
											{
												if ((blockMask >> digit & 1) != 0)
												{
													conclusions.Add(
														new(Elimination, cell, digit)
													);
												}
											}
										}
										foreach (var cell in elimMapLine)
										{
											foreach (var digit in grid.GetCandidates(cell))
											{
												if ((lineMask >> digit & 1) != 0)
												{
													conclusions.Add(
														new(Elimination, cell, digit)
													);
												}
											}
										}
										foreach (var cell in elimMapIsolated)
										{
											conclusions.Add(
												new(Elimination, cell, digitIsolated)
											);
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
														!cannibalMode && digit == digitIsolated
															? WellKnownColorIdentifier.Auxiliary2
															: WellKnownColorIdentifier.Normal,
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
														!cannibalMode && digit == digitIsolated
															? WellKnownColorIdentifier.Auxiliary2
															: WellKnownColorIdentifier.Auxiliary1,
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
															? WellKnownColorIdentifier.Auxiliary2
															: (blockMask >> digit & 1) != 0
																? WellKnownColorIdentifier.Normal
																: WellKnownColorIdentifier.Auxiliary1,
														cell * 9 + digit
													)
												);
											}
										}

										var step = new SueDeCoqStep(
											conclusions.ToArray(),
											new[]
											{
												View.Empty
													| candidateOffsets
													| new HouseViewNode[]
													{
														new(WellKnownColorIdentifier.Normal, coverSet),
														new(WellKnownColorIdentifier.Auxiliary2, baseSet)
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
											currentInterMap
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
		}

		return null;
	}
}
