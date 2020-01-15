using System.Diagnostics.CodeAnalysis;

namespace Sudoku.Solving.BruteForces.DancingLinks
{
	internal class Node<T> where T : notnull
	{
		public Node(int index) => (Left, Right, Index) = (this, this, index);


		public int Index { get; }

		[MaybeNull]
		public T Value { get; set; } = default!;

		public Node<T> Left { get; set; }

		public Node<T> Right { get; set; }

		public Node<T> Up { get; set; } = null!;

		public Node<T> Down { get; set; } = null!;

		public ColumnNode<T> ColumnNode { get; set; } = null!;


		public void RemoveVertical()
		{
			Up.Down = Down;
			Down.Up = Up;

			ColumnNode.DecSize();
		}

		public void RemoveHorizontal()
		{
			Right.Left = Left;
			Left.Right = Right;
		}

		public void ReplaceVertical()
		{
			Up.Down = this;
			Down.Up = this;

			ColumnNode.IncSize();
		}

		public void ReplaceHorizontal()
		{
			Right.Left = this;
			Left.Right = this;
		}
	}
}
