namespace Sudoku.UI.Drawing;

/// <summary>
/// Defines a border line type.
/// </summary>
public enum BorderLineType
{
	/// <summary>
	/// Indicates the border line is grid line.
	/// </summary>
	Grid = 1,

	/// <summary>
	/// Indicates the border line is block line.
	/// </summary>
	Block = 3,

	/// <summary>
	/// Indicates the border line is cell line.
	/// </summary>
	Cell = 9,

	/// <summary>
	/// Indicates the border line is candidate line.
	/// </summary>
	Candidate = 27
}
