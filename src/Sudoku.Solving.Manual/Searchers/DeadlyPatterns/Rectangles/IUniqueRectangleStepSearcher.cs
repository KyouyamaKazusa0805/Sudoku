namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Unique Rectangle</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Basic types:
/// <list type="bullet">
/// <item>Unique Rectangle Type 1, 2, 3, 4, 5, 6</item>
/// <item>Avoidable Rectangle Type 1, 2, 3, 5</item>
/// <item>Hidden Unique Rectangle</item>
/// <item>Hidden Avoidable Rectangle</item>
/// </list>
/// </item>
/// <item>
/// Structured types:
/// <list type="bullet">
/// <item>Unique Rectangle + Conjugate Pair (also called "Unique Rectangle + Strong Link")</item>
/// <item>Avoidable Rectangle + Hidden Single</item>
/// <item>Unique Rectangle + Unknown Covering</item>
/// <item>Unique Rectangle + Sue de Coq</item>
/// <item>
/// Unique Rectangle + Guardian (This program call it "Unique Rectangle External Types"):
/// <list type="bullet">
/// <item>Unique Rectangle External Type 1</item>
/// <item>Unique Rectangle External Type 2</item>
/// <item>Unique Rectangle External Type 3</item>
/// <item>Unique Rectangle External Type 4</item>
/// <!--<item>Unique Rectangle External Type with XY-Wing</item>-->
/// </list>
/// </item>
/// </list>
/// </item>
/// <item>
/// Miscellaneous types:
/// <list type="bullet">
/// <item>Unique Rectangle 2D</item>
/// <item>Unique Rectangle 3X</item>
/// </list>
/// </item>
/// </list>
/// </summary>
public interface IUniqueRectangleStepSearcher : IDeadlyPatternStepSearcher
{
	/// <summary>
	/// Indicates whether the UR can be incomplete. In other words,
	/// some of UR candidates can be removed before the pattern forms.
	/// </summary>
	/// <remarks>
	/// For example, the complete pattern is:
	/// <code><![CDATA[
	/// ab  |  ab
	/// ab  |  ab
	/// ]]></code>
	/// This is a complete pattern, and we may remove an <c>ab</c> in a certain corner.
	/// The incomplete pattern may not contain all four <c>ab</c>s in the structure.
	/// </remarks>
	public abstract bool AllowIncompleteUniqueRectangles { get; set; }

	/// <summary>
	/// Indicates whether the searcher can search for extended URs.
	/// </summary>
	/// <remarks>
	/// The basic types are type 1 to type 6, all other types are extended ones.
	/// </remarks>
	public abstract bool SearchForExtendedUniqueRectangles { get; set; }


	/// <summary>
	/// Check preconditions.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is searching for ARs.</param>
	/// <returns>Indicates whether the UR is passed to check.</returns>
	protected internal static sealed bool CheckPreconditions(scoped in Grid grid, int[] urCells, bool arMode)
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
	/// Checks whether the specified UR cells satisfies the precondition of an incomplete UR.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">The UR cells.</param>
	/// <param name="d1">The first digit used.</param>
	/// <param name="d2">The second digit used.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	protected internal static sealed bool CheckPreconditionsOnIncomplete(
		scoped in Grid grid, int[] urCells, int d1, int d2)
	{
		// Same-sided cells cannot contain only one digit of two digits 'd1' and 'd2'.
		foreach (var (a, b) in stackalloc[] { (0, 1), (2, 3), (0, 2), (1, 3) })
		{
			short gatheredMask = (short)(grid.GetCandidates(urCells[a]) | grid.GetCandidates(urCells[b]));
			if ((gatheredMask >> d1 & 1) == 0 || (gatheredMask >> d2 & 1) == 0)
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
	protected internal static sealed bool IsConjugatePair(int digit, scoped in CellMap map, int houseIndex)
		=> (HousesMap[houseIndex] & CandidatesMap[digit]) == map;

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
	protected internal static sealed int GetDiagonalCell(int[] urCells, int cell)
		=> cell == urCells[0]
			? urCells[3]
			: cell == urCells[1]
				? urCells[2]
				: cell == urCells[2]
					? urCells[1]
					: urCells[0];

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
	protected internal static sealed bool IsSameHouseCell(int cell1, int cell2, out int houses)
	{
		int v = (CellMap.Empty + cell1 + cell2).CoveredHouses;
		(bool r, houses) = v != 0 ? (true, v) : (false, 0);
		return r;
	}

	/// <summary>
	/// Get all highlight cells.
	/// </summary>
	/// <param name="urCells">The all UR cells used.</param>
	/// <returns>The list of highlight cells.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected internal static sealed IEnumerable<CellViewNode> GetHighlightCells(int[] urCells)
		=> new CellViewNode[]
		{
			new(DisplayColorKind.Normal, urCells[0]),
			new(DisplayColorKind.Normal, urCells[1]),
			new(DisplayColorKind.Normal, urCells[2]),
			new(DisplayColorKind.Normal, urCells[3])
		};
}

[StepSearcher]
[SukakuNotSupported]
internal sealed unsafe partial class UniqueRectangleStepSearcher : IUniqueRectangleStepSearcher
{
	/// <inheritdoc/>
	[StepSearcherProperty]
	public bool AllowIncompleteUniqueRectangles { get; set; }

	/// <inheritdoc/>
	[StepSearcherProperty]
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
						var otherCellsMap = (CellMap)urCells - corner1;

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

	partial void CheckType1(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, scoped in CellMap otherCellsMap, int index);
	partial void CheckType2(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, scoped in CellMap otherCellsMap, int index);
	partial void CheckType3(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, scoped in CellMap otherCellsMap, int index);
	partial void CheckType4(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, scoped in CellMap otherCellsMap, int index);
	partial void CheckType5(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, scoped in CellMap otherCellsMap, int index);
	partial void CheckType6(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, scoped in CellMap otherCellsMap, int index);
	partial void CheckHidden(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, scoped in CellMap otherCellsMap, int index);
	partial void Check2D(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, scoped in CellMap otherCellsMap, int index);
	partial void Check2B1SL(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, scoped in CellMap otherCellsMap, int index);
	partial void Check2D1SL(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, scoped in CellMap otherCellsMap, int index);
	partial void Check3X(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, scoped in CellMap otherCellsMap, int index);
	partial void Check3X2SL(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, scoped in CellMap otherCellsMap, int index);
	partial void Check3N2SL(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, scoped in CellMap otherCellsMap, int index);
	partial void Check3U2SL(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, scoped in CellMap otherCellsMap, int index);
	partial void Check3E2SL(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, scoped in CellMap otherCellsMap, int index);
	partial void Check4X3SL(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, scoped in CellMap otherCellsMap, int index);
	partial void Check4C3SL(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, scoped in CellMap otherCellsMap, int index);
	partial void CheckRegularWing(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, scoped in CellMap otherCellsMap, int size, int index);
	partial void CheckSueDeCoq(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, scoped in CellMap otherCellsMap, int index);
	partial void CheckUnknownCoveringUnique(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, short comparer, int d1, int d2, int index);
	partial void CheckExternalType1Or2(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, int d1, int d2, int index);
	partial void CheckExternalType3(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, short comparer, int d1, int d2, int index);
	partial void CheckExternalType4(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, short comparer, int d1, int d2, int index);
	partial void CheckHiddenSingleAvoidable(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells, int d1, int d2, int corner1, int corner2, scoped in CellMap otherCellsMap, int index);
}

unsafe partial class UniqueRectangleStepSearcher
{
	/// <summary>
	/// Check type 1.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///   ↓ cornerCell
	/// (abc) ab
	///  ab   ab
	/// ]]></code>
	/// </remarks>
	partial void CheckType1(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell,
		scoped in CellMap otherCellsMap, int index)
	{
		// Get the summary mask.
		short mask = grid.GetDigitsUnion(otherCellsMap);
		if (mask != comparer)
		{
			return;
		}

		// Type 1 found. Now check elimination.
		bool d1Exists = CandidatesMap[d1].Contains(cornerCell);
		bool d2Exists = CandidatesMap[d2].Contains(cornerCell);
		if (!d1Exists && d2Exists)
		{
			return;
		}

		var conclusions = new List<Conclusion>(2);
		if (d1Exists)
		{
			conclusions.Add(new(Elimination, cornerCell, d1));
		}
		if (d2Exists)
		{
			conclusions.Add(new(Elimination, cornerCell, d2));
		}
		if (conclusions.Count == 0)
		{
			return;
		}

		var candidateOffsets = new List<CandidateViewNode>(6);
		foreach (int cell in otherCellsMap)
		{
			foreach (int digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
			}
		}

		if (!AllowIncompleteUniqueRectangles && (candidateOffsets.Count, conclusions.Count) != (6, 2))
		{
			return;
		}

		accumulator.Add(
			new UniqueRectangleType1Step(
				ImmutableArray.CreateRange(conclusions),
				ImmutableArray.Create(
					View.Empty
						| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
						| (arMode ? null : candidateOffsets)
				),
				d1,
				d2,
				(CellMap)urCells,
				arMode,
				index
			)
		);
	}

	/// <summary>
	/// Check type 2.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///   ↓ corner1, corner2
	/// (abc) (abc)
	///  ab    ab
	/// ]]></code>
	/// </remarks>
	partial void CheckType2(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2,
		scoped in CellMap otherCellsMap, int index)
	{
		// Get the summary mask.
		short mask = grid.GetDigitsUnion(otherCellsMap);
		if (mask != comparer)
		{
			return;
		}

		// Gets the extra mask.
		// If the mask is the power of 2, the type 2 will be formed.
		int extraMask = (grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) ^ comparer;
		if (extraMask == 0 || (extraMask & extraMask - 1) != 0)
		{
			return;
		}

		// Type 2 or 5 found. Now check elimination.
		int extraDigit = TrailingZeroCount(extraMask);
		if ((+(CellMap.Empty + corner1 + corner2) & CandidatesMap[extraDigit]) is not (var elimMap and not []))
		{
			return;
		}

		var candidateOffsets = new List<CandidateViewNode>();
		foreach (int cell in urCells)
		{
			if (grid.GetStatus(cell) == CellStatus.Empty)
			{
				foreach (int digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add(
						new(
							digit == extraDigit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
							cell * 9 + digit
						)
					);
				}
			}
		}

		if (IsIncomplete(candidateOffsets))
		{
			return;
		}

		bool isType5 = !(CellMap.Empty + corner1 + corner2).InOneHouse;
		accumulator.Add(
			new UniqueRectangleType2Step(
				from cell in elimMap select new Conclusion(Elimination, cell, extraDigit),
				ImmutableArray.Create(
					View.Empty
						| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
						| candidateOffsets
				),
				d1,
				d2,
				(arMode, isType5) switch
				{
					(true, true) => Technique.AvoidableRectangleType5,
					(true, false) => Technique.AvoidableRectangleType2,
					(false, true) => Technique.UniqueRectangleType5,
					(false, false) => Technique.UniqueRectangleType2
				},
				(CellMap)urCells,
				arMode,
				extraDigit,
				index
			)
		);
	}

	/// <summary>
	/// Check type 3.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">
	/// The map of other cells during the current UR searching.
	/// </param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///  ↓ corner1, corner2
	/// (ab ) (ab )
	///  abx   aby
	/// ]]></code>
	/// </remarks>
	partial void CheckType3(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2,
		scoped in CellMap otherCellsMap, int index)
	{
		bool notSatisfiedType3 = false;
		foreach (int cell in otherCellsMap)
		{
			short currentMask = grid.GetCandidates(cell);
			if ((currentMask & comparer) == 0
				|| currentMask == comparer || arMode && grid.GetStatus(cell) != CellStatus.Empty)
			{
				notSatisfiedType3 = true;
				break;
			}
		}
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer || notSatisfiedType3)
		{
			return;
		}

		short mask = grid.GetDigitsUnion(otherCellsMap);
		if ((mask & comparer) != comparer)
		{
			return;
		}

		short otherDigitsMask = (short)(mask ^ comparer);
		foreach (int houseIndex in otherCellsMap.CoveredHouses)
		{
			if (((ValuesMap[d1] | ValuesMap[d2]) & HousesMap[houseIndex]) is not [])
			{
				return;
			}

			var iterationMap = (HousesMap[houseIndex] & EmptyCells) - otherCellsMap;
			for (int size = PopCount((uint)otherDigitsMask) - 1, count = iterationMap.Count; size < count; size++)
			{
				foreach (var iteratedCells in iterationMap & size)
				{
					short tempMask = grid.GetDigitsUnion(iteratedCells);
					if ((tempMask & comparer) != 0 || PopCount((uint)tempMask) - 1 != size
						|| (tempMask & otherDigitsMask) != otherDigitsMask)
					{
						continue;
					}

					var conclusions = new List<Conclusion>(16);
					foreach (int digit in tempMask)
					{
						foreach (int cell in (iterationMap - iteratedCells) & CandidatesMap[digit])
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var cellOffsets = new List<CellViewNode>();
					foreach (int cell in urCells)
					{
						if (grid.GetStatus(cell) != CellStatus.Empty)
						{
							cellOffsets.Add(new(DisplayColorKind.Normal, cell));
						}
					}

					var candidateOffsets = new List<CandidateViewNode>();
					foreach (int cell in urCells)
					{
						if (grid.GetStatus(cell) == CellStatus.Empty)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(
									new(
										(tempMask >> digit & 1) != 0
											? DisplayColorKind.Auxiliary1
											: DisplayColorKind.Normal,
										cell * 9 + digit
									)
								);
							}
						}
					}
					foreach (int cell in iteratedCells)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
						}
					}

					accumulator.Add(
						new UniqueRectangleType3Step(
							ImmutableArray.CreateRange(conclusions),
							ImmutableArray.Create(
								View.Empty
									| (arMode ? cellOffsets : null)
									| candidateOffsets
									| new HouseViewNode(DisplayColorKind.Normal, houseIndex)
							),
							d1,
							d2,
							(CellMap)urCells,
							iteratedCells,
							otherDigitsMask,
							houseIndex,
							arMode,
							index
						)
					);
				}
			}
		}
	}

	/// <summary>
	/// Check type 4.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">
	/// The map of other cells during the current UR searching.
	/// </param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///  ↓ corner1, corner2
	/// (ab ) ab
	///  abx  aby
	/// ]]></code>
	/// </remarks>
	partial void CheckType4(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2,
		scoped in CellMap otherCellsMap, int index)
	{
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		int* p = stackalloc[] { d1, d2 };
		foreach (int houseIndex in otherCellsMap.CoveredHouses)
		{
			if (houseIndex < 9)
			{
				// Process the case in lines.
				continue;
			}

			for (int digitIndex = 0; digitIndex < 2; digitIndex++)
			{
				int digit = p[digitIndex];
				if (!IUniqueRectangleStepSearcher.IsConjugatePair(digit, otherCellsMap, houseIndex))
				{
					continue;
				}

				// Yes, Type 4 found.
				// Now check elimination.
				int elimDigit = TrailingZeroCount(comparer ^ (1 << digit));
				if ((otherCellsMap & CandidatesMap[elimDigit]) is not (var elimMap and not []))
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>(6);
				foreach (int cell in urCells)
				{
					if (grid.GetStatus(cell) != CellStatus.Empty)
					{
						continue;
					}

					if (otherCellsMap.Contains(cell))
					{
						if (d1 != elimDigit && CandidatesMap[d1].Contains(cell))
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + d1));
						}
						if (d2 != elimDigit && CandidatesMap[d2].Contains(cell))
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + d2));
						}
					}
					else
					{
						// Corner1 and corner2.
						foreach (int d in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d));
						}
					}
				}

				var conclusions = from cell in elimMap select new Conclusion(Elimination, cell, elimDigit);
				if (!AllowIncompleteUniqueRectangles && (candidateOffsets.Count, conclusions.Length) != (6, 2))
				{
					continue;
				}

				int[] offsets = otherCellsMap.ToArray();
				accumulator.Add(
					new UniqueRectangleWithConjugatePairStep(
						conclusions,
						ImmutableArray.Create(
							View.Empty
								| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
								| candidateOffsets
								| new HouseViewNode(DisplayColorKind.Normal, houseIndex)
						),
						Technique.UniqueRectangleType4,
						d1,
						d2,
						(CellMap)urCells,
						arMode,
						new Conjugate[] { new(offsets[0], offsets[1], digit) },
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check type 5.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///  ↓ cornerCell
	/// (ab ) abc
	///  abc  abc
	/// ]]></code>
	/// </remarks>
	partial void CheckType5(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell,
		scoped in CellMap otherCellsMap, int index)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		// Get the summary mask.
		short otherCellsMask = grid.GetDigitsUnion(otherCellsMap);

		// Degenerate to type 1.
		short extraMask = (short)(otherCellsMask ^ comparer);
		if ((extraMask & extraMask - 1) != 0)
		{
			return;
		}

		// Type 5 found. Now check elimination.
		int extraDigit = TrailingZeroCount(extraMask);
		var cellsThatContainsExtraDigit = otherCellsMap & CandidatesMap[extraDigit];

		// Degenerate to type 1.
		if (cellsThatContainsExtraDigit.Count == 1)
		{
			return;
		}

		if ((+cellsThatContainsExtraDigit & CandidatesMap[extraDigit]) is not (var elimMap and not []))
		{
			return;
		}

		var candidateOffsets = new List<CandidateViewNode>(16);
		foreach (int cell in urCells)
		{
			if (grid.GetStatus(cell) != CellStatus.Empty)
			{
				continue;
			}

			foreach (int digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add(
					new(
						digit == extraDigit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
						cell * 9 + digit
					)
				);
			}
		}
		if (IsIncomplete(candidateOffsets))
		{
			return;
		}

		accumulator.Add(
			new UniqueRectangleType2Step(
				from cell in elimMap select new Conclusion(Elimination, cell, extraDigit),
				ImmutableArray.Create(
					View.Empty
						| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
						| candidateOffsets
				),
				d1,
				d2,
				arMode ? Technique.AvoidableRectangleType5 : Technique.UniqueRectangleType5,
				(CellMap)urCells,
				arMode,
				extraDigit,
				index
			)
		);
	}

	/// <summary>
	/// Check type 6.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///  ↓ corner1
	/// (ab )  aby
	///  abx  (ab)
	///        ↑corner2
	/// ]]></code>
	/// </remarks>
	partial void CheckType6(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2,
		scoped in CellMap otherCellsMap, int index)
	{
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		int o1 = otherCellsMap[0], o2 = otherCellsMap[1];
		int r1 = corner1.ToHouseIndex(HouseType.Row);
		int c1 = corner1.ToHouseIndex(HouseType.Column);
		int r2 = corner2.ToHouseIndex(HouseType.Row);
		int c2 = corner2.ToHouseIndex(HouseType.Column);
		int* p = stackalloc[] { d1, d2 };
		var q = stackalloc[] { (r1, r2), (c1, c2) };
		for (int digitIndex = 0; digitIndex < 2; digitIndex++)
		{
			int digit = p[digitIndex];
			for (int housePairIndex = 0; housePairIndex < 2; housePairIndex++)
			{
				var (h1, h2) = q[housePairIndex];
				gather(grid, otherCellsMap, h1 is >= 9 and < 18, digit, h1, h2);
			}
		}


		void gather(scoped in Grid grid, scoped in CellMap otherCellsMap, bool isRow, int digit, int house1, int house2)
		{
			bool precheck = isRow
				&& IUniqueRectangleStepSearcher.IsConjugatePair(digit, CellMap.Empty + corner1 + o1, house1)
				&& IUniqueRectangleStepSearcher.IsConjugatePair(digit, CellMap.Empty + corner2 + o2, house2)
				|| !isRow
				&& IUniqueRectangleStepSearcher.IsConjugatePair(digit, CellMap.Empty + corner1 + o2, house1)
				&& IUniqueRectangleStepSearcher.IsConjugatePair(digit, CellMap.Empty + corner2 + o1, house2);
			if (!precheck)
			{
				return;
			}

			// Check eliminations.
			if ((otherCellsMap & CandidatesMap[digit]) is not (var elimMap and not []))
			{
				return;
			}

			var candidateOffsets = new List<CandidateViewNode>(6);
			foreach (int cell in urCells)
			{
				if (otherCellsMap.Contains(cell))
				{
					if (d1 != digit && CandidatesMap[d1].Contains(cell))
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d1));
					}
					if (d2 != digit && CandidatesMap[d2].Contains(cell))
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d2));
					}
				}
				else
				{
					foreach (int d in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(d == digit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, cell * 9 + d)
						);
					}
				}
			}

			var conclusions = from cell in elimMap select new Conclusion(Elimination, cell, digit);
			if (!AllowIncompleteUniqueRectangles && (candidateOffsets.Count, conclusions.Length) != (6, 2))
			{
				return;
			}

			accumulator.Add(
				new UniqueRectangleWithConjugatePairStep(
					conclusions,
					ImmutableArray.Create(
						View.Empty
							| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
							| candidateOffsets
							| new HouseViewNode[]
							{
								new(DisplayColorKind.Normal, house1),
								new(DisplayColorKind.Normal, house2)
							}
					),
					Technique.UniqueRectangleType6,
					d1,
					d2,
					(CellMap)urCells,
					false,
					new Conjugate[]
					{
						new(corner1, isRow ? o1 : o2, digit),
						new(corner2, isRow ? o2 : o1, digit)
					},
					index
				)
			);
		}
	}

	/// <summary>
	/// Check hidden UR.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///  ↓ cornerCell
	/// (ab ) abx
	///  aby  abz
	/// ]]></code>
	/// </remarks>
	partial void CheckHidden(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell,
		scoped in CellMap otherCellsMap, int index)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		int abzCell = IUniqueRectangleStepSearcher.GetDiagonalCell(urCells, cornerCell);
		var adjacentCellsMap = otherCellsMap - abzCell;
		int abxCell = adjacentCellsMap[0], abyCell = adjacentCellsMap[1];
		int r = abzCell.ToHouseIndex(HouseType.Row), c = abzCell.ToHouseIndex(HouseType.Column);
		int* p = stackalloc[] { d1, d2 };
		for (int digitIndex = 0; digitIndex < 2; digitIndex++)
		{
			int digit = p[digitIndex];
			var map1 = CellMap.Empty + abzCell + abxCell;
			var map2 = CellMap.Empty + abzCell + abyCell;
			if (map1.CoveredLine is not (var m1cl and not InvalidValidOfTrailingZeroCountMethodFallback)
				|| map2.CoveredLine is not (var m2cl and not InvalidValidOfTrailingZeroCountMethodFallback))
			{
				// There's no common covered line to display.
				continue;
			}

			if (!IUniqueRectangleStepSearcher.IsConjugatePair(digit, map1, m1cl) || !IUniqueRectangleStepSearcher.IsConjugatePair(digit, map2, m2cl))
			{
				continue;
			}

			// Hidden UR found. Now check eliminations.
			int elimDigit = TrailingZeroCount(comparer ^ (1 << digit));
			if (!CandidatesMap[elimDigit].Contains(abzCell))
			{
				continue;
			}

			var candidateOffsets = new List<CandidateViewNode>();
			foreach (int cell in urCells)
			{
				if (grid.GetStatus(cell) != CellStatus.Empty)
				{
					continue;
				}

				if (otherCellsMap.Contains(cell))
				{
					if ((cell != abzCell || d1 != elimDigit) && CandidatesMap[d1].Contains(cell))
					{
						candidateOffsets.Add(
							new(
								d1 != elimDigit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
								cell * 9 + d1
							)
						);
					}
					if ((cell != abzCell || d2 != elimDigit) && CandidatesMap[d2].Contains(cell))
					{
						candidateOffsets.Add(
							new(
								d2 != elimDigit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
								cell * 9 + d2
							)
						);
					}
				}
				else
				{
					foreach (int d in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d));
					}
				}
			}

			if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
			{
				continue;
			}

			accumulator.Add(
				new HiddenUniqueRectangleStep(
					ImmutableArray.Create(new Conclusion(Elimination, abzCell, elimDigit)),
					ImmutableArray.Create(
						View.Empty
							| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
							| candidateOffsets
							| new HouseViewNode[] { new(DisplayColorKind.Normal, r), new(DisplayColorKind.Normal, c) }
					),
					d1,
					d2,
					(CellMap)urCells,
					arMode,
					new Conjugate[] { new(abzCell, abxCell, digit), new(abzCell, abyCell, digit) },
					index
				)
			);
		}
	}

	/// <summary>
	/// Check UR + 2D.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///   ↓ corner1
	/// (ab )  abx
	///  aby  (ab )  xy  *
	///         ↑ corner2
	/// ]]></code>
	/// </remarks>
	partial void Check2D(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2,
		int corner1, int corner2, scoped in CellMap otherCellsMap, int index)
	{
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		short o1 = grid.GetCandidates(otherCellsMap[0]), o2 = grid.GetCandidates(otherCellsMap[1]);
		short o = (short)(o1 | o2);
		if (PopCount((uint)o) != 4 || PopCount((uint)o1) > 3 || PopCount((uint)o2) > 3
			|| (o1 & comparer) == 0 || (o2 & comparer) == 0
			|| (o & comparer) != comparer)
		{
			return;
		}

		short xyMask = (short)(o ^ comparer);
		int x = TrailingZeroCount(xyMask), y = xyMask.GetNextSet(x);
		var inter = +otherCellsMap - (CellMap)urCells;
		foreach (int possibleXyCell in inter)
		{
			if (grid.GetCandidates(possibleXyCell) != xyMask)
			{
				continue;
			}

			// 'xy' found.
			// Now check eliminations.
			var elimMap = inter & PeersMap[possibleXyCell];
			var conclusions = new List<Conclusion>(10);
			foreach (int cell in elimMap)
			{
				if (CandidatesMap[x].Contains(cell))
				{
					conclusions.Add(new(Elimination, cell, x));
				}
				if (CandidatesMap[y].Contains(cell))
				{
					conclusions.Add(new(Elimination, cell, y));
				}
			}
			if (conclusions.Count == 0)
			{
				continue;
			}

			var candidateOffsets = new List<CandidateViewNode>(10);
			foreach (int cell in urCells)
			{
				if (otherCellsMap.Contains(cell))
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								(comparer >> digit & 1) == 0 ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
								cell * 9 + digit
							)
						);
					}
				}
				else
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
					}
				}
			}
			foreach (int digit in xyMask)
			{
				candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, possibleXyCell * 9 + digit));
			}

			if (IsIncomplete(candidateOffsets))
			{
				return;
			}

			accumulator.Add(
				new UniqueRectangle2DOr3XStep(
					ImmutableArray.CreateRange(conclusions),
					ImmutableArray.Create(
						View.Empty
							| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
							| candidateOffsets
					),
					arMode ? Technique.AvoidableRectangle2D : Technique.UniqueRectangle2D,
					d1,
					d2,
					(CellMap)urCells,
					arMode,
					x,
					y,
					possibleXyCell,
					index
				)
			);
		}
	}

	/// <summary>
	/// Check UR + 2B/1SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///   ↓ corner1, corner2
	/// (ab )  (ab )
	///  |
	///  | a
	///  |
	///  abx    aby
	/// ]]></code>
	/// </remarks>
	partial void Check2B1SL(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2,
		scoped in CellMap otherCellsMap, int index)
	{
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		int* corners = stackalloc[] { corner1, corner2 };
		int* digits = stackalloc[] { d1, d2 };
		for (int cellIndex = 0; cellIndex < 2; cellIndex++)
		{
			int cell = corners[cellIndex];
			foreach (int otherCell in otherCellsMap)
			{
				if (!IUniqueRectangleStepSearcher.IsSameHouseCell(cell, otherCell, out int houses))
				{
					continue;
				}

				foreach (int house in houses)
				{
					if (house < 9)
					{
						continue;
					}

					for (int digitIndex = 0; digitIndex < 2; digitIndex++)
					{
						int digit = digits[digitIndex];
						if (!IUniqueRectangleStepSearcher.IsConjugatePair(digit, CellMap.Empty + cell + otherCell, house))
						{
							continue;
						}

						int elimCell = (otherCellsMap - otherCell)[0];
						if (!CandidatesMap[digit].Contains(otherCell))
						{
							continue;
						}

						int elimDigit = TrailingZeroCount(comparer ^ (1 << digit));
						var conclusions = new List<Conclusion>(4);
						if (CandidatesMap[elimDigit].Contains(elimCell))
						{
							conclusions.Add(new(Elimination, elimCell, elimDigit));
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<CandidateViewNode>(10);
						foreach (int urCell in urCells)
						{
							if (urCell == corner1 || urCell == corner2)
							{
								int coveredHouses = (CellMap.Empty + urCell + otherCell).CoveredHouses;
								if ((coveredHouses >> house & 1) != 0)
								{
									foreach (int d in grid.GetCandidates(urCell))
									{
										candidateOffsets.Add(
											new(
												d == digit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
												urCell * 9 + d
											)
										);
									}
								}
								else
								{
									foreach (int d in grid.GetCandidates(urCell))
									{
										candidateOffsets.Add(new(DisplayColorKind.Normal, urCell * 9 + d));
									}
								}
							}
							else if (urCell == otherCell || urCell == elimCell)
							{
								if (CandidatesMap[d1].Contains(urCell))
								{
									if (urCell != elimCell || d1 != elimDigit)
									{
										candidateOffsets.Add(
											new(
												urCell == elimCell
													? DisplayColorKind.Normal
													: d1 == digit
														? DisplayColorKind.Auxiliary1
														: DisplayColorKind.Normal,
												urCell * 9 + d1
											)
										);
									}
								}
								if (CandidatesMap[d2].Contains(urCell))
								{
									if (urCell != elimCell || d2 != elimDigit)
									{
										candidateOffsets.Add(
											new(
												urCell == elimCell
													? DisplayColorKind.Normal
													: d2 == digit
														? DisplayColorKind.Auxiliary1
														: DisplayColorKind.Normal,
												urCell * 9 + d2
											)
										);
									}
								}
							}
						}

						if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
						{
							continue;
						}

						accumulator.Add(
							new UniqueRectangleWithConjugatePairStep(
								ImmutableArray.CreateRange(conclusions),
								ImmutableArray.Create(
									View.Empty
										| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
										| candidateOffsets
										| new HouseViewNode(DisplayColorKind.Normal, house)
								),
								Technique.UniqueRectangle2B1,
								d1,
								d2,
								(CellMap)urCells,
								arMode,
								new Conjugate[] { new(cell, otherCell, digit) },
								index
							)
						);
					}
				}
			}
		}
	}

	/// <summary>
	/// Check UR + 2D/1SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///   ↓ corner1
	/// (ab )   aby
	///  |
	///  | a
	///  |
	///  abx   (ab )
	///          ↑ corner2
	/// ]]></code>
	/// </remarks>
	partial void Check2D1SL(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2,
		int corner1, int corner2, scoped in CellMap otherCellsMap, int index)
	{
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		int* corners = stackalloc[] { corner1, corner2 };
		int* digits = stackalloc[] { d1, d2 };
		for (int cellIndex = 0; cellIndex < 2; cellIndex++)
		{
			int cell = corners[cellIndex];
			foreach (int otherCell in otherCellsMap)
			{
				if (!IUniqueRectangleStepSearcher.IsSameHouseCell(cell, otherCell, out int houses))
				{
					continue;
				}

				foreach (int house in houses)
				{
					if (house < 9)
					{
						continue;
					}

					for (int digitIndex = 0; digitIndex < 2; digitIndex++)
					{
						int digit = digits[digitIndex];
						if (!IUniqueRectangleStepSearcher.IsConjugatePair(digit, CellMap.Empty + cell + otherCell, house))
						{
							continue;
						}

						int elimCell = (otherCellsMap - otherCell)[0];
						if (!CandidatesMap[digit].Contains(otherCell))
						{
							continue;
						}

						var conclusions = new List<Conclusion>(4);
						if (CandidatesMap[digit].Contains(elimCell))
						{
							conclusions.Add(new(Elimination, elimCell, digit));
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<CandidateViewNode>(10);
						foreach (int urCell in urCells)
						{
							if (urCell == corner1 || urCell == corner2)
							{
								bool flag = false;
								foreach (int r in (CellMap.Empty + urCell + otherCell).CoveredHouses)
								{
									if (r == house)
									{
										flag = true;
										break;
									}
								}

								if (flag)
								{
									foreach (int d in grid.GetCandidates(urCell))
									{
										candidateOffsets.Add(
											new(
												d == digit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
												urCell * 9 + d
											)
										);
									}
								}
								else
								{
									foreach (int d in grid.GetCandidates(urCell))
									{
										candidateOffsets.Add(new(DisplayColorKind.Normal, urCell * 9 + d));
									}
								}
							}
							else if (urCell == otherCell || urCell == elimCell)
							{
								if (CandidatesMap[d1].Contains(urCell) && (urCell != elimCell || d1 != digit))
								{
									candidateOffsets.Add(
										new(
											urCell == elimCell
												? DisplayColorKind.Normal
												: d1 == digit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
											urCell * 9 + d1
										)
									);
								}
								if (CandidatesMap[d2].Contains(urCell) && (urCell != elimCell || d2 != digit))
								{
									candidateOffsets.Add(
										new(
											urCell == elimCell
												? DisplayColorKind.Normal
												: d2 == digit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
											urCell * 9 + d2
										)
									);
								}
							}
						}

						if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
						{
							continue;
						}

						accumulator.Add(
							new UniqueRectangleWithConjugatePairStep(
								ImmutableArray.CreateRange(conclusions),
								ImmutableArray.Create(
									View.Empty
										| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
										| candidateOffsets
										| new HouseViewNode(DisplayColorKind.Normal, house)
								),
								Technique.UniqueRectangle2D1,
								d1,
								d2,
								(CellMap)urCells,
								arMode,
								new Conjugate[] { new(cell, otherCell, digit) },
								index
							)
						);
					}
				}
			}
		}
	}

	/// <summary>
	/// Check UR + 3X.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///  ↓ cornerCell
	/// (ab )  abx
	///  aby   abz   xy  *
	/// ]]></code>
	/// Note: <c>z</c> is <c>x</c> or <c>y</c>.
	/// </remarks>
	partial void Check3X(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2,
		int cornerCell, scoped in CellMap otherCellsMap, int index)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		int c1 = otherCellsMap[0], c2 = otherCellsMap[1], c3 = otherCellsMap[2];
		short m1 = grid.GetCandidates(c1), m2 = grid.GetCandidates(c2), m3 = grid.GetCandidates(c3);
		short mask = (short)((short)(m1 | m2) | m3);

		if (PopCount((uint)mask) != 4
			|| PopCount((uint)m1) > 3 || PopCount((uint)m2) > 3 || PopCount((uint)m3) > 3
			|| (m1 & comparer) == 0 || (m2 & comparer) == 0 || (m3 & comparer) == 0
			|| (mask & comparer) != comparer)
		{
			return;
		}

		short xyMask = (short)(mask ^ comparer);
		int x = TrailingZeroCount(xyMask), y = xyMask.GetNextSet(x);
		var inter = +otherCellsMap - (CellMap)urCells;
		foreach (int possibleXyCell in inter)
		{
			if (grid.GetCandidates(possibleXyCell) != xyMask)
			{
				continue;
			}

			// Possible XY cell found.
			// Now check eliminations.
			var conclusions = new List<Conclusion>(10);
			foreach (int cell in inter & PeersMap[possibleXyCell])
			{
				if (CandidatesMap[x].Contains(cell))
				{
					conclusions.Add(new(Elimination, cell, x));
				}
				if (CandidatesMap[y].Contains(cell))
				{
					conclusions.Add(new(Elimination, cell, y));
				}
			}
			if (conclusions.Count == 0)
			{
				continue;
			}

			var candidateOffsets = new List<CandidateViewNode>(10);
			foreach (int cell in urCells)
			{
				if (otherCellsMap.Contains(cell))
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								(comparer >> digit & 1) == 0 ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
								cell * 9 + digit
							)
						);
					}
				}
				else
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
					}
				}
			}
			foreach (int digit in xyMask)
			{
				candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, possibleXyCell * 9 + digit));
			}
			if (IsIncomplete(candidateOffsets))
			{
				return;
			}

			accumulator.Add(
				new UniqueRectangle2DOr3XStep(
					ImmutableArray.CreateRange(conclusions),
					ImmutableArray.Create(
						View.Empty
							| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
							| candidateOffsets
					),
					arMode ? Technique.AvoidableRectangle3X : Technique.UniqueRectangle3X,
					d1,
					d2,
					(CellMap)urCells,
					arMode,
					x,
					y,
					possibleXyCell,
					index
				)
			);
		}
	}

	/// <summary>
	/// Check UR + 3X/2SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///  ↓ cornerCell
	/// (ab )    abx
	///           |
	///           | b
	///       a   |
	///  aby-----abz
	/// ]]></code>
	/// </remarks>
	partial void Check3X2SL(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2,
		int cornerCell, scoped in CellMap otherCellsMap, int index)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		int abzCell = IUniqueRectangleStepSearcher.GetDiagonalCell(urCells, cornerCell);
		var adjacentCellsMap = otherCellsMap - abzCell;
		var pairs = stackalloc[] { (d1, d2), (d2, d1) };
		for (int pairIndex = 0; pairIndex < 2; pairIndex++)
		{
			var (a, b) = pairs[pairIndex];
			int abxCell = adjacentCellsMap[0], abyCell = adjacentCellsMap[1];
			var map1 = CellMap.Empty + abzCell + abxCell;
			var map2 = CellMap.Empty + abzCell + abyCell;
			if (!IUniqueRectangleStepSearcher.IsConjugatePair(b, map1, map1.CoveredLine)
				|| !IUniqueRectangleStepSearcher.IsConjugatePair(a, map2, map2.CoveredLine))
			{
				continue;
			}

			var conclusions = new List<Conclusion>(2);
			if (CandidatesMap[a].Contains(abxCell))
			{
				conclusions.Add(new(Elimination, abxCell, a));
			}
			if (CandidatesMap[b].Contains(abyCell))
			{
				conclusions.Add(new(Elimination, abyCell, b));
			}
			if (conclusions.Count == 0)
			{
				continue;
			}

			var candidateOffsets = new List<CandidateViewNode>(6);
			foreach (int digit in grid.GetCandidates(abxCell))
			{
				if ((digit == d1 || digit == d2) && digit != a)
				{
					candidateOffsets.Add(
						new(digit == b ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, abxCell * 9 + digit)
					);
				}
			}
			foreach (int digit in grid.GetCandidates(abyCell))
			{
				if ((digit == d1 || digit == d2) && digit != b)
				{
					candidateOffsets.Add(
						new(digit == a ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, abyCell * 9 + digit)
					);
				}
			}
			foreach (int digit in grid.GetCandidates(abzCell))
			{
				if (digit == a || digit == b)
				{
					candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, abzCell * 9 + digit));
				}
			}
			foreach (int digit in comparer)
			{
				candidateOffsets.Add(new(DisplayColorKind.Normal, cornerCell * 9 + digit));
			}
			if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 6)
			{
				continue;
			}

			accumulator.Add(
				new UniqueRectangleWithConjugatePairStep(
					ImmutableArray.CreateRange(conclusions),
					ImmutableArray.Create(
						View.Empty
							| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
							| candidateOffsets
							| new HouseViewNode[]
							{
								new(DisplayColorKind.Normal, map1.CoveredLine),
								new(DisplayColorKind.Auxiliary1, map2.CoveredLine)
							}
					),
					Technique.UniqueRectangle3X2,
					d1,
					d2,
					(CellMap)urCells,
					arMode,
					new Conjugate[] { new(abxCell, abzCell, b), new(abyCell, abzCell, a) },
					index
				)
			);
		}
	}

	/// <summary>
	/// Check UR + 3N/2SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///  ↓ cornerCell
	/// (ab )-----abx
	///        a   |
	///            | b
	///            |
	///  aby      abz
	/// ]]></code>
	/// </remarks>
	partial void Check3N2SL(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells,
		bool arMode, short comparer, int d1, int d2, int cornerCell,
		scoped in CellMap otherCellsMap, int index)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		// Step 1: Get the diagonal cell of 'cornerCell' and determine
		// the existence of strong link.
		int abzCell = IUniqueRectangleStepSearcher.GetDiagonalCell(urCells, cornerCell);
		var adjacentCellsMap = otherCellsMap - abzCell;
		int abxCell = adjacentCellsMap[0], abyCell = adjacentCellsMap[1];
		var cellPairs = stackalloc[] { (abxCell, abyCell), (abyCell, abxCell) };
		var digitPairs = stackalloc[] { (d1, d2), (d2, d1) };
		int* digits = stackalloc[] { d1, d2 };
		for (int cellPairIndex = 0; cellPairIndex < 2; cellPairIndex++)
		{
			var (begin, end) = cellPairs[cellPairIndex];
			var linkMap = CellMap.Empty + begin + abzCell;
			for (int digitPairIndex = 0; digitPairIndex < 2; digitPairIndex++)
			{
				var (a, b) = digitPairs[digitPairIndex];
				if (!IUniqueRectangleStepSearcher.IsConjugatePair(b, linkMap, linkMap.CoveredLine))
				{
					continue;
				}

				// Step 2: Get the link cell that is adjacent to 'cornerCell'
				// and check the strong link.
				var secondLinkMap = CellMap.Empty + cornerCell + begin;
				if (!IUniqueRectangleStepSearcher.IsConjugatePair(a, secondLinkMap, secondLinkMap.CoveredLine))
				{
					continue;
				}

				// Step 3: Check eliminations.
				if (!CandidatesMap[a].Contains(end))
				{
					continue;
				}

				// Step 4: Check highlight candidates.
				var candidateOffsets = new List<CandidateViewNode>(7);
				foreach (int d in comparer)
				{
					candidateOffsets.Add(
						new(d == a ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, cornerCell * 9 + d)
					);
				}
				for (int digitIndex = 0; digitIndex < 2; digitIndex++)
				{
					int d = digits[digitIndex];
					if (CandidatesMap[d].Contains(abzCell))
					{
						candidateOffsets.Add(
							new(d == b ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, abzCell * 9 + d)
						);
					}
				}
				foreach (int d in grid.GetCandidates(begin))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, begin * 9 + d));
					}
				}
				foreach (int d in grid.GetCandidates(end))
				{
					if ((d == d1 || d == d2) && d != a)
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, end * 9 + d));
					}
				}
				if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
				{
					continue;
				}

				var conjugatePairs = new Conjugate[] { new(cornerCell, begin, a), new(begin, abzCell, b) };
				accumulator.Add(
					new UniqueRectangleWithConjugatePairStep(
						ImmutableArray.Create(new Conclusion(Elimination, end, a)),
						ImmutableArray.Create(
							View.Empty
								| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
								| candidateOffsets
								| new HouseViewNode[]
								{
									new(DisplayColorKind.Normal, conjugatePairs[0].Line),
									new(DisplayColorKind.Auxiliary1, conjugatePairs[1].Line)
								}
						),
						Technique.UniqueRectangle3N2,
						d1,
						d2,
						(CellMap)urCells,
						arMode,
						conjugatePairs,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR + 3U/2SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///  ↓ cornerCell
	/// (ab )-----abx
	///        a
	///
	///        b
	///  aby -----abz
	/// ]]></code>
	/// </remarks>
	partial void Check3U2SL(
		ICollection<UniqueRectangleStep> accumulator,
		scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2,
		int cornerCell, scoped in CellMap otherCellsMap, int index)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		int abzCell = IUniqueRectangleStepSearcher.GetDiagonalCell(urCells, cornerCell);
		var adjacentCellsMap = otherCellsMap - abzCell;
		int abxCell = adjacentCellsMap[0], abyCell = adjacentCellsMap[1];
		var cellPairs = stackalloc[] { (abxCell, abyCell), (abyCell, abxCell) };
		var digitPairs = stackalloc[] { (d1, d2), (d2, d1) };
		for (int cellPairIndex = 0; cellPairIndex < 2; cellPairIndex++)
		{
			var (begin, end) = cellPairs[cellPairIndex];
			var linkMap = CellMap.Empty + begin + abzCell;
			for (int digitPairIndex = 0; digitPairIndex < 2; digitPairIndex++)
			{
				var (a, b) = digitPairs[digitPairIndex];
				if (!IUniqueRectangleStepSearcher.IsConjugatePair(b, linkMap, linkMap.CoveredLine))
				{
					continue;
				}

				var secondLinkMap = CellMap.Empty + cornerCell + end;
				if (!IUniqueRectangleStepSearcher.IsConjugatePair(a, secondLinkMap, secondLinkMap.CoveredLine))
				{
					continue;
				}

				if (!CandidatesMap[a].Contains(begin))
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>(7);
				foreach (int d in comparer)
				{
					candidateOffsets.Add(
						new(d == a ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, cornerCell * 9 + d)
					);
				}
				foreach (int d in grid.GetCandidates(begin))
				{
					if ((d == d1 || d == d2) && d != a)
					{
						candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, begin * 9 + d));
					}
				}
				foreach (int d in grid.GetCandidates(end))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(
							new(d == a ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, end * 9 + d)
						);
					}
				}
				foreach (int d in grid.GetCandidates(abzCell))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(
							new(d == b ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, abzCell * 9 + d)
						);
					}
				}
				if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
				{
					continue;
				}

				var conjugatePairs = new Conjugate[] { new(cornerCell, end, a), new(begin, abzCell, b) };
				accumulator.Add(
					new UniqueRectangleWithConjugatePairStep(
						ImmutableArray.Create(new Conclusion(Elimination, begin, a)),
						ImmutableArray.Create(
							View.Empty
								| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
								| candidateOffsets
								| new HouseViewNode[]
								{
									new(DisplayColorKind.Normal, conjugatePairs[0].Line),
									new(DisplayColorKind.Auxiliary1, conjugatePairs[1].Line)
								}
						),
						Technique.UniqueRectangle3U2,
						d1,
						d2,
						(CellMap)urCells,
						arMode,
						conjugatePairs,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR + 3E/2SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///   ↓ cornerCell
	/// (ab )-----abx
	///        a
	///
	///        a
	///  aby -----abz
	/// ]]></code>
	/// </remarks>
	partial void Check3E2SL(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell,
		scoped in CellMap otherCellsMap, int index)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		int abzCell = IUniqueRectangleStepSearcher.GetDiagonalCell(urCells, cornerCell);
		var adjacentCellsMap = otherCellsMap - abzCell;
		int abxCell = adjacentCellsMap[0], abyCell = adjacentCellsMap[1];
		var cellPairs = stackalloc[] { (abxCell, abyCell), (abyCell, abxCell) };
		var digitPairs = stackalloc[] { (d1, d2), (d2, d1) };
		for (int cellPairIndex = 0; cellPairIndex < 2; cellPairIndex++)
		{
			var (begin, end) = cellPairs[cellPairIndex];
			var linkMap = CellMap.Empty + begin + abzCell;
			for (int digitPairIndex = 0; digitPairIndex < 2; digitPairIndex++)
			{
				var (a, b) = digitPairs[digitPairIndex];
				if (!IUniqueRectangleStepSearcher.IsConjugatePair(a, linkMap, linkMap.CoveredLine))
				{
					continue;
				}

				var secondLinkMap = CellMap.Empty + cornerCell + end;
				if (!IUniqueRectangleStepSearcher.IsConjugatePair(a, secondLinkMap, secondLinkMap.CoveredLine))
				{
					continue;
				}

				if (!CandidatesMap[b].Contains(abzCell))
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>(7);
				foreach (int d in comparer)
				{
					candidateOffsets.Add(
						new(d == a ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, cornerCell * 9 + d)
					);
				}
				foreach (int d in grid.GetCandidates(begin))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(
							new(d == a ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, begin * 9 + d)
						);
					}
				}
				foreach (int d in grid.GetCandidates(end))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(
							new(d == a ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, end * 9 + d)
						);
					}
				}
				foreach (int d in grid.GetCandidates(abzCell))
				{
					if ((d == d1 || d == d2) && d != b)
					{
						candidateOffsets.Add(
							new(d == a ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, abzCell * 9 + d)
						);
					}
				}
				if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
				{
					continue;
				}

				var conjugatePairs = new Conjugate[] { new(cornerCell, end, a), new(begin, abzCell, a) };
				accumulator.Add(
					new UniqueRectangleWithConjugatePairStep(
						ImmutableArray.Create(new Conclusion(Elimination, abzCell, b)),
						ImmutableArray.Create(
							View.Empty
								| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
								| candidateOffsets
								| new HouseViewNode[]
								{
									new(DisplayColorKind.Normal, conjugatePairs[0].Line),
									new(DisplayColorKind.Auxiliary1, conjugatePairs[1].Line)
								}
						),
						Technique.UniqueRectangle3E2,
						d1,
						d2,
						(CellMap)urCells,
						arMode,
						conjugatePairs,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR + 4X/3SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///   ↓ corner1, corner2
	/// (abx)-----(aby)
	///        a    |
	///             | b
	///        a    |
	///  abz ----- abw
	/// ]]></code>
	/// </remarks>
	partial void Check4X3SL(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells,
		bool arMode, short comparer, int d1, int d2, int corner1, int corner2,
		scoped in CellMap otherCellsMap, int index)
	{
		var link1Map = CellMap.Empty + corner1 + corner2;
		var digitPairs = stackalloc[] { (d1, d2), (d2, d1) };
		for (int digitPairIndex = 0; digitPairIndex < 2; digitPairIndex++)
		{
			var (a, b) = digitPairs[digitPairIndex];
			if (!IUniqueRectangleStepSearcher.IsConjugatePair(a, link1Map, link1Map.CoveredLine))
			{
				continue;
			}

			int abwCell = IUniqueRectangleStepSearcher.GetDiagonalCell(urCells, corner1);
			int abzCell = (otherCellsMap - abwCell)[0];
			var cellQuadruples = stackalloc[]
			{
				(corner2, corner1, abzCell, abwCell),
				(corner1, corner2, abwCell, abzCell)
			};

			for (int cellQuadrupleIndex = 0; cellQuadrupleIndex < 2; cellQuadrupleIndex++)
			{
				var (head, begin, end, extra) = cellQuadruples[cellQuadrupleIndex];
				var link2Map = CellMap.Empty + begin + end;
				if (!IUniqueRectangleStepSearcher.IsConjugatePair(b, link2Map, link2Map.CoveredLine))
				{
					continue;
				}

				var link3Map = CellMap.Empty + end + extra;
				if (!IUniqueRectangleStepSearcher.IsConjugatePair(a, link3Map, link3Map.CoveredLine))
				{
					continue;
				}

				var conclusions = new List<Conclusion>(2);
				if (CandidatesMap[b].Contains(head))
				{
					conclusions.Add(new(Elimination, head, b));
				}
				if (CandidatesMap[b].Contains(extra))
				{
					conclusions.Add(new(Elimination, extra, b));
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>(6);
				foreach (int d in grid.GetCandidates(head))
				{
					if ((d == d1 || d == d2) && d != b)
					{
						candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, head * 9 + d));
					}
				}
				foreach (int d in grid.GetCandidates(extra))
				{
					if ((d == d1 || d == d2) && d != b)
					{
						candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, extra * 9 + d));
					}
				}
				foreach (int d in grid.GetCandidates(begin))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, begin * 9 + d));
					}
				}
				foreach (int d in grid.GetCandidates(end))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, end * 9 + d));
					}
				}
				if (!AllowIncompleteUniqueRectangles && (candidateOffsets.Count, conclusions.Count) != (6, 2))
				{
					continue;
				}

				var conjugatePairs = new Conjugate[]
				{
					new(head, begin, a),
					new(begin, end, b),
					new(end, extra, a)
				};
				accumulator.Add(
					new UniqueRectangleWithConjugatePairStep(
						ImmutableArray.CreateRange(conclusions),
						ImmutableArray.Create(
							View.Empty
								| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
								| candidateOffsets
								| new HouseViewNode[]
								{
									new(DisplayColorKind.Normal, conjugatePairs[0].Line),
									new(DisplayColorKind.Auxiliary1, conjugatePairs[1].Line),
									new(DisplayColorKind.Normal, conjugatePairs[2].Line)
								}
						),
						Technique.UniqueRectangle4X3,
						d1,
						d2,
						(CellMap)urCells,
						arMode,
						conjugatePairs,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR + 4C/3SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// <para>The structures:</para>
	/// <para>
	/// Subtype 1:
	/// <code><![CDATA[
	///   ↓ corner1, corner2
	/// (abx)-----(aby)
	///        a    |
	///             | a
	///        b    |
	///  abz ----- abw
	/// ]]></code>
	/// </para>
	/// <para>
	/// Subtype 2:
	/// <code><![CDATA[
	///   ↓ corner1, corner2
	/// (abx)-----(aby)
	///   |    a    |
	///   | b       | a
	///   |         |
	///  abz       abw
	/// ]]></code>
	/// </para>
	/// </remarks>
	partial void Check4C3SL(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells,
		bool arMode, short comparer, int d1, int d2, int corner1, int corner2,
		scoped in CellMap otherCellsMap, int index)
	{
		var link1Map = CellMap.Empty + corner1 + corner2;
		var innerMaps = stackalloc CellMap[2];
		var digitPairs = stackalloc[] { (d1, d2), (d2, d1) };
		for (int digitPairIndex = 0; digitPairIndex < 2; digitPairIndex++)
		{
			var (a, b) = digitPairs[digitPairIndex];
			if (!IUniqueRectangleStepSearcher.IsConjugatePair(a, link1Map, link1Map.CoveredLine))
			{
				continue;
			}

			int end = IUniqueRectangleStepSearcher.GetDiagonalCell(urCells, corner1);
			int extra = (otherCellsMap - end)[0];
			var cellQuadruples = stackalloc[] { (corner2, corner1, extra, end), (corner1, corner2, end, extra) };
			for (int cellQuadrupleIndex = 0; cellQuadrupleIndex < 2; cellQuadrupleIndex++)
			{
				var (abx, aby, abw, abz) = cellQuadruples[cellQuadrupleIndex];
				var link2Map = CellMap.Empty + aby + abw;
				if (!IUniqueRectangleStepSearcher.IsConjugatePair(a, link2Map, link2Map.CoveredLine))
				{
					continue;
				}

				var link3Map1 = CellMap.Empty + abw + abz;
				var link3Map2 = CellMap.Empty + abx + abz;
				innerMaps[0] = link3Map1;
				innerMaps[1] = link3Map2;
				for (int i = 0; i < 2; i++)
				{
					var linkMap = innerMaps[i];
					if (!IUniqueRectangleStepSearcher.IsConjugatePair(b, link3Map1, link3Map1.CoveredLine))
					{
						continue;
					}

					if (!CandidatesMap[b].Contains(aby))
					{
						continue;
					}

					var candidateOffsets = new List<CandidateViewNode>(7);
					foreach (int d in grid.GetCandidates(abx))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add(
								new(
									i == 0
										? d == a
											? DisplayColorKind.Auxiliary1
											: DisplayColorKind.Normal
										: DisplayColorKind.Auxiliary1,
									abx * 9 + d
								)
							);
						}
					}
					foreach (int d in grid.GetCandidates(abz))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add(
								new(d == b ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, abz * 9 + d)
							);
						}
					}
					foreach (int d in grid.GetCandidates(aby))
					{
						if ((d == d1 || d == d2) && d != b)
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, aby * 9 + d));
						}
					}
					foreach (int d in grid.GetCandidates(abw))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, abw * 9 + d));
						}
					}
					if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
					{
						continue;
					}

					int[] offsets = linkMap.ToArray();
					var conjugatePairs = new Conjugate[]
					{
						new(abx, aby, a),
						new(aby, abw, a),
						new(offsets[0], offsets[1], b)
					};
					accumulator.Add(
						new UniqueRectangleWithConjugatePairStep(
							ImmutableArray.Create(new Conclusion(Elimination, aby, b)),
							ImmutableArray.Create(
								View.Empty
									| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
									| candidateOffsets
									| new HouseViewNode[]
									{
										new(DisplayColorKind.Normal, conjugatePairs[0].Line),
										new(DisplayColorKind.Normal, conjugatePairs[1].Line),
										new(DisplayColorKind.Auxiliary1, conjugatePairs[2].Line)
									}
							),
							Technique.UniqueRectangle4C3,
							d1,
							d2,
							(CellMap)urCells,
							arMode,
							conjugatePairs,
							index
						)
					);
				}
			}
		}
	}

	/// <summary>
	/// Check UR-XY-Wing, UR-XYZ-Wing, UR-WXYZ-Wing and AR-XY-Wing, AR-XYZ-Wing and AR-WXYZ-Wing.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="size">The size of the wing to search.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// <para>The structures:</para>
	/// <para>
	/// Subtype 1:
	/// <code><![CDATA[
	///   ↓ corner1
	/// (ab )  abxy  yz  xz
	/// (ab )  abxy  *
	///   ↑ corner2
	/// ]]></code>
	/// Note that the pair of cells <c>abxy</c> should be in the same house.
	/// </para>
	/// <para>
	/// Subtype 2:
	/// <code><![CDATA[
	///   ↓ corner1
	/// (ab )  abx   xz
	///  aby  (ab )  *   yz
	///         ↑ corner2
	/// ]]></code>
	/// </para>
	/// </remarks>
	partial void CheckRegularWing(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells,
		bool arMode, short comparer, int d1, int d2, int corner1, int corner2,
		scoped in CellMap otherCellsMap, int size, int index)
	{
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		if ((CellMap.Empty + corner1 + corner2).AllSetsAreInOneHouse(out int house) && house < 9)
		{
			// Subtype 1.
			int[] offsets = otherCellsMap.ToArray();
			int otherCell1 = offsets[0], otherCell2 = offsets[1];
			short mask1 = grid.GetCandidates(otherCell1);
			short mask2 = grid.GetCandidates(otherCell2);
			short mask = (short)(mask1 | mask2);

			if (PopCount((uint)mask) != 2 + size || (mask & comparer) != comparer
				|| mask1 == comparer || mask2 == comparer)
			{
				return;
			}

			var map = (PeersMap[otherCell1] | PeersMap[otherCell2]) & BivalueCells;
			if (map.Count < size)
			{
				return;
			}

			var testMap = +(CellMap.Empty + otherCell1 + otherCell2);
			short extraDigitsMask = (short)(mask ^ comparer);
			int[] cells = map.ToArray();
			for (int i1 = 0, length = cells.Length, outerLength = length - size + 1; i1 < outerLength; i1++)
			{
				int c1 = cells[i1];
				short m1 = grid.GetCandidates(c1);
				if ((m1 & ~extraDigitsMask) == 0)
				{
					continue;
				}

				for (int i2 = i1 + 1, lengthMinusSizePlus2 = length - size + 2; i2 < lengthMinusSizePlus2; i2++)
				{
					int c2 = cells[i2];
					short m2 = grid.GetCandidates(c2);
					if ((m2 & ~extraDigitsMask) == 0)
					{
						continue;
					}

					if (size == 2)
					{
						// Check XY-Wing.
						short m = (short)((short)(m1 | m2) ^ extraDigitsMask);
						if ((PopCount((uint)m), PopCount((uint)(m1 & m2))) != (1, 1))
						{
							continue;
						}

						// Now check whether all cells found should see their corresponding
						// cells in UR structure ('otherCells1' or 'otherCells2').
						bool flag = true;
						int* combinationCells = stackalloc[] { c1, c2 };
						for (int cellIndex = 0; cellIndex < 2; cellIndex++)
						{
							int cell = combinationCells[cellIndex];
							int extraDigit = TrailingZeroCount(grid.GetCandidates(cell) & ~m);
							if (!(testMap & CandidatesMap[extraDigit]).Contains(cell))
							{
								flag = false;
								break;
							}
						}
						if (!flag)
						{
							continue;
						}

						// Now check eliminations.
						int elimDigit = TrailingZeroCount(m);
						if ((+(CellMap.Empty + c1 + c2) & CandidatesMap[elimDigit]) is not (var elimMap and not []))
						{
							continue;
						}

						var candidateOffsets = new List<CandidateViewNode>(12);
						foreach (int cell in urCells)
						{
							if (grid.GetStatus(cell) == CellStatus.Empty)
							{
								foreach (int digit in grid.GetCandidates(cell))
								{
									candidateOffsets.Add(
										new(
											digit == elimDigit
												? otherCellsMap.Contains(cell)
													? DisplayColorKind.Auxiliary2
													: DisplayColorKind.Normal
												: (extraDigitsMask >> digit & 1) != 0
													? DisplayColorKind.Auxiliary1
													: DisplayColorKind.Normal,
											cell * 9 + digit
										)
									);
								}
							}
						}
						foreach (int digit in grid.GetCandidates(c1))
						{
							candidateOffsets.Add(
								new(
									digit == elimDigit ? DisplayColorKind.Auxiliary2 : DisplayColorKind.Auxiliary1,
									c1 * 9 + digit
								)
							);
						}
						foreach (int digit in grid.GetCandidates(c2))
						{
							candidateOffsets.Add(
								new(
									digit == elimDigit ? DisplayColorKind.Auxiliary2 : DisplayColorKind.Auxiliary1,
									c2 * 9 + digit
								)
							);
						}
						if (IsIncomplete(candidateOffsets))
						{
							return;
						}

						accumulator.Add(
							new UniqueRectangleWithWingStep(
								from cell in elimMap select new Conclusion(Elimination, cell, elimDigit),
								ImmutableArray.Create(
									View.Empty
										| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
										| candidateOffsets
								),
								arMode
									? Technique.AvoidableRectangleXyWing
									: Technique.UniqueRectangleXyWing,
								d1,
								d2,
								(CellMap)urCells,
								arMode,
								CellMap.Empty + c1 + c2,
								otherCellsMap,
								extraDigitsMask,
								index
							)
						);
					}
					else // size > 2
					{
						for (
							int i3 = i2 + 1, lengthMinusSizePlus3 = length - size + 3;
							i3 < lengthMinusSizePlus3;
							i3++
						)
						{
							int c3 = cells[i3];
							short m3 = grid.GetCandidates(c3);
							if ((m3 & ~extraDigitsMask) == 0)
							{
								continue;
							}

							if (size == 3)
							{
								// Check XYZ-Wing.
								short m = (short)(((short)(m1 | m2) | m3) ^ extraDigitsMask);
								if ((PopCount((uint)m), PopCount((uint)(m1 & m2 & m3))) != (1, 1))
								{
									continue;
								}

								// Now check whether all cells found should see their corresponding
								// cells in UR structure ('otherCells1' or 'otherCells2').
								bool flag = true;
								int* combinationCells = stackalloc[] { c1, c2, c3 };
								for (int cellIndex = 0; cellIndex < 3; cellIndex++)
								{
									int cell = combinationCells[cellIndex];
									int extraDigit = TrailingZeroCount(grid.GetCandidates(cell) & ~m);
									if (!(testMap & CandidatesMap[extraDigit]).Contains(cell))
									{
										flag = false;
										break;
									}
								}
								if (!flag)
								{
									continue;
								}

								// Now check eliminations.
								int elimDigit = TrailingZeroCount(m);
								var elimMap = +(CellMap.Empty + c1 + c2 + c3) & CandidatesMap[elimDigit];
								if (elimMap is [])
								{
									continue;
								}

								var candidateOffsets = new List<CandidateViewNode>();
								foreach (int cell in urCells)
								{
									if (grid.GetStatus(cell) == CellStatus.Empty)
									{
										foreach (int digit in grid.GetCandidates(cell))
										{
											candidateOffsets.Add(
												new(
													(extraDigitsMask >> digit & 1) != 0
														? DisplayColorKind.Auxiliary1
														: DisplayColorKind.Normal,
													cell * 9 + digit
												)
											);
										}
									}
								}
								foreach (int digit in grid.GetCandidates(c1))
								{
									candidateOffsets.Add(
										new(
											digit == elimDigit
												? DisplayColorKind.Auxiliary2
												: DisplayColorKind.Auxiliary1,
											c1 * 9 + digit
										)
									);
								}
								foreach (int digit in grid.GetCandidates(c2))
								{
									candidateOffsets.Add(
										new(
											digit == elimDigit
												? DisplayColorKind.Auxiliary2
												: DisplayColorKind.Auxiliary1,
											c2 * 9 + digit
										)
									);
								}
								foreach (int digit in grid.GetCandidates(c3))
								{
									candidateOffsets.Add(
										new(
											digit == elimDigit
												? DisplayColorKind.Auxiliary2
												: DisplayColorKind.Auxiliary1,
											c3 * 9 + digit
										)
									);
								}
								if (IsIncomplete(candidateOffsets))
								{
									return;
								}

								accumulator.Add(
									new UniqueRectangleWithWingStep(
										from cell in elimMap select new Conclusion(Elimination, cell, elimDigit),
										ImmutableArray.Create(
											View.Empty
												| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
												| candidateOffsets
										),
										arMode
											? Technique.AvoidableRectangleXyzWing
											: Technique.UniqueRectangleXyzWing,
										d1,
										d2,
										(CellMap)urCells,
										arMode,
										CellMap.Empty + c1 + c2 + c3,
										otherCellsMap,
										extraDigitsMask,
										index
									)
								);
							}
							else // size == 4
							{
								for (int i4 = i3 + 1; i4 < length; i4++)
								{
									int c4 = cells[i4];
									short m4 = grid.GetCandidates(c4);
									if ((m4 & ~extraDigitsMask) == 0)
									{
										continue;
									}

									// Check WXYZ-Wing.
									short m = (short)((short)((short)((short)(m1 | m2) | m3) | m4) ^ extraDigitsMask);
									if ((PopCount((uint)m), PopCount((uint)(m1 & m2 & m3 & m4))) != (1, 1))
									{
										continue;
									}

									// Now check whether all cells found should see their corresponding
									// cells in UR structure ('otherCells1' or 'otherCells2').
									bool flag = true;
									int* combinationCells = stackalloc[] { c1, c2, c3, c4 };
									for (int cellIndex = 0; cellIndex < 4; cellIndex++)
									{
										int cell = combinationCells[cellIndex];
										int extraDigit = TrailingZeroCount(grid.GetCandidates(cell) & ~m);
										if (!(testMap & CandidatesMap[extraDigit]).Contains(cell))
										{
											flag = false;
											break;
										}
									}
									if (!flag)
									{
										continue;
									}

									// Now check eliminations.
									int elimDigit = TrailingZeroCount(m);
									var elimMap = +(CellMap.Empty + c1 + c2 + c3 + c4) & CandidatesMap[elimDigit];
									if (elimMap is [])
									{
										continue;
									}

									var candidateOffsets = new List<CandidateViewNode>();
									foreach (int cell in urCells)
									{
										if (grid.GetStatus(cell) == CellStatus.Empty)
										{
											foreach (int digit in grid.GetCandidates(cell))
											{
												candidateOffsets.Add(
													new(
														(extraDigitsMask >> digit & 1) != 0
															? DisplayColorKind.Auxiliary1
															: DisplayColorKind.Normal,
														cell * 9 + digit
													)
												);
											}
										}
									}
									foreach (int digit in grid.GetCandidates(c1))
									{
										candidateOffsets.Add(
											new(
												digit == elimDigit
													? DisplayColorKind.Auxiliary2
													: DisplayColorKind.Auxiliary1,
												c1 * 9 + digit
											)
										);
									}
									foreach (int digit in grid.GetCandidates(c2))
									{
										candidateOffsets.Add(
											new(
												digit == elimDigit
													? DisplayColorKind.Auxiliary2
													: DisplayColorKind.Auxiliary1,
												c2 * 9 + digit
											)
										);
									}
									foreach (int digit in grid.GetCandidates(c3))
									{
										candidateOffsets.Add(
											new(
												digit == elimDigit
													? DisplayColorKind.Auxiliary2
													: DisplayColorKind.Auxiliary1,
												c3 * 9 + digit
											)
										);
									}
									foreach (int digit in grid.GetCandidates(c4))
									{
										candidateOffsets.Add(
											new(
												digit == elimDigit
													? DisplayColorKind.Auxiliary2
													: DisplayColorKind.Auxiliary1,
												c4 * 9 + digit
											)
										);
									}
									if (IsIncomplete(candidateOffsets))
									{
										return;
									}

									accumulator.Add(
										new UniqueRectangleWithWingStep(
											from cell in elimMap
											select new Conclusion(Elimination, cell, elimDigit),
											ImmutableArray.Create(
												View.Empty
													| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
													| candidateOffsets
											),
											arMode
												? Technique.AvoidableRectangleWxyzWing
												: Technique.UniqueRectangleWxyzWing,
											d1,
											d2,
											(CellMap)urCells,
											arMode,
											CellMap.Empty + c1 + c2 + c3 + c4,
											otherCellsMap,
											extraDigitsMask,
											index
										)
									);
								}
							}
						}
					}
				}
			}
		}
		else
		{
			// TODO: Check for subtype 2 on UR/AR Wings.
		}
	}

	/// <summary>
	/// Check UR + SdC.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///           |   xyz
	///  ab+ ab+  | abxyz abxyz
	///           |   xyz
	/// ----------+------------
	/// (ab)(ab)  |
	///  ↑ corner1, corner2
	/// ]]></code>
	/// </remarks>
	partial void CheckSueDeCoq(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells,
		bool arMode, short comparer, int d1, int d2, int corner1, int corner2,
		scoped in CellMap otherCellsMap, int index)
	{
		bool notSatisfiedType3 = false;
		short mergedMaskInOtherCells = 0;
		foreach (int cell in otherCellsMap)
		{
			short currentMask = grid.GetCandidates(cell);
			mergedMaskInOtherCells |= currentMask;
			if ((currentMask & comparer) == 0
				|| currentMask == comparer || arMode && grid.GetStatus(cell) != CellStatus.Empty)
			{
				notSatisfiedType3 = true;
				break;
			}
		}

		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer || notSatisfiedType3
			|| (mergedMaskInOtherCells & comparer) != comparer)
		{
			return;
		}

		// Check whether the corners spanned two blocks. If so, UR + SdC can't be found.
		short blockMaskInOtherCells = otherCellsMap.BlockMask;
		if (!IsPow2(blockMaskInOtherCells))
		{
			return;
		}

		bool* cannibalModeCases = stackalloc[] { false, true };

		short otherDigitsMask = (short)(mergedMaskInOtherCells & ~comparer);
		byte line = (byte)otherCellsMap.CoveredLine;
		byte block = (byte)TrailingZeroCount(otherCellsMap.CoveredHouses & ~(1 << line));
		var (a, _, _, d) = IntersectionMaps[(line, block)];
		using scoped var list = new ValueList<CellMap>(4);
		for (int caseIndex = 0; caseIndex < 2; caseIndex++)
		{
			bool cannibalMode = cannibalModeCases[caseIndex];
			foreach (byte otherBlock in d)
			{
				var emptyCellsInInterMap = HousesMap[otherBlock] & HousesMap[line] & EmptyCells;
				if (emptyCellsInInterMap.Count < 2)
				{
					// The intersection needs at least two empty cells.
					continue;
				}

				CellMap b = HousesMap[otherBlock] - HousesMap[line], c = a & b;

				list.Clear();
				switch (emptyCellsInInterMap)
				{
					case [_, _]:
					{
						list.Add(emptyCellsInInterMap);

						break;
					}
					case [var i, var j, var k]:
					{
						list.Add(CellMap.Empty + i + j);
						list.Add(CellMap.Empty + j + k);
						list.Add(CellMap.Empty + i + k);
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
					var lineMap = (a & EmptyCells) - otherCellsMap;

					// Iterate on the number of the cells that should be selected in block.
					for (int i = 1; i <= blockMap.Count - 1; i++)
					{
						// Iterate on each combination in block.
						foreach (var selectedCellsInBlock in blockMap & i)
						{
							bool flag = false;
							foreach (int digit in otherDigitsMask)
							{
								foreach (int cell in selectedCellsInBlock)
								{
									if (CandidatesMap[digit].Contains(cell))
									{
										flag = true;
										break;
									}
								}
							}
							if (flag)
							{
								continue;
							}

							var currentBlockMap = selectedCellsInBlock;
							var elimMapBlock = CellMap.Empty;
							var elimMapLine = CellMap.Empty;

							// Get the links of the block.
							short blockMask = grid.GetDigitsUnion(selectedCellsInBlock);

							// Get the elimination map in the block.
							foreach (int digit in blockMask)
							{
								elimMapBlock |= CandidatesMap[digit];
							}
							elimMapBlock &= blockMap - currentBlockMap;

							foreach (int digit in otherDigitsMask)
							{
								elimMapLine |= CandidatesMap[digit];
							}
							elimMapLine &= lineMap - currentInterMap;

							checkGeneralizedSdc(
								accumulator, grid, arMode, cannibalMode, d1, d2, urCells,
								line, otherBlock, otherDigitsMask, blockMask, selectedInterMask,
								otherDigitsMask, elimMapLine, elimMapBlock, otherCellsMap, currentBlockMap,
								currentInterMap, i, 0, index
							);
						}
					}
				}
			}
		}


		static void checkGeneralizedSdc(
			ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
			bool arMode, bool cannibalMode, int digit1, int digit2, int[] urCells,
			int line, int block, short lineMask, short blockMask,
			short selectedInterMask, short otherDigitsMask, scoped in CellMap elimMapLine,
			scoped in CellMap elimMapBlock, scoped in CellMap currentLineMap, scoped in CellMap currentBlockMap,
			scoped in CellMap currentInterMap, int i, int j, int index)
		{
			short maskOnlyInInter = (short)(selectedInterMask & ~(blockMask | lineMask));
			short maskIsolated = (short)(
				cannibalMode ? (lineMask & blockMask & selectedInterMask) : maskOnlyInInter
			);
			if (!cannibalMode && ((blockMask & lineMask) != 0 || maskIsolated != 0 && !IsPow2(maskIsolated))
				|| cannibalMode && !IsPow2(maskIsolated))
			{
				return;
			}

			var elimMapIsolated = CellMap.Empty;
			int digitIsolated = TrailingZeroCount(maskIsolated);
			if (digitIsolated != InvalidValidOfTrailingZeroCountMethodFallback)
			{
				elimMapIsolated = (cannibalMode ? currentBlockMap | currentLineMap : currentInterMap)
					% CandidatesMap[digitIsolated]
					& EmptyCells;
			}

			if (currentInterMap.Count + i + j + 1 == PopCount((uint)blockMask) + PopCount((uint)lineMask) + PopCount((uint)maskOnlyInInter)
				&& (elimMapBlock | elimMapLine | elimMapIsolated) is not [])
			{
				// Check eliminations.
				var conclusions = new List<Conclusion>(10);
				foreach (int cell in elimMapBlock)
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						if ((blockMask >> digit & 1) != 0)
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
				}
				foreach (int cell in elimMapLine)
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						if ((lineMask >> digit & 1) != 0)
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
				}
				foreach (int cell in elimMapIsolated)
				{
					conclusions.Add(new(Elimination, cell, digitIsolated));
				}
				if (conclusions.Count == 0)
				{
					return;
				}

				// Record highlight candidates and cells.
				var candidateOffsets = new List<CandidateViewNode>();
				foreach (int cell in urCells)
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								(otherDigitsMask >> digit & 1) != 0
									? DisplayColorKind.Auxiliary1
									: DisplayColorKind.Normal,
								cell * 9 + digit
							)
						);
					}
				}
				foreach (int cell in currentBlockMap)
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								!cannibalMode && digit == digitIsolated
									? DisplayColorKind.Auxiliary3
									: DisplayColorKind.Auxiliary2,
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
									? DisplayColorKind.Auxiliary3
									: (otherDigitsMask >> digit & 1) != 0
										? DisplayColorKind.Auxiliary1
										: DisplayColorKind.Auxiliary2,
								cell * 9 + digit
							)
						);
					}
				}

				accumulator.Add(
					new UniqueRectangleWithSueDeCoqStep(
						ImmutableArray.CreateRange(conclusions),
						ImmutableArray.Create(
							View.Empty
								| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
								| candidateOffsets
								| new HouseViewNode[]
								{
									new(DisplayColorKind.Normal, block),
									new(DisplayColorKind.Auxiliary2, line)
								}
						),
						digit1,
						digit2,
						(CellMap)urCells,
						arMode,
						block,
						line,
						blockMask,
						lineMask,
						selectedInterMask,
						cannibalMode,
						maskIsolated,
						currentBlockMap,
						currentLineMap,
						currentInterMap,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR + Unknown covering.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="comparer">The comparer.</param>
	/// <param name="d1">The digit 1.</param>
	/// <param name="d2">The digit 2.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// <para>The structures:</para>
	/// <para>
	/// Subtype 1:
	/// <code><![CDATA[
	///      ↓urCellInSameBlock
	/// ab  abc      abc  ←anotherCell
	///
	///     abcx-----abcy ←resultCell
	///           c
	///      ↑targetCell
	/// ]]></code>
	/// Where the digit <c>a</c> and <c>b</c> in the bottom-left cell <c>abcx</c> can be removed.
	/// </para>
	/// <para>
	/// Subtype 2:
	/// <code><![CDATA[
	/// abcx   | ab  abc
	///  |     |
	///  | c   |
	///  |     |
	/// abcy   |     abc
	/// ]]></code>
	/// </para>
	/// </remarks>
	partial void CheckUnknownCoveringUnique(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells,
		short comparer, int d1, int d2, int index)
	{
		checkType1(grid);
#if IMPLEMENTED
		checkType2(grid);
#endif

		void checkType1(scoped in Grid grid)
		{
			var cells = (CellMap)urCells;

			// Check all cells are empty.
			bool containsValueCells = false;
			foreach (int cell in cells)
			{
				if (grid.GetStatus(cell) != CellStatus.Empty)
				{
					containsValueCells = true;
					break;
				}
			}
			if (containsValueCells)
			{
				return;
			}

			// Iterate on each cell.
			foreach (int targetCell in cells)
			{
				int block = targetCell.ToHouseIndex(HouseType.Block);
				var bivalueCellsToCheck = (PeersMap[targetCell] & HousesMap[block] & BivalueCells) - cells;
				if (bivalueCellsToCheck is [])
				{
					continue;
				}

				// Check all bi-value cells.
				foreach (int bivalueCellToCheck in bivalueCellsToCheck)
				{
					if ((CellMap.Empty + bivalueCellToCheck + targetCell).CoveredLine != InvalidValidOfTrailingZeroCountMethodFallback)
					{
						// 'targetCell' and 'bivalueCellToCheck' can't lie on a same line.
						continue;
					}

					if (grid.GetCandidates(bivalueCellToCheck) != comparer)
					{
						// 'bivalueCell' must contain both 'd1' and 'd2'.
						continue;
					}

					int urCellInSameBlock = ((HousesMap[block] & cells) - targetCell)[0];
					int coveredLine = (CellMap.Empty + bivalueCellToCheck + urCellInSameBlock).CoveredLine;
					if (coveredLine == InvalidValidOfTrailingZeroCountMethodFallback)
					{
						// The bi-value cell 'bivalueCellToCheck' should be lie on a same house
						// as 'urCellInSameBlock'.
						continue;
					}

					int anotherCell = (cells - urCellInSameBlock & HousesMap[coveredLine])[0];
					foreach (int extraDigit in grid.GetCandidates(targetCell) & ~comparer)
					{
						short abcMask = (short)(comparer | (short)(1 << extraDigit));

						if (grid.GetCandidates(anotherCell) != abcMask)
						{
							continue;
						}

						// Check the conjugate pair of the extra digit.
						int resultCell = (cells - urCellInSameBlock - anotherCell - targetCell)[0];
						var map = CellMap.Empty + targetCell + resultCell;
						int line = map.CoveredLine;
						if (!IUniqueRectangleStepSearcher.IsConjugatePair(extraDigit, map, line))
						{
							continue;
						}

						if (grid.GetCandidates(urCellInSameBlock) != abcMask)
						{
							goto SubType2;
						}

						// Here, is the basic sub-type having passed the checking.
						// Gather conclusions.
						var conclusions = new List<Conclusion>();
						foreach (int digit in grid.GetCandidates(targetCell))
						{
							if (digit == d1 || digit == d2)
							{
								conclusions.Add(new(Elimination, targetCell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							goto SubType2;
						}

						// Gather views.
						var candidateOffsets = new List<CandidateViewNode>
						{
							new(DisplayColorKind.Auxiliary1, targetCell * 9 + extraDigit)
						};
						if (CandidatesMap[d1].Contains(resultCell))
						{
							candidateOffsets.Add(new(DisplayColorKind.Normal, resultCell * 9 + d1));
						}
						if (CandidatesMap[d2].Contains(resultCell))
						{
							candidateOffsets.Add(new(DisplayColorKind.Normal, resultCell * 9 + d2));
						}
						if (CandidatesMap[extraDigit].Contains(resultCell))
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, resultCell * 9 + extraDigit));
						}

						foreach (int digit in grid.GetCandidates(urCellInSameBlock) & abcMask)
						{
							candidateOffsets.Add(new(DisplayColorKind.Normal, urCellInSameBlock * 9 + digit));
						}
						foreach (int digit in grid.GetCandidates(anotherCell))
						{
							candidateOffsets.Add(new(DisplayColorKind.Normal, anotherCell * 9 + digit));
						}
						short _xOr_yMask = grid.GetCandidates(bivalueCellToCheck);
						foreach (int digit in _xOr_yMask)
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary2, bivalueCellToCheck * 9 + digit));
						}

						// Add into the list.
						byte extraDigitId = (byte)(char)(extraDigit + '1');
						short extraDigitMask = (short)(1 << extraDigit);
						accumulator.Add(
							new UniqueRectangleWithUnknownCoveringStep(
								ImmutableArray.CreateRange(conclusions),
								ImmutableArray.Create(
									View.Empty
										| new CellViewNode(DisplayColorKind.Normal, targetCell)
										| candidateOffsets
										| new HouseViewNode[]
										{
											new(DisplayColorKind.Normal, block),
											new(DisplayColorKind.Auxiliary1, line)
										},
									View.Empty
										| new CandidateViewNode[]
										{
											new(DisplayColorKind.Auxiliary1, resultCell * 9 + extraDigit),
											new(DisplayColorKind.Auxiliary1, targetCell * 9 + extraDigit)
										}
										| new UnknownViewNode[]
										{
											new(DisplayColorKind.Normal, bivalueCellToCheck, (byte)'y', _xOr_yMask),
											new(DisplayColorKind.Normal, targetCell, (byte)'x', _xOr_yMask),
											new(DisplayColorKind.Normal, urCellInSameBlock, extraDigitId, extraDigitMask),
											new(DisplayColorKind.Normal, anotherCell, (byte)'x', _xOr_yMask),
											new(DisplayColorKind.Normal, resultCell, extraDigitId, extraDigitMask)
										}
								),
								d1,
								d2,
								(CellMap)urCells,
								targetCell,
								extraDigit,
								index
							)
						);

					SubType2:
						// Sub-type 2.
						// The extra digit should form a conjugate pair in that line.
						var anotherMap = CellMap.Empty + urCellInSameBlock + anotherCell;
						int anotherLine = anotherMap.CoveredLine;
						if (!IUniqueRectangleStepSearcher.IsConjugatePair(extraDigit, anotherMap, anotherLine))
						{
							continue;
						}

						// Gather conclusions.
						var conclusionsAnotherSubType = new List<Conclusion>();
						foreach (int digit in grid.GetCandidates(targetCell))
						{
							if (digit == d1 || digit == d2)
							{
								conclusionsAnotherSubType.Add(new(Elimination, targetCell, digit));
							}
						}
						if (conclusionsAnotherSubType.Count == 0)
						{
							continue;
						}

						// Gather views.
						var candidateOffsetsAnotherSubtype = new List<CandidateViewNode>
						{
							new(DisplayColorKind.Auxiliary1, targetCell * 9 + extraDigit)
						};
						if (CandidatesMap[d1].Contains(resultCell))
						{
							candidateOffsetsAnotherSubtype.Add(new(DisplayColorKind.Normal, resultCell * 9 + d1));
						}
						if (CandidatesMap[d2].Contains(resultCell))
						{
							candidateOffsetsAnotherSubtype.Add(new(DisplayColorKind.Normal, resultCell * 9 + d2));
						}
						if (CandidatesMap[extraDigit].Contains(resultCell))
						{
							candidateOffsetsAnotherSubtype.Add(
								new(DisplayColorKind.Auxiliary1, resultCell * 9 + extraDigit)
							);
						}

						var candidateOffsetsAnotherSubtypeLighter = new List<CandidateViewNode>
						{
							new(DisplayColorKind.Auxiliary1, resultCell * 9 + extraDigit),
							new(DisplayColorKind.Auxiliary1, targetCell * 9 + extraDigit)
						};
						foreach (int digit in grid.GetCandidates(urCellInSameBlock) & abcMask)
						{
							if (digit == extraDigit)
							{
								candidateOffsetsAnotherSubtype.Add(
									new(DisplayColorKind.Auxiliary1, urCellInSameBlock * 9 + digit)
								);
								candidateOffsetsAnotherSubtypeLighter.Add(
									new(DisplayColorKind.Auxiliary1, urCellInSameBlock * 9 + digit)
								);
							}
							else
							{
								candidateOffsetsAnotherSubtype.Add(
									new(DisplayColorKind.Normal, urCellInSameBlock * 9 + digit)
								);
							}
						}
						foreach (int digit in grid.GetCandidates(anotherCell))
						{
							if (digit == extraDigit)
							{
								candidateOffsetsAnotherSubtype.Add(
									new(DisplayColorKind.Auxiliary1, anotherCell * 9 + digit)
								);
								candidateOffsetsAnotherSubtypeLighter.Add(
									new(DisplayColorKind.Auxiliary1, anotherCell * 9 + digit)
								);
							}
							else
							{
								candidateOffsetsAnotherSubtype.Add(
									new(DisplayColorKind.Normal, anotherCell * 9 + digit)
								);
							}
						}
						short _xOr_yMask2 = grid.GetCandidates(bivalueCellToCheck);
						foreach (int digit in _xOr_yMask2)
						{
							candidateOffsetsAnotherSubtype.Add(
								new(DisplayColorKind.Auxiliary2, bivalueCellToCheck * 9 + digit)
							);
						}

						// Add into the list.
						byte extraDigitId2 = (byte)(char)(extraDigit + '1');
						short extraDigitMask2 = (short)(1 << extraDigit);
						accumulator.Add(
							new UniqueRectangleWithUnknownCoveringStep(
								ImmutableArray.CreateRange(conclusionsAnotherSubType),
								ImmutableArray.Create(
									View.Empty
										| new CellViewNode(DisplayColorKind.Normal, targetCell)
										| candidateOffsetsAnotherSubtype
										| new HouseViewNode[]
										{
											new(DisplayColorKind.Normal, block),
											new(DisplayColorKind.Auxiliary1, line),
											new(DisplayColorKind.Auxiliary1, anotherLine)
										},
									View.Empty
										| candidateOffsetsAnotherSubtypeLighter
										| new UnknownViewNode[]
										{
											new(DisplayColorKind.Normal, bivalueCellToCheck, (byte)'y', _xOr_yMask2),
											new(DisplayColorKind.Normal, targetCell, (byte)'x', _xOr_yMask2),
											new(DisplayColorKind.Normal, urCellInSameBlock, extraDigitId2, extraDigitMask2),
											new(DisplayColorKind.Normal, anotherCell, (byte)'x', _xOr_yMask2),
											new(DisplayColorKind.Normal, resultCell, extraDigitId2, extraDigitMask2)
										}
								),
								d1,
								d2,
								(CellMap)urCells,
								targetCell,
								extraDigit,
								index
							)
						);
					}
				}
			}
		}

#if false
		void checkType2(scoped in Grid grid)
		{
			// TODO: Check type 2.
		}
#endif
	}

	/// <summary>
	/// <para>Check UR + Guardian (i.e. UR External Type 2) and UR External Type 1.</para>
	/// <para>
	/// A UR external type 1 is a special case for type 2, which means only one guardian cell will be used.
	/// </para>
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="index">The index.</param>
	partial void CheckExternalType1Or2(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, int d1, int d2, int index)
	{
		var cells = (CellMap)urCells;

		if (!IUniqueRectangleStepSearcher.CheckPreconditionsOnIncomplete(grid, urCells, d1, d2))
		{
			return;
		}

		// Iterate on two houses used.
		foreach (int[] houseCombination in cells.Houses.GetAllSets().GetSubsets(2))
		{
			var houseCells = HousesMap[houseCombination[0]] | HousesMap[houseCombination[1]];
			if ((houseCells & cells) != cells)
			{
				// The houses must contain all 4 UR cells.
				continue;
			}

			var guardian1 = houseCells - cells & CandidatesMap[d1];
			var guardian2 = houseCells - cells & CandidatesMap[d2];
			if (!(guardian1 is [] ^ guardian2 is []))
			{
				// Only one digit can contain guardians.
				continue;
			}

			int guardianDigit = -1;
			CellMap? targetElimMap = null, targetGuardianMap = null;
			if (guardian1 is not [] && (+guardian1 & CandidatesMap[d1]) is var a and not [])
			{
				targetElimMap = a;
				guardianDigit = d1;
				targetGuardianMap = guardian1;
			}
			else if (guardian2 is not [] && (+guardian2 & CandidatesMap[d2]) is var b and not [])
			{
				targetElimMap = b;
				guardianDigit = d2;
				targetGuardianMap = guardian2;
			}

			if (targetElimMap is not { } elimMap || targetGuardianMap is not { } guardianMap
				|| guardianDigit == -1)
			{
				continue;
			}

			var candidateOffsets = new List<CandidateViewNode>(16);
			foreach (int cell in urCells)
			{
				if (CandidatesMap[d1].Contains(cell))
				{
					candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d1));
				}
				if (CandidatesMap[d2].Contains(cell))
				{
					candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d2));
				}
			}
			foreach (int cell in guardianMap)
			{
				candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + guardianDigit));
			}

			accumulator.Add(
				new UniqueRectangleWithGuardianStep(
					from cell in elimMap select new Conclusion(Elimination, cell, guardianDigit),
					ImmutableArray.Create(
						View.Empty
							| candidateOffsets
							| new HouseViewNode[]
							{
								new(DisplayColorKind.Normal, houseCombination[0]),
								new(DisplayColorKind.Normal, houseCombination[1])
							}
					),
					d1,
					d2,
					(CellMap)urCells,
					guardianMap,
					guardianDigit,
					IsIncomplete(candidateOffsets),
					index
				)
			);
		}
	}

	/// <summary>
	/// Check UR + Guardian, with external subset (i.e. UR External Type 3).
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="index">The index.</param>
	partial void CheckExternalType3(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, short comparer, int d1, int d2, int index)
	{
		if (!IUniqueRectangleStepSearcher.CheckPreconditionsOnIncomplete(grid, urCells, d1, d2))
		{
			return;
		}

		var cells = (CellMap)urCells;

		// Iterate on two houses used.
		foreach (int[] houseCombination in cells.Houses.GetAllSets().GetSubsets(2))
		{
			var guardianMap = HousesMap[houseCombination[0]] | HousesMap[houseCombination[1]];
			if ((guardianMap & cells) != cells)
			{
				// The houses must contain all 4 UR cells.
				continue;
			}

			var guardianCells = guardianMap - cells & EmptyCells;
			foreach (var guardianCellPair in guardianCells & 2)
			{
				int c1 = guardianCellPair[0], c2 = guardianCellPair[1];
				if (!IUniqueRectangleStepSearcher.IsSameHouseCell(c1, c2, out int houses))
				{
					// Those two cells must lie in a same house.
					continue;
				}

				short mask = (short)(grid.GetCandidates(c1) | grid.GetCandidates(c2));
				if ((mask & comparer) != comparer)
				{
					// The two cells must contain both two digits.
					continue;
				}

				if ((grid.GetCandidates(c1) & comparer) == 0 || (grid.GetCandidates(c2) & comparer) == 0)
				{
					// Both two cells chosen must contain at least one of two UR digits.
					continue;
				}

				if ((guardianCells & (CandidatesMap[d1] | CandidatesMap[d2])) != guardianCellPair)
				{
					// The current map must be equal to the whole guardian full map.
					continue;
				}

				foreach (int house in houses)
				{
					var houseCells = HousesMap[house] - cells - guardianCellPair & EmptyCells;
					for (int size = 2; size <= houseCells.Count; size++)
					{
						foreach (var otherCells in houseCells & size - 1)
						{
							short subsetDigitsMask = (short)(grid.GetDigitsUnion(otherCells) | comparer);
							if (PopCount((uint)subsetDigitsMask) != size)
							{
								// The subset cannot formed.
								continue;
							}

							// UR Guardian External Subsets found. Now check eliminations.
							var elimMap = (houseCells | guardianCellPair) - otherCells;
							var conclusions = new List<Conclusion>();
							foreach (int cell in elimMap)
							{
								short elimDigitsMask = guardianCellPair.Contains(cell)
									? (short)(subsetDigitsMask & ~comparer)
									: subsetDigitsMask;

								foreach (int digit in elimDigitsMask)
								{
									if (CandidatesMap[digit].Contains(cell))
									{
										conclusions.Add(new(Elimination, cell, digit));
									}
								}
							}
							if (conclusions.Count == 0)
							{
								continue;
							}

							var candidateOffsets = new List<CandidateViewNode>();
							foreach (int cell in urCells)
							{
								if (CandidatesMap[d1].Contains(cell))
								{
									candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d1));
								}
								if (CandidatesMap[d2].Contains(cell))
								{
									candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d2));
								}
							}
							foreach (int cell in guardianCellPair)
							{
								if (CandidatesMap[d1].Contains(cell))
								{
									candidateOffsets.Add(new(DisplayColorKind.Auxiliary2, cell * 9 + d1));
								}
								if (CandidatesMap[d2].Contains(cell))
								{
									candidateOffsets.Add(new(DisplayColorKind.Auxiliary2, cell * 9 + d2));
								}
							}
							foreach (int cell in otherCells)
							{
								foreach (int digit in grid.GetCandidates(cell))
								{
									candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
								}
							}

							accumulator.Add(
								new UniqueRectangleWithGuardianSubsetStep(
									conclusions.ToImmutableArray(),
									ImmutableArray.Create(
										View.Empty
											| candidateOffsets
											| new HouseViewNode[]
											{
												new(DisplayColorKind.Normal, house),
												new(DisplayColorKind.Auxiliary2, houseCombination[0]),
												new(DisplayColorKind.Auxiliary2, houseCombination[1])
											}
									),
									d1,
									d2,
									cells,
									guardianCellPair,
									otherCells,
									subsetDigitsMask,
									IsIncomplete(candidateOffsets),
									index
								)
							);
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Check UR + Guardian, with external conjugate pair (i.e. UR External Type 4).
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="index">The index.</param>
	partial void CheckExternalType4(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells,
		short comparer, int d1, int d2, int index)
	{
		if (!IUniqueRectangleStepSearcher.CheckPreconditionsOnIncomplete(grid, urCells, d1, d2))
		{
			return;
		}

		var cells = (CellMap)urCells;

		// Iterate on two houses used.
		foreach (int[] houseCombination in cells.Houses.GetAllSets().GetSubsets(2))
		{
			var guardianMap = HousesMap[houseCombination[0]] | HousesMap[houseCombination[1]];
			if ((guardianMap & cells) != cells)
			{
				// The houses must contain all 4 UR cells.
				continue;
			}

			var guardianCells = guardianMap - cells & EmptyCells;
			foreach (var guardianCellPair in guardianCells & 2)
			{
				int c1 = guardianCellPair[0], c2 = guardianCellPair[1];
				if (!IUniqueRectangleStepSearcher.IsSameHouseCell(c1, c2, out int houses))
				{
					// Those two cells must lie in a same house.
					continue;
				}

				short mask = (short)(grid.GetCandidates(c1) | grid.GetCandidates(c2));
				if ((mask & comparer) != comparer)
				{
					// The two cells must contain both two digits.
					continue;
				}

				if ((grid.GetCandidates(c1) & comparer) == 0 || (grid.GetCandidates(c2) & comparer) == 0)
				{
					// Both two cells chosen must contain at least one of two UR digits.
					continue;
				}

				if ((guardianCells & (CandidatesMap[d1] | CandidatesMap[d2])) != guardianCellPair)
				{
					// The current map must be equal to the whole guardian full map.
					continue;
				}

				short possibleConjugatePairDigitsMask = (short)(grid.GetDigitsUnion(guardianCellPair) & ~comparer);
				foreach (int house in houses)
				{
					foreach (int conjugatePairDigit in possibleConjugatePairDigitsMask)
					{
						if (!CandidatesMap[conjugatePairDigit].Contains(c1)
							|| !CandidatesMap[conjugatePairDigit].Contains(c2))
						{
							// The conjugate pair can be formed if and only if both guardian cells
							// must contain that digit.
							continue;
						}

						if ((CandidatesMap[conjugatePairDigit] & HousesMap[house]) != guardianCellPair)
						{
							// The house cannot contain any other cells containing that digit.
							continue;
						}

						var conclusions = new List<Conclusion>();
						short elimDigitsMask = (short)(possibleConjugatePairDigitsMask & ~(1 << conjugatePairDigit));
						foreach (int elimDigit in elimDigitsMask)
						{
							foreach (int cell in CandidatesMap[elimDigit] & guardianCellPair)
							{
								conclusions.Add(new(Elimination, cell, elimDigit));
							}
						}
						if (conclusions.Count == 0)
						{
							// No eliminations found.
							continue;
						}

						var candidateOffsets = new List<CandidateViewNode>();
						foreach (int cell in urCells)
						{
							if (CandidatesMap[d1].Contains(cell))
							{
								candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d1));
							}
							if (CandidatesMap[d2].Contains(cell))
							{
								candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d2));
							}
						}
						foreach (int cell in guardianCellPair)
						{
							if (CandidatesMap[d1].Contains(cell))
							{
								candidateOffsets.Add(new(DisplayColorKind.Auxiliary2, cell * 9 + d1));
							}
							if (CandidatesMap[d2].Contains(cell))
							{
								candidateOffsets.Add(new(DisplayColorKind.Auxiliary2, cell * 9 + d2));
							}

							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + conjugatePairDigit));
						}

						accumulator.Add(
							new UniqueRectangleWithGuardianConjugatePairStep(
								conclusions.ToImmutableArray(),
								ImmutableArray.Create(
									View.Empty
										| candidateOffsets
										| new HouseViewNode[]
										{
											new(DisplayColorKind.Normal, house),
											new(DisplayColorKind.Auxiliary2, houseCombination[0]),
											new(DisplayColorKind.Auxiliary2, houseCombination[1])
										}
								),
								d1,
								d2,
								cells,
								guardianCellPair,
								new(guardianCellPair, conjugatePairDigit),
								IsIncomplete(candidateOffsets),
								index
							)
						);
					}
				}
			}
		}
	}

	/// <summary>
	/// Check AR + Hidden single.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">
	/// The map of other cells during the current UR searching.
	/// </param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// <para>The structure:</para>
	/// <para>
	/// <code><![CDATA[
	/// ↓corner1
	/// a   | aby  -  -
	/// abx | a    -  b
	///     | -    -  -
	///       ↑corner2(cell 'a')
	/// ]]></code>
	/// There's only one cell can be filled with the digit 'b' besides the cell 'aby'.
	/// </para>
	/// </remarks>
	partial void CheckHiddenSingleAvoidable(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, int d1, int d2, int corner1, int corner2, scoped in CellMap otherCellsMap, int index)
	{
		if (grid.GetStatus(corner1) != CellStatus.Modifiable
			|| grid.GetStatus(corner2) != CellStatus.Modifiable
			|| grid[corner1] != grid[corner2] || grid[corner1] != d1 && grid[corner1] != d2)
		{
			return;
		}

		// Get the base digit ('a') and the other digit ('b').
		// Here 'b' is the digit that we should check the possible hidden single.
		int baseDigit = grid[corner1], otherDigit = baseDigit == d1 ? d2 : d1;
		var cellsThatTwoOtherCellsBothCanSee = +otherCellsMap & CandidatesMap[otherDigit];

		// Iterate on two cases (because we holds two other cells,
		// and both those two cells may contain possible elimination).
		for (int i = 0; i < 2; i++)
		{
			var (baseCell, anotherCell) = i == 0 ? (otherCellsMap[0], otherCellsMap[1]) : (otherCellsMap[1], otherCellsMap[0]);

			// Iterate on each house type.
			foreach (var houseType in HouseTypes)
			{
				int houseIndex = baseCell.ToHouseIndex(houseType);

				// If the house doesn't overlap with the specified house, just skip it.
				if ((cellsThatTwoOtherCellsBothCanSee & HousesMap[houseIndex]) is [])
				{
					continue;
				}

				var otherCells = HousesMap[houseIndex] & CandidatesMap[otherDigit] & PeersMap[anotherCell];
				int sameHouses = (otherCells + anotherCell).CoveredHouses;
				foreach (int sameHouse in sameHouses)
				{
					// Check whether all possible positions of the digit 'b' in this house only
					// lies in the given cells above ('cellsThatTwoOtherCellsBothCanSee').
					if ((HousesMap[sameHouse] - anotherCell & CandidatesMap[otherDigit]) != otherCells)
					{
						continue;
					}

					// Possible hidden single found.
					// If the elimination doesn't exist, just skip it.
					if (!CandidatesMap[otherDigit].Contains(baseCell))
					{
						continue;
					}

					var cellOffsets = new List<CellViewNode>();
					foreach (int cell in urCells)
					{
						cellOffsets.Add(new(DisplayColorKind.Normal, cell));
					}

					var candidateOffsets = new List<CandidateViewNode>
					{
						new(DisplayColorKind.Normal, anotherCell * 9 + otherDigit)
					};
					foreach (int cell in otherCells)
					{
						candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + otherDigit));
					}

					accumulator.Add(
						new AvoidableRectangleWithHiddenSingleStep(
							ImmutableArray.Create(new Conclusion(Elimination, baseCell, otherDigit)),
							ImmutableArray.Create(
								View.Empty
									| cellOffsets
									| candidateOffsets
									| new HouseViewNode(DisplayColorKind.Normal, sameHouse)
							),
							d1,
							d2,
							(CellMap)urCells,
							baseCell,
							anotherCell,
							sameHouse,
							index
						)
					);
				}
			}
		}
	}
}
