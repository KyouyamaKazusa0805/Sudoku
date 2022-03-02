using Microsoft.UI;
using Windows.UI;

namespace Sudoku.UI;

/// <summary>
/// Defines the user preferences in the program.
/// </summary>
public sealed class UserPreference
{
	/// <summary>
	/// Indicates whether the current grid displays the candidates.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	public bool ShowCandidates = true;

	/// <summary>
	/// Indicates whether the candidate border lines will be shown in the sudoku pane.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	public bool ShowCandidateBorderLines = false;

	/// <summary>
	/// Indicates whether the sudoku grid pane will display for wrong digits (cell or candidate values),
	/// using the different color.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	public bool EnableDeltaValuesDisplaying = true;

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
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	public bool DescendingOrderedInfoBarBoard = true;

	/// <summary>
	/// Indicates the outside border width. The value cannot be negative.
	/// </summary>
	/// <remarks>
	/// The default value is <c>0</c>.
	/// </remarks>
	public double OutsideBorderWidth = 0;

	/// <summary>
	/// Indicates the width of the block border lines. The value cannot be negative.
	/// </summary>
	/// <remarks>
	/// The default value is <c>4</c>.
	/// </remarks>
	public double BlockBorderWidth = 4;

	/// <summary>
	/// Indicates the width of the cell border lines. The value cannot be negative.
	/// </summary>
	/// <remarks>
	/// The default value is <c>1</c>.
	/// </remarks>
	public double CellBorderWidth = 1;

	/// <summary>
	/// Indicates the width of the candidate border lines. The value cannot be negative.
	/// </summary>
	/// <remarks>
	/// The default value is <c>1</c>.
	/// </remarks>
	public double CandidateBorderWidth = 1;

	/// <summary>
	/// Indicates the value font size.
	/// </summary>
	/// <remarks>
	/// The default value is <c>36</c>.
	/// </remarks>
	public double ValueFontSize = 36;

	/// <summary>
	/// Indicates the candidate font size.
	/// </summary>
	/// <remarks>
	/// The default value is <c>16</c>.
	/// </remarks>
	public double CandidateFontSize = 16;

	/// <summary>
	/// Indicates the value font name.
	/// </summary>
	/// <remarks>
	/// The default value is <c>"Tahoma"</c>.
	/// </remarks>
	public string ValueFontName = "Tahoma";

	/// <summary>
	/// Indicates the candidate font name.
	/// </summary>
	/// <remarks>
	/// The default value is <c>"Tahoma"</c>.
	/// </remarks>
	public string CandidateFontName = "Tahoma";

	/// <summary>
	/// Indicates the color of the outside borders.
	/// </summary>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	public Color OutsideBorderColor = Colors.Black;

	/// <summary>
	/// Indicates the color of the block borders.
	/// </summary>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	public Color BlockBorderColor = Colors.Black;

	/// <summary>
	/// Indicates the color of the cell borders.
	/// </summary>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	public Color CellBorderColor = Colors.Black;

	/// <summary>
	/// Indicates the color of the candidate borders.
	/// </summary>
	/// <remarks>
	/// The default value is <c>#FFD3D3D3</c> (i.e. <see cref="Colors.LightGray"/>).
	/// </remarks>
	public Color CandidateBorderColor = Colors.LightGray;

	/// <summary>
	/// Indicates the color of the given values.
	/// </summary>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	public Color GivenColor = Colors.Black;

	/// <summary>
	/// Indicates the color of the modifiable values.
	/// </summary>
	/// <remarks>
	/// The default value is <c>#FF0000FF</c> (i.e. <see cref="Colors.Blue"/>).
	/// </remarks>
	public Color ModifiableColor = Colors.Blue;

	/// <summary>
	/// Indicates the color of the candidate values.
	/// </summary>
	/// <remarks>
	/// The default value is <c>#FF696969</c> (i.e. <see cref="Colors.DimGray"/>).
	/// </remarks>
	public Color CandidateColor = Colors.DimGray;

	/// <summary>
	/// Indicates the color of the wrong cell value input.
	/// </summary>
	/// <remarks>
	/// The default value is <c>#FFFF0000</c> (i.e. <see cref="Colors.Red"/>).
	/// </remarks>
	public Color CellDeltaColor = Colors.Red;

	/// <summary>
	/// Indicates the color of the wrong candidate value input.
	/// </summary>
	/// <remarks>
	/// The default value is <c>#FFFFB9B9</c> (i.e. <see cref="Color"/> {255, 255, 185, 185}).
	/// </remarks>
	public Color CandidateDeltaColor = Color.FromArgb(255, 255, 185, 185);
}
