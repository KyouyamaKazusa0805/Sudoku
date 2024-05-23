namespace Sudoku.Concepts.Primitive;

/// <summary>
/// Represents a type of transformation.
/// </summary>
/// <remarks><include file="../../global-doc-comments.xml" path="/g/flags-attribute"/></remarks>
[Flags]
public enum TransformType
{
	/// <summary>
	/// The placeholder of this type.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	None = 0,

	/// <summary>
	/// Indicates the transform type is to swap digits.
	/// </summary>
	DigitSwap = 1 << 0,

	/// <summary>
	/// Indicates the transform type is to swap rows.
	/// </summary>
	RowSwap = 1 << 1,

	/// <summary>
	/// Indicates the transform type is to swap columns.
	/// </summary>
	ColumnSwap = 1 << 2,

	/// <summary>
	/// Indicates the transform type is to swap bands.
	/// </summary>
	BandSwap = 1 << 3,

	/// <summary>
	/// Indicates the transform type is to swap towers.
	/// </summary>
	TowerSwap = 1 << 4,

	/// <summary>
	/// Indicates the transform type is to mirror left and right.
	/// </summary>
	MirrorLeftRight = 1 << 5,

	/// <summary>
	/// Indicates the transform type is to mirror top and bottom.
	/// </summary>
	MirrorTopBottom = 1 << 6,

	/// <summary>
	/// Indicates the transform type is to mirror diagonal.
	/// </summary>
	MirrorDiagonal = 1 << 7,

	/// <summary>
	/// Indicates the transform type is to mirror anti-diagonal.
	/// </summary>
	MirrorAntidiagonal = 1 << 8,

	/// <summary>
	/// Indicates the transform type is to rotate clockwise.
	/// </summary>
	RotateClockwise = 1 << 9,

	/// <summary>
	/// Indicates the transform type is to ratate counter-clockwise.
	/// </summary>
	RotateCounterclockwise = 1 << 10
}
