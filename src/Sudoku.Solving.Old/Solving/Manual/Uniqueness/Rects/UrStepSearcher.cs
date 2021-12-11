namespace Sudoku.Solving.Manual.Uniqueness.Rects;

/// <summary>
/// Encapsulates an <b>unique rectangle</b> (UR) or
/// <b>avoidable rectangle</b> (AR) technique searcher.
/// </summary>
public sealed partial class UrStepSearcher : UniquenessStepSearcher
{
	/// <summary>
	/// Indicates whether the UR can be incomplete. In other words,
	/// some of UR candidates can be removed before the pattern forms.
	/// </summary>
	public bool AllowIncompleteUniqueRectangles { get; set; }

	/// <summary>
	/// Indicates whether the searcher can search for extended URs.
	/// </summary>
	public bool SearchForExtendedUniqueRectangles { get; set; }

	/// <summary>
	/// Indicates the searcher properties.
	/// </summary>
	/// <remarks>
	/// Please note that all technique searches should contain
	/// this static property in order to display on settings window. If the searcher doesn't contain,
	/// when we open the settings window, it'll throw an exception to report about this.
	/// </remarks>
	public static TechniqueProperties Properties { get; } = new(8, nameof(Technique.UrType1))
	{
		DisplayLevel = 2
	};


	/// <inheritdoc/>
	public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
	{
		// Iterate on mode (whether use AR or UR mode to search).
		var tempList = new List<UrStepInfo>();
		GetAll(tempList, grid, false);
		GetAll(tempList, grid, true);

		// Sort and remove duplicate instances if worth.
		if (tempList.Count == 0)
		{
			return;
		}

		accumulator.AddRange(
			from info in tempList.RemoveDuplicateItems()
			orderby info.TechniqueCode, info.AbsoluteOffset
			select info
		);
	}

	/// <summary>
	/// Get all possible hints from the grid.
	/// </summary>
	/// <param name="gathered">The list stored the result.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="arMode">Indicates whether the current mode is searching for ARs.</param>
	private void GetAll(IList<UrStepInfo> gathered, in SudokuGrid grid, bool arMode)
	{
		// Iterate on each possible UR structure.
		for (int index = 0, l = PossibleUrList.Length; index < l; index++)
		{
			int[] urCells = PossibleUrList[index];

			// Check preconditions.
			if (!CheckPreconditions(grid, urCells, arMode))
			{
				continue;
			}

			// Get all candidates that all four cells appeared.
			short mask = 0;
			foreach (int urCell in urCells)
			{
				mask |= grid.GetCandidates(urCell);
			}

			// Iterate on each possible digit combination.
			var allDigitsInThem = mask.GetAllSets();
			for (
				int i = 0, length = allDigitsInThem.Length, iterationLength = length - 1;
				i < iterationLength;
				i++
			)
			{
				int d1 = allDigitsInThem[i];
				for (int j = i + 1; j < length; j++)
				{
					int d2 = allDigitsInThem[j];

					// All possible UR patterns should contain at least one cell
					// that contains both 'd1' and 'd2'.
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
						CheckGuardianUnique(gathered, grid, urCells, comparer, d1, d2, index);
						CheckGuardianAvoidable(gathered, grid, urCells, comparer, d1, d2, index);
					}

					// Iterate on each corner of four cells.
					for (int c1 = 0; c1 < 4; c1++)
					{
						int corner1 = urCells[c1];
						var otherCellsMap = new Cells(urCells) { ~corner1 };

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
						// Therefore, the 'break' clause should be written here, rather than
						// 'for' loop declaration.
						// If that declaration is 'for (int c1 = 0; c1 < 3; c1++)',
						// we'll miss the cases for checking the last cell.
						if (c1 == 3)
						{
							break;
						}

						for (int c2 = c1 + 1; c2 < 4; c2++)
						{
							int corner2 = urCells[c2];
							var tempOtherCellsMap = new Cells(otherCellsMap) { ~corner2 };

							// Both diagonal and non-diagonal.
							CheckType2(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);

							if (SearchForExtendedUniqueRectangles)
							{
								for (int size = 2; size <= 4; size++)
								{
									CheckWing(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, size, index);
								}
							}

							switch ((A: c1, B: c2))
							{
								case (A: 0, B: 3) or (A: 1, B: 2): // Diagonal type.
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
										if (SearchForExtendedUniqueRectangles)
										{
											CheckHiddenSingleAvoidable(gathered, grid, urCells, d1, d2, corner1, corner2, tempOtherCellsMap, index);
										}
									}

									break;
								}
								default: // Non-diagonal type.
								{
									CheckType3Naked(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
									CheckType3Hidden(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);

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
										CheckSdc(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
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
	/// Check preconditions.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is searching for ARs.</param>
	/// <returns>Indicates whether the UR is passed to check.</returns>
	private static bool CheckPreconditions(in SudokuGrid grid, IEnumerable<int> urCells, bool arMode)
	{
		byte emptyCountWhenArMode = 0, modifiableCount = 0;
		foreach (int urCell in urCells)
		{
			switch (grid.GetStatus(urCell))
			{
				case CellStatus.Given:
				case CellStatus.Modifiable when !arMode:
				{
					return false;
				}
				case CellStatus.Empty when arMode:
				{
					emptyCountWhenArMode++;
					break;
				}
				case CellStatus.Modifiable:
				{
					modifiableCount++;
					break;
				}
			}
		}

		return modifiableCount != 4 && emptyCountWhenArMode != 4;
	}


	/// <summary>
	/// To determine whether the specified region forms a conjugate pair
	/// of the specified digit, and the cells where they contain the digit
	/// is same as the given map contains.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <param name="map">The map.</param>
	/// <param name="region">The region.</param>
	/// <returns>A <see cref="bool"/> value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsConjugatePair(int digit, in Cells map, int region) =>
		(RegionMaps[region] & CandMaps[digit]) == map;

	/// <summary>
	/// Check whether the highlight UR candidates is incomplete.
	/// </summary>
	/// <param name="list">The list to check.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool IsIncompleteUr(IEnumerable<DrawingInfo> list) =>
		!AllowIncompleteUniqueRectangles && list.Count(static pair => pair.Id == 0) != 8;

	/// <summary>
	/// Get a cell that can't see each other.
	/// </summary>
	/// <param name="urCells">The UR cells.</param>
	/// <param name="cell">The current cell.</param>
	/// <returns>The diagonal cell.</returns>
	/// <exception cref="ArgumentException">
	/// Throws when the specified argument <paramref name="cell"/> is invalid.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetDiagonalCell(int[] urCells, int cell) =>
		cell == urCells[0]
		? urCells[3]
		: cell == urCells[1]
		? urCells[2]
		: cell == urCells[2] ? urCells[1] : urCells[0];

	/// <summary>
	/// Get whether two cells are in a same region.
	/// </summary>
	/// <param name="cell1">The cell 1 to check.</param>
	/// <param name="cell2">The cell 2 to check.</param>
	/// <param name="region">
	/// The result regions that both two cells lie in. If the cell can't be found, this argument will be 0.
	/// </param>
	/// <returns>
	/// The <see cref="bool"/> value indicating whether the another cell is same region as the current one.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsSameRegionCell(int cell1, int cell2, out int region)
	{
		(bool r, region) = new Cells { cell1, cell2 }.CoveredRegions is var v and not 0 ? (true, v) : (false, 0);
		return r;
	}

	/// <summary>
	/// Get all highlight cells.
	/// </summary>
	/// <param name="urCells">The all UR cells used.</param>
	/// <returns>The list of highlight cells.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static IReadOnlyList<DrawingInfo> GetHighlightCells(int[] urCells) => new DrawingInfo[4]
	{
		new(0, urCells[0]), new(0, urCells[1]), new(0, urCells[2]), new(0, urCells[3])
	};
}
