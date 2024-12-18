namespace Sudoku.Inferring;

/// <summary>
/// Represents a type that checks and infers for a pattern (specified as a <see cref="Grid"/>, but invalid - multiple solutions),
/// determining whether the pattern is a deadly pattern.
/// </summary>
/// <seealso cref="Grid"/>
public sealed class DeadlyPatternInferrer : IInferrable<DeadlyPatternInferredResult>
{
	/// <inheritdoc/>
	/// <exception cref="DeadlyPatternInferrerLimitReachedException">
	/// Throws when the pattern contains more than 10000 solutions.
	/// </exception>
	public static bool TryInfer(ref readonly Grid grid, out DeadlyPatternInferredResult result)
		=> TryInfer(in grid, in Unsafe.NullRef<CellMap>(), out result);

	/// <inheritdoc cref="TryInfer(ref readonly Grid, out DeadlyPatternInferredResult)"/>
	/// <exception cref="DeadlyPatternInferrerLimitReachedException">
	/// Throws when the pattern contains more than 10000 solutions.
	/// </exception>
	public static bool TryInfer(ref readonly Grid grid, [AllowNull] ref readonly CellMap cells, out DeadlyPatternInferredResult result)
	{
		var patternCandidates = CandidateMap.Empty;
		if (grid.GetIsValid() || grid.EmptyCellsCount != 81 || grid.PuzzleType != SudokuType.Standard)
		{
			// Invalid values to be checked.
			goto FastFail;
		}

		// Collect all used cells. This value may not be necessary but will make program be a little bit faster if cached.
		CellMap cellsUsed;
		if (Unsafe.IsNullRef(in cells))
		{
			cellsUsed = CellMap.Empty;
			for (var cell = 0; cell < 81; cell++)
			{
				if (grid.GetCandidates(cell) != Grid.MaxCandidatesMask)
				{
					cellsUsed.Add(cell);
				}
			}
		}
		else
		{
			cellsUsed = cells;
		}

		// Verify whether at least one cell in pattern hold nothing.
		foreach (var cell in cellsUsed)
		{
			var mask = grid.GetCandidates(cell);
			if (mask == 0)
			{
				goto FastFail;
			}

			foreach (var digit in mask)
			{
				patternCandidates.Add(cell * 9 + digit);
			}
		}

		// Step 0: Determine whether at least one house the pattern spanned only hold one cell used.
		// A valid deadly pattern must hold at least 2 cells for all spanned houses.
		foreach (var house in cellsUsed.Houses)
		{
			if ((HousesMap[house] & cellsUsed).Count == 1)
			{
				goto FastFail;
			}
		}

		// Step 1: Get all solutions for that pattern.
		var playground = grid;
		var solutions = new List<Grid>();
		dfs(ref playground, in cellsUsed, solutions, 0);
		if (solutions.Count == 0)
		{
			goto FastFail;
		}

		var failedCases = new List<Grid>();
		foreach (ref readonly var solution in solutions.AsSpan())
		{
			// Step 2: Iterate on all the other solutions,
			// and find whether each solution contains at least one possible corresponding solution
			// whose digits used in *all* houses are completely same.
			var tempSolutions = new List<Grid>();
			foreach (ref readonly var tempGrid in solutions.AsSpan()[..])
			{
				if (tempGrid == solution)
				{
					continue;
				}

				// Check for all possible houses.
				var flag = true;
				foreach (var house in cellsUsed.Houses)
				{
					var mask1 = solution[HousesMap[house] & cellsUsed, true];
					var mask2 = tempGrid[HousesMap[house] & cellsUsed, true];
					if (mask1 != mask2)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					tempSolutions.AddRef(in tempGrid);
				}
			}

			// Step 3: Check for the validity on this case.
			// If failed to check, we should collect the case into the result, as an item in failed cases set.
			if (tempSolutions.Count == 0)
			{
				failedCases.AddRef(in solution);
			}
		}

		// If all possible solutions has exchangable patterns, the pattern will be a real deadly pattern;
		// otherwise, not a deadly pattern.
		result = new(in grid, failedCases.Count == 0, failedCases.AsSpan(), in patternCandidates);
		return true;

	FastFail:
		result = new(in grid, false, [], in patternCandidates);
		return true;


		static void dfs(ref Grid grid, ref readonly CellMap cellsRange, List<Grid> solutions, Cell currentCell)
		{
			if (currentCell == 81)
			{
				if (solutions.Count >= 9999)
				{
					throw new DeadlyPatternInferrerLimitReachedException();
				}

				solutions.AddRef(in grid);
				return;
			}

			if (!cellsRange.Contains(currentCell))
			{
				dfs(ref grid, in cellsRange, solutions, currentCell + 1);
			}
			else
			{
				var (r, c, digits) = (currentCell / 9, currentCell % 9, grid.GetCandidates(currentCell));
				foreach (var digit in digits)
				{
					grid[currentCell] = (Mask)(Grid.ModifiableMask | 1 << digit);
					if (BacktrackingSolver.IsValid(in grid, r, c))
					{
						dfs(ref grid, in cellsRange, solutions, currentCell + 1);
					}
				}
				grid[currentCell] = (Mask)(Grid.EmptyMask | digits);
			}
		}
	}
}
