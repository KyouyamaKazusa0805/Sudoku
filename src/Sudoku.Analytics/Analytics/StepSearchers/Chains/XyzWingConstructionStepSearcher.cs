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
	protected internal override unsafe Step? Collect(ref AnalysisContext context)
	{
		var accumulator = new List<NormalChainStep>();
		var baseRules = ChainingRule.ChainingLinkTypes.Aggregate(@delegate.EnumFlagMerger) | LinkType.XyzWing;
		if (ChainModule.CollectCore(ref context, accumulator, baseRules, &createStep, &filter) is { } step)
		{
			return step;
		}

		if (accumulator.Count != 0 && !context.OnlyFindOne)
		{
			StepMarshal.SortItems(accumulator);
			context.Accumulator.AddRange(accumulator);
		}
		return null;


		static bool filter(ChainPattern pattern)
		{
			var count = 0;
			foreach (var link in pattern.Links)
			{
				if (link.GroupedLinkPattern is XyzWing && ++count >= 2)
				{
					return false;
				}
			}
			return count == 1;
		}

		static XyzWingConstructionStep createStep(
			ChainPattern pattern,
			Conclusion[] conclusions,
			View[] views,
			StepSearcherOptions options
		) => new(
			conclusions,
			views,
			options,
			pattern,
			(XyzWing)pattern.Links.First(static l => l.GroupedLinkPattern is XyzWing).GroupedLinkPattern!
		);
	}
}
