using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;
using Sudoku.Solving.Manual;

namespace Sudoku.Solving.Checking
{
	public sealed class BackdoorSearcher : IEnumerable<IReadOnlyList<Conclusion>>
	{
		private readonly List<List<Conclusion>> _result = new List<List<Conclusion>>();

		private static readonly ManualSolver TestSolver = new ManualSolver
		{
			OptimizedApplyingOrder = false,
			EnableFullHouse = false,
			EnableLastDigit = false
		};


		public BackdoorSearcher(Grid grid, int depth)
		{
			Depth = depth >= 0 && depth <= 3
				? depth
				: throw new ArgumentOutOfRangeException(nameof(depth));

			FindBackdoors(grid);
		}


		public int Depth { get; }


		public override string ToString()
		{
			const string separator = ", ";
			var sb = new StringBuilder();

			foreach (var eachConclusionList in _result)
			{
				foreach (var conclusion in eachConclusionList)
				{
					sb.Append($"{conclusion}{separator}");
				}

				sb.RemoveFromEnd(separator.Length);
				sb.AppendLine();
			}

			return sb.ToString();
		}

		public IEnumerator<IReadOnlyList<Conclusion>> GetEnumerator() =>
			_result.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _result.GetEnumerator();

		private void FindBackdoors(Grid grid)
		{
			if (!grid.IsUnique(out var solution))
			{
				throw new InvalidOperationException("The puzzle does not have unique solution.");
			}

			var tempGrid = grid.Clone();

			// Search backdoors (Assignments).
			for (int cellOffset = 0; cellOffset < 81; cellOffset++)
			{
				if (tempGrid.GetCellStatus(cellOffset) != CellStatus.Empty)
				{
					continue;
				}

				int digit = solution[cellOffset];
				tempGrid[cellOffset] = digit;

				var analysisResult = TestSolver.Solve(tempGrid);
				if (analysisResult.HasSolved
					&& analysisResult.DifficultyLevel == DifficultyLevels.Easy)
				{
					// Solve successfully.
					_result.Add(new List<Conclusion>
					{
						new Conclusion(ConclusionType.Assignment, cellOffset, digit)
					});
				}

				// Restore data.
				// Simply assigning to trigger the event to re-compute all candidates.
				tempGrid[cellOffset] = -1;
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
					if (grid.GetCellStatus(cellOffset) != CellStatus.Empty)
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
			for (int c1 = 0; c1 < 82 - Depth; c1++)
			{
				if (tempGrid.GetCellStatus(c1) != CellStatus.Empty)
				{
					continue;
				}

				foreach (int cand1 in incorrectCandidates[c1])
				{
					int d1 = cand1 % 9;
					tempGrid[c1, d1] = true;

					tempList.Add(new Conclusion(ConclusionType.Elimination, c1, d1));

					var analysisResult = TestSolver.Solve(tempGrid);
					if (!analysisResult.HasSolved
						|| analysisResult.DifficultyLevel != DifficultyLevels.Easy)
					{
						// Fail to solve.
						if (Depth > 1)
						{
							for (int c2 = c1 + 1; c2 < 83 - Depth; c2++)
							{
								if (tempGrid.GetCellStatus(c2) != CellStatus.Empty)
								{
									continue;
								}

								foreach (int cand2 in incorrectCandidates[c2])
								{
									int d2 = cand2 % 9;
									tempGrid[c2, d2] = true;

									tempList.Add(new Conclusion(ConclusionType.Elimination, c2, d2));

									analysisResult = TestSolver.Solve(tempGrid);
									if (!analysisResult.HasSolved
										|| analysisResult.DifficultyLevel != DifficultyLevels.Easy)
									{
										// Fail to solve.
										if (Depth > 2)
										{
											for (int c3 = c2 + 1; c3 < 81; c3++)
											{
												if (tempGrid.GetCellStatus(c3) != CellStatus.Empty)
												{
													continue;
												}

												foreach (int cand3 in incorrectCandidates[c3])
												{
													int d3 = cand3 % 9;
													tempGrid[c3, d3] = true;

													tempList.Add(new Conclusion(ConclusionType.Elimination, c3, d3));

													analysisResult = TestSolver.Solve(tempGrid);
													if (analysisResult.HasSolved
														&& analysisResult.DifficultyLevel == DifficultyLevels.Easy)
													{
														// Solve successfully.
														// Note this condition.
														_result.Add(new List<Conclusion>(tempList));
													}

													tempGrid[c3, d3] = false;
													tempList.Clear();
												} // foreach cand3 in incorrectCandidates[c3]
											} // for c3 (c2 + 1)..81
										} // if Depth > 2
									}
									else
									{
										// Solve successfully.
										_result.Add(new List<Conclusion>(tempList));
										tempList.RemoveAt(tempList.Count - 1);
									}

									tempGrid[c2, d2] = false;
									tempList.Clear();
								} // foreach cand2 in incorrectCandidates[c2]
							} // for c2 (c1 + 1)..(83 - Depth)
						} // if Depth > 1
					}
					else
					{
						// Solve successfully.
						_result.Add(new List<Conclusion>(tempList));
						tempList.RemoveAt(tempList.Count - 1);
					}

					tempGrid[c1, d1] = false;
					tempList.Clear();
				} // foreach cand1 in incorrectCandidates[c1]
			} // for c1 0..(82 - Depth)
		}
	}
}
