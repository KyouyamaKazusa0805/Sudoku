namespace Sudoku.Concepts.Solving.TechniqueStructures;

/// <summary>
/// Represents a relation between two adjacent nodes.
/// </summary>
public enum AdjacentNodesRelation
{
	/// <summary>
	/// Indicates the relation is normal.
	/// </summary>
	Normal,

	/// <summary>
	/// Indicates the relation is almost locked set.
	/// </summary>
	AlmostLockedSet,

	/// <summary>
	/// Indicates the relation is almost hidden set.
	/// </summary>
	AlmostHiddenSet
}
