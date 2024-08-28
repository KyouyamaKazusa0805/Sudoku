namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Remote Pair</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Remote Pair</item>
/// <item>Complex Remote Pair</item>
/// </list>
/// </summary>
[StepSearcher("StepSearcherName_RemotePairStepSearcher", Technique.RemotePair, Technique.ComplexRemotePair)]
public sealed partial class RemotePairStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(ref StepAnalysisContext context)
	{
		return null;
	}
}
