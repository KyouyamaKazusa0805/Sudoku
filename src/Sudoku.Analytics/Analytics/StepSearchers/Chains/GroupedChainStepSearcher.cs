#define LOCKED_CANDIDATES
#define ALMOST_LOCKED_SET
#undef ALMOST_HIDDEN_SET // Can only enable in big memory :(

namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Grouped Chain</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Grouped Alternating Inference Chains</item>
/// <item>Grouped Continuous Nice Loops</item>
/// </list>
/// </summary>
[StepSearcher(
	"StepSearcherName_GroupedChainStepSearcher",
	// 3-Chains
	Technique.GroupedSkyscraper, Technique.GroupedTwoStringKite, Technique.GroupedTurbotFish,

	// 5-Chains
	Technique.GroupedWWing, Technique.GroupedMWing,
	Technique.GroupedSWing, Technique.GroupedLWing, Technique.GroupedHWing,
	Technique.GroupedPurpleCow,

	// Chains
	Technique.GroupedXChain, Technique.XyChain, Technique.GroupedXyXChain,
	Technique.GroupedAlternatingInferenceChain, Technique.GroupedDiscontinuousNiceLoop,

	// Overlappings
	Technique.GroupedSelfConstraint, Technique.NodeCollision,

	// Loops
	Technique.GroupedContinuousNiceLoop, Technique.GroupedXyCycle, Technique.GroupedFishyCycle)]
#if LOCKED_CANDIDATES
[SplitStepSearcher(0, nameof(LinkTypes), LinkType.NonGrouped | LinkType.LockedCandidates)]
#endif
#if ALMOST_LOCKED_SET
[SplitStepSearcher(1, nameof(LinkTypes), LinkType.NonGrouped | LinkType.LockedCandidates | LinkType.AlmostLockedSet)]
#endif
#if ALMOST_HIDDEN_SET
[SplitStepSearcher(2, nameof(LinkTypes), LinkType.NonGrouped | LinkType.LockedCandidates | LinkType.AlmostLockedSet | LinkType.AlmostHiddenSet)]
#endif
public sealed partial class GroupedChainStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates the rule router.
	/// </summary>
	private static readonly Dictionary<LinkType, ChainingRule> RuleRouter = new()
	{
		{ LinkType.SingleDigit, new CachedXChainingRule() },
		{ LinkType.SingleCell, new CachedYChainingRule() },
#if LOCKED_CANDIDATES
		{ LinkType.LockedCandidates, new CachedLockedCandidatesChainingRule() },
#endif
#if ALMOST_LOCKED_SET
		{ LinkType.AlmostLockedSet, new CachedAlmostLockedSetsChainingRule() },
#endif
#if ALMOST_HIDDEN_SET
		{ LinkType.AlmostHiddenSet, new CachedAlmostHiddenSetsChainingRule() },
#endif
	};


	/// <summary>
	/// Indicates the link types supported.
	/// </summary>
	public LinkType LinkTypes { get; init; }


	/// <inheritdoc/>
	protected internal override Step? Collect(ref AnalysisContext context) => ChainModule.CollectCore(ref context, LinkTypes, RuleRouter);
}
