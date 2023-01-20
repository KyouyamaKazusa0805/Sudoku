namespace Sudoku.Checking;

/// <summary>
/// Checks a technique structure whether it forms a deadly pattern.
/// </summary>
public static class DeadlyPatternChecker
{
	/// <summary>
	/// Determines whether the specified technique structure forms a deadly pattern.
	/// This method only checks for at most 1000 solutions. If the pattern contains more than 1000 solutions,
	/// this method will always return <see langword="false"/>.
	/// </summary>
	/// <param name="maskArray">An array of mask list as a technique structure.</param>
	/// <param name="firstFoundGrid">The first found grid.</param>
	/// <param name="secondFoundGrid">The second found grid.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	/// <exception cref="ArgumentException">
	/// Throws when the argument <paramref name="maskArray"/> is a valid puzzle, or length is not 81.
	/// </exception>
	public static unsafe bool IsDeadlyPattern(
		short[] maskArray,
		[NotNullWhen(true)] out int[]? firstFoundGrid,
		[NotNullWhen(true)] out int[]? secondFoundGrid)
	{
		Argument.ThrowIfFalse(maskArray.Length == 81);

		var patternGrid = Grid.Create(maskArray);
		if (patternGrid.IsValid())
		{
			throw new ArgumentException("The argument must be invalid here.", nameof(patternGrid));
		}

		// Checks whether the whole mask list contains any cell that only contains one possible candidate.
		// If so, the method should return false.
		foreach (var mask in maskArray)
		{
			if (mask != 0 && IsPow2(mask))
			{
				goto AssignOutVariablesAndReturnFalse;
			}
		}

		// Gathers the cells used in this pattern.
		var cellsUsed = CellMap.Empty;
		for (var cell = 0; cell < 81; cell++)
		{
			if (patternGrid._values[cell] != 0)
			{
				cellsUsed.Add(cell);
			}
		}

		// Now complement candidates, and check whether the pattern contains only occupies one cell in a house.
		foreach (var house in cellsUsed.Houses)
		{
			var currentHouseCells = HousesMap[house] & cellsUsed;
			if (currentHouseCells.Count == 1)
			{
				// This is an invalid case, which means we can directly return the false value.
				goto AssignOutVariablesAndReturnFalse;
			}

			var targetMask = patternGrid.GetDigitsUnion(currentHouseCells);
			foreach (var currentHouseCell in currentHouseCells)
			{
				patternGrid.GetMaskRef(currentHouseCell) |= targetMask;
			}
		}

		// Then we should solve this semi-puzzle, to get all possible solutions corresponding to the puzzle.
		// Here I use backtracking algorithm because it is easier for algorithm learners than other algorithms.
		uint solutionsCount = 0;
		var gridValues = new int[81];
		var solutionList = new List<BitArray>();
		Array.Fill(gridValues, -1);
		try
		{
			getSolutions(ref solutionsCount, maskArray, gridValues, 0, cellsUsed);
		}
		catch (Exception ex) when (ex is { Message: "Okay for exit.", Data: var data })
		{
			var firstData = (int[])data[nameof(firstFoundGrid)]!;
			var secondData = (int[])data[nameof(secondFoundGrid)]!;

			firstFoundGrid = firstData;
			secondFoundGrid = secondData;
			return true;
		}

	AssignOutVariablesAndReturnFalse:
		firstFoundGrid = secondFoundGrid = null;
		return false;


		void getSolutions(scoped ref uint c, short[] m, int[] g, int p, scoped in CellMap u)
		{
			if (p == 81)
			{
				// Solution found.
				if (++c >= 1000)
				{
					throw new("The puzzle has too many solutions to be checked.");
				}

				var bitArray = new BitArray(81 * 4);
				for (var cell = 0; cell < 81; cell++)
				{
					var digit = g[cell];
					var bit1 = (digit >> 3 & 1) == 1;
					var bit2 = (digit >> 2 & 1) == 1;
					var bit3 = (digit >> 1 & 1) == 1;
					var bit4 = (digit & 1) == 1;
					bitArray[(cell << 2) + 3] = bit1;
					bitArray[(cell << 2) + 2] = bit2;
					bitArray[(cell << 2) + 1] = bit3;
					bitArray[cell << 2] = bit4;
				}

				// Checks masks.
				foreach (var tempBitArray in solutionList)
				{
					var tempSolution = new int[81];
					for (var cell = 0; cell < 81; cell++)
					{
						var bit1 = (byte)(tempBitArray[cell << 2] ? 1 : 0);
						var bit2 = (byte)(tempBitArray[(cell << 2) + 1] ? 1 : 0);
						var bit3 = (byte)(tempBitArray[(cell << 2) + 2] ? 1 : 0);
						var bit4 = (byte)(tempBitArray[(cell << 2) + 3] ? 1 : 0);
						tempSolution[cell] = (bit1 | bit2 << 1 | bit3 << 2 | bit4 << 3) is var final and not 15 ? final : -1;
					}

					if (compareGrids(tempSolution, g, u))
					{
						var exception = new Exception("Okay for exit.");
						exception.Data.Add(nameof(firstFoundGrid), tempSolution);
						exception.Data.Add(nameof(secondFoundGrid), g.Clone());

						throw exception;
					}
				}

				solutionList.Add(bitArray);
				return;
			}

			int row = p / 9, column = p % 9;
			foreach (var digit in m[p])
			{
				g[p] = digit;

				if (isValid(g, row, column))
				{
					var previousPos = p;

					skipToNextCell(ref p, m, g);
					getSolutions(ref c, m, g, p, u);

					p = previousPos;
				}
			}

			// Backtrack.
			g[p] = -1;


			static void skipToNextCell(ref int currentPos, short[] maskArray, int[] gridValues)
			{
				for (currentPos++; currentPos < 81; currentPos++)
				{
					if (maskArray[currentPos] != 0 && gridValues[currentPos] == -1)
					{
						return;
					}
				}
			}

			static bool isValid(int[] gridValues, int r, int c)
			{
				var number = gridValues[r * 9 + c];

				// Check lines.
				for (var i = 0; i < 9; i++)
				{
					if (i != r && gridValues[i * 9 + c] is var a and not -1 && a == number
						|| i != c && gridValues[r * 9 + i] is var b and not -1 && b == number)
					{
						return false;
					}
				}

				// Check blocks.
				for (int ii = r / 3 * 3, i = ii; i < ii + 3; i++)
				{
					for (int jj = c / 3 * 3, j = jj; j < jj + 3; j++)
					{
						if ((i != r || j != c) && gridValues[i * 9 + j] is var a and not -1 && a == number)
						{
							return false;
						}
					}
				}

				return true;
			}
		}

		static bool compareGrids(int[] firstGrid, int[] secondGrid, scoped in CellMap cellsUsed)
		{
			// Checks whether two masks from two different grids with a same index holds a same value.
			// If so, we should return false.
			for (var cell = 0; cell < 81; cell++)
			{
				if ((firstGrid[cell], secondGrid[cell]) is (var a and not -1, var b and not -1) && a == b)
				{
					// Same digit has been found. Now return false.
					return false;
				}
			}

			// Checks masks.
			foreach (var house in cellsUsed.Houses)
			{
				short mask1 = 0, mask2 = 0;
				foreach (var cell in HousesMap[house])
				{
					if (firstGrid[cell] is var a and not -1)
					{
						mask1 |= (short)(1 << a);
					}
					if (secondGrid[cell] is var b and not -1)
					{
						mask2 |= (short)(1 << b);
					}
				}

				if (mask1 != mask2)
				{
					// Mask is not same, which means two houses hold at least one digit not same.
					return false;
				}
			}

			return true;
		}
	}
}
