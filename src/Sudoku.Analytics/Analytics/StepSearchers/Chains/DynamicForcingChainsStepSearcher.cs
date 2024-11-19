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
		// Test examples:
		// 9876.....65....8....4.......7.9..4......3..5......2..1..68..7......5...3.....1.2.:424 724 234 334 734 135 235 336 536 637 937 138 338 239 539 841 843 646 657 259 368 471 472 972 979 486 986 188 688 599

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
