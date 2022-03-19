namespace Sudoku.Test;

/// <summary>
/// Defines a node type.
/// </summary>
public enum NodeType
{
	/// <summary>
	/// Indicates the node type is a sole candidate.
	/// </summary>
	[NodeTypeName<NodeType>("Sole candidate")]
	Sole,

	/// <summary>
	/// Indicates the node type is a locked candidates.
	/// </summary>
	[NodeTypeName<NodeType>("Locked candidates node")]
	LockedCandidates,

	/// <summary>
	/// Indicates the node type is an almost locked sets.
	/// </summary>
	[NodeTypeName<NodeType>("Almost locked sets node")]
	AlmostLockedSets,
}
