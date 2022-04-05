namespace Sudoku.Concepts;

/// <summary>
/// Define a symmetry type.
/// </summary>
[Flags]
public enum SymmetryType : byte
{
	/// <summary>
	/// Indicates none of symmetry type.
	/// </summary>
	[EnumFieldName("No symmetry")]
	None = 0,

	/// <summary>
	/// Indicates the central symmetry type.
	/// </summary>
	[EnumFieldName("Central symmetry type")]
	Central = 1,

	/// <summary>
	/// Indicates the diagonal symmetry type.
	/// </summary>
	[EnumFieldName("Diagonal symmetry type")]
	Diagonal = 2,

	/// <summary>
	/// Indicates the anti-diagonal symmetry type.
	/// </summary>
	[EnumFieldName("Anti-diagonal symmetry type")]
	AntiDiagonal = 4,

	/// <summary>
	/// Indicates the x-axis symmetry type.
	/// </summary>
	[EnumFieldName("X-axis symmetry type")]
	XAxis = 8,

	/// <summary>
	/// Indicates the y-axis symmetry type.
	/// </summary>
	[EnumFieldName("Y-axis symmetry type")]
	YAxis = 16,

	/// <summary>
	/// Indicates both X-axis and Y-axis symmetry types.
	/// </summary>
	[EnumFieldName("Both X-axis and Y-axis")]
	AxisBoth = 32,

	/// <summary>
	/// Indicates both diagonal and anti-diagonal symmetry types.
	/// </summary>
	[EnumFieldName("Both diagonal and anti-diagonal")]
	DiagonalBoth = 64,

	/// <summary>
	/// Indicates all symmetry types should be satisfied.
	/// </summary>
	[EnumFieldName("All symmetry type")]
	All = 128,
}
