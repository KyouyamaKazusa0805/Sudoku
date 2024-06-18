namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Chain</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Alternating Inference Chains</item>
/// <item>Continuous Nice Loops</item>
/// </list>
/// </summary>
[StepSearcher(
	"StepSearcherName_ChainStepSearcher",
	// 3-Chains
	Technique.Skyscraper, Technique.TwoStringKite, Technique.TurbotFish,

	// 5-Chains
	Technique.WWing, Technique.MWing, Technique.SWing, Technique.LWing, Technique.HWing, Technique.PurpleCow,

	// Chains
	Technique.XChain, Technique.XyChain, Technique.XyXChain,
	Technique.AlternatingInferenceChain, Technique.DiscontinuousNiceLoop,

	// Overlappings
	Technique.SelfConstraint,

	// Loops
	Technique.ContinuousNiceLoop, Technique.XyCycle, Technique.FishyCycle)]
public sealed partial class ChainStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="/g/developer-notes" />
	/// <para>
	/// All implementations are extracted in type <see cref="ChainingDriver"/>. Please visit it to learn more information.
	/// </para>
	/// </remarks>
	/// <seealso cref="ChainingDriver"/>
	protected internal override Step? Collect(ref AnalysisContext context)
	{
		var accumulator = new List<NormalChainStep>();
		ref readonly var grid = ref context.Grid;
		var linkTypes = ChainingRule.ElementaryLinkTypes.Aggregate(@delegate.EnumFlagMerger);
		LinkPool.Initialize(in grid, linkTypes, LinkOption.House, LinkOption.House, out var rules);
		if (ChainModule.CollectCore(ref context, accumulator, rules) is { } step)
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
