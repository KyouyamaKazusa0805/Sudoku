namespace SudokuStudio.Drawing;

/// <summary>
/// Indicates the drawing mode.
/// </summary>
public enum DrawingMode
{
	/// <summary>
	/// Indicates the mode is none.
	/// </summary>
	None,

	/// <summary>
	/// Indicates the mode is cell filling.
	/// </summary>
	Cell,

	/// <summary>
	/// Indicates the mode is candidate filling.
	/// </summary>
	Candidate,

	/// <summary>
	/// Indicates the mode is house filling.
	/// </summary>
	House,

	/// <summary>
	/// Indicates the mode is chute filling.
	/// </summary>
	Chute,

	/// <summary>
	/// Indicates the mode is link drawing.
	/// </summary>
	Link,

	/// <summary>
	/// Indicates the mode is baba grouping.
	/// </summary>
	BabaGrouping
}
