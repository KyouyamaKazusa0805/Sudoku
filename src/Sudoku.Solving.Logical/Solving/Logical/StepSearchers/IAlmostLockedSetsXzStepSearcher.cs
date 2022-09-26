namespace Sudoku.Solving.Logical.StepSearchers;

/// <summary>
/// Provides with an <b>Almost Locked Sets XZ Rule</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Extended Subset Principle</item>
/// <item>Singly-linked Almost Locked Sets XZ Rule</item>
/// <item>Doubly-linked Almost Locked Sets XZ Rule</item>
/// </list>
/// </summary>
public interface IAlmostLockedSetsXzStepSearcher : IAlmostLockedSetsStepSearcher
{
	/// <summary>
	/// Indicates whether two ALSes make an collision, which means they share the some same cells. 
	/// </summary>
	public abstract bool AllowCollision { get; set; }

	/// <summary>
	/// Indicates whether the searcher will enhance the searching to find all possible eliminations
	/// for looped-ALS eliminations.
	/// </summary>
	public abstract bool AllowLoopedPatterns { get; set; }
}
