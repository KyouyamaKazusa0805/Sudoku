namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher that searches for forcing chain steps.
/// </summary>
public interface IForcingChainStepSearcher : IChainStepSearcher
{
	/// <summary>
	/// Indicates whether the step searcher will search for nishio forcing chains.
	/// </summary>
	bool IsNishio { get; set; }

	/// <summary>
	/// Indicates whether the step searcher will search for multiple forcing chains.
	/// </summary>
	bool IsMultiple { get; set; }
}
