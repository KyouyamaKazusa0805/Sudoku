namespace Sudoku.Solving.Manual.Chaining;

/// <summary>
/// Encapsulates an <b>dynamic multiple forcing chains</b> (<b>Dynamic Multiple FCs</b>) technique searcher.
/// </summary>
public sealed class DynamicMultipleFcStepSearcher : FcStepSearcher
{
	/// <summary>
	/// Initializes a <see cref="DynamicMultipleFcStepSearcher"/> instance.
	/// </summary>
	public DynamicMultipleFcStepSearcher() : base(false, true, true)
	{
	}
}
