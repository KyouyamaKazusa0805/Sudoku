namespace Sudoku.Solving.Backtracking;

/// <summary>
/// Defines a solver that can solve a sudoku puzzle, using backtracking algorithm.
/// </summary>
/// <remarks>
/// <para>
/// Please note that the current type has no optimization on solving. Therefore sometimes the puzzle will be
/// extremely slowly to be solved although it is not very hard by manually solved.
/// One of the examples satisfying the above condition is:
/// <code>
/// ..............3.85..1.2.......5.7.....4...1...9.......5......73..2.1........4...9
/// </code>
/// The current solver may spend about 4.5 min on solving this puzzle on my machine.
/// </para>
/// <para>
/// For more information, please visit
/// <see href="https://en.wikipedia.org/wiki/Sudoku_solving_algorithms#cite_note-difficult_17_clue-1">this link</see>.
/// </para>
/// </remarks>
public sealed class BacktrackingSolver : ISolver
{
	/// <inheritdoc/>
	public static string UriLink => "https://simple.wikipedia.org/wiki/Backtracking";


	/// <summary>
	/// Indicates whether the solver uses breadth-first searching algorithm instead of traditional depth-first searching.
	/// </summary>
	public bool UseBreadthFirstSearch { get; set; }


	/// <inheritdoc/>
	public bool? Solve(in Grid grid, out Grid result)
	{
		var resultArray = default(Digit[]);
		try
		{
			var solutionsCount = 0;
			var gridArray = grid.ToDigitsArray(); // Implicit plus one operation.
			if (UseBreadthFirstSearch)
			{
				bfs(ref resultArray, gridArray);
			}
			else
			{
				dfs(ref resultArray, gridArray, ref solutionsCount, 0);
			}

			if (resultArray is null)
			{
				result = default;
				return null;
			}

			result = Grid.Create(resultArray, GridCreatingOption.MinusOne);
			return true;
		}
		catch (MultipleSolutionException)
		{
			result = Grid.Create(resultArray!, GridCreatingOption.MinusOne);
			return false;
		}


		static void dfs(ref Digit[]? result, Digit[] gridValues, ref int solutionsCount, int finishedCellsCount)
		{
			if (finishedCellsCount == 81)
			{
				// Solution found.
				if (++solutionsCount > 1)
				{
					throw new MultipleSolutionException();
				}

				// We should catch the result.
				// If we use normal assignment, we well get the initial grid rather a solution,
				// because this is a recursive function.
				result = gridValues[..];
				return; // Exit the recursion.
			}

			if (gridValues[finishedCellsCount] != 0)
			{
				dfs(ref result, gridValues, ref solutionsCount, finishedCellsCount + 1);
			}
			else
			{
				// Here may try 9 times.
				// Of course, you can add a new variable to save all candidates to let the algorithm run faster.
				var r = finishedCellsCount / 9;
				var c = finishedCellsCount % 9;
				for (var i = 0; i < 9; i++)
				{
					gridValues[finishedCellsCount]++;
					if (IsValid(gridValues, r, c))
					{
						dfs(ref result, gridValues, ref solutionsCount, finishedCellsCount + 1);
					}
				}

				// All values are wrong, which means the value before we calculate is already wrong.
				// Backtracking the cell...
				gridValues[finishedCellsCount] = 0;
			}
		}

		static void bfs(ref Digit[]? result, Digit[] gridValues)
		{
			var queue = new LinkedList<Digit[]>();
			queue.AddLast(gridValues);

			var resultGrids = new List<Digit[]>(2);
			while (queue.Count != 0)
			{
				var currentGrid = queue.RemoveFirstNode();

				// Find for the last unfilled cell.
				var lastUnfilledCell = currentGrid.IndexOf(0);
				if (lastUnfilledCell == -1)
				{
					// No unfilled cell.
					resultGrids.Add(currentGrid);
					if (resultGrids.Count >= 2)
					{
						throw new MultipleSolutionException();
					}
					continue;
				}

				for (var digit = 1; digit <= 9; digit++)
				{
					var copied = (Digit[])currentGrid.Clone();
					copied[lastUnfilledCell] = digit;

					var lastRow = lastUnfilledCell / 9;
					var lastColumn = lastUnfilledCell % 9;
					if (IsValid(copied, lastRow, lastColumn))
					{
						queue.AddLast(copied);
					}
				}
			}

			result = resultGrids is [var resultGrid] ? resultGrid : null;
		}
	}


	/// <summary>
	/// Determine whether the specified grid has confliction with the specified row and column.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="r">The row index.</param>
	/// <param name="c">The column index.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public static bool IsValid(ref readonly Grid grid, RowIndex r, ColumnIndex c)
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

	/// <inheritdoc cref="IsValid(ref readonly Grid, RowIndex, ColumnIndex)"/>
	public static bool IsValid(Digit[] grid, RowIndex r, ColumnIndex c)
	{
		var number = grid[r * 9 + c];
		for (var i = 0; i < 9; i++)
		{
			if (i != r && grid[i * 9 + c] == number || i != c && grid[r * 9 + i] == number)
			{
				return false;
			}
		}
		for (RowIndex ii = r / 3 * 3, i = ii; i < ii + 3; i++)
		{
			for (ColumnIndex jj = c / 3 * 3, j = jj; j < jj + 3; j++)
			{
				if ((i != r || j != c) && grid[i * 9 + j] == number)
				{
					return false;
				}
			}
		}
		return true;
	}
}
