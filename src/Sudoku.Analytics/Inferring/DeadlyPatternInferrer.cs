namespace Sudoku.Inferring;

/// <summary>
/// Represents a type that checks and infers for a pattern (specified as a <see cref="Grid"/>, but invalid - multiple solutions),
/// determining whether the pattern is a deadly pattern.
/// </summary>
/// <seealso cref="Grid"/>
public sealed class DeadlyPatternInferrer : IInferrable<DeadlyPatternInferredResult>
{
	/// <summary>
	/// Hides the constructor of this type.
	/// </summary>
	[Obsolete("Don't instantiate this type.", true)]
	private DeadlyPatternInferrer() => throw new NotSupportedException();


	/// <inheritdoc/>
	public static bool TryInfer(ref readonly Grid grid, out DeadlyPatternInferredResult result)
	{
		if (grid.GetIsValid() || grid.EmptiesCount != 81 || grid.PuzzleType != SudokuType.Standard)
		{
			// Invalid values to be checked.
			goto FastFail;
		}

		// Collect all used cells. This value may not be necessary but will make program be a little bit faster if cached.
		var cellsUsed = CellMap.Empty;
		for (var cell = 0; cell < 81; cell++)
		{
			if (grid.GetCandidates(cell) != Grid.MaxCandidatesMask)
			{
				cellsUsed.Add(cell);
			}
		}

		// Step 0: Determine whether at least one house the pattern spanned only hold one cell used.
		// A valid deadly pattern must hold at least 2 cells for all spanned houses.
		foreach (var house in cellsUsed.Houses)
		{
			if ((HousesMap[house] & cellsUsed).Count == 1)
			{
				result = new(in grid, false, []);
				return true;
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
		foreach (ref readonly var solution in solutions.AsReadOnlySpan())
		{
			// Step 2: Iterate on all the other solutions,
			// and find whether each solution contains at least one possible corresponding solution
			// whose digits used in *all* houses are completely same.
			var tempSolutions = new List<Grid>();
			foreach (ref readonly var tempGrid in solutions.AsReadOnlySpan()[..])
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
		result = new(in grid, failedCases.Count == 0, failedCases.AsReadOnlySpan());
		return true;

	FastFail:
		result = default;
		return false;


		static void dfs(ref Grid grid, ref readonly CellMap cellsRange, List<Grid> solutions, Cell currentCell)
		{
			if (currentCell == 81)
			{
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
					if (isValid(in grid, r, c))
					{
						dfs(ref grid, in cellsRange, solutions, currentCell + 1);
					}
				}
				grid[currentCell] = (Mask)(Grid.EmptyMask | digits);
			}


			static bool isValid(ref readonly Grid grid, RowIndex r, ColumnIndex c)
			{
				var number = grid.GetDigit(r * 9 + c);
				for (var i = 0; i < 9; i++)
				{
					if (i != r && grid.GetDigit(i * 9 + c) == number || i != c && grid.GetDigit(r * 9 + i) == number)
					{
						return false;
					}
				}
				for (RowIndex ii = r / 3 * 3, i = ii; i < ii + 3; i++)
				{
					for (ColumnIndex jj = c / 3 * 3, j = jj; j < jj + 3; j++)
					{
						if ((i != r || j != c) && grid.GetDigit(i * 9 + j) == number)
						{
							return false;
						}
					}
				}
				return true;
			}
		}
	}
}
