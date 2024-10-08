namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with an <b>Anonymous Deadly Pattern</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Anonymous Deadly Pattern Type 1</item>
/// <item>Anonymous Deadly Pattern Type 2</item>
/// <item>Anonymous Deadly Pattern Type 3</item>
/// <item>Anonymous Deadly Pattern Type 4</item>
/// </list>
/// </summary>
[StepSearcher(
	"StepSearcherName_AnonymousDeadlyPatternStepSearcher",
	Technique.AnonymousDeadlyPatternType1, Technique.AnonymousDeadlyPatternType2,
	Technique.AnonymousDeadlyPatternType3, Technique.AnonymousDeadlyPatternType4,
	RuntimeFlags = StepSearcherRuntimeFlags.TimeComplexity,
	SupportedSudokuTypes = SudokuType.Standard,
	SupportAnalyzingMultipleSolutionsPuzzle = false)]
public sealed partial class AnonymousDeadlyPatternStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates all eight-cell patterns.
	/// </summary>
	private static readonly ReadOnlyMemory<CellMap> EightCellPatterns;


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static AnonymousDeadlyPatternStepSearcher()
	{
		// Construct patterns of 8 cells.
		// We can unify two patterns of 8 cells into one, by using extended rectangles.
		// Enumerate all possible XR patterns with 6 cells, and enumerate all cells.
		// For each cell, we will remove it and insert a unique rectangle pattern that covers the current cell.
		var eightCellPatterns = new HashSet<CellMap>();
		foreach (var pattern in ExtendedRectanglePattern.AllPatterns[..1620]) // [0..1620] -> XR size 6
		{
			var patternCells = pattern.PatternCells;
			foreach (var missingCell in patternCells)
			{
				// Determine which side has more cells (row or column).
				var lastPatternCells = patternCells - missingCell;
				var row = missingCell.ToHouse(HouseType.Row);
				var column = missingCell.ToHouse(HouseType.Column);
				var rowCells = patternCells & HousesMap[row];
				var columnCells = patternCells & HousesMap[column];
				var isRow = rowCells.Count > columnCells.Count;
				var targetHouse = pattern.IsFat ? isRow ? column : row : isRow ? row : column;
				var chute = default(Chute);
				foreach (ref readonly var c in Chutes.AsReadOnlySpan())
				{
					if ((c.HousesMask >> targetHouse & 1) != 0)
					{
						chute = c;
						break;
					}
				}

				// Iterate the other two houses that exclude the current cell, and find all possible cells.
				// For each cell, we should treat it as the diagonal cell of UR pattern with the missing cell.
				// If two cells are RxCy and RzCw, all 4 UR cells are RxCy, RxCw, RzCy and RzCw.
				var (x, y) = (missingCell / 9, missingCell % 9);
				foreach (var house in chute.HousesMask & ~(1 << targetHouse))
				{
					foreach (var cell in HousesMap[house])
					{
						if ((missingCell.AsCellMap() + cell).InOneHouse(out _))
						{
							continue;
						}

						// Add it into the whole pattern.
						var (z, w) = (cell / 9, cell % 9);
						var extraCells = cell.AsCellMap() + (x * 9 + w) + (z * 9 + y);
						if (lastPatternCells & extraCells)
						{
							continue;
						}

						eightCellPatterns.Add(lastPatternCells | extraCells);
					}
				}
			}
		}
		EightCellPatterns = eightCellPatterns.ToArray();
	}


	/// <inheritdoc/>
	protected internal override Step? Collect(ref StepAnalysisContext context)
	{
		if (Collect_8Cells(ref context) is { } eightCellsStep)
		{
			return eightCellsStep;
		}
		if (Collect_8Cells_Rotating(ref context) is { } eightCells2Step)
		{
			return eightCells2Step;
		}
		if (Collect_9Cells(ref context) is { } nineCellsStep)
		{
			return nineCellsStep;
		}
		return null;
	}

	/// <remarks>
	/// There're 2 patterns:
	/// <code><![CDATA[
	/// (1) UL + XR, 3 digits
	/// 12 .  .  |  23 .  .  |  13 .  .
	/// .  12 .  |  23 .  .  |  13 .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	/// ---------+-----------+---------
	/// 12 12 .  |  .  .  .  |  .  .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	///
	/// (2) XR + UR, 3 digits
	/// 12 23 .  |  13 .  .  |  .  .  .
	/// .  .  13 |  13 .  .  |  .  .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	/// ---------+-----------+---------
	/// 12 23 13 |  .  .  .  |  .  .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	/// ]]></code>
	/// </remarks>
	private AnonymousDeadlyPatternStep? Collect_8Cells(ref StepAnalysisContext context)
	{
		ref readonly var grid = ref context.Grid;
		foreach (ref readonly var pattern in EightCellPatterns)
		{
			if ((EmptyCells & pattern) != pattern)
			{
				// The pattern may contain non-empty cells. Skip it.
				continue;
			}

			// Check the number of digits appeared in the pattern, and determine which type it would be.
			var digitsMask = grid[in pattern];

			// The pattern may be very complex to be checked, so we should append an extra check here:
			// determine which digits can be used as a deadly pattern, and which are not.
			// Obviously, both patterns of size 8 should use 3 digits, and two of them must use >= 6 times,
			// and the last one must use >= 4 times.
			// We should check which digits can be used in pattern. If the number of found digits are not enough 3,
			// the pattern will also be ignored to be checked.
			var (greaterThan6Digits, greaterThan4Digits) = ((Mask)0, (Mask)0);
			foreach (var digit in digitsMask)
			{
				var m = (pattern & CandidatesMap[digit]).Count;
				if (m >= 6)
				{
					greaterThan6Digits |= (Mask)(1 << digit);
				}
				else if (m >= 4)
				{
					greaterThan4Digits |= (Mask)(1 << digit);
				}
			}
			if (Mask.PopCount(greaterThan6Digits) < 2
				|| (Mask)(greaterThan4Digits | greaterThan6Digits) is var possiblePatternDigitsMask
				&& Mask.PopCount(possiblePatternDigitsMask) < 3)
			{
				continue;
			}

			// Iterate on each combination.
			foreach (var combination in possiblePatternDigitsMask.GetAllSets().GetSubsets(3))
			{
				// Try to get all cells that holds extra digits.
				var extraDigitsMask = (Mask)(digitsMask & ~MaskOperations.Create(combination));
				var extraCells = CellMap.Empty;
				foreach (var cell in pattern)
				{
					if ((grid.GetCandidates(cell) & extraDigitsMask) != 0)
					{
						extraCells.Add(cell);
					}
				}
				if (extraCells.Count >= 2 && !extraCells.InOneHouse(out _))
				{
					// All extra cells must share with a same house.
					continue;
				}

				if (!VerifyPattern(in grid, in pattern, extraDigitsMask, out var p))
				{
					// The pattern cannot be passed to be verified.
					continue;
				}

				switch (Mask.PopCount(extraDigitsMask))
				{
					case 0:
					{
						throw new PuzzleInvalidException(in grid, typeof(AnonymousDeadlyPatternStep));
					}
					case 1:
					{
						var extraDigit = Mask.Log2(extraDigitsMask);
						if (CheckType1Or2(
							ref context, in grid, in pattern, digitsMask, extraDigit, in p,
							(pattern & CandidatesMap[extraDigit]).Count == 1
								? Technique.AnonymousDeadlyPatternType1
								: Technique.AnonymousDeadlyPatternType2) is { } type1Or2Step)
						{
							return type1Or2Step;
						}
						continue;
					}
					case 2:
					{
						if (CheckType3(
							ref context, in grid, in pattern, digitsMask,
							extraDigitsMask, in extraCells, in p, Technique.AnonymousDeadlyPatternType3) is { } type3Step)
						{
							return type3Step;
						}
						goto default;
					}
					default:
					{
						if (CheckType4(
							ref context, in grid, in pattern, digitsMask,
							extraDigitsMask, in extraCells, in p, Technique.AnonymousDeadlyPatternType4) is { } type4Step)
						{
							return type4Step;
						}
						break;
					}
				}
			}
		}
		return null;
	}

	/// <remarks>
	/// There's only 1 pattern:
	/// <code><![CDATA[
	/// 12 .  .  |  24 .  .  |  14 .  .
	/// 23 .  .  |  23 .  .  |  .  .  .
	/// 13 .  .  |  34 .  .  |  14 .  .
	/// ]]></code>
	/// </remarks>
	private AnonymousDeadlyPatternStep? Collect_8Cells_Rotating(ref StepAnalysisContext context)
	{
		// Test examples:
		// 17..9..83389......6..38..7..931287..8.1673...72695431891...2.37.3....29.26..39.51
		// +6+4+7...+2+89+2+914+687+35+5382+7+9..+1+726+8+9...+3..+31.79+2+6.+1..+2..783..+7....+2.72...+3.4+16+49+3285+7:564 466 566 575 176 576 678 586

		ref readonly var grid = ref context.Grid;

		// Iterate on each pattern. Here the pattern cells is same as patterns used in Unique Matrix pattern.
		// We should remove one cell from the pattern, to make the pattern become a possible Rotating Deadly Pattern.
		foreach (ref readonly var cells in UniqueMatrixStepSearcher.Patterns)
		{
			// Iterate on each cell as missing cell.
			foreach (var missingCell in cells)
			{
				// Check whether all last 8 cells are empty cells.
				var pattern = cells - missingCell;
				if ((EmptyCells & pattern) != pattern)
				{
					// The pattern may contain non-empty cells. Skip it.
					continue;
				}

				// Check the number of digits appeared in the pattern, and determine which type it would be.
				var digitsMask = grid[in pattern];

				// Adds an filter nearly same as anonymous deadly pattern, but with 4 different digits.
				var possiblePatternDigitsMask = (Mask)0;
				foreach (var digit in digitsMask)
				{
					if ((pattern & CandidatesMap[digit]).Count >= 4)
					{
						possiblePatternDigitsMask |= (Mask)(1 << digit);
					}
				}
				if (Mask.PopCount(possiblePatternDigitsMask) < 4)
				{
					continue;
				}

				// Iterate on each combination.
				foreach (var combination in possiblePatternDigitsMask.GetAllSets().GetSubsets(4))
				{
					// Try to get all cells that holds extra digits.
					var extraDigitsMask = (Mask)(digitsMask & ~MaskOperations.Create(combination));
					var extraCells = CellMap.Empty;
					foreach (var cell in pattern)
					{
						if ((grid.GetCandidates(cell) & extraDigitsMask) != 0)
						{
							extraCells.Add(cell);
						}
					}
					if (extraCells.Count >= 2 && !extraCells.InOneHouse(out _))
					{
						// All extra cells must share with a same house.
						continue;
					}

					if (!VerifyPattern(in grid, in pattern, extraDigitsMask, out var p))
					{
						// The pattern cannot be passed to be verified.
						continue;
					}

					switch (Mask.PopCount(extraDigitsMask))
					{
						case 0:
						{
							throw new PuzzleInvalidException(in grid, typeof(AnonymousDeadlyPatternStep));
						}
						case 1:
						{
							var extraDigit = Mask.Log2(extraDigitsMask);
							if (CheckType1Or2(
								ref context, in grid, in pattern, digitsMask, extraDigit, in p,
								(pattern & CandidatesMap[extraDigit]).Count == 1
									? Technique.RotatingDeadlyPatternType1
									: Technique.RotatingDeadlyPatternType2) is { } type1Or2Step)
							{
								return type1Or2Step;
							}
							continue;
						}
						case 2:
						{
							if (CheckType3(
								ref context, in grid, in pattern, digitsMask,
								extraDigitsMask, in extraCells, in p, Technique.RotatingDeadlyPatternType3) is { } type3Step)
							{
								return type3Step;
							}
							goto default;
						}
						default:
						{
							if (CheckType4(
								ref context, in grid, in pattern, digitsMask,
								extraDigitsMask, in extraCells, in p, Technique.RotatingDeadlyPatternType4) is { } type4Step)
							{
								return type4Step;
							}
							break;
						}
					}
				}
			}
		}
		return null;
	}

	/// <remarks>
	/// There're 3 patterns:
	/// <code><![CDATA[
	/// (1) 2URs + XR, 3 digits
	/// 12 .  .  |  12 .  .  |  .  .  .
	/// .  23 .  |  23 .  .  |  .  .  .
	/// .  .  13 |  13 .  .  |  .  .  .
	/// ---------+-----------+---------
	/// 12 23 13 |  .  .  .  |  .  .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	///
	/// (2) 2XRs, 4 digits
	/// 12 24 .  |  14 .  .  |  .  .  .
	/// 13 .  34 |  14 .  .  |  .  .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	/// ---------+-----------+---------
	/// 23 24 34 |  .  .  .  |  .  .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	///
	/// (3) BDP + UR, 3 digits
	/// 12 .  .  |  12 .  .  |  .  .  .
	/// 23 .  .  |  .  .  .  |  23 .  .
	/// .  13 .  |  12 .  .  |  23 .  .
	/// ---------+-----------+---------
	/// 13 13 .  |  .  .  .  |  .  .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	/// ]]></code>
	/// </remarks>
	private AnonymousDeadlyPatternStep? Collect_9Cells(ref StepAnalysisContext context)
	{
		return null;
	}

	/// <summary>
	/// Check for type 1 and 2.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="pattern">The pattern.</param>
	/// <param name="digitsMask">The digits used.</param>
	/// <param name="targetDigit">The target digit.</param>
	/// <param name="p">All candidates used.</param>
	/// <param name="technique">The technique.</param>
	/// <returns>The found step.</returns>
	private AnonymousDeadlyPatternStep? CheckType1Or2(
		ref StepAnalysisContext context,
		ref readonly Grid grid,
		ref readonly CellMap pattern,
		Mask digitsMask,
		Digit targetDigit,
		ref readonly CandidateMap p,
		Technique technique
	)
	{
		// Congratulations, you have found a deadly pattern with 8 cells without any names!
		// Now check eliminations.
		var extraCells = CandidatesMap[targetDigit] & pattern;
		var eliminations = extraCells is [var extraCell]
			?
			from digit in (Mask)(grid.GetCandidates(extraCell) & ~(1 << targetDigit))
			select new Conclusion(Elimination, extraCell, digit)
			: from cell in extraCells % CandidatesMap[targetDigit] select new Conclusion(Elimination, cell, targetDigit);
		if (eliminations.Length == 0)
		{
			return null;
		}

		if (extraCells.Count == 1)
		{
			var candidateOffsets = new List<CandidateViewNode>();
			foreach (var cell in pattern - extraCells[0])
			{
				foreach (var digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add(new(ColorIdentifier.Normal, cell * 9 + digit));
				}
			}

			var step = new AnonymousDeadlyPatternType1Step(
				eliminations.ToArray(),
				[[.. candidateOffsets]],
				context.Options,
				in p,
				extraCells[0],
				digitsMask,
				technique
			);
			if (context.OnlyFindOne)
			{
				return step;
			}
			context.Accumulator.Add(step);
		}
		else
		{
			var candidateOffsets = new List<CandidateViewNode>();
			foreach (var cell in pattern)
			{
				foreach (var digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add(
						new(
							digit == targetDigit ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal,
							cell * 9 + digit
						)
					);
				}
			}

			var step = new AnonymousDeadlyPatternType2Step(
				eliminations.ToArray(),
				[[.. candidateOffsets]],
				context.Options,
				in p,
				in extraCells,
				targetDigit,
				technique
			);
			if (context.OnlyFindOne)
			{
				return step;
			}
			context.Accumulator.Add(step);
		}
		return null;
	}

	/// <summary>
	/// Check for type 3.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="pattern">The pattern.</param>
	/// <param name="digitsMask">The digits used.</param>
	/// <param name="extraDigitsMask">The extra digits.</param>
	/// <param name="extraCells">Indicates the extra cells used.</param>
	/// <param name="p">All candidates used.</param>
	/// <param name="technique">The technique.</param>
	/// <returns>The found step.</returns>
	private AnonymousDeadlyPatternType3Step? CheckType3(
		ref StepAnalysisContext context,
		ref readonly Grid grid,
		ref readonly CellMap pattern,
		Mask digitsMask,
		Mask extraDigitsMask,
		ref readonly CellMap extraCells,
		ref readonly CandidateMap p,
		Technique technique
	)
	{
		// Test examples:
		// 8 cells
		// ..5..+73+6..3+78+6...26.9.3...7+89..54+1+7.+3+746..5+2.+5+1.7...4.2+6+3.4.785+9+4+8+5+762+3+1+751...6+9+4:214 126 234 136 956 865 869 969

		// Iterate on each shared house, to find a subset.
		foreach (var house in extraCells.SharedHouses)
		{
			var availableCells = HousesMap[house] & EmptyCells & ~pattern;
			if (availableCells.Count <= 2)
			{
				continue;
			}

			// Iterate each combination.
			foreach (ref readonly var subsetCells in availableCells | availableCells.Count - 1)
			{
				var subsetDigitsMask = (Mask)(grid[in subsetCells] | extraDigitsMask);
				if (Mask.PopCount(subsetDigitsMask) != subsetCells.Count + 1)
				{
					// The (n) digits should be inside (n - 1) cells.
					continue;
				}

				var eliminations = new List<Conclusion>();
				foreach (var cell in availableCells & ~subsetCells)
				{
					foreach (var digit in (Mask)(grid.GetCandidates(cell) & subsetDigitsMask))
					{
						eliminations.Add(new(Elimination, cell, digit));
					}
				}
				if (eliminations.Count == 0)
				{
					// No eliminations found.
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>();
				foreach (var cell in pattern)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								HousesMap[house].Contains(cell) && (subsetDigitsMask >> digit & 1) != 0
									? ColorIdentifier.Auxiliary1
									: ColorIdentifier.Normal,
								cell * 9 + digit
							)
						);
					}
				}
				foreach (var cell in subsetCells)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, cell * 9 + digit));
					}
				}

				var step = new AnonymousDeadlyPatternType3Step(
					eliminations.AsReadOnlyMemory(),
					[[.. candidateOffsets, new HouseViewNode(ColorIdentifier.Normal, house)]],
					context.Options,
					in p,
					in pattern,
					in subsetCells,
					subsetDigitsMask,
					technique
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
	/// Check for type 4.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="pattern">The pattern.</param>
	/// <param name="digitsMask">The digits used.</param>
	/// <param name="extraDigitsMask">The extra digits.</param>
	/// <param name="extraCells">Indicates the extra cells used.</param>
	/// <param name="p">All candidates used.</param>
	/// <param name="technique">The technique.</param>
	/// <returns>The found step.</returns>
	private AnonymousDeadlyPatternType4Step? CheckType4(
		ref StepAnalysisContext context,
		ref readonly Grid grid,
		ref readonly CellMap pattern,
		Mask digitsMask,
		Mask extraDigitsMask,
		ref readonly CellMap extraCells,
		ref readonly CandidateMap p,
		Technique technique
	)
	{
		// Test examples:
		// 8 cells
		// 6+147+5+3.+8.+39+7.+2.+1652+5+8+9+61+4+7+37..6..8..+8....9.....1.8..4..+7.2...3..2+6..579..8+3+1...+24:149 557 657 159 259 567 667 269 969 571 971 679
		// 7..6..3+41.46....9.2...4.+68...+74.3..8..4.6.1..5.+28+17....2..8...3.7....81.4.8..9..6:321 124 224 724 225 226 526 729 134 334 641 947 248 959 962 176 576 577 777 286 586 289 589 294 794 295 798
		// ..5..+73+6..3+78+6...26.9.3...7+89..54+1+7.+3+746..5+2.+5+1.7...4.2+6+3.4.785+9+4+8+5+762+3+1+751...6+9+4:214 126 234 136 956 865 869 969

		var house = extraCells.FirstSharedHouse;
		var cells = HousesMap[house] & pattern;
		if (cells is [var firstCell, var secondCell])
		{
			// Today we only allow for 2 cells.
			// Check the number of digits appeared in pattern. The number of digits should be 2.
			var digits = (Mask)((grid.GetCandidates(firstCell) | grid.GetCandidates(secondCell)) & digitsMask & ~extraDigitsMask);
			if (Mask.PopCount(digits) != 2)
			{
				return null;
			}

			var firstDigit = (Digit)Mask.TrailingZeroCount(digits);
			var secondDigit = digits.GetNextSet(firstDigit);
			foreach (var (conjugatePairDigit, eliminationDigit) in ((firstDigit, secondDigit), (secondDigit, firstDigit)))
			{
				// Determine whether the digit is a conjugate in such house.
				if ((HousesMap[house] & CandidatesMap[conjugatePairDigit]) != cells)
				{
					continue;
				}

				// The other digit can be eliminated.
				var conclusions = (
					from cell in cells & CandidatesMap[eliminationDigit]
					select new Conclusion(Elimination, cell, eliminationDigit)
				).ToArray();
				if (conclusions.Length == 0)
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>();
				foreach (var cell in pattern & ~cells)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(ColorIdentifier.Normal, cell * 9 + digit));
					}
				}
				foreach (var cell in cells)
				{
					candidateOffsets.Add(new(ColorIdentifier.Auxiliary1, cell * 9 + conjugatePairDigit));
				}

				var step = new AnonymousDeadlyPatternType4Step(
					conclusions,
					[[.. candidateOffsets, new ConjugateLinkViewNode(ColorIdentifier.Normal, firstCell, secondCell, conjugatePairDigit)]],
					context.Options,
					in p,
					house,
					(Mask)(1 << conjugatePairDigit | 1 << eliminationDigit),
					technique
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
	/// Try to verify whether a pattern is a deadly pattern without the specified digits from a grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="pattern">The pattern.</param>
	/// <param name="digits">The digits.</param>
	/// <param name="c">Indicates all candidates used.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	private static bool VerifyPattern(ref readonly Grid grid, ref readonly CellMap pattern, Mask digits, out CandidateMap c)
	{
		var emptyGrid = Grid.Empty;
		foreach (var cell in pattern)
		{
			emptyGrid.SetCandidates(cell, (Mask)(grid.GetCandidates(cell) & ~digits));
		}

		var flag = DeadlyPatternInferrer.TryInfer(in emptyGrid, in pattern, out var result);
		c = result.PatternCandidates;
		return flag && result.IsDeadlyPattern;
	}
}
