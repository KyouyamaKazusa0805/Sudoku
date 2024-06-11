namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Represents a chaining rule on AHS rule (i.e. <see cref="LinkType.AlmostHiddenSet"/>).
/// </summary>
/// <seealso cref="LinkType.AlmostHiddenSet"/>
internal sealed class CachedAlmostHiddenSetsChainingRule : ChainingRule
{
	/// <inheritdoc/>
	protected internal override void CollectLinks(ref readonly Grid grid, LinkDictionary strongLinks, LinkDictionary weakLinks)
	{
		var ahses = AlmostHiddenSetsModule.CollectAlmostHiddenSets(in grid);

		// Strong.
		foreach (var ahs in ahses)
		{
			var cells = ahs.Cells;
			var subsetDigitsMask = ahs.SubsetDigitsMask;
			foreach (var digitPair in subsetDigitsMask.GetAllSets().GetSubsets(2))
			{
				var digit1 = digitPair[0];
				var digit2 = digitPair[1];
				var cells1 = cells & CandidatesMap[digit1];
				var cells2 = cells & CandidatesMap[digit2];
				var node1 = new Node(Subview.ExpandedCellFromDigit(in cells1, digit1), false, true);
				var node2 = new Node(Subview.ExpandedCellFromDigit(in cells2, digit2), true, true);
				strongLinks.AddEntry(node1, node2, true, ahs);
			}
		}

		// Weak.
		foreach (var ahs in ahses)
		{
			var weakLinkCandidates = ahs.CandidatesCanFormWeakLink;
			foreach (ref readonly var candidates1 in weakLinkCandidates | 3)
			{
				if (!candidates1.Cells.IsInIntersection)
				{
					continue;
				}

				foreach (var candidates2 in weakLinkCandidates & ~candidates1 | 3)
				{
					if (!candidates2.Cells.IsInIntersection)
					{
						continue;
					}

					if ((candidates1 | candidates2).Cells.Count == 1)
					{
						// The weak link cannot be inside one cell.
						continue;
					}

					var node1 = new Node(in candidates1, true, true);
					var node2 = new Node(in candidates2, false, true);
					weakLinks.AddEntry(node1, node2, false, ahs);
				}
			}
		}
	}

	/// <inheritdoc/>
	protected internal override void CollectExtraViewNodes(ref readonly Grid grid, ChainPattern pattern, ref View[] views)
	{
		var (ahsIndex, view) = (0, views[0]);
		foreach (var link in pattern.Links)
		{
			if (link.GroupedLinkPattern is not AlmostHiddenSet { Cells: var cells, SubsetDigitsMask: var subsetDigitsMask })
			{
				continue;
			}

			var id = (ColorIdentifier)(WellKnownColorIdentifierKind.AlmostLockedSet1 + ahsIndex);
			foreach (var cell in cells)
			{
				view.Add(new CellViewNode(id, cell));
				foreach (var digit in (Mask)(grid.GetCandidates(cell) & subsetDigitsMask))
				{
					view.Add(new CandidateViewNode(id, cell * 9 + digit));
				}
			}

			ahsIndex = (ahsIndex + 1) % 5;
		}
	}

	/// <inheritdoc/>
	protected internal override ConclusionSet CollectLoopConclusions(Loop loop, ref readonly Grid grid)
	{
		// A valid AHS can be eliminated as a real hidden subset.
		var result = ConclusionSet.Empty;
		foreach (var element in loop.Links)
		{
			if (element is not
				{
					IsStrong: false,
					FirstNode.Map: var firstNodeMap,
					SecondNode.Map: var secondNodeMap,
					GroupedLinkPattern: AlmostHiddenSet { Cells: var cells, SubsetDigitsMask: var subsetDigitsMask }
				})
			{
				continue;
			}

			var nodesMap = firstNodeMap | secondNodeMap;
			foreach (var cell in cells)
			{
				foreach (var digit in (Mask)(grid.GetCandidates(cell) & (Mask)~subsetDigitsMask))
				{
					var candidate = cell * 9 + digit;
					if (!nodesMap.Contains(candidate))
					{
						result.Add(new Conclusion(Elimination, candidate));
					}
				}
			}
		}
		return result;
	}
}
