namespace Sudoku.Solving.Dlx;

/// <summary>
/// Represents as a dancing link.
/// </summary>
/// <param name="_root">The root node.</param>
public sealed class DancingLink(ColumnNode _root)
{
	/// <summary>
	/// Indicates the number of nodes created in the data model.
	/// </summary>
	private const int NodesCount = 81 << 2;


	/// <summary>
	/// Indicates the entry instance.
	/// </summary>
	public static DancingLink Entry => new(new(-1));


	/// <summary>
	/// Try to create a <see cref="ColumnNode"/> instance, including connection
	/// with all candidates from the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The column node for the root node.</returns>
	/// <seealso cref="ColumnNode"/>
	public ColumnNode Create(ref readonly Grid grid)
	{
		var columns = new ColumnNode[NodesCount];
		for (var columnIndex = 0; columnIndex < NodesCount; columnIndex++)
		{
			var col = new ColumnNode(columnIndex) { Right = _root, Left = _root.Left };
			_root.Left.Right = col;
			_root.Left = col;
			columns[columnIndex] = col;
		}

		for (var cell = 0; cell < 81; cell++)
		{
			var (x, y) = (cell / 9, cell % 9);
			foreach (var digit in grid.GetCandidates(cell))
			{
				formLinks(columns, x, y, digit);
			}
		}
		return _root;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void formLinks(ColumnNode[] cols, RowIndex r, ColumnIndex c, Digit d)
		{
			var candidate = r * 81 + c * 9 + d;
			var cell = new DancingLinkNode(candidate, cols[r * 9 + c]);
			var row = new DancingLinkNode(candidate, cols[81 + r * 9 + d]);
			var column = new DancingLinkNode(candidate, cols[162 + c * 9 + d]);
			var block = new DancingLinkNode(candidate, cols[243 + (r / 3 * 3 + c / 3) * 9 + d]);
			var matrixRow = new MatrixRow(cell, row, column, block);
			linkRow(ref matrixRow);
			linkRowToColumn(matrixRow.Cell);
			linkRowToColumn(matrixRow.Row);
			linkRowToColumn(matrixRow.Column);
			linkRowToColumn(matrixRow.Block);
		}

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
		static void linkRowToColumn(DancingLinkNode s)
		{
			if (s.Column is { } col)
			{
				col.Size++;
				s.Down = col;
				s.Up = col.Up;
				col.Up.Down = s;
				col.Up = s;
			}
		}
	}

	/// <inheritdoc cref="Create(ref readonly Grid)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ColumnNode Create(Digit[] grid) => Create(Grid.Create(grid));
}
