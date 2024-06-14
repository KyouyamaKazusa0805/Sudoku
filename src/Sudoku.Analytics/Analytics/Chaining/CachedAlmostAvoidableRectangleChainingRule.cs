namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Represents a chaining rule on AAR rule (i.e. <see cref="LinkType.AlmostAvoidableRectangle"/>).
/// </summary>
/// <seealso cref="LinkType.AlmostAvoidableRectangle"/>
internal sealed class CachedAlmostAvoidableRectangleChainingRule : ChainingRule
{
	/// <inheritdoc/>
	protected internal override void CollectLinks(
		ref readonly Grid grid,
		LinkDictionary strongLinks,
		LinkDictionary weakLinks,
		LinkOption linkOption,
		LinkOption alsLinkOption
	)
	{
		// Weak.
		foreach (CellMap urCells in UniqueRectangleModule.PossiblePatterns)
		{
			var (modifiableCells, emptyCells) = (CellMap.Empty, CellMap.Empty);
			foreach (var cell in urCells)
			{
				if (grid.GetState(cell) == CellState.Modifiable)
				{
					modifiableCells.Add(cell);
				}
				else if (EmptyCells.Contains(cell))
				{
					emptyCells.Add(cell);
				}
			}
			if (modifiableCells.Count != 2 || emptyCells.Count != 2)
			{
				continue;
			}

			var digit1 = grid.GetDigit(modifiableCells[0]);
			var digit2 = grid.GetDigit(modifiableCells[1]);
			var digitsMask = (Mask)(1 << digit1 | 1 << digit2);
			if (modifiableCells.CanSeeEachOther)
			{
				var cells1 = emptyCells & CandidatesMap[digit1];
				var cells2 = emptyCells & CandidatesMap[digit2];
				if (!cells1 || !cells2)
				{
					continue;
				}

				var node1 = new Node(cells1 * digit1, true, true);
				var node2 = new Node(cells2 * digit2, false, true);
				var ar = new AvoidableRectangle(in urCells, digitsMask, in modifiableCells);
				weakLinks.AddEntry(node1, node2, false, ar);
			}
			else if (digit1 == digit2)
			{
				var digitsOtherCellsContained = (Mask)0;
				foreach (var digit in grid[in emptyCells])
				{
					if ((grid.GetCandidates(emptyCells[0]) >> digit & 1) != 0
						&& (grid.GetCandidates(emptyCells[1]) >> digit & 1) != 0)
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
					var node1 = new Node(emptyCells[0], digit, true, true);
					var node2 = new Node(emptyCells[1], digit, false, true);
					var ar = new AvoidableRectangle(in urCells, (Mask)(1 << digit1 | 1 << digit), urCells & ~emptyCells);
					weakLinks.AddEntry(node1, node2, false, ar);
				}
			}
		}
	}

	/// <inheritdoc/>
	protected internal override void CollectExtraViewNodes(ref readonly Grid grid, ChainOrLoop pattern, ref View[] views)
	{
		var (view, id) = (views[0], ColorIdentifier.Auxiliary3);
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
				view.Add(new CellViewNode(id, cell));
			}
		}
	}
}
