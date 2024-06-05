namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Represents a chaining rule on X rule (i.e. <see cref="LinkType.SingleDigit"/>).
/// </summary>
/// <seealso cref="LinkType.SingleDigit"/>
internal sealed class CachedXChainingRule : ChainingRule
{
	/// <inheritdoc/>
	public override void CollectStrongLinks(ref readonly Grid grid, LinkDictionary linkDictionary)
	{
		for (var digit = 0; digit < 9; digit++)
		{
			for (var house = 0; house < 27; house++)
			{
				if ((HousesMap[house] & CandidatesMap[digit]) is not { Count: not (0 or 1) } cellsInThisHouse)
				{
					continue;
				}

				var mask = Subview.ReduceCellByHouse(in cellsInThisHouse, house);
				if (PopCount((uint)mask) != 2)
				{
					continue;
				}

				var pos1 = TrailingZeroCount(mask);
				var pos2 = mask.GetNextSet(pos1);
				linkDictionary.AddEntry(
					new(HousesCells[house][pos1], digit, false),
					new(HousesCells[house][pos2], digit, true)
				);
			}
		}
	}

	/// <inheritdoc/>
	public override void CollectWeakLinks(ref readonly Grid grid, LinkDictionary linkDictionary)
	{
		for (var digit = 0; digit < 9; digit++)
		{
			for (var house = 0; house < 27; house++)
			{
				if ((HousesMap[house] & CandidatesMap[digit]) is not { Count: not (0 or 1) } cellsInThisHouse)
				{
					continue;
				}

				var mask = Subview.ReduceCellByHouse(in cellsInThisHouse, house);
				foreach (var combinationPair in mask.GetAllSets().GetSubsets(2))
				{
					linkDictionary.AddEntry(
						new(HousesCells[house][combinationPair[0]], digit, true),
						new(HousesCells[house][combinationPair[1]], digit, false)
					);
				}
			}
		}
	}
}
