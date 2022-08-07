namespace Sudoku.Solving.Manual.Searchers;

[StepSearcher]
internal sealed unsafe partial class UniqueRectangleStepSearcher : IUniqueRectangleStepSearcher
{
	/// <inheritdoc/>
	[SearcherProperty]
	public bool AllowIncompleteUniqueRectangles { get; set; }

	/// <inheritdoc/>
	[SearcherProperty]
	public bool SearchForExtendedUniqueRectangles { get; set; }


	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		var list = new List<UniqueRectangleStep>();

		// Iterate on mode (whether use AR or UR mode to search).
		GetAll(list, grid, false);
		GetAll(list, grid, true);

		if (list.Count == 0)
		{
			return null;
		}

		// Sort and remove duplicate instances if worth.
		var resultList =
			from step in IDistinctableStep<UniqueRectangleStep>.Distinct(list)
			orderby step.TechniqueCode, step.AbsoluteOffset
			select step;
		if (onlyFindOne)
		{
			goto ReturnFirst;
		}

		accumulator.AddRange(resultList);

	ReturnFirst:
		return resultList.FirstOrDefault();
	}

	/// <summary>
	/// Get all possible hints from the grid.
	/// </summary>
	/// <param name="gathered">The list stored the result.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="arMode">Indicates whether the current mode is searching for ARs.</param>
	private void GetAll(ICollection<UniqueRectangleStep> gathered, scoped in Grid grid, bool arMode)
	{
		// Iterate on each possible UR structure.
		for (int index = 0, outerLength = UniqueRectanglePatterns.Length; index < outerLength; index++)
		{
			int[] urCells = UniqueRectanglePatterns[index];

			// Check preconditions.
			if (!IUniqueRectangleStepSearcher.CheckPreconditions(grid, urCells, arMode))
			{
				continue;
			}

			// Get all candidates that all four cells appeared.
			short mask = grid.GetDigitsUnion(urCells);

			// Iterate on each possible digit combination.
			scoped var allDigitsInThem = mask.GetAllSets();
			for (int i = 0, length = allDigitsInThem.Length, innerLength = length - 1; i < innerLength; i++)
			{
				int d1 = allDigitsInThem[i];
				for (int j = i + 1; j < length; j++)
				{
					int d2 = allDigitsInThem[j];

					// All possible UR patterns should contain at least one cell that contains both 'd1' and 'd2'.
					short comparer = (short)(1 << d1 | 1 << d2);
					bool isNotPossibleUr = true;
					foreach (int cell in urCells)
					{
						if (PopCount((uint)(grid.GetCandidates(cell) & comparer)) == 2)
						{
							isNotPossibleUr = false;
							break;
						}
					}
					if (!arMode && isNotPossibleUr)
					{
						continue;
					}

					if (SearchForExtendedUniqueRectangles)
					{
						CheckUnknownCoveringUnique(gathered, grid, urCells, comparer, d1, d2, index);
						CheckExternalType1Or2(gathered, grid, urCells, d1, d2, index);
						CheckExternalType3(gathered, grid, urCells, comparer, d1, d2, index);
						CheckExternalType4(gathered, grid, urCells, comparer, d1, d2, index);
					}

					// Iterate on each corner of four cells.
					for (int c1 = 0; c1 < 4; c1++)
					{
						int corner1 = urCells[c1];
						var otherCellsMap = (Cells)urCells - corner1;

						CheckType1(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, otherCellsMap, index);
						CheckType5(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, otherCellsMap, index);
						CheckHidden(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, otherCellsMap, index);

						if (!arMode && SearchForExtendedUniqueRectangles)
						{
							Check3X(gathered, grid, urCells, false, comparer, d1, d2, corner1, otherCellsMap, index);
							Check3X2SL(gathered, grid, urCells, false, comparer, d1, d2, corner1, otherCellsMap, index);
							Check3N2SL(gathered, grid, urCells, false, comparer, d1, d2, corner1, otherCellsMap, index);
							Check3U2SL(gathered, grid, urCells, false, comparer, d1, d2, corner1, otherCellsMap, index);
							Check3E2SL(gathered, grid, urCells, false, comparer, d1, d2, corner1, otherCellsMap, index);
						}

						// If we aim to a single cell, all four cells should be checked.
						// Therefore, the 'break' clause should be written here,
						// rather than continuing the execution.
						// In addition, I think you may ask me a question why the outer for loop is limited
						// the variable 'c1' from 0 to 4 instead of 0 to 3.
						// If so, we'll miss the cases for checking the last cell.
						if (c1 == 3)
						{
							break;
						}

						for (int c2 = c1 + 1; c2 < 4; c2++)
						{
							int corner2 = urCells[c2];
							var tempOtherCellsMap = otherCellsMap - corner2;

							// Both diagonal and non-diagonal.
							CheckType2(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);

							if (SearchForExtendedUniqueRectangles)
							{
								for (int size = 2; size <= 4; size++)
								{
									CheckRegularWing(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, size, index);
								}
							}

							switch (c1, c2)
							{
								// Diagonal type.
								case (0, 3) or (1, 2):
								{
									if (!arMode)
									{
										CheckType6(gathered, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);

										if (SearchForExtendedUniqueRectangles)
										{
											Check2D(gathered, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
											Check2D1SL(gathered, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
										}
									}
									else
									{
										// Don't merge this else-if block. The code in this block may be extended.
										if (SearchForExtendedUniqueRectangles)
										{
											CheckHiddenSingleAvoidable(gathered, grid, urCells, d1, d2, corner1, corner2, tempOtherCellsMap, index);
										}
									}

									break;
								}

								// Non-diagonal type.
								default:
								{
									CheckType3(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);

									if (!arMode)
									{
										CheckType4(gathered, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);

										if (SearchForExtendedUniqueRectangles)
										{
											Check2B1SL(gathered, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
											Check4X3SL(gathered, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
											Check4C3SL(gathered, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
										}
									}

									if (SearchForExtendedUniqueRectangles)
									{
										CheckSueDeCoq(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
									}

									break;
								}
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Check whether the highlight UR candidates is incomplete.
	/// </summary>
	/// <param name="list">The list to check.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	/// <remarks>
	/// This method uses a trick to check a UR structure: to count up the number of "Normal colored"
	/// candidates used in the current UR structure. If and only if the full structure uses 8 candidates
	/// colored with normal one, the structure will be complete.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool IsIncomplete(IEnumerable<CandidateViewNode> list)
	{
		return !AllowIncompleteUniqueRectangles && list.Count(predicate) != 8;


		static bool predicate(CandidateViewNode d)
			=> d.Identifier is { Mode: IdentifierColorMode.Named, NamedKind: DisplayColorKind.Normal };
	}

	partial void CheckType1(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, scoped in Cells otherCellsMap, int index);
	partial void CheckType2(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, scoped in Cells otherCellsMap, int index);
	partial void CheckType3(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, scoped in Cells otherCellsMap, int index);
	partial void CheckType4(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, scoped in Cells otherCellsMap, int index);
	partial void CheckType5(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, scoped in Cells otherCellsMap, int index);
	partial void CheckType6(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, scoped in Cells otherCellsMap, int index);
	partial void CheckHidden(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, scoped in Cells otherCellsMap, int index);
	partial void Check2D(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, scoped in Cells otherCellsMap, int index);
	partial void Check2B1SL(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, scoped in Cells otherCellsMap, int index);
	partial void Check2D1SL(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, scoped in Cells otherCellsMap, int index);
	partial void Check3X(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, scoped in Cells otherCellsMap, int index);
	partial void Check3X2SL(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, scoped in Cells otherCellsMap, int index);
	partial void Check3N2SL(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, scoped in Cells otherCellsMap, int index);
	partial void Check3U2SL(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, scoped in Cells otherCellsMap, int index);
	partial void Check3E2SL(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, scoped in Cells otherCellsMap, int index);
	partial void Check4X3SL(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, scoped in Cells otherCellsMap, int index);
	partial void Check4C3SL(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, scoped in Cells otherCellsMap, int index);
	partial void CheckRegularWing(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, scoped in Cells otherCellsMap, int size, int index);
	partial void CheckSueDeCoq(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, scoped in Cells otherCellsMap, int index);
	partial void CheckUnknownCoveringUnique(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, short comparer, int d1, int d2, int index);
	partial void CheckExternalType1Or2(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, int d1, int d2, int index);
	partial void CheckExternalType3(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, short comparer, int d1, int d2, int index);
	partial void CheckExternalType4(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, short comparer, int d1, int d2, int index);
	partial void CheckHiddenSingleAvoidable(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, int d1, int d2, int corner1, int corner2, scoped in Cells otherCellsMap, int index);
}
