namespace Sudoku.Runtime.AnalysisServices;

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
	/// Indicates the searcher can search on sole digit in the same house.
	/// </summary>
	SoleDigit = 1,

	/// <summary>
	/// Indicates the searcher can search on candidates in a same cell.
	/// </summary>
	SoleCell = 1 << 1,

	/// <summary>
	/// Indicates the searcher can search for locked candidate nodes.
	/// </summary>
	LockedCandidates = 1 << 2,

	/// <summary>
	/// Indicates the searcher can search for locked set nodes.
	/// </summary>
	LockedSet = 1 << 3,

	/// <summary>
	/// Indicates the searcher can search for hidden set nodes.
	/// </summary>
	HiddenSet = 1 << 4,

	/// <summary>
	/// Indicates the searcher can search for almost rectangle nodes.
	/// </summary>
	UniqueRectangle = 1 << 5,

	/// <summary>
	/// Indicates the searcher can search for kraken fish nodes.
	/// </summary>
	Kraken = 1 << 6
}
