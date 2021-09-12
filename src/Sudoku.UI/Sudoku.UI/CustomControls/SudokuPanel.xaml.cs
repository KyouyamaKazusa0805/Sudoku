namespace Sudoku.UI.CustomControls;

/// <summary>
/// Defines a basic sudoku panel.
/// </summary>
public sealed partial class SudokuPanel : UserControl
{
	/// <summary>
	/// Initializes a <see cref="SudokuPanel"/> instance.
	/// </summary>
	public SudokuPanel()
	{
		InitializeComponent();

		GridCanvas = new(MainSudokuGrid);
	}


	/// <summary>
	/// Indicates the grid canvas.
	/// </summary>
	public SudokuGridCanvas GridCanvas { get; }
}
