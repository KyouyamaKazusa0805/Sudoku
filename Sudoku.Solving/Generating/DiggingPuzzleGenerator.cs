using System.Text;
using Sudoku.Data;
using Sudoku.Solving.BruteForces.Bitwise;
using static Sudoku.Constants.Processings;

namespace Sudoku.Solving.Generating
{
	/// <summary>
	/// Encapsulates a puzzle generator, whose basic algorithm is digging
	/// some values out of a random answer grid.
	/// </summary>
	public abstract class DiggingPuzzleGenerator : PuzzleGenerator
	{
		/// <summary>
		/// The fast solver.
		/// </summary>
		protected static readonly UnsafeBitwiseSolver FastSolver = new();


		/// <summary>
		/// To generate an answer grid.
		/// </summary>
		/// <param name="puzzle">The puzzle string.</param>
		/// <param name="solution">The solution string.</param>
		protected virtual void GenerateAnswerGrid(StringBuilder puzzle, StringBuilder solution)
		{
			do
			{
				for (int i = 0; i < 81; i++)
				{
					puzzle[i] = '0';
				}

				var map = GridMap.Empty;
				for (int i = 0; i < 16; i++)
				{
					while (true)
					{
						int cell = Rng.Next(0, 81);
						if (!map[cell])
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
						puzzle[cell] = (char)(Rng.Next(1, 9) + '0');
					} while (CheckDuplicate(puzzle, cell));
				}
			} while (FastSolver.Solve(puzzle.ToString(), solution, 2) == 0);
		}

		/// <summary>
		/// To create the pattern.
		/// </summary>
		/// <param name="pattern">The pattern array.</param>
		protected abstract void CreatePattern(int[] pattern);

		/// <summary>
		/// Check whether the digit in its peer cells has duplicate ones.
		/// </summary>
		/// <param name="gridArray">The grid array.</param>
		/// <param name="cell">The cell.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		private static bool CheckDuplicate(StringBuilder gridArray, int cell)
		{
			char value = gridArray[cell];
			foreach (int c in PeerMaps[cell])
			{
				if (value != '0' && gridArray[c] == value)
				{
					return true;
				}
			}

			return false;
		}
	}
}
