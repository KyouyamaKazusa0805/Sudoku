namespace Sudoku.Solving.Manual;

/// <summary>
/// Encapsulates an enumeration type that stores all possible identifiers for <see cref="IStepSearcher"/>s.
/// </summary>
/// <seealso cref="IStepSearcher"/>
public enum SearcherIdentifier
{
	/// <summary>
	/// Indicates the single step searcher.
	/// </summary>
	Single,

	/// <summary>
	/// Indicates the locked candidates searcher.
	/// </summary>
	LockedCandidates,

	/// <summary>
	/// Indicates the subset searcher.
	/// </summary>
	Subset,

	/// <summary>
	/// Indicates the fish searcher.
	/// </summary>
	Fish,

	/// <summary>
	/// Indicates the deadly pattern searcher.
	/// </summary>
	DeadlyPattern
}
