namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher that searches for fish steps.
/// </summary>
public interface IFishStepSearcher : IStepSearcher
{
	/// <summary>
	/// Indicates the maximum size the searcher can search for.
	/// </summary>
	int MaxSize { get; set; }
}