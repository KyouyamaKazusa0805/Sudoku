namespace Sudoku.Solving.DancingLinx;

/// <summary>
/// Represents as a dancing link.
/// </summary>
internal sealed class DancingLink
{
	/// <summary>
	/// Initializes a <see cref="DancingLink"/> instance via the specified root node.
	/// </summary>
	/// <param name="root">The root node.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DancingLink(ColumnNode root) => Root = root;


	/// <summary>
	/// Indicates the root node.
	/// </summary>
	public ColumnNode Root { get; set; }


	/// <summary>
	/// Creates the links.
	/// </summary>
	/// <param name="gridArray">The grid array.</param>
	/// <returns>The column node for the root node.</returns>
	public ColumnNode CreateLinkedList(int[] gridArray)
	{
		var columns = new List<ColumnNode>();
		for (int columnIndex = 0; columnIndex < 324; columnIndex++)
		{
			var col = new ColumnNode(columnIndex) { Right = Root, Left = Root.Left };
			Root.Left.Right = col;
			Root.Left = col;
			columns.Add(col);
		}

		for (int i = 0; i < 81; i++)
		{
			int x = i / 9, y = i % 9;
			if (gridArray[i] == 0)
			{
				// The cell is empty.
				for (int d = 0; d < 9; d++)
				{
					FormLinx(columns, x, y, d);
				}
			}
			else
			{
				// The cell is given.
				int d = gridArray[i] - 1;
				FormLinx(columns, x, y, d);
			}
		}

		return Root;
	}

	/// <summary>
	/// Links the row.
	/// </summary>
	/// <param name="d">The matrix row instance.</param>
	private void LinkRow(ref FileLocalType_MatrixRow d)
	{
		d.Cell.Right = d.Column;
		d.Cell.Left = d.Block;
		d.Column.Right = d.Row;
		d.Column.Left = d.Cell;
		d.Row.Right = d.Block;
		d.Row.Left = d.Column;
		d.Block.Right = d.Cell;
		d.Block.Left = d.Row;
	}

	/// <summary>
	/// Links the row to the column.
	/// </summary>
	/// <param name="section">The section.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void LinkRowToColumn(DataNode section)
	{
		if (section.Column is { } col)
		{
			col.Size++;
			section.Down = col;
			section.Up = col.Up;
			col.Up.Down = section;
			col.Up = section;
		}
	}

	/// <summary>
	/// To form the links via the specified columns, the cell index and the digit used.
	/// </summary>
	/// <param name="columns">The columns having been stored.</param>
	/// <param name="x">The current row index.</param>
	/// <param name="y">The current column index.</param>
	/// <param name="d">The current digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void FormLinx(List<ColumnNode> columns, int x, int y, int d)
	{
		var cell = new DataNode(x * 81 + y * 9 + d, columns[x * 9 + y]);
		var row = new DataNode(x * 81 + y * 9 + d, columns[81 + x * 9 + d]);
		var column = new DataNode(x * 81 + y * 9 + d, columns[162 + y * 9 + d]);
		var block = new DataNode(x * 81 + y * 9 + d, columns[243 + (3 * (x / 3) + y / 3) * 9 + d]);
		var matrixRow = new FileLocalType_MatrixRow(cell, row, column, block);

		LinkRow(ref matrixRow);
		LinkRowToColumn(matrixRow.Cell);
		LinkRowToColumn(matrixRow.Row);
		LinkRowToColumn(matrixRow.Column);
		LinkRowToColumn(matrixRow.Block);
	}
}

/// <summary>
/// Represents a matrix row.
/// </summary>
/// <param name="Cell">Indicates the node that represents the current cell.</param>
/// <param name="Row">Indicates the nodes at the current row.</param>
/// <param name="Column">Indicates the nodes at the current column.</param>
/// <param name="Block">Indicates the nodes at the current block.</param>
internal record struct FileLocalType_MatrixRow(DataNode Cell, DataNode Row, DataNode Column, DataNode Block);
