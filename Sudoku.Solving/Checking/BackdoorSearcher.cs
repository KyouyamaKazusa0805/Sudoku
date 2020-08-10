using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sudoku.Data;
using Sudoku.Solving.Manual;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.ConclusionType;
using Conclusions = System.Collections.Generic.IReadOnlyList<Sudoku.Data.Conclusion>;

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
		/// <exception cref="SudokuRuntimeException">
		/// Throws when the specified grid is invalid.
		/// </exception>
		public IEnumerable<Conclusions> SearchForBackdoors(IReadOnlyGrid grid, int depth)
		{
			if (depth is < 0 or > 3)
			{
				return Array.Empty<Conclusions>();
			}

			if (!grid.IsValid(out _))
			{
				throw new SudokuRuntimeException();
			}

			var result = new List<Conclusions>();
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
		public IEnumerable<Conclusions> SearchForBackdoorsExact(IReadOnlyGrid grid, int depth)
		{
			if (depth is < 0 or > 3)
			{
				return Array.Empty<Conclusions>();
			}

			var result = new List<Conclusions>();
			SearchForBackdoors(result, grid, depth);
			return result;
		}

		/// <summary>
		/// Search all backdoors whose depth is exactly same as the argument asynchronizedly.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="depth">
		/// The depth you want to search for. The depth value must be between 0 and 3.
		/// where value 0 is for searching for assignments.
		/// </param>
		/// <returns>The task to calculate all conclusions.</returns>
		public async Task<IEnumerable<Conclusions>> SearchForBackdoorsExactAsync(IReadOnlyGrid grid, int depth) =>
			await Task.Run(() => SearchForBackdoorsExact(grid, depth));


		/// <summary>
		/// To find all backdoors in a sudoku grid.
		/// </summary>
		/// <param name="result">The result list.</param>
		/// <param name="grid">A sudoku grid to search backdoors.</param>
		/// <param name="depth">The depth to search.</param>
		/// <exception cref="InvalidOperationException">
		/// Throws when the grid is invalid (has no solution or multiple solutions).
		/// </exception>
		private static void SearchForBackdoors(IList<Conclusions> result, IReadOnlyGrid grid, int depth)
		{
			if (!grid.IsValid(out var solution))
			{
				throw new InvalidOperationException("The puzzle does not have unique solution.");
			}

			var tempGrid = grid.Clone();

			if (depth == 0)
			{
				// Search backdoors (Assignments).
				if (TestSolver.CanSolve(tempGrid))
				{
					// All candidates will be marked.
					for (int c = 0; c < 81; c++)
					{
						if (grid.GetStatus(c) != Empty)
						{
							continue;
						}

						int z = solution[c];
						foreach (int d in grid.GetCandidates(c))
						{
							result.Add(new[] { new Conclusion(d == z ? Assignment : Elimination, c, d) });
						}
					}
				}
				else
				{
					for (int cell = 0; cell < 81; cell++)
					{
						if (tempGrid.GetStatus(cell) != Empty)
						{
							continue;
						}

						int digit = solution[cell];
						tempGrid[cell] = digit;

						if (TestSolver.CanSolve(tempGrid))
						{
							// Solve successfully.
							result.Add(new[] { new Conclusion(Assignment, cell, digit) });
						}

						// Restore data.
						// Simply assigning to trigger the event to re-compute all candidates.
						tempGrid[cell] = -1;
					}
				}

				return;
			}

			// Store all incorrect candidates to prepare for search elimination backdoors.
			int[] incorrectCandidates = (
				from cell in Enumerable.Range(0, 81)
				where grid.GetStatus(cell) == Empty
				let Value = solution[cell]
				from digit in Enumerable.Range(0, 9)
				where !grid[cell, digit] && Value != digit
				select cell * 9 + digit).ToArray();

			// Search backdoors (Eliminations).
			for (int i1 = 0, count = incorrectCandidates.Length; i1 < count + 1 - depth; i1++)
			{
				int c1 = incorrectCandidates[i1];
				tempGrid[c1 / 9, c1 % 9] = true;

				if (depth == 1)
				{
					if (TestSolver.CanSolve(tempGrid))
					{
						result.Add(new[] { new Conclusion(Elimination, c1) });
					}
				}
				else // depth > 1
				{
					for (int i2 = i1 + 1; i2 < count + 2 - depth; i2++)
					{
						int c2 = incorrectCandidates[i2];
						tempGrid[c2 / 9, c2 % 9] = true;

						if (depth == 2)
						{
							if (TestSolver.CanSolve(tempGrid))
							{
								result.Add(
									new[]
									{
										new Conclusion(Elimination, c1),
										new Conclusion(Elimination, c2)
									});
							}
						}
						else // depth == 3
						{
							for (int i3 = i2 + 1; i3 < count + 3 - depth; i3++)
							{
								int c3 = incorrectCandidates[i3];
								tempGrid[c3 / 9, c3 % 9] = true;

								if (TestSolver.CanSolve(tempGrid))
								{
									result.Add(
										new[]
										{
											new Conclusion(Elimination, c1),
											new Conclusion(Elimination, c2),
											new Conclusion(Elimination, c3)
										});
								}

								tempGrid[c3 / 9, c3 % 9] = false;
							}
						}

						tempGrid[c2 / 9, c2 % 9] = false;
					}
				}

				tempGrid[c1 / 9, c1 % 9] = false;
			}
		}
	}
}
