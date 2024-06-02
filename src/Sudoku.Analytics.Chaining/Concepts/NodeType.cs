namespace Sudoku.Concepts;

/// <summary>
/// Represents a node type.
/// </summary>
public enum NodeType
{
	/// <summary>
	/// Indicates the node type is a single candidate.
	/// </summary>
	SingleCandidate,

	/// <summary>
	/// Indicates the node type is a locked candidates.
	/// </summary>
	LockedCandidates,

	/// <summary>
	/// Indicates the node type is an almost locked set.
	/// </summary>
	AlmostLockedSet,

	/// <summary>
	/// Indicates the node type is an almost hidden set.
	/// </summary>
	AlmostHiddenSet,

	/// <summary>
	/// Indicates the node type is a kraken normal fish.
	/// </summary>
	KrakenNormalFish,

	/// <summary>
	/// Indicates the node type is an almost unique rectangle.
	/// </summary>
	AlmostUniqueRectangle
}
