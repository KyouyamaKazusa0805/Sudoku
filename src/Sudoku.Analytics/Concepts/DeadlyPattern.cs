namespace Sudoku.Concepts;

/// <summary>
/// Represents a type that checks and infers for a pattern (specified as a <see cref="Grid"/>, but invalid - multiple solutions),
/// determining whether the pattern is a deadly pattern.
/// </summary>
/// <seealso cref="Grid"/>
public static class DeadlyPattern
{
	/// <summary>
	/// Determines whether a pattern is a deadly pattern.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="grid"/> is valid.</exception>
	/// <remarks><b>
	/// Please note that you should use inline array syntax like <c>grid[cellIndex]</c> to visit and update mask,
	/// in order to avoid events <see cref="Grid.ValueChanged"/> and <see cref="Grid.RefreshingCandidates"/> being triggered.
	/// </b></remarks>
	/// <seealso cref="Grid.ValueChanged"/>
	/// <seealso cref="Grid.RefreshingCandidates"/>
	public static bool IsDeadlyPattern(scoped ref readonly Grid grid)
	{
		ArgumentOutOfRangeException.ThrowIfNotEqual(grid.IsValid, false);
		ArgumentOutOfRangeException.ThrowIfNotEqual(grid.EmptiesCount, 81);

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
				return false;
			}
		}

		// Step 1: Get all solutions for that pattern.
		var playground = grid;
		var solutions = new List<Grid>();
		dfs(ref playground, in cellsUsed, solutions, 0);
		if (solutions.Count == 0)
		{
			return false;
		}

		foreach (ref var solution in solutions.AsSpan())
		{
			// Step 2: Make complete pattern for each solution.
			foreach (var cell in cellsUsed)
			{
				var rowMask = grid[HousesMap[cell.ToHouseIndex(HouseType.Row)] & cellsUsed, true];
				var columnMask = grid[HousesMap[cell.ToHouseIndex(HouseType.Column)] & cellsUsed, true];
				var blockMask = grid[HousesMap[cell.ToHouseIndex(HouseType.Block)] & cellsUsed, true];
				if ((rowMask & columnMask) == 0 || (rowMask & blockMask) == 0 || (columnMask & blockMask) == 0)
				{
					return false;
				}

				solution[cell] = (Mask)(Grid.EmptyMask | rowMask & columnMask & blockMask);
			}

			// Step 3: Try to get solutions for that pattern, then determine whether any solutions to the current state exists.
			var tempSolutions = new List<Grid>();
			var playgroundCopied = solution;
			dfs(ref playgroundCopied, in cellsUsed, tempSolutions, 0);
			if (tempSolutions.Count == 0)
			{
				return false;
			}
		}

		// If all possible solutions has exchangable patterns, the pattern will be a real deadly pattern.
		return true;


		static void dfs(scoped ref Grid grid, scoped ref readonly CellMap cellsRange, List<Grid> solutions, Cell currentCell)
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
				var r = currentCell / 9;
				var c = currentCell % 9;
				var digits = grid.GetCandidates(currentCell);

				foreach (var digit in digits)
				{
					grid[currentCell] = (Mask)(Grid.ModifiableMask | (Mask)(1 << digit));
					if (isValid(in grid, r, c))
					{
						dfs(ref grid, in cellsRange, solutions, currentCell + 1);
					}
				}

				grid[currentCell] = (Mask)(Grid.EmptyMask | digits);
			}


			static bool isValid(scoped ref readonly Grid grid, RowIndex r, ColumnIndex c)
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
