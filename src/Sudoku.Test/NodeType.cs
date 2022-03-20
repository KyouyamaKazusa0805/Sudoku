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
	[MemoryUsageEvaluation<NodeType>(20)]
	[TimeElapsedEvaluation<NodeType>(80)]
	Sole,

	/// <summary>
	/// Indicates the node type is a locked candidates.
	/// </summary>
	[NodeTypeName<NodeType>("Locked candidates node")]
	[MemoryUsageEvaluation<NodeType>(60)]
	[TimeElapsedEvaluation<NodeType>(200)]
	LockedCandidates,

	/// <summary>
	/// Indicates the node type is an almost locked sets.
	/// </summary>
	[NodeTypeName<NodeType>("Almost locked sets node")]
	[MemoryUsageEvaluation<NodeType>(60)]
	[TimeElapsedEvaluation<NodeType>(700)]
	AlmostLockedSets,
}
