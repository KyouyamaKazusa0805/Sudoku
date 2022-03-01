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
	/// Indicates whether the sudoku grid pane will display for wrong digits (cell or candidate values),
	/// using the different color.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	public bool EnableDeltaValuesDisplaying = true;

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
	/// The default value is <c>#FF000000</c> (i.e. Black).
	/// </remarks>
	public Color OutsideBorderColor = Colors.Black;

	/// <summary>
	/// Indicates the color of the block borders.
	/// </summary>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. Black).
	/// </remarks>
	public Color BlockBorderColor = Colors.Black;

	/// <summary>
	/// Indicates the color of the cell borders.
	/// </summary>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. Black).
	/// </remarks>
	public Color CellBorderColor = Colors.Black;

	/// <summary>
	/// Indicates the color of the given values.
	/// </summary>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. Black).
	/// </remarks>
	public Color GivenColor = Colors.Black;

	/// <summary>
	/// Indicates the color of the modifiable values.
	/// </summary>
	/// <remarks>
	/// The default value is <c>#FF0000FF</c> (i.e. Blue).
	/// </remarks>
	public Color ModifiableColor = Colors.Blue;

	/// <summary>
	/// Indicates the color of the candidate values.
	/// </summary>
	/// <remarks>
	/// The default value is <c>#FF696969</c> (i.e. DimGray).
	/// </remarks>
	public Color CandidateColor = Colors.DimGray;

	/// <summary>
	/// Indicates the color of the wrong cell value input.
	/// </summary>
	/// <remarks>
	/// The default value is <c>#FFFF0000</c> (i.e. Red).
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
