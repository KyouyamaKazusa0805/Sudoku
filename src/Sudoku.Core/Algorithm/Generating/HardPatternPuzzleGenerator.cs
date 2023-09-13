using System.Runtime.CompilerServices;
using Sudoku.Algorithm.Solving;
using Sudoku.Concepts;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Algorithm.Generating;

/// <summary>
/// Defines a puzzle generator that makes the given pattern as a hard one.
/// A <b>hard pattern</b> doesn't mean the puzzle will be hard or fiendish.
/// </summary>
public sealed unsafe class HardPatternPuzzleGenerator : IPuzzleGenerator
{
	/// <summary>
	/// Indicates the block factor.
	/// </summary>
	private static readonly int[] BlockFactor = [0, 6, 54, 60, 3, 27, 33, 57, 30];

	/// <summary>
	/// Indicates the swapping factor.
	/// </summary>
	private static readonly int[][] SwappingFactor = [[0, 1, 2], [0, 2, 1], [1, 0, 2], [1, 2, 0], [2, 0, 1], [2, 1, 0]];

	/// <summary>
	/// Indicates the inner solver that can fast solve a sudoku puzzle, to check the validity
	/// of a puzzle being generated.
	/// </summary>
	private static readonly BitwiseSolver Solver = new();

	/// <summary>
	/// Indicates the random number generator.
	/// </summary>
	private static readonly Random Rng = new();


	/// <inheritdoc/>
	public Grid Generate(IProgress<GeneratorProgress>? progress = null, CancellationToken cancellationToken = default)
	{
		var puzzle = stackalloc char[81];
		var solution = stackalloc char[81];
		var holeCells = stackalloc Cell[81];
		var progressTimes = 0;
		while (true)
		{
			fixed (char* pEmptyString = Grid.EmptyString)
			{
				Unsafe.CopyBlock(puzzle, pEmptyString, sizeof(char) * 81);
				Unsafe.CopyBlock(solution, pEmptyString, sizeof(char) * 81);
			}

			GenerateAnswerGrid(puzzle, solution);

			Unsafe.InitBlock(holeCells, 0, 81 * sizeof(Cell));
			CreatePattern(holeCells);
			for (var trial = 0; trial < 1000; trial++)
			{
				for (var cell = 0; cell < 81; cell++)
				{
					var p = holeCells[cell];
					var temp = solution[p];
					solution[p] = '0';

					if (!Solver.CheckValidity(solution))
					{
						// Reset the value.
						solution[p] = temp;
					}
				}

				if (Solver.CheckValidity(solution) && Grid.Parse(new string(solution)) is var grid)
				{
					return grid;
				}

				RecreatePattern(holeCells);
			}

			progressTimes += 1000;
			progress?.Report(new(progressTimes));

			cancellationToken.ThrowIfCancellationRequested();
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
					var cell = Rng.Next(81);
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
					pPuzzle[cell] = (char)(Rng.Next(1, 9) + '0');
				} while (CheckDuplicate(pPuzzle, cell));
			}
		} while (Solver.Solve(pPuzzle, pSolution, 2) == 0);
	}

	/// <summary>
	/// Creates a start pattern based on a base pattern.
	/// </summary>
	/// <param name="pattern">The base pattern.</param>
	private void CreatePattern(Cell* pattern)
	{
		for (var (i, a, b) = (0, 54, 0); i < 9; i++)
		{
			var n = (int)(Rng.NextDouble() * 6);
			for (var j = 0; j < 3; j++)
			{
				for (var k = 0; k < 3; k++)
				{
					pattern[(k == SwappingFactor[n][j] ? ref a : ref b)++] = BlockFactor[i] + j * 9 + k;
				}
			}
		}

		RecreatePattern(pattern);
	}

	/// <summary>
	/// To re-create the pattern.
	/// </summary>
	/// <param name="pattern">The pointer that points to an array of the pattern values.</param>
	private void RecreatePattern(Cell* pattern)
	{
#if true
		var target = stackalloc[] { (23, 0, 1), (47, 24, -23), (53, 48, -47), (80, 54, 27) };
		for (var index = 0; index < 4; index++)
		{
			var (initial, boundary, delta) = target[index];
			for (var i = initial; i >= boundary; i--)
			{
				PointerOperations.Swap(pattern + i, pattern + boundary + (Cell)((index == 3 ? delta : (i + delta)) * Rng.NextDouble()));
			}
		}
#else
		for (var i = 23; i >= 0; i--)
		{
			PointerOperations.Swap(pattern + i, pattern + (Cell)((i + 1) * _random.NextDouble()));
		}
		for (var i = 47; i >= 24; i--)
		{
			PointerOperations.Swap(pattern + i, pattern + 24 + (Cell)((i - 23) * _random.NextDouble()));
		}
		for (var i = 53; i >= 48; i--)
		{
			PointerOperations.Swap(pattern + i, pattern + 48 + (Cell)((i - 47) * _random.NextDouble()));
		}
		for (var i = 80; i >= 54; i--)
		{
			PointerOperations.Swap(pattern + i, pattern + 54 + (Cell)(27 * _random.NextDouble()));
		}
#endif
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
