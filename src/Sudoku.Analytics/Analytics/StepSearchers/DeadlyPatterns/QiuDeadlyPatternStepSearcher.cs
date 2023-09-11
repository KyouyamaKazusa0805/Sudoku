using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Metadata;
using Sudoku.Analytics.Steps;
using Sudoku.Rendering;
using Sudoku.Rendering.Nodes;

namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Qiu's Deadly Pattern</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Qiu's Deadly Pattern Type 1</item>
/// <item>Qiu's Deadly Pattern Type 2</item>
/// <item>Qiu's Deadly Pattern Type 3</item>
/// <item>Qiu's Deadly Pattern Type 4</item>
/// <item>Qiu's Deadly Pattern Locked Type</item>
/// </list>
/// </summary>
[StepSearcher(
	Technique.QiuDeadlyPatternType1, Technique.QiuDeadlyPatternType2,
	Technique.QiuDeadlyPatternType3, Technique.QiuDeadlyPatternType4, Technique.LockedQiuDeadlyPattern,
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
		var lineOffsets = (int[][])[[0, 1, 2], [0, 2, 1], [1, 2, 0], [3, 4, 5], [3, 5, 4], [4, 5, 3], [6, 7, 8], [6, 8, 7], [7, 8, 6]];
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
			if (Collect(ref context, pattern) is { } step)
			{
				return step;
			}
		}

		// Handle for case 2.
		foreach (var pattern in PatternsForCase2)
		{
			if (Collect(ref context, pattern) is { } step)
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
	private QiuDeadlyPatternStep? Collect(scoped ref AnalysisContext context, scoped in Pattern1 pattern)
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
		if (CheckForBaseType(ref context, grid, pattern, valueCellsInBothLines, isRow) is { } type1Step)
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
	private QiuDeadlyPatternStep? Collect(scoped ref AnalysisContext context, scoped in Pattern2 pattern)
	{
		// TODO: Re-implement later.
		return null;
	}

	/// <summary>
	/// Check for base type.
	/// </summary>
	/// <example>
	/// Test example:
	/// <code><![CDATA[
	/// # Type 1, 2, 4
	/// 12...+4+35..731.5..+4+4...23..1...8..41.+8+145.2..3...4.+158.....17.+49.913+48..+574..+5.+13.:715 632 833 341 641 543 643 645 955 661 663 665 271 671 672 673 299
	/// ]]></code>
	/// </example>
	private QiuDeadlyPatternStep? CheckForBaseType(
		scoped ref AnalysisContext context,
		scoped in Grid grid,
		scoped in Pattern1 pattern,
		scoped in CellMap valueCellsInBothLines,
		bool isRow
	)
	{
		// Check whether the distinction is 1.
		var lines = pattern.Lines;
		var l1 = TrailingZeroCount(lines);
		var l2 = lines.GetNextSet(l1);
		var columnAlignedMask = (Mask)0;
		var (l1AlignedMask, l2AlignedMask) = ((Mask)0, (Mask)0);
		for (var pos = 0; pos < 9; pos++)
		{
			if (valueCellsInBothLines.Contains(HouseCells[l1][pos]))
			{
				columnAlignedMask |= (Mask)(1 << pos);
				l1AlignedMask |= (Mask)(1 << pos);
			}
			if (valueCellsInBothLines.Contains(HouseCells[l2][pos]))
			{
				columnAlignedMask |= (Mask)(1 << pos);
				l2AlignedMask |= (Mask)(1 << pos);
			}
		}
		if (PopCount((uint)(l1AlignedMask | l2AlignedMask)) > Math.Max(PopCount((uint)l1AlignedMask), PopCount((uint)l2AlignedMask)))
		{
			// Distinction is not 1.
			return null;
		}

		var crossline = pattern.Crossline;
		crossline.InOneHouse(out var block);
		var allDigitsMaskNotAppearedInCrossline = grid[HousesMap[block] - crossline];
		var allDigitsMaskAppearedInCrossline = grid[crossline];
		var digitsMask = allDigitsMaskAppearedInCrossline & ~allDigitsMaskNotAppearedInCrossline;
		if (PopCount((uint)digitsMask) < 2)
		{
			return null;
		}

		var corner = pattern.Corner;
		var digitsMaskAppearedInCornerCells = grid[corner, false, GridMaskMergingMethod.And];
		if ((digitsMask & digitsMaskAppearedInCornerCells) != digitsMaskAppearedInCornerCells)
		{
			// Not all digits intersected in corner cells are hold in crossline cells.
			return null;
		}

		return checkFor4Types(ref context, grid, corner, crossline, l1, l2, digitsMaskAppearedInCornerCells) is { } foundStep ? foundStep : null;


		static QiuDeadlyPatternStep? checkFor4Types(
			scoped ref AnalysisContext context,
			scoped in Grid grid,
			scoped in CellMap corner,
			scoped in CellMap crossline,
			House l1,
			House l2,
			Mask digitsMaskAppearedInCornerCells
		)
		{
			// One cell only holds those two and the other doesn't only hold them.
			var (c1, c2) = (corner[0], corner[1]);
			var extraDigitsMask = (grid.GetCandidates(c1) | grid.GetCandidates(c2)) & ~digitsMaskAppearedInCornerCells;
			var cornerCellsContainingExtraDigit = corner;
			var tempMap = CellMap.Empty;
			foreach (var digit in extraDigitsMask)
			{
				tempMap |= CandidatesMap[digit];
			}
			cornerCellsContainingExtraDigit &= tempMap;

			if (cornerCellsContainingExtraDigit is [var targetCell])
			{
				// Type 1.
				var elimDigitsMask = (Mask)(grid.GetCandidates(targetCell) & digitsMaskAppearedInCornerCells);
				var candidateOffsets = new List<CandidateViewNode>();
				foreach (var digit in digitsMaskAppearedInCornerCells)
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
			}

			if (IsPow2(extraDigitsMask))
			{
				// Type 2.
				var extraDigit = Log2((uint)extraDigitsMask);
				var elimMap = corner % CandidatesMap[extraDigit];
				if (!elimMap)
				{
					return null;
				}

				var candidateOffsets = new List<CandidateViewNode>();
				foreach (var cell in cornerCellsContainingExtraDigit)
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
				foreach (var digit in digitsMaskAppearedInCornerCells)
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
			}

			foreach (var digit in digitsMaskAppearedInCornerCells)
			{
				foreach (var cornerCellCoveredHouse in (corner & CandidatesMap[digit]).CoveredHouses)
				{
					if ((CandidatesMap[digit] & HousesMap[cornerCellCoveredHouse]) == corner)
					{
						// Type 4.
						var conclusions = new List<Conclusion>();
						foreach (var elimDigit in (Mask)(digitsMaskAppearedInCornerCells & ~(1 << digit)))
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
						foreach (var d in digitsMaskAppearedInCornerCells)
						{
							foreach (var cell in CandidatesMap[d] & crossline)
							{
								candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, cell * 9 + d));
							}
						}
						foreach (var cell in cornerCellsContainingExtraDigit)
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
							true,
							1 << l1 | 1 << l2,
							corner[0],
							corner[1],
							new(corner, digit)
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
	private readonly record struct Pattern1(scoped in CellMap Corner, HouseMask Lines)
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
