using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Metadata;
using Sudoku.Analytics.Steps;
using Sudoku.Analytics.StepSearcherModules;
using Sudoku.Concepts;
using Sudoku.Concepts.ObjectModel;
using Sudoku.Rendering;
using Sudoku.Rendering.Nodes;
using static System.Numerics.BitOperations;
using static Sudoku.Analytics.CachedFields;
using static Sudoku.Analytics.ConclusionType;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Death Blossom</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Death Blossom</item>
/// </list>
/// </summary>
[StepSearcher(Technique.DeathBlossom)]
[StepSearcherRuntimeName("StepSearcherName_DeathBlossomStepSearcher")]
public sealed partial class DeathBlossomStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates whether the step searcher searches for extended types.
	/// </summary>
	[RuntimeIdentifier(RuntimeIdentifier.SearchExtendedDeathBlossomTypes)]
	public bool SearchExtendedTypes { get; set; }


	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		scoped var alses = AlmostLockedSetsModule.CollectAlmostLockedSets(in grid);

		var accumulatorNormal = new List<DeathBlossomStep>();
		var accumulatorComplex = new List<NTimesAlmostLockedSetDeathBlossomStep>();
		var alsesUsed = new CellMap[10, 9];
		var usedIndex = new int[729];
		var alsReferenceTable = new int[729];
		Array.Fill(alsReferenceTable, -1);

		// Iterate on each cell to collect cell-blossom type.
		var playgroundCached = grid.ToCandidateMaskArray();
		foreach (var entryElimCell in EmptyCells)
		{
			if (PopCount((uint)playgroundCached[entryElimCell]) < 2)
			{
				// If the cell only contain one candidate (i.e. a Naked Single), the cell won't produce any possible eliminations.
				// Just skip it.
				continue;
			}

			var wrongDigitsMask = (Mask)(playgroundCached[entryElimCell] & ~(1 << Solution.GetDigit(entryElimCell)));
			foreach (var wrongDigit in wrongDigitsMask)
			{
				int satisfiedSize;
				var availablePivots = CellMap.Empty;
				var playground = grid.ToCandidateMaskArray();
				for (var alsCurrentIndex = 0; alsCurrentIndex < alses.Length; alsCurrentIndex++)
				{
					var (alsDigitsMask, _, _, alsElimMap) = alses[alsCurrentIndex];
					if ((alsDigitsMask >> wrongDigit & 1) == 0 || !alsElimMap[wrongDigit].Contains(entryElimCell))
					{
						continue;
					}

					foreach (var currentSelectedDigit in (Mask)(alsDigitsMask & ~(1 << wrongDigit)))
					{
						foreach (var pivot in alsElimMap[currentSelectedDigit])
						{
							if ((playground[pivot] >> currentSelectedDigit & 1) == 0)
							{
								continue;
							}

							playground[pivot] &= (Mask)~(1 << currentSelectedDigit);
							alsReferenceTable[pivot * 9 + currentSelectedDigit] = alsCurrentIndex;
							availablePivots.Add(pivot);

							// We should ensure the target cell should be empty.
							if (playground[pivot] != 0)
							{
								continue;
							}

							var zDigitsMask = (Mask)0;
							var branches = new BlossomBranch();
							var pivotDigitsMask = grid.GetCandidates(pivot);
							var isFirstEncountered = true;
							foreach (var pivotDigit in pivotDigitsMask)
							{
								var branch = alses[alsReferenceTable[pivot * 9 + pivotDigit]];
								branches.Add(pivotDigit, branch);

								if (isFirstEncountered)
								{
									zDigitsMask = branch.DigitsMask;
									isFirstEncountered = false;
								}
								else
								{
									zDigitsMask &= branch.DigitsMask;
								}
							}
							zDigitsMask &= (Mask)~pivotDigitsMask;

							var branchCellsContainingZ = CellMap.Empty;
							foreach (var digit in zDigitsMask)
							{
								foreach (var (_, branchCells) in branches.Values)
								{
									branchCellsContainingZ |= branchCells & CandidatesMap[digit];
								}
							}

							var validZ = (Mask)0;
							var conclusions = new List<Conclusion>();
							foreach (var zDigit in zDigitsMask)
							{
								if (branchCellsContainingZ % CandidatesMap[zDigit] is not (var elimMap and not []))
								{
									continue;
								}

								validZ |= (Mask)(1 << zDigit);
								foreach (var c in elimMap)
								{
									conclusions.Add(new(Elimination, c, zDigit));

									if (SearchExtendedTypes)
									{
										playgroundCached[c] &= (Mask)~zDigit;
									}
								}
							}

							var cellOffsets = new List<CellViewNode> { new(WellKnownColorIdentifier.Normal, pivot) };
							var candidateOffsets = new List<CandidateViewNode>();
							var detailViews = new View[branches.Count];
							foreach (ref var view in detailViews.AsSpan())
							{
								view = [new CellViewNode(WellKnownColorIdentifier.Normal, pivot)];
							}

							var indexOfAls = 0;
							foreach (var digit in grid.GetCandidates(pivot))
							{
								var node = new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, pivot * 9 + digit);
								candidateOffsets.Add(node);
								detailViews[indexOfAls++].Add(node);
							}

							indexOfAls = 0;
							foreach (var (branchDigit, (_, alsCells)) in branches)
							{
								foreach (var alsCell in alsCells)
								{
									var alsColor = AlmostLockedSetsModule.GetColor(indexOfAls);
									foreach (var digit in grid.GetCandidates(alsCell))
									{
										var node = new CandidateViewNode(
											branchDigit == digit
												? WellKnownColorIdentifier.Auxiliary2
												: (zDigitsMask >> digit & 1) != 0
													? WellKnownColorIdentifier.Auxiliary1
													: alsColor,
											alsCell * 9 + digit
										);
										candidateOffsets.Add(node);
										detailViews[indexOfAls].Add(node);
									}

									var cellNode = new CellViewNode(alsColor, alsCell);
									cellOffsets.Add(cellNode);
									detailViews[indexOfAls].Add(cellNode);
								}

								indexOfAls++;
							}

							var step = new DeathBlossomStep(
								[.. conclusions],
								[[.. cellOffsets, .. candidateOffsets], .. detailViews],
								context.PredefinedOptions,
								pivot,
								branches,
								validZ
							);
							if (context.OnlyFindOne)
							{
								return step;
							}

							accumulatorNormal.Add(step);
						}
					}
				}

				if (!SearchExtendedTypes || !availablePivots)
				{
					return null;
				}

				// Try to search for advanced type.
				// The main idea of the type is to suppose the ALSes can make a subset to form an invalid state
				// (i.e. n cells only contain at most (n - 1) kinds of digits).

				// Try to suppose for the target wrong digit, removing from its peer cells.
				foreach (var deletionCell in PeersMap[entryElimCell] & CandidatesMap[wrongDigit])
				{
					playground[deletionCell] &= (Mask)~(1 << wrongDigit);
				}

				var finalCells = new int[9];
				var selectedCellDigitsMask = new Mask[9];
				var selectedAlsEntryCell = new int[9];
				foreach (var availablePivotHouse in availablePivots.Houses)
				{
					if ((HousesMap[availablePivotHouse] & availablePivots) is not (var pivotsLyingInHouse and not []))
					{
						continue;
					}

					var preeliminationsCount = -1;
					foreach (var cell in pivotsLyingInHouse)
					{
						if (playground[cell] != 0)
						{
							finalCells[++preeliminationsCount] = cell;
						}
					}

					var tempCount = preeliminationsCount;
					foreach (var cell in (HousesMap[availablePivotHouse] & EmptyCells) - pivotsLyingInHouse)
					{
						if (PopCount((uint)playground[cell]) >= 2)
						{
							finalCells[++tempCount] = cell;
						}
					}

					for (satisfiedSize = 2; satisfiedSize <= 8; satisfiedSize++)
					{
						for (var a = 0; a <= preeliminationsCount; a++)
						{
							selectedCellDigitsMask[0] = playground[finalCells[a]];
							selectedAlsEntryCell[0] = finalCells[a];
							for (var b = a + 1; b <= tempCount; b++)
							{
								selectedCellDigitsMask[1] = (Mask)(selectedCellDigitsMask[0] | playground[finalCells[b]]);
								selectedAlsEntryCell[1] = finalCells[b];
								if (PopCount((uint)selectedCellDigitsMask[1]) < 2) { goto AlmostAlmostLockedSetDeletion; }
								if (satisfiedSize < 3) { continue; }

								for (var c = b + 1; c <= tempCount; c++)
								{
									selectedCellDigitsMask[2] = (Mask)(selectedCellDigitsMask[1] | playground[finalCells[c]]);
									selectedAlsEntryCell[2] = finalCells[c];
									if (PopCount((uint)selectedCellDigitsMask[2]) < 3) { goto AlmostAlmostLockedSetDeletion; }
									if (satisfiedSize < 4) { continue; }

									for (var d = c + 1; d <= tempCount; d++)
									{
										selectedCellDigitsMask[3] = (Mask)(selectedCellDigitsMask[2] | playground[finalCells[d]]);
										selectedAlsEntryCell[3] = finalCells[d];
										if (PopCount((uint)selectedCellDigitsMask[3]) < 4) { goto AlmostAlmostLockedSetDeletion; }
										if (satisfiedSize < 5) { continue; }

										for (var e = d + 1; e <= tempCount; e++)
										{
											selectedCellDigitsMask[4] = (Mask)(selectedCellDigitsMask[3] | playground[finalCells[e]]);
											selectedAlsEntryCell[4] = finalCells[e];
											if (PopCount((uint)selectedCellDigitsMask[4]) < 5) { goto AlmostAlmostLockedSetDeletion; }
											if (satisfiedSize < 6) { continue; }

											for (var f = e + 1; f <= tempCount; f++)
											{
												selectedCellDigitsMask[5] = (Mask)(selectedCellDigitsMask[4] | playground[finalCells[f]]);
												selectedAlsEntryCell[5] = finalCells[f];
												if (PopCount((uint)selectedCellDigitsMask[5]) < 6) { goto AlmostAlmostLockedSetDeletion; }
												if (satisfiedSize < 7) { continue; }

												for (var g = f + 1; g <= tempCount; g++)
												{
													selectedCellDigitsMask[6] = (Mask)(selectedCellDigitsMask[5] | playground[finalCells[g]]);
													selectedAlsEntryCell[6] = finalCells[g];
													if (PopCount((uint)selectedCellDigitsMask[6]) < 7) { goto AlmostAlmostLockedSetDeletion; }
													if (satisfiedSize < 8) { continue; }

													for (var h = g + 1; h <= tempCount; h++)
													{
														selectedCellDigitsMask[7] = (Mask)(selectedCellDigitsMask[6] | playground[finalCells[h]]);
														selectedAlsEntryCell[7] = finalCells[h];
														if (PopCount((uint)selectedCellDigitsMask[7]) < 8) { goto AlmostAlmostLockedSetDeletion; }
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

				continue;

			AlmostAlmostLockedSetDeletion:
				{
					Array.Clear(alsesUsed);
					Array.Clear(usedIndex);

					//var clrCands = grid.ToCandidateMaskArray();
					var (usedAlsesCount, tCand, zDigitsMask, entryCellDigitsMask, indexUsed2All) = (0, (Mask)0, (Mask)0, (Mask)0, new int[10]);
					foreach (var cell in selectedAlsEntryCell.AsReadOnlySpan()[..satisfiedSize])
					{
						var currentCellDigitsMask = grid.GetCandidates(cell);
						entryCellDigitsMask |= currentCellDigitsMask;
						foreach (var digit in tCand = (Mask)(
							currentCellDigitsMask
								& ~(selectedCellDigitsMask[satisfiedSize - 1] | (Mask)(1 << wrongDigit))
						))
						{
							var candidate = cell * 9 + digit;
							scoped ref var currentUsedIndex = ref usedIndex[alsReferenceTable[candidate]];
							if (currentUsedIndex == 0)
							{
								currentUsedIndex = ++usedAlsesCount;
								indexUsed2All[currentUsedIndex] = alsReferenceTable[candidate];
							}

							Debug.Assert(usedAlsesCount <= 10, "There's a special case that more than 10 branches found.");

							alsesUsed[currentUsedIndex, digit].Add(cell);
							if (zDigitsMask == 0)
							{
								zDigitsMask = (Mask)(alses[indexUsed2All[currentUsedIndex]].DigitsMask & ~(1 << digit));
							}
							else
							{
								zDigitsMask &= (Mask)(alses[indexUsed2All[currentUsedIndex]].DigitsMask & ~(1 << digit));
							}
						}
					}

					var complexType = (
						entryCellDigitsMask >> wrongDigit & 1,
						selectedCellDigitsMask[satisfiedSize - 1] >> wrongDigit & 1
					) switch
					{ (not 0, 0) => 1, _ => 2 };
					if (complexType == 1)
					{
						zDigitsMask &= (Mask)(selectedCellDigitsMask[satisfiedSize - 1] | (Mask)(1 << wrongDigit));
					}

					var cellOffsets = new List<CellViewNode>();
					var candidateOffsets = new List<CandidateViewNode>();
					var alsIndex = 0;
					var detailViews = new List<View>(9);
					var branches = new BlossomBranch();
					var nTimesAlsDigitsMask = (Mask)0;
					var nTimesAlsCells = CellMap.Empty;
					var cellsAllAlsesUsed = CellMap.Empty;
					for (var usedAlsIndex = 1; usedAlsIndex <= usedAlsesCount; usedAlsIndex++)
					{
						var rcc = (Mask)0;
						var view = new View();
						var branchDigit = -1;
						for (var currentDigit = 0; currentDigit < 9; currentDigit++)
						{
							if (!alsesUsed[usedAlsIndex, currentDigit])
							{
								continue;
							}

							nTimesAlsDigitsMask |= (Mask)(1 << currentDigit);
							branchDigit = currentDigit;
							rcc |= (Mask)(1 << currentDigit);
							foreach (var cell in alsesUsed[usedAlsIndex, currentDigit])
							{
								if (grid.Exists(cell, currentDigit) is true)
								{
									var candidateNode = new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, cell * 9 + currentDigit);
									view.Add(candidateNode);
									candidateOffsets.Add(candidateNode);
								}

								var node = new CellViewNode(WellKnownColorIdentifier.Normal, cell);
								view.Add(node);
								cellOffsets.Add(node);
								//clrCands[cell] &= (Mask)~tCand;

								nTimesAlsCells.Add(cell);
							}
						}

						cellsAllAlsesUsed |= alses[indexUsed2All[usedAlsIndex]].Cells;
						var targetAls = alses[indexUsed2All[usedAlsIndex]];
						foreach (var cell in targetAls.Cells)
						{
							var cellNode = new CellViewNode(AlmostLockedSetsModule.GetColor(alsIndex), cell);
							view.Add(cellNode);
							cellOffsets.Add(cellNode);

							foreach (var digit in grid.GetCandidates(cell))
							{
								var colorIdentifier = (rcc >> digit & 1) != 0
									? WellKnownColorIdentifier.Auxiliary2
									: (zDigitsMask >> digit & 1) != 0
										? WellKnownColorIdentifier.Auxiliary1
										: AlmostLockedSetsModule.GetColor(alsIndex);
								var candidateNode = new CandidateViewNode(colorIdentifier, cell * 9 + digit);
								view.Add(candidateNode);
								candidateOffsets.Add(candidateNode);
							}
						}

						branches.TryAdd(branchDigit, targetAls);
						detailViews.Add(view);
						alsIndex++;
					}

					//var rank0 = false;
					var temp = (CellMap)([.. selectedAlsEntryCell.AsSpan()[..satisfiedSize]]);
					var conclusions = new List<Conclusion>();
					foreach (var digit in zDigitsMask)
					{
						var elimMap = cellsAllAlsesUsed;
						if (complexType == 1)
						{
							elimMap |= temp;
						}

						elimMap &= CandidatesMap[digit];
						//if (((cellsAllAlsesUsed | temp) & CandidatesMap[digit]).InOneHouse(out _))
						//{
						//	rank0 = true;
						//}

						foreach (var cell in elimMap.PeerIntersection & CandidatesMap[digit])
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var step = new NTimesAlmostLockedSetDeathBlossomStep(
						[.. conclusions],
						[[.. cellOffsets, .. candidateOffsets], .. detailViews],
						context.PredefinedOptions,
						nTimesAlsDigitsMask,
						in nTimesAlsCells,
						branches,
						PopCount((uint)grid[in nTimesAlsCells]) - nTimesAlsCells.Count
					);
					if (context.OnlyFindOne)
					{
						return step;
					}

					accumulatorComplex.Add(step);
				}
			}
		}

		if (!context.OnlyFindOne)
		{
			switch (accumulatorNormal.Count, accumulatorComplex.Count)
			{
				case (0, 0):
				{
					break;
				}
				case (_, 0):
				{
					context.Accumulator.AddRange(accumulatorNormal);
					break;
				}
				case (0, _):
				{
					context.Accumulator.AddRange(accumulatorComplex.Distinct());
					break;
				}
				default:
				{
					context.Accumulator.AddRange(accumulatorNormal);
					context.Accumulator.AddRange(accumulatorComplex.Distinct());
					break;
				}
			}
		}

		return null;
	}
}
