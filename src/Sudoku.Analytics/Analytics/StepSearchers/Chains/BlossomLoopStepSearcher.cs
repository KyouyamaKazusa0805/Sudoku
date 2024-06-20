namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Blossom Loop</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Blossom Loop</item>
/// </list>
/// </summary>
[StepSearcher("StepSearcherName_BlossomLoopStepSearcher", Technique.BlossomLoop)]
public sealed partial class BlossomLoopStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(ref AnalysisContext context)
	{
		return null;
	}
}
