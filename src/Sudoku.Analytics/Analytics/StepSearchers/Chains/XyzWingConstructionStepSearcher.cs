namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>XYZ-Wing Construction</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>XYZ-Wing Construction</item>
/// </list>
/// </summary>
[StepSearcher("StepSearcherName_XyzWingConstructionStepSearcher", Technique.XyzWingConstruction)]
public sealed partial class XyzWingConstructionStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(ref AnalysisContext context)
	{
		return null;
	}
}
