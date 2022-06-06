namespace Sudoku.UI.Drawing;

/// <summary>
/// Defines the drawing-related preferences in the program.
/// </summary>
public interface IDrawingPreference
{
	/// <summary>
	/// Indicates whether the current grid displays the candidates.
	/// </summary>
	bool ShowCandidates { get; set; }

	/// <summary>
	/// Indicates whether the candidate border lines will be shown in the sudoku pane.
	/// </summary>
	bool ShowCandidateBorderLines { get; set; }

	/// <summary>
	/// Indicates whether the sudoku grid pane will display for wrong digits (cell or candidate values),
	/// using the different color.
	/// </summary>
	bool EnableDeltaValuesDisplaying { get; set; }

	/// <summary>
	/// <para>
	/// Indicates whether the info bar controls will always be updated and inserted into the first place
	/// of the whole info bar board. If <see langword="true"/>, descending ordered mode will be enabled,
	/// the behavior will be like the above; otherwise, the new controls will be appended into the last place
	/// of the board.
	/// </para>
	/// <para>
	/// Sets the value to <see langword="true"/> may help you check new hints more quickly than
	/// the case setting the value to <see langword="false"/>.
	/// </para>
	/// </summary>
	bool DescendingOrderedInfoBarBoard { get; set; }

	/// <summary>
	/// Indicates the outside border width. The value cannot be negative.
	/// </summary>
	double OutsideBorderWidth { get; set; }

	/// <summary>
	/// Indicates the width of the block border lines. The value cannot be negative.
	/// </summary>
	double BlockBorderWidth { get; set; }

	/// <summary>
	/// Indicates the width of the cell border lines. The value cannot be negative.
	/// </summary>
	double CellBorderWidth { get; set; }

	/// <summary>
	/// Indicates the width of the candidate border lines. The value cannot be negative.
	/// </summary>
	double CandidateBorderWidth { get; set; }

	/// <summary>
	/// Indicates the value font scale. The value must be between 0 and 1.
	/// </summary>
	double ValueFontScale { get; set; }

	/// <summary>
	/// Indicates the candidate font scale.
	/// </summary>
	double CandidateFontScale { get; set; }

	/// <summary>
	/// Indicates the value font name.
	/// </summary>
	string ValueFontName { get; set; }

	/// <summary>
	/// Indicates the candidate font name.
	/// </summary>
	string CandidateFontName { get; set; }

	/// <summary>
	/// Indicates the color of the outside borders.
	/// </summary>
	Color OutsideBorderColor { get; set; }

	/// <summary>
	/// Indicates the color of the block borders.
	/// </summary>
	Color BlockBorderColor { get; set; }

	/// <summary>
	/// Indicates the color of the cell borders.
	/// </summary>
	Color CellBorderColor { get; set; }

	/// <summary>
	/// Indicates the color of the candidate borders.
	/// </summary>
	Color CandidateBorderColor { get; set; }

	/// <summary>
	/// Indicates the color of the given values.
	/// </summary>
	Color GivenColor { get; set; }

	/// <summary>
	/// Indicates the color of the modifiable values.
	/// </summary>
	Color ModifiableColor { get; set; }

	/// <summary>
	/// Indicates the color of the candidate values.
	/// </summary>
	Color CandidateColor { get; set; }

	/// <summary>
	/// Indicates the color of the wrong cell value input.
	/// </summary>
	Color CellDeltaColor { get; set; }

	/// <summary>
	/// Indicates the color of the wrong candidate value input.
	/// </summary>
	Color CandidateDeltaColor { get; set; }
}
