namespace Sudoku.Solving;

/// <summary>
/// Defines a solver that can solve a sudoku puzzle, using backtracking algorithm.
/// </summary>
/// <remarks>
/// <para>
/// Please note that the solve has no optimization on solving sudokus, sometimes the puzzle will be
/// extremely slowly to be solved although it is not very hard. One of the examples described above
/// is this:
/// <code>
/// ..............3.85..1.2.......5.7.....4...1...9.......5......73..2.1........4...9
/// </code>
/// The solver will spend about 4.5 min on solving this puzzle.
/// </para>
/// <para>
/// For more information, please visit <see href="http://www.matrix67.com/blog/archives/725">this link</see>.
/// </para>
/// </remarks>
[Algorithm("Backtracking", UriLink = "https://simple.wikipedia.org/wiki/Backtracking")]
public sealed class BacktrackingSolver : ISimpleSolver
{
	/// <inheritdoc/>
	public bool? Solve(in Grid grid, out Grid result)
	{
		Unsafe.SkipInit(out result);

		try
		{
			int solutionsCount = 0;
			int[]? resultArray = null, gridArray = grid.ToArray();
			solve(ref solutionsCount, ref resultArray, gridArray, 0);

			if (resultArray is null)
			{
				return null;
			}

			result = new(resultArray, GridCreatingOption.MinusOne);
			return true;
		}
		catch (InvalidOperationException ex) when (ex.Message == "The grid contains multiple solutions.")
		{
			return false;
		}


		static void solve(ref int solutionsCount, ref int[]? result, int[] gridValues, int finishedCellsCount)
		{
			if (finishedCellsCount == 81)
			{
				// Solution found.
				if (++solutionsCount > 1)
				{
					throw new InvalidOperationException("The grid contains multiple solutions.");
				}

				// We should catch the result.
				// If we use normal assignment, we well get the
				// initial grid rather a solution, because
				// this is a recursive function!!!
				result = (int[])gridValues.Clone();
				return; // Exit the recursion.
			}

			if (gridValues[finishedCellsCount] != 0)
			{
				solve(ref solutionsCount, ref result, gridValues, finishedCellsCount + 1);
			}
			else
			{
				// Here may try 9 times.
				// Of course, you can add a new variable to save
				// all candidates to let the algorithm run faster.
				int r = finishedCellsCount / 9, c = finishedCellsCount % 9;
				for (int i = 0; i < 9; i++)
				{
					gridValues[finishedCellsCount]++; // Only use value increment operator.
					if (isValid(gridValues, r, c))
					{
						solve(ref solutionsCount, ref result, gridValues, finishedCellsCount + 1);
					}
				}

				// All values are wrong, which means the value before
				// we calculate is already wrong.
				// Backtracking the cell...
				gridValues[finishedCellsCount] = 0;
			}


			static bool isValid(int[] gridValues, int r, int c)
			{
				int number = gridValues[r * 9 + c];

				// Check lines.
				for (int i = 0; i < 9; i++)
				{
					if (i != r && gridValues[i * 9 + c] == number || i != c && gridValues[r * 9 + i] == number)
					{
						return false;
					}
				}

				// Check blocks.
				for (int ii = r / 3 * 3, i = ii, iiPlus3 = ii + 3; i < iiPlus3; i++)
				{
					for (int jj = c / 3 * 3, j = jj, jjPlus3 = jj + 3; j < jjPlus3; j++)
					{
						if ((i != r || j != c) && gridValues[i * 9 + j] == number)
						{
							return false;
						}
					}
				}

				// All region are checked and passed, return true.
				return true;
			}
		}
	}
}
