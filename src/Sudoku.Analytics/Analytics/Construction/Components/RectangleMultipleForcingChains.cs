namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a multiple forcing chains that is applied to a unique rectangle.
/// </summary>
/// <param name="conclusions"><inheritdoc cref="MultipleForcingChains(Conclusion[])" path="/param[@name='conclusions']"/></param>
public sealed class RectangleMultipleForcingChains(params Conclusion[] conclusions) : MultipleForcingChains(conclusions)
{
	/// <inheritdoc/>
	public override bool IsCellMultiple => false;

	/// <inheritdoc/>
	public override bool IsHouseMultiple => false;

	/// <inheritdoc/>
	public override bool IsAdvancedMultiple => true;


	/// <inheritdoc/>
	protected override ReadOnlySpan<ViewNode> GetInitialViewNodes()
		=> from cell in Candidates.Cells select (ViewNode)new CellViewNode(ColorIdentifier.Normal, cell);
}
