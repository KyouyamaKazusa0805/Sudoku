namespace Sudoku.Solving;

/// <summary>
/// Indicates an ittouryu solver that can solve a sudoku grid using ittouryu mode.
/// </summary>
public sealed class IttouryuSolver
{
	/// <summary>
	/// The grid to be solved.
	/// </summary>
	private readonly Grid _grid;


	/// <summary>
	/// Initializes an <see cref="IttouryuSolver"/> using a sudoku grid to be solved.
	/// </summary>
	/// <param name="grid">The pointer to the grid.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IttouryuSolver(in Grid grid) => _grid = grid;


	/// <summary>
	/// Determine whether the specified sudoku grid is ittouryu.
	/// </summary>
	/// <param name="solvingPath">The solving path.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	/// <remarks>
	/// An <b>ittouryu puzzle</b> is a puzzle that be finished using digits 1 to 9 one by one.
	/// </remarks>
	public bool IsIttouryu([NotNullWhen(true)] out (int Candidate, bool IsHiddenSingle)[]? solvingPath)
	{
		var (listOfSteps, tempGrid, currentDigit) = (new List<(int, bool)>(), _grid, -1);

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
						// The current region already contains the value of the current digit.
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
