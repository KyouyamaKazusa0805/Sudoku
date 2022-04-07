namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher that searches for almost locked sets XZ steps.
/// </summary>
public interface IAlmostLockedSetsXzStepSearcher : IAlmostLockedSetsStepSearcher
{
	/// <summary>
	/// Indicates whether two ALSes make an collision, which means they share the some same cells. 
	/// </summary>
	bool AllowCollision { get; set; }

	/// <summary>
	/// Indicates whether the searcher will enhance the searching to find all possible eliminations
	/// for looped-ALS eliminations.
	/// </summary>
	bool AllowLoopedPatterns { get; set; }
}
