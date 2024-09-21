namespace Sudoku.Analytics.Patterning.Chaining.Rules;

/// <summary>
/// Represents a chaining rule on locked candidates (i.e. <see cref="LinkType.LockedCandidates"/>).
/// </summary>
/// <seealso cref="LinkType.LockedCandidates"/>
internal sealed class CachedLockedCandidatesChainingRule : ChainingRule
{
	/// <inheritdoc/>
	public override void GetLinks(ref ChainingRuleLinkContext context)
	{
		if (context.GetLinkOption(LinkType.LockedCandidates) == LinkOption.None)
		{
			return;
		}

		// Strong.
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
				var h1 = HouseMask.TrailingZeroCount(firstPair);
				var h2 = firstPair.GetNextSet(h1);
				var cells1 = cells & HousesMap[h1];
				var cells2 = cells & HousesMap[h2];
				var node1 = new Node(cells1 * digit, false, false);
				var node2 = new Node(cells2 * digit, true, false);
				context.StrongLinks.AddEntry(node1, node2);
			}
		}

		// Weak.
		for (var house = 0; house < 27; house++)
		{
			for (var digit = 0; digit < 9; digit++)
			{
				if ((HousesMap[house] & CandidatesMap[digit]) is not { Count: > 2 } cells)
				{
					continue;
				}

				foreach (ref readonly var cells1 in cells | 3)
				{
					if (!cells1.IsInIntersection)
					{
						continue;
					}

					var lastCellsMap = cells & ~cells1;
					foreach (ref readonly var cells2 in lastCellsMap | 3)
					{
						if (cells1.Count == 1 && cells2.Count == 1 || !cells2.IsInIntersection)
						{
							continue;
						}

						var node1 = new Node(cells1 * digit, true, false);
						var node2 = new Node(cells2 * digit, false, false);
						context.WeakLinks.AddEntry(node1, node2);
					}
				}
			}
		}
	}
}