namespace Sudoku.Algorithm.Generating;

/// <summary>
/// Defines a symmetric puzzle generator, that is, a generator than can include the symmetrical placement
/// of all givens while generating puzzles.
/// </summary>
public sealed unsafe class SymmetricPatternPuzzleGenerator : IPuzzleGenerator
{
	/// <inheritdoc cref="HardPatternPuzzleGenerator.Solver"/>
	private static readonly BitwiseSolver Solver = new();


	/// <summary>
	/// Indicates the random number generator.
	/// </summary>
	private readonly Random _random = new();


	/// <inheritdoc/>
	public Grid Generate(CancellationToken cancellationToken = default)
	{
		try
		{
			return Generate(28, SymmetryType.Central, cancellationToken);
		}
		catch (OperationCanceledException)
		{
			return Grid.Undefined;
		}
	}

	/// <summary>
	/// Generates a sudoku puzzle, via the specified number of givens used, the symmetry type, and
	/// a cancellation token to cancel the operation.
	/// </summary>
	/// <param name="max">The maximum number of givens generated.</param>
	/// <param name="symmetryType">The symmetry type.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
	/// <returns>The result sudoku puzzle.</returns>
	private Grid Generate(int max, SymmetryType symmetryType, CancellationToken cancellationToken)
	{
		string puzzle = new('0', 81), solution = new('0', 81);
		fixed (char* pPuzzle = puzzle, pSolution = solution)
		{
			GenerateAnswerGrid(pPuzzle, pSolution);

			// Now we remove some digits from the grid.
			var allTypes = symmetryType.GetAllFlags() ?? new[] { SymmetryType.None };
			var count = allTypes.Length;
			var tempSolution = solution.ToString();
			string result;
			do
			{
				var selectedType = allTypes[_random.Next(count)];
				fixed (char* pTempSolution = tempSolution)
				{
					CopyBlock(pSolution, pTempSolution, sizeof(char) * 81);
				}

				var totalMap = CellMap.Empty;
				do
				{
					int cell;
					do
					{
						cell = _random.Next(81);
					} while (totalMap.Contains(cell));

					int r = cell / 9, c = cell % 9;

					// Get new value of 'last'.
					var tempMap = CellMap.Empty;
					foreach (var tCell in GetCells(selectedType, r, c))
					{
						pSolution[tCell] = '0';
						totalMap.Add(tCell);
						tempMap.Add(tCell);
					}

					if (cancellationToken.IsCancellationRequested)
					{
						throw new OperationCanceledException();
					}
				} while (81 - totalMap.Count > max);
			} while (!Solver.CheckValidity(result = solution));

			return Grid.Parse(result);
		}
	}

	/// <summary>
	/// Generates the answer sudoku grid via the specified puzzle and the solution variable pointer.
	/// </summary>
	/// <param name="pPuzzle">The pointer that points to the puzzle.</param>
	/// <param name="pSolution">
	/// The pointer that points to the solution. The result value will be changed here.
	/// </param>
	private void GenerateAnswerGrid(char* pPuzzle, char* pSolution)
	{
		do
		{
			for (var i = 0; i < 81; i++)
			{
				pPuzzle[i] = '0';
			}

			var map = CellMap.Empty;
			for (var i = 0; i < 16; i++)
			{
				while (true)
				{
					var cell = _random.Next(81);
					if (!map.Contains(cell))
					{
						map.Add(cell);
						break;
					}
				}
			}

			foreach (var cell in map)
			{
				do
				{
					pPuzzle[cell] = (char)(_random.Next(1, 9) + '0');
				} while (CheckDuplicate(pPuzzle, cell));
			}
		} while (Solver.Solve(pPuzzle, pSolution, 2) == 0);
	}


	/// <summary>
	/// Get the cells that is used for swapping via the specified symmetry type, and the specified row
	/// and column value.
	/// </summary>
	/// <param name="symmetryType">The symmetry type.</param>
	/// <param name="row">The row value.</param>
	/// <param name="column">The column value.</param>
	/// <returns>The cells.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int[] GetCells(SymmetryType symmetryType, int row, int column)
		=> symmetryType switch
		{
			SymmetryType.Central => new[] { row * 9 + column, (8 - row) * 9 + 8 - column },
			SymmetryType.Diagonal => new[] { row * 9 + column, column * 9 + row },
			SymmetryType.AntiDiagonal => new[] { row * 9 + column, (8 - column) * 9 + 8 - row },
			SymmetryType.XAxis => new[] { row * 9 + column, (8 - row) * 9 + column },
			SymmetryType.YAxis => new[] { row * 9 + column, row * 9 + 8 - column },
			SymmetryType.DiagonalBoth => new[] { row * 9 + column, column * 9 + row, (8 - column) * 9 + 8 - row, (8 - row) * 9 + 8 - column },
			SymmetryType.AxisBoth => new[] { row * 9 + column, (8 - row) * 9 + column, row * 9 + 8 - column, (8 - row) * 9 + 8 - column },
			SymmetryType.All
				=> new[]
				{
					row * 9 + column,
					row * 9 + (8 - column),
					(8 - row) * 9 + column,
					(8 - row) * 9 + (8 - column),
					column * 9 + row,
					column * 9 + (8 - row),
					(8 - column) * 9 + row,
					(8 - column) * 9 + (8 - row)
				},
			SymmetryType.None => new[] { row * 9 + column },
			_ => Array.Empty<int>()
		};

	/// <summary>
	/// Check whether the digit in its peer cells has duplicate ones.
	/// </summary>
	/// <param name="ptrGrid">The pointer that points to a grid.</param>
	/// <param name="cell">The cell.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	private static bool CheckDuplicate(char* ptrGrid, int cell)
	{
		var value = ptrGrid[cell];
		foreach (var c in Peers[cell])
		{
			if (value != '0' && ptrGrid[c] == value)
			{
				return true;
			}
		}

		return false;
	}
}
