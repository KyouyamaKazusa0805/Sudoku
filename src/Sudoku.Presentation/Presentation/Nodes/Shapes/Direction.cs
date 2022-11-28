namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Defines a direction.
/// </summary>
[Flags]
public enum Direction : byte
{
	/// <summary>
	/// Indicates the direction is none.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the direction is top-left.
	/// </summary>
	TopLeft = 1,

	/// <summary>
	/// Indicates the direction is top-center.
	/// </summary>
	TopCenter = 1 << 1,

	/// <summary>
	/// Indicates the direction is top-right.
	/// </summary>
	TopRight = 1 << 2,

	/// <summary>
	/// Indicates the direction is middle-left.
	/// </summary>
	MiddleLeft = 1 << 3,

	/// <summary>
	/// Indicates the direction is middle-right.
	/// </summary>
	MiddleRight = 1 << 4,

	/// <summary>
	/// Indicates the direction is bottom-left.
	/// </summary>
	BottomLeft = 1 << 5,

	/// <summary>
	/// Indicates the direction is bottom-center.
	/// </summary>
	BottomCenter = 1 << 6,

	/// <summary>
	/// Indicates the direction is bottom-right.
	/// </summary>
	BottomRight = 1 << 7
}
