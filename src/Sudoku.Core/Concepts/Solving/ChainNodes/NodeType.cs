namespace Sudoku.Concepts.Solving.ChainNodes;

/// <summary>
/// Defines a node type.
/// </summary>
public enum NodeType
{
	/// <summary>
	/// Indicates the node type is a sole candidate.
	/// </summary>
	[NodeTypeName("Sole candidate")]
	Sole,

	/// <summary>
	/// Indicates the node type is a locked candidates.
	/// </summary>
	[NodeTypeName("Locked candidates")]
	LockedCandidates,

	/// <summary>
	/// Indicates the node type is an almost locked sets.
	/// </summary>
	[NodeTypeName("Almost locked set")]
	AlmostLockedSets,
}
