namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Represents a chaining rule on Y rule (i.e. <see cref="LinkType.SingleCell"/>).
/// </summary>
/// <seealso cref="LinkType.SingleCell"/>
internal sealed class CachedYChainingRule : ChainingRule
{
	/// <inheritdoc/>
	public override void CollectStrongLinks(ref readonly Grid grid, LinkDictionary linkDictionary)
	{
		foreach (var cell in BivalueCells)
		{
			var mask = grid.GetCandidates(cell);
			var digit1 = TrailingZeroCount(mask);
			var digit2 = mask.GetNextSet(digit1);
			linkDictionary.AddEntry(new(cell * 9 + digit1, false), new(cell * 9 + digit2, true));
		}
	}

	/// <inheritdoc/>
	public override void CollectWeakLinks(ref readonly Grid grid, LinkDictionary linkDictionary)
	{
		foreach (var cell in EmptyCells)
		{
			var mask = grid.GetCandidates(cell);
			if (PopCount((uint)mask) < 2)
			{
				continue;
			}

			foreach (var combinationPair in mask.GetAllSets().GetSubsets(2))
			{
				linkDictionary.AddEntry(new(cell * 9 + combinationPair[0], true), new(cell * 9 + combinationPair[1], false));
			}
		}
	}
}
