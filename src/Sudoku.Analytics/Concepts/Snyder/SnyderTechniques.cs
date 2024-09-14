namespace Sudoku.Concepts.Snyder;

/// <summary>
/// Provides with techniques that will be used in Snyder rule.
/// </summary>
/// <remarks><include file="../../global-doc-comments.xml" path="/g/flags-attribute"/></remarks>
[Flags]
public enum SnyderTechniques
{
	/// <summary>
	/// The placeholder of the current enumeration type.
	/// </summary>
	None = 0,

	/// <summary>
	/// Represents Pointing rule.
	/// </summary>
	Pointing = 1 << 0,

	/// <summary>
	/// Represents Claiming rule.
	/// </summary>
	Claiming = 1 << 1,

	/// <summary>
	/// Represents Naked Pair rule.
	/// </summary>
	NakedPair = 1 << 2,

	/// <summary>
	/// Represents Naked Triple rule.
	/// </summary>
	NakedTriple = 1 << 3,

	/// <summary>
	/// Represents Naked Quadruple rule.
	/// </summary>
	NakedQuadruple = 1 << 4,

	/// <summary>
	/// Represents Naked Pair (+) rule.
	/// </summary>
	NakedPairPlus = 1 << 5,

	/// <summary>
	/// Represents Naked Triple (+) rule.
	/// </summary>
	NakedTriplePlus = 1 << 6,

	/// <summary>
	/// Represents Naked Quadruple (+) rule.
	/// </summary>
	NakedQuadruplePlus = 1 << 7,

	/// <summary>
	/// Represents Hidden Pair rule.
	/// </summary>
	HiddenPair = 1 << 8,

	/// <summary>
	/// Represents Hidden Triple rule.
	/// </summary>
	HiddenTriple = 1 << 9,

	/// <summary>
	/// Represents Hidden Quadruple rule.
	/// </summary>
	HiddenQuadruple = 1 << 10,

	/// <summary>
	/// Represents Locked Pair rule.
	/// </summary>
	LockedPair = 1 << 11,

	/// <summary>
	/// Represents Locked Triple rule.
	/// </summary>
	LockedTriple = 1 << 12,

	/// <summary>
	/// Represents Locked Hidden Pair rule.
	/// </summary>
	LockedHiddenPair = 1 << 13,

	/// <summary>
	/// Represents Locked Hidden Triple rule.
	/// </summary>
	LockedHiddenTriple = 1 << 14
}
