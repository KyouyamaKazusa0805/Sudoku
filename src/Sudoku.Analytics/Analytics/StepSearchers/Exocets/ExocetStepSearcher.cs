using System.Numerics;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Metadata;
using Sudoku.Analytics.Steps;
using Sudoku.Concepts;
using Sudoku.Linq;
using Sudoku.Rendering;
using Sudoku.Rendering.Nodes;
using Sudoku.Runtime.MaskServices;
using static System.Numerics.BitOperations;
using static Sudoku.Analytics.CachedFields;
using static Sudoku.Analytics.ConclusionType;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Junior Exocet</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Junior Exocet</item>
/// <item>Senior Exocet</item>
/// </list>
/// </summary>
[StepSearcher(Technique.JuniorExocet, Technique.SeniorExocet)]
public sealed partial class ExocetStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates the mini-lines to be iterated, grouped by chute index.
	/// </summary>
	private static readonly CellMap[][] MinilinesGroupedByChuteIndex;


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static ExocetStepSearcher()
	{
		MinilinesGroupedByChuteIndex = new CellMap[6][];
		for (var i = 0; i < 6; i++)
		{
			MinilinesGroupedByChuteIndex[i] = [[], [], [], [], [], [], [], [], []];

			var (_, _, _, chuteHouses) = Chutes[i];
			var isRow = i is 0 or 1 or 2;
			var tempIndex = 0;
			foreach (var chuteHouse in chuteHouses)
			{
				for (var (houseCell, j) = (HouseFirst[chuteHouse], 0); j < 3; houseCell += isRow ? 3 : 27, j++)
				{
					scoped ref var current = ref MinilinesGroupedByChuteIndex[i][tempIndex++];
					current.Add(houseCell);
					current.Add(houseCell + (isRow ? 1 : 9));
					current.Add(houseCell + (isRow ? 2 : 18));
				}
			}
		}
	}


	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		if (CollectForAllRowColumnType(ref context) is { } baseTypeStep)
		{
			return baseTypeStep;
		}

		return null;
	}

	/// <summary>
	/// Try to fetch for all-row or all-column types.
	/// </summary>
	/// <param name="context"><inheritdoc cref="StepSearcher.Collect(ref AnalysisContext)" path="/param[@name='context']"/></param>
	/// <returns><inheritdoc cref="StepSearcher.Collect(ref AnalysisContext)" path="/returns"/></returns>
	private ExocetStep? CollectForAllRowColumnType(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;

		// Iterate by size of houses to be iterated.
		foreach (var isRow in (true, false))
		{
			for (var size = 3; size <= 4; size++)
			{
				// Iterate on all possible rows and columns on size 3 or 4.
				foreach (var houses in (isRow ? AllRowsMask : AllColumnsMask).GetAllSets().GetSubsets(size))
				{
					var housesEmptyCells = CellMap.Empty;
					var housesCells = CellMap.Empty;
					var housesMask = MaskOperations.CreateHouse(houses);
					foreach (var house in houses)
					{
						housesEmptyCells |= HousesMap[house] & EmptyCells;
						housesCells |= HousesMap[house];
					}

					// Iterate on each chute (mega rows or columns) in order to check for each empty cell,
					// determining whether it can be used as a base.
					for (var (i, timesOfI) = (isRow ? 3 : 0, 0); timesOfI < 3; i++, timesOfI++)
					{
						var (_, chuteCells, _, chuteHouses) = Chutes[i];
						var chuteEmptyCells = chuteCells & EmptyCells;

						// Now iterate by size of base cells. The minimum value is 1, e.g.:
						//
						//   ..64.....1....39.7.5.............3..2....1.89....59....4......83....2....126...7.
						//
						// For digits 4 & 5 in houses r258, with base cell r9c7 and target cell r2c8 => r2c8 must be 4 or 5.
						for (var sizeOfBaseAndTarget = 1; sizeOfBaseAndTarget <= 2; sizeOfBaseAndTarget++)
						{
							// Iterate on each empty cells, or a cell group whose length is equal to iteration variable 'baseCellsSize'.
							scoped ref readonly var chuteHousesInCurrentChute = ref MinilinesGroupedByChuteIndex[i];
							for (var j = 0; j < 9; j++)
							{
								scoped ref readonly var minilineBaseCells = ref chuteHousesInCurrentChute[j];
								var baseEmptyCellsToBeIterated = minilineBaseCells & EmptyCells;
								if (!baseEmptyCellsToBeIterated)
								{
									// No cells can be iterated.
									continue;
								}

								// Iterate on each miniline, to get all possible cases.
								foreach (ref readonly var baseCells in baseEmptyCellsToBeIterated.GetSubsets(sizeOfBaseAndTarget).EnumerateRef())
								{
									if (housesEmptyCells & baseCells)
									{
										// Base cells shouldn't be located in the current list of houses being iterated.
										continue;
									}

									var baseCellsDigitsMask = grid[in baseCells];
									if (PopCount((uint)baseCellsDigitsMask) > baseCells.Count + 3)
									{
										// The base cells hold too much digits to be checked.
										continue;
									}

									// Now we should check for target cells.
									// The target cells must be located in houses being iterated, and intersects with the current chute.
									var targetCells = (chuteEmptyCells & housesEmptyCells) - baseCells.PeerIntersection;
									var generializedTargetCells = (housesCells & chuteCells) - baseCells.PeerIntersection;
									var targetCellsDigitsMask = grid[in targetCells];
									if ((targetCellsDigitsMask & baseCellsDigitsMask) == 0)
									{
										// They are out of relation.
										continue;
									}

									// Get the count delta (target.count - base.count). The result value must be -2, -1, 0, 1 and 2.
									// The details are mentioned below:
									//
									//   1) if < 0, the base contain more cells than the target, meaning the pattern may be a senior exocet;
									//   2) if > 0, the target contain more cells than the base,
									//      meaning the pattern may contain conjugate pairs of digits other than the mentioned ones;
									//   3) if == 0, the base has same number of cells with the target, a standard junior exocet will be formed.
									//
									// Other values (like 3) hold invalid cases we may not consider.
									var delta = targetCells.Count - baseCells.Count;

									// Note: Today we should only consider the cases on delta <= 0.
									// I'll adjust the code later for supporting on delta > 0.
									if (delta > 0)
									{
										continue;
									}

									// Note: Today I'll disable the case that both target cells are located in cross-line cells.
									// I'll adjust the code later for supporting on delta == -2.
									if (delta == -2)
									{
										continue;
									}

									// Check whether all digits appeared in base cells can be filled in target empty cells.
									var allDigitsCanBeFilledInTargetEmptyCells = true;
									foreach (var digit in baseCellsDigitsMask)
									{
										if (!(targetCells & CandidatesMap[digit]))
										{
											allDigitsCanBeFilledInTargetEmptyCells = false;
											break;
										}
									}
									if (!allDigitsCanBeFilledInTargetEmptyCells)
									{
										continue;
									}

									// Check whether generalized target cells (non-empty) don't contain
									// any possible digits appeared in base cells.
									var targetUncoveredCellsContainDigitsAppearedInBaseCells = false;
									foreach (var cell in generializedTargetCells - EmptyCells)
									{
										if ((baseCellsDigitsMask >> grid.GetDigit(cell) & 1) != 0)
										{
											targetUncoveredCellsContainDigitsAppearedInBaseCells = true;
											break;
										}
									}
									if (targetUncoveredCellsContainDigitsAppearedInBaseCells)
									{
										continue;
									}

									// Check whether cross-line non-empty cells contains digits appeared in base cells.
									// If so, they will be endo-target cells.
									// The maximum possible number of appearing times is 2, corresponding to the real target cells count.
									var crossline = housesCells - chuteCells;
									var crosslineContainsDigitsAppearedInBaseCells = false;
									foreach (var cell in crossline)
									{
										if ((baseCellsDigitsMask >> grid.GetDigit(cell) & 1) != 0)
										{
											crosslineContainsDigitsAppearedInBaseCells = true;
											break;
										}
									}
									if (crosslineContainsDigitsAppearedInBaseCells)
									{
										continue;
									}

									// Try to fetch all possible endo-target cells if worth.
									var endoTargetCells = CellMap.Empty;
									if (delta != 0)
									{
										// Here delta is strictly equal to -1 because I disable delta == -2 temporarily.
										foreach (var cell in crossline)
										{
											if (grid.GetState(cell) != CellState.Empty)
											{
												continue;
											}

											// Endo-target cells must contain at least one digit appeared in base cells.
											if ((grid.GetCandidates(cell) & baseCellsDigitsMask) == 0)
											{
												continue;
											}

											// Check if the current cell is filled with the digit not appeared in base cells,
											// then all base cell digits can only fill (size - 1) times at most in cross-line cells.
											// For example, if the size = 3, digits should only appear 2 times at most in cross-line cells.
											// If greater (times > size - 1), an exocet cannot be formed;
											// and if less (times < size - 1), we cannot conclude which digits are the target cells.
											var allDigitsCanBeFilledExactlySizeMinusOneTimes = true;
											foreach (var digit in baseCellsDigitsMask)
											{
												var mostTimes = MostTimesOf(digit, housesMask, chuteCells + cell);
												if (mostTimes != size - 1)
												{
													allDigitsCanBeFilledExactlySizeMinusOneTimes = false;
													break;
												}
											}
											if (!allDigitsCanBeFilledExactlySizeMinusOneTimes)
											{
												// All digits should strictly appear (size - 1) times at most in cross-line cells.
												continue;
											}

											endoTargetCells.Add(cell);
										}

										if (!endoTargetCells)
										{
											// No possible endo-target cells are found.
											continue;
										}
									}
									else
									{
										// Check for maximum times can be appeared in cross-line cells.
										var allDigitsCanBeFilledExactlySizeMinusOneTimes = true;
										foreach (var digit in baseCellsDigitsMask)
										{
											var mostTimes = MostTimesOf(digit, housesMask, in chuteCells);
											if (mostTimes != size - 1)
											{
												allDigitsCanBeFilledExactlySizeMinusOneTimes = false;
												break;
											}
										}
										if (!allDigitsCanBeFilledExactlySizeMinusOneTimes)
										{
											// All digits should strictly appear (size - 1) times at most in cross-line cells.
											// For example, if the size = 3, digits should only appear 2 times at most in cross-line cells.
											// If greater (times > size - 1), an exocet cannot be formed;
											// and if less (times < size - 1), we cannot conclude which digits are the target cells.
											continue;
										}
									}

									// An exocet will be formed.
									var conclusions = new List<Conclusion>();
									foreach (var cell in targetCells | endoTargetCells)
									{
										foreach (var digit in (Mask)(grid.GetCandidates(cell) & ~baseCellsDigitsMask))
										{
											conclusions.Add(new(Elimination, cell, digit));
										}
									}
									if (conclusions.Count == 0)
									{
										// No eliminations found.
										continue;
									}

									var gridCopied = grid;
									switch (delta)
									{
										case -1:
										{
											foreach (var endoTargetCell in endoTargetCells)
											{
												var step = new ExocetStep(
													[.. conclusions],
													[
														[
															..
															from cell in baseCells
															select new CellViewNode(WellKnownColorIdentifier.Normal, cell),
															..
															from cell in targetCells
															select new CellViewNode(WellKnownColorIdentifier.Auxiliary1, cell),
															..
															from cell in endoTargetCells
															select new CellViewNode(WellKnownColorIdentifier.Auxiliary1, cell),
															..
															from cell in crossline - endoTargetCells
															select new CellViewNode(WellKnownColorIdentifier.Auxiliary2, cell),
															..
															from cell in baseCells
															from digit in gridCopied.GetCandidates(cell)
															select new CandidateViewNode(WellKnownColorIdentifier.Normal, cell * 9 + digit),
															..
															from cell in crossline - endoTargetCells
															where gridCopied.GetState(cell) == CellState.Empty
															from digit in (Mask)(gridCopied.GetCandidates(cell) & baseCellsDigitsMask)
															select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, cell * 9 + digit)
														]
													],
													context.PredefinedOptions,
													baseCellsDigitsMask,
													in baseCells,
													in targetCells,
													[endoTargetCell],
													in crossline
												);
												if (context.OnlyFindOne)
												{
													return step;
												}

												context.Accumulator.Add(step);
											}

											break;
										}
										case 0:
										{
											var step = new ExocetStep(
												[.. conclusions],
												[
													[
														..
														from cell in baseCells select new CellViewNode(WellKnownColorIdentifier.Normal, cell),
														..
														from cell in targetCells
														select new CellViewNode(WellKnownColorIdentifier.Auxiliary1, cell),
														..
														from cell in crossline
														select new CellViewNode(WellKnownColorIdentifier.Auxiliary2, cell),
														..
														from cell in baseCells
														from digit in gridCopied.GetCandidates(cell)
														select new CandidateViewNode(WellKnownColorIdentifier.Normal, cell * 9 + digit),
														..
														from cell in crossline
														where gridCopied.GetState(cell) == CellState.Empty
														from digit in (Mask)(gridCopied.GetCandidates(cell) & baseCellsDigitsMask)
														select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, cell * 9 + digit)
													]
												],
												context.PredefinedOptions,
												baseCellsDigitsMask,
												in baseCells,
												in targetCells,
												[],
												in crossline
											);
											if (context.OnlyFindOne)
											{
												return step;
											}

											context.Accumulator.Add(step);
											break;
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


	/// <summary>
	/// Try to get the maximum times that the specified digit, describing it can be filled with the specified houses in maximal case.
	/// </summary>
	/// <param name="digit">The digit to be checked.</param>
	/// <param name="houses">The houses that the digit can be filled with.</param>
	/// <param name="excludedCells">
	/// Indicates the cells the method doesn't cover them. If the value is not <see cref="CellMap.Empty"/>,
	/// all cells in the houses should be checked.
	/// </param>
	/// <returns>
	/// <para>The number of times that the digit can be filled with the specified houses, at most.</para>
	/// <para>
	/// If any one of the houses from argument <paramref name="houses"/> doesn't contain that digit,
	/// or the digit has already been filled with that house as a value, the value will be 0. No exception will be thrown.
	/// </para>
	/// </returns>
	private static int MostTimesOf(Digit digit, HouseMask houses, scoped ref readonly CellMap excludedCells)
	{
		var cells = CandidatesMap[digit];
		var cellsInHouses = CellMap.Empty;
		foreach (var house in houses)
		{
			cellsInHouses |= HousesMap[house] - excludedCells;
		}
		cells &= cellsInHouses;

		for (var size = Math.Min(9, PopCount((uint)houses)); size >= 1; size--)
		{
			foreach (var cellsChosen in cells.GetSubsets(size))
			{
				if (size >= 2)
				{
					var duplicated = false;
					foreach (var cellPair in cellsChosen.GetSubsets(2))
					{
						if (cellPair.InOneHouse(out _))
						{
							duplicated = true;
							break;
						}
					}
					if (duplicated)
					{
						continue;
					}
				}

				return size;
			}
		}

		return 0;
	}
}
