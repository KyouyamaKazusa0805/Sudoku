using System;
using System.Collections.Generic;
using Sudoku.Data.Meta;
using Sudoku.Solving.Manual;

namespace Sudoku.Solving.Checking
{
	public sealed class BackdoorSearcher
	{
		private static readonly ManualSolver TestSolver = new ManualSolver
		{
			OptimizedApplyingOrder = false,
			EnableFullHouse = false,
			EnableLastDigit = false
		};


		public BackdoorSearcher(int depth)
		{
			Depth = depth > 0 && depth <= 3
				? depth
				: throw new ArgumentOutOfRangeException(nameof(depth));
		}


		public int Depth { get; }


		public IReadOnlyList<IReadOnlyList<Conclusion>> FindBackdoors(Grid grid)
		{
			if (!grid.IsUnique(out var solution))
			{
				throw new InvalidOperationException("The puzzle does not have unique solution.");
			}

			// Store all incorrect candidates.
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

			// Search backdoors.
			var result = new List<IReadOnlyList<Conclusion>>();
			var tempList = new List<Conclusion>();
			var tempGrid = grid.Clone();
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
														result.Add(new List<Conclusion>(tempList));
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
										result.Add(new List<Conclusion>(tempList));
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
						result.Add(new List<Conclusion>(tempList));
						tempList.RemoveAt(tempList.Count - 1);
					}

					tempGrid[c1, d1] = false;
					tempList.Clear();
				} // foreach cand1 in incorrectCandidates[c1]
			} // for c1 0..(82 - Depth)

			// Return the value.
			return result;
		}
	}
}
