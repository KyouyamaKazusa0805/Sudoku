using System.Numerics;
using System.Runtime.CompilerServices;
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
using static Sudoku.Concepts.Intersection;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Analytics.StepSearchers;

using LockedMember = (CellMap LockedCells, House LockedBlock);
using TargetCellsGroup = BitStatusMapGroup<CellMap, Cell, House>;

/// <summary>
/// Provides with an <b>Exocet</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Junior Exocets:
/// <list type="bullet">
/// <item>Standard Junior Exocet</item>
/// <item>Junior Exocet (Target Conjugate Pair)</item>
/// <item>Junior Exocet (Mirror Conjugate Pair)</item>
/// <item>Junior Exocet (Adjacent Target)</item>
/// <item>Junior Exocet (Incompatible Pair)</item>
/// <item>Junior Exocet (Target Pair)</item>
/// <item>Junior Exocet (Generalized Fish)</item>
/// <item>Junior Exocet (Mirror AHS)</item>
/// <item>Junior Exocet (Locked Member)</item>
/// <item>
/// Weak Exocets:
/// <list type="bullet">
/// <item>Standard Weak Exocet</item>
/// <item>Weak Exocet (Adjacent Target)</item>
/// <item>Weak Exocet (Slash)</item>
/// <item>Weak Exocet (BZ Rectangle)</item>
/// <item>Lame Weak Exocet</item>
/// </list>
/// </item>
/// <item>
/// Double Exocets:
/// <list type="bullet">
/// <item>Standard Double Exocet</item>
/// <item>Double Exocet (Uni-Fish Pattern)</item>
/// <item>Double Exocet (Multi-Fish Pattern)</item>
/// </list>
/// </item>
/// <item>Pattern-Locked Quadruple</item>
/// </list>
/// </item>
/// <item>
/// Senior Exocets:
/// <list type="bullet">
/// <item>Standard Senior Exocet</item>
/// <item>Senior Exocet (Mirror)</item>
/// <item>Senior Exocet (Locked Member)</item>
/// </list>
/// </item>
/// </list>
/// </summary>
[StepSearcher(
	Technique.JuniorExocet, Technique.JuniorExocetConjugatePair, Technique.JuniorExocetMirror, Technique.JuniorExocetAdjacentTarget,
	Technique.JuniorExocetIncompatiblePair, Technique.JuniorExocetTargetPair, Technique.JuniorExocetGeneralizedFish,
	Technique.JuniorExocetMirrorAlmostHiddenSet, Technique.JuniorExocetLockedMember, Technique.SeniorExocet, Technique.SeniorExocetMirror,
	Technique.SeniorExocetLockedMember, Technique.WeakExocet, Technique.WeakExocetAdjacentTarget, Technique.WeakExocetSlash,
	Technique.WeakExocetBzRectangle, Technique.LameWeakExocet, Technique.DoubleExocet, Technique.DoubleExocetGeneralizedFish,
	Technique.FrankenJuniorExocet, Technique.FrankenSeniorExocet, Technique.MutantJuniorExocet,
	Technique.MutantSeniorExocet, Technique.PatternLockedQuadruple)]
public sealed partial class ExocetStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		scoped var chuteIndexBox = (stackalloc Count[3]);
		foreach (var isRow in (true, false))
		{
			// Iterate by size of houses to be iterated.
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

					// We manually disable the case that 3 of 3 (or 4) houses in a same chute.
					chuteIndexBox.Clear();
					foreach (var house in houses)
					{
						chuteIndexBox[(isRow ? house - 9 : house - 18) / 3]++;
					}
					if (chuteIndexBox.Contains(3))
					{
						// This case is invalid.
						continue;
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
						for (var baseSize = 1; baseSize <= 2; baseSize++)
						{
							// Iterate on each empty cells, or a cell group whose length is equal to iteration variable 'baseCellsSize'.
							foreach (ref readonly var minilineBaseCells in MinilinesGroupedByChuteIndex[i].AsReadOnlySpan())
							{
								var baseEmptyCellsToBeIterated = minilineBaseCells & EmptyCells;
								if (!baseEmptyCellsToBeIterated)
								{
									// No cells can be iterated.
									continue;
								}

								// Iterate on each miniline, to get all possible cases.
								foreach (ref readonly var baseCells in baseEmptyCellsToBeIterated.GetSubsets(baseSize).AsReadOnlySpan())
								{
									if (housesEmptyCells & baseCells)
									{
										// Base cells shouldn't be located in the current list of houses being iterated.
										continue;
									}

									var baseCellsDigitsMask = grid[in baseCells];

									// Now we should check for target cells.
									// The target cells must be located in houses being iterated, and intersects with the current chute.
									var targetCells = (chuteEmptyCells & housesEmptyCells) - baseCells.PeerIntersection;
									var targetCellsDigitsMask = grid[in targetCells];

									// Check whether all digits appeared in base cells can be filled in target empty cells.
									if ((targetCellsDigitsMask & baseCellsDigitsMask) != baseCellsDigitsMask)
									{
										// They are out of relation.
										continue;
									}

									// Check whether the number of total target cell groups must be 2.
									scoped var groupsOfTargetCells = GroupTargets(in targetCells, housesMask);
									if (groupsOfTargetCells.Length != baseSize)
									{
										continue;
									}

									// Check whether all cross-line lines contains at least one digit appeared in base cells.
									var crossline = housesCells - chuteCells;
									var atLeastOneLineContainNoDigitsAppearedInBase = false;
									foreach (var line in isRow ? crossline.RowMask << 9 : crossline.ColumnMask << 18)
									{
										var lineMask = grid[HousesMap[line] & crossline & EmptyCells];
										if ((lineMask & baseCellsDigitsMask) == 0)
										{
											atLeastOneLineContainNoDigitsAppearedInBase = true;
											break;
										}
									}
									if (atLeastOneLineContainNoDigitsAppearedInBase)
									{
										continue;
									}

									// Check whether escape cells contain any digits appeared in base. If so, invalid.
									var escapeCellsContainValueCellsDigitAppearedInBaseCells = false;
									foreach (var cell in housesCells - crossline - EmptyCells)
									{
										if ((baseCellsDigitsMask >> grid.GetDigit(cell) & 1) != 0)
										{
											escapeCellsContainValueCellsDigitAppearedInBaseCells = true;
											break;
										}
									}
									if (escapeCellsContainValueCellsDigitAppearedInBaseCells)
									{
										continue;
									}

									// Check whether all groups of target cells don't exceed the maximum limit, 2 cells.
									var containsAtLeastOneGroupMoreThanTwoCells = false;
									foreach (ref readonly var element in groupsOfTargetCells)
									{
										if (element.Count > 2)
										{
											containsAtLeastOneGroupMoreThanTwoCells = true;
											break;
										}
									}
									if (containsAtLeastOneGroupMoreThanTwoCells)
									{
										continue;
									}

									// Get the count delta (target group by block.count - base.count).
									// Delta can be -2, -1 or 0. In fact the possible values can be [-2, 2], but 1 and 2 are invalid. Details:
									//
									//   delta < 0:
									//      The base contain more cells than the target, meaning the pattern may be a senior exocet;
									//   delta > 0:
									//      The target contain more cells than the base, meaning the pattern contains more group
									//      than the number of base cells, to lead to no conclusion;
									//   delta == 0:
									//      The base has same number of cells with the target, a standard junior exocet will be formed.
									//
									// Therefore, I just check for the value on -2, -1 and 0.
									// Note: Today we should only consider the cases on delta = 0 or -1.
									var delta = groupsOfTargetCells.Length - baseCells.Count;
									switch (delta)
									{
										case -1:
										{
											if (CollectSeniorExocets(
												ref context, in grid, in baseCells, in targetCells, groupsOfTargetCells, in crossline,
												in housesCells, in chuteCells, baseCellsDigitsMask, housesMask, isRow, size, delta, i
											) is { } seniorTypeStep)
											{
												return seniorTypeStep;
											}

											break;
										}
										case 0:
										{
											if (CollectWeakExocets(
												ref context, in grid, in baseCells, groupsOfTargetCells, in crossline, baseCellsDigitsMask,
												housesMask, isRow, size, i
											) is { } weakTypeStep)
											{
												return weakTypeStep;
											}

											if (CollectJuniorExocets(
												ref context, in grid, in baseCells, in targetCells, groupsOfTargetCells, in crossline,
												baseCellsDigitsMask, housesMask, isRow, size, delta, i
											) is { } juniorTypeStep)
											{
												return juniorTypeStep;
											}

											if (CollectDoubleExocets(
												ref context, in grid, in baseCells, in targetCells, groupsOfTargetCells, in crossline,
												in housesEmptyCells, baseCellsDigitsMask, housesMask, isRow, size, delta, i
											) is { } doubleTypeStep)
											{
												return doubleTypeStep;
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
		}

		return null;
	}


	/// <summary>
	/// The core method to check for Junior Exocet sub-types.
	/// </summary>
	/// <param name="context">The analysis context to be used for accumulate steps.</param>
	/// <param name="grid">The grid as the candidate referencing object.</param>
	/// <param name="baseCells">The cells of the base.</param>
	/// <param name="targetCells">The cells of the target. The target cell only contains empty cells; non-empty ones will be ignored.</param>
	/// <param name="groupsOfTargetCells">
	/// The grouped cells for the argument <paramref name="targetCells"/>, by is containing crossline house.
	/// </param>
	/// <param name="crossline">
	/// The cells of cross-line. The cross-line cells contain 18 cells regardless of its state: empty or non-empty.
	/// </param>
	/// <param name="baseCellsDigitsMask">The mask that holds a list of digits that are appeared in base cells.</param>
	/// <param name="housesMask">The mask that holds a list of houses being iterated.</param>
	/// <param name="isRow">Indicates whether the exocet pattern is row-ish. The direction is same as cross-line cells.</param>
	/// <param name="size">
	/// The size of houses used in the cross-line cells. The value can be 3 or 4; higher-sized cases cannot be determined
	/// because I don't know. :(
	/// </param>
	/// <param name="delta">The delta value. The value can be 0, -1 and -2.</param>
	/// <param name="chuteIndex">The chute index to be used. Valid values are between 0 and 6.</param>
	/// <returns>A valid <see cref="ExocetStep"/> instance calculated and found.</returns>
	private static ExocetStep? CollectJuniorExocets(
		scoped ref AnalysisContext context,
		scoped ref readonly Grid grid,
		scoped ref readonly CellMap baseCells,
		scoped ref readonly CellMap targetCells,
		scoped ReadOnlySpan<TargetCellsGroup> groupsOfTargetCells,
		scoped ref readonly CellMap crossline,
		Mask baseCellsDigitsMask,
		HouseMask housesMask,
		bool isRow,
		Count size,
		Offset delta,
		Offset chuteIndex
	)
	{
		// Check for maximum times can be appeared in cross-line cells,
		// and collect all possible digits can be filled in cross-line for (size - 1) times at most.
		// Here will cause an confliction: Locked Members will make digits appeared in base cells
		// appear in cross-line cells as values (i.e. non-empty cells).
		// However, sometimes, we may encounter a case that digits appeared in base cells indeed appears
		// in cross-line cells as values, but they are non-locked members.
		// Here is an example:
		//
		//   98.7.....6.....97...7.....54...3..2...86..4.......4..1..68..5......1...4.....2.3.
		//
		// We should treat the digit as a non-locked member.

		var (digitsMaskExactlySizeMinusOneTimes, digitsMaskAppearedInCrossline) = ((Mask)0, (Mask)0);
		foreach (var digit in baseCellsDigitsMask)
		{
			if (grid.ExactlyAppearingTimesWith(digit, in crossline, size - 1))
			{
				// The current digit can be filled in cross-line cells at most (size - 1) times.
				digitsMaskExactlySizeMinusOneTimes |= (Mask)(1 << digit);
			}

			foreach (var cell in crossline)
			{
				if (grid.GetDigit(cell) == digit)
				{
					digitsMaskAppearedInCrossline |= (Mask)(1 << digit);
					break;
				}
			}
		}

		var lockedMemberDigitsMask = baseCellsDigitsMask;

		// Filters the digits they may not be locked members
		// when they matches exactly appeared (size - 1) times.
		scoped var lockedMembers = GetLockedMembers(in baseCells, in targetCells, ref lockedMemberDigitsMask);

		// Check whether at least one digit appeared in base cells isn't locked
		// and doesn't satisfy the (size - 1) rule.
		var atLeastOneDigitIsNotLockedAndNotSatisfyCrosslineAppearingRule = false;
		foreach (var filteredDigit in (Mask)(baseCellsDigitsMask & ~lockedMemberDigitsMask))
		{
			if ((digitsMaskExactlySizeMinusOneTimes >> filteredDigit & 1) == 0)
			{
				atLeastOneDigitIsNotLockedAndNotSatisfyCrosslineAppearingRule = true;
				break;
			}
		}
		if (atLeastOneDigitIsNotLockedAndNotSatisfyCrosslineAppearingRule)
		{
			return null;
		}

		if (CheckMirrorSync(
			ref context, grid, in baseCells, in targetCells, in crossline, baseCellsDigitsMask,
			housesMask, chuteIndex, digitsMaskAppearedInCrossline
		) is { } mirrorSyncTypeStep)
		{
			return mirrorSyncTypeStep;
		}

		switch (PopCount((uint)lockedMemberDigitsMask))
		{
			case 1 or 2:
			{
				if (CheckJeLockedMember(
					ref context, grid, in baseCells, in targetCells, in crossline, baseCellsDigitsMask,
					lockedMembers, chuteIndex, groupsOfTargetCells, out _, out var lockedDigitsMask
				) is { } lockedMemberTypeStep)
				{
					return lockedMemberTypeStep;
				}

				// Check whether the locked members are really used.
				// If not, we should check for them, whether they are appeared in cross-line cells in (size - 1) times.
				var lockedMembersAreSatisfySizeMinusOneRule = true;
				foreach (var digit in lockedDigitsMask)
				{
					if (!grid.ExactlyAppearingTimesWith(digit, in crossline, size - 1))
					{
						// The current digit can be filled in cross-line cells at most (size - 1) times.
						lockedMembersAreSatisfySizeMinusOneRule = false;
						break;
					}
				}
				if (lockedMembersAreSatisfySizeMinusOneRule)
				{
					goto FallThrough;
				}
				break;
			}
#pragma warning disable format
			case 0: FallThrough:
			{
				if (CheckBaseJeOrSe(
					ref context, grid, in baseCells, in targetCells, -1, in crossline, baseCellsDigitsMask,
					housesMask, out var inferredTargetConjugatePairs
				) is { } baseTypeStep)
				{
					return baseTypeStep;
				}

				if (CheckMirror(
					ref context, grid, in baseCells, in targetCells, in crossline, baseCellsDigitsMask,
					isRow, chuteIndex, housesMask
				) is { } mirrorTypeStep)
				{
					return mirrorTypeStep;
				}

				if (CheckAdjacentTarget(
					ref context, grid, in baseCells, in targetCells, in crossline, baseCellsDigitsMask,
					isRow, chuteIndex, housesMask
				) is { } singleMirrorTypeStep)
				{
					return singleMirrorTypeStep;
				}

				if (CheckIncompatiblePair(
					ref context, grid, in baseCells, in targetCells, in crossline, baseCellsDigitsMask,
					delta, housesMask, digitsMaskAppearedInCrossline, out var inferredTargetPairMask
				) is { } incompatiblePairTypeStep)
				{
					return incompatiblePairTypeStep;
				}

				if (CheckTargetPair(
					ref context, grid, in baseCells, in targetCells, in crossline, baseCellsDigitsMask,
					inferredTargetPairMask, delta, housesMask, inferredTargetConjugatePairs
				) is { } targetPairTypeStep)
				{
					return targetPairTypeStep;
				}

				if (CheckGeneralizedFish(
					ref context, grid, in baseCells, in targetCells, in crossline, baseCellsDigitsMask,
					inferredTargetPairMask, delta, isRow, housesMask
				) is { } generalizedFishTypeStep)
				{
					return generalizedFishTypeStep;
				}

				if (CheckMirrorAlmostHiddenSet(
					ref context, grid, in baseCells, in targetCells, in crossline, baseCellsDigitsMask,
					delta, isRow, housesMask, chuteIndex
				) is { } mirrorAhsTypeStep)
				{
					return mirrorAhsTypeStep;
				}

				break;
			}
#pragma warning restore format
		}

		return null;
	}

	/// <summary>
	/// The core method to check for Senior Exocet sub-types.
	/// </summary>
	/// <inheritdoc cref="CollectJuniorExocets(ref AnalysisContext, ref readonly Grid, ref readonly CellMap, ref readonly CellMap, ReadOnlySpan{TargetCellsGroup}, ref readonly CellMap, Mask, HouseMask, bool, Count, Offset, Offset)"/>
	private static ExocetStep? CollectSeniorExocets(
		scoped ref AnalysisContext context,
		scoped ref readonly Grid grid,
		scoped ref readonly CellMap baseCells,
		scoped ref readonly CellMap targetCells,
		scoped ReadOnlySpan<TargetCellsGroup> groupsOfTargetCells,
		scoped ref readonly CellMap crossline,
		scoped ref readonly CellMap housesCells,
		scoped ref readonly CellMap chuteCells,
		Mask baseCellsDigitsMask,
		HouseMask housesMask,
		bool isRow,
		Count size,
		Offset delta,
		Offset chuteIndex
	)
	{
		// Try to fetch all possible endo-target cells if worth.
		// Check whether cross-line non-empty cells contains digits appeared in base cells.
		// If so, they will be endo-target cells.
		// The maximum possible number of appearing times is 2, corresponding to the real target cells count.
		var lockedDigitsMask = (Mask)0;
		foreach (var cell in crossline)
		{
			var digit = grid.GetDigit(cell);
			if ((baseCellsDigitsMask >> digit & 1) != 0)
			{
				lockedDigitsMask |= (Mask)(1 << digit);
			}
		}

		switch (PopCount((uint)lockedDigitsMask))
		{
			case 0:
			{
				var endoTargetCells = CellMap.Empty;

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
						if (grid.ExactlyAppearingTimesWith(digit, housesCells - chuteCells - cell, size - 1))
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
					return null;
				}

				foreach (var endoTargetCell in endoTargetCells)
				{
					if (CheckBaseJeOrSe(
						ref context, grid, in baseCells, in targetCells, endoTargetCell, in crossline,
						baseCellsDigitsMask, housesMask, out _
					) is { } baseTypeStep)
					{
						return baseTypeStep;
					}
				}
				break;
			}
			case 1:
			{
				// Check for maximum times can be appeared in cross-line cells.
				// Due to consideration on locked members, we may not handle for them.
				var allDigitsCanBeFilledExactlySizeMinusOneTimes = true;
				foreach (var digit in (Mask)(baseCellsDigitsMask & ~lockedDigitsMask))
				{
					if (grid.ExactlyAppearingTimesWith(digit, housesCells - chuteCells, size - 1))
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
					return null;
				}

				if (CheckLockedMemberSe(
					ref context, grid, in baseCells, in targetCells, in crossline, baseCellsDigitsMask,
					TrailingZeroCount(lockedDigitsMask)
				) is { } lockedMemberTypeStep)
				{
					return lockedMemberTypeStep;
				}

				break;
			}
			//case 2:
			//{
			//	// TODO: Fixed base digits due to fixed target cell digits.
			//	break;
			//}
		}

		return null;
	}

	/// <summary>
	/// The core method to check for Weak Exocet sub-types.
	/// </summary>
	/// <inheritdoc cref="CollectJuniorExocets(ref AnalysisContext, ref readonly Grid, ref readonly CellMap, ref readonly CellMap, ReadOnlySpan{TargetCellsGroup}, ref readonly CellMap, Mask, HouseMask, bool, Count, Offset, Offset)"/>
	private static ExocetStep? CollectWeakExocets(
		scoped ref AnalysisContext context,
		scoped ref readonly Grid grid,
		scoped ref readonly CellMap baseCells,
		scoped ReadOnlySpan<TargetCellsGroup> groupsOfTargetCells,
		scoped ref readonly CellMap crossline,
		Mask baseCellsDigitsMask,
		HouseMask housesMask,
		bool isRow,
		Count size,
		Offset chuteIndex
	)
	{
		// See following links to learn more information about this technique:
		//
		//   * http://forum.enjoysudoku.com/weak-exocet-t39651.html
		//   * https://tieba.baidu.com/p/7637653732
		//
		// The conditions of this technique is very hard to be described so... here is an example:
		//
		//   ,--------------------------,------------------------,-----------------------,
		//   | 1278    237     m1237    | 23468   5      3468    | 2678   2478   9       |
		//   | a4      2579    279      | *1      268    689     | 25678  a3     568     |
		//   | 2589    a6      239      | 7       2348   3489    | a1     2458   458     |
		//   :--------------------------+------------------------+-----------------------:
		//   | 1267    2347    5        | 9       13468  13468   | 2368   1248   T1346-8 |
		//   | 1269    8       123469   | B346    7      B1346   | 23569  12459  13456   |
		//   | 169     349     T1346-9  | 5       13468  2       | 3689   1489   7       |
		//   :--------------------------+------------------------+-----------------------:
		//   | a3      2579-4  2479     | 248     1248   14578   | 5789   a6     158     |
		//   | 567     457     8        | 346     9      1346-57 | 357    157    2       |
		//   | 2579-6  a1      2679     | 2368    2368   35678   | a4     5789   358     |
		//   '--------------------------'------------------------'-----------------------'
		//                     crossline          crossline                      crossline
		//
		// (Single-line text code: ....5...94..1...3..6.7..1....59......8..7.......5.2..73......6...8.9...2.1....4..)
		// Here is an exocet with digits [1, 3, 4, 6], base: r5c46, target: r4c9 & r6c3, crossline cells: r123789c359.
		//
		// If we don't have the value digit 1 from the cell r2c4 (marked as '*'), the cell r1c3 must be an value digit not be 1, 3, 4 and 6
		// to ensure every digit [1, 3, 4, 6] appearing at most 2 times (size = 3, 3 - 1 = 2).
		// However, this cell is an empty cell, which means it may cause an extra chance to arrange the filling the digit 1,
		// so here the digit 1 may contain 3 times to be filled with the cross-line cells if so.
		// In this way a standard JE may not be formed because of the digit 1.
		// We should use an extra digit 1 to make this rule to be "balanced" again.
		// If the value digit 1 (r2c4) exists in a different block as cell r1c3, the JE can also be formed,
		// with all standard JE rule being obeyed (even adjacent target, i.e. the mirror rules).
		// In addition, this exocet pattern may raise extra eliminations to be used, which will not introduced by a standard JE.
		// This technique is called "Weak Exocet", having been found and proved by Borescoper, my friend.
		//
		// However, a weak exocet should rely on some extra conditions:
		//
		//   1) All four digits in base should contain value representations in 4 blocks cross-line cells used.
		//   2) All value cells using such four digits should appear twice for each.
		//   3) For each value digit, it should be paired with another digit to make a group; the last digit will be the other group.
		//   4) Such two groups should be filled into four blocks mentioned in condition 1) with diagonal-distributed blocks.
		//   5) The missing-value cell shouldn't lie in any blocks mentioned in condition 1) and 4).
		//   6) An extra value digit should be placed in a block that 4 blocks mentioned above not covered,
		//      but not in a same chute with base cells.
		//   7) The missing-value cell cannot lie in a same line with that given value digit condition 6) mentioned.
		//
		// A weak exocet may contain some extra eliminations that a normal JE pattern (even not containing the missing-value cell) doesn't have.

		// Target cells must be 2.
		if (groupsOfTargetCells.Length != 2)
		{
			return null;
		}

		// Try to get cross-line cells, count up the positions of the value cells in the cross-line cells.
		if (crossline - EmptyCells is not { Count: 5 or 6, RowMask: var rowsCovered, ColumnMask: var columnsCovered })
		{
			return null;
		}

		// Check whether such 5 and 6 cells are in a 2 * 3 "rectangle".
		if ((isRow, PopCount((uint)rowsCovered), PopCount((uint)columnsCovered)) is not ((false, 2, 3) or (true, 3, 2)))
		{
			return null;
		}

		// Check whether the rows or columns are spanned 3 different chute in the same direction of the cross-line cells.
		var spanningLinesMask = isRow ? rowsCovered : columnsCovered;
		var spanningLinesChute1 = spanningLinesMask & 7;
		var spanningLinesChute2 = spanningLinesMask >> 3 & 7;
		var spanningLinesChute3 = spanningLinesMask >> 6 & 7;
		if ((PopCount((uint)spanningLinesChute1), PopCount((uint)spanningLinesChute2), PopCount((uint)spanningLinesChute3)) is not (1, 1, 1))
		{
			return null;
		}

		// Try to fetch the missing-value cell.
		// Please note here is an exceception: If the missing-value cell isn't missing the digit, it can also be a weak exocet
		// if the digit belongs to digits appeared in base cells, and such cells should only contain 1. 2 or more cells is invalid.
		var missingValueCell = -1;
		foreach (var row in rowsCovered)
		{
			foreach (var column in columnsCovered)
			{
				var cell = row * 9 + column;
				if (grid.GetState(cell) == CellState.Empty || (baseCellsDigitsMask >> grid.GetDigit(cell) & 1) != 0)
				{
					if (missingValueCell != -1)
					{
						// At least 2 cells satisfied this condition, which is invalid in a weak exocet.
						return null;
					}

					missingValueCell = cell;
				}
			}
		}
		if (missingValueCell == -1)
		{
			return null;
		}

		// Check the outside value digit, whether the digit doesn't share a same house as the missing-value cell.
		var (baseCellUncoveredBlocksMaskCoveringCrossline, baseCellCoveredBlocksMaskCoveringCrossline) = ((Mask)0, (Mask)0);
		var (baseCellUncoveredBlockCells, baseCellCoveredBlockCells) = (CellMap.Empty, CellMap.Empty);
		foreach (var (_, chuteCells, _, _) in Chutes[isRow ? ..3 : 3..])
		{
			if (chuteCells & baseCells)
			{
				foreach (var block in baseCellCoveredBlocksMaskCoveringCrossline = (chuteCells & crossline).BlockMask)
				{
					baseCellCoveredBlockCells |= HousesMap[block];
				}
				foreach (var block in baseCellUncoveredBlocksMaskCoveringCrossline = (Mask)(crossline.BlockMask & ~baseCellCoveredBlocksMaskCoveringCrossline))
				{
					baseCellUncoveredBlockCells |= HousesMap[block];
				}
				break;
			}
		}

		// Check whether the missing-value cell isn't in the chute that base cells can cover.
		if (baseCellCoveredBlockCells.Contains(missingValueCell))
		{
			return null;
		}

		// Now fetch the value cell outside the blocks of the missing-value cell.
		const Cell invalidPos = -2;
		Cell valueDigitCell;
		using (scoped var valueDigitsPos = new ValueList<Cell>(6))
		{
			foreach (var block in baseCellCoveredBlocksMaskCoveringCrossline)
			{
				foreach (var cell in HousesMap[block] - EmptyCells - crossline)
				{
					if ((baseCellsDigitsMask >> grid.GetDigit(cell) & 1) != 0 && !PeersMap[missingValueCell].Contains(cell)
						&& !valueDigitsPos.TryAdd(cell))
					{
						// Exceeds the adding limit.
						return null;
					}
				}
			}

			valueDigitCell = valueDigitsPos switch { [] => -1, [var vdc] => vdc, _ => invalidPos };
		}
		if (valueDigitCell == invalidPos)
		{
			// This case is invalid to be checked.
			return null;
		}

		// Check whether the missing-value cell has a value, and they are same.
		if (valueDigitCell != -1 && grid.GetDigit(missingValueCell) is var missingValueCellDigit and not -1
			&& missingValueCellDigit != grid.GetDigit(valueDigitCell))
		{
			return null;
		}

		// Check for cross-line (size - 1) rule.
		// Due to the reason of the exocet pattern forming rule, the missing-value cell may cause some digits appeared in base cells
		// exceed the (size - 1) times appearing.
		var sizeMinusOneRule = true;
		var exceptionDigit = valueDigitCell == -1 ? -1 : grid.GetDigit(valueDigitCell);
		foreach (var digit in baseCellsDigitsMask)
		{
			if (grid.ExactlyAppearingTimesWith(digit, crossline - missingValueCell, exceptionDigit != -1 && exceptionDigit == digit ? size - 1 : size))
			{
				// The current digit can be filled in cross-line cells at most (size - 1) times.
				sizeMinusOneRule = false;
				break;
			}
		}
		if (!sizeMinusOneRule)
		{
			return null;
		}

		// Check whether the value cell digit isn't covered in the same line as value cells in cross-line.
		scoped var coveredLinesForValueCells = (isRow ? columnsCovered << 18 : rowsCovered << 9).GetAllSets();
		var intersectedLinesForSuchLastCells = (HousesMap[coveredLinesForValueCells[0]] | HousesMap[coveredLinesForValueCells[1]]) - crossline;
		if (valueDigitCell != -1 && intersectedLinesForSuchLastCells.Contains(valueDigitCell))
		{
			return null;
		}

		// Check whether lines of value cells don't contain the digits appeared in base cells, of value representation.
		var intersectedLinesContainAnyValueCellDigitsApperaedInBaseCells = false;
		foreach (var cell in intersectedLinesForSuchLastCells)
		{
			if ((baseCellsDigitsMask >> grid.GetDigit(cell) & 1) != 0)
			{
				intersectedLinesContainAnyValueCellDigitsApperaedInBaseCells = true;
				break;
			}
		}
		if (intersectedLinesContainAnyValueCellDigitsApperaedInBaseCells)
		{
			return null;
		}

		// Then check for digits of values in houses that base cell chute does not cover.
		// All four blocks should contain 4 kinds of digits of value representation, with diagonal-distributed rule.
		scoped var blocks = baseCellUncoveredBlocksMaskCoveringCrossline.GetAllSets();
		var lastSixteenCells = (HousesMap[blocks[0]] | HousesMap[blocks[1]] | HousesMap[blocks[2]] | HousesMap[blocks[3]]) - crossline - intersectedLinesForSuchLastCells;
		var isDiagonallyDistributed = true;
		foreach (var ((a1, da1), (a2, da2)) in (((0, 3), (1, 2)), ((1, 2), (0, 3))))
		{
			var ca1 = (Mask)(grid[(HousesMap[blocks[a1]] & lastSixteenCells) - EmptyCells, true] & baseCellsDigitsMask);
			var cda1 = (Mask)(grid[(HousesMap[blocks[da1]] & lastSixteenCells) - EmptyCells, true] & baseCellsDigitsMask);
			var ca2 = (Mask)(grid[(HousesMap[blocks[a2]] & lastSixteenCells) - EmptyCells, true] & baseCellsDigitsMask);
			var cda2 = (Mask)(grid[(HousesMap[blocks[da2]] & lastSixteenCells) - EmptyCells, true] & baseCellsDigitsMask);
			if (ca1 != cda1 || ca2 != cda2
				|| (Mask)(ca1 | ca2) != baseCellsDigitsMask
				|| (PopCount((uint)ca1), PopCount((uint)cda1), PopCount((uint)ca2), PopCount((uint)cda2)) is not (2, 2, 2, 2))
			{
				isDiagonallyDistributed = false;
				break;
			}
		}
		if (!isDiagonallyDistributed)
		{
			return null;
		}

		// A weak exocet is formed. Phew! Now check for eliminations.
		if (CheckBaseWeak(
			ref context, grid, in baseCells, groupsOfTargetCells, in crossline, valueDigitCell, missingValueCell,
			baseCellsDigitsMask
		) is { } baseWeakTypeStep)
		{
			return baseWeakTypeStep;
		}

		if (CheckWeakAdjacentTarget(
			ref context, grid, in baseCells, groupsOfTargetCells, in crossline, valueDigitCell, missingValueCell,
			baseCellsDigitsMask, chuteIndex
		) is { } adjacentTargetTypeStep)
		{
			return adjacentTargetTypeStep;
		}

		if (CheckWeakExocetSlash(
			ref context, grid, in baseCells, groupsOfTargetCells, in crossline, in lastSixteenCells, valueDigitCell, missingValueCell,
			baseCellsDigitsMask, isRow, chuteIndex, out var cellsCanBeEliminated
		) is { } slashTypeStep)
		{
			return slashTypeStep;
		}

		if (CheckWeakExocetBzRectangle(
			ref context, grid, in baseCells, groupsOfTargetCells, in crossline, in lastSixteenCells, in cellsCanBeEliminated,
			valueDigitCell, missingValueCell, baseCellsDigitsMask, isRow, chuteIndex
		) is { } bzRectangleTypeStep)
		{
			return bzRectangleTypeStep;
		}

		return null;
	}

	/// <summary>
	/// The core method to check for Double Exocet sub-types.
	/// </summary>
	/// <inheritdoc cref="CollectJuniorExocets(ref AnalysisContext, ref readonly Grid, ref readonly CellMap, ref readonly CellMap, ReadOnlySpan{TargetCellsGroup}, ref readonly CellMap, Mask, HouseMask, bool, Count, Offset, Offset)"/>
	private static ExocetStep? CollectDoubleExocets(
		scoped ref AnalysisContext context,
		scoped ref readonly Grid grid,
		scoped ref readonly CellMap baseCells,
		scoped ref readonly CellMap targetCells,
		scoped ReadOnlySpan<TargetCellsGroup> groupsOfTargetCells,
		scoped ref readonly CellMap crossline,
		scoped ref readonly CellMap housesEmptyCells,
		Mask baseCellsDigitsMask,
		HouseMask housesMask,
		bool isRow,
		Count size,
		Offset delta,
		Offset chuteIndex
	)
	{
		// Before the checking we have a normal JE pattern. Now we should check for the other one, to form a double JE.
		// A double JE is not two combined JEs. A double JE can contain extra eliminations that won't be formed in a single JE pattern.

		// Check whether the number of target cells is 2.
		if (baseCells.Count != targetCells.Count || targetCells.Count != 2)
		{
			return null;
		}

		// Check for the other pair of base cells.
		var blocksMask = AllBlocksMask & ~baseCells.BlockMask & ~crossline.BlockMask;
		var lastCells = CellMap.Empty;
		foreach (var block in blocksMask)
		{
			lastCells |= HousesMap[block];
		}
		lastCells -= baseCells.PeerIntersection;
		foreach (var line in isRow ? crossline.RowMask << 9 : crossline.ColumnMask << 18)
		{
			lastCells -= HousesMap[line];
		}

		// Iterate on each intersection to get the other side of base cells.
		foreach (ref readonly var intersection in MinilinesGroupedByChuteIndex[chuteIndex].AsReadOnlySpan())
		{
			var theOtherBaseCells = intersection & lastCells;
			if (theOtherBaseCells.Count != 2)
			{
				// The other side of cells should contain the same number of the current base cells.
				continue;
			}

			var theOtherBaseCellsDigitsMask = grid[in theOtherBaseCells];
			if (theOtherBaseCellsDigitsMask != baseCellsDigitsMask)
			{
				// The other side of base cells should hold same digits.
				continue;
			}

			// Try to calculate the target cells.
			var theOtherTargetCells = (Chutes[chuteIndex].Cells & housesEmptyCells & EmptyCells) - theOtherBaseCells.PeerIntersection;
			var theOtherTargetCellsDigitsMask = grid[in theOtherTargetCells];

			// Check whether all digits appeared in base cells can be filled in target empty cells.
			if ((theOtherTargetCellsDigitsMask & baseCellsDigitsMask) != baseCellsDigitsMask)
			{
				// They are out of relation.
				continue;
			}

			if (targetCells & theOtherTargetCells)
			{
				// Two target cells pair cannot intersect with each other.
				continue;
			}

			// If the current accumulator has already collected for the same-cell steps, we won't add it.
			if (!context.OnlyFindOne)
			{
				var alreadyContain = false;
				foreach (var s in context.Accumulator)
				{
					if (s is not (DoubleExocetBaseStep or DoubleExocetGeneralizedFishStep))
					{
						continue;
					}

					var d = (dynamic)s;
					var a = (CellMap)d.BaseCells | (CellMap)d.BaseCellsTheOther | (CellMap)d.TargetCells | (CellMap)d.TargetCellsTheOther;
					var b = baseCells | theOtherBaseCells | targetCells | theOtherTargetCells;
					if (a == b)
					{
						alreadyContain = true;
					}
				}
				if (alreadyContain)
				{
					return null;
				}
			}

			// A Double JE is found. Now check for eliminations.
			if (CheckDoubleBase(
				ref context, grid, in baseCells, in targetCells, in theOtherBaseCells, in theOtherTargetCells, in crossline,
				baseCellsDigitsMask, housesMask
			) is { } doubleBaseTypeStep)
			{
				return doubleBaseTypeStep;
			}

			if (CheckDoubleGeneralizedFish(
				ref context, grid, in baseCells, in targetCells, in theOtherBaseCells, in theOtherTargetCells, in crossline,
				size, isRow, baseCellsDigitsMask, housesMask
			) is { } unifishTypeStep)
			{
				return unifishTypeStep;
			}
		}

		return null;
	}

	private static ExocetBaseStep? CheckBaseJeOrSe(
		scoped ref AnalysisContext context,
		Grid grid,
		scoped ref readonly CellMap baseCells,
		scoped ref readonly CellMap targetCells,
		Cell endoTargetCell,
		scoped ref readonly CellMap crossline,
		Mask baseCellsDigitsMask,
		HouseMask housesMask,
		out ReadOnlySpan<Conjugate> inferredTargetConjugatePairs
	)
	{
		var (conclusions, conjugatePairs) = (new List<Conclusion>(), new List<Conjugate>(2));
		switch (baseCells, targetCells, endoTargetCell)
		{
			case ({ Count: 1 }, [var targetCell], -1):
			{
				foreach (var digit in (Mask)(grid.GetCandidates(targetCell) & ~baseCellsDigitsMask))
				{
					conclusions.Add(new(Elimination, targetCell, digit));
				}
				break;
			}
			case ({ Count: 1 }, { Count: 2 }, -1):
			{
				var digitsMask = (Mask)(grid[in targetCells, false, GridMaskMergingMethod.And] & ~baseCellsDigitsMask);
				if (digitsMask == 0)
				{
					break;
				}

				foreach (var coveredLine in targetCells.CoveredHouses)
				{
					foreach (var conjugatePairDigit in digitsMask)
					{
						if ((HousesMap[coveredLine] & CandidatesMap[conjugatePairDigit]) != targetCells)
						{
							continue;
						}

						// This digit is a conjugate pair.
						foreach (var cell in targetCells)
						{
							foreach (var digit in (Mask)(grid.GetCandidates(cell) & ~baseCellsDigitsMask & ~(1 << conjugatePairDigit)))
							{
								conclusions.Add(new(Elimination, cell, digit));
							}
						}

						conjugatePairs.Add(new(in targetCells, conjugatePairDigit));
						break;
					}
				}
				break;
			}
			case (_, { Count: 2 }, _):
			{
				foreach (var cell in endoTargetCell == -1 ? targetCells : targetCells + endoTargetCell)
				{
					foreach (var digit in (Mask)(grid.GetCandidates(cell) & ~baseCellsDigitsMask))
					{
						conclusions.Add(new(Elimination, cell, digit));
					}
				}
				break;
			}
			case (_, { Count: 3 or 4 }, -1):
			{
				foreach (var (_, cellsInThisBlock) in GroupTargets(in targetCells, housesMask))
				{
					switch (cellsInThisBlock.Count)
					{
						case 1:
						{
							foreach (var cell in cellsInThisBlock)
							{
								foreach (var digit in (Mask)(grid.GetCandidates(cell) & ~baseCellsDigitsMask))
								{
									conclusions.Add(new(Elimination, cell, digit));
								}
							}
							break;
						}
						case 2:
						{
							var digitsMask = (Mask)(grid[in cellsInThisBlock, false, GridMaskMergingMethod.And] & ~baseCellsDigitsMask);
							if (digitsMask == 0)
							{
								break;
							}

							foreach (var coveredLine in cellsInThisBlock.CoveredHouses)
							{
								foreach (var conjugatePairDigit in digitsMask)
								{
									if ((HousesMap[coveredLine] & CandidatesMap[conjugatePairDigit]) != cellsInThisBlock)
									{
										continue;
									}

									// This digit is a conjugate pair.
									foreach (var cell in cellsInThisBlock)
									{
										foreach (var digit in (Mask)(grid.GetCandidates(cell) & ~baseCellsDigitsMask & ~(1 << conjugatePairDigit)))
										{
											conclusions.Add(new(Elimination, cell, digit));
										}
									}

									conjugatePairs.Add(new(in cellsInThisBlock, conjugatePairDigit));
									break;
								}
							}
							break;
						}
					}
				}
				break;
			}
		}

		// Try to get conjugate pairs in target cells.
		inferredTargetConjugatePairs = conjugatePairs.ToArray();

		if (conclusions.Count == 0)
		{
			// No eliminations found.
			return null;
		}

		var step = new ExocetBaseStep(
			[.. conclusions],
			[
				[
					.. from cell in baseCells select new CellViewNode(WellKnownColorIdentifier.Normal, cell),
					.. from cell in targetCells select new CellViewNode(WellKnownColorIdentifier.Auxiliary1, cell),
					.. endoTargetCell != -1 ? [new CellViewNode(WellKnownColorIdentifier.Auxiliary1, endoTargetCell)] : (ViewNode[])[],
					.. from cell in crossline - endoTargetCell select new CellViewNode(WellKnownColorIdentifier.Auxiliary2, cell),
					..
					from conjugatePair in conjugatePairs
					from cell in conjugatePair.Map
					select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary3, cell * 9 + conjugatePair.Digit),
					..
					from cell in baseCells
					from digit in grid.GetCandidates(cell)
					select new CandidateViewNode(WellKnownColorIdentifier.Normal, cell * 9 + digit),
					..
					from cell in crossline - endoTargetCell
					where grid.GetState(cell) == CellState.Empty
					from digit in (Mask)(grid.GetCandidates(cell) & baseCellsDigitsMask)
					select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, cell * 9 + digit),
					//.. from house in housesMask select new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, house)
				]
			],
			context.PredefinedOptions,
			baseCellsDigitsMask,
			in baseCells,
			in targetCells,
			endoTargetCell != -1 ? [endoTargetCell] : [],
			in crossline,
			[.. conjugatePairs]
		);
		if (context.OnlyFindOne)
		{
			return step;
		}

		context.Accumulator.Add(step);
		return null;
	}

	private static ExocetMirrorStep? CheckMirror(
		scoped ref AnalysisContext context,
		Grid grid,
		scoped ref readonly CellMap baseCells,
		scoped ref readonly CellMap targetCells,
		scoped ref readonly CellMap crossline,
		Mask baseCellsDigitsMask,
		bool isRow,
		Offset chuteIndex,
		HouseMask housesMask
	)
	{
		// Mirror conjugate pair cannot be used for same-side target cells.
		if (targetCells.InOneHouse(out _))
		{
			return null;
		}

		var conclusions = new List<Conclusion>();
		var conjugatePairs = new List<Conjugate>(2);
		scoped var cellGroups = GroupTargets(in targetCells, housesMask);
		if (cellGroups.Length != 2)
		{
			return null;
		}

		foreach (ref readonly var cellGroup in cellGroups)
		{
			if (cellGroup.Count == 2)
			{
				// If the number of target cells in one side is 2, we cannot determine which one is correct.
				continue;
			}

			foreach (var targetCell in cellGroup)
			{
				var theOtherTwoCells = GetMirrorCells(targetCell, chuteIndex, out _);
				var theOtherEmptyCells = theOtherTwoCells & EmptyCells;
				if (!theOtherEmptyCells)
				{
					// The current miniline cannot contain any eliminations.
					continue;
				}

				var otherCellsDigitsMask = grid[in theOtherEmptyCells];
				foreach (var house in theOtherEmptyCells.CoveredHouses)
				{
					// Check whether the current house has a conjugate pair in the current cells.
					foreach (var digit in otherCellsDigitsMask)
					{
						var cellsContainingDigit = (CandidatesMap[digit] & HousesMap[house]) - targetCell;
						if (cellsContainingDigit != theOtherEmptyCells)
						{
							continue;
						}

						// Here a conjugate pair will be formed.
						// Now check for eliminations.
						foreach (var elimCell in theOtherEmptyCells)
						{
							foreach (var elimDigit in (Mask)(grid.GetCandidates(elimCell) & ~baseCellsDigitsMask & ~(1 << digit)))
							{
								conclusions.Add(new(Elimination, elimCell, elimDigit));
							}
						}

						conjugatePairs.Add(new(in theOtherEmptyCells, digit));
					}
				}
			}
		}
		if (conclusions.Count == 0)
		{
			// No eliminations found.
			return null;
		}

		var step = new ExocetMirrorStep(
			[.. conclusions],
			[
				[
					.. from cell in baseCells select new CellViewNode(WellKnownColorIdentifier.Normal, cell),
					.. from cell in targetCells select new CellViewNode(WellKnownColorIdentifier.Auxiliary1, cell),
					.. from cell in crossline select new CellViewNode(WellKnownColorIdentifier.Auxiliary2, cell),
					..
					from cell in baseCells
					from d in grid.GetCandidates(cell)
					select new CandidateViewNode(WellKnownColorIdentifier.Normal, cell * 9 + d),
					..
					from cell in crossline
					where grid.GetState(cell) == CellState.Empty
					from d in (Mask)(grid.GetCandidates(cell) & baseCellsDigitsMask)
					select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, cell * 9 + d),
					..
					from conjugatePair in conjugatePairs
					from cell in conjugatePair.Map
					select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary3, cell * 9 + conjugatePair.Digit),
					//.. from house in housesMask select new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, house)
				]
			],
			context.PredefinedOptions,
			baseCellsDigitsMask,
			in baseCells,
			in targetCells,
			[],
			in crossline,
			[.. conjugatePairs]
		);
		if (context.OnlyFindOne)
		{
			return step;
		}

		context.Accumulator.Add(step);
		return null;
	}

	private static JuniorExocetAdjacentTargetStep? CheckAdjacentTarget(
		scoped ref AnalysisContext context,
		Grid grid,
		scoped ref readonly CellMap baseCells,
		scoped ref readonly CellMap targetCells,
		scoped ref readonly CellMap crossline,
		Mask baseCellsDigitsMask,
		bool isRow,
		Offset chuteIndex,
		HouseMask housesMask
	)
	{
		// Adjacent target cannot be used for same-side target cells.
		if (targetCells.InOneHouse(out _))
		{
			return null;
		}

		var conclusions = new List<Conclusion>();
		var singleMirrors = CellMap.Empty;
		foreach (ref readonly var cellGroup in GroupTargets(in targetCells, housesMask))
		{
			if (cellGroup.Count == 2)
			{
				// This side contain 2 target empty cells. We cannot conclude for this case.
				continue;
			}

			foreach (var targetCell in cellGroup)
			{
				var mirrorCells = GetMirrorCells(targetCell, chuteIndex, out _);
				if ((mirrorCells & EmptyCells) is not [var theOnlyMirrorCell])
				{
					// The mirror cells contain not 1 cell, it may not be included in this type.
					continue;
				}

				// Try to get the target cell that is not share with a same block with this mirror cell.
				var theOtherTargetCells = targetCells - targetCell;
				if (theOtherTargetCells is not [var theOtherTargetCell])
				{
					// The number of the other side of target cell is not 1, we cannot conclude for that.
					continue;
				}

				// Check for the only mirror cell, determining whether the cell contains an arbitrary extra digits.
				var digitsInMirrorCell = grid.GetCandidates(theOnlyMirrorCell);
				var elimDigitsFromTheOnlyMirrorCell = (Mask)(digitsInMirrorCell & ~baseCellsDigitsMask);

				// Check for the containing digits in mirror cells, and fetch which digits are appeared in base cells.
				// Such digits will be sync'ed with the other target cell.
				var containedDigitsAppearedInBaseCellsInMirror = (Mask)(digitsInMirrorCell & baseCellsDigitsMask);
				var elimDigitsFromTheOtherTargetCell = (Mask)(grid.GetCandidates(theOtherTargetCell) & ~containedDigitsAppearedInBaseCellsInMirror);

				// Try to fetch eliminations.
				if (elimDigitsFromTheOnlyMirrorCell != 0)
				{
					foreach (var elimDigit in elimDigitsFromTheOnlyMirrorCell)
					{
						conclusions.Add(new(Elimination, theOnlyMirrorCell, elimDigit));
					}
				}
				if (elimDigitsFromTheOtherTargetCell != 0)
				{
					foreach (var elimDigit in elimDigitsFromTheOtherTargetCell)
					{
						conclusions.Add(new(Elimination, theOtherTargetCell, elimDigit));
					}
				}

				singleMirrors.Add(theOnlyMirrorCell);
			}
			if (conclusions.Count == 0 || !singleMirrors)
			{
				// No eliminations found.
				return null;
			}

			var step = new JuniorExocetAdjacentTargetStep(
				[.. conclusions],
				[
					[
						.. from cell in baseCells select new CellViewNode(WellKnownColorIdentifier.Normal, cell),
						.. from cell in targetCells select new CellViewNode(WellKnownColorIdentifier.Auxiliary1, cell),
						.. from cell in crossline select new CellViewNode(WellKnownColorIdentifier.Auxiliary2, cell),
						.. from cell in singleMirrors select new CellViewNode(WellKnownColorIdentifier.Auxiliary3, cell),
						..
						from cell in baseCells
						from d in grid.GetCandidates(cell)
						select new CandidateViewNode(WellKnownColorIdentifier.Normal, cell * 9 + d),
						..
						from cell in crossline
						where grid.GetState(cell) == CellState.Empty
						from d in (Mask)(grid.GetCandidates(cell) & baseCellsDigitsMask)
						select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, cell * 9 + d),
						//.. from house in housesMask select new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, house)
					]
				],
				context.PredefinedOptions,
				baseCellsDigitsMask,
				in baseCells,
				in targetCells,
				[],
				in crossline,
				in singleMirrors
			);
			if (context.OnlyFindOne)
			{
				return step;
			}

			context.Accumulator.Add(step);
		}

		return null;
	}

	private static JuniorExocetIncompatiblePairStep? CheckIncompatiblePair(
		scoped ref AnalysisContext context,
		Grid grid,
		scoped ref readonly CellMap baseCells,
		scoped ref readonly CellMap targetCells,
		scoped ref readonly CellMap crossline,
		Mask baseCellsDigitsMask,
		Offset delta,
		HouseMask housesMask,
		Mask digitsMaskAppearedInCrossline,
		out Mask inferredTargetPairMask
	)
	{
		inferredTargetPairMask = 0;

		// This rule can only apply for the case on such conditions:
		//   1) The number of base cells must be 2.
		//   2) The delta value must be 0 (i.e. a standard JE).
		//   3) The cross-line cells contain at least one cell filled with digits appeared in base cells.

		if (digitsMaskAppearedInCrossline != 0)
		{
			return null;
		}

		if (delta != 0)
		{
			return null;
		}

		if (baseCells is not [var base1, var base2])
		{
			return null;
		}

		// Now try to fetch the defining cells. First, try to get uncovered 4 blocks that the final cells should be located in.
		var cellsDoNotCover = CellMap.Empty;
		foreach (var (_, chuteCells, _, _) in Chutes)
		{
			if (chuteCells & baseCells)
			{
				cellsDoNotCover |= chuteCells;
			}
		}
		var lastFourBlocks = ~CellMap.Empty - cellsDoNotCover;
		var lastFourBlocksNotIntersectedWithCrossline = lastFourBlocks - crossline;
		var lastFourBlocksIntersectedWithCrossline = lastFourBlocks & crossline;

		// Try to check for cross-line cells, get all values and its containing cells.
		var valueCellsInLastFourBlocksIntersectedWithCrossline = lastFourBlocksIntersectedWithCrossline - EmptyCells;

		// Determine whether the value cells only span with 2 rows and columns, forming a square shape:
		//
		//   1 . . | 2 . .
		//   . . . | . . .
		//   . . . | . . .
		//   ------+------
		//   2 . . | 1 . .
		//   . . . | . . .
		//   . . . | . . .
		//
		// If not, the rule cannot be formed.
		if (valueCellsInLastFourBlocksIntersectedWithCrossline.Count != 4
			|| PopCount((uint)valueCellsInLastFourBlocksIntersectedWithCrossline.RowMask) != 2
			|| PopCount((uint)valueCellsInLastFourBlocksIntersectedWithCrossline.ColumnMask) != 2)
		{
			return null;
		}

		// Try to fetch the last 16 cells to be checked.
		var lastSixteenCells = lastFourBlocksNotIntersectedWithCrossline;
		foreach (var house in valueCellsInLastFourBlocksIntersectedWithCrossline.RowMask << 9)
		{
			lastSixteenCells -= HousesMap[house];
		}
		foreach (var house in valueCellsInLastFourBlocksIntersectedWithCrossline.ColumnMask << 18)
		{
			lastSixteenCells -= HousesMap[house];
		}

		// Now we have the 16 cells to be checked. Such cells are called "X Region".
		// We should check for value cells, determining which combinations of digits have spanned all 4 blocks the current 16 cells located in.
		scoped var valuesGroupedByBlock = (stackalloc Mask[2]);
		scoped var valueCellsBlocks = lastSixteenCells.BlockMask.GetAllSets();
		foreach (var blockIndex in (0, 1))
		{
			var valueCellsFromBlock1 = (lastSixteenCells & HousesMap[valueCellsBlocks[blockIndex]]) - EmptyCells;
			var valueCellsFromBlock2 = (lastSixteenCells & HousesMap[valueCellsBlocks[3 - blockIndex]]) - EmptyCells;
			var valuesFromBlock1 = MaskOperations.Create(from cell in valueCellsFromBlock1 select grid.GetDigit(cell));
			var valuesFromBlock2 = MaskOperations.Create(from cell in valueCellsFromBlock2 select grid.GetDigit(cell));
			var valuesFromBothBlocks = (Mask)(valuesFromBlock1 & valuesFromBlock2);
			if (valuesFromBothBlocks == 0)
			{
				// It seems no digits will be intersected with two blocks in diagonal direction...
				// The current "X Region" may not contain any possible conclusions. Just return.
				return null;
			}

			// Write down such digits.
			valuesGroupedByBlock[blockIndex] = valuesFromBothBlocks;
		}

		// Then we should check for combinations, determining which combinations are not correct.
		// The rule is, if two different digits from two groups of blocks in a diagonal direction are found,
		// they will not be the final pair of digits appeared in base cells.
		// For example, if the following diagram exists:
		//
		//   1 . . | 2 . .
		//   . . . | . . .
		//   . . . | . . .
		//   ------+------
		//   2 . . | 1 . .
		//   . . . | . . .
		//   . . . | . . .
		//
		// The pair of digits 1 and 2 cannot be the final pair in base cells,
		// meaning the base cells cannot be filled with 1 and 2 at the same time. They are incompatible.
		// Here is the prove (Chinese version):
		//
		//   https://tieba.baidu.com/p/5916787916
		//
		// Which tells us that, if so, the last pattern of exocet may form a deadly pattern.
		scoped var incompatibleCombinationsGroupedByDigit = (stackalloc Mask[9]);
		incompatibleCombinationsGroupedByDigit.Clear();
		foreach (var value1 in valuesGroupedByBlock[0])
		{
			foreach (var value2 in valuesGroupedByBlock[1])
			{
				incompatibleCombinationsGroupedByDigit[value1] |= (Mask)(1 << value2);
				incompatibleCombinationsGroupedByDigit[value2] |= (Mask)(1 << value1);
			}
		}

		// Now check for eliminations.
		var incompatibleCandidates = CandidateMap.Empty;
		var conclusions = new List<Conclusion>();
		var targetCellsDigitsMask = grid[in targetCells];
		foreach (var (elimCell, theOtherCell) in ((base1, base2), (base2, base1)))
		{
			var allDigits = grid.GetCandidates(theOtherCell);
			foreach (var elimDigit in grid.GetCandidates(elimCell))
			{
				if (incompatibleCombinationsGroupedByDigit[elimDigit] == (Mask)(allDigits & ~(1 << elimDigit)))
				{
					conclusions.Add(new(Elimination, elimCell, elimDigit));
					incompatibleCandidates.Add(elimCell * 9 + elimDigit);
				}
			}
		}
		if (conclusions.Count == 0)
		{
			return null;
		}

		// Temporarily remove the eliminated digits, and check whether the last digits in target cells form a pair.
		// Because we know a JE pattern will lead to a conclusion:
		//
		//   "The target cells should be a pair of different digits if a JE formed."
		//
		// we can conclude that the target cells will form a distributed disjointed pair (from concept "Distributed Disjointed Subset", DDS).
		// Then we can remove all digits from the cells that both target cells can see.
		var baseCellsLastDigitsMask = baseCellsDigitsMask;
		foreach (var digitCanBeRemoved in targetCellsDigitsMask)
		{
			if (baseCells == from conclusion in conclusions where conclusion.Digit == digitCanBeRemoved select conclusion.Cell)
			{
				baseCellsLastDigitsMask &= (Mask)~(1 << digitCanBeRemoved);
			}
		}
		if (PopCount((uint)baseCellsLastDigitsMask) == 2)
		{
			// The JE has formed a distribution disjointed pair.
			inferredTargetPairMask = baseCellsLastDigitsMask;
		}

		var step = new JuniorExocetIncompatiblePairStep(
			[.. conclusions],
			[
				[
					.. from cell in baseCells select new CellViewNode(WellKnownColorIdentifier.Normal, cell),
					.. from cell in targetCells select new CellViewNode(WellKnownColorIdentifier.Auxiliary1, cell),
					.. from cell in crossline select new CellViewNode(WellKnownColorIdentifier.Auxiliary2, cell),
					.. from cell in lastSixteenCells - EmptyCells select new CellViewNode(WellKnownColorIdentifier.Auxiliary3, cell),
					..
					from cell in baseCells
					from d in grid.GetCandidates(cell)
					select new CandidateViewNode(WellKnownColorIdentifier.Normal, cell * 9 + d),
					..
					from cell in crossline
					where grid.GetState(cell) == CellState.Empty
					from d in (Mask)(grid.GetCandidates(cell) & baseCellsDigitsMask)
					select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, cell * 9 + d),
					//.. from house in housesMask select new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, house)
				]
			],
			context.PredefinedOptions,
			baseCellsDigitsMask,
			in incompatibleCandidates,
			in baseCells,
			in targetCells,
			in crossline
		);
		if (context.OnlyFindOne)
		{
			return step;
		}

		context.Accumulator.Add(step);
		return null;
	}

	private static JuniorExocetTargetPairStep? CheckTargetPair(
		scoped ref AnalysisContext context,
		Grid grid,
		scoped ref readonly CellMap baseCells,
		scoped ref readonly CellMap targetCells,
		scoped ref readonly CellMap crossline,
		Mask baseCellsDigitsMask,
		Mask inferredTargetPairMask,
		Offset delta,
		HouseMask housesMask,
		scoped ReadOnlySpan<Conjugate> inferredTargetConjugatePairs
	)
	{
		if (inferredTargetPairMask == 0)
		{
			return null;
		}

		if (delta != 0)
		{
			return null;
		}

		if (baseCells.Count != 2)
		{
			return null;
		}

		var conclusions = new List<Conclusion>();
		foreach (var cell in baseCells)
		{
			foreach (var digit in (Mask)(grid.GetCandidates(cell) & ~inferredTargetPairMask))
			{
				conclusions.Add(new(Elimination, cell, digit));
			}
		}
		foreach (ref readonly var cellGroup in GroupTargets(in targetCells, housesMask))
		{
			var (_, values) = cellGroup;
			switch (values.Count)
			{
				case 1:
				{
					foreach (var cell in values)
					{
						foreach (var digit in (Mask)(grid.GetCandidates(cell) & ~inferredTargetPairMask))
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
					break;
				}
				case 2 when inferredTargetConjugatePairs.First(conj => conj.Map == values) is { Digit: var conjDigit }:
				{
					foreach (var cell in values)
					{
						foreach (var digit in (Mask)(grid.GetCandidates(cell) & ~inferredTargetPairMask & ~(1 << conjDigit)))
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
					break;
				}
			}
		}
		if (targetCells.Count == 2)
		{
			foreach (var cell in targetCells.PeerIntersection)
			{
				foreach (var digit in (Mask)(grid.GetCandidates(cell) & inferredTargetPairMask))
				{
					conclusions.Add(new(Elimination, cell, digit));
				}
			}
		}
		foreach (var cell in baseCells.PeerIntersection)
		{
			foreach (var digit in (Mask)(grid.GetCandidates(cell) & inferredTargetPairMask))
			{
				conclusions.Add(new(Elimination, cell, digit));
			}
		}
		if (conclusions.Count == 0)
		{
			return null;
		}

		var step = new JuniorExocetTargetPairStep(
			[.. conclusions],
			[
				[
					.. from cell in baseCells select new CellViewNode(WellKnownColorIdentifier.Normal, cell),
					.. from cell in targetCells select new CellViewNode(WellKnownColorIdentifier.Auxiliary1, cell),
					.. from cell in crossline select new CellViewNode(WellKnownColorIdentifier.Auxiliary2, cell),
					..
					from cell in baseCells
					from d in grid.GetCandidates(cell)
					select new CandidateViewNode(WellKnownColorIdentifier.Normal, cell * 9 + d),
					..
					from cell in crossline
					where grid.GetState(cell) == CellState.Empty
					from d in (Mask)(grid.GetCandidates(cell) & baseCellsDigitsMask)
					select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, cell * 9 + d),
					//.. from house in housesMask select new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, house)
				]
			],
			context.PredefinedOptions,
			baseCellsDigitsMask,
			inferredTargetPairMask,
			in baseCells,
			in targetCells,
			in crossline
		);
		if (context.OnlyFindOne)
		{
			return step;
		}

		context.Accumulator.Add(step);
		return null;
	}

	private static JuniorExocetGeneralizedFishStepConclusion? CheckGeneralizedFish(
		scoped ref AnalysisContext context,
		Grid grid,
		scoped ref readonly CellMap baseCells,
		scoped ref readonly CellMap targetCells,
		scoped ref readonly CellMap crossline,
		Mask baseCellsDigitsMask,
		Mask inferredTargetPairMask,
		Offset delta,
		bool isRow,
		HouseMask housesMask
	)
	{
		if (inferredTargetPairMask == 0)
		{
			return null;
		}

		if (delta != 0)
		{
			return null;
		}

		if (baseCells.Count != 2)
		{
			return null;
		}

		var inferredTargetPairMaskDigit1 = TrailingZeroCount(inferredTargetPairMask);
		var inferredTargetPairMaskDigit2 = inferredTargetPairMask.GetNextSet(inferredTargetPairMaskDigit1);

		var conclusions = new List<Conclusion>();
		foreach (var line in isRow ? crossline.ColumnMask << 18 : crossline.RowMask << 9)
		{
			var crosslineCellsIntersectedWithLine = HousesMap[line] & crossline & (CandidatesMap[inferredTargetPairMaskDigit1] | CandidatesMap[inferredTargetPairMaskDigit2]);
			if (!crosslineCellsIntersectedWithLine)
			{
				// The current line does not contain any eliminations because all intersected cells in cross-line are not empty.
				continue;
			}

			var elimCells = (HousesMap[line] & EmptyCells) - crossline;
			foreach (var cell in elimCells)
			{
				foreach (var digit in (Mask)(grid.GetCandidates(cell) & inferredTargetPairMask))
				{
					conclusions.Add(new(Elimination, cell, digit));
				}
			}
		}
		if (conclusions.Count == 0)
		{
			return null;
		}

		var step = new JuniorExocetGeneralizedFishStepConclusion(
			[.. conclusions],
			[
				[
					.. from cell in baseCells select new CellViewNode(WellKnownColorIdentifier.Normal, cell),
					.. from cell in targetCells select new CellViewNode(WellKnownColorIdentifier.Auxiliary1, cell),
					.. from cell in crossline select new CellViewNode(WellKnownColorIdentifier.Auxiliary2, cell),
					..
					from cell in baseCells
					from d in grid.GetCandidates(cell)
					select new CandidateViewNode(WellKnownColorIdentifier.Normal, cell * 9 + d),
					..
					from cell in targetCells
					from d in grid.GetCandidates(cell)
					where (inferredTargetPairMask >> d & 1) != 0
					select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary3, cell * 9 + d),
					..
					from cell in crossline
					where grid.GetState(cell) == CellState.Empty
					from d in (Mask)(grid.GetCandidates(cell) & baseCellsDigitsMask)
					let isSwordfishDigit = (inferredTargetPairMask >> d & 1) != 0
					let colorIdentifier = isSwordfishDigit ? WellKnownColorIdentifier.Auxiliary3 : WellKnownColorIdentifier.Auxiliary2
					select new CandidateViewNode(colorIdentifier, cell * 9 + d),
					//.. from house in housesMask select new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, house)
				]
			],
			context.PredefinedOptions,
			baseCellsDigitsMask,
			inferredTargetPairMask,
			in baseCells,
			in targetCells,
			in crossline
		);
		if (context.OnlyFindOne)
		{
			return step;
		}

		context.Accumulator.Add(step);
		return null;
	}

	private static JuniorExocetMirrorAlmostHiddenSetStep? CheckMirrorAlmostHiddenSet(
		scoped ref AnalysisContext context,
		Grid grid,
		scoped ref readonly CellMap baseCells,
		scoped ref readonly CellMap targetCells,
		scoped ref readonly CellMap crossline,
		Mask baseCellsDigitsMask,
		Offset delta,
		bool isRow,
		HouseMask housesMask,
		Offset chuteIndex
	)
	{
		if (delta != 0)
		{
			return null;
		}

		// AHS cannot be used on same-side target cells.
		if (targetCells.InOneHouse(out _))
		{
			return null;
		}

		foreach (ref readonly var cellGroup in GroupTargets(in targetCells, housesMask))
		{
			if (cellGroup is not (_, [var targetCell]))
			{
				continue;
			}

			// Try to fetch the miniline of the current target cell located in.
			var mirrorCells = GetMirrorCells(targetCell, chuteIndex, out var miniline);
			var mirrorEmptyCells = mirrorCells & EmptyCells;
			if (!mirrorEmptyCells)
			{
				// The current miniline cannot contain any eliminations.
				continue;
			}

			// Now check for empty cells in this house, removing all cells located in the miniline that the target cell located in.
			foreach (var coveredHouse in mirrorEmptyCells.CoveredHouses)
			{
				var otherCells = (HousesMap[coveredHouse] & EmptyCells) - miniline;
				if (otherCells.Count < 2)
				{
					// The target house does not contain enough cells to form an AHS.
					continue;
				}

				for (var size = 2; size <= otherCells.Count - 1; size++)
				{
					foreach (ref readonly var extraCells in otherCells.GetSubsets(size - 1).AsReadOnlySpan())
					{
						var ahsCells = extraCells | mirrorEmptyCells;
						foreach (var digitsMaskGroup in ((Mask)(grid[in ahsCells] & ~baseCellsDigitsMask)).GetAllSets().GetSubsets(size))
						{
							var extraDigitsMask = MaskOperations.Create(digitsMaskGroup);
							var lastHoldingMap = CellMap.Empty;
							foreach (var digit in digitsMaskGroup)
							{
								lastHoldingMap |= CandidatesMap[digit];
							}

							lastHoldingMap &= HousesMap[coveredHouse];
							if (lastHoldingMap - targetCell != ahsCells)
							{
								// Final map does not match.
								continue;
							}

							var conclusions = new List<Conclusion>();
							foreach (var cell in extraCells)
							{
								foreach (var digit in (Mask)(grid.GetCandidates(cell) & ~extraDigitsMask))
								{
									conclusions.Add(new(Elimination, cell, digit));
								}
							}
							if (conclusions.Count == 0)
							{
								// No valid conclusions exist.
								continue;
							}

							var step = new JuniorExocetMirrorAlmostHiddenSetStep(
								[.. conclusions],
								[
									[
										.. from cell in baseCells select new CellViewNode(WellKnownColorIdentifier.Normal, cell),
										.. from cell in targetCells select new CellViewNode(WellKnownColorIdentifier.Auxiliary1, cell),
										.. from cell in crossline select new CellViewNode(WellKnownColorIdentifier.Auxiliary2, cell),
										.. from cell in extraCells select new CellViewNode(WellKnownColorIdentifier.Auxiliary3, cell),
										.. from cell in mirrorEmptyCells select new CellViewNode(WellKnownColorIdentifier.Auxiliary3, cell),
										..
										from cell in baseCells
										from d in grid.GetCandidates(cell)
										select new CandidateViewNode(WellKnownColorIdentifier.Normal, cell * 9 + d),
										..
										from cell in mirrorEmptyCells
										from d in (Mask)(grid.GetCandidates(cell) & extraDigitsMask)
										select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary3, cell * 9 + d),
										..
										from cell in extraCells
										from d in (Mask)(grid.GetCandidates(cell) & extraDigitsMask)
										select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary3, cell * 9 + d),
										..
										from cell in crossline
										where grid.GetState(cell) == CellState.Empty
										from d in (Mask)(grid.GetCandidates(cell) & baseCellsDigitsMask)
										select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, cell * 9 + d),
										//.. from house in housesMask select new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, house)
									]
								],
								context.PredefinedOptions,
								baseCellsDigitsMask,
								in baseCells,
								in targetCells,
								in crossline,
								in extraCells,
								extraDigitsMask
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

		return null;
	}

	private static ExocetLockedMemberStep? CheckJeLockedMember(
		scoped ref AnalysisContext context,
		Grid grid,
		scoped ref readonly CellMap baseCells,
		scoped ref readonly CellMap targetCells,
		scoped ref readonly CellMap crossline,
		Mask baseCellsDigitsMask,
		scoped ReadOnlySpan<LockedMember?> lockedMembers,
		Offset chuteIndex,
		scoped ReadOnlySpan<TargetCellsGroup> groupsOfTargetCells,
		out Mask inferredLastTargetDigitsMask,
		out Mask lockedDigitsMask
	)
	{
		// Same-side target cells cannot be used in this case, because locked member will check for mirror cells,
		// invalid for mirror checking on same-side cells.
		if (targetCells.InOneHouse(out _))
		{
			(inferredLastTargetDigitsMask, lockedDigitsMask) = (0, 0);
			return null;
		}

		if (baseCells.Count != 2)
		{
			// No conclusions when the number of base cells is not 2.
			(inferredLastTargetDigitsMask, lockedDigitsMask) = (0, 0);
			return null;
		}

		// Check whether the digit is a real locked member. Locked member:
		//
		//   B B / | . . . | L L L
		//   . . . | T . . | V / /
		//   . . . | V . . | T / /
		//
		// Symbols:
		//   B: Base cells that contain candidate 'a'.
		//   T: Target cells that are empty.
		//   V: A cell that is filled with a value that is not appeared in base cells.
		//   L: Cells forming a locked member of digit 'a'.
		//   /: Cells don't contain candidate 'a'.
		if (lockedMembers.IsEmpty || groupsOfTargetCells is not [(_, [var targetCell1]), (_, [var targetCell2])])
		{
			goto AssignBaseMaskByDefault;
		}

		var (cellOffsets, candidateOffsets, houseOffsets) = (new List<CellViewNode>(4), new List<CandidateViewNode>(), new List<HouseViewNode>(2));
		(lockedDigitsMask, inferredLastTargetDigitsMask, var conclusions) = (0, 0, new List<Conclusion>());

		// Collect eliminations via mirror cells.
		// For each locked member, we should check for its mirror cells,
		// determining whether the mirror cells can make this target cell and the other target cell any conclusions.
		// Rule:
		//
		//   1)
		//   If all mirror cells of a target cell are non-empty, check its containing values and determine
		//   whether one of the digits is concluded as a locked member.
		//   If so, the other side of the target cell can be fixed with that digit (Mirror Synchronization) -
		//   all digits non-fixed can be removed.
		//
		//   2)
		//   If one of two mirror cells is non-empty, check it, determining whether it is a locked member digit.
		//   If so, record all digits appeared in the other empty mirror cells, and this locked member digit,
		//   they are possible candidates in the other side of target cell. Other digits can be removed.
		//
		// Here is an example:
		//
		//   98.7..6..5...9..7...7..4...4...8...3.3....4.6..54...9.2.....1...5..12.....89...6.
		var lockedDigitElimTimes = 0;
		foreach (var lockedDigit in baseCellsDigitsMask)
		{
			if (lockedMembers[lockedDigit] is var (lockedMemberMap, lockedBlock))
			{
				var (thisTargetCell, theOtherTargetCell) = HousesMap[lockedBlock].Contains(targetCell1)
					? (targetCell1, targetCell2)
					: (targetCell2, targetCell1);
				var mirrorCellsThisTarget = GetMirrorCells(thisTargetCell, chuteIndex, out _);
				var finalDigitsMask = (mirrorCellsThisTarget - EmptyCells) switch
				{
					[] when mirrorCellsThisTarget is [var a, var b]
						=> (Mask)((grid.GetCandidates(a) | grid.GetCandidates(b)) & baseCellsDigitsMask),
					[var a] when mirrorCellsThisTarget - a is [var b]
						=> (Mask)(((Mask)(1 << grid.GetDigit(a)) | grid.GetCandidates(b)) & baseCellsDigitsMask),
					[var a, var b]
						=> (Mask)((1 << grid.GetDigit(a) | 1 << grid.GetDigit(b)) & baseCellsDigitsMask)
				};
				foreach (var digit in (Mask)(grid.GetCandidates(theOtherTargetCell) & ~finalDigitsMask))
				{
					conclusions.Add(new(Elimination, theOtherTargetCell, digit));
				}

				cellOffsets.AddRange(from cell in mirrorCellsThisTarget select new CellViewNode(WellKnownColorIdentifier.Auxiliary3, cell));
				candidateOffsets.AddRange(
					from cell in lockedMemberMap
					select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary1, cell * 9 + lockedDigit)
				);
				houseOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, lockedBlock));

				lockedDigitsMask |= (Mask)(1 << lockedDigit);
				inferredLastTargetDigitsMask |= (Mask)(grid.GetCandidates(theOtherTargetCell) & finalDigitsMask);
				lockedDigitElimTimes++;
			}
		}

		if (lockedDigitElimTimes == 2)
		{
			// Sync candidates in base cells from target cells.
			foreach (var cell in baseCells)
			{
				foreach (var digit in (Mask)(grid.GetCandidates(cell) & ~inferredLastTargetDigitsMask))
				{
					conclusions.Add(new(Elimination, cell, digit));
				}
			}
		}

		if (conclusions.Count == 0)
		{
			// No eliminations found.
			goto AssignBaseMaskByDefault;
		}

		var ld = lockedDigitsMask;
		var step = new ExocetLockedMemberStep(
			[.. conclusions],
			[
				[
					.. from cell in baseCells select new CellViewNode(WellKnownColorIdentifier.Normal, cell),
					.. from cell in targetCells select new CellViewNode(WellKnownColorIdentifier.Auxiliary1, cell),
					.. from cell in crossline select new CellViewNode(WellKnownColorIdentifier.Auxiliary2, cell),
					.. cellOffsets,
					.. candidateOffsets,
					..
					from cell in baseCells
					from d in grid.GetCandidates(cell)
					let colorIdentifier = (ld >> d & 1) != 0 ? WellKnownColorIdentifier.Auxiliary2 : WellKnownColorIdentifier.Normal
					select new CandidateViewNode(colorIdentifier, cell * 9 + d),
					..
					from cell in crossline
					where grid.GetState(cell) == CellState.Empty
					from d in (Mask)(grid.GetCandidates(cell) & baseCellsDigitsMask)
					select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, cell * 9 + d),
					.. houseOffsets,
					//.. from house in housesMask select new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, house)
				]
			],
			context.PredefinedOptions,
			baseCellsDigitsMask,
			lockedDigitsMask,
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
		return null;

	AssignBaseMaskByDefault:
		(inferredLastTargetDigitsMask, lockedDigitsMask) = (baseCellsDigitsMask, 0);
		return null;
	}

	private static ExocetLockedMemberStep? CheckLockedMemberSe(
		scoped ref AnalysisContext context,
		Grid grid,
		scoped ref readonly CellMap baseCells,
		scoped ref readonly CellMap targetCells,
		scoped ref readonly CellMap crossline,
		Mask baseCellsDigitsMask,
		Digit lockedDigit
	)
	{
		switch (targetCells)
		{
			case [var targetCell]:
			{
				// Check whether the endo-target cell only holds one.
				var endoTargetCell = -1;
				var multipleEndoTargetCellsFound = false;
				foreach (var cell in crossline)
				{
					if (grid.GetDigit(cell) == lockedDigit)
					{
						if (endoTargetCell != -1)
						{
							// Multiple endo-target cells found.
							multipleEndoTargetCellsFound = true;
							break;
						}

						endoTargetCell = cell;
					}
				}
				if (multipleEndoTargetCellsFound)
				{
					// Invalid.
					break;
				}

				var conclusions = new List<Conclusion>();

				// First, check for elimination on target cell.
				foreach (var digit in (Mask)(grid.GetCandidates(targetCell) & ~(baseCellsDigitsMask & ~(1 << lockedDigit))))
				{
					conclusions.Add(new(Elimination, targetCell, digit));
				}

				// Second, check for locked candidates for base cells - base cells form a locked candidates of digit 'lockedDigit'.
				foreach (var cell in baseCells % CandidatesMap[lockedDigit])
				{
					conclusions.Add(new(Elimination, cell, lockedDigit));
				}

				if (conclusions.Count == 0)
				{
					// No eliminations found.
					break;
				}

				var step = new ExocetLockedMemberStep(
					[.. conclusions],
					[
						[
							.. from cell in baseCells select new CellViewNode(WellKnownColorIdentifier.Normal, cell),
							.. from cell in targetCells select new CellViewNode(WellKnownColorIdentifier.Auxiliary1, cell),
							.. from cell in crossline - endoTargetCell select new CellViewNode(WellKnownColorIdentifier.Auxiliary2, cell),
							..
							from cell in baseCells
							from d in grid.GetCandidates(cell)
							let colorIdentifier = lockedDigit != d ? WellKnownColorIdentifier.Normal : WellKnownColorIdentifier.Auxiliary1
							select new CandidateViewNode(colorIdentifier, cell * 9 + d),
							..
							from cell in crossline - EmptyCells
							where grid.GetDigit(cell) == lockedDigit
							select new CellViewNode(WellKnownColorIdentifier.Auxiliary1, cell),
							..
							from cell in crossline
							where grid.GetState(cell) == CellState.Empty
							from d in (Mask)(grid.GetCandidates(cell) & baseCellsDigitsMask)
							select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, cell * 9 + d),
							//.. from house in housesMask select new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, house)
						]
					],
					context.PredefinedOptions,
					baseCellsDigitsMask,
					(Mask)(1 << lockedDigit),
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
				return null;
			}
			//case { Count: 2 }:
			//{
			//	// TODO: May be conjugate pair or AHS.
			//}
		}

		return null;
	}

	private static JuniorExocetMirrorSyncStep? CheckMirrorSync(
		scoped ref AnalysisContext context,
		Grid grid,
		scoped ref readonly CellMap baseCells,
		scoped ref readonly CellMap targetCells,
		scoped ref readonly CellMap crossline,
		Mask baseCellsDigitsMask,
		HouseMask housesMask,
		Offset chuteIndex,
		Mask digitsMaskAppearedInCrossline
	)
	{
		if (digitsMaskAppearedInCrossline != 0)
		{
			// Mirror rule requires the digit cannot be appeared in cross-line as values.
			return null;
		}

		if (targetCells.Count != 2 || targetCells.InOneHouse(out _))
		{
			// TODO: Now ignores the case on conjugate pairs and AHS.
			return null;
		}

		var conclusions = new List<Conclusion>();
		foreach (var block in targetCells.BlockMask)
		{
			var targetCell = (HousesMap[block] & targetCells)[0];
			var theOtherTargetCell = (targetCells - targetCell)[0];
			collectFor(conclusions, in grid, targetCell, theOtherTargetCell);
		}

		if (conclusions.Count == 0)
		{
			// No conclusions found.
			return null;
		}

		var step = new JuniorExocetMirrorSyncStep(
			[.. conclusions],
			[
				[
					.. from cell in baseCells select new CellViewNode(WellKnownColorIdentifier.Normal, cell),
					.. from cell in targetCells select new CellViewNode(WellKnownColorIdentifier.Auxiliary1, cell),
					.. from cell in crossline select new CellViewNode(WellKnownColorIdentifier.Auxiliary2, cell),
					..
					from cell in baseCells
					from d in grid.GetCandidates(cell)
					select new CandidateViewNode(WellKnownColorIdentifier.Normal, cell * 9 + d),
					..
					from cell in crossline
					where grid.GetState(cell) == CellState.Empty
					from d in (Mask)(grid.GetCandidates(cell) & baseCellsDigitsMask)
					select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, cell * 9 + d),
					//.. from house in housesMask select new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, house)
				]
			],
			context.PredefinedOptions,
			baseCellsDigitsMask,
			in baseCells,
			in targetCells,
			in crossline
		);
		if (context.OnlyFindOne)
		{
			return step;
		}

		context.Accumulator.Add(step);
		return null;


		void collectFor(List<Conclusion> conclusions, scoped ref readonly Grid grid, Cell targetCell, Cell theOtherTargetCell)
		{
			Unsafe.SkipInit(out CellMap miniline);
			foreach (var tempMiniline in MinilinesGroupedByChuteIndex[chuteIndex])
			{
				if (tempMiniline.Contains(theOtherTargetCell))
				{
					miniline = tempMiniline;
					break;
				}
			}

			switch (miniline - theOtherTargetCell & EmptyCells)
			{
				case [var mirrorEmptyCellFromTheOtherTargetCell]:
				{
					var digitsMaskForMirrorFromTheOtherTargetCell = (Mask)(grid.GetCandidates(mirrorEmptyCellFromTheOtherTargetCell) & baseCellsDigitsMask);
					if (digitsMaskForMirrorFromTheOtherTargetCell == baseCellsDigitsMask)
					{
						// They are same. Don't need to sync candidates.
						return;
					}

					var digitsMaskTargetCell = (Mask)(grid.GetCandidates(targetCell) & ~digitsMaskForMirrorFromTheOtherTargetCell);
					if (digitsMaskTargetCell == 0)
					{
						// No candidates should be sync'ed.
						return;
					}

					// Sync for candidates for the other side of target cell.
					var lastDigitsMaskForTarget = grid.GetCandidates(targetCell) & baseCellsDigitsMask;
					foreach (var digit in digitsMaskTargetCell)
					{
						lastDigitsMaskForTarget &= (Mask)~(1 << digit);
						conclusions.Add(new(Elimination, targetCell, digit));
					}

					// Sync for mirror cells.
					foreach (var digit in (Mask)(grid.GetCandidates(mirrorEmptyCellFromTheOtherTargetCell) & ~lastDigitsMaskForTarget))
					{
						conclusions.Add(new(Elimination, mirrorEmptyCellFromTheOtherTargetCell, digit));
					}

					break;
				}
			}
		}
	}

	private static WeakExocetStep? CheckBaseWeak(
		scoped ref AnalysisContext context,
		Grid grid,
		scoped ref readonly CellMap baseCells,
		scoped ReadOnlySpan<TargetCellsGroup> groupsOfTargetCells,
		scoped ref readonly CellMap crossline,
		Cell valueDigitCell,
		Cell missingValueCell,
		Mask baseCellsDigitsMask
	)
	{
		var conclusions = new List<Conclusion>();
		var targetCells = CellMap.Empty;
		foreach (var groupOfTargetCells in groupsOfTargetCells)
		{
			foreach (var cell in groupOfTargetCells)
			{
				targetCells.Add(cell);

				if (valueDigitCell != -1 || !PeersMap[cell].Contains(missingValueCell))
				{
					foreach (var digit in (Mask)(grid.GetCandidates(cell) & ~baseCellsDigitsMask))
					{
						conclusions.Add(new(Elimination, cell, digit));
					}
				}
			}
		}
		if (conclusions.Count == 0)
		{
			return null;
		}

		var step = new WeakExocetStep(
			[.. conclusions],
			[
				[
					.. from cell in baseCells select new CellViewNode(WellKnownColorIdentifier.Normal, cell),
					.. from cell in targetCells select new CellViewNode(WellKnownColorIdentifier.Auxiliary1, cell),
					.. from cell in crossline - missingValueCell select new CellViewNode(WellKnownColorIdentifier.Auxiliary2, cell),
					.. valueDigitCell != -1 ? [new CellViewNode(WellKnownColorIdentifier.Auxiliary3, valueDigitCell)] : (ViewNode[])[],
					new CellViewNode(WellKnownColorIdentifier.Auxiliary3, missingValueCell),
					..
					from cell in baseCells
					from d in grid.GetCandidates(cell)
					select new CandidateViewNode(WellKnownColorIdentifier.Normal, cell * 9 + d),
					..
					from cell in crossline
					where grid.GetState(cell) == CellState.Empty
					from d in (Mask)(grid.GetCandidates(cell) & baseCellsDigitsMask)
					select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, cell * 9 + d),
					//.. from house in housesMask select new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, house)
				]
			],
			context.PredefinedOptions,
			baseCellsDigitsMask,
			valueDigitCell,
			missingValueCell,
			in baseCells,
			in targetCells,
			in crossline
		);
		if (context.OnlyFindOne)
		{
			return step;
		}

		context.Accumulator.Add(step);
		return null;
	}

	private static WeakExocetAdjacentTargetStep? CheckWeakAdjacentTarget(
		scoped ref AnalysisContext context,
		Grid grid,
		scoped ref readonly CellMap baseCells,
		scoped ReadOnlySpan<TargetCellsGroup> groupsOfTargetCells,
		scoped ref readonly CellMap crossline,
		Cell valueDigitCell,
		Cell missingValueCell,
		Mask baseCellsDigitsMask,
		Offset chuteIndex
	)
	{
		if (valueDigitCell == -1)
		{
			// The value-digit cell shouldn't be missing.
			return null;
		}

		var conclusions = new List<Conclusion>();
		var cellOffsets = new List<CellViewNode>();
		var targetCells = (CellMap)([groupsOfTargetCells[0][0], groupsOfTargetCells[1][0]]);
		foreach (var (thisTargetCell, theOtherTargetCell) in ((groupsOfTargetCells[0][0], groupsOfTargetCells[1][0]), (groupsOfTargetCells[1][0], groupsOfTargetCells[0][0])))
		{
			var mirrorCellsThisTarget = GetMirrorCells(thisTargetCell, chuteIndex, out _);
			var finalDigitsMask = (mirrorCellsThisTarget - EmptyCells) switch
			{
				[] when mirrorCellsThisTarget is [var a, var b]
					=> (Mask)((grid.GetCandidates(a) | grid.GetCandidates(b)) & baseCellsDigitsMask),
				[var a] when mirrorCellsThisTarget - a is [var b]
					=> (Mask)(((Mask)(1 << grid.GetDigit(a)) | grid.GetCandidates(b)) & baseCellsDigitsMask),
				[var a, var b]
					=> (Mask)((1 << grid.GetDigit(a) | 1 << grid.GetDigit(b)) & baseCellsDigitsMask)
			};
			foreach (var digit in (Mask)(grid.GetCandidates(theOtherTargetCell) & ~finalDigitsMask))
			{
				conclusions.Add(new(Elimination, theOtherTargetCell, digit));
			}

			cellOffsets.AddRange(from cell in mirrorCellsThisTarget select new CellViewNode(WellKnownColorIdentifier.Auxiliary3, cell));
		}
		if (conclusions.Count == 0)
		{
			return null;
		}

		var step = new WeakExocetAdjacentTargetStep(
			[.. conclusions],
			[
				[
					.. from cell in baseCells select new CellViewNode(WellKnownColorIdentifier.Normal, cell),
					.. from cell in targetCells select new CellViewNode(WellKnownColorIdentifier.Auxiliary1, cell),
					.. cellOffsets,
					.. from cell in crossline - missingValueCell select new CellViewNode(WellKnownColorIdentifier.Auxiliary2, cell),
					new CellViewNode(WellKnownColorIdentifier.Auxiliary3, valueDigitCell),
					new CellViewNode(WellKnownColorIdentifier.Auxiliary3, missingValueCell),
					..
					from cell in baseCells
					from d in grid.GetCandidates(cell)
					select new CandidateViewNode(WellKnownColorIdentifier.Normal, cell * 9 + d),
					..
					from cell in crossline
					where grid.GetState(cell) == CellState.Empty
					from d in (Mask)(grid.GetCandidates(cell) & baseCellsDigitsMask)
					select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, cell * 9 + d),
					//.. from house in housesMask select new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, house)
				]
			],
			context.PredefinedOptions,
			baseCellsDigitsMask,
			valueDigitCell,
			missingValueCell,
			in baseCells,
			in targetCells,
			in crossline
		);
		if (context.OnlyFindOne)
		{
			return step;
		}

		context.Accumulator.Add(step);
		return null;
	}

	private static WeakExocetSlashStep? CheckWeakExocetSlash(
		scoped ref AnalysisContext context,
		Grid grid,
		scoped ref readonly CellMap baseCells,
		scoped ReadOnlySpan<TargetCellsGroup> groupsOfTargetCells,
		scoped ref readonly CellMap crossline,
		scoped ref readonly CellMap lastSixteenCells,
		Cell valueDigitCell,
		Cell missingValueCell,
		Mask baseCellsDigitsMask,
		bool isRow,
		Offset chuteIndex,
		out CellMap cellsCanBeEliminated
	)
	{
		if (valueDigitCell == -1)
		{
			// The value-digit cell shouldn't be missing.
			cellsCanBeEliminated = [];
			return null;
		}

		Unsafe.SkipInit(out Chute sharedChute);
		foreach (var chute in Chutes[isRow ? 3.. : ..3])
		{
			if (chute.Cells.Contains(valueDigitCell))
			{
				sharedChute = chute;
				break;
			}
		}

		scoped var blocks = ((Mask)(lastSixteenCells.BlockMask & ~sharedChute.Cells.BlockMask)).GetAllSets();
		cellsCanBeEliminated = lastSixteenCells & (HousesMap[blocks[0]] | HousesMap[blocks[1]]);

		var conclusions = new List<Conclusion>();
		foreach (var block in blocks)
		{
			// Check for digits appeared in this block.
			var valueCells = (cellsCanBeEliminated & HousesMap[block]) - EmptyCells;
			var valueCellsBaseDigitsMask = (Mask)0;
			foreach (var digit in baseCellsDigitsMask)
			{
				if (valueCells & ValuesMap[digit])
				{
					valueCellsBaseDigitsMask |= (Mask)(1 << digit);
				}
			}

			foreach (var cell in cellsCanBeEliminated & HousesMap[block] & EmptyCells)
			{
				foreach (var digit in (Mask)(grid.GetCandidates(cell) & baseCellsDigitsMask))
				{
					conclusions.Add(new(Elimination, cell, digit));
				}
			}
		}
		if (conclusions.Count == 0)
		{
			return null;
		}

		var targetCells = from @group in groupsOfTargetCells select @group[0];
		var step = new WeakExocetSlashStep(
			[.. conclusions],
			[
				[
					.. from cell in baseCells select new CellViewNode(WellKnownColorIdentifier.Normal, cell),
					.. from cell in targetCells select new CellViewNode(WellKnownColorIdentifier.Auxiliary1, cell),
					.. from cell in crossline - missingValueCell select new CellViewNode(WellKnownColorIdentifier.Auxiliary2, cell),
					new CellViewNode(WellKnownColorIdentifier.Auxiliary3, valueDigitCell),
					new CellViewNode(WellKnownColorIdentifier.Auxiliary3, missingValueCell),
					..
					from cell in baseCells
					from d in grid.GetCandidates(cell)
					select new CandidateViewNode(WellKnownColorIdentifier.Normal, cell * 9 + d),
					..
					from cell in crossline
					where grid.GetState(cell) == CellState.Empty
					from d in (Mask)(grid.GetCandidates(cell) & baseCellsDigitsMask)
					select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, cell * 9 + d),
					//.. from house in housesMask select new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, house)
				]
			],
			context.PredefinedOptions,
			baseCellsDigitsMask,
			valueDigitCell,
			missingValueCell,
			in baseCells,
			in targetCells,
			in crossline
		);
		if (context.OnlyFindOne)
		{
			return step;
		}

		context.Accumulator.Add(step);
		return null;
	}

	private static WeakExocetBzRectangleStep? CheckWeakExocetBzRectangle(
		scoped ref AnalysisContext context,
		Grid grid,
		scoped ref readonly CellMap baseCells,
		scoped ReadOnlySpan<TargetCellsGroup> groupsOfTargetCells,
		scoped ref readonly CellMap crossline,
		scoped ref readonly CellMap lastSixteenCells,
		scoped ref readonly CellMap cellsCanBeEliminated,
		Cell valueDigitCell,
		Cell missingValueCell,
		Mask baseCellsDigitsMask,
		bool isRow,
		Offset chuteIndex
	)
	{
		if (valueDigitCell == -1)
		{
			// The value-digit cell shouldn't be missing.
			return null;
		}

		scoped var elimLines = (isRow ? baseCells.RowMask << 9 : baseCells.ColumnMask << 18).GetAllSets();
		var elimLinesMap = HousesMap[elimLines[0]] | HousesMap[elimLines[1]];
		scoped var intersectedLines = (isRow ? cellsCanBeEliminated.ColumnMask << 18 : cellsCanBeEliminated.RowMask << 9).GetAllSets();
		var intersectedLinesMap = HousesMap[intersectedLines[0]] | HousesMap[intersectedLines[1]];
		var finalIntersectedFourCells = elimLinesMap & intersectedLinesMap;

		var conclusions = new List<Conclusion>();
		foreach (var cell in HousesMap[TrailingZeroCount(finalIntersectedFourCells.BlockMask)] - crossline - finalIntersectedFourCells)
		{
			foreach (var digit in (Mask)(grid.GetCandidates(cell) & ~baseCellsDigitsMask))
			{
				conclusions.Add(new(Elimination, cell, digit));
			}
		}
		if (conclusions.Count == 0)
		{
			return null;
		}

		var targetCells = from @group in groupsOfTargetCells select @group[0];
		var step = new WeakExocetBzRectangleStep(
			[.. conclusions],
			[
				[
					.. from cell in baseCells select new CellViewNode(WellKnownColorIdentifier.Normal, cell),
					.. from cell in targetCells select new CellViewNode(WellKnownColorIdentifier.Auxiliary1, cell),
					.. from cell in crossline - missingValueCell select new CellViewNode(WellKnownColorIdentifier.Auxiliary2, cell),
					new CellViewNode(WellKnownColorIdentifier.Auxiliary3, valueDigitCell),
					new CellViewNode(WellKnownColorIdentifier.Auxiliary3, missingValueCell),
					..
					from cell in baseCells
					from d in grid.GetCandidates(cell)
					select new CandidateViewNode(WellKnownColorIdentifier.Normal, cell * 9 + d),
					..
					from cell in crossline
					where grid.GetState(cell) == CellState.Empty
					from d in (Mask)(grid.GetCandidates(cell) & baseCellsDigitsMask)
					select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, cell * 9 + d),
					//.. from house in housesMask select new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, house)
				]
			],
			context.PredefinedOptions,
			baseCellsDigitsMask,
			valueDigitCell,
			missingValueCell,
			in baseCells,
			in targetCells,
			in crossline
		);
		if (context.OnlyFindOne)
		{
			return step;
		}

		context.Accumulator.Add(step);
		return null;
	}

	private static DoubleExocetBaseStep? CheckDoubleBase(
		scoped ref AnalysisContext context,
		Grid grid,
		scoped ref readonly CellMap baseCells,
		scoped ref readonly CellMap targetCells,
		scoped ref readonly CellMap theOtherBaseCells,
		scoped ref readonly CellMap theOtherTargetCells,
		scoped ref readonly CellMap crossline,
		Mask baseCellsDigitsMask,
		HouseMask housesMask
	)
	{
		var conclusions = new List<Conclusion>();
		foreach (var cell in baseCells.PeerIntersection & theOtherBaseCells.PeerIntersection)
		{
			foreach (var digit in (Mask)(grid.GetCandidates(cell) & baseCellsDigitsMask))
			{
				conclusions.Add(new(Elimination, cell, digit));
			}
		}
		foreach (var cell in (targetCells | theOtherTargetCells).PeerIntersection)
		{
			foreach (var digit in (Mask)(grid.GetCandidates(cell) & baseCellsDigitsMask))
			{
				conclusions.Add(new(Elimination, cell, digit));
			}
		}
		if (conclusions.Count == 0)
		{
			// No eliminations found.
			return null;
		}

		var step = new DoubleExocetBaseStep(
			[.. conclusions],
			[
				[
					.. from cell in baseCells | theOtherBaseCells select new CellViewNode(WellKnownColorIdentifier.Normal, cell),
					.. from cell in targetCells | theOtherTargetCells select new CellViewNode(WellKnownColorIdentifier.Auxiliary1, cell),
					.. from cell in crossline select new CellViewNode(WellKnownColorIdentifier.Auxiliary2, cell),
					..
					from cell in baseCells | theOtherBaseCells
					from d in grid.GetCandidates(cell)
					select new CandidateViewNode(WellKnownColorIdentifier.Normal, cell * 9 + d),
					..
					from cell in crossline
					where grid.GetState(cell) == CellState.Empty
					from d in (Mask)(grid.GetCandidates(cell) & baseCellsDigitsMask)
					select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary2, cell * 9 + d),
					//.. from house in housesMask select new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, house)
				]
			],
			context.PredefinedOptions,
			baseCellsDigitsMask,
			in baseCells,
			in targetCells,
			in crossline,
			in theOtherBaseCells,
			in theOtherTargetCells
		);
		if (context.OnlyFindOne)
		{
			return step;
		}

		context.Accumulator.Add(step);
		return null;
	}

	private static DoubleExocetGeneralizedFishStep? CheckDoubleGeneralizedFish(
		scoped ref AnalysisContext context,
		Grid grid,
		scoped ref readonly CellMap baseCells,
		scoped ref readonly CellMap targetCells,
		scoped ref readonly CellMap theOtherBaseCells,
		scoped ref readonly CellMap theOtherTargetCells,
		scoped ref readonly CellMap crossline,
		Count size,
		bool isRow,
		Mask baseCellsDigitsMask,
		HouseMask housesMask
	)
	{
		var conclusions = new List<Conclusion>();
		var lockedDigitsMask = (Mask)0;
		foreach (var digit in baseCellsDigitsMask)
		{
			var digitDistribution = CandidatesMap[digit] & crossline;
			if (PopCount((uint)(isRow ? digitDistribution.ColumnMask : digitDistribution.RowMask)) != size - 1)
			{
				// Cannot form a generalized fish.
				continue;
			}

			foreach (var line in isRow ? digitDistribution.ColumnMask << 18 : digitDistribution.RowMask << 9)
			{
				var cells = (HousesMap[line] & CandidatesMap[digit]) - crossline;
				if (!cells)
				{
					continue;
				}

				lockedDigitsMask |= (Mask)(1 << digit);
				foreach (var cell in cells)
				{
					conclusions.Add(new(Elimination, cell, digit));
				}
			}
		}
		if (conclusions.Count == 0)
		{
			return null;
		}

		var step = new DoubleExocetGeneralizedFishStep(
			[.. conclusions],
			[
				[
					.. from cell in baseCells | theOtherBaseCells select new CellViewNode(WellKnownColorIdentifier.Normal, cell),
					.. from cell in targetCells | theOtherTargetCells select new CellViewNode(WellKnownColorIdentifier.Auxiliary1, cell),
					.. from cell in crossline select new CellViewNode(WellKnownColorIdentifier.Auxiliary2, cell),
					..
					from cell in baseCells | theOtherBaseCells
					from d in grid.GetCandidates(cell)
					select new CandidateViewNode(WellKnownColorIdentifier.Normal, cell * 9 + d),
					..
					from cell in targetCells | theOtherTargetCells
					from d in grid.GetCandidates(cell)
					where (lockedDigitsMask >> d & 1) != 0
					select new CandidateViewNode(WellKnownColorIdentifier.Auxiliary3, cell * 9 + d),
					..
					from cell in crossline
					where grid.GetState(cell) == CellState.Empty
					from d in (Mask)(grid.GetCandidates(cell) & baseCellsDigitsMask)
					let colorIdentifier = (lockedDigitsMask >> d & 1) != 0 ? WellKnownColorIdentifier.Auxiliary3 : WellKnownColorIdentifier.Auxiliary2
					select new CandidateViewNode(colorIdentifier, cell * 9 + d),
					//.. from house in housesMask select new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, house)
				]
			],
			context.PredefinedOptions,
			baseCellsDigitsMask,
			in baseCells,
			in targetCells,
			in crossline,
			in theOtherBaseCells,
			in theOtherTargetCells
		);
		if (context.OnlyFindOne)
		{
			return step;
		}

		context.Accumulator.Add(step);
		return null;
	}

	/// <summary>
	/// Try to get the mirror cells for the specified target cell at the specified index of the chute.
	/// </summary>
	/// <param name="chuteIndex">The chute index (in range 0..6).</param>
	/// <param name="targetCell">The target cell.</param>
	/// <param name="miniline">The miniline cells the target cell and mirror cells lie in.</param>
	/// <returns>The mirror cells that may contain non-empty cells.</returns>
	private static CellMap GetMirrorCells(Cell targetCell, Offset chuteIndex, out CellMap miniline)
	{
		Unsafe.SkipInit(out miniline);
		foreach (ref readonly var temp in MinilinesGroupedByChuteIndex[chuteIndex].AsReadOnlySpan())
		{
			if (temp.Contains(targetCell))
			{
				miniline = temp;
				break;
			}
		}

		return miniline - targetCell;
	}

	/// <summary>
	/// Try to group up with target cells, separating into multiple parts, grouped by its containing row or column.
	/// </summary>
	/// <param name="targetCells">The target cells to be split.</param>
	/// <param name="houses">The mask value holding a list of houses to be matched.</param>
	/// <returns>A list of <see cref="CellMap"/> grouped, representing as a <see cref="TargetCellsGroup"/>.</returns>
	private static ReadOnlySpan<TargetCellsGroup> GroupTargets(scoped ref readonly CellMap targetCells, HouseMask houses)
	{
		var result = new TargetCellsGroup[PopCount((uint)houses)];
		var i = 0;
		foreach (var house in houses)
		{
			if ((targetCells & HousesMap[house]) is var map and not [])
			{
				result[i++] = new(house, in map);
			}
		}

		return result.AsReadOnlySpan()[..i];
	}

	/// <summary>
	/// Try to create a <see cref="ReadOnlySpan{T}"/> of <see cref="CellMap"/> instances
	/// indicating the locked members for the digit at the specified index.
	/// </summary>
	/// <param name="baseCells">The base cells.</param>
	/// <param name="targetCells">The target cells.</param>
	/// <param name="lockedDigitsMask">A mask that holds a list of digits that may be locked memebrs.</param>
	/// <returns>A <see cref="ReadOnlySpan{T}"/> of <see cref="CellMap"/> instances.</returns>
	private static ReadOnlySpan<LockedMember?> GetLockedMembers(
		scoped ref readonly CellMap baseCells,
		scoped ref readonly CellMap targetCells,
		scoped ref Mask lockedDigitsMask
	)
	{
		var flag = false;
		var r = new LockedMember?[9];
		var realLockedDigitsMask = (Mask)0;
		foreach (var lockedDigit in lockedDigitsMask)
		{
			var lockedMemberMap = CellMap.Empty;
			var lockedBlock = -1;
			foreach (var block in targetCells.BlockMask)
			{
				var lastMap = HousesMap[block] - targetCells & CandidatesMap[lockedDigit];
				if (!!lastMap && (HousesMap[baseCells.CoveredLine] & lastMap) == lastMap)
				{
					(lockedMemberMap, lockedBlock) = (lastMap, block);
					break;
				}
			}
			if (lockedBlock == -1)
			{
				continue;
			}

			r[lockedDigit] = (lockedMemberMap, lockedBlock);
			realLockedDigitsMask |= (Mask)(1 << lockedDigit);
			flag = true;
		}

		(var @return, lockedDigitsMask) = flag ? (r, realLockedDigitsMask) : ([], 0);
		return @return;
	}
}
