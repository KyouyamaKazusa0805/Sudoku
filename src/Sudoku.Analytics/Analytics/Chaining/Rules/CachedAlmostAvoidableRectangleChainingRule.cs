namespace Sudoku.Analytics.Chaining.Rules;

/// <summary>
/// Represents a chaining rule on AAR rule (i.e. <see cref="LinkType.AlmostAvoidableRectangle"/>).
/// </summary>
/// <seealso cref="LinkType.AlmostAvoidableRectangle"/>
internal sealed class CachedAlmostAvoidableRectangleChainingRule : ChainingRule
{
	/// <inheritdoc/>
	protected internal override void CollectLinks(ref readonly ChainingRuleLinkCollectingContext context)
	{
		ref readonly var grid = ref context.Grid;

		// Weak.
		foreach (CellMap urCells in UniqueRectangleModule.PossiblePatterns)
		{
			var (modifiableCellsInPattern, emptyCellsInPattern, isValid) = (CellMap.Empty, CellMap.Empty, true);
			foreach (var cell in urCells)
			{
				switch (grid.GetState(cell))
				{
					case CellState.Modifiable:
					{
						modifiableCellsInPattern.Add(cell);
						break;
					}
					case CellState.Given:
					{
						isValid = false;
						goto OutsideValidityCheck;
					}
					default:
					{
						emptyCellsInPattern.Add(cell);
						break;
					}
				}
			}
		OutsideValidityCheck:
			if (!isValid || modifiableCellsInPattern.Count != 2 || emptyCellsInPattern.Count != 2)
			{
				continue;
			}

			var digit1 = grid.GetDigit(modifiableCellsInPattern[0]);
			var digit2 = grid.GetDigit(modifiableCellsInPattern[1]);
			var digitsMask = (Mask)(1 << digit1 | 1 << digit2);
			if (modifiableCellsInPattern.CanSeeEachOther)
			{
				var cells1 = emptyCellsInPattern & CandidatesMap[digit1];
				var cells2 = emptyCellsInPattern & CandidatesMap[digit2];
				if (!cells1 || !cells2)
				{
					continue;
				}

				var node1 = new Node(cells1 * digit1, true, true);
				var node2 = new Node(cells2 * digit2, false, true);
				var ar = new AvoidableRectangle(in urCells, digitsMask, in modifiableCellsInPattern);
				context.WeakLinks.AddEntry(node1, node2, false, ar);
			}
			else if (digit1 == digit2)
			{
				var digitsOtherCellsContained = (Mask)0;
				foreach (var digit in grid[in emptyCellsInPattern])
				{
					if ((grid.GetCandidates(emptyCellsInPattern[0]) >> digit & 1) != 0
						&& (grid.GetCandidates(emptyCellsInPattern[1]) >> digit & 1) != 0)
					{
						digitsOtherCellsContained |= (Mask)(1 << digit);
					}
				}
				if (digitsOtherCellsContained == 0)
				{
					continue;
				}

				foreach (var digit in digitsOtherCellsContained)
				{
					var node1 = new Node(emptyCellsInPattern[0], digit, true, true);
					var node2 = new Node(emptyCellsInPattern[1], digit, false, true);
					var ar = new AvoidableRectangle(in urCells, (Mask)(1 << digit1 | 1 << digit), urCells & ~emptyCellsInPattern);
					context.WeakLinks.AddEntry(node1, node2, false, ar);
				}
			}
		}
	}

	/// <inheritdoc/>
	protected internal override void MapViewNodes(ref ChainingRuleViewNodesMappingContext context)
	{
		ref readonly var grid = ref context.Grid;
		var pattern = context.Pattern;
		var view = context.View;
		var result = new List<ViewNode>();
		foreach (var link in pattern.Links)
		{
			if (link.GroupedLinkPattern is not AvoidableRectangle { Cells: var urCells })
			{
				continue;
			}

			foreach (var cell in urCells)
			{
				if (view.FindCell(cell) is { } cellViewNode)
				{
					view.Remove(cellViewNode);
				}

				var node = new CellViewNode(ColorIdentifier.Auxiliary3, cell);
				view.Add(node);
				result.Add(node);
			}
		}
		context.ProducedViewNodes = result.AsReadOnlySpan();
	}
}
