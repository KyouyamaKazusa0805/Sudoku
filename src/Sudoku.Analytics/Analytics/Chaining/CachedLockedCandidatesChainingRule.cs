namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Represents a chaining rule on locked candidates (i.e. <see cref="LinkType.LockedCandidates"/>).
/// </summary>
/// <seealso cref="LinkType.LockedCandidates"/>
internal sealed class CachedLockedCandidatesChainingRule : ChainingRule
{
	/// <inheritdoc/>
	public override void CollectStrongLinks(ref readonly Grid grid, LinkDictionary linkDictionary)
	{
		for (var house = 0; house < 27; house++)
		{
			for (var digit = 0; digit < 9; digit++)
			{
				var cells = HousesMap[house] & CandidatesMap[digit];
				if (!Grouped.IsGroupedStrongLink(in cells, house, out var pairHouse))
				{
					continue;
				}

				var firstPair = pairHouse[0];
				var h1 = TrailingZeroCount((uint)firstPair);
				var h2 = firstPair.GetNextSet(h1);
				var cells1 = cells & HousesMap[h1];
				var cells2 = cells & HousesMap[h2];
				var node1 = new Node(Subview.ExpandedCellFromDigit(in cells1, digit), false);
				var node2 = new Node(Subview.ExpandedCellFromDigit(in cells2, digit), true);
				linkDictionary.AddEntry(node1, node2);
			}
		}
	}

	/// <inheritdoc/>
	public override void CollectWeakLinks(ref readonly Grid grid, LinkDictionary linkDictionary)
	{
		for (var house = 0; house < 27; house++)
		{
			for (var digit = 0; digit < 9; digit++)
			{
				if ((HousesMap[house] & CandidatesMap[digit]) is not { Count: > 2 } cells)
				{
					continue;
				}

				foreach (var cells1 in cells | cells.Count)
				{
					var lastCellsMap = cells & ~cells1;
					foreach (var cells2 in lastCellsMap | lastCellsMap.Count)
					{
						if (cells1.Count == 1 && cells2.Count == 1)
						{
							// Skip for the case that both nodes only contain 1 cell.
							continue;
						}

						var node1 = new Node(Subview.ExpandedCellFromDigit(in cells1, digit), true);
						var node2 = new Node(Subview.ExpandedCellFromDigit(in cells2, digit), false);
						linkDictionary.AddEntry(node1, node2);
					}
				}
			}
		}
	}
}
