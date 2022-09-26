namespace Sudoku.Solving.Logical.Implementations.Data;

/// <summary>
/// Encapsulates a type that represents the reason why the exocet eliminations can be removed.
/// </summary>
[Flags]
[EnumSwitchExpressionRoot("GetName", MethodDescription = "Gets the name of the current eliminated reason field.", ThisParameterDescription = "The field.", ReturnValueDescription = "The name of the field.")]
public enum ExocetEliminatedReason : byte
{
	/// <summary>
	/// Indicates the elimination is the basic elimination (Target eliminations).
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "Base")]
	Basic = 1,

	/// <summary>
	/// Indicates the target inference eliminations (that is eliminated via mirror cells).
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "Target inference")]
	TargetInference = 1 << 1,

	/// <summary>
	/// Indicates the mirror eliminations.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "Mirror")]
	Mirror = 1 << 2,

	/// <summary>
	/// Indicates the bi-bi pattern.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "Bi-bi pattern")]
	BiBiPattern = 1 << 3,

	/// <summary>
	/// Indicates the target pair eliminations.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "Target pair")]
	TargetPair = 1 << 4,

	/// <summary>
	/// Indicates the generalized swordfish eliminations.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "Generalized swordfish")]
	GeneralizedSwordfish = 1 << 5,

	/// <summary>
	/// Indicates the true base eliminations.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "True base")]
	TrueBase = 1 << 6,

	/// <summary>
	/// Indicates the compatibility test eliminations.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "Compatibility test")]
	CompatibilityTest = 1 << 7
}
