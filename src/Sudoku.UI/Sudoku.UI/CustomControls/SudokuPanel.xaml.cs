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
	/// Indicates the preference.
	/// </summary>
	[DisallowNull]
	public Preference? Preference { get; set; }

	/// <summary>
	/// Indicates the point calculator.
	/// </summary>
	[DisallowNull]
	public PointCalculator? PointCalculator { get; set; }

	/// <summary>
	/// Indicates the grid image generator.
	/// </summary>
	[DisallowNull]
	public GridImageGenerator? GridImageGenerator { get; set; }


	//private void F() => _gig.DrawOnto(BaseCanvas);
}
