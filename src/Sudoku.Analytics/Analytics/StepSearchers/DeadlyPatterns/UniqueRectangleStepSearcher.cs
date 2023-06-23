namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Unique Rectangle</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Basic types:
/// <list type="bullet">
/// <item>Unique Rectangle Type 1-6</item>
/// <item>Avoidable Rectangle Type 1-3, 5</item>
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
/// <item>Unique Rectangle External Type 1-4</item>
/// <item>Unique Rectangle External Type with XY-Wing</item>
/// <item>Unique Rectangle External Type with ALS-XZ</item>
/// <item>Unique Rectangle External Type with Skyscraper</item>
/// </list>
/// </item>
/// <item>Avoidable Rectangle + Guardian (Sub-types are same like above)</item>
/// </list>
/// </item>
/// <item>
/// Miscellaneous types:
/// <list type="bullet">
/// <item>Unique Rectangle 2D, 3X</item>
/// </list>
/// </item>
/// </list>
/// </summary>
[StepSearcher(new[] { DifficultyLevel.Hard, DifficultyLevel.Fiendish }, ConditionalCases = ConditionalCase.Standard)]
public sealed partial class UniqueRectangleStepSearcher : StepSearcher
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
	[RuntimeIdentifier(RuntimeIdentifier.AllowIncompleteUniqueRectangles)]
	public bool AllowIncompleteUniqueRectangles { get; set; }

	/// <summary>
	/// Indicates whether the searcher can search for extended URs.
	/// </summary>
	/// <remarks>
	/// The basic types are type 1 to type 6, all other types are extended ones.
	/// </remarks>
	[RuntimeIdentifier(RuntimeIdentifier.SearchForExtendedUniqueRectangles)]
	public bool SearchForExtendedUniqueRectangles { get; set; }


	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		var list = new List<UniqueRectangleStep>();

		scoped ref readonly var grid = ref context.Grid;

		// Iterate on mode (whether use AR or UR mode to search).
		Collect(list, grid, false);
		Collect(list, grid, true);

		if (list.Count == 0)
		{
			return null;
		}

		// Sort and remove duplicate instances if worth.
		var resultList = from step in list.Distinct() orderby step.Code, step.AbsoluteOffset select step;
		if (context.OnlyFindOne)
		{
			return resultList.FirstOrDefault();
		}

		context.Accumulator.AddRange(resultList);
		return null;
	}

	/// <summary>
	/// Get all possible hints from the grid.
	/// </summary>
	/// <param name="gathered"><inheritdoc cref="AnalysisContext.Accumulator" path="/summary"/></param>
	/// <param name="grid"><inheritdoc cref="AnalysisContext.Grid" path="/summary"/></param>
	/// <param name="arMode">Indicates whether the current mode is searching for ARs.</param>
	private void Collect(ICollection<UniqueRectangleStep> gathered, scoped in Grid grid, bool arMode)
	{
		// Search for ALSes. This result will be used by UR External ALS-XZ structures.
		var alses = grid.GatherAlmostLockedSets();

		// Iterate on each possible UR structure.
		for (var index = 0; index < UniqueRectangleStepSearcherHelper.CountOfPatterns; index++)
		{
			var urCells = UniqueRectangleStepSearcherHelper.UniqueRectanglePatterns[index];

			// Check preconditions.
			if (!UniqueRectangleStepSearcherHelper.CheckPreconditions(grid, urCells, arMode))
			{
				continue;
			}

			// Get all candidates that all four cells appeared.
			var mask = grid.GetDigitsUnion(urCells);

			// Iterate on each possible digit combination.
			scoped var allDigitsInThem = mask.GetAllSets();
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
						CheckBabaGroupingUnique(gathered, grid, urCells, comparer, d1, d2, index);
						CheckExternalType1Or2(gathered, grid, urCells, d1, d2, index, arMode);
						CheckExternalType3(gathered, grid, urCells, comparer, d1, d2, index, arMode);
						CheckExternalType4(gathered, grid, urCells, comparer, d1, d2, index, arMode);
						CheckExternalTurbotFish(gathered, grid, urCells, comparer, d1, d2, index, arMode);
						CheckExternalXyWing(gathered, grid, urCells, comparer, d1, d2, index, arMode);
						CheckExternalAlmostLockedSetsXz(gathered, grid, urCells, alses, comparer, d1, d2, index, arMode);
					}

					// Iterate on each corner of four cells.
					for (var c1 = 0; c1 < 4; c1++)
					{
						var corner1 = urCells[c1];
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

						for (var c2 = c1 + 1; c2 < 4; c2++)
						{
							var corner2 = urCells[c2];
							var tempOtherCellsMap = otherCellsMap - corner2;

							// Both diagonal and non-diagonal.
							CheckType2(gathered, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);

							if (SearchForExtendedUniqueRectangles)
							{
								for (var size = 2; size <= 4; size++)
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

	partial void CheckType1(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell cornerCell, scoped in CellMap otherCellsMap, int index);
	partial void CheckType2(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, scoped in CellMap otherCellsMap, int index);
	partial void CheckType3(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, scoped in CellMap otherCellsMap, int index);
	partial void CheckType4(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, scoped in CellMap otherCellsMap, int index);
	partial void CheckType5(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell cornerCell, scoped in CellMap otherCellsMap, int index);
	partial void CheckType6(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, scoped in CellMap otherCellsMap, int index);
	partial void CheckHidden(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell cornerCell, scoped in CellMap otherCellsMap, int index);
	partial void Check2D(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, scoped in CellMap otherCellsMap, int index);
	partial void Check2B1SL(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, scoped in CellMap otherCellsMap, int index);
	partial void Check2D1SL(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, scoped in CellMap otherCellsMap, int index);
	partial void Check3X(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell cornerCell, scoped in CellMap otherCellsMap, int index);
	partial void Check3X2SL(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell cornerCell, scoped in CellMap otherCellsMap, int index);
	partial void Check3N2SL(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell cornerCell, scoped in CellMap otherCellsMap, int index);
	partial void Check3U2SL(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell cornerCell, scoped in CellMap otherCellsMap, int index);
	partial void Check3E2SL(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell cornerCell, scoped in CellMap otherCellsMap, int index);
	partial void Check4X3SL(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, scoped in CellMap otherCellsMap, int index);
	partial void Check4C3SL(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, scoped in CellMap otherCellsMap, int index);
	partial void CheckRegularWing(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, scoped in CellMap otherCellsMap, int size, int index);
	partial void CheckSueDeCoq(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, bool arMode, Mask comparer, Digit d1, Digit d2, Cell corner1, Cell corner2, scoped in CellMap otherCellsMap, int index);
	partial void CheckBabaGroupingUnique(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, Mask comparer, Digit d1, Digit d2, int index);
	partial void CheckExternalType1Or2(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, Digit d1, Digit d2, int index, bool arMode);
	partial void CheckExternalType3(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, Mask comparer, Digit d1, Digit d2, int index, bool arMode);
	partial void CheckExternalType4(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, Mask comparer, Digit d1, Digit d2, int index, bool arMode);
	partial void CheckExternalTurbotFish(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, Mask comparer, Digit d1, Digit d2, int index, bool arMode);
	partial void CheckExternalXyWing(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, Mask comparer, Digit d1, Digit d2, int index, bool arMode);
	partial void CheckExternalAlmostLockedSetsXz(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, AlmostLockedSet[] alses, Mask comparer, Digit d1, Digit d2, int index, bool arMode);
	partial void CheckHiddenSingleAvoidable(ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, Cell[] urCells, Digit d1, Digit d2, Cell corner1, Cell corner2, scoped in CellMap otherCellsMap, int index);
}
