namespace Sudoku.Analytics.Eliminations;

/// <summary>
/// Encapsulates a type that represents the reason why the exocet eliminations can be removed.
/// </summary>
[Flags]
public enum ExocetEliminatedReason
{
	/// <summary>
	/// Indicates the elimination is the basic elimination (Target eliminations).
	/// </summary>
	Basic = 1,

	/// <summary>
	/// Indicates the target inference eliminations (that is eliminated via mirror cells).
	/// </summary>
	TargetInference = 1 << 1,

	/// <summary>
	/// Indicates the mirror eliminations.
	/// </summary>
	Mirror = 1 << 2,

	/// <summary>
	/// Indicates the bi-bi pattern.
	/// </summary>
	BiBiPattern = 1 << 3,

	/// <summary>
	/// Indicates the target pair eliminations.
	/// </summary>
	TargetPair = 1 << 4,

	/// <summary>
	/// Indicates the generalized swordfish eliminations.
	/// </summary>
	GeneralizedSwordfish = 1 << 5,

	/// <summary>
	/// Indicates the true base eliminations.
	/// </summary>
	TrueBase = 1 << 6,

	/// <summary>
	/// Indicates the compatibility test eliminations.
	/// </summary>
	CompatibilityTest = 1 << 7
}
