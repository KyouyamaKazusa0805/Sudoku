namespace Sudoku.Concepts.Solving.ChainNodes;

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
	/// Indicates the searcher can search for locked set nodes.
	/// </summary>
	LockedSet = 8,

	/// <summary>
	/// Indicates the searcher can search for hidden set nodes.
	/// </summary>
	HiddenSet = 16,

	/// <summary>
	/// Indicates the searcher can search for almost rectangle nodes.
	/// </summary>
	UniqueRectangle = 32,

	/// <summary>
	/// Indicates the searcher can search for kraken fish nodes.
	/// </summary>
	Kraken = 64
}
