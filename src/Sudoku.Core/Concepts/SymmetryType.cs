namespace Sudoku.Concepts;

/// <summary>
/// Define a symmetry type.
/// </summary>
[Flags]
[EnumSwitchExpressionRoot("GetName")]
public enum SymmetryType : byte
{
	/// <summary>
	/// Indicates none of symmetry type.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "No symmetry")]
	None = 0,

	/// <summary>
	/// Indicates the central symmetry type.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "Central symmetry type")]
	Central = 1,

	/// <summary>
	/// Indicates the diagonal symmetry type.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "Diagonal symmetry type")]
	Diagonal = 2,

	/// <summary>
	/// Indicates the anti-diagonal symmetry type.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "Anti-diagonal symmetry type")]
	AntiDiagonal = 4,

	/// <summary>
	/// Indicates the x-axis symmetry type.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "X-axis symmetry type")]
	XAxis = 8,

	/// <summary>
	/// Indicates the y-axis symmetry type.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "Y-axis symmetry type")]
	YAxis = 16,

	/// <summary>
	/// Indicates both X-axis and Y-axis symmetry types.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "Both X-axis and Y-axis")]
	AxisBoth = 32,

	/// <summary>
	/// Indicates both diagonal and anti-diagonal symmetry types.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "Both diagonal and anti-diagonal")]
	DiagonalBoth = 64,

	/// <summary>
	/// Indicates all symmetry types should be satisfied.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "All symmetry type")]
	All = 128,
}
