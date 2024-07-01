namespace Sudoku.Analytics.Chaining.Rules;

/// <summary>
/// Represents a chaining rule on X rule (i.e. <see cref="LinkType.SingleDigit"/>).
/// </summary>
/// <seealso cref="LinkType.SingleDigit"/>
internal sealed class CachedXChainingRule : ChainingRule
{
	/// <inheritdoc/>
	protected internal override void CollectLinks(ref readonly ChainingRuleContext context)
	{
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
				if (PopCount((uint)mask) != 2)
				{
					continue;
				}

				var pos1 = TrailingZeroCount(mask);
				var pos2 = mask.GetNextSet(pos1);
				var node1 = new Node(HousesCells[house][pos1], digit, false, false);
				var node2 = new Node(HousesCells[house][pos2], digit, true, false);
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
					var node1 = new Node(HousesCells[house][combinationPair[0]], digit, true, false);
					var node2 = new Node(HousesCells[house][combinationPair[1]], digit, false, false);
					context.WeakLinks.AddEntry(node1, node2);
				}
			}
		}
	}
}
