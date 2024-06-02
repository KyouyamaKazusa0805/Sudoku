namespace Sudoku.Concepts;

/// <summary>
/// Represents a link type.
/// </summary>
public enum LinkType
{
	/// <summary>
	/// Indicates the link type is a single digit (X rule).
	/// </summary>
	SingleDigit,

	/// <summary>
	/// Indicates the link type is a single cell (Y rule).
	/// </summary>
	SingleCell,

	/// <summary>
	/// Indicates the link type is a locked candidates.
	/// </summary>
	LockedCandidates,

	/// <summary>
	/// Indicates the link type is an almost locked set.
	/// </summary>
	AlmostLockedSet,

	/// <summary>
	/// Indicates the link type is an almost hidden set.
	/// </summary>
	AlmostHiddenSet,

	/// <summary>
	/// Indicates the link type is a kraken normal fish.
	/// </summary>
	KrakenNormalFish,

	/// <summary>
	/// Indicates the link type is an almost unique rectangle.
	/// </summary>
	AlmostUniqueRectangle
}
