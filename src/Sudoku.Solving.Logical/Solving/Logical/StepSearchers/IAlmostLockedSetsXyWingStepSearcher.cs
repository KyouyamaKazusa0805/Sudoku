namespace Sudoku.Solving.Logical.StepSearchers;

/// <summary>
/// Provides with an <b>Almost Locked Sets XY-Wing</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Almost Locked Sets XY-Wing</item>
/// </list>
/// </summary>
public interface IAlmostLockedSetsXyWingStepSearcher : IStepSearcher
{
	/// <summary>
	/// Indicates whether two ALSes make an collision, which means they share the some same cells. 
	/// </summary>
	public abstract bool AllowCollision { get; set; }
}
