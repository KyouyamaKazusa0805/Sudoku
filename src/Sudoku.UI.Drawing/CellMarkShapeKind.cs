namespace Sudoku.UI.Drawing;

/// <summary>
/// Defines a shape kind that is used for a mark-like drawing element.
/// </summary>
public enum ShapeKind : byte
{
	/// <summary>
	/// Indicates none of shapes will be displayed.
	/// </summary>
	None,

	/// <summary>
	/// Indicates the shape kind is a rectangle.
	/// </summary>
	Rectangle,

	/// <summary>
	/// Indicates the shape kind is a circle.
	/// </summary>
	Circle,

	/// <summary>
	/// Indicates the shape kind is a cross mark.
	/// </summary>
	CrossMark
}
