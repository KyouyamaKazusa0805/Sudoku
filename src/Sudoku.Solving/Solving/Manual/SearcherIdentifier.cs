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
	/// Indicates the deadly pattern searcher.
	/// </summary>
	DeadlyPattern
}
