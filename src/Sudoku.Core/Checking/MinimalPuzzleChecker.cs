namespace Sudoku.Checking;

/// <summary>
/// Provides with a checker that determines whether the puzzle is a
/// <see href="https://sunnieshine.github.io/Sudoku/terms/minimal-puzzle">minimal puzzle</see>.
/// </summary>
public static class MinimalPuzzleChecker
{
	/// <inheritdoc cref="IsMinimal(in Grid, out int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsMinimal(in Grid grid) => IsMinimal(grid, out _);

	/// <summary>
	/// Determines whether the puzzle is a minimal puzzle, which means the puzzle will become multiple solution
	/// if arbitrary one given digit will be removed from the grid.
	/// </summary>
	/// <param name="grid">The grid to be determined.</param>
	/// <param name="firstCandidateMakePuzzleNotMinimal">
	/// <para>
	/// Indicates the first found candidate that can make the puzzle not minimal, which means
	/// if we remove the digit in the cell, the puzzle will still keep unique.
	/// </para>
	/// <para>If the return value is <see langword="true"/>, this argument will be -1.</para>
	/// </param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	/// <exception cref="ArgumentException">Throws when the puzzle is invalid (i.e. not unique).</exception>
	public static bool IsMinimal(in Grid grid, out int firstCandidateMakePuzzleNotMinimal)
	{
		switch (grid)
		{
			case { IsValid: false }:
			{
				throw new ArgumentException("The puzzle is not unique.", nameof(grid));
			}
			case { IsSolved: true, GivenCells.Count: 81 }:
			{
				// Very special case: all cells are givens.
				// The puzzle is considered not a minimal puzzle, because any one digit in the grid can be removed.
				firstCandidateMakePuzzleNotMinimal = grid[0];
				return false;
			}
			default:
			{
				var gridCanBeModified = grid;
				gridCanBeModified.Unfix();

				foreach (int cell in gridCanBeModified.ModifiableCells)
				{
					var newGrid = gridCanBeModified;
					newGrid[cell] = -1;
					newGrid.Fix();

					if (newGrid.IsValid)
					{
						firstCandidateMakePuzzleNotMinimal = cell * 9 + grid[cell];
						return false;
					}
				}

				firstCandidateMakePuzzleNotMinimal = -1;
				return true;
			}
		}
	}
}
