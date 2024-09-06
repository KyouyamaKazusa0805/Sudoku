namespace Sudoku.Runtime.ChainingServices.Rules;

/// <summary>
/// Represents a chaining rule on Y rule (i.e. <see cref="LinkType.SingleCell"/>).
/// </summary>
/// <seealso cref="LinkType.SingleCell"/>
internal sealed class CachedYChainingRule : ChainingRule
{
	/// <inheritdoc/>
	public override void GetLinks(ref ChainingRuleLinkContext context)
	{
		if (context.GetLinkOption(LinkType.SingleCell) == LinkOption.None)
		{
			return;
		}

		ref readonly var grid = ref context.Grid;
		foreach (var cell in EmptyCells)
		{
			var mask = grid.GetCandidates(cell);
			if (Mask.PopCount(mask) < 2)
			{
				continue;
			}

			if (BivalueCells.Contains(cell))
			{
				var digit1 = Mask.TrailingZeroCount(mask);
				var digit2 = mask.GetNextSet(digit1);
				var node1 = new Node((cell * 9 + digit1).AsCandidateMap(), false, false);
				var node2 = new Node((cell * 9 + digit2).AsCandidateMap(), true, false);
				context.StrongLinks.AddEntry(node1, node2);
			}

			foreach (var combinationPair in mask.GetAllSets().GetSubsets(2))
			{
				var node1 = new Node((cell * 9 + combinationPair[0]).AsCandidateMap(), true, false);
				var node2 = new Node((cell * 9 + combinationPair[1]).AsCandidateMap(), false, false);
				context.WeakLinks.AddEntry(node1, node2);
			}
		}
	}
}
