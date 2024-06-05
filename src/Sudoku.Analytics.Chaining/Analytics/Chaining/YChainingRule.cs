namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Represents a chaining rule on Y rule (i.e. <see cref="LinkType.SingleCell"/>).
/// </summary>
/// <seealso cref="LinkType.SingleCell"/>
public sealed class YChainingRule : ChainingRule
{
	/// <inheritdoc/>
	public override void CollectStrongLinks(ref readonly Grid grid, LinkDictionary linkDictionary)
	{
		foreach (var cell in grid.BivalueCells)
		{
			var mask = grid.GetCandidates(cell);
			var digit1 = TrailingZeroCount(mask);
			var digit2 = mask.GetNextSet(digit1);
			linkDictionary.AddEntry(new(cell, digit1, false), new(cell, digit2, true));
		}
	}

	/// <inheritdoc/>
	public override void CollectWeakLinks(ref readonly Grid grid, LinkDictionary linkDictionary)
	{
		foreach (var cell in grid.EmptyCells)
		{
			var mask = grid.GetCandidates(cell);
			if (PopCount((uint)mask) < 2)
			{
				continue;
			}

			foreach (var combinationPair in mask.GetAllSets().GetSubsets(2))
			{
				linkDictionary.AddEntry(new(cell, combinationPair[0], true), new(cell, combinationPair[1], false));
			}
		}
	}
}
