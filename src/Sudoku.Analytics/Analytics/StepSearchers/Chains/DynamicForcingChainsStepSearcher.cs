namespace Sudoku.Analytics.StepSearchers.Chains;

/// <summary>
/// Provides with a <b>Dynamic Forcing Chains</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Dynamic Cell Forcing Chains</item>
/// <item>Dynamic Region Forcing Chains (i.e. Dynamic House Forcing Chains)</item>
/// <item>Dynamic Contradiction Forcing Chains</item>
/// <item>Dynamic Double Forcing Chains</item>
/// </list>
/// </summary>
[StepSearcher(
	"StepSearcherName_DynamicForcingChainsStepSearcher",
	Technique.DynamicCellForcingChains, Technique.DynamicRegionForcingChains,
	Technique.DynamicContradictionForcingChains, Technique.DynamicDoubleForcingChains)]
public sealed partial class DynamicForcingChainsStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(ref StepAnalysisContext context)
	{
		var accumulator = new List<PatternBasedChainStep>();
		if (ChainingDriver.CollectDynamicForcingChainsCore(ref context, accumulator) is { } step)
		{
			return step;
		}

		if (!context.OnlyFindOne && accumulator.Count != 0)
		{
			context.Accumulator.AddRange(accumulator);
		}
		return null;
	}
}
