namespace Sudoku.Analytics.Construction.Chaining.Rules;

/// <summary>
/// Represents a chaining rule on X rule (i.e. <see cref="LinkType.SingleDigit"/>).
/// </summary>
/// <seealso cref="LinkType.SingleDigit"/>
internal sealed class CachedXChainingRule : ChainingRule
{
	/// <inheritdoc/>
	public override void GetLinks(ref ChainingRuleLinkContext context)
	{
		if (context.GetLinkOption(LinkType.SingleDigit) == LinkOption.None)
		{
			return;
		}

		// Strong.
		for (var digit = 0; digit < 9; digit++)
		{
			for (var house = 0; house < 27; house++)
			{
				if ((HousesMap[house] & CandidatesMap[digit]) is not { Count: not (0 or 1) } cellsInThisHouse)
				{
					continue;
				}

				var mask = cellsInThisHouse / house;
				if (Mask.PopCount(mask) != 2)
				{
					continue;
				}

				var pos1 = Mask.TrailingZeroCount(mask);
				var pos2 = mask.GetNextSet(pos1);
				var node1 = new Node((HousesCells[house][pos1] * 9 + digit).AsCandidateMap(), false, false);
				var node2 = new Node((HousesCells[house][pos2] * 9 + digit).AsCandidateMap(), true, false);
				context.StrongLinks.AddEntry(node1, node2);
			}
		}

		// Weak.
		for (var digit = 0; digit < 9; digit++)
		{
			for (var house = 0; house < 27; house++)
			{
				if ((HousesMap[house] & CandidatesMap[digit]) is not { Count: not (0 or 1) } cellsInThisHouse)
				{
					continue;
				}

				var mask = cellsInThisHouse / house;
				foreach (var combinationPair in mask.GetAllSets().GetSubsets(2))
				{
					var node1 = new Node((HousesCells[house][combinationPair[0]] * 9 + digit).AsCandidateMap(), true, false);
					var node2 = new Node((HousesCells[house][combinationPair[1]] * 9 + digit).AsCandidateMap(), false, false);
					context.WeakLinks.AddEntry(node1, node2);
				}
			}
		}
	}
}
