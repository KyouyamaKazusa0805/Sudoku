namespace Sudoku.Primitive;

/// <summary>
/// Represents a symmetric type that can describe which one a pattern or a puzzle uses.
/// </summary>
/// <remarks><include file="../../global-doc-comments.xml" path="/g/flags-attribute"/></remarks>
[Flags]
public enum SymmetricType
{
	/// <summary>
	/// Indicates none of symmetry type.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the central symmetry type.
	/// </summary>
	Central = 1 << 0,

	/// <summary>
	/// Indicates the diagonal symmetry type.
	/// </summary>
	Diagonal = 1 << 1,

	/// <summary>
	/// Indicates the anti-diagonal symmetry type.
	/// </summary>
	AntiDiagonal = 1 << 2,

	/// <summary>
	/// Indicates the x-axis symmetry type.
	/// </summary>
	XAxis = 1 << 3,

	/// <summary>
	/// Indicates the y-axis symmetry type.
	/// </summary>
	YAxis = 1 << 4,

	/// <summary>
	/// Indicates both X-axis and Y-axis symmetry types.
	/// </summary>
	AxisBoth = 1 << 5,

	/// <summary>
	/// Indicates both diagonal and anti-diagonal symmetry types.
	/// </summary>
	DiagonalBoth = 1 << 6,

	/// <summary>
	/// Indicates all symmetry types should be satisfied.
	/// </summary>
	All = 1 << 7
}
