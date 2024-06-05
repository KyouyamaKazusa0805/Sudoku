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
[SplitStepSearcher(0, nameof(LinkTypes), LinkType.SingleDigit)]
[SplitStepSearcher(1, nameof(LinkTypes), LinkType.SingleDigit | LinkType.SingleCell)]
public sealed partial class ChainStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates the rule router.
	/// </summary>
	private static readonly Dictionary<LinkType, ChainingRule> RuleRouter = new()
	{
		{ LinkType.SingleDigit, new CachedXChainingRule() },
		{ LinkType.SingleCell, new CachedYChainingRule() }
	};


	/// <summary>
	/// Indicates the link types supported.
	/// </summary>
	public LinkType LinkTypes { get; init; }


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
		ref readonly var grid = ref context.Grid;
		var supportedRules = from type in LinkTypes.GetAllFlags() select RuleRouter[type];
		var foundChains = ChainingDriver.CollectChainPatterns(in context.Grid, supportedRules);
		foreach (var foundChain in foundChains)
		{
			var conclusions = foundChain.GetConclusions(in grid);
			var step = new NormalChainStep(
				[.. conclusions],
				[
					[
						.. GetCandidateNodes(foundChain),
						..
						from link in foundChain.Links
						let node1 = link.FirstNode
						let node2 = link.SecondNode
						select new ChainLinkViewNode(ColorIdentifier.Normal, in node1.Map, in node2.Map, link.IsStrong)
					]
				],
				context.Options,
				foundChain
			);
			if (context.OnlyFindOne)
			{
				return step;
			}

			context.Accumulator.Add(step);
		}

		return null;
	}

	/// <summary>
	/// Collects for <see cref="CandidateViewNode"/> instances from the specified <see cref="ChainPattern"/> instance.
	/// </summary>
	/// <param name="pattern">A <see cref="ChainPattern"/> instance.</param>
	/// <returns>The final node.</returns>
	private ReadOnlySpan<CandidateViewNode> GetCandidateNodes(ChainPattern pattern)
	{
		var result = new List<CandidateViewNode>();
		for (var i = 0; i < pattern.Length; i++)
		{
			var node = pattern[i];
			result.Add(new((i & 1) == 0 ? ColorIdentifier.Auxiliary1 : ColorIdentifier.Normal, node.Map[0]));
		}
		return result.AsReadOnlySpan();
	}
}
