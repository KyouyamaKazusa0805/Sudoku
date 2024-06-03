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
	Technique.XChain, Technique.YChain, Technique.AlternatingInferenceChain, Technique.ContinuousNiceLoop, Technique.DiscontinuousNiceLoop,
	Technique.XyXChain, Technique.XyChain, Technique.FishyCycle, Technique.PurpleCow)]
[SplitStepSearcher(0, nameof(LinkTypes), LinkType.SingleDigit)]
[SplitStepSearcher(1, nameof(LinkTypes), LinkType.SingleDigit | LinkType.SingleCell)]
public sealed partial class ChainStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates the backing chaining rule router instance.
	/// </summary>
	private static readonly Dictionary<LinkType, ChainingRule> ChainingRuleRouter = new()
	{
		{ LinkType.SingleDigit, new XChainingRule() },
		{ LinkType.SingleCell, new YChainingRule() }
	};


	/// <summary>
	/// Indicates the link types supported.
	/// </summary>
	public LinkType LinkTypes { get; init; }


	/// <inheritdoc/>
	protected internal override Step? Collect(ref AnalysisContext context)
	{
		// Step 1: Collect for all strong and weak links appeared in the grid.
		ref readonly var grid = ref context.Grid;
		var (strongLinks, weakLinks) = (CreateStrong(in grid, LinkTypes), CreateWeak(in grid, LinkTypes));

		// Step 2: Iterate on dictionary to get chains.
		// TODO: Implement (Consider using DFS instead).
		return null;
	}


	/// <summary>
	/// Creates a <see cref="LinkDictionary"/> instance that holds a list of strong link relations.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="linkTypes">The link types to be checked.</param>
	/// <returns>A <see cref="LinkDictionary"/> as result.</returns>
	private static LinkDictionary CreateStrong(ref readonly Grid grid, LinkType linkTypes)
	{
		var result = new LinkDictionary();
		foreach (var linkType in linkTypes)
		{
			ChainingRuleRouter[linkType].CollectStrongLinks(in grid, result);
		}
		return result;
	}

	/// <summary>
	/// Creates a <see cref="LinkDictionary"/> instance that holds a list of weak link relations.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="linkTypes">The link types to be checked.</param>
	/// <returns>A <see cref="LinkDictionary"/> as result.</returns>
	private static LinkDictionary CreateWeak(ref readonly Grid grid, LinkType linkTypes)
	{
		var result = new LinkDictionary();
		foreach (var linkType in linkTypes)
		{
			ChainingRuleRouter[linkType].CollectWeakLinks(in grid, result);
		}
		return result;
	}
}
