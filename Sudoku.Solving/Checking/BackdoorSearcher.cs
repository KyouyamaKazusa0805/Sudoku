using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Solving.Manual;
using static Sudoku.Data.CellStatus;
using static Sudoku.Solving.ConclusionType;
using IBackdoorSet = System.Collections.Generic.IReadOnlyList<Sudoku.Solving.Conclusion>;

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
		private static readonly LightManualSolver TestSolver = new LightManualSolver();


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
		public IEnumerable<IBackdoorSet> SearchForBackdoors(IReadOnlyGrid grid, int depth)
		{
			if (depth < 0 || depth > 3)
			{
				return Array.Empty<IBackdoorSet>();
			}

			if (!grid.IsValid(out _))
			{
				throw new SudokuRuntimeException();
			}

			var result = new List<IBackdoorSet>();
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
		public IEnumerable<IBackdoorSet> SearchForBackdoorsExact(
			IReadOnlyGrid grid, int depth)
		{
			if (depth < 0 || depth > 3)
			{
				return Array.Empty<IBackdoorSet>();
			}

			var result = new List<IBackdoorSet>();
			SearchForBackdoors(result, grid, depth);
			return result;
		}


		/// <summary>
		/// To find all backdoors in a sudoku grid.
		/// </summary>
		/// <param name="result">The result list.</param>
		/// <param name="grid">A sudoku grid to search backdoors.</param>
		/// <param name="depth">The depth to search.</param>
		/// <exception cref="InvalidOperationException">
		/// Throws when the grid is invalid (has no solution or multiple solutions).
		/// </exception>
		private static void SearchForBackdoors(
			IList<IBackdoorSet> result, IReadOnlyGrid grid, int depth)
		{
			if (!grid.IsValid(out var solution))
			{
				throw new InvalidOperationException("The puzzle does not have unique solution.");
			}

			var tempGrid = grid.Clone();

			if (depth == 0)
			{
				// Search backdoors (Assignments).
				for (int cellOffset = 0; cellOffset < 81; cellOffset++)
				{
					if (tempGrid.GetCellStatus(cellOffset) != Empty)
					{
						continue;
					}

					int digit = solution[cellOffset];
					tempGrid[cellOffset] = digit;

					if (TestSolver.Solve(tempGrid).HasSolved)
					{
						// Solve successfully.
						result.Add(new[] { new Conclusion(Assignment, cellOffset, digit) });
					}

					// Restore data.
					// Simply assigning to trigger the event to re-compute all candidates.
					tempGrid[cellOffset] = -1;
				}

				return;
			}

			// Store all incorrect candidates to prepare for search elimination backdoors.
			var incorrectCandidates = new IList<int>[81];
			for (int cellOffset = 0; cellOffset < 81; cellOffset++)
			{
				int value = solution[cellOffset];
				ref var currentList = ref incorrectCandidates[cellOffset];
				currentList = new List<int>();
				for (int digit = 0; digit < 9; digit++)
				{
					if (grid.GetCellStatus(cellOffset) != Empty)
					{
						// This cell does not need to fill with a digit.
						continue;
					}

					if (!grid[cellOffset, digit] && value != digit)
					{
						currentList.Add(cellOffset * 9 + digit);
					}
				}
			}

			// Search backdoors (Eliminations).
			var tempList = new List<Conclusion>();
			for (int c1 = 0; c1 < 82 - depth; c1++)
			{
				if (tempGrid.GetCellStatus(c1) != Empty)
				{
					continue;
				}

				foreach (int cand1 in incorrectCandidates[c1])
				{
					int d1 = cand1 % 9;
					tempGrid[c1, d1] = true;

					tempList.Add(new Conclusion(Elimination, c1, d1));

					if (TestSolver.Solve(tempGrid).HasSolved)
					{
						// Solve successfully.
						result.Add(new List<Conclusion>(tempList));
					}
					else
					{
						// Fail to solve.
						if (depth > 1)
						{
							for (int c2 = c1 + 1; c2 < 83 - depth; c2++)
							{
								if (tempGrid.GetCellStatus(c2) != Empty)
								{
									continue;
								}

								foreach (int cand2 in incorrectCandidates[c2])
								{
									int d2 = cand2 % 9;
									tempGrid[c2, d2] = true;

									tempList.Add(new Conclusion(Elimination, c2, d2));

									if (TestSolver.Solve(tempGrid).HasSolved)
									{
										// Solve successfully.
										result.Add(new List<Conclusion>(tempList));
									}
									else
									{
										// Fail to solve.
										if (depth > 2)
										{
											for (int c3 = c2 + 1; c3 < 81; c3++)
											{
												if (tempGrid.GetCellStatus(c3) != Empty)
												{
													continue;
												}

												foreach (int cand3 in incorrectCandidates[c3])
												{
													int d3 = cand3 % 9;
													tempGrid[c3, d3] = true;

													tempList.Add(new Conclusion(Elimination, c3, d3));

													if (TestSolver.Solve(tempGrid).HasSolved)
													{
														// Solve successfully.
														result.Add(new List<Conclusion>(tempList));
													}

													tempGrid[c3, d3] = false;
													tempList.RemoveLastElement();
												}
											}
										}
									}

									tempGrid[c2, d2] = false;
									tempList.RemoveLastElement();
								}
							}
						}
					}

					tempGrid[c1, d1] = false;
					tempList.RemoveLastElement();
				}
			}
		}
	}
}
