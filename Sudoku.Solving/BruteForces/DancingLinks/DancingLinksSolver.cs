using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Sudoku.Data.Meta;
using Sudoku.Runtime;

namespace Sudoku.Solving.BruteForces.DancingLinks
{
	/// <summary>
	/// Provides a solver using dancing links (DLX) algorithm.
	/// </summary>
	public sealed class DancingLinksSolver : Solver
	{
		/// <inheritdoc/>
		public override string SolverName => "Dancing links";


		/// <summary>
		/// Checks whether the grid has only one solution. If the grid has multiple
		/// solutions or no solution, this method will return <see langword="false"/>.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="solutionIfValid">
		/// (<see langword="out"/> parameter) The result grid if the puzzle is valid.
		/// If the grid is invalid, this value will be <see langword="null"/>.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public bool CheckValidity(
			IReadOnlyGrid grid, [NotNullWhen(true)] out IReadOnlyGrid? solutionIfValid)
		{
			try
			{
				// Initializes the sudoku value list.
				var s = new SudokuValueList();
				for (int i = 0; i < 9; i++)
				{
					int[] values = new int[9];
					for (int j = 0; j < 9; j++)
					{
						values[j] = grid[i * 9 + j] + 1;
					}

					s.SetRow(values, i);
				}

				// Processes the value list.
				var (booleanList, rcvList) = s.CalculateMatrix();

				var list = new TorodialDoublyLinkedList<bool>(9 * 9 << 2);
				list.ProcessMatrix(booleanList);

				var resultSeries = Search(list, 2);
				switch (resultSeries.Count)
				{
					case 1:
					{
						foreach (var result in resultSeries[0])
						{
							var (r, c, v) = rcvList[result.Index];
							s.Values[r, c] = v;
						}

						solutionIfValid = Grid.CreateInstance(s.Values);
						return true;
					}
					case 0:
					{
						throw new NoSolutionException(grid);
					}
					default:
					{
						throw new MultipleSolutionsException(grid);
					}
				}
			}
			catch
			{
				solutionIfValid = null;
				return false;
			}
		}

		/// <inheritdoc/>
		public override AnalysisResult Solve(IReadOnlyGrid grid) =>
			Solve(grid.ToArray(), int.MaxValue);

		/// <summary>
		/// Solves the specified grid.
		/// </summary>
		/// <param name="gridValues">The grid values.</param>
		/// <param name="count">The number of solutions to search for.</param>
		/// <returns>The analysis result.</returns>
		public AnalysisResult Solve(int[] gridValues, int count)
		{
			var stopwatch = new Stopwatch();

			try
			{
				// Starts the stopwatch.
				stopwatch.Start();

				// Initializes the sudoku value list.
				var s = new SudokuValueList();
				for (int i = 0; i < 9; i++)
				{
					int[] values = new int[9];
					for (int j = 0; j < 9; j++)
					{
						values[j] = gridValues[i * 9 + j];
					}

					s.SetRow(values, i);
				}

				// Processes the value list.
				var (booleanList, rcvList) = s.CalculateMatrix();

				var list = new TorodialDoublyLinkedList<bool>(9 * 9 << 2);
				list.ProcessMatrix(booleanList);

				var resultSeries = Search(list, count);
				var grid = Grid.CreateInstance(gridValues);
				switch (resultSeries.Count)
				{
					case 1:
					{
						foreach (var result in resultSeries[0])
						{
							var (r, c, v) = rcvList[result.Index];
							s.Values[r, c] = v;
						}

						return new AnalysisResult(
							puzzle: grid,
							solverName: SolverName,
							hasSolved: true,
							solution: Grid.CreateInstance(s.Values),
							elapsedTime: stopwatch.Elapsed,
							solvingList: null,
							additional: null);
					}
					case 0:
					{
						throw new NoSolutionException(grid);
					}
					default:
					{
						throw new MultipleSolutionsException(grid);
					}
				}
			}
			catch (Exception ex)
			{
				if (stopwatch.IsRunning)
				{
					stopwatch.Stop();
				}

				return new AnalysisResult(
					puzzle: Grid.CreateInstance(gridValues),
					solverName: SolverName,
					hasSolved: false,
					solution: null,
					elapsedTime: stopwatch.Elapsed,
					solvingList: null,
					additional: $"{ex.Message}");
			}
		}

		[Obsolete]
		[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
		private IReadOnlyList<Node<bool>>? Search_Old(TorodialDoublyLinkedList<bool> list) =>
			SearchRecursively_Old(list, new List<Node<bool>>());

		[Obsolete]
		private IReadOnlyList<Node<bool>>? SearchRecursively_Old(
			TorodialDoublyLinkedList<bool> list, List<Node<bool>> solutions)
		{
			if (ReferenceEquals(list.Head.Right, list.Head))
			{
				return solutions;
			}
			else
			{
				var tempColumn = GetNextColumn(list);
				if (!(tempColumn is null))
				{
					Cover(tempColumn);
					Node<bool> rowNode = tempColumn;
					while (!ReferenceEquals(rowNode.Down, tempColumn))
					{
						rowNode = rowNode.Down;

						solutions.Add(rowNode);

						var rightNode = rowNode;

						while (!ReferenceEquals(rightNode.Right, rowNode))
						{
							rightNode = rightNode.Right;

							Cover(rightNode);
						}

						var result = SearchRecursively_Old(list, solutions);

						if (!(result is null))
						{
							return result;
						}

						solutions.Remove(rowNode);
						tempColumn = rowNode.ColumnNode;

						var leftNode = rowNode;

						while (!ReferenceEquals(leftNode.Left, rowNode))
						{
							leftNode = leftNode.Left;

							Uncover(leftNode);
						}
					}

					Uncover(tempColumn);
				}
			}

			return null;
		}

		private static IReadOnlyList<IReadOnlyList<Node<bool>>> Search(
			TorodialDoublyLinkedList<bool> list, int count)
		{
			var resultNodeSeries = new List<List<Node<bool>>>();
			int tempCount = count;
			SearchRecursively(list, new List<Node<bool>>(), resultNodeSeries, ref tempCount);
			return resultNodeSeries;
		}

		/// <exception cref="SudokuRuntimeException">
		/// Throws when the puzzle has multiple solutions.
		/// </exception>
		private static void SearchRecursively(
			TorodialDoublyLinkedList<bool> list, List<Node<bool>> solutions,
			List<List<Node<bool>>> resultNodeSeries, ref int count)
		{
			if (ReferenceEquals(list.Head.Right, list.Head))
			{
				resultNodeSeries.Add(new List<Node<bool>>(solutions));
				if (--count == 0)
				{
					throw new SudokuRuntimeException(
						"The searching should be terminated ahead of time due to " +
						"the puzzle has found the specified maximum number of solutions.");
				}
			}

			var tempColumn = GetNextColumn(list);
			if (!(tempColumn is null))
			{
				Cover(tempColumn);
				Node<bool> rowNode = tempColumn;
				while (!ReferenceEquals(rowNode.Down, tempColumn))
				{
					rowNode = rowNode.Down;

					solutions.Add(rowNode);

					var rightNode = rowNode;

					while (!ReferenceEquals(rightNode.Right, rowNode))
					{
						rightNode = rightNode.Right;

						Cover(rightNode);
					}

					SearchRecursively(list, solutions, resultNodeSeries, ref count);

					solutions.Remove(rowNode);
					tempColumn = rowNode.ColumnNode;

					var leftNode = rowNode;

					while (!ReferenceEquals(leftNode.Left, rowNode))
					{
						leftNode = leftNode.Left;

						Uncover(leftNode);
					}
				}

				Uncover(tempColumn);
			}
		}

		private static ColumnNode<bool>? GetNextColumn(TorodialDoublyLinkedList<bool> list)
		{
			var node = list.Head;
			ColumnNode<bool>? chosenNode = null;

			while (!ReferenceEquals(node.Right, list.Head))
			{
				node = (ColumnNode<bool>)node.Right;
				if (chosenNode is null || node.Size < chosenNode.Size)
				{
					chosenNode = node;
				}
			}

			return chosenNode;
		}

		private static void Cover(Node<bool> node)
		{
			var column = node.ColumnNode;

			column.RemoveHorizontal();

			Node<bool> verticalNode = column;

			while (!ReferenceEquals(verticalNode.Down, column))
			{
				verticalNode = verticalNode.Down;

				var removeNode = verticalNode;
				while (!ReferenceEquals(removeNode.Right, verticalNode))
				{
					removeNode = removeNode.Right;

					removeNode.RemoveVertical();
				}
			}
		}

		private static void Uncover(Node<bool> node)
		{
			var column = node.ColumnNode;
			Node<bool> verticalNode = column;

			while (!ReferenceEquals(verticalNode.Up, column))
			{
				verticalNode = verticalNode.Up;

				var removeNode = verticalNode;
				while (!ReferenceEquals(removeNode.Left, verticalNode))
				{
					removeNode = removeNode.Left;

					removeNode.ReplaceVertical();
				}
			}

			column.ReplaceHorizontal();
		}
	}
}
