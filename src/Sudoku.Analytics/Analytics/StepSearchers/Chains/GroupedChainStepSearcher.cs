#define LOCKED_SET
#undef HIDDEN_SET // Requires large memory
#define UNIQUE_RECTANGLE
#define AVOIDABLE_RECTANGLE

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
public sealed partial class GroupedChainStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates the rule router.
	/// </summary>
	private static readonly Dictionary<LinkType, ChainingRule> RuleRouter = new()
	{
		{ LinkType.SingleDigit, new CachedXChainingRule() },
		{ LinkType.SingleCell, new CachedYChainingRule() },
		{ LinkType.LockedCandidates, new CachedLockedCandidatesChainingRule() },
#if LOCKED_SET
		{ LinkType.AlmostLockedSet, new CachedAlmostLockedSetsChainingRule() },
#endif
#if HIDDEN_SET
		{ LinkType.AlmostHiddenSet, new CachedAlmostHiddenSetsChainingRule() },
#endif
#if UNIQUE_RECTANGLE
		{ LinkType.AlmostUniqueRectangle, new CachedAlmostUniqueRectangleChainingRule() },
#endif
#if AVOIDABLE_RECTANGLE
		{ LinkType.AlmostAvoidableRectangle, new CachedAlmostAvoidableRectangleChainingRule() },
#endif
	};


	/// <inheritdoc/>
	protected internal override Step? Collect(ref AnalysisContext context)
	{
		var accumulator = new List<NormalChainStep>();
		var baseRules = LinkType.SingleDigit | LinkType.SingleCell;
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
			yield return LinkType.LockedCandidates;
#if LOCKED_SET
			yield return LinkType.AlmostLockedSet;
#endif
#if HIDDEN_SET
			yield return LinkType.AlmostHiddenSet;
#endif
#if UNIQUE_RECTANGLE
			yield return LinkType.AlmostUniqueRectangle;
#endif
#if AVOIDABLE_RECTANGLE
			yield return LinkType.AlmostAvoidableRectangle;
#endif
		}
	}
}
