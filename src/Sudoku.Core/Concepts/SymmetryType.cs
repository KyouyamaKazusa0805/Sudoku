namespace Sudoku.Concepts;

/// <summary>
/// Define a symmetry type.
/// </summary>
[Flags]
[EnumSwitchExpressionRoot(
	"GetName", MethodDescription = "Get the name of the current symmetry type.",
	ThisParameterDescription = "The type.", ReturnValueDescription = "The name.")]
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
	Diagonal = 1 << 1,

	/// <summary>
	/// Indicates the anti-diagonal symmetry type.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "Anti-diagonal symmetry type")]
	AntiDiagonal = 1 << 2,

	/// <summary>
	/// Indicates the x-axis symmetry type.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "X-axis symmetry type")]
	XAxis = 1 << 3,

	/// <summary>
	/// Indicates the y-axis symmetry type.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "Y-axis symmetry type")]
	YAxis = 1 << 4,

	/// <summary>
	/// Indicates both X-axis and Y-axis symmetry types.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "Both X-axis and Y-axis")]
	AxisBoth = 1 << 5,

	/// <summary>
	/// Indicates both diagonal and anti-diagonal symmetry types.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "Both diagonal and anti-diagonal")]
	DiagonalBoth = 1 << 6,

	/// <summary>
	/// Indicates all symmetry types should be satisfied.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "All symmetry type")]
	All = 1 << 7
}
