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
		var accumulator = new List<BlossomLoopStep>();
		var elementary = ChainingRule.ElementaryLinkTypes.Aggregate(@delegate.EnumFlagMerger);
		var advanced = ChainingRule.AdvancedLinkTypes.Aggregate(@delegate.EnumFlagMerger);
		ref readonly var grid = ref context.Grid;
		InitializeLinks(in grid, elementary | advanced, context.Options, out var rules);
		if (ChainModule.CollectBlossomLoopCore(ref context, accumulator, rules) is { } step)
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
