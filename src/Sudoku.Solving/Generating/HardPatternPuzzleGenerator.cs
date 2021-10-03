using static Sudoku.Generating.IPuzzleGenerator;

namespace Sudoku.Generating;

/// <summary>
/// Defines a puzzle generator that makes the given pattern as a hard one. However,
/// a <b>hard pattern</b> doesn't mean the puzzle will be hard or fiendish.
/// </summary>
public sealed unsafe class HardPatternPuzzleGenerator : IPuzzleGenerator
{
	/// <summary>
	/// Indicates the block factor.
	/// </summary>
	private static readonly int[] BlockFactor = { 0, 6, 54, 60, 3, 27, 33, 57, 30 };

	/// <summary>
	/// Indicates the swapping factor.
	/// </summary>
	private static readonly int[,] SwappingFactor =
	{
		{ 0, 1, 2 }, { 0, 2, 1 }, { 1, 0, 2 },
		{ 1, 2, 0 }, { 2, 0, 1 }, { 2, 1, 0 }
	};


	/// <inheritdoc/>
	public Grid Generate(CancellationToken cancellationToken = default)
	{
		char* puzzle = stackalloc char[81], solution = stackalloc char[81];
		int* holeCells = stackalloc int[81];
		while (true)
		{
			fixed (char* pEmptyString = Grid.EmptyString)
			{
				Unsafe.CopyBlock(puzzle, pEmptyString, sizeof(char) * 81);
				Unsafe.CopyBlock(solution, pEmptyString, sizeof(char) * 81);
			}

			GenerateAnswerGrid(puzzle, solution);

			Unsafe.InitBlock(holeCells, 0, 81);
			CreatePattern(holeCells);
			for (int trial = 0; trial < 1000; trial++)
			{
				cancellationToken.ThrowIfCancellationRequested();

				for (int cell = 0; cell < 81; cell++)
				{
					int p = holeCells[cell];
					char temp = solution[p];
					solution[p] = '0';

					if (!Solver.CheckValidity(solution))
					{
						// Reset the value.
						solution[p] = temp;
					}
				}

				if (Solver.CheckValidity(solution) && Grid.Parse(solution) is var grid)
				{
					return grid;
				}

				RecreatePattern(holeCells);
			}
		}
	}

	/// <inheritdoc/>
	public ValueTask<Grid> GenerateAsync(CancellationToken cancellationToken = default) =>
		new(Generate(cancellationToken));

	/// <summary>
	/// Generates the answer sudoku grid via the specified puzzle and the solution variable pointer.
	/// </summary>
	/// <param name="pPuzzle">The pointer that points to the puzzle.</param>
	/// <param name="pSolution">
	/// The pointer that points to the solution. The result value will be changed here.
	/// </param>
	private void GenerateAnswerGrid([NotNull, DisallowNull] char* pPuzzle, char* pSolution)
	{
		do
		{
			for (int i = 0; i < 81; i++)
			{
				pPuzzle[i] = '0';
			}

			var map = Cells.Empty;
			for (int i = 0; i < 16; i++)
			{
				while (true)
				{
					int cell = Random.Shared.Next(0, 81);
					if (!map.Contains(cell))
					{
						map.AddAnyway(cell);
						break;
					}
				}
			}

			foreach (int cell in map)
			{
				do
				{
					pPuzzle[cell] = (char)(Random.Shared.Next(1, 9) + '0');
				} while (CheckDuplicate(pPuzzle, cell));
			}
		} while (Solver.Solve(pPuzzle, pSolution, 2) == 0);
	}

	/// <summary>
	/// Creates a start pattern based on a base pattern.
	/// </summary>
	/// <param name="pattern">The base pattern.</param>
	private void CreatePattern([NotNull, DisallowNull] int* pattern)
	{
		int a = 54, b = 0;
		for (int i = 0; i < 9; i++)
		{
			int n = (int)(Random.Shared.NextDouble() * 6);
			for (int j = 0; j < 3; j++)
			{
				for (int k = 0; k < 3; k++)
				{
					pattern[(*(k == SwappingFactor[n, j] ? &a : &b))++] = BlockFactor[i] + j * 9 + k;
				}
			}
		}

		RecreatePattern(pattern);
	}


	/// <summary>
	/// To re-create the pattern.
	/// </summary>
	/// <param name="pattern">The pointer that points to an array of the pattern values.</param>
	private static void RecreatePattern([NotNull, DisallowNull] int* pattern)
	{
		for (int i = 23; i >= 0; i--)
		{
			PointerMarshal.Swap(pattern + i, pattern + (int)((i + 1) * Random.Shared.NextDouble()));
		}
		for (int i = 47; i >= 24; i--)
		{
			PointerMarshal.Swap(pattern + i, pattern + 24 + (int)((i - 23) * Random.Shared.NextDouble()));
		}
		for (int i = 53; i >= 48; i--)
		{
			PointerMarshal.Swap(pattern + i, pattern + 48 + (int)((i - 47) * Random.Shared.NextDouble()));
		}
		for (int i = 80; i >= 54; i--)
		{
			PointerMarshal.Swap(pattern + i, pattern + 54 + (int)(27 * Random.Shared.NextDouble()));
		}
	}

	/// <summary>
	/// Check whether the digit in its peer cells has duplicate ones.
	/// </summary>
	/// <param name="ptrGrid">The pointer that pointes to a grid.</param>
	/// <param name="cell">The cell.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	private static bool CheckDuplicate([DisallowNull, NotNullWhen(true)] char* ptrGrid, int cell)
	{
#if DEBUG
		Debug.Assert(ptrGrid != null);
#endif

		char value = ptrGrid[cell];
		foreach (int c in PeerMaps[cell])
		{
			if (value != '0' && ptrGrid[c] == value)
			{
				return true;
			}
		}

		return false;
	}
}
