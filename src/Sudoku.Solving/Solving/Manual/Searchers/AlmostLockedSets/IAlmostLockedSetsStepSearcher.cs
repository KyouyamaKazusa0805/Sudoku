namespace Sudoku.Solving.Manual.Searchers.AlmostLockedSets;

/// <summary>
/// Defines a step searcher that searches for almost locked sets steps.
/// </summary>
public interface IAlmostLockedSetsStepSearcher : IStepSearcher
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