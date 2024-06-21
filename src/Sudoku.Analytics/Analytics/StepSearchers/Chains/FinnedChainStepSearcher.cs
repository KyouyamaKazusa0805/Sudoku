namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Finned Chain</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Finned Chain</item>
/// <item>Grouped Finned Chain</item>
/// </list>
/// </summary>
[StepSearcher(
	"StepSearcherName_FinnedChainStepSearcher",
	Technique.FinnedChain, Technique.FinnedGroupedChain)]
public sealed partial class FinnedChainStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(ref AnalysisContext context)
	{
		var accumulator = new List<ChainStep>();
		var elementary = ChainingRule.ElementaryLinkTypes.Aggregate(@delegate.EnumFlagMerger);
		var advanced = ChainingRule.AdvancedLinkTypes.Aggregate(@delegate.EnumFlagMerger);
		ref readonly var grid = ref context.Grid;
		Initialize(in grid, elementary | advanced, LinkOption.House, LinkOption.House, out var rules);
		if (ChainModule.CollectMultipleCore(ref context, accumulator, rules, true) is { } step)
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
