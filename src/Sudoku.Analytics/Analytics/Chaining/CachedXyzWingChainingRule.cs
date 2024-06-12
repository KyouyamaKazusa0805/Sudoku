namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Represents a chaining rule on XYZ-Wing rule (i.e. <see cref="LinkType.XyzWing"/>).
/// </summary>
/// <seealso cref="LinkType.XyzWing"/>
internal sealed class CachedXyzWingChainingRule : ChainingRule
{
	/// <inheritdoc/>
	internal override void CollectLinks(ref readonly Grid grid, LinkDictionary strongLinks, LinkDictionary weakLinks)
	{
		// Iterate on each XYZ-Wing pattern, to get strong links.
		foreach (var pattern in XyzWingPatternSearcher.Search(in grid))
		{
			var (pivot, leafCell1, leafCell2, _, _, _, zDigit) = pattern;
			var cells = pivot.AsCellMap() + leafCell1 + leafCell2;
			foreach (ref readonly var pair in cells & 2)
			{
				var node1 = new Node(Subview.ExpandedCellFromDigit(in pair, zDigit), false, true);
				var node2 = new Node(Subview.ExpandedCellFromDigit(cells & ~pair, zDigit), true, true);
				strongLinks.AddEntry(node1, node2, true, pattern);
			}
		}
	}

	/// <inheritdoc/>
	internal override void CollectExtraViewNodes(ref readonly Grid grid, ChainPattern pattern, ref View[] views)
	{
		var (view, id) = (views[0], ColorIdentifier.Normal);
		foreach (var link in pattern.Links)
		{
			if (link.GroupedLinkPattern is not XyzWing { Cells: var cells })
			{
				continue;
			}

			foreach (var cell in cells)
			{
				view.Add(new CellViewNode(id, cell));
			}
		}
	}
}
