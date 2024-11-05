namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a multiple forcing chains that is applied to a unique rectangle.
/// </summary>
/// <param name="cells">Indicates the pattern cells.</param>
/// <param name="urDigitsMask">Indicates the digits used in pattern.</param>
/// <param name="conclusions"><inheritdoc cref="MultipleForcingChains(Conclusion[])" path="/param[@name='conclusions']"/></param>
public sealed partial class RectangleForcingChains(
	[Property] Cell[] cells,
	[Property] Mask urDigitsMask,
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
	protected override ReadOnlySpan<ViewNode> GetInitialViewNodes(ref readonly Grid grid)
	{
		var result = new List<ViewNode>();
		foreach (var cell in Cells)
		{
			result.Add(new CellViewNode(ColorIdentifier.Rectangle1, cell));
			foreach (var digit in UrDigitsMask & grid.GetCandidates(cell))
			{
				result.Add(new CandidateViewNode(ColorIdentifier.Rectangle1, cell * 9 + digit));
			}
		}
		return result.AsSpan();
	}
}
