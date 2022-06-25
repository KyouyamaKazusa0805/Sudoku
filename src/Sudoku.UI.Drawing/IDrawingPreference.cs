namespace Sudoku.UI.Drawing;

/// <summary>
/// Defines the drawing-related preferences in the program.
/// </summary>
public interface IDrawingPreference
{
	/// <summary>
	/// Indicates whether the current grid displays the candidates.
	/// </summary>
	public abstract bool ShowCandidates { get; set; }

	/// <summary>
	/// Indicates whether the candidate border lines will be shown in the sudoku pane.
	/// </summary>
	public abstract bool ShowCandidateBorderLines { get; set; }

	/// <summary>
	/// Indicates whether the sudoku grid pane will display for wrong digits (cell or candidate values),
	/// using the different color.
	/// </summary>
	public abstract bool EnableDeltaValuesDisplaying { get; set; }

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
	public abstract bool DescendingOrderedInfoBarBoard { get; set; }

	/// <summary>
	/// Indicates the outside border width. The value cannot be negative.
	/// </summary>
	public abstract double OutsideBorderWidth { get; set; }

	/// <summary>
	/// Indicates the width of the block border lines. The value cannot be negative.
	/// </summary>
	public abstract double BlockBorderWidth { get; set; }

	/// <summary>
	/// Indicates the width of the cell border lines. The value cannot be negative.
	/// </summary>
	public abstract double CellBorderWidth { get; set; }

	/// <summary>
	/// Indicates the width of the candidate border lines. The value cannot be negative.
	/// </summary>
	public abstract double CandidateBorderWidth { get; set; }

	/// <summary>
	/// Indicates the value font scale. The value must be between 0 and 1.
	/// </summary>
	public abstract double ValueFontScale { get; set; }

	/// <summary>
	/// Indicates the candidate font scale.
	/// </summary>
	public abstract double CandidateFontScale { get; set; }

	/// <summary>
	/// Indicates the thickness of the stroke lines surrounding with highlight cell.
	/// </summary>
	public abstract double HighlightCellStrokeThickness { get; set; }

#if AUTHOR_FEATURE_CELL_MARKS
	/// <summary>
	/// Indicates the thickness of the stroke lines of the cross mark.
	/// </summary>
	public abstract double AuthorDefined_CrossMarkStrokeThickness { get; set; }
#endif

#if AUTHOR_FEATURE_CANDIDATE_MARKS
	/// <summary>
	/// Indicates the candidate mark stroke thickness.
	/// </summary>
	public abstract double AuthorDefined_CandidateMarkStrokeThickness { get; set; }
#endif

	/// <summary>
	/// Indicates the value font name.
	/// </summary>
	public abstract string ValueFontName { get; set; }

	/// <summary>
	/// Indicates the candidate font name.
	/// </summary>
	public abstract string CandidateFontName { get; set; }

	/// <summary>
	/// Indicates the peer focusing mode.
	/// </summary>
	public abstract PeerFocusingMode PeerFocusingMode { get; set; }

	/// <summary>
	/// Indicates the color of the outside borders.
	/// </summary>
	public abstract Color OutsideBorderColor { get; set; }

	/// <summary>
	/// Indicates the color of the grid background color for filling.
	/// </summary>
	public abstract Color GridBackgroundFillColor { get; set; }

	/// <summary>
	/// Indicates the color of the block borders.
	/// </summary>
	public abstract Color BlockBorderColor { get; set; }

	/// <summary>
	/// Indicates the color of the cell borders.
	/// </summary>
	public abstract Color CellBorderColor { get; set; }

	/// <summary>
	/// Indicates the color of the candidate borders.
	/// </summary>
	public abstract Color CandidateBorderColor { get; set; }

	/// <summary>
	/// Indicates the color of the given values.
	/// </summary>
	public abstract Color GivenColor { get; set; }

	/// <summary>
	/// Indicates the color of the modifiable values.
	/// </summary>
	public abstract Color ModifiableColor { get; set; }

	/// <summary>
	/// Indicates the color of the candidate values.
	/// </summary>
	public abstract Color CandidateColor { get; set; }

	/// <summary>
	/// Indicates the color of the wrong cell value input.
	/// </summary>
	public abstract Color CellDeltaColor { get; set; }

	/// <summary>
	/// Indicates the color of the wrong candidate value input.
	/// </summary>
	public abstract Color CandidateDeltaColor { get; set; }

	/// <summary>
	/// Indicates the color of the mask ellipse color.
	/// </summary>
	public abstract Color MaskEllipseColor { get; set; }

	/// <summary>
	/// Indicates the normal color.
	/// </summary>
	public abstract Color NormalColor { get; set; }

	/// <summary>
	/// Indicates the first auxiliary color.
	/// </summary>
	public abstract Color Auxiliary1Color { get; set; }

	/// <summary>
	/// Indicates the second auxiliary color.
	/// </summary>
	public abstract Color Auxiliary2Color { get; set; }

	/// <summary>
	/// Indicates the third auxiliary color.
	/// </summary>
	public abstract Color Auxiliary3Color { get; set; }

	/// <summary>
	/// Indicates the color that describes an elimination.
	/// </summary>
	public abstract Color EliminationColor { get; set; }

	/// <summary>
	/// Indicates the color that describes an exo-fin.
	/// </summary>
	public abstract Color ExofinColor { get; set; }

	/// <summary>
	/// Indicates the color that describes an endo-fin.
	/// </summary>
	public abstract Color EndofinColor { get; set; }

	/// <summary>
	/// Indicates the color that describes a cannibalism.
	/// </summary>
	public abstract Color CannibalismColor { get; set; }

	/// <summary>
	/// Indicates the color of links used by a chain.
	/// </summary>
	public abstract Color LinkColor { get; set; }

	/// <summary>
	/// Indicates the color of the first ALS recorded.
	/// </summary>
	public abstract Color AlmostLockedSet1Color { get; set; }

	/// <summary>
	/// Indicates the color of the second ALS recorded.
	/// </summary>
	public abstract Color AlmostLockedSet2Color { get; set; }

	/// <summary>
	/// Indicates the color of the third ALS recorded.
	/// </summary>
	public abstract Color AlmostLockedSet3Color { get; set; }

	/// <summary>
	/// Indicates the color of the fourth ALS recorded.
	/// </summary>
	public abstract Color AlmostLockedSet4Color { get; set; }

	/// <summary>
	/// Indicates the color of the fifth ALS recorded.
	/// </summary>
	public abstract Color AlmostLockedSet5Color { get; set; }

	/// <summary>
	/// Indicates the color of the stroke lines surrounding with highlight cell.
	/// </summary>
	public abstract Color HighlightCellStrokeColor { get; set; }

	/// <summary>
	/// Indicates the focused cell color.
	/// </summary>
	public abstract Color FocusedCellColor { get; set; }

	/// <summary>
	/// Indicates the peers focused cell color.
	/// </summary>
	public abstract Color PeersFocusedCellColor { get; set; }

#if AUTHOR_FEATURE_CELL_MARKS
	/// <summary>
	/// Indicates the author-defined cell rectangle color used for filling.
	/// </summary>
	public abstract Color AuthorDefined_CellRectangleFillColor { get; set; }

	/// <summary>
	/// Indicates the author-defined cell circle color used for filling.
	/// </summary>
	public abstract Color AuthorDefined_CellCircleFillColor { get; set; }

	/// <summary>
	/// Indicates the author-defined cross mark stroke color.
	/// </summary>
	public abstract Color AuthorDefined_CrossMarkStrokeColor { get; set; }

	/// <summary>
	/// Indicates the author-defined cell star color used for filling.
	/// </summary>
	public abstract Color AuthorDefined_StarFillColor { get; set; }

	/// <summary>
	/// Indicates the author-defined cell triangle color used for filling.
	/// </summary>
	public abstract Color AuthorDefined_TriangleFillColor { get; set; }

	/// <summary>
	/// Indicates the author-defined cell diamond color used for filling.
	/// </summary>
	public abstract Color AuthorDefined_DiamondFillColor { get; set; }
#endif

#if AUTHOR_FEATURE_CANDIDATE_MARKS
	/// <summary>
	/// Indicates the stroke color of the mark applied to a candidate.
	/// </summary>
	public abstract Color AuthorDefined_CandidateMarkStrokeColor { get; set; }
#endif

	/// <summary>
	/// Indicates the palette colors.
	/// </summary>
	public abstract Color[] PaletteColors { get; set; }


	/// <summary>
	/// Gets the color at the specified index of the palette color list, i.e. the property <see cref="PaletteColors"/>s.
	/// </summary>
	/// <param name="paletteColorIndex">The index.</param>
	/// <returns>The color result.</returns>
	/// <seealso cref="PaletteColors"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed Color GetPaletteColor(int paletteColorIndex) => PaletteColors[paletteColorIndex];
}
