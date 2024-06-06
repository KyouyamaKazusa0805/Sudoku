namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Represents a chaining rule on ALS rule (i.e. <see cref="LinkType.AlmostLockedSet"/>).
/// </summary>
/// <seealso cref="LinkType.AlmostLockedSet"/>
internal sealed class CachedAlmostLockedSetsChainingRule : ChainingRule
{
	/// <inheritdoc/>
	public override void CollectStrongLinks(ref readonly Grid grid, LinkDictionary linkDictionary)
	{
		// Collect ALSes appeared in the current grid.
		foreach (var als in AlmostLockedSetsModule.CollectAlmostLockedSets(in grid))
		{
			if (als is not
				{
					IsBivalueCell: false,
					Cells: var cells,
					StrongLinks: { Length: >= 2 } strongLinks,
					House: var house
				})
			{
				// This ALS is special case - it only uses 2 digits in a cell.
				// This will be handled as a normal bi-value strong link (Y rule).
				continue;
			}

			foreach (var digitsPair in strongLinks)
			{
				var node1ExtraMap = CandidateMap.Empty;
				foreach (var cell in cells)
				{
					node1ExtraMap.AddRange(from digit in grid.GetCandidates(cell) select cell * 9 + digit);
				}
				var node2ExtraMap = CandidateMap.Empty;
				foreach (var cell in cells)
				{
					node2ExtraMap.AddRange(from digit in grid.GetCandidates(cell) select cell * 9 + digit);
				}

				var digit1 = TrailingZeroCount(digitsPair);
				var digit2 = digitsPair.GetNextSet(digit1);
				var node1Cells = HousesMap[house] & CandidatesMap[digit1];
				var node2Cells = HousesMap[house] & CandidatesMap[digit2];
				var node1 = new Node(Subview.ExpandedCellFromDigit(in node1Cells, digit1), false, in node1ExtraMap);
				var node2 = new Node(Subview.ExpandedCellFromDigit(in node2Cells, digit2), true, in node2ExtraMap);
				linkDictionary.AddEntry(node1, node2, als);
			}
		}
	}

	/// <inheritdoc/>
	public override void CollectWeakLinks(ref readonly Grid grid, LinkDictionary linkDictionary)
	{
		// A valid ALS doesn't contain any weak links.
	}

	/// <inheritdoc/>
	public override void CollectExtraViewNodes(ChainPattern pattern, ref View[] views)
	{
		var alsIndex = 0;
		var view = views[0];
		foreach (var node in pattern)
		{
			if (node.ExtraMap is not { Count: not 0, Cells: var extraMapCells } extraMap)
			{
				continue;
			}

			var id = (ColorIdentifier)(alsIndex + WellKnownColorIdentifierKind.AlmostLockedSet1);
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

			// Advance ALS color ID pointer.
			alsIndex = (alsIndex + 1) % 5;
		}
	}

	/// <inheritdoc/>
	public override ConclusionSet CollectLoopConclusions(Loop loop, ref readonly Grid grid)
	{
		// A valid ALS can be eliminated as a real naked subset.
		var result = ConclusionSet.Empty;
		foreach (var element in loop.Links)
		{
			if (element is
				{
					IsStrong: false, // The strong links may not contain extra eliminations.
					GroupedLinkPattern: AlmostLockedSet { House: var alsHouse, Cells: var alsCells, DigitsMask: var digitsMask }
				})
			{
				foreach (var cell in HousesMap[alsHouse] & EmptyCells & ~alsCells)
				{
					foreach (var digit in (Mask)(grid.GetCandidates(cell) & digitsMask))
					{
						result.Add(new Conclusion(Elimination, cell, digit));
					}
				}
			}
		}
		return result;
	}
}
