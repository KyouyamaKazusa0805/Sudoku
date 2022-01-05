namespace Sudoku.UI.Windows.DataContexts;

/// <summary>
/// Defines a <see cref="DataContext"/> type that is applied to <see cref="SudokuGridPane"/>.
/// </summary>
/// <seealso cref="SudokuGridPane"/>
public sealed class SudokuGridPaneDataContext : UserControlDataContext
{
	/// <summary>
	/// The back field of the property <see cref="CurrentGrid"/>.
	/// </summary>
	/// <seealso cref="CurrentGrid"/>
	private SudokuGrid _currentGrid;


	/// <summary>
	/// Initializes a <see cref="SudokuGridPaneDataContext"/> instance.
	/// </summary>
	public SudokuGridPaneDataContext()
	{
	}


	/// <summary>
	/// Indicates the sudoku grid that influences the pane.
	/// </summary>
	public SudokuGrid CurrentGrid
	{
		get => _currentGrid;

		set
		{
			if (_currentGrid != value)
			{
				_currentGrid = value;

				RaiseNotification(nameof(CurrentGrid));
			}
		}
	}
}
