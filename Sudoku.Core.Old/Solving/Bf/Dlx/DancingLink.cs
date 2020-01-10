using System.Collections.Generic;
using Sudoku.Data.Meta;

namespace Sudoku.Solving.Bf.Dlx
{
	public partial class DancingLink
	{
		public DancingLink(ColumnNode root) => Root = root;


		public ColumnNode Root { get; set; }


		public ColumnNode CreateLinkedList(Grid grid) =>
			CreateLinkedList(GetArray(grid));

		public ColumnNode CreateLinkedList(int[,] grid)
		{
			var columns = new List<ColumnNode>();
			for (int columnIndex = 0; columnIndex < 324; columnIndex++)
			{
				var col = new ColumnNode(columnIndex)
				{
					Right = Root,
					Left = Root.Left
				};
				Root.Left.Right = col;
				Root.Left = col;
				columns.Add(col);
			}

			for (int x = 0; x < 9; x++)
			{
				for (int y = 0; y < 9; y++)
				{
					if (grid[x, y] == 0)
					{
						for (int d = 0; d < 9; d++)
						{
							// This cell is blank so add all candidates.
							FormLinks(columns, x, y, d);
						}
					}
					else
					{
						// Hint exists in this location so add only for that candidate
						int d = grid[x, y] - 1;
						FormLinks(columns, x, y, d);
					}
				}
			}

			return Root;
		}


		private static void FormLinks(List<ColumnNode> columns, int x, int y, int d)
		{
			var position = new DataNode(x * 81 + y * 9 + d, columns[x * 9 + y]);
			var row = new DataNode(x * 81 + y * 9 + d, columns[81 + x * 9 + d]);
			var column = new DataNode(x * 81 + y * 9 + d, columns[162 + y * 9 + d]);
			var block = new DataNode(
				x * 81 + y * 9 + d, columns[243 + (3 * (x / 3) + y / 3) * 9 + d]);

			var matrix_row = new MatrixRow(position, row, column, block);
			LinkRow(matrix_row);
			LinkRowToColumn(matrix_row.Position);
			LinkRowToColumn(matrix_row.Row);
			LinkRowToColumn(matrix_row.Column);
			LinkRowToColumn(matrix_row.Block);
		}


		private static int[,] GetArray(Grid grid)
		{
			var result = new int[9, 9];
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					var info = grid[i, j];
					result[i, j] = info.IsValueCell ? info.Value + 1 : 0;
				}
			}

			return result;
		}

		private static void LinkRow(MatrixRow d)
		{
			d.Position.Right = d.Column;
			d.Position.Left = d.Block;
			d.Column.Right = d.Row;
			d.Column.Left = d.Position;
			d.Row.Right = d.Block;
			d.Row.Left = d.Column;
			d.Block.Right = d.Position;
			d.Block.Left = d.Row;
		}

		private static void LinkRowToColumn(DataNode section)
		{
			var col = section.Column;
			if (!(col is null))
			{
				col.Size++;
				section.Down = col;
				section.Up = col.Up;
				col.Up.Down = section;
				col.Up = section;
			}
		}
	}
}
