namespace Sudoku.UI.Windows.Controls;

/// <summary>
/// Defines a pane that displays a sudoku grid.
/// </summary>
public sealed partial class SudokuGridPane : UserControl
{
	/// <summary>
	/// Initializes a <see cref="SudokuGridPane"/> instance.
	/// </summary>
	public SudokuGridPane()
	{
		InitializeComponent();

		// Initializes '_GridBase'.
		_GridBase.AddRowsCount(27);
		_GridBase.AddColumnsCount(27);
	}

}
