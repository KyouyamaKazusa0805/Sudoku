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


		/// <inheritdoc/>
		public override AnalysisResult Solve(Grid grid) => Solve(grid.ToArray());

		/// <summary>
		/// Solves the specified grid.
		/// </summary>
		/// <param name="gridValues">The grid values.</param>
		/// <returns>The analysis result.</returns>
		public AnalysisResult Solve(int[] gridValues)
		{
			var stopwatch = new Stopwatch();

			try
			{
				// Starts the stopwatch.
				stopwatch.Start();

				// Initalizes the sudoku value list.
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

				var resultSeries = Search(list);
				var grid = Grid.CreateInstance(gridValues);
				if (resultSeries.Count == 1)
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
				else if (resultSeries.Count == 0)
				{
					throw new NoSolutionException(grid);
				}
				else
				{
					throw new MultipleSolutionsException(grid);
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
		private IReadOnlyList<Node<bool>>? Search_Older(TorodialDoublyLinkedList<bool> list) =>
			SearchRecursively(list, new List<Node<bool>>());

		[Obsolete]
		private IReadOnlyList<Node<bool>>? SearchRecursively(
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

						var result = SearchRecursively(list, solutions);

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
			TorodialDoublyLinkedList<bool> list)
		{
			var resultNodeSeries = new List<List<Node<bool>>>();
			SearchRecursively1(list, new List<Node<bool>>(), resultNodeSeries);
			return resultNodeSeries;
		}

		private static void SearchRecursively1(
			TorodialDoublyLinkedList<bool> list, List<Node<bool>> solutions,
			List<List<Node<bool>>> resultNodeSeries)
		{
			if (ReferenceEquals(list.Head.Right, list.Head))
			{
				resultNodeSeries.Add(new List<Node<bool>>(solutions));
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

					SearchRecursively1(list, solutions, resultNodeSeries);

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
