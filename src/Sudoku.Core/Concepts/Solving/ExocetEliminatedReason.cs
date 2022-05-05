namespace Sudoku.Concepts.Solving;

/// <summary>
/// Encapsulates a type that represents the reason why the exocet eliminations can be removed.
/// </summary>
[Flags]
public enum ExocetEliminatedReason : byte
{
	/// <summary>
	/// Indicates the elimination is the basic elimination (Target eliminations).
	/// </summary>
	[EnumFieldName("Base")]
	Basic = 1,

	/// <summary>
	/// Indicates the target inference eliminations (that is eliminated via mirror cells).
	/// </summary>
	[EnumFieldName("Target inference")]
	TargetInference = 1 << 1,

	/// <summary>
	/// Indicates the mirror eliminations.
	/// </summary>
	[EnumFieldName("Mirror")]
	Mirror = 1 << 2,

	/// <summary>
	/// Indicates the bi-bi pattern.
	/// </summary>
	[EnumFieldName("Bi-bi pattern")]
	BiBiPattern = 1 << 3,

	/// <summary>
	/// Indicates the target pair eliminations.
	/// </summary>
	[EnumFieldName("Target pair")]
	TargetPair = 1 << 4,

	/// <summary>
	/// Indicates the generalized swordfish eliminations.
	/// </summary>
	[EnumFieldName("Generalized swordfish")]
	GeneralizedSwordfish = 1 << 5,

	/// <summary>
	/// Indicates the true base eliminations.
	/// </summary>
	[EnumFieldName("True base")]
	TrueBase = 1 << 6,

	/// <summary>
	/// Indicates the compatibility test eliminations.
	/// </summary>
	[EnumFieldName("Compatibility test")]
	CompatibilityTest = 1 << 7
}
