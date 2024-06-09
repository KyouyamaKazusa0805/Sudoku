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
			var node1 = new Node(cell, digit1, false, false);
			var node2 = new Node(cell, digit2, true, false);
			linkDictionary.AddEntry(node1, node2);
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
				var node1 = new Node(cell, combinationPair[0], true, false);
				var node2 = new Node(cell, combinationPair[1], false, false);
				linkDictionary.AddEntry(node1, node2);
			}
		}
	}
}
