namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Represents a chaining rule on Y rule (i.e. <see cref="LinkType.LockedCandidates"/>).
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
				if ((HousesMap[house] & CandidatesMap[digit]) is not
					{
						Count: > 2,
						BlockMask: var blocks,
						RowMask: var rows,
						ColumnMask: var columns
					} cells)
				{
					continue;
				}

				_ = house.ToHouseType() switch
				{
					HouseType.Block when PopCount((uint)rows) >= 2 => getNodePair(rows << 9, in cells, digit),
					HouseType.Block when PopCount((uint)columns) >= 2 => getNodePair(columns << 18, in cells, digit),
					HouseType.Row or HouseType.Column when PopCount((uint)blocks) >= 2 => getNodePair(blocks, in cells, digit),
					_ => default
				};
			}
		}


		byte getNodePair(HouseMask houseMask, ref readonly CellMap cells, Digit digit)
		{
			foreach (var combination in houseMask.GetAllSets().GetSubsets(2))
			{
				var cells1 = cells & HousesMap[combination[0]];
				var cells2 = cells & HousesMap[combination[1]];
				var node1 = new Node(Subview.ExpandedCellFromDigit(in cells1, digit), true);
				var node2 = new Node(Subview.ExpandedCellFromDigit(in cells2, digit), false);
				linkDictionary.AddEntry(node1, node2);
			}
			return default;
		}
	}
}
