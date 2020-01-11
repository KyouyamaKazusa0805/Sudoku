using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;
using Sudoku.Runtime;

namespace Sudoku.Solving.BruteForces.DancingLinks
{
	public sealed class DancingLinksSolver : Solver
	{
		private readonly StringBuilder _debuggingInfoStringBuilder = new StringBuilder();


		public override string SolverName => "Dancing links";


		public override AnalysisResult Solve(Grid grid) => Solve(grid.ToArray());

		public AnalysisResult Solve(int[] gridValues)
		{
			var stopwatch = new Stopwatch();
			var sb = new StringBuilder();

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

				var list = new TorodialDoubleLinkedList<bool>(9 * 9 << 2);
				list.ProcessMatrix(booleanList);

				var results = DancingLinks(list);
				if (results is null)
				{
					throw new NoSolutionException();
				}
				else
				{
					sb.AppendLine("Linking IDs (For debugging):");
					sb.AppendLine(_debuggingInfoStringBuilder);
					sb.AppendLine("Filling steps:");
					foreach (var result in results)
					{
						var (r, c, v) = rcvList[result.Index];

						sb.AppendLine($"r{r + 1}c{c + 1} = {v};");
						s.Values[r, c] = v;
					}

					return new AnalysisResult(
						initialGrid: Grid.CreateInstance(gridValues),
						solverName: SolverName,
						hasSolved: true,
						solution: Grid.CreateInstance(s.Values),
						elapsedTime: stopwatch.Elapsed,
						solvingList: null,
						additional: sb.ToString());
				}
			}
			catch (Exception)
			{
				if (stopwatch.IsRunning)
				{
					stopwatch.Stop();
				}

				return new AnalysisResult(
					initialGrid: Grid.CreateInstance(gridValues),
					solverName: SolverName,
					hasSolved: false,
					solution: null,
					elapsedTime: stopwatch.Elapsed,
					solvingList: null,
					additional: sb.ToString());
			}
		}

		private IList<Node<bool>>? DancingLinks(TorodialDoubleLinkedList<bool> list) =>
			Search(list, new List<Node<bool>>());

		private IList<Node<bool>>? Search(TorodialDoubleLinkedList<bool> list, List<Node<bool>> solutions)
		{
			if (ReferenceEquals(list.Head.Right, list.Head))
			{
				foreach (var result in solutions)
				{
					_debuggingInfoStringBuilder.AppendLine((result.ColumnNode.Id, result.Index));
				}

				_debuggingInfoStringBuilder.AppendLine();
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

						var result = Search(list, solutions);

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

		private static ColumnNode<bool>? GetNextColumn(TorodialDoubleLinkedList<bool> list)
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
