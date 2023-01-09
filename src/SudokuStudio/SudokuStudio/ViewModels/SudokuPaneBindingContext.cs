namespace SudokuStudio.ViewModels;

/// <summary>
/// Provides with a binding context that is used by <see cref="SudokuPane"/>.
/// </summary>
/// <seealso cref="SudokuPane"/>
internal sealed class SudokuPaneBindingContext : BindingContext
{
	private int _selectedCell = -1;

	private bool _showCoordinateLines;

	private GridCellData[] _cells;


	public SudokuPaneBindingContext()
	{
		_cells = new GridCellData[81];
		for (var i = 0; i < 81; i++)
		{
			_cells[i] = new() { CellIndex = i };
		}
	}


	/// <summary>
	/// Indicates the selected cell.
	/// </summary>
	public int SelectedCell
	{
		get => _selectedCell;

		set => SetBackingField(ref _selectedCell, value, static (f, v) => f == v, static v => v is >= -1 and < 81);
	}

	/// <summary>
	/// Indicates whether the pane displays for coordinate lines.
	/// </summary>
	public bool ShowCoordinateLines
	{
		get => _showCoordinateLines;

		set => SetBackingField(ref _showCoordinateLines, value, static (f, v) => f == v);
	}

	/// <summary>
	/// Indicates the internal cells used.
	/// </summary>
	public GridCellData[] Cells
	{
		get => _cells;

		set => SetBackingField(ref _cells, value, static (_, _) => true, static v => v.Length == 81);
	}
}
