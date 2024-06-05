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
	Technique.SelfConstraint, Technique.NodeCollision,

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
	/// A valid chain can only belong to the following three cases:
	/// <list type="number">
	/// <item>
	/// <b>Discontinuous Nice Loop</b><br/>
	/// Start with weak link, alternating strong and weak links and return to itself by weak link
	/// (with an odd number of nodes).
	/// </item>
	/// <item>
	/// <b>Discontinuous Nice Loop</b><br/>
	/// Start with strong link, alternating strong and weak links and return to itself by strong link
	/// (with an odd number of nodes).
	/// </item>
	/// <item>
	/// <b>Continuous Nice Loop</b><br/>
	/// Start with strong link, alternating strong and weak links and return to itself by weak link
	/// (with an even number of nodes).
	/// </item>
	/// </list>
	/// </remarks>
	protected internal override Step? Collect(ref AnalysisContext context)
	{
		var supportedRules = from type in LinkTypes.GetAllFlags() select RuleRouter[type];
		var foundChains = ChainingDriver.CollectChainPatterns(in context.Grid, supportedRules);
		return null;
	}
}
