namespace SudokuStudio.Rendering;

/// <summary>
/// Defines a mode that is used for displaying candidate view nodes.
/// </summary>
public enum CandidateViewNodeDisplay
{
	/// <summary>
	/// Indicates the display mode is to draw a hollow circle.
	/// </summary>
	CircleHollow,

	/// <summary>
	/// Indicates the display mode is to draw a solid circle.
	/// </summary>
	CircleSolid,

	/// <summary>
	/// Indicates the display mode is to draw a hollow square.
	/// </summary>
	SquareHollow,

	/// <summary>
	/// Indicates the display mode is to draw a solid square.
	/// </summary>
	SquareSolid,

	/// <summary>
	/// Indicates the display mode is to draw a hollow rounded rectangle.
	/// </summary>
	RoundedRectangleHollow,

	/// <summary>
	/// Indicates the display mode is to draw a solid rounded rectangle.
	/// </summary>
	RoundedRectangleSolid
}
