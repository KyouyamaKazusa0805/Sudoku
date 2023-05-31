namespace SudokuStudio.Input;

/// <summary>
/// Defines a kind of position.
/// </summary>
public enum Position : byte
{
	/// <summary>
	/// Indicates the center position.
	/// </summary>
	Center,

	/// <summary>
	/// Indicates the top-left position.
	/// </summary>
	TopLeft,

	/// <summary>
	/// Indicates the top-right position.
	/// </summary>
	TopRight,

	/// <summary>
	/// Indicates the bottom-left position.
	/// </summary>
	BottomLeft,

	/// <summary>
	/// Indicates the bottom-right position.
	/// </summary>
	BottomRight
}
