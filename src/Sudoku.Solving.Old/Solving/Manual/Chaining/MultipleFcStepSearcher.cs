namespace Sudoku.Solving.Manual.Chaining;

/// <summary>
/// Encapsulates an <b>multiple forcing chains</b> (<b>Multiple FCs</b>) technique searcher.
/// </summary>
public sealed class MultipleFcStepSearcher : FcStepSearcher
{
	/// <summary>
	/// Initializes a <see cref="MultipleFcStepSearcher"/> instance.
	/// </summary>
	public MultipleFcStepSearcher() : base(false, true, false)
	{
	}
}
