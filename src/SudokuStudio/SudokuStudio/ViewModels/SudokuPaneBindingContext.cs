namespace SudokuStudio.ViewModels;

/// <summary>
/// Provides with a binding context that is used by <see cref="SudokuPane"/>.
/// </summary>
/// <seealso cref="SudokuPane"/>
internal sealed class SudokuPaneBindingContext : BindingContext
{
	private bool _showCoordinateLines;

	private GridCellData[] _cells = null!;


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
