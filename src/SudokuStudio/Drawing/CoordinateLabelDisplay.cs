namespace SudokuStudio.Drawing;

/// <summary>
/// Defines a mode that is used for describing a kind of displaying case on coordinate labels.
/// </summary>
public enum CoordinateLabelDisplay
{
	/// <summary>
	/// Indicates the mode that never displays coordinate labels.
	/// </summary>
	None,

	/// <summary>
	/// Indicates the mode that displays for coordinate labels only at upper and left-side.
	/// </summary>
	UpperAndLeft,

	/// <summary>
	/// Indicates the mode that displays for coordinate labels at four directions.
	/// </summary>
	FourDirection
}
