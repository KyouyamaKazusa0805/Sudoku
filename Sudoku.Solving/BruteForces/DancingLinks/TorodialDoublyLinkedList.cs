using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Sudoku.Data.Extensions;

namespace Sudoku.Solving.BruteForces.DancingLinks
{
	/// <summary>
	/// Provides a torodial doubly linked list (also called
	/// circular doubly linked list).
	/// </summary>
	/// <typeparam name="T">The type of the element.</typeparam>
	internal sealed class TorodialDoublyLinkedList<T> where T : notnull
	{
		/// <summary>
		/// All columns.
		/// </summary>
		private readonly IList<ColumnNode<T>> _columns = new List<ColumnNode<T>>();


		/// <summary>
		/// Initializes an instance with column number.
		/// </summary>
		/// <param name="noColumns">The column number.</param>
		public TorodialDoublyLinkedList(int noColumns)
		{
			_columns.AddRange(from i in Enumerable.Range(0, noColumns) select new ColumnNode<T>(i));

			Head.Right = _columns[0];
			_columns[0].Left = Head;
			Head.Left = _columns[noColumns - 1];
			_columns[noColumns - 1].Right = Head;

			for (int i = 0; i < noColumns - 1; i++)
			{
				_columns[i].Right = _columns[i + 1];
				_columns[i + 1].Left = _columns[i];
			}
		}


		/// <summary>
		/// Indicates the head node.
		/// </summary>
		public ColumnNode<T> Head { get; } = new ColumnNode<T>(-1);


		/// <summary>
		/// Process the matrix.
		/// </summary>
		/// <param name="matrix">The matrix.</param>
		public void ProcessMatrix(bool[,] matrix)
		{
			for (int y = 0; y < matrix.GetLength(0); y++)
			{
				var nodes = new List<(int, Node<T>)>();

				for (int x = 0; x < _columns.Count; x++)
				{
					if (matrix[y, x])
					{
						nodes.Add((x, new Node<T>(y)));
					}
				}

				ProcessMatrixRow(nodes);
			}
		}

		/// <summary>
		/// Process the matrix.
		/// </summary>
		/// <param name="matrix">The matrix.</param>
		public void ProcessMatrix(List<bool[]> matrix)
		{
			for (int y = 0; y < matrix.Count; y++)
			{
				var nodes = new List<(int, Node<T>)>();

				for (int x = 0; x < _columns.Count; x++)
				{
					if (matrix[y][x])
					{
						nodes.Add((x, new Node<T>(y)));
					}
				}

				ProcessMatrixRow(nodes);
			}
		}

		/// <summary>
		/// Add the specified node to the specified index in column.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="node">The node.</param>
		public void AddToColumn(int index, Node<T> node)
		{
			var lowestNode = _columns[index].Up;

			lowestNode.Down = node;
			node.Up = lowestNode;
			_columns[index].Up = node;
			node.Down = _columns[index];
			node.ColumnNode = _columns[index];

			_columns[index].IncSize();
		}

#if DEBUG
		/// <inheritdoc/>
		public override string ToString()
		{
			var builder = new StringBuilder();

			var table = new List<IList<byte>>();
			var columns = new List<int>();

			var horizontalNode = Head;
			int maxHeight = 0;

			char[] letters =
			{
				'A', 'B', 'C', 'D', 'E', 'F', 'G',
				'H', 'I', 'J', 'K', 'L', 'M', 'N',
				'O', 'P', 'Q', 'R', 'S', 'T',
				'U', 'V', 'W', 'X', 'Y', 'Z'
			};

			while (!ReferenceEquals(horizontalNode.Right, Head))
			{
				horizontalNode = (ColumnNode<T>)horizontalNode.Right;

				Node<T> verticalNode = horizontalNode;

				columns.Add(horizontalNode.Id);
				table.Add(new List<byte>());

				while (!ReferenceEquals(verticalNode.Down, horizontalNode))
				{
					verticalNode = verticalNode.Down;

					InsertAt(verticalNode.Index, table[^1], 1);

					if (verticalNode.Index >= maxHeight)
					{
						maxHeight = verticalNode.Index + 1;
					}
				}
			}

			builder.Append("H ");

			for (int x = 0; x < columns.Count; x++)
			{
				builder.Append(letters[columns[x] % 26] + " ");
			}

			builder.AppendLine("|");

			for (int y = 0; y < maxHeight; y++)
			{
				builder.Append("  ");

				for (int x = 0; x < columns.Count; x++)
				{
					builder.Append(table[x].Count > y && table[x][y] > 0 ? "1 " : "0 ");
				}

				builder.AppendLine("|");
			}

			return builder.ToString();
		}
#endif

		/// <summary>
		/// Insert the specified value into the list.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="list">The list.</param>
		/// <param name="value">The value.</param>
		private void InsertAt(int index, IList<byte> list, byte value)
		{
			while (list.Count < index)
			{
				list.Add(0);
			}

			list.Add(value);
		}

		/// <summary>
		/// Process the matrix row.
		/// </summary>
		/// <param name="nodes">All nodes.</param>
		private void ProcessMatrixRow(List<(int _id, Node<T> _nodeList)> nodes)
		{
			for (int i = 0; i < nodes.Count; i++)
			{
				nodes[i]._nodeList.Left = nodes[WrapIndex(i - 1, nodes.Count)]._nodeList;
				nodes[i]._nodeList.Right = nodes[WrapIndex(i + 1, nodes.Count)]._nodeList;

				AddToColumn(nodes[i]._id, nodes[i]._nodeList);
			}
		}


		/// <summary>
		/// Wraps the index.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="length">The length.</param>
		/// <returns>The new index.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int WrapIndex(int value, int length) =>
			value >= length ? value - length : value < 0 ? value + length : value;
	}
}
