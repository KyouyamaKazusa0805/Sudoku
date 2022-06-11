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
	/// Indicates the thickness of the stroke lines surrounding with highlight cell.
	/// </summary>
	double HighlightCellStrokeThickness { get; set; }

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

	/// <summary>
	/// Indicates the color of the mask ellipse color.
	/// </summary>
	Color MaskEllipseColor { get; set; }

	/// <summary>
	/// Indicates the 1st color in the color palette.
	/// </summary>
	Color PaletteColor1 { get; set; }

	/// <summary>
	/// Indicates the 2nd color in the color palette.
	/// </summary>
	Color PaletteColor2 { get; set; }

	/// <summary>
	/// Indicates the 3rd color in the color palette.
	/// </summary>
	Color PaletteColor3 { get; set; }

	/// <summary>
	/// Indicates the 4th color in the color palette.
	/// </summary>
	Color PaletteColor4 { get; set; }

	/// <summary>
	/// Indicates the 5th color in the color palette.
	/// </summary>
	Color PaletteColor5 { get; set; }

	/// <summary>
	/// Indicates the 6th color in the color palette.
	/// </summary>
	Color PaletteColor6 { get; set; }

	/// <summary>
	/// Indicates the 7th color in the color palette.
	/// </summary>
	Color PaletteColor7 { get; set; }

	/// <summary>
	/// Indicates the 8th color in the color palette.
	/// </summary>
	Color PaletteColor8 { get; set; }

	/// <summary>
	/// Indicates the 9th color in the color palette.
	/// </summary>
	Color PaletteColor9 { get; set; }

	/// <summary>
	/// Indicates the 10th color in the color palette.
	/// </summary>
	Color PaletteColor10 { get; set; }

	/// <summary>
	/// Indicates the 11th color in the color palette.
	/// </summary>
	Color PaletteColor11 { get; set; }

	/// <summary>
	/// Indicates the 12th color in the color palette.
	/// </summary>
	Color PaletteColor12 { get; set; }

	/// <summary>
	/// Indicates the 13th color in the color palette.
	/// </summary>
	Color PaletteColor13 { get; set; }

	/// <summary>
	/// Indicates the 14th color in the color palette.
	/// </summary>
	Color PaletteColor14 { get; set; }

	/// <summary>
	/// Indicates the 15th color in the color palette.
	/// </summary>
	Color PaletteColor15 { get; set; }

	/// <summary>
	/// Indicates the normal color.
	/// </summary>
	Color NormalColor { get; set; }

	/// <summary>
	/// Indicates the color that describes an elimination.
	/// </summary>
	Color EliminationColor { get; set; }

	/// <summary>
	/// Indicates the color that describes an exo-fin.
	/// </summary>
	Color ExofinColor { get; set; }

	/// <summary>
	/// Indicates the color that describes an endo-fin.
	/// </summary>
	Color EndofinColor { get; set; }

	/// <summary>
	/// Indicates the color that describes a cannibalism.
	/// </summary>
	Color CannibalismColor { get; set; }

	/// <summary>
	/// Indicates the color of links used by a chain.
	/// </summary>
	Color LinkColor { get; set; }

	/// <summary>
	/// Indicates the color of the stroke lines surrounding with highlight cell.
	/// </summary>
	Color HighlightCellStrokeColor { get; set; }
}
