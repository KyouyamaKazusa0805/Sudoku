namespace Sudoku.UI.CustomControls;

/// <summary>
/// Defines a basic sudoku panel.
/// </summary>
public sealed partial class SudokuPanel : UserControl
{
	/// <summary>
	/// Initializes a <see cref="SudokuPanel"/> instance.
	/// </summary>
	public SudokuPanel() => InitializeComponent();


	/// <summary>
	/// Indicates the grid canvas.
	/// </summary>
	public SudokuGridCanvas GridCanvas { get; private set; } = null!;

	/// <summary>
	/// Indicates the preference used.
	/// </summary>
	public Preference Preference { get; } = new();


	private partial void This_Loaded(object sender, RoutedEventArgs e);
	private partial void MainSudokuGrid_DragEnterAsync(object sender, DragEventArgs e);
}
