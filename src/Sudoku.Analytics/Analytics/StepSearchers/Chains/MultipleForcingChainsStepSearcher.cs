namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Multiple Forcing Chains</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Cell Forcing Chains</item>
/// <item>Region Forcing Chains (i.e. House Forcing Chains)</item>
/// </list>
/// </summary>
[StepSearcher(
	"StepSearcherName_MultipleForcingChainsStepSearcher",
	Technique.CellForcingChains, Technique.RegionForcingChains)]
public sealed partial class MultipleForcingChainsStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(ref AnalysisContext context)
	{
		var accumulator = new List<MultipleForcingChainsStep>();
		var elementary = ChainingRule.ElementaryLinkTypes.Aggregate(@delegate.EnumFlagMerger);
		if (ChainModule.CollectMultipleCore(ref context, accumulator, elementary | LinkType.LockedCandidates) is { } step)
		{
			return step;
		}

		if (accumulator.Count != 0 && !context.OnlyFindOne)
		{
			StepMarshal.SortItems(accumulator);
			context.Accumulator.AddRange(accumulator);
		}
		return null;
	}
}
