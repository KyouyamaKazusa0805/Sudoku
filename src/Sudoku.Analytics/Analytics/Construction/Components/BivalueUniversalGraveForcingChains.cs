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
	protected override ReadOnlySpan<ViewNode> GetInitialViewNodes()
		=> from candidate in TrueCandidates select (ViewNode)new CandidateViewNode(ColorIdentifier.Auxiliary1, candidate);
}
