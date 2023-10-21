using System.Numerics;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Metadata;
using Sudoku.Analytics.Steps;
using Sudoku.Concepts;
using Sudoku.Linq;
using Sudoku.Rendering;
using Sudoku.Rendering.Nodes;
using static System.Numerics.BitOperations;
using static Sudoku.Analytics.CachedFields;
using static Sudoku.Analytics.ConclusionType;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Qiu's Deadly Pattern</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Standard types:
/// <list type="bullet">
/// <item>Qiu's Deadly Pattern Type 1</item>
/// <item>Qiu's Deadly Pattern Type 2</item>
/// <item>Qiu's Deadly Pattern Type 3</item>
/// <item>Qiu's Deadly Pattern Type 4</item>
/// <item>Qiu's Deadly Pattern Locked Type</item>
/// </list>
/// </item>
/// <item>
/// External types:
/// <list type="bullet">
/// <item>Qiu's Deadly Pattern External Type 1</item>
/// <item>Qiu's Deadly Pattern External Type 2</item>
/// </list>
/// </item>
/// </list>
/// </summary>
[StepSearcher(
	Technique.QiuDeadlyPatternType1, Technique.QiuDeadlyPatternType2, Technique.QiuDeadlyPatternType3, Technique.QiuDeadlyPatternType4,
	Technique.LockedQiuDeadlyPattern, Technique.QiuDeadlyPatternExternalType1, Technique.QiuDeadlyPatternExternalType2,
	Flags = ConditionalFlags.Standard)]
public sealed partial class QiuDeadlyPatternStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates the patterns for case 1.
	/// </summary>
	private static readonly Pattern1[] PatternsForCase1;

	/// <summary>
	/// Indicates the patterns for case 2.
	/// </summary>
	private static readonly Pattern2[] PatternsForCase2;


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static QiuDeadlyPatternStepSearcher()
	{
		// Case 1: 2 lines + 2 cells.
		var lineOffsets = (Offset[][])[[0, 1, 2], [0, 2, 1], [1, 2, 0], [3, 4, 5], [3, 5, 4], [4, 5, 3], [6, 7, 8], [6, 8, 7], [7, 8, 6]];
		var patternsForCase1 = new List<Pattern1>();
		foreach (var isRow in (true, false))
		{
			var (@base, fullHousesMask) = isRow ? (9, AllRowsMask) : (18, AllColumnsMask);
			foreach (var lineOffsetPair in lineOffsets)
			{
				var (l1, l2, l3) = (lineOffsetPair[0] + @base, lineOffsetPair[1] + @base, lineOffsetPair[2] + @base);
				var linesMask = 1 << l1 | 1 << l2;
				foreach (var cornerHouse in fullHousesMask & ~linesMask & ~(1 << l3))
				{
					foreach (var posPair in lineOffsets)
					{
						patternsForCase1.Add(new([HouseCells[cornerHouse][posPair[0]], HouseCells[cornerHouse][posPair[1]]], linesMask));
					}
				}
			}
		}
		PatternsForCase1 = [.. patternsForCase1];

		// Case 2: 2 rows + 2 columns.
		var patternsForCase2 = new List<Pattern2>();
		scoped var rows = AllRowsMask.GetAllSets();
		scoped var columns = AllColumnsMask.GetAllSets();
		foreach (var lineOffsetPairRow in lineOffsets)
		{
			var rowsMask = 1 << rows[lineOffsetPairRow[0]] | 1 << rows[lineOffsetPairRow[1]];
			foreach (var lineOffsetPairColumn in lineOffsets)
			{
				var columnsMask = 1 << columns[lineOffsetPairColumn[0]] | 1 << columns[lineOffsetPairColumn[1]];
				patternsForCase2.Add(new(rowsMask, columnsMask));
			}
		}
		PatternsForCase2 = [.. patternsForCase2];
	}


	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		// Handle for case 1.
		foreach (var pattern in PatternsForCase1)
		{
			if (Collect(ref context, in pattern) is { } step)
			{
				return step;
			}
		}

		// Handle for case 2.
		foreach (var pattern in PatternsForCase2)
		{
			if (Collect(ref context, in pattern) is { } step)
			{
				return step;
			}
		}

		return null;
	}

	/// <summary>
	/// <inheritdoc cref="StepSearcher.Collect(ref AnalysisContext)" path="/summary"/>
	/// </summary>
	/// <param name="context"><inheritdoc cref="StepSearcher.Collect(ref AnalysisContext)" path="/param[@name='context']"/></param>
	/// <param name="pattern">The target pattern.</param>
	/// <returns><inheritdoc cref="StepSearcher.Collect(ref AnalysisContext)" path="/returns"/></returns>
	private QiuDeadlyPatternStep? Collect(scoped ref AnalysisContext context, scoped ref readonly Pattern1 pattern)
	{
		scoped ref readonly var grid = ref context.Grid;

		// We should check for the distinction for the lines.
		var lines = pattern.Lines;
		var l1 = TrailingZeroCount(lines);
		var l2 = lines.GetNextSet(l1);
		var valueCellsInBothLines = CellMap.Empty;
		foreach (var cell in HousesMap[l1] | HousesMap[l2])
		{
			if (grid.GetState(cell) != CellState.Empty)
			{
				valueCellsInBothLines.Add(cell);
			}
		}

		if (pattern.Corner - EmptyCells)
		{
			// Corners cannot be non-empty.
			return null;
		}

		var isRow = l1 is >= 9 and < 18;
		if (CheckForBaseType(ref context, in grid, in pattern, in valueCellsInBothLines, isRow) is { } type1Step)
		{
			return type1Step;
		}

		return null;
	}

	/// <summary>
	/// <inheritdoc cref="StepSearcher.Collect(ref AnalysisContext)" path="/summary"/>
	/// </summary>
	/// <param name="context"><inheritdoc cref="StepSearcher.Collect(ref AnalysisContext)" path="/param[@name='context']"/></param>
	/// <param name="pattern">The target pattern.</param>
	/// <returns><inheritdoc cref="StepSearcher.Collect(ref AnalysisContext)" path="/returns"/></returns>
	private QiuDeadlyPatternStep? Collect(scoped ref AnalysisContext context, scoped ref readonly Pattern2 pattern)
	{
		// TODO: Re-implement later.
		return null;
	}

	/// <summary>
	/// Check for base type.
	/// </summary>
	private QiuDeadlyPatternStep? CheckForBaseType(
		scoped ref AnalysisContext context,
		scoped ref readonly Grid grid,
		scoped ref readonly Pattern1 pattern,
		scoped ref readonly CellMap valueCellsInBothLines,
		bool isRow
	)
	{
		var lines = pattern.Lines;
		var l1 = TrailingZeroCount(lines);
		var l2 = lines.GetNextSet(l1);

		// Check whether both two lines are finished.
		if (((HousesMap[l1] | HousesMap[l2]) - EmptyCells).Count == 18)
		{
			goto FastReturn;
		}

		var alignedPosMask = (Mask)0;
		var (l1AlignedMask, l2AlignedMask) = ((Mask)0, (Mask)0);
		for (var pos = 0; pos < 9; pos++)
		{
			if (valueCellsInBothLines.Contains(HouseCells[l1][pos]))
			{
				alignedPosMask |= (Mask)(1 << pos);
				l1AlignedMask |= (Mask)(1 << pos);
			}
			if (valueCellsInBothLines.Contains(HouseCells[l2][pos]))
			{
				alignedPosMask |= (Mask)(1 << pos);
				l2AlignedMask |= (Mask)(1 << pos);
			}
		}
		if (PopCount((uint)(l1AlignedMask | l2AlignedMask)) > Math.Max(PopCount((uint)l1AlignedMask), PopCount((uint)l2AlignedMask)))
		{
			// Distinction is not 1.
			goto FastReturn;
		}

		// Check whether the paired cells contain at least 2 empty cells and not in a block.
		var emptyCellsInPairedCells = CellMap.Empty;
		foreach (var pos in alignedPosMask)
		{
			if (HouseCells[l1][pos] is var cell1 && grid.GetState(cell1) == CellState.Empty)
			{
				emptyCellsInPairedCells.Add(cell1);
			}
			if (HouseCells[l2][pos] is var cell2 && grid.GetState(cell2) == CellState.Empty)
			{
				emptyCellsInPairedCells.Add(cell2);
			}
		}
		if (emptyCellsInPairedCells.Count >= 2)
		{
			// Distinction is not 1.
			goto FastReturn;
		}

		// Check whether the digits contain only 1 digit different.
		var nonEmptyCellsDigitsMaskForLine1 = grid[valueCellsInBothLines & HousesMap[l1]];
		var nonEmptyCellsDigitsMaskForLine2 = grid[valueCellsInBothLines & HousesMap[l2]];
		if (PopCount((uint)(nonEmptyCellsDigitsMaskForLine1 ^ nonEmptyCellsDigitsMaskForLine2)) >= 2)
		{
			goto FastReturn;
		}

		// Check whether at least 2 cells is empty in cross-line.
		var crossline = pattern.Crossline;
		if ((crossline - EmptyCells).Count >= 3)
		{
			goto FastReturn;
		}

		// Check whether the number of locked digits appeared in cross-line is at least 2.
		crossline.InOneHouse(out var block);
		var allDigitsMaskNotAppearedInCrossline = grid[HousesMap[block] - crossline];
		var allDigitsMaskAppearedInCrossline = grid[in crossline];
		var cornerLockedDigitsMask = (Mask)(allDigitsMaskAppearedInCrossline & ~allDigitsMaskNotAppearedInCrossline);
		if (PopCount((uint)cornerLockedDigitsMask) < 2)
		{
			goto FastReturn;
		}

		var corner = pattern.Corner;

		// Check whether corner cells intersect with at least 1 digit. e.g. Digits [1, 2, 3] & [1, 4].
		// This is necessary because we should guarantee the target eliminations should cover at least one digit.
		// For example, digits [1, 2, 3] and [1, 4] intersect with digit [1]. If the cross-line locks the digit [1, 2],
		// then we can conclude that if 1 is set in [1, 4] and 2 is set in [1, 2, 3], the target pattern will form a deadly pattern.
		var cornerDigitsMaskIntersected = grid[in corner, false, GridMaskMergingMethod.And];
		if (cornerDigitsMaskIntersected == 0)
		{
			goto FastReturn;
		}

		var cornerDigitsMask = grid[in corner];

		// Okay. Now we have a rigorous pattern. Now check for mask and determine its sub-type that can be categorized.
		// Part 1: check for corner cells.
		var maskIntersected = cornerLockedDigitsMask & cornerDigitsMaskIntersected;
		if (maskIntersected == cornerDigitsMaskIntersected)
		{
			// One cell only holds those two and the other doesn't only hold them.
			var cornerExtraDigitsMask = (Mask)(cornerDigitsMask & ~cornerDigitsMaskIntersected);
			var cornerContainingExtraDigit = corner;
			var tempMap = CellMap.Empty;
			foreach (var digit in cornerExtraDigitsMask)
			{
				tempMap |= CandidatesMap[digit];
			}
			cornerContainingExtraDigit &= tempMap;

			if (BaseType_Type1(
				ref context, in corner, in crossline, in grid, l1, l2,
				cornerDigitsMaskIntersected, in cornerContainingExtraDigit) is { } type1Step)
			{
				return type1Step;
			}

			if (BaseType_Type2(
				ref context, in corner, in crossline, in grid, l1, l2, cornerDigitsMaskIntersected,
				cornerExtraDigitsMask, in cornerContainingExtraDigit) is { } type2Step)
			{
				return type2Step;
			}

			if (BaseType_Type3(
				ref context, in corner, in crossline, in grid, l1, l2, cornerDigitsMaskIntersected,
				cornerExtraDigitsMask, in cornerContainingExtraDigit) is { } type3Step)
			{
				return type3Step;
			}

			if (BaseType_Type4(
				ref context, in corner, in crossline, l1, l2,
				cornerDigitsMaskIntersected, in cornerContainingExtraDigit) is { } type4Step)
			{
				return type4Step;
			}
		}
		else if (maskIntersected != 0)
		{
			if (BaseType_TypeLocked(ref context, in corner, in crossline, in grid, l1, l2, cornerLockedDigitsMask) is { } typeLockedStep)
			{
				return typeLockedStep;
			}
		}

		// Part 2: check for mirror cells.
		if ((grid[in crossline] & cornerDigitsMask) == cornerDigitsMask)
		{
			// Check whether the number of empty cross-line cells are same as the number of locked digits.
			if (PopCount((uint)cornerDigitsMask) > (crossline & EmptyCells).Count)
			{
				return null;
			}

			var mirror = pattern.MirrorStrict;
			if (BaseType_ExternalType1(ref context, in corner, in crossline, in mirror, in grid, l1, l2, cornerDigitsMask) is { } externalType1Step)
			{
				return externalType1Step;
			}

			if (BaseType_ExternalType2(ref context, in corner, in crossline, in mirror, in grid, l1, l2, cornerDigitsMask) is { } externalType2Step)
			{
				return externalType2Step;
			}
		}

	FastReturn:
		// No valid steps found. Return null.
		return null;
	}

	/// <summary>
	/// Check for base type (type 1).
	/// </summary>
	/// <param name="context"><inheritdoc cref="StepSearcher.Collect(ref AnalysisContext)" path="/param[@name='context']"/></param>
	/// <param name="corner">Indicates the corner cells used in a pattern.</param>
	/// <param name="crossline">Indicates the cross-line cells used in a pattern.</param>
	/// <param name="grid">The grid used.</param>
	/// <param name="l1">The line 1.</param>
	/// <param name="l2">The line 2.</param>
	/// <param name="digitsMaskAppearedInCorner">A mask value that holds a list of digits appeared in corner cells.</param>
	/// <param name="cornerContainingExtraDigit">Indicates the cells which are corner cells and contain extra digit.</param>
	/// <returns><inheritdoc cref="StepSearcher.Collect(ref AnalysisContext)" path="/returns"/></returns>
	/// <example>
	/// Test example:
	/// <code><![CDATA[
	/// 12...+4+35..731.5..+4+4...23..1...8..41.+8+145.2..3...4.+158.....17.+49.913+48..+574..+5.+13.:715 632 833 341 641 543 643 645 955 661 663 665 271 671 672 673 299
	/// .48.163.736...2.4.5..3.....+4.....+1.8...451+7..1.....+4..+7....9..4.2.1...736.478.21+9:918 938 862 574
	/// +4+6+2+9+517+38.7.+843..6.+83+62+7..474+8+3+6+25.....7+89+6+4...6+4+1+5.878.+7+13+64.+53..57+4+86.6+542+9+8.+7.:928 938 252 159 262 967 189
	/// 1+6.72+48.....65.....57....6+4.2647+59+1....+1.+2+6...+71.8654+2+79..+6.42+1+6...19+7...+18.47+3+96:326 329 331 931 359 374 583 584 884
	/// 5....4+762..+4...5+9.....6.41.2.+7.5..34..6+48+32+7.43..7...9.72.9..+4...1.+4....84.3.+6..7:321 126 331 942 144 867 677 379 879 687 588 389 889
	/// 1+85+43.62.+9.4.8....7....1...6..8....+1+2186+9473+5+5....3..6+8..3....4+4...2.3..+351+74.9.2:222 234 537 938 942 746 962 467 672 973 578 983 688
	/// +412..39+5+7+3+97.4+5..86+8+5..+7.4+3.+2..7.38..+3.5.1....69+32.....5..+3...19+7..5.8+3..+437..59.:624 227 161 274 876 284
	/// ..+3......+2.+7...659+95..27..3+4398+16+5+27.2.975.3.1+7+52+4398+67+8+246+1+39+5561......+3+9+4....+6.:815 816 117 417 418 119 419 137
	/// .+2.75.....9.+3...2.5+8392...1+47+2+59+3186.+3.+8...+5.856+21..3.2+1.+4356.8+36+8+1+7+9.4..+4+5+682.+1.:416 616 426 626 727 729 457 957 959
	/// ]]></code>
	/// </example>
	private QiuDeadlyPatternType1Step? BaseType_Type1(
		scoped ref AnalysisContext context,
		scoped ref readonly CellMap corner,
		scoped ref readonly CellMap crossline,
		scoped ref readonly Grid grid,
		House l1,
		House l2,
		Mask digitsMaskAppearedInCorner,
		scoped ref readonly CellMap cornerContainingExtraDigit
	)
	{
		if (cornerContainingExtraDigit is not [var targetCell])
		{
			return null;
		}

		var elimDigitsMask = (Mask)(grid.GetCandidates(targetCell) & digitsMaskAppearedInCorner);
		var candidateOffsets = new List<CandidateViewNode>();
		foreach (var digit in digitsMaskAppearedInCorner)
		{
			foreach (var cell in CandidatesMap[digit] & crossline)
			{
				candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, cell * 9 + digit));
			}
		}

		var theOtherCornerCellNoElimination = (corner - targetCell)[0];
		foreach (var digit in grid.GetCandidates(theOtherCornerCellNoElimination))
		{
			candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, theOtherCornerCellNoElimination * 9 + digit));
		}

		var step = new QiuDeadlyPatternType1Step(
			[.. from digit in elimDigitsMask select new Conclusion(Elimination, targetCell, digit)],
			[
				[
					.. candidateOffsets,
					new HouseViewNode(WellKnownColorIdentifier.Normal, l1),
					new HouseViewNode(WellKnownColorIdentifier.Normal, l2),
					new CellViewNode(WellKnownColorIdentifier.Normal, corner[0]),
					new CellViewNode(WellKnownColorIdentifier.Normal, corner[1])
				]
			],
			context.PredefinedOptions,
			true,
			1 << l1 | 1 << l2,
			corner[0],
			corner[1],
			targetCell,
			elimDigitsMask
		);
		if (context.OnlyFindOne)
		{
			return step;
		}

		context.Accumulator.Add(step);
		return null;
	}

	/// <summary>
	/// Check for base type (type 2).
	/// </summary>
	/// <param name="context"><inheritdoc cref="StepSearcher.Collect(ref AnalysisContext)" path="/param[@name='context']"/></param>
	/// <param name="corner">Indicates the corner cells used in a pattern.</param>
	/// <param name="crossline">Indicates the cross-line cells used in a pattern.</param>
	/// <param name="grid">The grid used.</param>
	/// <param name="l1">The line 1.</param>
	/// <param name="l2">The line 2.</param>
	/// <param name="digitsMaskAppearedInCorner">A mask value that holds a list of digits appeared in corner cells.</param>
	/// <param name="extraDigitsMask">Indicates a mask value that holds extra digits.</param>
	/// <param name="cornerContainingExtraDigit">Indicates the cells which are corner cells and contain extra digit.</param>
	/// <returns><inheritdoc cref="StepSearcher.Collect(ref AnalysisContext)" path="/returns"/></returns>
	private QiuDeadlyPatternType2Step? BaseType_Type2(
		scoped ref AnalysisContext context,
		scoped ref readonly CellMap corner,
		scoped ref readonly CellMap crossline,
		scoped ref readonly Grid grid,
		House l1,
		House l2,
		Mask digitsMaskAppearedInCorner,
		Mask extraDigitsMask,
		scoped ref readonly CellMap cornerContainingExtraDigit
	)
	{
		if (!IsPow2(extraDigitsMask))
		{
			return null;
		}

		var extraDigit = Log2((uint)extraDigitsMask);
		var elimMap = corner % CandidatesMap[extraDigit];
		if (!elimMap)
		{
			return null;
		}

		var candidateOffsets = new List<CandidateViewNode>();
		foreach (var cell in cornerContainingExtraDigit)
		{
			foreach (var digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add(
					new(
						digit == extraDigit ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal,
						cell * 9 + digit
					)
				);
			}
		}
		foreach (var digit in digitsMaskAppearedInCorner)
		{
			foreach (var cell in CandidatesMap[digit] & crossline)
			{
				candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, cell * 9 + digit));
			}
		}

		var step = new QiuDeadlyPatternType2Step(
			[.. from cell in elimMap select new Conclusion(Elimination, cell, extraDigit)],
			[
				[
					.. candidateOffsets,
					new HouseViewNode(WellKnownColorIdentifier.Normal, l1),
					new HouseViewNode(WellKnownColorIdentifier.Normal, l2),
					new CellViewNode(WellKnownColorIdentifier.Normal, corner[0]),
					new CellViewNode(WellKnownColorIdentifier.Normal, corner[1])
				]
			],
			context.PredefinedOptions,
			true,
			1 << l1 | 1 << l2,
			corner[0],
			corner[1],
			extraDigit
		);
		if (context.OnlyFindOne)
		{
			return step;
		}

		context.Accumulator.Add(step);
		return null;
	}

	/// <summary>
	/// Check for base type (type 3).
	/// </summary>
	/// <param name="context"><inheritdoc cref="StepSearcher.Collect(ref AnalysisContext)" path="/param[@name='context']"/></param>
	/// <param name="corner">Indicates the corner cells used in a pattern.</param>
	/// <param name="crossline">Indicates the cross-line cells used in a pattern.</param>
	/// <param name="grid">The grid used.</param>
	/// <param name="l1">The line 1.</param>
	/// <param name="l2">The line 2.</param>
	/// <param name="digitsMaskAppearedInCorner">A mask value that holds a list of digits appeared in corner cells.</param>
	/// <param name="cornerContainingExtraDigit">Indicates the cells which are corner cells and contain extra digit.</param>
	/// <param name="extraDigitsMask">Indicates a mask value that holds extra digits.</param>
	/// <returns><inheritdoc cref="StepSearcher.Collect(ref AnalysisContext)" path="/returns"/></returns>
	private QiuDeadlyPatternType3Step? BaseType_Type3(
		scoped ref AnalysisContext context,
		scoped ref readonly CellMap corner,
		scoped ref readonly CellMap crossline,
		scoped ref readonly Grid grid,
		House l1,
		House l2,
		Mask digitsMaskAppearedInCorner,
		Mask extraDigitsMask,
		scoped ref readonly CellMap cornerContainingExtraDigit
	)
	{
		if (IsPow2((uint)digitsMaskAppearedInCorner))
		{
			return null;
		}

		if (IsPow2((uint)extraDigitsMask))
		{
			return null;
		}

		if (cornerContainingExtraDigit != corner)
		{
			return null;
		}

		var size = PopCount((uint)digitsMaskAppearedInCorner);
		foreach (var cornerCellCoveredHouse in corner.CoveredHouses)
		{
			var emptyCellsInCurrentHouse = HousesMap[cornerCellCoveredHouse] & EmptyCells;
			var availableCellsToBeIterated = emptyCellsInCurrentHouse - corner;
			if (availableCellsToBeIterated.Count - 1 < size)
			{
				// No more cells or eliminations to exist.
				continue;
			}

			foreach (ref readonly var extraCells in emptyCellsInCurrentHouse.GetSubsets(size).AsReadOnlySpan())
			{
				var currentDigitsMask = grid[in extraCells];
				if (currentDigitsMask != extraDigitsMask)
				{
					continue;
				}

				var elimMap = emptyCellsInCurrentHouse - extraCells - corner;
				var conclusions = new List<Conclusion>();
				foreach (var digit in currentDigitsMask)
				{
					foreach (var cell in elimMap & CandidatesMap[digit])
					{
						conclusions.Add(new(Elimination, cell, digit));
					}
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>();
				foreach (var cell in corner)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								(extraDigitsMask >> digit & 1) != 0 ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal,
								cell * 9 + digit
							)
						);
					}
				}
				foreach (var d in digitsMaskAppearedInCorner)
				{
					foreach (var cell in CandidatesMap[d] & crossline)
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, cell * 9 + d));
					}
				}
				foreach (var cell in extraCells)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary3, cell * 9 + digit));
					}
				}

				var step = new QiuDeadlyPatternType3Step(
					[.. conclusions],
					[
						[
							.. candidateOffsets,
							new HouseViewNode(WellKnownColorIdentifier.Normal, l1),
							new HouseViewNode(WellKnownColorIdentifier.Normal, l2),
							new HouseViewNode(WellKnownColorIdentifier.Auxiliary1, cornerCellCoveredHouse),
							new CellViewNode(WellKnownColorIdentifier.Normal, corner[0]),
							new CellViewNode(WellKnownColorIdentifier.Normal, corner[1])
						]
					],
					context.PredefinedOptions,
					true,
					1 << l1 | 1 << l2,
					corner[0],
					corner[1],
					in extraCells,
					extraDigitsMask,
					true
				);
				if (context.OnlyFindOne)
				{
					return step;
				}

				context.Accumulator.Add(step);
			}
		}

		return null;
	}

	/// <summary>
	/// Check for base type (type 4).
	/// </summary>
	/// <param name="context"><inheritdoc cref="StepSearcher.Collect(ref AnalysisContext)" path="/param[@name='context']"/></param>
	/// <param name="corner">Indicates the corner cells used in a pattern.</param>
	/// <param name="crossline">Indicates the cross-line cells used in a pattern.</param>
	/// <param name="l1">The line 1.</param>
	/// <param name="l2">The line 2.</param>
	/// <param name="digitsMaskAppearedInCorner">A mask value that holds a list of digits appeared in corner cells.</param>
	/// <param name="cornerContainingExtraDigit">Indicates the cells which are corner cells and contain extra digit.</param>
	/// <returns><inheritdoc cref="StepSearcher.Collect(ref AnalysisContext)" path="/returns"/></returns>
	/// <example>
	/// Test examples:
	/// <code><![CDATA[
	/// 83+59+71+6+4+27.4+63+28+5..+2+6.5....4+5.+2+869+1...2.1.5.+8.+815+9...4....4......9.2.4+86+2+4+81+69+735:139 939 171 172 376
	/// ]]></code>
	/// </example>
	private QiuDeadlyPatternType4Step? BaseType_Type4(
		scoped ref AnalysisContext context,
		scoped ref readonly CellMap corner,
		scoped ref readonly CellMap crossline,
		House l1,
		House l2,
		Mask digitsMaskAppearedInCorner,
		scoped ref readonly CellMap cornerContainingExtraDigit
	)
	{
		foreach (var digit in digitsMaskAppearedInCorner)
		{
			foreach (var cornerCellCoveredHouse in (corner & CandidatesMap[digit]).CoveredHouses)
			{
				if ((CandidatesMap[digit] & HousesMap[cornerCellCoveredHouse]) == corner)
				{
					var conclusions = new List<Conclusion>();
					foreach (var elimDigit in (Mask)(digitsMaskAppearedInCorner & ~(1 << digit)))
					{
						foreach (var cell in CandidatesMap[elimDigit] & corner)
						{
							conclusions.Add(new(Elimination, cell, elimDigit));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<CandidateViewNode>();
					foreach (var d in digitsMaskAppearedInCorner)
					{
						foreach (var cell in CandidatesMap[d] & crossline)
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, cell * 9 + d));
						}
					}
					foreach (var cell in corner)
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, cell * 9 + digit));
					}

					var step = new QiuDeadlyPatternType4Step(
						[.. conclusions],
						[
							[
								.. candidateOffsets,
								new HouseViewNode(WellKnownColorIdentifier.Normal, l1),
								new HouseViewNode(WellKnownColorIdentifier.Normal, l2),
								new HouseViewNode(WellKnownColorIdentifier.Auxiliary1, cornerCellCoveredHouse),
								new CellViewNode(WellKnownColorIdentifier.Normal, corner[0]),
								new CellViewNode(WellKnownColorIdentifier.Normal, corner[1])
							]
						],
						context.PredefinedOptions,
						true,
						1 << l1 | 1 << l2,
						corner[0],
						corner[1],
						new(in corner, digit)
					);
					if (context.OnlyFindOne)
					{
						return step;
					}

					context.Accumulator.Add(step);
				}
			}
		}

		return null;
	}

	/// <summary>
	/// Check for base type (locked type).
	/// </summary>
	/// <param name="context"><inheritdoc cref="StepSearcher.Collect(ref AnalysisContext)" path="/param[@name='context']"/></param>
	/// <param name="corner">Indicates the corner cells used in a pattern.</param>
	/// <param name="crossline">Indicates the cross-line cells used in a pattern.</param>
	/// <param name="grid">The grid used.</param>
	/// <param name="l1">The line 1.</param>
	/// <param name="l2">The line 2.</param>
	/// <param name="cornerLockedDigitsMask">A mask value that holds digits locked in cross-line cells.</param>
	/// <returns><inheritdoc cref="StepSearcher.Collect(ref AnalysisContext)" path="/returns"/></returns>
	/// <example>
	/// Test examples:
	/// <code><![CDATA[
	/// .....+2+185..591..23.12.+5+3.7.3+8.52+1.46.....93+1.+14..3..5...12........1.....6.83.5.+9+1:613 713 913 931 751 652 752 259 677 783 983 687
	/// ...45....+6+58...+43...+46+8..+5.....7+6+9+4+5....+4178+646+7+8+9+5.+1......+4.2....9+3.5.4.+43..8...:224 129 171 871 172 872 173 177 677 179 779 979 191 991 194 294 195
	/// ]]></code>
	/// </example>
	private QiuDeadlyPatternLockedTypeStep? BaseType_TypeLocked(
		scoped ref AnalysisContext context,
		scoped ref readonly CellMap corner,
		scoped ref readonly CellMap crossline,
		scoped ref readonly Grid grid,
		House l1,
		House l2,
		Mask cornerLockedDigitsMask
	)
	{
		var currentDigitsMask = grid[in corner];
		if (PopCount((uint)currentDigitsMask) > 4)
		{
			// Corner cells hold at least 5 digits, which is disallowed in the pattern.
			return null;
		}

		var digitsShouldBeLocked = (Mask)(cornerLockedDigitsMask & currentDigitsMask);
		var currentDigitsNotLocked = (Mask)(currentDigitsMask & ~digitsShouldBeLocked);
		if (!IsPow2(currentDigitsNotLocked))
		{
			return null;
		}

		var elimDigit = Log2((uint)currentDigitsNotLocked);
		var elimMap = crossline & CandidatesMap[elimDigit];
		if (!elimMap)
		{
			return null;
		}

		var candidateOffsets = new List<CandidateViewNode>();
		foreach (var digit in digitsShouldBeLocked)
		{
			foreach (var cell in CandidatesMap[digit] & crossline)
			{
				candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, cell * 9 + digit));
			}
		}
		foreach (var cell in corner)
		{
			foreach (var digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add(
					new(
						(digitsShouldBeLocked >> digit & 1) != 0 ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal,
						cell * 9 + digit
					)
				);
			}
		}

		crossline.InOneHouse(out var house);
		var lockedMap = CandidateMap.Empty;
		foreach (var digit in currentDigitsMask)
		{
			foreach (var cell in HousesMap[house] & CandidatesMap[digit])
			{
				lockedMap.Add(cell * 9 + digit);
			}
		}

		var step = new QiuDeadlyPatternLockedTypeStep(
			[.. from cell in elimMap select new Conclusion(Elimination, cell, elimDigit)],
			[
				[
					.. candidateOffsets,
					new HouseViewNode(WellKnownColorIdentifier.Normal, l1),
					new HouseViewNode(WellKnownColorIdentifier.Normal, l2),
					new HouseViewNode(WellKnownColorIdentifier.Auxiliary1, house),
					new CellViewNode(WellKnownColorIdentifier.Normal, corner[0]),
					new CellViewNode(WellKnownColorIdentifier.Normal, corner[1])
				]
			],
			context.PredefinedOptions,
			true,
			1 << l1 | 1 << l2,
			corner[0],
			corner[1],
			in lockedMap
		);
		if (context.OnlyFindOne)
		{
			return step;
		}

		context.Accumulator.Add(step);
		return null;
	}

	/// <summary>
	/// Check for base type (external type 1).
	/// </summary>
	/// <param name="context"><inheritdoc cref="StepSearcher.Collect(ref AnalysisContext)" path="/param[@name='context']"/></param>
	/// <param name="corner">Indicates the corner cells used in a pattern.</param>
	/// <param name="crossline">Indicates the cross-line cells used in a pattern.</param>
	/// <param name="mirror">Indicates the mirror cells used in a pattern.</param>
	/// <param name="grid">The grid used.</param>
	/// <param name="l1">The line 1.</param>
	/// <param name="l2">The line 2.</param>
	/// <param name="externalDigitsMaskToBeChecked">A mask value that holds digits should be checked.</param>
	/// <returns><inheritdoc cref="StepSearcher.Collect(ref AnalysisContext)" path="/returns"/></returns>
	/// <example>
	/// Test examples:
	/// <code><![CDATA[
	/// ...45....+6+58...+43...+46+8..+5.....7+6+9+4+5....+4178+646+7+8+9+5.+1......+4.2....9+3.5.4.+43..8...:224 129 171 871 172 872 173 177 677 179 779 979 191 991 194 294 195
	/// ]]></code>
	/// </example>
	private QiuDeadlyPatternExternalType1Step? BaseType_ExternalType1(
		scoped ref AnalysisContext context,
		scoped ref readonly CellMap corner,
		scoped ref readonly CellMap crossline,
		scoped ref readonly CellMap mirror,
		scoped ref readonly Grid grid,
		House l1,
		House l2,
		Mask externalDigitsMaskToBeChecked
	)
	{
		if ((mirror & EmptyCells) is not [var targetCell])
		{
			return null;
		}

		var elimDigits = (Mask)(grid.GetCandidates(targetCell) & ~externalDigitsMaskToBeChecked);
		if (elimDigits == 0)
		{
			return null;
		}

		var candidateOffsets = new List<CandidateViewNode>();
		foreach (var cell in crossline & EmptyCells)
		{
			foreach (var digit in (Mask)(grid.GetCandidates(cell) & externalDigitsMaskToBeChecked))
			{
				candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, cell * 9 + digit));
			}
		}
		foreach (var cell in corner)
		{
			foreach (var digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
			}
		}

		var step = new QiuDeadlyPatternExternalType1Step(
			[.. from digit in elimDigits select new Conclusion(Elimination, targetCell, digit)],
			[
				[
					.. candidateOffsets,
					new HouseViewNode(WellKnownColorIdentifier.Normal, l1),
					new HouseViewNode(WellKnownColorIdentifier.Normal, l2),
					new CellViewNode(WellKnownColorIdentifier.Normal, corner[0]),
					new CellViewNode(WellKnownColorIdentifier.Normal, corner[1])
				]
			],
			context.PredefinedOptions,
			true,
			1 << l1 | 1 << l2,
			corner[0],
			corner[1],
			targetCell,
			externalDigitsMaskToBeChecked
		);
		if (context.OnlyFindOne)
		{
			return step;
		}

		context.Accumulator.Add(step);
		return null;
	}

	/// <summary>
	/// Check for base type (external type 2).
	/// </summary>
	/// <param name="context"><inheritdoc cref="StepSearcher.Collect(ref AnalysisContext)" path="/param[@name='context']"/></param>
	/// <param name="corner">Indicates the corner cells used in a pattern.</param>
	/// <param name="crossline">Indicates the cross-line cells used in a pattern.</param>
	/// <param name="mirror">Indicates the mirror cells used in a pattern.</param>
	/// <param name="grid">The grid used.</param>
	/// <param name="l1">The line 1.</param>
	/// <param name="l2">The line 2.</param>
	/// <param name="externalDigitsMaskToBeChecked">A mask value that holds digits should be checked.</param>
	/// <returns><inheritdoc cref="StepSearcher.Collect(ref AnalysisContext)" path="/returns"/></returns>
	/// <example>
	/// Test examples:
	/// <code><![CDATA[
	/// ...45....+6+58...+43...+46+8..+5.....7+6+9+4+5....+4178+646+7+8+9+5.+1......+4.2....9+3.5.4.+43..8...:224 129 171 871 172 872 173 177 677 179 779 979 191 991 194 294 195
	/// ]]></code>
	/// </example>
	private QiuDeadlyPatternExternalType2Step? BaseType_ExternalType2(
		scoped ref AnalysisContext context,
		scoped ref readonly CellMap corner,
		scoped ref readonly CellMap crossline,
		scoped ref readonly CellMap mirror,
		scoped ref readonly Grid grid,
		House l1,
		House l2,
		Mask externalDigitsMaskToBeChecked
	)
	{
		var elimDigits = (Mask)(grid[in mirror] & externalDigitsMaskToBeChecked);
		if (!IsPow2(elimDigits))
		{
			return null;
		}

		var elimDigit = Log2((uint)elimDigits);
		var elimMap = mirror.PeerIntersection & CandidatesMap[elimDigit];
		if (!elimMap)
		{
			return null;
		}

		var candidateOffsets = new List<CandidateViewNode>();
		foreach (var cell in crossline & EmptyCells)
		{
			foreach (var digit in (Mask)(grid.GetCandidates(cell) & externalDigitsMaskToBeChecked))
			{
				candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, cell * 9 + digit));
			}
		}
		foreach (var cell in corner)
		{
			foreach (var digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
			}
		}
		foreach (var cell in mirror)
		{
			candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, cell * 9 + elimDigit));
		}

		var step = new QiuDeadlyPatternExternalType2Step(
			[.. from cell in elimMap select new Conclusion(Elimination, cell, elimDigit)],
			[
				[
					.. candidateOffsets,
					new HouseViewNode(WellKnownColorIdentifier.Normal, l1),
					new HouseViewNode(WellKnownColorIdentifier.Normal, l2),
					new CellViewNode(WellKnownColorIdentifier.Normal, corner[0]),
					new CellViewNode(WellKnownColorIdentifier.Normal, corner[1])
				]
			],
			context.PredefinedOptions,
			true,
			1 << l1 | 1 << l2,
			corner[0],
			corner[1],
			in mirror,
			elimDigit
		);
		if (context.OnlyFindOne)
		{
			return step;
		}

		context.Accumulator.Add(step);
		return null;
	}


	/// <summary>
	/// Defines a pattern that is a Qiu's deadly pattern technique pattern in theory. The sketch is like:
	/// <code><![CDATA[
	/// .-------.-------.-------.
	/// | . . . | . . . | . . . |
	/// | . . . | . . . | . . . |
	/// | P P . | . . . | . . . |
	/// :-------+-------+-------:
	/// | S S B | B B B | B B B |
	/// | S S B | B B B | B B B |
	/// | . . . | . . . | . . . |
	/// :-------+-------+-------:
	/// | . . . | . . . | . . . |
	/// | . . . | . . . | . . . |
	/// | . . . | . . . | . . . |
	/// '-------'-------'-------'
	/// ]]></code>
	/// Where:
	/// <list type="table">
	/// <item><term>P</term><description>Corner Cells.</description></item>
	/// <item><term>S</term><description>Cross-line Cells.</description></item>
	/// <item><term>B</term><description>Base-line Cells.</description></item>
	/// </list>
	/// </summary>
	/// <param name="Corner">The corner cells that is <c>P</c> in that sketch.</param>
	/// <param name="Lines">The base-line cells that is <c>B</c> in that sketch.</param>
	private readonly record struct Pattern1(scoped ref readonly CellMap Corner, HouseMask Lines)
	{
		/// <summary>
		/// Indicates the crossline cells.
		/// </summary>
		public CellMap Crossline
		{
			get
			{
				var l1 = TrailingZeroCount(Lines);
				var l2 = Lines.GetNextSet(l1);
				return (HousesMap[l1] | HousesMap[l2]) & PeersMap[Corner[0]] | (HousesMap[l1] | HousesMap[l2]) & PeersMap[Corner[1]];
			}
		}

		/// <summary>
		/// Indicates the mirror cells.
		/// </summary>
		public CellMap Mirror
		{
			get
			{
				Crossline.InOneHouse(out var block);
				var l1 = TrailingZeroCount(Lines);
				var l2 = Lines.GetNextSet(l1);
				return HousesMap[block] - (HousesMap[l1] | HousesMap[l2]);
			}
		}

		/// <summary>
		/// Indicates the mirror cells, with strict check.
		/// </summary>
		public CellMap MirrorStrict => Mirror & Corner.ExpandedPeers;
	}

	/// <summary>
	/// Defines a pattern that is a Qiu's deadly pattern technique pattern in theory. The sketch is like:
	/// <code><![CDATA[
	/// .-------.-------.-------.
	/// | B B . | . . . | . . . |
	/// | B B . | . . . | . . . |
	/// | B B . | . . . | . . . |
	/// :-------+-------+-------:
	/// | S S B | B B B | B B B |
	/// | S S B | B B B | B B B |
	/// | B B . | . . . | . . . |
	/// :-------+-------+-------:
	/// | B B . | . . . | . . . |
	/// | B B . | . . . | . . . |
	/// | B B . | . . . | . . . |
	/// '-------'-------'-------'
	/// ]]></code>
	/// Where:
	/// <list type="table">
	/// <item><term>S</term><description>Cross-line Cells.</description></item>
	/// <item><term>B</term><description>Base-line Cells.</description></item>
	/// </list>
	/// </summary>
	/// <param name="Lines1">The first pair of lines.</param>
	/// <param name="Lines2">The second pair of lines.</param>
	private readonly record struct Pattern2(HouseMask Lines1, HouseMask Lines2)
	{
		/// <summary>
		/// Indicates the crossline cells.
		/// </summary>
		public CellMap Crossline
		{
			get
			{
				var l11 = TrailingZeroCount(Lines1);
				var l21 = Lines1.GetNextSet(l11);
				var l12 = TrailingZeroCount(Lines2);
				var l22 = Lines2.GetNextSet(l12);
				var result = CellMap.Empty;
				foreach (var (a, b) in ((l11, l12), (l11, l22), (l21, l12), (l21, l22)))
				{
					result |= HousesMap[a] & HousesMap[b];
				}

				return result;
			}
		}
	}
}
