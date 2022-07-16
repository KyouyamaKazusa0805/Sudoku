namespace Sudoku.Checking;

/// <summary>
/// Provides with a checker that determines whether the puzzle is an
/// <see href="https://sunnieshine.github.io/Sudoku/terms/ittouryu-puzzle">ittouryu puzzle</see>.
/// </summary>
public static class IttouryuPuzzleChecker
{
	/// <inheritdoc cref="IsIttouryu(in Grid, out ValueTuple{int, bool}[])"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsIttouryu(scoped in Grid grid) => IsIttouryu(grid, out _);

	/// <summary>
	/// Determines whether the puzzle is an ittouryu puzzle, which means we can fill the puzzle digit by digit.
	/// </summary>
	/// <param name="grid">The grid to be determined.</param>
	/// <param name="solvingPath">The solving path.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	public static bool IsIttouryu(
		scoped in Grid grid, [NotNullWhen(true)] out (int Candidate, bool IsHiddenSingle)[]? solvingPath)
	{
		var (listOfSteps, tempGrid, currentDigit) = (new List<(int, bool)>(), grid, -1);

		// Iterates on each digit.
		// Here we introduce an extra variable 'validatedTimes', which records how many times we have skipped
		// the searching on a digit.
		// For example, we find a hidden single of digit 1, but the next conclusion is about the digit 5,
		// the variable 'validatedTimes' will be 4.
		// If the variable is greater than 9, which means we have searched for all possible digit kinds
		// that can be appeared in a sudoku grid, so we can conclude
		// that all possible hidden and naked singles are used,
		// which means the puzzle cannot be solved, unless we use advanced solving techniques.
		// In this case, the puzzle is not an ittouryu one, just return false.
		for (int digit = 0, validatedTimes = 0; validatedTimes < 9; digit = (digit + 1) % 9, validatedTimes++)
		{
			// Try to find hidden singles.
			for (int cell = 0; cell < 81; cell++)
			{
				foreach (var houseType in HouseTypes)
				{
					int house = cell.ToHouseIndex(houseType);
					var houseCells = HouseMaps[house];
					if ((tempGrid.ValuesMap[digit] & houseCells) is not [])
					{
						// The current house already contains the value of the current digit.
						continue;
					}

					int conclusionCell = -1;
					for (int index = 0, count = 0; index < 9; index++)
					{
						int tempCell = houseCells[index];
						if (tempGrid.Exists(tempCell, digit) is true)
						{
							if (++count >= 2)
							{
								conclusionCell = -1;
								break;
							}

							conclusionCell = tempCell;
						}
					}

					if (conclusionCell != -1)
					{
						// Hidden single found.
						if (currentDigit != -1
							&& (currentDigit == 9 && digit != 1 || currentDigit != 9 && digit - currentDigit > 1))
						{
							solvingPath = null;
							return false;
						}

						int hiddenSingleCandidate = conclusionCell * 9 + digit;
						listOfSteps.Add((hiddenSingleCandidate, true));
						tempGrid[conclusionCell] = digit;
						currentDigit = digit;
						validatedTimes = 0;

						// If we find that a cell can be filled with the digit,
						// all houses that contains the current cell will contain same conclusion.
						// Just skip.
						break;
					}
				}
			}

			// Try to find naked singles.
			for (int cell = 0; cell < 81; cell++)
			{
				if (tempGrid.GetStatus(cell) != CellStatus.Empty)
				{
					continue;
				}

				short mask = tempGrid.GetCandidates(cell);
				if (IsPow2(mask) && TrailingZeroCount(mask) is var conclusionDigit && conclusionDigit == digit)
				{
					// Naked single found.
					if (currentDigit != -1
						&& (currentDigit == 9 && digit != 1 || currentDigit != 9 && digit - currentDigit > 1))
					{
						solvingPath = null;
						return false;
					}

					int nakedSingleCandidate = cell * 9 + conclusionDigit;
					listOfSteps.Add((nakedSingleCandidate, false));
					tempGrid[cell] = conclusionDigit;
					currentDigit = conclusionDigit;
					validatedTimes = 0;
				}
			}
		}

		(solvingPath, bool @return) = tempGrid.IsSolved ? (listOfSteps.ToArray(), true) : (null, false);
		return @return;
	}
}
