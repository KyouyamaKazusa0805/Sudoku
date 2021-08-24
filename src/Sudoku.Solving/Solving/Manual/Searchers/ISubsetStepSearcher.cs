namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher that searches for subset steps.
/// </summary>
public interface ISubsetStepSearcher : IStepSearcher
{
	/// <summary>
	/// Indicates the maximum size of the searcher can search for.
	/// </summary>
	int MaxSize { get; set; }
}