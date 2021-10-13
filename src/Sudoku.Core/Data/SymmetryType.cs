namespace Sudoku.Data;

/// <summary>
/// Define a symmetry type.
/// </summary>
[Flags]
public enum SymmetryType : byte
{
	/// <summary>
	/// Indicates none of symmetry type.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the central symmetry type.
	/// </summary>
	Central = 1,

	/// <summary>
	/// Indicates the diagonal symmetry type.
	/// </summary>
	Diagonal = 2,

	/// <summary>
	/// Indicates the anti-diagonal symmetry type.
	/// </summary>
	AntiDiagonal = 4,

	/// <summary>
	/// Indicates the x-axis symmetry type.
	/// </summary>
	XAxis = 8,

	/// <summary>
	/// Indicates the y-axis symmetry type.
	/// </summary>
	YAxis = 16,

	/// <summary>
	/// Indicates both X-axis and Y-axis symmetry types.
	/// </summary>
	AxisBoth = 32,

	/// <summary>
	/// Indicates both diagonal and anti-diagonal symmetry types.
	/// </summary>
	DiagonalBoth = 64,

	/// <summary>
	/// Indicates all symmetry types should be satisfied.
	/// </summary>
	All = 128,
}
