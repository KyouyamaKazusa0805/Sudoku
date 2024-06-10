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
		var accumulator = new List<NormalChainStep>();
		var baseRules = LinkType.Unknown;
		foreach (var ruleKey in yieldLinkTypes())
		{
			baseRules |= ruleKey;

			if (ChainModule.CollectCore(ref context, accumulator, baseRules, RuleRouter) is { } step)
			{
				return step;
			}
		}

		if (accumulator.Count != 0 && !context.OnlyFindOne)
		{
			StepMarshal.SortItems(accumulator);
			context.Accumulator.AddRange(accumulator);
		}
		return null;


		static IEnumerable<LinkType> yieldLinkTypes()
		{
			yield return LinkType.SingleDigit;
			yield return LinkType.SingleCell;
		}
	}
}
