using Microsoft.UI;
using Windows.UI;

namespace Sudoku.UI;

/// <summary>
/// Defines the user preferences in the program.
/// </summary>
internal sealed class UserPreference
{
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
}
