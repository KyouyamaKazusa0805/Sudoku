namespace Sudoku.Test;

/// <summary>
/// Defines a node type.
/// </summary>
public enum NodeType
{
	/// <summary>
	/// Indicates the node type is a sole candidate.
	/// </summary>
	Sole,

	/// <summary>
	/// Indicates the node type is a locked candidates.
	/// </summary>
	LockedCandidates,
}
