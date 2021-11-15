namespace Sudoku.Solving.Manual.Searchers.AlmostLockedSets;

/// <summary>
/// Defines a step searcher that searches for almost locked sets XY-Wing steps.
/// </summary>
public interface IAlmostLockedSetsXyWingStepSearcher : IStepSearcher
{
	/// <summary>
	/// Indicates whether two ALSes make an collision, which means they share the some same cells. 
	/// </summary>
	bool AllowCollision { get; set; }
}
