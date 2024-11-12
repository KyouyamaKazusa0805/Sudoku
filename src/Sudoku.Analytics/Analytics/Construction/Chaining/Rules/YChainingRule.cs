namespace Sudoku.Analytics.Construction.Chaining.Rules;

/// <summary>
/// Represents a chaining rule on Y rule (i.e. <see cref="LinkType.SingleCell"/>).
/// </summary>
/// <seealso cref="LinkType.SingleCell"/>
public sealed class YChainingRule : ChainingRule
{
	/// <inheritdoc/>
	public override void GetLinks(ref ChainingRuleLinkContext context)
	{
		if (context.GetLinkOption(LinkType.SingleCell) == LinkOption.None)
		{
			return;
		}

		ref readonly var grid = ref context.Grid;

		// VARIABLE_DECLARATION_BEGIN
		_ = grid is { BivalueCells: var __BivalueCells, EmptyCells: var __EmptyCells };
		// VARIABLE_DECLARATION_END

		foreach (var cell in __EmptyCells)
		{
			var mask = grid.GetCandidates(cell);
			if (Mask.PopCount(mask) < 2)
			{
				continue;
			}

			if (__BivalueCells.Contains(cell))
			{
				var digit1 = Mask.TrailingZeroCount(mask);
				var digit2 = mask.GetNextSet(digit1);
				var node1 = new Node((cell * 9 + digit1).AsCandidateMap(), false);
				var node2 = new Node((cell * 9 + digit2).AsCandidateMap(), true);
				context.StrongLinks.AddEntry(node1, node2);
			}

			foreach (var combinationPair in mask.GetAllSets().GetSubsets(2))
			{
				var node1 = new Node((cell * 9 + combinationPair[0]).AsCandidateMap(), true);
				var node2 = new Node((cell * 9 + combinationPair[1]).AsCandidateMap(), false);
				context.WeakLinks.AddEntry(node1, node2);
			}
		}
	}
}
