namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Represents a chaining rule on XYZ-Wing rule (i.e. <see cref="LinkType.XyzWing"/>).
/// </summary>
/// <seealso cref="LinkType.XyzWing"/>
internal sealed class CachedXyzWingChainingRule : ChainingRule
{
	/// <inheritdoc/>
	protected internal override void CollectLinks(
		ref readonly Grid grid,
		LinkDictionary strongLinks,
		LinkDictionary weakLinks,
		LinkOption linkOption,
		LinkOption alsLinkOption
	)
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
				if (linkOption == LinkOption.Intersection && !(cells1.IsInIntersection && cells2.IsInIntersection)
					|| linkOption == LinkOption.House && !(cells1.InOneHouse(out _) && cells2.InOneHouse(out _)))
				{
					goto CollectWeak;
				}

				// Strong.
				var node1 = new Node(cells1 * zDigit, false, true);
				var node2 = new Node(cells2 * zDigit, true, true);
				strongLinks.AddEntry(node1, node2, true, pattern);

			CollectWeak:
				// Weak.
				var possibleCells1 = cells1.PeerIntersection & CandidatesMap[zDigit];
				var possibleCells2 = cells2.PeerIntersection & CandidatesMap[zDigit];
				var (limit1, limit2) = linkOption switch
				{
					LinkOption.House => (Math.Min((EmptyCells & possibleCells1).Count, 9), Math.Min((EmptyCells & possibleCells2).Count, 9)),
					LinkOption.All => (possibleCells1.Count, possibleCells2.Count),
					_ => (3, 3)
				};
				foreach (ref readonly var cells in possibleCells1 | limit1)
				{
					if (linkOption == LinkOption.Intersection && !cells.IsInIntersection
						|| linkOption == LinkOption.House && !cells.InOneHouse(out _))
					{
						continue;
					}

					var node3 = new Node(cells1 * zDigit, true, true);
					var node4 = new Node(cells * zDigit, false, true);
					weakLinks.AddEntry(node3, node4, false, pattern);
				}
				foreach (ref readonly var cells in possibleCells2 | limit2)
				{
					if (linkOption == LinkOption.Intersection && !cells.IsInIntersection
						|| linkOption == LinkOption.House && !cells.InOneHouse(out _))
					{
						continue;
					}

					var node3 = new Node(cells2 * zDigit, true, true);
					var node4 = new Node(cells * zDigit, false, true);
					weakLinks.AddEntry(node3, node4, false, pattern);
				}
			}
		}
	}

	/// <inheritdoc/>
	protected internal override void CollectExtraViewNodes(ref readonly Grid grid, ChainOrLoop pattern, ref View[] views)
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
