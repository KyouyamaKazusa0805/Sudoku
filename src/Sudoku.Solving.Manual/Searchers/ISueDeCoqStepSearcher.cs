namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Sue de Coq</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Sue de Coq</item>
/// <item>Sue de Coq with Isolated Digit</item>
/// <item>Cannibalistic Sue de Coq</item>
/// </list>
/// </summary>
public interface ISueDeCoqStepSearcher : IRankTheoryStepSearcher
{
}

[StepSearcher]
internal sealed unsafe partial class SueDeCoqStepSearcher : ISueDeCoqStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		// A valid SdC needs at least 4 cells like:
		//
		//     abcd abcd | ab
		//     cd        |
		if (EmptyCells.Count < 4)
		{
			return null;
		}

		bool* cannibalModeCases = stackalloc[] { false, true };

		using scoped var list = new ValueList<Cells>(4);
		for (int caseIndex = 0; caseIndex < 2; caseIndex++)
		{
			bool cannibalMode = cannibalModeCases[caseIndex];
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
						list.Add(Cells.Empty + i + j);
						list.Add(Cells.Empty + j + k);
						list.Add(Cells.Empty + i + k);
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

					var blockMap = (b | c - currentInterMap) & EmptyCells;
					var lineMap = a & EmptyCells;

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
								elimMapBlock |= CandidatesMap[digit];
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
										elimMapLine |= CandidatesMap[digit];
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
										) % CandidatesMap[digitIsolated] & EmptyCells;
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
														new(Elimination, cell, digit)
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
														new(Elimination, cell, digit)
													);
												}
											}
										}
										foreach (int cell in elimMapIsolated)
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
										foreach (int cell in currentBlockMap)
										{
											foreach (int digit in grid.GetCandidates(cell))
											{
												candidateOffsets.Add(
													new(
														!cannibalMode && digit == digitIsolated
															? DisplayColorKind.Auxiliary2
															: DisplayColorKind.Normal,
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
														!cannibalMode && digit == digitIsolated
															? DisplayColorKind.Auxiliary2
															: DisplayColorKind.Auxiliary1,
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
															? DisplayColorKind.Auxiliary2
															: (blockMask >> digit & 1) != 0
																? DisplayColorKind.Normal
																: DisplayColorKind.Auxiliary1,
														cell * 9 + digit
													)
												);
											}
										}

										var step = new SueDeCoqStep(
											ImmutableArray.CreateRange(conclusions),
											ImmutableArray.Create(
												View.Empty
													| candidateOffsets
													| new HouseViewNode[]
													{
														new(DisplayColorKind.Normal, coverSet),
														new(DisplayColorKind.Auxiliary2, baseSet)
													}
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
