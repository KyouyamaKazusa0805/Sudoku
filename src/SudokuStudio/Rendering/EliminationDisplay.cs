namespace SudokuStudio.Rendering;

/// <summary>
/// Defines a mode that is used for displaying an elimination.
/// </summary>
public enum EliminationDisplay
{
	/// <summary>
	/// Indicates the display mode is to draw a solid circle.
	/// </summary>
	CircleSolid,

	/// <summary>
	/// Indicates the display mode is to draw a hollow circle.
	/// </summary>
	CircleHollow,

	/// <summary>
	/// Indicates the display mode is to draw a cross sign.
	/// </summary>
	Cross,

	/// <summary>
	/// Indicates the display mode is to draw a slash.
	/// </summary>
	Slash,

	/// <summary>
	/// Indicates the display mode is to draw a backslash.
	/// </summary>
	Backslash
}
