namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a multiple forcing chains that is applied to a bi-value universal grave + n.
/// </summary>
/// <param name="trueCandidates">Indicates all true candidates.</param>
/// <param name="conclusions"><inheritdoc cref="MultipleForcingChains(Conclusion[])" path="/param[@name='conclusions']"/></param>
public sealed partial class BivalueUniversalGraveForcingChains(
	[Property] ref readonly CandidateMap trueCandidates,
	params Conclusion[] conclusions
) : MultipleForcingChains(conclusions)
{
	/// <inheritdoc/>
	public override bool IsCellMultiple => false;

	/// <inheritdoc/>
	public override bool IsHouseMultiple => false;

	/// <inheritdoc/>
	public override bool IsAdvancedMultiple => true;


	/// <inheritdoc/>
	protected internal override void PrepareFinnedChainViewNodes(
		NamedChain finnedChain,
		ref int cachedAlsIndex,
		ChainingRuleCollection supportedRules,
		ref readonly Grid grid,
		ref readonly CandidateMap fins,
		out View[] views
	)
	{
		base.PrepareFinnedChainViewNodes(finnedChain, ref cachedAlsIndex, supportedRules, in grid, in fins, out views);
		foreach (var candidate in Candidates)
		{
			var node = new CandidateViewNode(ColorIdentifier.Auxiliary1, candidate);
			foreach (var view in views)
			{
				view.Add(node);
			}
		}
	}

	/// <inheritdoc/>
	protected override ReadOnlySpan<ViewNode> GetInitialViewNodes(ref readonly Grid grid)
		=> from candidate in TrueCandidates select (ViewNode)new CandidateViewNode(ColorIdentifier.Auxiliary1, candidate);
}
