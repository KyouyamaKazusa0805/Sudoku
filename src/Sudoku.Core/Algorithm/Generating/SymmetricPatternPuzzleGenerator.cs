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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	Grid IPuzzleGenerator.Generate(IProgress<GeneratorProgress>? progress, CancellationToken cancellationToken) => Generate(cancellationToken);

	/// <inheritdoc/>
	public Grid Generate(CancellationToken cancellationToken = default)
	{
		try
		{
			return Generate(28, SymmetricType.Central, cancellationToken);
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
	private Grid Generate(int max, SymmetricType symmetryType, CancellationToken cancellationToken)
	{
		var (puzzle, solution) = (new string('0', 81), new string('0', 81));
		fixed (char* pPuzzle = puzzle, pSolution = solution)
		{
			GenerateAnswerGrid(pPuzzle, pSolution);

			// Now we remove some digits from the grid.
			var allTypes = symmetryType.GetAllFlags() ?? new[] { SymmetricType.None };
			var (count, tempSolution) = (allTypes.Length, solution.ToString());
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
					Cell cell;
					do { cell = _random.Next(81); } while (totalMap.Contains(cell));

					var (r, c) = (cell / 9, cell % 9);

					// Get new value of 'last'.
					var tempMap = CellMap.Empty;
					foreach (var tCell in selectedType.GetCells(r, c))
					{
						pSolution[tCell] = '0';
						totalMap.Add(tCell);
						tempMap.Add(tCell);
					}

					cancellationToken.ThrowIfCancellationRequested();
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
				do { pPuzzle[cell] = (char)(_random.Next(1, 9) + '0'); }
				while (CheckDuplicate(pPuzzle, cell));
			}
		} while (Solver.Solve(pPuzzle, pSolution, 2) == 0);
	}


	/// <summary>
	/// Check whether the digit in its peer cells has duplicate ones.
	/// </summary>
	/// <param name="ptrGrid">The pointer that points to a grid.</param>
	/// <param name="cell">The cell.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	private static bool CheckDuplicate(char* ptrGrid, Cell cell)
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
