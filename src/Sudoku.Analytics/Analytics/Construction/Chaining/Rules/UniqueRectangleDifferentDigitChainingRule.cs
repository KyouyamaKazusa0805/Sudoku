namespace Sudoku.Analytics.Construction.Chaining.Rules;

/// <summary>
/// Represents a chaining rule on AUR rule (i.e. <see cref="LinkType.UniqueRectangle_DifferentDigit"/>).
/// </summary>
/// <seealso cref="LinkType.UniqueRectangle_DifferentDigit"/>
public sealed class UniqueRectangleDifferentDigitChainingRule : UniqueRectangleChainingRule
{
	/// <inheritdoc/>
	public override void GetLinks(ref ChainingRuleLinkContext context)
	{
		if (context.GetLinkOption(LinkType.UniqueRectangle_DifferentDigit) == LinkOption.None)
		{
			return;
		}

		ref readonly var grid = ref context.Grid;
		if (grid.GetUniqueness() != Uniqueness.Unique)
		{
			return;
		}

		// VARIABLE_DECLARATION_BEGIN
		_ = grid is { EmptyCells: var __EmptyCells, CandidatesMap: var __CandidatesMap };
		// VARIABLE_DECLARATION_END

		var linkOption = context.GetLinkOption(LinkType.UniqueRectangle_DifferentDigit);
		foreach (var pattern in UniqueRectanglePattern.AllPatterns)
		{
			var urCells = pattern.AsCellMap();
			if ((__EmptyCells & urCells) != urCells)
			{
				// Four cells must be empty.
				continue;
			}

			var allDigitsMask = grid[urCells];
			foreach (var digitPair in grid[urCells].GetAllSets().GetSubsets(2))
			{
				var (d1, d2) = (digitPair[0], digitPair[1]);
				if (!UniqueRectanglePattern.CanMakeDeadlyPattern(in grid, d1, d2, pattern))
				{
					continue;
				}

				var urDigitsMask = (Mask)(1 << digitPair[0] | 1 << digitPair[1]);
				var otherDigitsMask = (Mask)(allDigitsMask & ~urDigitsMask);
				if (Mask.PopCount(otherDigitsMask) != 2)
				{
					continue;
				}

				var ur = new UniqueRectanglePattern(in urCells, urDigitsMask, otherDigitsMask);

				var theOtherDigit1 = Mask.TrailingZeroCount(otherDigitsMask);
				var theOtherDigit2 = otherDigitsMask.GetNextSet(theOtherDigit1);
				var cells1 = __CandidatesMap[theOtherDigit1] & urCells;
				var cells2 = __CandidatesMap[theOtherDigit2] & urCells;
				if (linkOption == LinkOption.Intersection && (cells1.IsInIntersection || cells2.IsInIntersection)
					|| linkOption != LinkOption.Intersection)
				{
					var node1 = new Node(cells1 * theOtherDigit1, false);
					var node2 = new Node(cells2 * theOtherDigit2, true);
					context.StrongLinks.AddEntry(node1, node2, true, ur);
				}
			}
		}
	}
}
