using System;
using System.Collections.Generic;
using System.Extensions;
using System.Threading.Tasks;
using Sudoku.Data;
using Sudoku.Solving.Manual;

namespace Sudoku.Solving.Checking
{
	/// <summary>
	/// Provides a backdoor searcher.
	/// </summary>
	/// <remarks>
	/// <b>Backdoor</b>s are <see cref="Conclusion"/>s making the difficulty of
	/// a puzzle decrease sharply after they are applied to a grid.
	/// </remarks>
	/// <seealso cref="Conclusion"/>
	public sealed class BackdoorSearcher
	{
		/// <summary>
		/// The temporary test solver used in this searcher.
		/// </summary>
		private static readonly LightManualSolver TestSolver = new();


		/// <summary>
		/// Search all backdoors whose level is lower or equals than the
		/// specified depth.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="depth">
		/// The depth you want to search for. The depth value must be between 0 and 3.
		/// where value 0 is for searching for assignments.
		/// </param>
		/// <returns>All backdoors.</returns>
		/// <exception cref="InvalidPuzzleException">Throws when the specified grid is invalid.</exception>
		public IEnumerable<IReadOnlyList<Conclusion>> SearchForBackdoors(in SudokuGrid grid, int depth)
		{
			if (depth is < 0 or > 3)
			{
				return Array.Empty<IReadOnlyList<Conclusion>>();
			}

			if (!grid.IsValid())
			{
				throw new InvalidPuzzleException(
					grid: grid,
					reason: "the grid should contain unique solution before checking backdoors"
				);
			}

			var result = new List<IReadOnlyList<Conclusion>>();
			for (int dep = 0; dep <= depth; dep++)
			{
				SearchForBackdoors(result, grid, dep);
			}

			return result;
		}

		/// <summary>
		/// Search all backdoors whose depth is exactly same as the argument.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="depth">
		/// The depth you want to search for. The depth value must be between 0 and 3.
		/// where value 0 is for searching for assignments.
		/// </param>
		/// <returns>All backdoors.</returns>
		public IEnumerable<IReadOnlyList<Conclusion>> SearchForBackdoorsExact(in SudokuGrid grid, int depth)
		{
			if (depth is < 0 or > 3)
			{
				return Array.Empty<IReadOnlyList<Conclusion>>();
			}

			var result = new List<IReadOnlyList<Conclusion>>();
			SearchForBackdoors(result, grid, depth);
			return result;
		}

		/// <summary>
		/// Search all backdoors whose depth is exactly same as the argument asynchronously.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="depth">
		/// The depth you want to search for. The depth value must be between 0 and 3.
		/// where value 0 is for searching for assignments.
		/// </param>
		/// <returns>The task to calculate all conclusions.</returns>
		public async Task<IEnumerable<IReadOnlyList<Conclusion>>> SearchForBackdoorsExactAsync(
			SudokuGrid grid, int depth) =>
			await Task.Run(() => SearchForBackdoorsExact(grid, depth));


		/// <summary>
		/// To find all backdoors in a sudoku grid.
		/// </summary>
		/// <param name="result">The result list.</param>
		/// <param name="grid">A sudoku grid to search backdoors.</param>
		/// <param name="depth">The depth to search.</param>
		/// <exception cref="InvalidPuzzleException">
		/// Throws when the grid is invalid (has no solution or multiple solutions).
		/// </exception>
		private static void SearchForBackdoors(
			IList<IReadOnlyList<Conclusion>> result, in SudokuGrid grid, int depth)
		{
			if (!grid.IsValid(out SudokuGrid solution))
			{
				throw new InvalidPuzzleException(
					grid: grid,
					reason: "the grid should contain unique solution before checking backdoors"
				);
			}

			var tempGrid = grid;
			if (depth == 0)
			{
				// Search backdoors (Assignments).
				if (TestSolver.CanSolve(tempGrid))
				{
					// All candidates will be marked.
					for (int c = 0; c < 81; c++)
					{
						if (grid.GetStatus(c) != CellStatus.Empty)
						{
							continue;
						}

						int z = solution[c];
						foreach (int d in grid.GetCandidates(c))
						{
							result.Add(new Conclusion[]
							{
								new(d == z ? ConclusionType.Assignment : ConclusionType.Elimination, c, d) }
							);
						}
					}
				}
				else
				{
					for (int cell = 0; cell < 81; cell++)
					{
						if (tempGrid.GetStatus(cell) != CellStatus.Empty)
						{
							continue;
						}

						int digit = solution[cell];
						tempGrid[cell] = digit;

						if (TestSolver.CanSolve(tempGrid))
						{
							// Solve successfully.
							result.Add(new Conclusion[] { new(ConclusionType.Assignment, cell, digit) });
						}

						// Restore data.
						// Simply assigning to trigger the event to re-compute all candidates.
						tempGrid[cell] = -1;
					}
				}

				return;
			}

			// Store all incorrect candidates to prepare for search elimination backdoors.
			var temp = grid;
			var incorrectCandidates = new List<int>();
			for (int cell = 0; cell < 81; cell++)
			{
				if (temp.GetStatus(cell) == CellStatus.Empty)
				{
					int value = solution[cell];
					for (int digit = 0; digit < 9; digit++)
					{
						if (temp[cell, digit] && value != digit)
						{
							incorrectCandidates.Add(cell * 9 + digit);
						}
					}
				}
			}

			// Search backdoors (Eliminations).
			for (int i1 = 0, count = incorrectCandidates.Count; i1 < count + 1 - depth; i1++)
			{
				int c1 = incorrectCandidates[i1];
				tempGrid[c1 / 9, c1 % 9] = false;

				if (depth == 1)
				{
					if (TestSolver.CanSolve(tempGrid))
					{
						result.Add(new Conclusion[] { new(ConclusionType.Elimination, c1) });
					}
				}
				else // depth > 1
				{
					for (int i2 = i1 + 1; i2 < count + 2 - depth; i2++)
					{
						int c2 = incorrectCandidates[i2];
						tempGrid[c2 / 9, c2 % 9] = false;

						if (depth == 2)
						{
							if (TestSolver.CanSolve(tempGrid))
							{
								result.Add(new Conclusion[]
								{
									new(ConclusionType.Elimination, c1),
									new(ConclusionType.Elimination, c2)
								});
							}
						}
						else // depth == 3
						{
							for (int i3 = i2 + 1; i3 < count + 3 - depth; i3++)
							{
								int c3 = incorrectCandidates[i3];
								tempGrid[c3 / 9, c3 % 9] = false;

								if (TestSolver.CanSolve(tempGrid))
								{
									result.Add(new Conclusion[]
									{
										new(ConclusionType.Elimination, c1),
										new(ConclusionType.Elimination, c2),
										new(ConclusionType.Elimination, c3)
									});
								}

								tempGrid[c3 / 9, c3 % 9] = true;
							}
						}

						tempGrid[c2 / 9, c2 % 9] = true;
					}
				}

				tempGrid[c1 / 9, c1 % 9] = true;
			}
		}
	}
}
