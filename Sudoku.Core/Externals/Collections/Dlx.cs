using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.CallingConvention;

namespace System.Collections.Generic
{
	/// <summary>
	/// Encapsulates the dancing link.
	/// </summary>
	public unsafe struct Dlx
	{
		/// <summary>
		/// The number of max nodes.
		/// </summary>
		private const int KMaxNodes = 1 + 81 * 4 + 9 * 9 * 9 * 4;

		/// <summary>
		/// The number of max columns.
		/// </summary>
		private const int KMaxColumns = 400;

		/// <summary>
		/// The number of max rows / columns / blocks.
		/// </summary>
		private const int KRow = 100, KColumn = 200, KBlock = 300;

		/// <summary>
		/// The size.
		/// </summary>
		private const int N = 9;


		/// <summary>
		/// Initializes an instance with the specified grid array.
		/// </summary>
		/// <param name="gridArray">The array.</param>
		public Dlx(int[] gridArray)
		{
			var c = stackalloc DlxNode*[100];
			Stack = c;

			this = CreateInstance();
			Root = GetNewColumn();
			Root->Left = Root->Right = Root;

			bool[,] rows = new bool[N, 10];
			bool[,] columns = new bool[N, 10];
			bool[,] blocks = new bool[N, 10];

			for (int i = 0; i < N; i++)
			{
				int row = i / 9;
				int column = i % 9;
				int block = row / 3 * 3 + column / 3;
				int value = gridArray[i];
				rows[row, value] = true;
				columns[column, value] = true;
				blocks[block, value] = true;
			}

			for (int i = 0; i < N; i++)
			{
				if (gridArray[i] == 0)
				{
					AppendColumn(i);
				}
			}

			for (int i = 0; i < 9; i++)
			{
				for (int value = 1; value < 10; value++)
				{
					if (!rows[i, value])
					{
						AppendColumn(GetRowColumn(i, value));
					}
					if (!columns[i, value])
					{
						AppendColumn(GetColumnColumn(i, value));
					}
					if (!blocks[i, value])
					{
						AppendColumn(GetBlockColumn(i, value));
					}
				}
			}

			for (int i = 0; i < N; i++)
			{
				if (gridArray[i] == 0)
				{
					int row = i / 9;
					int column = i % 9;
					int block = row / 3 * 3 + column / 3;
					for (int value = 1; value < 10; value++)
					{
						if (!(rows[row, value] || columns[column, value] || blocks[block, value]))
						{
							var n0 = GetNewRow(i);
							var nr = GetNewRow(GetRowColumn(row, value));
							var nc = GetNewRow(GetColumnColumn(column, value));
							var nb = GetNewRow(GetBlockColumn(block, value));

							PutLeft(n0, nr);
							PutLeft(n0, nc);
							PutLeft(n0, nb);
						}
					}
				}
			}
		}


		/// <summary>
		/// The root.
		/// </summary>
		public DlxNode* Root;

		/// <summary>
		/// In out value.
		/// </summary>
		public int* InOut;

		/// <summary>
		/// The array of all columns.
		/// </summary>
		public DlxNode*[] Columns;

		/// <summary>
		/// The stack.
		/// </summary>
		public DlxNode** Stack;

		/// <summary>
		/// The nodes.
		/// </summary>
		public DlxNode[] Nodes;

		/// <summary>
		/// The current node.
		/// </summary>
		public int CurrentNode;


		/// <summary>
		/// Get the index of the value lying on the specified row.
		/// </summary>
		/// <param name="row">The row.</param>
		/// <param name="value">The value.</param>
		/// <returns>The code.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int GetRowColumn(int row, int value) => KRow + row * 10 + value;

		/// <summary>
		/// Get the index of the value lying on the specified column.
		/// </summary>
		/// <param name="column">The column.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int GetColumnColumn(int column, int value) => KColumn + column * 10 + value;

		/// <summary>
		/// Get the index of the value lying on the specified block.
		/// </summary>
		/// <param name="block">The block.</param>
		/// <param name="value">The value.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int GetBlockColumn(int block, int value) => KBlock + block * 10 + value;

		/// <summary>
		/// Append a column.
		/// </summary>
		/// <param name="n">The number label.</param>
		/// <exception cref="Exception">Throws when the current node is not null.</exception>
		public void AppendColumn(int n)
		{
			if (Columns[n] != null)
			{
				throw new Exception("The current node is not null.");
			}

			var c = GetNewColumn(n);
			PutLeft(Root, c);
			Columns[n] = c;
		}

		/// <summary>
		/// Cover the specified column.
		/// </summary>
		/// <param name="c">The column.</param>
		public void Cover(DlxNode* c)
		{
			// Remove the node from the head.
			c->Right->Left = c->Left;
			c->Left->Right = c->Right;

			for (var row = c->Down; row != c; row = row->Down)
			{
				for (var j = row->Right; j != row; j = j->Right)
				{
					j->Down->Up = j->Up;
					j->Up->Down = j->Down;
					j->Column->Size--;
				}
			}
		}

		/// <summary>
		/// Uncover the specified column.
		/// </summary>
		/// <param name="c">The column.</param>
		public void Uncover(DlxNode* c)
		{
			for (var row = c->Up; row != c; row = row->Up)
			{
				for (var j = row->Left; j != row; j = j->Left)
				{
					j->Column->Size++;
					j->Down->Up = j;
					j->Up->Down = j;
				}
			}

			c->Right->Left = c;
			c->Left->Right = c;
		}

		/// <summary>
		/// Put the new node into the left side of the old node.
		/// </summary>
		/// <param name="oldNode">The old node.</param>
		/// <param name="newNode">The new node.</param>
		public void PutLeft(DlxNode* oldNode, DlxNode* newNode)
		{
			newNode->Left = oldNode->Left;
			newNode->Right = oldNode;
			oldNode->Left->Right = newNode;
			oldNode->Left = newNode;
		}

		/// <summary>
		/// Put the new node into the up side of the old node.
		/// </summary>
		/// <param name="oldNode">The old node.</param>
		/// <param name="newNode">The new node.</param>
		public void PutUp(DlxNode* oldNode, DlxNode* newNode)
		{
			// If the old node is the head node, we should insert the new node into the tail.
			// However, to be honest, the relative position between the old node and the new node
			// is unnecessary, because we determine the position without using those two nodes.
			newNode->Up = oldNode->Up;
			newNode->Down = oldNode;
			oldNode->Up->Down = newNode;
			oldNode->Up = newNode;
			oldNode->Size++;
			newNode->Column = oldNode;
		}

		/// <summary>
		/// To solve the specified puzzle.
		/// </summary>
		public void Solve()
		{
			// TODO: Implement the DLX solver.
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get a new row.
		/// </summary>
		/// <param name="column">The column label.</param>
		/// <returns>The pointer.</returns>
		public DlxNode* GetNewRow(int column)
		{
			if (Columns[column] == null || CurrentNode >= KMaxNodes)
			{
				throw new Exception("The current state is invalid.");
			}

			fixed (DlxNode* r = &Nodes[CurrentNode++])
			{
				MemSet(r, 0, sizeof(DlxNode));
				r->Left = r;
				r->Right = r;
				r->Left = r;
				r->Right = r;
				r->Code = column;
				r->Column = Columns[column];
				PutUp(r->Column, r);

				return r;
			}
		}

		/// <summary>
		/// Get a new column with the specified number label.
		/// </summary>
		/// <param name="n">The number label.</param>
		/// <returns>The pointer of the node.</returns>
		/// <exception cref="Exception">
		/// Throws when the current node is greater than <see cref="KMaxNodes"/>.
		/// </exception>
		/// <seealso cref="KMaxNodes"/>
		public DlxNode* GetNewColumn(int n = 0)
		{
			if (CurrentNode >= KMaxNodes)
			{
				throw new Exception("The current node is greater than the total number of nodes.");
			}

			fixed (DlxNode* c = &Nodes[CurrentNode++])
			{
				c->Left = c;
				c->Right = c;
				c->Up = c;
				c->Down = c;
				c->Column = c;
				c->Code = n;

				return c;
			}
		}


		/// <summary>
		/// The default constructor.
		/// </summary>
		/// <returns>The instance.</returns>
		public static Dlx CreateInstance() => new Dlx { Columns = new DlxNode*[400], Nodes = new DlxNode[KMaxNodes] };


		/// <summary>
		/// The C language <c>memset</c> method.
		/// </summary>
		/// <param name="dest">The destination pointer.</param>
		/// <param name="c">The value.</param>
		/// <param name="count">The count.</param>
		/// <returns>The pointer.</returns>
		[DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = Cdecl, SetLastError = false)]
		private static extern unsafe void* MemSet(void* dest, int c, int count);
	}
}
