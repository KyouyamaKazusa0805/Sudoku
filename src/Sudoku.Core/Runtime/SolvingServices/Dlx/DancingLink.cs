namespace Sudoku.Runtime.SolvingServices.Dlx;

/// <summary>
/// Represents as a dancing link.
/// </summary>
/// <param name="_root">The root node.</param>
public sealed class DancingLink(ColumnNode _root)
{
	/// <summary>
	/// Indicates the entry instance.
	/// </summary>
	public static DancingLink Entry => new(new(-1));


	/// <summary>
	/// Creates the links.
	/// </summary>
	/// <param name="gridArray">The grid array.</param>
	/// <returns>The column node for the root node.</returns>
	public ColumnNode CreateLinkedList(Digit[] gridArray)
	{
		var columns = new ColumnNode[324];
		for (var columnIndex = 0; columnIndex < 324; columnIndex++)
		{
			var col = new ColumnNode(columnIndex) { Right = _root, Left = _root.Left };
			_root.Left.Right = col;
			_root.Left = col;
			columns[columnIndex] = col;
		}

		for (var cell = 0; cell < 81; cell++)
		{
			var (x, y) = (cell / 9, cell % 9);
			if (gridArray[cell] == 0)
			{
				// The cell is empty.
				for (var digit = 0; digit < 9; digit++)
				{
					FormLinks(columns, x, y, digit);
				}
			}
			else
			{
				// The cell is given.
				var digit = gridArray[cell] - 1;
				FormLinks(columns, x, y, digit);
			}
		}
		return _root;
	}

	/// <summary>
	/// To form the links via the specified columns, the cell index and the digit used.
	/// </summary>
	/// <param name="columns">The columns having been stored.</param>
	/// <param name="x">The current row index.</param>
	/// <param name="y">The current column index.</param>
	/// <param name="d">The current digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void FormLinks(ColumnNode[] columns, RowIndex x, ColumnIndex y, Digit d)
	{
		var cell = new DancingLinkNode(x * 81 + y * 9 + d, columns[x * 9 + y]);
		var row = new DancingLinkNode(x * 81 + y * 9 + d, columns[81 + x * 9 + d]);
		var column = new DancingLinkNode(x * 81 + y * 9 + d, columns[162 + y * 9 + d]);
		var block = new DancingLinkNode(x * 81 + y * 9 + d, columns[243 + (3 * (x / 3) + y / 3) * 9 + d]);
		var matrixRow = new MatrixRow(cell, row, column, block);
		linkRow(ref matrixRow);
		linkRowToColumn(matrixRow.Cell);
		linkRowToColumn(matrixRow.Row);
		linkRowToColumn(matrixRow.Column);
		linkRowToColumn(matrixRow.Block);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void linkRow(ref MatrixRow d)
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void linkRowToColumn(DancingLinkNode section)
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
	}
}
