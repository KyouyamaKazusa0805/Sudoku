namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Grouped Finned Chain</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Grouped Finned Chain</item>
/// </list>
/// </summary>
[StepSearcher("StepSearcherName_GroupedFinnedChainStepSearcher", Technique.FinnedGroupedChain)]
public sealed partial class GroupedFinnedChainStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(ref StepAnalysisContext context)
	{
		var accumulator = new List<ChainStep>();
		if (ChainModule.CollectMultipleCore(ref context, accumulator, true, true) is { } step)
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