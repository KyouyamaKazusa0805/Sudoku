namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Represents a chaining rule on AHS rule (i.e. <see cref="LinkType.AlmostHiddenSet"/>).
/// </summary>
/// <seealso cref="LinkType.AlmostHiddenSet"/>
internal sealed class CachedAlmostHiddenSetsChainingRule : ChainingRule
{
	/// <inheritdoc/>
	public override void CollectStrongLinks(ref readonly Grid grid, LinkDictionary linkDictionary)
	{
		foreach (var ahs in AlmostHiddenSetsModule.CollectAlmostHiddenSets(in grid))
		{
			var cells = ahs.Cells;
			var subsetDigitsMask = ahs.SubsetDigitsMask;
			var allCandidates = ahs.GetAllCandidates(in grid);
			foreach (var digitPair in subsetDigitsMask.GetAllSets().GetSubsets(2))
			{
				var digit1 = digitPair[0];
				var digit2 = digitPair[1];
				var cells1 = cells & CandidatesMap[digit1];
				var cells2 = cells & CandidatesMap[digit2];
				var node1 = new Node(Subview.ExpandedCellFromDigit(in cells1, digit1), false, in allCandidates);
				var node2 = new Node(Subview.ExpandedCellFromDigit(in cells2, digit2), true, in allCandidates);
				linkDictionary.AddEntry(node1, node2, true, ahs);
			}
		}
	}

	/// <inheritdoc/>
	public override void CollectWeakLinks(ref readonly Grid grid, LinkDictionary linkDictionary)
	{
		foreach (var ahs in AlmostHiddenSetsModule.CollectAlmostHiddenSets(in grid))
		{
			var weakLinkCandidates = ahs.CandidatesCanFormWeakLink;
			var allCandidates = ahs.GetAllCandidates(in grid);
			foreach (ref readonly var candidates1 in weakLinkCandidates | weakLinkCandidates.Count - 1)
			{
				var candidates2 = weakLinkCandidates & ~candidates1;
				var node1 = new Node(in candidates1, true, in allCandidates);
				var node2 = new Node(in candidates2, false, in allCandidates);
				linkDictionary.AddEntry(node1, node2, false, ahs);
			}
		}
	}

	/// <inheritdoc/>
	public override void CollectExtraViewNodes(ChainPattern pattern, ref View[] views)
	{
		var ahsIndex = 0;
		var view = views[0];
		foreach (var node in pattern)
		{
			if (node.ExtraMap is not { Count: not 0, Cells: var extraMapCells } extraMap)
			{
				continue;
			}

			var id = (ColorIdentifier)(ahsIndex + WellKnownColorIdentifierKind.AlmostLockedSet1);
			foreach (var alsCandidate in extraMap)
			{
				if (!pattern.Contains(alsCandidate))
				{
					view.Add(new CandidateViewNode(id, alsCandidate));
				}
			}
			foreach (var alsCell in extraMapCells)
			{
				view.Add(new CellViewNode(id, alsCell));
			}

			// Advance AHS color ID pointer.
			ahsIndex = (ahsIndex + 1) % 5;
		}
	}

	/// <inheritdoc/>
	public override ConclusionSet CollectLoopConclusions(Loop loop, ref readonly Grid grid)
	{
		// A valid AHS can be eliminated as a real hidden subset.
		var result = ConclusionSet.Empty;
		foreach (var element in loop.Links)
		{
			if (element is { IsStrong: false, GroupedLinkPattern: AlmostHiddenSet { Cells: var cells } ahs })
			{
				var allCandidates = ahs.GetAllCandidates(in grid);
				foreach (var cell in cells)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						if (!allCandidates.Contains(cell * 9 + digit))
						{
							result.Add(new Conclusion(Elimination, cell, digit));
						}
					}
				}
			}
		}
		return result;
	}
}
