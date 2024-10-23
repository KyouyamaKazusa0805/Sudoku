namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Whip</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Whip</item>
/// </list>
/// </summary>
[StepSearcher("StepSearcherName_WhipStepSearcher", Technique.Whip)]
public sealed partial class WhipStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(ref StepAnalysisContext context)
	{
		return null;
	}
}
