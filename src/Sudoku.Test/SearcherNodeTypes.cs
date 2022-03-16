namespace Sudoku.Test;

/// <summary>
/// Defines a node type that provides with the choices for the chain node types.
/// </summary>
[Flags]
public enum SearcherNodeTypes
{
	/// <summary>
	/// Indicates the searcher doesn't search for any extended-typed nodes.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the searcher can search on sole digit in the same region.
	/// </summary>
	SoleDigit = 1,

	/// <summary>
	/// Indicates the searcher can search on candidates in a same cell.
	/// </summary>
	SoleCell = 2,

	/// <summary>
	/// Indicates the searcher can search for locked candidate nodes.
	/// </summary>
	LockedCandidates = 4,

	/// <summary>
	/// Indicates the searcher can search for almost locked set nodes.
	/// </summary>
	AlmostLockedSet = 8,

	/// <summary>
	/// Indicates the searcher can search for almost unique rectangle nodes.
	/// </summary>
	AlmostUniqueRectangle = 16,

	/// <summary>
	/// Indicates the searcher can search for kraken fish nodes.
	/// </summary>
	Kraken = 32
}
