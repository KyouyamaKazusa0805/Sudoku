using System.Diagnostics.CodeAnalysis;

namespace Sudoku.Solving.BruteForces.DancingLinks
{
	/// <summary>
	/// Provides a node in a torodial doubly linked list.
	/// </summary>
	/// <typeparam name="T">The type of element.</typeparam>
	internal class Node<T> where T : notnull
	{
		/// <summary>
		/// Initializes an instance with the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		public Node(int index) => (Left, Right, Index) = (this, this, index);


		/// <summary>
		/// The index.
		/// </summary>
		public int Index { get; }

		/// <summary>
		/// The data.
		/// </summary>
		[MaybeNull]
		public T Value { get; set; } = default!;

		/// <summary>
		/// The left node of this current node.
		/// </summary>
		public Node<T> Left { get; set; }

		/// <summary>
		/// The right node of this current node.
		/// </summary>
		public Node<T> Right { get; set; }

		/// <summary>
		/// The up node of this current node.
		/// </summary>
		public Node<T> Up { get; set; } = null!;

		/// <summary>
		/// The down node of this current node.
		/// </summary>
		public Node<T> Down { get; set; } = null!;

		/// <summary>
		/// The column node.
		/// </summary>
		public ColumnNode<T> ColumnNode { get; set; } = null!;


		/// <summary>
		/// Remove the vertical node.
		/// </summary>
		public void RemoveVertical()
		{
			Up.Down = Down;
			Down.Up = Up;

			ColumnNode.DecSize();
		}
		
		/// <summary>
		/// Remove the horizontal node.
		/// </summary>
		public void RemoveHorizontal()
		{
			Right.Left = Left;
			Left.Right = Right;
		}

		/// <summary>
		/// Replace the vertical node with 'this'.
		/// </summary>
		public void ReplaceVertical()
		{
			Up.Down = this;
			Down.Up = this;

			ColumnNode.IncSize();
		}

		/// <summary>
		/// Replace the horizontal node with 'this'.
		/// </summary>
		public void ReplaceHorizontal()
		{
			Right.Left = this;
			Left.Right = this;
		}
	}
}
