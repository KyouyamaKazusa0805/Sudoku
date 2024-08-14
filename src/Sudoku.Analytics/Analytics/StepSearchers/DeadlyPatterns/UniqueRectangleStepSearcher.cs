namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Unique Rectangle</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Basic types:
/// <list type="bullet">
/// <item>Unique Rectangle Type 1-6, Hidden Unique Rectangle</item>
/// <item>Avoidable Rectangle Type 1-3, 5, Hidden Avoidable Rectangle</item>
/// </list>
/// </item>
/// <item>
/// Other types:
/// <list type="bullet">
/// <item>Unique Rectangle + Conjugate Pair (also called "Unique Rectangle + Strong Link")</item>
/// <item>Avoidable Rectangle + Hidden Single</item>
/// <item>Unique Rectangle + Baba Grouping</item>
/// <item>Unique Rectangle + ALS-XZ</item>
/// <item>Avoidable Rectangle + ALS-XZ</item>
/// <item>Unique Rectangle + Sue de Coq</item>
/// <item>Avoidable Rectangle + Sue de Coq</item>
/// <item>Unique Rectangle + XY-Wing, XYZ-Wing, WXYZ-Wing and W-Wing</item>
/// <item>Avoidable Rectangle + XY-Wing, XYZ-Wing, WXYZ-Wing and W-Wing</item>
/// <item>
/// Unique/Avoidable Rectangle + Guardian (This program call it "External Types"):
/// <list type="bullet">
/// <item>Unique Rectangle External Type 1-4</item>
/// <item>Unique Rectangle External XY-Wing</item>
/// <item>Unique Rectangle External W-Wing</item>
/// <item>Unique Rectangle External ALS-XZ</item>
/// <item>Unique Rectangle External Turbot Fish</item>
/// </list>
/// </item>
/// <item>
/// Unique Rectangle burred types:
/// <list type="bullet">
/// <item>Unique Rectangle Burred Subset</item>
/// </list>
/// </item>
/// </list>
/// </item>
/// <item>
/// Miscellaneous:
/// <list type="bullet">
/// <item>Unique Rectangle 2D, 3X</item>
/// <item>Avoidable Rectangle 2D, 3X</item>
/// </list>
/// </item>
/// </list>
/// </summary>
[StepSearcher(
	"StepSearcherName_UniqueRectangleStepSearcher",

	// Basic types (Avoidable rectangle lacks type 4 and 6)
	Technique.UniqueRectangleType1, Technique.UniqueRectangleType2, Technique.UniqueRectangleType3, Technique.UniqueRectangleType4,
	Technique.UniqueRectangleType5, Technique.UniqueRectangleType6, Technique.HiddenUniqueRectangle,
	Technique.AvoidableRectangleType1, Technique.AvoidableRectangleType2, Technique.AvoidableRectangleType3,
	Technique.AvoidableRectangleType5, Technique.HiddenAvoidableRectangle,

	// Strong-link types (Conjugate pair types)
	Technique.UniqueRectangle2B1, Technique.UniqueRectangle2D1, Technique.UniqueRectangle3E2, Technique.UniqueRectangle3N2,
	Technique.UniqueRectangle3U2, Technique.UniqueRectangle3X1L, Technique.UniqueRectangle3X1U, Technique.UniqueRectangle3X2,
	Technique.UniqueRectangle4C3, Technique.UniqueRectangle4X1L, Technique.UniqueRectangle4X1U, Technique.UniqueRectangle4X2L,
	Technique.UniqueRectangle4X2U, Technique.UniqueRectangle4X3,

	// Burred types
	Technique.UniqueRectangleBurredSubset,

	// Pattern-based types
	Technique.UniqueRectangleBabaGrouping, Technique.UniqueRectangleSueDeCoq,
	Technique.UniqueRectangleSinglyLinkedAlmostLockedSetsXz, Technique.UniqueRectangleDoublyLinkedAlmostLockedSetsXz,
	Technique.UniqueRectangleXyWing, Technique.UniqueRectangleXyzWing, Technique.UniqueRectangleWxyzWing,
	Technique.UniqueRectangleWWing,
	Technique.AvoidableRectangleHiddenSingleBlock, Technique.AvoidableRectangleHiddenSingleRow,
	Technique.AvoidableRectangleHiddenSingleColumn, Technique.AvoidableRectangleSueDeCoq, Technique.AvoidableRectangleXyWing,
	Technique.AvoidableRectangleXyzWing, Technique.AvoidableRectangleWxyzWing, Technique.AvoidableRectangleWWing,
	Technique.AvoidableRectangleSinglyLinkedAlmostLockedSetsXz, Technique.AvoidableRectangleDoublyLinkedAlmostLockedSetsXz,

	// External types (UR/AR + Guardian)
	Technique.UniqueRectangleExternalType1, Technique.UniqueRectangleExternalType2, Technique.UniqueRectangleExternalType3,
	Technique.UniqueRectangleExternalType4, Technique.UniqueRectangleExternalXyWing, Technique.UniqueRectangleExternalTurbotFish,
	Technique.UniqueRectangleExternalWWing, Technique.UniqueRectangleExternalAlmostLockedSetsXz,
	Technique.AvoidableRectangleExternalType1, Technique.AvoidableRectangleExternalType2, Technique.AvoidableRectangleExternalType3,
	Technique.AvoidableRectangleExternalType4, Technique.AvoidableRectangleExternalXyWing, Technique.AvoidableRectangleExternalWWing,
	Technique.AvoidableRectangleExternalAlmostLockedSetsXz,

	// Miscellaneous
	Technique.UniqueRectangle2D, Technique.UniqueRectangle3X,
	Technique.AvoidableRectangle2D, Technique.AvoidableRectangle3X,

	SupportedSudokuTypes = SudokuType.Standard | SudokuType.Sukaku,
	SupportAnalyzingMultipleSolutionsPuzzle = false)]
public sealed partial class UniqueRectangleStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates whether the UR can be incomplete. In other words,
	/// some of UR candidates can be removed before the pattern forms.
	/// </summary>
	/// <remarks>
	/// For example, the complete pattern is:
	/// <code><![CDATA[
	/// ab | ab
	/// ab | ab
	/// ]]></code>
	/// This is a complete pattern, and we may remove an <c>ab</c> in a certain corner.
	/// The incomplete pattern may not contain all four <c>ab</c>s in the pattern.
	/// </remarks>
	[SettingItemName(SettingItemNames.AllowIncompleteUniqueRectangles)]
	public bool AllowIncompleteUniqueRectangles { get; set; }

	/// <summary>
	/// Indicates whether the searcher can search for extended URs.
	/// </summary>
	/// <remarks>
	/// The basic types are type 1 to type 6, all other types are extended ones.
	/// </remarks>
	[SettingItemName(SettingItemNames.SearchForExtendedUniqueRectangles)]
	public bool SearchForExtendedUniqueRectangles { get; set; }


	/// <inheritdoc/>
	protected internal override Step? Collect(ref StepAnalysisContext context)
	{
		ref readonly var grid = ref context.Grid;

		// Iterate on mode (whether use AR or UR mode to search).
		var list = new List<UniqueRectangleStep>();
		Collect(list, in grid, ref context, false);
		Collect(list, in grid, ref context, true);

		if (list.Count == 0)
		{
			return null;
		}

		// Sort and remove duplicate instances if worth.
		var sortedList = StepMarshal.RemoveDuplicateItems(list).ToList();
		StepMarshal.SortItems(sortedList);

		// Sukaku extra checking:
		// The pattern can be used if and only if the UR/AR cells can be form a complete pattern
		// at puzzle's initial state, meaning no digits can be missing.
		// For example:
		//
		//   ab | ab
		//   ab | ab
		//
		// This UR/AR pattern won't be formed, until the pattern at initial grid state forms without any missing candidates.
		// If not, digits appeared in this pattern cannot be swapped in all possible cases,
		// which cannot guarantee whether the deadly pattern at current state will be formed or not.
		// By using the same rule, we can also check for other possible deadly patterns such as unique loops
		// and Borescoper's deadly patterns.
		var isSukaku = grid.PuzzleType == SudokuType.Sukaku;
		var tempList = isSukaku ? new List<UniqueRectangleStep>(sortedList.Count) : sortedList;
		if (isSukaku)
		{
			ref readonly var initialGrid = ref context.InitialGrid;
			foreach (var element in sortedList)
			{
				var comparer = MaskOperations.Create(element.Digit1, element.Digit2);

				var isValid = true;
				foreach (var cell in element.Cells)
				{
					var digitsMask = initialGrid.GetCandidates(cell);
					if ((digitsMask & comparer) != comparer)
					{
						// Invalid because at least one digit used in UR disappeared from the UR cell.
						isValid = false;
						break;
					}
				}
				if (isValid)
				{
					if (context.OnlyFindOne)
					{
						return element;
					}

					tempList.Add(element);
				}
			}

			if (context.OnlyFindOne)
			{
				return tempList is [var firstStep, ..] ? firstStep : null;
			}
		}

		if (!context.OnlyFindOne)
		{
			context.Accumulator.AddRange(tempList);
			return null;
		}
		return tempList[0];
	}

	/// <summary>
	/// Get all possible hints from the grid.
	/// </summary>
	/// <param name="collected"><inheritdoc cref="StepAnalysisContext.Accumulator" path="/summary"/></param>
	/// <param name="grid"><inheritdoc cref="StepAnalysisContext.Grid" path="/summary"/></param>
	/// <param name="context">
	/// <inheritdoc cref="Collect(ref StepAnalysisContext)" path="/param[@name='context']"/>
	/// </param>
	/// <param name="arMode">Indicates whether the current mode is searching for ARs.</param>
	private void Collect(List<UniqueRectangleStep> collected, ref readonly Grid grid, ref StepAnalysisContext context, bool arMode)
	{
		// Search for ALSes. This result will be used by UR External ALS-XZ structures.
		var alses = AlmostLockedSetsModule.CollectAlmostLockedSets(in grid);

		// Iterate on each possible UR pattern.
		for (var index = 0; index < UniqueRectangleModule.PossiblePatterns.Length; index++)
		{
			var urCells = UniqueRectangleModule.PossiblePatterns[index];

			// Check preconditions.
			if (!CheckPreconditions(in grid, urCells, arMode))
			{
				continue;
			}

			// Get all candidates that all four cells appeared.
			var mask = grid[urCells.AsCellMap()];

			// Iterate on each possible digit combination.
			var allDigitsInThem = mask.GetAllSets();
			for (var (i, length) = (0, allDigitsInThem.Length); i < length - 1; i++)
			{
				var d1 = allDigitsInThem[i];
				for (var j = i + 1; j < length; j++)
				{
					var d2 = allDigitsInThem[j];

					// All possible UR patterns should contain at least one cell that contains both 'd1' and 'd2'.
					var comparer = (Mask)(1 << d1 | 1 << d2);
					var isNotPossibleUr = true;
					foreach (var cell in urCells)
					{
						if (Mask.PopCount((Mask)(grid.GetCandidates(cell) & comparer)) == 2)
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
						CheckBabaGroupingUnique(collected, in grid, ref context, urCells, comparer, d1, d2, index);
						CheckAlmostLockedSetsXz(collected, in grid, ref context, urCells, arMode, comparer, d1, d2, alses, index);
						CheckExternalType1Or2(collected, in grid, ref context, urCells, d1, d2, index, arMode);
						CheckExternalType3(collected, in grid, ref context, urCells, comparer, d1, d2, index, arMode);
						CheckExternalType4(collected, in grid, ref context, urCells, comparer, d1, d2, index, arMode);
						CheckExternalXyWing(collected, in grid, ref context, urCells, comparer, d1, d2, index, arMode);
						CheckExternalAlmostLockedSetsXz(collected, in grid, ref context, urCells, alses, comparer, d1, d2, index, arMode);

						if (!arMode)
						{
							CheckExternalTurbotFish(collected, in grid, ref context, urCells, comparer, d1, d2, index);
							CheckExternalWWing(collected, in grid, ref context, urCells, comparer, d1, d2, index);
						}
					}

					// Iterate on each corner of four cells.
					for (var c1 = 0; c1 < 4; c1++)
					{
						var corner1 = urCells[c1];
						var cellsExcluding1CornerCell = urCells.AsCellMap() - corner1;

						CheckType1(collected, in grid, ref context, urCells, arMode, comparer, d1, d2, corner1, in cellsExcluding1CornerCell, index);
						CheckType5(collected, in grid, ref context, urCells, arMode, comparer, d1, d2, corner1, in cellsExcluding1CornerCell, index);
						CheckHidden(collected, in grid, ref context, urCells, arMode, comparer, d1, d2, corner1, in cellsExcluding1CornerCell, index);

						if (!arMode && SearchForExtendedUniqueRectangles)
						{
							Check3X2SL(collected, in grid, ref context, urCells, false, comparer, d1, d2, corner1, in cellsExcluding1CornerCell, index);
							Check3N2SL(collected, in grid, ref context, urCells, false, comparer, d1, d2, corner1, in cellsExcluding1CornerCell, index);
							Check3U2SL(collected, in grid, ref context, urCells, false, comparer, d1, d2, corner1, in cellsExcluding1CornerCell, index);
							Check3E2SL(collected, in grid, ref context, urCells, false, comparer, d1, d2, corner1, in cellsExcluding1CornerCell, index);
						}

						// If we aim to a single cell, all four cells should be checked.
						// Therefore, the 'break' clause should be written here, rather than continuing the execution.
						// In addition, I think you may ask me a question why the outer for loop is limited
						// the variable 'c1' from 0 to 4 instead of 0 to 3. If so, we'll miss the cases for checking the last cell.
						if (c1 == 3)
						{
							break;
						}

						for (var c2 = c1 + 1; c2 < 4; c2++)
						{
							var corner2 = urCells[c2];
							var cellsExcluding2CornerCells = cellsExcluding1CornerCell - corner2;

							// Both diagonal and non-diagonal.
							CheckType2(collected, in grid, ref context, urCells, arMode, comparer, d1, d2, corner1, corner2, in cellsExcluding2CornerCells, index);
							if (SearchForExtendedUniqueRectangles)
							{
								CheckRegularWing(collected, in grid, ref context, urCells, arMode, comparer, d1, d2, corner1, corner2, in cellsExcluding2CornerCells, index, (c1, c2) is (0, 3) or (1, 2));
								//CheckWWing(collected, in grid, ref context, urCells, arMode, comparer, d1, d2, corner1, corner2, in cellsExcluding2CornerCells, index);
							}

							switch (c1, c2)
							{
								// Diagonal type.
								case (0, 3) or (1, 2):
								{
									if (arMode)
									{
										if (SearchForExtendedUniqueRectangles)
										{
											CheckHiddenSingleAvoidable(collected, in grid, ref context, urCells, d1, d2, corner1, corner2, in cellsExcluding2CornerCells, index);
										}
									}
									else
									{
										CheckType6(collected, in grid, ref context, urCells, comparer, d1, d2, corner1, corner2, in cellsExcluding2CornerCells, index);
										if (SearchForExtendedUniqueRectangles)
										{
											Check2D1SL(collected, in grid, ref context, urCells, false, comparer, d1, d2, corner1, corner2, in cellsExcluding2CornerCells, index);
										}
									}
									break;
								}

								// Non-diagonal type.
								default:
								{
									CheckType3(collected, in grid, ref context, urCells, arMode, comparer, d1, d2, corner1, corner2, in cellsExcluding2CornerCells, index);
									if (!arMode)
									{
										CheckType4(collected, in grid, ref context, urCells, comparer, d1, d2, corner1, corner2, in cellsExcluding2CornerCells, index);
										if (SearchForExtendedUniqueRectangles)
										{
											Check2B1SL(collected, in grid, ref context, urCells, false, comparer, d1, d2, corner1, corner2, in cellsExcluding2CornerCells, index);
											Check4X3SL(collected, in grid, ref context, urCells, false, comparer, d1, d2, corner1, corner2, in cellsExcluding2CornerCells, index);
											Check4C3SL(collected, in grid, ref context, urCells, false, comparer, d1, d2, corner1, corner2, in cellsExcluding2CornerCells, index);
											CheckBurredSubset(collected, in grid, ref context, urCells, false, comparer, d1, d2, corner1, corner2, in cellsExcluding2CornerCells, index);
										}
									}

									if (SearchForExtendedUniqueRectangles)
									{
										CheckSueDeCoq(collected, in grid, ref context, urCells, arMode, comparer, d1, d2, corner1, corner2, in cellsExcluding2CornerCells, index);
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
	private static bool CheckPreconditions(ref readonly Grid grid, Cell[] urCells, bool arMode)
	{
		var (emptyCountWhenArMode, modifiableCount) = ((byte)0, (byte)0);
		foreach (var urCell in urCells)
		{
			switch (grid.GetState(urCell))
			{
				case CellState.Given:
				case CellState.Modifiable when !arMode:
				{
					return false;
				}
				case CellState.Empty when arMode:
				{
					emptyCountWhenArMode++;
					break;
				}
				case CellState.Modifiable:
				{
					modifiableCount++;
					break;
				}
			}
		}

		return modifiableCount != 4 && emptyCountWhenArMode != 4;
	}

#pragma warning disable IDE0051
	/// <summary>
	/// Checks whether the specified UR cells satisfies the precondition of an incomplete UR.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">The UR cells.</param>
	/// <param name="d1">The first digit used.</param>
	/// <param name="d2">The second digit used.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	private static bool CheckPreconditionsOnIncomplete(ref readonly Grid grid, Cell[] urCells, Digit d1, Digit d2)
	{
		// Same-sided cells cannot contain only one digit of two digits 'd1' and 'd2'.
		foreach (var (a, b) in ((0, 1), (2, 3), (0, 2), (1, 3)))
		{
			var collectedMask = (Mask)(grid.GetCandidates(urCells[a]) | grid.GetCandidates(urCells[b]));
			if ((collectedMask >> d1 & 1) == 0 || (collectedMask >> d2 & 1) == 0)
			{
				return false;
			}
		}

		// All four cells must contain at least one digit appeared in the UR.
		var comparer = (Mask)(1 << d1 | 1 << d2);
		foreach (var cell in urCells)
		{
			if ((grid.GetCandidates(cell) & comparer) == 0)
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// To determine whether the specified house forms a conjugate pair
	/// of the specified digit, and the cells where they contain the digit
	/// is same as the given map contains.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <param name="map">The map.</param>
	/// <param name="houseIndex">The house index.</param>
	/// <returns>A <see cref="bool"/> value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsConjugatePair(Digit digit, ref readonly CellMap map, House houseIndex)
		=> (HousesMap[houseIndex] & CandidatesMap[digit]) == map;

	/// <summary>
	/// Get whether two cells are in a same house.
	/// </summary>
	/// <param name="cell1">The cell 1 to check.</param>
	/// <param name="cell2">The cell 2 to check.</param>
	/// <param name="houses">
	/// The result houses that both two cells lie in. If the cell can't be found, this argument will be 0.
	/// </param>
	/// <returns>
	/// The <see cref="bool"/> value indicating whether the another cell is same house as the current one.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsSameHouseCell(Cell cell1, Cell cell2, out HouseMask houses)
	{
		var v = (cell1.AsCellMap() + cell2).SharedHouses;
		(var r, houses) = v != 0 ? (true, v) : (false, 0);
		return r;
	}

	/// <summary>
	/// Check whether the highlight UR candidates is incomplete.
	/// </summary>
	/// <param name="arMode">Indicates whether the current searching mode is for ARs.</param>
	/// <param name="allowIncomplete"><inheritdoc cref="AllowIncompleteUniqueRectangles" path="/summary"/></param>
	/// <param name="list">The list to check.</param>
	/// <param name="isIncomplete">Indicates whether the pattern is incomplete.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	/// <remarks>
	/// This method uses a trick to check a UR pattern: to count up the number of "Normal colored"
	/// candidates used in the current UR pattern. If and only if the full pattern uses 8 candidates
	/// colored with normal one, the pattern will be complete.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsIncompleteValid(bool arMode, bool allowIncomplete, List<CandidateViewNode> list, out bool isIncomplete)
	{
		isIncomplete = !allowIncomplete && list.Count(nodeChecker) != 8 || allowIncomplete;
		return !arMode && isIncomplete || arMode;


		static bool nodeChecker(CandidateViewNode d)
			=> d.Identifier is WellKnownColorIdentifier { Kind: WellKnownColorIdentifierKind.Normal };
	}

	/// <summary>
	/// Get a cell that can't see each other.
	/// </summary>
	/// <param name="urCells">The UR cells.</param>
	/// <param name="cell">The current cell.</param>
	/// <returns>The diagonal cell.</returns>
	/// <exception cref="ArgumentException">Throws when the specified argument <paramref name="cell"/> is invalid.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Cell GetDiagonalCell(Cell[] urCells, Cell cell)
		=> cell == urCells[0] ? urCells[3] : cell == urCells[1] ? urCells[2] : cell == urCells[2] ? urCells[1] : urCells[0];

	/// <summary>
	/// Get all highlight cells.
	/// </summary>
	/// <param name="urCells">The all UR cells used.</param>
	/// <returns>The list of highlight cells.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static CellViewNode[] GetHighlightCells(Cell[] urCells)
		=> from urCell in urCells select new CellViewNode(ColorIdentifier.Normal, urCell);
#pragma warning restore IDE0051


	//
	// Basic Types
	//
	private partial void CheckType1(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell cornerCell, ref readonly CellMap otherCellsMap, int index);
	private partial void CheckType2(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, ref readonly CellMap otherCellsMap, int index);
	private partial void CheckType3(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, ref readonly CellMap otherCellsMap, int index);
	private partial void CheckType4(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, ref readonly CellMap otherCellsMap, int index);
	private partial void CheckType5(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell cornerCell, ref readonly CellMap otherCellsMap, int index);
	private partial void CheckType6(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, ref readonly CellMap otherCellsMap, int index);
	private partial void CheckHidden(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell cornerCell, ref readonly CellMap otherCellsMap, int index);

	//
	// Strong Link Types
	//
	private partial void Check2B1SL(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, ref readonly CellMap otherCellsMap, int index);
	private partial void Check2D1SL(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, ref readonly CellMap otherCellsMap, int index);
	private partial void Check3X2SL(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell cornerCell, ref readonly CellMap otherCellsMap, int index);
	private partial void Check3N2SL(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell cornerCell, ref readonly CellMap otherCellsMap, int index);
	private partial void Check3U2SL(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell cornerCell, ref readonly CellMap otherCellsMap, int index);
	private partial void Check3E2SL(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell cornerCell, ref readonly CellMap otherCellsMap, int index);
	private partial void Check4X3SL(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, ref readonly CellMap otherCellsMap, int index);
	private partial void Check4C3SL(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, ref readonly CellMap otherCellsMap, int index);

	//
	// Pattern-Based Types
	//
	private partial void CheckBurredSubset(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, ref readonly CellMap otherCellsMap, int index);
	private partial void CheckRegularWing(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, ref readonly CellMap otherCellsMap, int index, bool areCornerCellsAligned);
	private partial void CheckWWing(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, ref readonly CellMap otherCellsMap, int index);
	private partial void CheckSueDeCoq(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, ref readonly CellMap otherCellsMap, int index);
	private partial void CheckAlmostLockedSetsXz(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, scoped ReadOnlySpan<AlmostLockedSet> alses, int index);
	private partial void CheckBabaGroupingUnique(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, Mask comparer, Digit d1, Digit d2, int index);
	private partial void CheckHiddenSingleAvoidable(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, Digit d1, Digit d2, Cell corner1, Cell corner2, ref readonly CellMap otherCellsMap, int index);

	//
	// External Types
	//
	private partial void CheckExternalType1Or2(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, Digit d1, Digit d2, int index, bool arMode);
	private partial void CheckExternalType3(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, Mask comparer, Digit d1, Digit d2, int index, bool arMode);
	private partial void CheckExternalType4(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, Mask comparer, Digit d1, Digit d2, int index, bool arMode);
	private partial void CheckExternalTurbotFish(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, Mask comparer, Digit d1, Digit d2, int index);
	private partial void CheckExternalWWing(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, Mask comparer, Digit d1, Digit d2, int index);
	private partial void CheckExternalXyWing(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, Mask comparer, Digit d1, Digit d2, int index, bool arMode);
	private partial void CheckExternalAlmostLockedSetsXz(List<UniqueRectangleStep> accumulator, ref readonly Grid grid, ref StepAnalysisContext context, Cell[] urCells, scoped ReadOnlySpan<AlmostLockedSet> alses, Mask comparer, Digit d1, Digit d2, int index, bool arMode);
}
