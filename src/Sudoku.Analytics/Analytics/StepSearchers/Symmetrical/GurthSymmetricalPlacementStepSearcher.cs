namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Gurth's Symmetrical Placement</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Gurth's Symmetrical Placement</item>
/// <!--<item>Gurth's Symmetrical Placement (Shuffling Type)</item>-->
/// </list>
/// </summary>
[StepSearcher, Direct]
public sealed unsafe partial class GurthSymmetricalPlacementStepSearcher : StepSearcher
{
	/// <summary>
	/// The methods.
	/// </summary>
	private static readonly delegate*<in Grid, GurthSymmetricalPlacementStep?>[] Methods = { &CheckDiagonal, &CheckAntiDiagonal, &CheckCentral };


	/// <inheritdoc/>
	protected internal override Step? GetAll(scoped ref AnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		for (var i = 0; i < 3; i++)
		{
			if (Methods[i](grid) is not { } step)
			{
				continue;
			}

			if (context.OnlyFindOne)
			{
				return step;
			}

			context.Accumulator.Add(step);
		}

		return null;
	}


	/// <summary>
	/// Records all possible highlight cells.
	/// </summary>
	/// <param name="grid">The grid as reference.</param>
	/// <param name="cellOffsets">The target collection.</param>
	/// <param name="mapping">The mapping relation.</param>
	private static void RecordHighlightCells(scoped in Grid grid, List<CellViewNode> cellOffsets, int?[] mapping)
	{
		scoped var colorIndices = (stackalloc int[9]);
		for (var (digit, colorIndexCurrent, digitsMaskBucket) = (0, 0, (Mask)0); digit < 9; digit++)
		{
			if ((digitsMaskBucket >> digit & 1) != 0)
			{
				continue;
			}

			var currentMappingRelationDigit = mapping[digit];

			colorIndices[digit] = colorIndexCurrent;
			digitsMaskBucket |= (Mask)(1 << digit);
			if (currentMappingRelationDigit is { } relatedDigit && relatedDigit != digit)
			{
				colorIndices[relatedDigit] = colorIndexCurrent;
				digitsMaskBucket |= (Mask)(1 << relatedDigit);
			}

			colorIndexCurrent++;
		}

		foreach (var cell in ~grid.EmptyCells)
		{
			cellOffsets.Add(new(colorIndices[grid[cell]], cell));
		}
	}

	/// <summary>
	/// Checks for diagonal symmetry steps.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>A correct step if found; otherwise, <see langword="null"/>.</returns>
	private static GurthSymmetricalPlacementStep? CheckDiagonal(in Grid grid)
	{
		var diagonalHasEmptyCell = false;
		for (var i = 0; i < 9; i++)
		{
			if (grid.GetStatus(i * 9 + i) == CellStatus.Empty)
			{
				diagonalHasEmptyCell = true;
				break;
			}
		}
		if (!diagonalHasEmptyCell)
		{
			// No conclusion.
			return null;
		}

		var mapping = new int?[9];
		for (var i = 0; i < 9; i++)
		{
			for (var j = 0; j < i; j++)
			{
				var c1 = i * 9 + j;
				var c2 = j * 9 + i;
				var condition = grid.GetStatus(c1) == CellStatus.Empty;
				if (condition ^ grid.GetStatus(c2) == CellStatus.Empty)
				{
					// One of two cells is empty. Not this symmetry.
					return null;
				}

				if (condition)
				{
					continue;
				}

				var d1 = grid[c1];
				var d2 = grid[c2];
				if (d1 == d2)
				{
					var o1 = mapping[d1];
					if (o1 is null)
					{
						mapping[d1] = d1;
						continue;
					}

					if (o1 != d1)
					{
						return null;
					}
				}
				else
				{
					var o1 = mapping[d1];
					var o2 = mapping[d2];
					if (o1.HasValue ^ o2.HasValue)
					{
						return null;
					}

					if (o1 is null && o2 is null)
					{
						mapping[d1] = d2;
						mapping[d2] = d1;
						continue;
					}

					// 'o1' and 'o2' are both not null.
					if (o1 != d2 || o2 != d1)
					{
						return null;
					}
				}
			}
		}

		var singleDigitList = new List<int>();
		for (var digit = 0; digit < 9; digit++)
		{
			var mappingDigit = mapping[digit];
			if (!mappingDigit.HasValue || mappingDigit == digit)
			{
				singleDigitList.Add(digit);
			}
		}

		var cellOffsets = new List<CellViewNode>();
		var candidateOffsets = new List<CandidateViewNode>();
		var conclusions = new List<Conclusion>();
		for (var i = 0; i < 9; i++)
		{
			var cell = i * 9 + i;
			if (grid.GetStatus(cell) != CellStatus.Empty)
			{
				continue;
			}

			foreach (var digit in grid.GetCandidates(cell))
			{
				if (singleDigitList.Contains(digit))
				{
					candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
					continue;
				}

				conclusions.Add(new(Elimination, cell, digit));
			}
		}
		RecordHighlightCells(grid, cellOffsets, mapping);

		return conclusions.Count == 0
			? null
			: new GurthSymmetricalPlacementStep(
				conclusions.ToArray(),
				new[] { View.Empty | cellOffsets | candidateOffsets },
				SymmetryType.Diagonal,
				mapping
			);
	}

	/// <summary>
	/// Checks for anti-diagonal symmetry steps.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>A correct step if found; otherwise, <see langword="null"/>.</returns>
	private static GurthSymmetricalPlacementStep? CheckAntiDiagonal(in Grid grid)
	{
		var antiDiagonalHasEmptyCell = false;
		for (var i = 0; i < 9; i++)
		{
			if (grid.GetStatus(i * 9 + (8 - i)) == CellStatus.Empty)
			{
				antiDiagonalHasEmptyCell = true;
				break;
			}
		}
		if (!antiDiagonalHasEmptyCell)
		{
			// No conclusion.
			return null;
		}

		var mapping = new int?[9];
		for (var i = 0; i < 9; i++)
		{
			for (var j = 0; j < 8 - i; j++)
			{
				var c1 = i * 9 + j;
				var c2 = (8 - j) * 9 + (8 - i);
				var condition = grid.GetStatus(c1) == CellStatus.Empty;
				if (condition ^ grid.GetStatus(c2) == CellStatus.Empty)
				{
					// One of two cells is empty. Not this symmetry.
					return null;
				}

				if (condition)
				{
					continue;
				}

				var d1 = grid[c1];
				var d2 = grid[c2];
				if (d1 == d2)
				{
					var o1 = mapping[d1];
					if (o1 is null)
					{
						mapping[d1] = d1;
						continue;
					}

					if (o1 != d1)
					{
						return null;
					}
				}
				else
				{
					var o1 = mapping[d1];
					var o2 = mapping[d2];
					if (o1.HasValue ^ o2.HasValue)
					{
						return null;
					}

					if (o1 is null || o2 is null)
					{
						mapping[d1] = d2;
						mapping[d2] = d1;
						continue;
					}

					// 'o1' and 'o2' are both not null.
					if (o1 != d2 || o2 != d1)
					{
						return null;
					}
				}
			}
		}

		var singleDigitList = new List<int>();
		for (var digit = 0; digit < 9; digit++)
		{
			var mappingDigit = mapping[digit];
			if (!mappingDigit.HasValue || mappingDigit == digit)
			{
				singleDigitList.Add(digit);
			}
		}

		var cellOffsets = new List<CellViewNode>();
		var candidateOffsets = new List<CandidateViewNode>();
		var conclusions = new List<Conclusion>();
		for (var i = 0; i < 9; i++)
		{
			var cell = i * 9 + (8 - i);
			if (grid.GetStatus(cell) != CellStatus.Empty)
			{
				continue;
			}

			foreach (var digit in grid.GetCandidates(cell))
			{
				if (singleDigitList.Contains(digit))
				{
					candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
					continue;
				}

				conclusions.Add(new(Elimination, cell, digit));
			}
		}
		RecordHighlightCells(grid, cellOffsets, mapping);

		return conclusions.Count == 0
			? null
			: new(conclusions.ToArray(), new[] { View.Empty | cellOffsets | candidateOffsets }, SymmetryType.AntiDiagonal, mapping);
	}

	/// <summary>
	/// Checks for central symmetry steps.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>A correct step if found; otherwise, <see langword="null"/>.</returns>
	private static GurthSymmetricalPlacementStep? CheckCentral(in Grid grid)
	{
		if (grid.GetStatus(40) != CellStatus.Empty)
		{
			// Has no conclusion even though the grid may be symmetrical.
			return null;
		}

		var mapping = new int?[9];
		for (var cell = 0; cell < 40; cell++)
		{
			var anotherCell = 80 - cell;
			var condition = grid.GetStatus(cell) == CellStatus.Empty;
			if (condition ^ grid.GetStatus(anotherCell) == CellStatus.Empty)
			{
				// One of two cell is empty, not central symmetry type.
				return null;
			}

			if (condition)
			{
				continue;
			}

			var d1 = grid[cell];
			var d2 = grid[anotherCell];
			if (d1 == d2)
			{
				var o1 = mapping[d1];
				if (o1 is null)
				{
					mapping[d1] = d1;
					continue;
				}

				if (o1 != d1)
				{
					return null;
				}
			}
			else
			{
				var o1 = mapping[d1];
				var o2 = mapping[d2];
				if (o1 is not null ^ o2 is not null)
				{
					return null;
				}

				if (o1 is null || o2 is null)
				{
					mapping[d1] = d2;
					mapping[d2] = d1;
					continue;
				}

				// 'o1' and 'o2' are both not null.
				if (o1 != d2 || o2 != d1)
				{
					return null;
				}
			}
		}

		for (var digit = 0; digit < 9; digit++)
		{
			if (mapping[digit] is not null && mapping[digit] != digit)
			{
				continue;
			}

			var cellOffsets = new List<CellViewNode>();
			RecordHighlightCells(grid, cellOffsets, mapping);

			return new(
				new[] { new Conclusion(Assignment, 40, digit) },
				new[] { View.Empty | cellOffsets | new CandidateViewNode(DisplayColorKind.Normal, 360 + digit) },
				SymmetryType.Central,
				mapping
			);
		}

		return null;
	}
}
