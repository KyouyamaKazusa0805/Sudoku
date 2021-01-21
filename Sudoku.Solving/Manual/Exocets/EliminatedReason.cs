using System;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Encapsulates a type that represents the reason why the eliminations can be removed.
	/// </summary>
	[Flags]
	public enum EliminatedReason : byte
	{
		/// <summary>
		/// Indicates the elimination is the basic elimination (Target eliminations).
		/// </summary>
		Basic = 1,

		/// <summary>
		/// Indicates the mirror eliminations.
		/// </summary>
		Mirror = 2,

		/// <summary>
		/// Indicates the bi-bi pattern.
		/// </summary>
		BiBiPattern = 4,

		/// <summary>
		/// Indicates the target pair eliminations.
		/// </summary>
		TargetPair = 8,

		/// <summary>
		/// Indicates the generalized swordfish eliminations.
		/// </summary>
		GeneralizedSwordfish = 16,

		/// <summary>
		/// Indicates the true base assignments.
		/// </summary>
		TrueBase = 32,

		/// <summary>
		/// Indicates the compatibility test eliminations.
		/// </summary>
		CompatibilityTest = 64
	}
}
