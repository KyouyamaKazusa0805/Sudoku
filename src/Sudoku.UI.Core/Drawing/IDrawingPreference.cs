#if AUTHOR_FEATURE_CELL_MARKS || AUTHOR_FEATURE_CANDIDATE_MARKS
#pragma warning disable IDE1006
#endif

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
	/// Indicates the thickness of the stroke lines surrounding with highlight cell.
	/// </summary>
	public abstract double HighlightCellStrokeThickness { get; set; }

#if AUTHOR_FEATURE_CELL_MARKS
	/// <summary>
	/// Indicates the thickness of the stroke lines of the cross mark.
	/// </summary>
	public abstract double __CrossMarkStrokeThickness { get; set; }
#endif

#if AUTHOR_FEATURE_CANDIDATE_MARKS
	/// <summary>
	/// Indicates the candidate mark stroke thickness.
	/// </summary>
	public abstract double __CandidateMarkStrokeThickness { get; set; }
#endif

	/// <summary>
	/// Indicates the peer focusing mode.
	/// </summary>
	public abstract PeerFocusingMode PeerFocusingMode { get; set; }

	/// <summary>
	/// Indicates the value font.
	/// </summary>
	public abstract FontData ValueFont { get; set; }

	/// <summary>
	/// Indicates the candidate font.
	/// </summary>
	public abstract FontData CandidateFont { get; set; }

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
	/// Indicates the color that describes an assignment.
	/// </summary>
	public abstract Color AssignmentColor { get; set; }

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
	public abstract Color __CellRectangleFillColor { get; set; }

	/// <summary>
	/// Indicates the author-defined cell circle color used for filling.
	/// </summary>
	public abstract Color __CellCircleFillColor { get; set; }

	/// <summary>
	/// Indicates the author-defined cross mark stroke color.
	/// </summary>
	public abstract Color __CrossMarkStrokeColor { get; set; }

	/// <summary>
	/// Indicates the author-defined cell star color used for filling.
	/// </summary>
	public abstract Color __StarFillColor { get; set; }

	/// <summary>
	/// Indicates the author-defined cell triangle color used for filling.
	/// </summary>
	public abstract Color __TriangleFillColor { get; set; }

	/// <summary>
	/// Indicates the author-defined cell diamond color used for filling.
	/// </summary>
	public abstract Color __DiamondFillColor { get; set; }
#endif

#if AUTHOR_FEATURE_CANDIDATE_MARKS
	/// <summary>
	/// Indicates the stroke color of the mark applied to a candidate.
	/// </summary>
	public abstract Color __CandidateMarkStrokeColor { get; set; }
#endif

	/// <summary>
	/// Indicates the auxiliary color that marks and describes the extra highlight candidates.
	/// </summary>
	public abstract Color[] AuxiliaryColors { get; set; }

	/// <summary>
	/// Indicates the almost locked set colors for differing different ALS structures.
	/// </summary>
	public abstract Color[] AlmostLockedSetColors { get; set; }

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

	/// <summary>
	/// Covers the config file by the specified preference instance.
	/// </summary>
	/// <typeparam name="TPreference">The type of the drawing preference.</typeparam>
	/// <param name="preference">The preference instance.</param>
	protected internal sealed void CoverPreferenceBy<TPreference>(TPreference preference)
		where TPreference : IDrawingPreference
	{
		foreach (var propertyInfo in GetType().GetProperties())
		{
			if (propertyInfo is { CanRead: true, CanWrite: true })
			{
				propertyInfo.SetValue(this, propertyInfo.GetValue(preference));
			}
		}
	}
}
