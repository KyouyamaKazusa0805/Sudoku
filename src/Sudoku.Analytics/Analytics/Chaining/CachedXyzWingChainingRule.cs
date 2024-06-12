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
			var patternCells = pivot.AsCellMap() + leafCell1 + leafCell2;
			foreach (ref readonly var pair in patternCells & 2)
			{
				if (!pair.CanSeeEachOther)
				{
					continue;
				}

				var cells1 = pair;
				var cells2 = patternCells & ~pair;
				var node1 = new Node(Subview.ExpandedCellFromDigit(in cells1, zDigit), false, true);
				var node2 = new Node(Subview.ExpandedCellFromDigit(in cells2, zDigit), true, true);
				strongLinks.AddEntry(node1, node2, true, pattern);

				foreach (var cells in cells1.PeerIntersection & CandidatesMap[zDigit] | 3)
				{
					if (!cells.IsInIntersection)
					{
						continue;
					}

					var node3 = new Node(Subview.ExpandedCellFromDigit(in cells1, zDigit), true, true);
					var node4 = new Node(Subview.ExpandedCellFromDigit(in cells, zDigit), false, true);
					weakLinks.AddEntry(node3, node4, false, pattern);
				}
				foreach (var cells in cells2.PeerIntersection & CandidatesMap[zDigit] | 3)
				{
					if (!cells.IsInIntersection)
					{
						continue;
					}

					var node3 = new Node(Subview.ExpandedCellFromDigit(in cells2, zDigit), true, true);
					var node4 = new Node(Subview.ExpandedCellFromDigit(in cells, zDigit), false, true);
					weakLinks.AddEntry(node3, node4, false, pattern);
				}
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
