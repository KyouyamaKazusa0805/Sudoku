#define TYPE_UNIQUE_RECTANGLE_BLOOMING

namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Death Blossom</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Death Blossom</item>
/// <item>Death Blossom (House Blooming)</item>
/// <item>Death Blossom (Rectangle Blooming)</item>
/// <item>Death Blossom (A^nLS Blooming)</item>
/// </list>
/// </summary>
[StepSearcher(
	Technique.DeathBlossom, Technique.HouseDeathBlossom, Technique.RectangleDeathBlossom,
	Technique.NTimesAlmostLockedSetDeathBlossom)]
[StepSearcherRuntimeName("StepSearcherName_DeathBlossomStepSearcher")]
public sealed partial class DeathBlossomStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates whether the step searcher searches for extended types.
	/// </summary>
	[RuntimeIdentifier(RuntimeIdentifier.SearchExtendedDeathBlossomTypes)]
	public bool SearchExtendedTypes { get; set; }


	/// <inheritdoc/>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="/g/developer-notes" />
	/// <para>
	/// This searcher uses a trick that starts with eliminations. From the solution, we can learn which candidates can be removed
	/// from the grid in fact.
	/// </para>
	/// <para>
	/// Just suppose one of them is correct, and this case will lead to a confliction if some ALSes related to that supposed digit
	/// can lead to a same cell to be null (i.e. no digits can be filled).
	/// Therefore, we can remove the supposed digit from the containing cell.
	/// </para>
	/// <para>
	/// This algorithm just makes the calculation simplified, in order to avoid complex searching on ALSes
	/// and a same-deletion leading.
	/// </para>
	/// <para>
	/// Test example:
	/// <code><![CDATA[
	/// # Normal Blossom
	/// 8+7+14..6..5+42...7..+93+6+8+57+42+1.28.7.+3.....3.6+2....+3+21.54..1.7..+93.+3.9...8+72.+87..3+1.4:
	///   915 919 924 929 641 946 948 958 959 669 475 675 476 584 694
	/// 
	/// # A^nLS Blossom
	/// 65.9....4..2.......18..63...957.........6......+6..125..+643..59.+5.+1+6..7..3+7+9..2.86:
	///   129 239 845 847 751 352 856 465 865 869 288
	/// ]]></code>
	/// </para>
	/// </remarks>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		scoped var alses = AlmostLockedSetsModule.CollectAlmostLockedSets(in grid);
		scoped var alsesUsed = (stackalloc CellMap[90]); // First 10 elements are not used.
		scoped var usedIndex = (stackalloc int[729]);
		scoped var finalCells = (stackalloc Cell[9]);
		scoped var selectedCellDigitsMask = (stackalloc Mask[9]);
		scoped var selectedAlsEntryCell = (stackalloc Cell[9]);
		scoped var alsReferenceTable = (stackalloc Candidate[729]);
		alsReferenceTable.Fill(-1);
		var accumulatorNormal = new List<DeathBlossomStep>();
		var accumulatorHouse = new List<HouseDeathBlossomStep>();
		var accumulatorRectangle = new List<RectangleDeathBlossomStep>();
		var accumulatorNTimesAls = new List<NTimesAlmostLockedSetDeathBlossomStep>();

		// Iterate on each cell to collect cell-blooming type.
		var playgroundCached = grid.ToCandidateMaskArray();
		foreach (var entryElimCell in EmptyCells)
		{
			if (PopCount((uint)playgroundCached[entryElimCell]) < 2)
			{
				// If the cell only contain one candidate (i.e. a Naked Single), the cell won't produce any possible eliminations.
				// Just skip it.
				continue;
			}

			// Iterate on each wrong digit, to find any confliction.
			var wrongDigitsMask = (Mask)(playgroundCached[entryElimCell] & ~(1 << Solution.GetDigit(entryElimCell)));
			foreach (var wrongDigit in wrongDigitsMask)
			{
				int satisfiedSize;
				var availablePivots = CellMap.Empty;
				var playground = grid.ToCandidateMaskArray();

				// Iterate on each ALS, to find related ALSes.
				for (var alsCurrentIndex = 0; alsCurrentIndex < alses.Length; alsCurrentIndex++)
				{
					var (alsDigitsMask, _, _, alsElimMap) = alses[alsCurrentIndex];

					// Determine whether the ALS current iterated contain the wrong digit (that will make the link work).
					// Also, the ALS should see the entry elimination cell.
					if ((alsDigitsMask >> wrongDigit & 1) == 0 || !alsElimMap[wrongDigit].Contains(entryElimCell))
					{
						continue;
					}

					// If the found ALS is valid, iterate on each digits from the ALS, as selected one.
					foreach (var currentSelectedDigit in (Mask)(alsDigitsMask & ~(1 << wrongDigit)))
					{
						// Iterate on each pivot cells that contains the selected digit.
						foreach (var pivot in alsElimMap[currentSelectedDigit])
						{
							// Determine whether the playground (a copy from the current grid) doesn't contain the digit
							// from the selected cell. If so, the digit won't work.
							if ((playground[pivot] >> currentSelectedDigit & 1) == 0)
							{
								continue;
							}

							// Try to delete it from playground, and check for whether the target cell has no possible candidates.
							playground[pivot] &= (Mask)~(1 << currentSelectedDigit);
							alsReferenceTable[pivot * 9 + currentSelectedDigit] = alsCurrentIndex;
							availablePivots.Add(pivot);

							// Check for normal type.
							if (playground[pivot] == 0
								&& CreateStep_NormalType(ref context, in grid, pivot, alsReferenceTable, alses, playgroundCached, accumulatorNormal) is { } normalTypeStep)
							{
								return normalTypeStep;
							}

							if (!SearchExtendedTypes)
							{
								continue;
							}

							// Check for house type.
							foreach (var houseType in HouseTypes)
							{
								var house = pivot.ToHouseIndex(houseType);
								var disappearedDigitsMask = Grid.MaxCandidatesMask;
								foreach (var cell in HouseCells[house])
								{
									disappearedDigitsMask &= (Mask)~playground[cell];
								}

								if (disappearedDigitsMask != 0
									&& CreateStep_HouseType(ref context, in grid, house, disappearedDigitsMask, alsReferenceTable, alses, playgroundCached, accumulatorHouse) is { } houseTypeStep)
								{
									return houseTypeStep;
								}
							}

#if TYPE_UNIQUE_RECTANGLE_BLOOMING
							// Check for UR type.

							// Today I don't have any possible examples to test for this type,
							// so I don't know whether the module is correctly-impl'ed.
							foreach (CellMap urCells in UniqueRectangleModule.PossiblePatterns)
							{
								var existsGivenCellsOrMoreThan2ModifiableCells = false;
								var (modifiableCellsCount, fixedDigitsMask, mergedDigitsMask) = (0, (Mask)0, (Mask)0);
								foreach (var cell in urCells)
								{
									switch (grid.GetState(cell))
									{
										case CellState.Given:
										case CellState.Modifiable when ++modifiableCellsCount >= 3:
										{
											existsGivenCellsOrMoreThan2ModifiableCells = true;
											break;
										}
										case CellState.Modifiable when (Mask)(1 << grid.GetDigit(cell)) is var digitMask:
										{
											mergedDigitsMask |= digitMask;
											fixedDigitsMask |= digitMask;
											break;
										}
										case CellState.Empty:
										{
											mergedDigitsMask |= grid.GetCandidates(cell);
											break;
										}
									}
								}
								if (existsGivenCellsOrMoreThan2ModifiableCells)
								{
									// The current pattern cells contains at least one given cell, invalid for checking for URs;
									// or the number of modifiable cells is greater than 2, meaning the pattern must contain
									// either three different digits, or a naked single.
									continue;
								}

								// Iterate on each pair of UR digits, and check whether other digits are eliminated in all four cells.
								foreach (var urDigits in ((Mask)(mergedDigitsMask & ~fixedDigitsMask)).GetAllSets().GetSubsets(2))
								{
									var urDigitsMask = MaskOperations.Create(urDigits);
									var allCellsOnlyContainUrDigits = true;
									foreach (var cell in urCells)
									{
										if ((urDigitsMask & playgroundCached[cell]) != playgroundCached[cell])
										{
											allCellsOnlyContainUrDigits = false;
											break;
										}
									}
									if (!allCellsOnlyContainUrDigits)
									{
										continue;
									}

									if (CreateStep_RectangleType(
										ref context, in grid, urDigitsMask, mergedDigitsMask, in urCells, alsReferenceTable, alses,
										playgroundCached, accumulatorRectangle
									) is { } rectangleTypeStep)
									{
										return rectangleTypeStep;
									}
								}
							}
#endif
						}
					}
				}

				// We should check for whether a user allowed searching for complex types at first.
				if (!SearchExtendedTypes || !availablePivots)
				{
					return null;
				}

				// Try to search for advanced type.
				// Here is a very complex case:
				// The main idea of the type is to suppose the ALSes can make a subset to form an invalid state
				// (i.e. (n) cells only contain at most (n - 1) kinds of digits).

				// Try to suppose for the target wrong digit, removing from its peer cells.
				foreach (var deletionCell in PeersMap[entryElimCell] & CandidatesMap[wrongDigit])
				{
					playground[deletionCell] &= (Mask)~(1 << wrongDigit);
				}

				finalCells.Clear();
				selectedCellDigitsMask.Clear();
				selectedAlsEntryCell.Clear();

				// Check for available pivots collected before, and iterate on each house that available pivot cells lie in.
				// This will help the following logic collect A^nLSes.
				// A "A^nLS" is a "n-Times ALS" (The sign '^' means the exponent, i.e. multiple A's),
				// meaning a pattern uses (m) cells, but contain (m + n) kinds of digits.
				foreach (var availablePivotHouse in availablePivots.Houses)
				{
					// If the current house don't contain enough empty cells to form a A^nLS, skip for it.
					if ((HousesMap[availablePivotHouse] & availablePivots) is not (var pivotsLyingInHouse and not []))
					{
						continue;
					}

					// Check for pre-eliminations if any possible digits appeared in the cell.
					// This will be used to form a weak link (a connection).
					var preeliminationsCount = -1;
					foreach (var cell in pivotsLyingInHouse)
					{
						if (playground[cell] != 0)
						{
							finalCells[++preeliminationsCount] = cell;
						}
					}

					// Add empty cells also.
					var tempCount = preeliminationsCount;
					foreach (var cell in (HousesMap[availablePivotHouse] & EmptyCells) - pivotsLyingInHouse)
					{
						if (PopCount((uint)playground[cell]) >= 2)
						{
							finalCells[++tempCount] = cell;
						}
					}

					// Iterate on them, to form a A^nLS. Here I use a very tricky way to check,
					// because a sudoku grid must contain at most 9 cells, and an ALS or A^nLS at most use 8 of 9 cells.
					// Just iterate them using a for-loop to get them.
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
				if (CreateStep_NTimesAlsType(
					ref context, in grid, alses, alsesUsed, usedIndex, selectedAlsEntryCell, satisfiedSize,
					selectedCellDigitsMask, wrongDigit, alsReferenceTable, accumulatorNTimesAls
				) is { } anlsBloomingTypeStep)
				{
					return anlsBloomingTypeStep;
				}
			}
		}

		if (!context.OnlyFindOne)
		{
			if (accumulatorNormal.Count != 0)
			{
				context.Accumulator.AddRange(accumulatorNormal);
			}

			if (accumulatorHouse.Count != 0)
			{
				context.Accumulator.AddRange(EquatableStep.Distinct(accumulatorHouse));
			}

			if (accumulatorRectangle.Count != 0)
			{
				context.Accumulator.AddRange(EquatableStep.Distinct(accumulatorRectangle));
			}

			if (accumulatorNTimesAls.Count != 0)
			{
				ComparableStep.Order(accumulatorNTimesAls);
				context.Accumulator.AddRange(EquatableStep.Distinct(accumulatorNTimesAls));
			}
		}

		return null;
	}

	/// <summary>
	/// Create a <see cref="DeathBlossomStep"/> instance and add it into the accumulator.
	/// </summary>
	private DeathBlossomStep? CreateStep_NormalType(
		scoped ref AnalysisContext context,
		scoped ref readonly Grid grid,
		Cell pivot,
		scoped Span<int> alsReferenceTable,
		scoped ReadOnlySpan<AlmostLockedSet> alses,
		Mask[] playgroundCached,
		List<DeathBlossomStep> accumulator
	)
	{
		// A normal death blossom is found. Now check for eliminations.
		// A "Z Digit" is a digit as the target digit for the target eliminations.
		// Due to the flexibility of the technique, "Z digit" may not only hold one.
		// Therefore, here 'zDigitsMask' is not a digit, but a 'Mask' instance.
		var zDigitsMask = (Mask)0;
		var branches = new NormalBlossomBranchCollection();
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

		// Delete invalid digits.
		zDigitsMask &= (Mask)~pivotDigitsMask;

		// Collect information for branch cells, checking whether branch cells contain any possible Z digits.
		var branchCellsContainingZ = CellMap.Empty;
		foreach (var digit in zDigitsMask)
		{
			foreach (var (_, branchCells) in branches.Values)
			{
				branchCellsContainingZ |= branchCells & CandidatesMap[digit];
			}
		}

		// Check for eliminations.
		var (validZ, conclusions) = ((Mask)0, new List<Conclusion>());
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

		// Collect for view nodes.
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
							: (zDigitsMask >> digit & 1) != 0 ? WellKnownColorIdentifier.Auxiliary1 : alsColor,
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

		accumulator.Add(step);
		return null;
	}

	/// <summary>
	/// Create a <see cref="HouseDeathBlossomStep"/> instance and add it into the accumulator.
	/// </summary>
	private HouseDeathBlossomStep? CreateStep_HouseType(
		scoped ref AnalysisContext context,
		scoped ref readonly Grid grid,
		House house,
		Mask disappearedDigitsMask,
		scoped Span<int> alsReferenceTable,
		scoped ReadOnlySpan<AlmostLockedSet> alses,
		Mask[] playgroundCached,
		List<HouseDeathBlossomStep> accumulator
	)
	{
		// A house death blossom is found. Now check for eliminations.
		foreach (var disappearedDigit in disappearedDigitsMask)
		{
			var branches = new HouseBlossomBranchCollection();
			var zDigitsMask = (Mask)0;
			var targetCells = HousesMap[house] & CandidatesMap[disappearedDigit];
			var isFirstEncountered = true;
			foreach (var cell in targetCells)
			{
				var branch = alses[alsReferenceTable[cell * 9 + disappearedDigit]];
				branches.Add(cell, branch);

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

			// Delete invalid digits.
			zDigitsMask &= (Mask)~(1 << disappearedDigit);

			// Collect information for branch cells, checking whether branch cells contain any possible Z digits.
			var branchCellsContainingZ = CellMap.Empty;
			foreach (var digit in zDigitsMask)
			{
				foreach (var (_, branchCells) in branches.Values)
				{
					branchCellsContainingZ |= branchCells & CandidatesMap[digit];
				}
			}

			// Check for eliminations.
			var (validZ, conclusions) = ((Mask)0, new List<Conclusion>());
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

			// Collect for view nodes.
			var cellOffsets = new List<CellViewNode>();
			var candidateOffsets = (List<CandidateViewNode>)[
				..
				from cell in targetCells
				select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, cell * 9 + disappearedDigit)
			];
			var houseOffset = new HouseViewNode(WellKnownColorIdentifier.Normal, house);
			var detailViews = new View[branches.Count];
			var i = 0;
			foreach (ref var view in detailViews.AsSpan())
			{
				view = [houseOffset, new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, targetCells[i++] * 9 + disappearedDigit)];
			}

			var indexOfAls = 0;
			foreach (var (_, (_, alsCells)) in branches)
			{
				foreach (var alsCell in alsCells)
				{
					var alsColor = AlmostLockedSetsModule.GetColor(indexOfAls);
					foreach (var digit in grid.GetCandidates(alsCell))
					{
						var node = new CandidateViewNode(
							disappearedDigit == digit
								? WellKnownColorIdentifier.Auxiliary2
								: (zDigitsMask >> digit & 1) != 0 ? WellKnownColorIdentifier.Auxiliary1 : alsColor,
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

			var step = new HouseDeathBlossomStep(
				[.. conclusions],
				[[.. cellOffsets, .. candidateOffsets, houseOffset], .. detailViews],
				context.PredefinedOptions,
				house,
				disappearedDigit,
				branches,
				validZ
			);
			if (context.OnlyFindOne)
			{
				return step;
			}

			accumulator.Add(step);
		}

		return null;
	}

#if TYPE_UNIQUE_RECTANGLE_BLOOMING
	/// <summary>
	/// Search for A^nLS blooming type, and create a <see cref="RectangleDeathBlossomStep"/> instance
	/// and add it into the accumulator if worth.
	/// </summary>
	private RectangleDeathBlossomStep? CreateStep_RectangleType(
		scoped ref AnalysisContext context,
		scoped ref readonly Grid grid,
		Mask urDigitsMask,
		Mask mergedDigitsMask,
		scoped ref readonly CellMap urCells,
		scoped Span<int> alsReferenceTable,
		scoped ReadOnlySpan<AlmostLockedSet> alses,
		Mask[] playgroundCached,
		List<RectangleDeathBlossomStep> accumulator
	)
	{
		// An invalid UR pattern is found. Now check for eliminations.
		var branches = new RectangleBlossomBranchCollection();
		var isFirstEncountered = true;
		var zDigitsMask = (Mask)0;
		var disappearedCandidates = CandidateMap.Empty;
		foreach (var urCell in urCells)
		{
			foreach (var digit in (Mask)(grid.GetCandidates(urCell) & ~urDigitsMask))
			{
				var branch = alses[alsReferenceTable[urCell * 9 + digit]];
				branches.Add(urCell * 9 + digit, branch);

				disappearedCandidates.Add(urCell * 9 + digit);

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
		}

		// Delete invalid digits.
		zDigitsMask &= (Mask)~(Mask)(mergedDigitsMask & ~urDigitsMask);

		// Collect information for branch cells, checking whether branch cells contain any possible Z digits.
		var branchCellsContainingZ = CellMap.Empty;
		foreach (var digit in zDigitsMask)
		{
			foreach (var (_, branchCells) in branches.Values)
			{
				branchCellsContainingZ |= branchCells & CandidatesMap[digit];
			}
		}

		// Check for eliminations.
		var (validZ, conclusions) = ((Mask)0, new List<Conclusion>());
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

		// Collect for view nodes.
		var cellOffsets = new List<CellViewNode>();
		var candidateOffsets = (List<CandidateViewNode>)[.. from c in disappearedCandidates select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, c)];
		var detailViews = new View[branches.Count];
		var i = 0;
		foreach (ref var view in detailViews.AsSpan())
		{
			view = [new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, disappearedCandidates[i++])];
		}

		var indexOfAls = 0;
		foreach (var (_, (_, alsCells)) in branches)
		{
			foreach (var alsCell in alsCells)
			{
				var alsColor = AlmostLockedSetsModule.GetColor(indexOfAls);
				foreach (var digit in grid.GetCandidates(alsCell))
				{
					var node = new CandidateViewNode(
						disappearedCandidates.Contains(alsCell * 9 + digit)
							? WellKnownColorIdentifier.Auxiliary2
							: (zDigitsMask >> digit & 1) != 0 ? WellKnownColorIdentifier.Auxiliary1 : alsColor,
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

		var step = new RectangleDeathBlossomStep(
			[.. conclusions],
			[[.. cellOffsets, .. candidateOffsets], .. detailViews],
			context.PredefinedOptions,
			in urCells,
			!!(urCells - EmptyCells),
			branches,
			validZ
		);
		if (context.OnlyFindOne)
		{
			return step;
		}

		accumulator.Add(step);
		return null;
	}
#endif

	/// <summary>
	/// Search for A^nLS blooming type, and create a <see cref="NTimesAlmostLockedSetDeathBlossomStep"/> instance
	/// and add it into the accumulator if worth.
	/// </summary>
	private NTimesAlmostLockedSetDeathBlossomStep? CreateStep_NTimesAlsType(
		scoped ref AnalysisContext context,
		scoped ref readonly Grid grid,
		scoped ReadOnlySpan<AlmostLockedSet> alses,
		scoped Span<CellMap> alsesUsed,
		scoped Span<Cell> usedIndex,
		scoped Span<Cell> selectedAlsEntryCell,
		int satisfiedSize,
		scoped Span<Mask> selectedCellDigitsMask,
		Digit wrongDigit,
		scoped Span<Candidate> alsReferenceTable,
		List<NTimesAlmostLockedSetDeathBlossomStep> accumulator
	)
	{
		alsesUsed[9..].Clear();
		usedIndex.Clear();

		// Check for Z digits.
		//var clrCands = grid.ToCandidateMaskArray();
		var (usedAlsesCount, tCand, zDigitsMask, entryCellDigitsMask, indexUsed2All) = (0, (Mask)0, (Mask)0, (Mask)0, new int[10]);
		foreach (var cell in selectedAlsEntryCell[..satisfiedSize])
		{
			var currentCellDigitsMask = grid.GetCandidates(cell);
			entryCellDigitsMask |= currentCellDigitsMask;

			tCand = (Mask)(currentCellDigitsMask & ~(selectedCellDigitsMask[satisfiedSize - 1] | (Mask)(1 << wrongDigit)));
			foreach (var digit in tCand)
			{
				var candidate = cell * 9 + digit;
				scoped ref var currentUsedIndex = ref usedIndex[alsReferenceTable[candidate]];
				if (currentUsedIndex == 0)
				{
					currentUsedIndex = ++usedAlsesCount;
					indexUsed2All[currentUsedIndex] = alsReferenceTable[candidate];
				}

				Debug.Assert(usedAlsesCount <= 10, "There's a special case that more than 10 branches found.");

				alsesUsed[currentUsedIndex * 9 + digit].Add(cell);
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

		// Check for the complex type. I don't implement the fully check, e.g. A 0-rank pattern checking.
		// I just reserve the code and implement in the future.
		var complexType = (entryCellDigitsMask >> wrongDigit & 1, selectedCellDigitsMask[satisfiedSize - 1] >> wrongDigit & 1) switch
		{
			(not 0, 0) => 1,
			_ => 2
		};
		if (complexType == 1)
		{
			zDigitsMask &= (Mask)(selectedCellDigitsMask[satisfiedSize - 1] | (Mask)(1 << wrongDigit));
		}

		// Collect for view nodes.
		var cellOffsets = new List<CellViewNode>();
		var candidateOffsets = new List<CandidateViewNode>();
		var alsIndex = 0;
		var detailViews = new List<View>(9);
		var branches = new NTimesAlmostLockedSetsBlossomBranchCollection();
		var nTimesAlsDigitsMask = (Mask)0;
		var nTimesAlsCells = CellMap.Empty;
		var cellsAllAlsesUsed = CellMap.Empty;
		for (var usedAlsIndex = 1; usedAlsIndex <= usedAlsesCount; usedAlsIndex++)
		{
			var rcc = (Mask)0;
			var branchCandidates = CandidateMap.Empty;
			var view = new View();
			var branchDigit = -1;
			for (var currentDigit = 0; currentDigit < 9; currentDigit++)
			{
				if (!alsesUsed[usedAlsIndex * 9 + currentDigit])
				{
					continue;
				}

				nTimesAlsDigitsMask |= (Mask)(1 << currentDigit);
				branchDigit = currentDigit;
				rcc |= (Mask)(1 << currentDigit);
				foreach (var cell in alsesUsed[usedAlsIndex * 9 + currentDigit])
				{
					if (grid.Exists(cell, currentDigit) is true)
					{
						var candidateNode = new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, cell * 9 + currentDigit);
						view.Add(candidateNode);
						candidateOffsets.Add(candidateNode);

						branchCandidates.Add(cell * 9 + currentDigit);
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

			branches.Add(branchCandidates, targetAls);
			detailViews.Add(view);
			alsIndex++;
		}

		// Collect for eliminations.
		//var rank0 = false;
		var temp = (CellMap)(ReadOnlySpan<Cell>)selectedAlsEntryCell[..satisfiedSize];
		var conclusions = new List<Conclusion>();
		foreach (var digit in zDigitsMask)
		{
			var elimMap = cellsAllAlsesUsed;
			if (complexType == 1)
			{
				elimMap |= temp;
			}

			//if (((cellsAllAlsesUsed | temp) & CandidatesMap[digit]).InOneHouse(out _))
			//{
			//	rank0 = true;
			//}

			foreach (var cell in elimMap % CandidatesMap[digit])
			{
				conclusions.Add(new(Elimination, cell, digit));
			}
		}
		if (conclusions.Count == 0)
		{
			return null;
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

		accumulator.Add(step);
		return null;
	}
}
