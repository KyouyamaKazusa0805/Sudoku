namespace Sudoku.Rendering.Nodes.Shapes;

/// <summary>
/// Defines a direction. This type provides both 4-direction values and 8-direction values.
/// If you want to use by 4-direction notation, just use values:
/// <list type="bullet">
/// <item><see cref="Up"/></item>
/// <item><see cref="Down"/></item>
/// <item><see cref="Left"/></item>
/// <item><see cref="Right"/></item>
/// </list>
/// Others are provided and used by 8-direction notation.
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
	/// Indicates the up direction.
	/// </summary>
	Up = TopCenter,

	/// <summary>
	/// Indicates the direction is top-right.
	/// </summary>
	TopRight = 1 << 2,

	/// <summary>
	/// Indicates the direction is middle-left.
	/// </summary>
	MiddleLeft = 1 << 3,

	/// <summary>
	/// Indicates the left direction.
	/// </summary>
	Left = MiddleLeft,

	/// <summary>
	/// Indicates the direction is middle-right.
	/// </summary>
	MiddleRight = 1 << 4,

	/// <summary>
	/// Indicates the right direction.
	/// </summary>
	Right = MiddleRight,

	/// <summary>
	/// Indicates the direction is bottom-left.
	/// </summary>
	BottomLeft = 1 << 5,

	/// <summary>
	/// Indicates the direction is bottom-center.
	/// </summary>
	BottomCenter = 1 << 6,

	/// <summary>
	/// Indicates the down direction.
	/// </summary>
	Down = BottomCenter,

	/// <summary>
	/// Indicates the direction is bottom-right.
	/// </summary>
	BottomRight = 1 << 7
}
