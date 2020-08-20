using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using Sudoku.Data.Meta;
using Sudoku.Runtime;

namespace Sudoku.Solving.Bf.Dlx
{
	public sealed class DancingLinksSolver : BruteForceSolver
	{
		private int _solutionCount;

		private readonly Stack<DataNode> _answerNodesStack = new Stack<DataNode>();

		private ColumnNode? _root;

		private Grid? _resultGrid;


		public override string Name => "Dancing links";


		public override Grid? Solve(Grid grid, out AnalysisInfo analysisInfo)
		{
			var dlx = new DancingLink(new(-1));
			_root = dlx.CreateLinkedList(grid);
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			try
			{
				Search();

				stopwatch.Stop();

				analysisInfo = new(Name, null, stopwatch.Elapsed, true);
				return _resultGrid;
			}
			catch
			{
				stopwatch.Stop();

				analysisInfo = new(Name, null, stopwatch.Elapsed, false);
				return null;
			}
		}

		public Grid? Solve(int[,] gridArray, out AnalysisInfo analysisInfo)
		{
			var dlx = new DancingLink(new(-1));
			_root = dlx.CreateLinkedList(gridArray);
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			try
			{
				Search();

				stopwatch.Stop();

				analysisInfo = new(Name, null, stopwatch.Elapsed, true);
				return _resultGrid;
			}
			catch
			{
				stopwatch.Stop();

				analysisInfo = new(Name, null, stopwatch.Elapsed, false);
				return null;
			}
		}


		private void Search()
		{
			if (_solutionCount > 1)
			{
				throw new MultipleSolutionsException();
			}

			Contract.Assume(_root is not null);
			if (_root.Right == _root)
			{
				// All columns were removed!
				_solutionCount++;
				RecordSolution(_answerNodesStack, out _resultGrid);
			}
			else
			{
				var c = ChooseNextColumn();
				c.Cover();

				for (var r = c.Down; r != c; r = r.Down)
				{
					_answerNodesStack.Push(r);
					for (var j = r.Right; j != r; j = j.Right)
					{
						Contract.Assert(j.Column is not null);

						j.Column.Uncover();
					}
					Search();
					r = _answerNodesStack.Pop();
					Contract.Assume(r.Column is not null);
					c = r.Column;

					for (var j = r.Left; j != r; j = j.Left)
					{
						Contract.Assert(j.Column is not null);

						j.Column.Uncover();
					}
				}

				c.Uncover();
			}
		}

		private ColumnNode ChooseNextColumn()
		{
			Contract.Assume(!(_root is null));

			int size = int.MaxValue;
			var nextColumn = new ColumnNode(-1);
			var j = _root.Right.Column;
			while (j != _root)
			{
				Contract.Assert(!(j is null));

				if (j.Size < size)
				{
					nextColumn = j;
					size = j.Size;
				}

				j = j.Right.Column;
			}
			return nextColumn;
		}


		private static void RecordSolution(Stack<DataNode> answer, out Grid result)
		{
			Contract.Assume(!(answer is null));

			var resultArray = new int[9, 9];
			var idList = new List<int>(from k in answer select k.Id);
			idList.Sort();

			var gridList = new List<int>(from id in idList select id % 9 + 1);
			for (int i = 0, x = 0, y = 0; i < 81; i++)
			{
				resultArray[y, x++] = gridList[i];
				if ((i + 1) % 9 == 0)
				{
					y++;
					x = 0;
				}
			}

			var grid = new Grid(resultArray);
			result = grid.SimplyValidate() ? grid : throw new NoSolutionException();
		}
	}
}
