#define LIMIT_STRONG_LINK_NODE_IN_INTERSECTION

namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Represents a chaining rule on AUR rule (i.e. <see cref="LinkType.AlmostUniqueRectangle"/>).
/// </summary>
/// <seealso cref="LinkType.AlmostUniqueRectangle"/>
internal sealed class CachedAlmostUniqueRectangleChainingRule : ChainingRule
{
	/// <inheritdoc/>
	public override void CollectStrongLinks(ref readonly Grid grid, LinkDictionary linkDictionary)
	{
		foreach (CellMap urCells in UniqueRectangleModule.PossiblePatterns)
		{
			if ((EmptyCells & urCells) != urCells)
			{
				// Four cells must be empty.
				continue;
			}

			// Collect valid digits.
			// A valid UR digit can be filled inside a UR pattern twice,
			// meaning the placement of this digit must be diagonally arranged to make it. For example,
			//   x  .
			//   .  x
			// can be filled 2 times. However, pattern
			//   .  x
			//   .  x
			// cannot.
			// Just check for spanned rows and columns, determining whether both the numbers of spanned rows and columns are 2.
			var validDigitsMask = (Mask)0;
			var allDigitsMask = grid[in urCells];
			foreach (var digit in allDigitsMask)
			{
				var cellsToFillDigit = CandidatesMap[digit] & urCells;
				if (PopCount((uint)cellsToFillDigit.RowMask) == 2 && PopCount((uint)cellsToFillDigit.ColumnMask) == 2)
				{
					validDigitsMask |= (Mask)(1 << digit);
				}
			}
			if (PopCount((uint)validDigitsMask) < 2)
			{
				// Not enough digits to form a UR.
				continue;
			}

			foreach (var digitPair in validDigitsMask.GetAllSets().GetSubsets(2))
			{
				var (digit1, digit2) = (digitPair[0], digitPair[1]);
				var urDigitsMask = (Mask)(1 << digit1 | 1 << digit2);
				var otherDigitsMask = (Mask)(allDigitsMask & (Mask)~urDigitsMask);
				var ur = new UniqueRectangle(in urCells, urDigitsMask, otherDigitsMask);
				var urCandidatesMap = CandidateMap.Empty;
				foreach (var cell in urCells)
				{
					foreach (var digit in digitPair)
					{
						if ((grid.GetCandidates(cell) >> digit & 1) != 0)
						{
							urCandidatesMap.Add(cell * 9 + digit);
						}
					}
				}

				switch (PopCount((uint)otherDigitsMask))
				{
					case 1:
					{
						// Split the digit into two parts.
						var otherOnlyDigit = Log2((uint)otherDigitsMask);
						var cellsContainingThisDigit = CandidatesMap[otherOnlyDigit] & urCells;

						var rowsSpanned = cellsContainingThisDigit.RowMask << 9;
						var row1 = TrailingZeroCount(rowsSpanned);
						var row2 = rowsSpanned.GetNextSet(row1);
						var cells1 = cellsContainingThisDigit & HousesMap[row1];
						var cells2 = cellsContainingThisDigit & HousesMap[row2];
#if LIMIT_STRONG_LINK_NODE_IN_INTERSECTION
						if (cells1.IsInIntersection && cells2.IsInIntersection)
#endif
						{
							var node1 = new Node(Subview.ExpandedCellFromDigit(in cells1, otherOnlyDigit), false, in urCandidatesMap);
							var node2 = new Node(Subview.ExpandedCellFromDigit(in cells2, otherOnlyDigit), true, in urCandidatesMap);
							linkDictionary.AddEntry(node1, node2, true, ur);
						}
						var columnsSpanned = cellsContainingThisDigit.ColumnMask << 18;
						var column1 = TrailingZeroCount(columnsSpanned);
						var column2 = columnsSpanned.GetNextSet(column1);
						var cells3 = cellsContainingThisDigit & HousesMap[column1];
						var cells4 = cellsContainingThisDigit & HousesMap[column2];
#if LIMIT_STRONG_LINK_NODE_IN_INTERSECTION
						if (cells3.IsInIntersection && cells4.IsInIntersection)
#endif
						{
							var node3 = new Node(Subview.ExpandedCellFromDigit(in cells3, otherOnlyDigit), false, in urCandidatesMap);
							var node4 = new Node(Subview.ExpandedCellFromDigit(in cells4, otherOnlyDigit), true, in urCandidatesMap);
							linkDictionary.AddEntry(node3, node4, false, ur);
						}
						break;
					}
					case 2:
					{
						var theOtherDigit1 = TrailingZeroCount(otherDigitsMask);
						var theOtherDigit2 = otherDigitsMask.GetNextSet(theOtherDigit1);
						var cells1 = CandidatesMap[theOtherDigit1] & urCells;
						var cells2 = CandidatesMap[theOtherDigit2] & urCells;
#if LIMIT_STRONG_LINK_NODE_IN_INTERSECTION
						if (cells1.IsInIntersection && cells2.IsInIntersection)
#endif
						{
							var node1 = new Node(Subview.ExpandedCellFromDigit(in cells1, theOtherDigit1), false, in urCandidatesMap);
							var node2 = new Node(Subview.ExpandedCellFromDigit(in cells2, theOtherDigit2), true, in urCandidatesMap);
							linkDictionary.AddEntry(node1, node2, true, ur);
						}
						break;
					}
					default:
					{
						continue;
					}
				}
			}
		}
	}

	/// <inheritdoc/>
	public override void CollectWeakLinks(ref readonly Grid grid, LinkDictionary linkDictionary)
	{
#if LIMIT_WEAK_LINK_NODE_IN_INTERSECTION
		foreach (CellMap urCells in UniqueRectangleModule.PossiblePatterns)
		{
			if ((EmptyCells & urCells) != urCells)
			{
				// Four cells must be empty.
				continue;
			}

			// Collect valid digits.
			var validDigitsMask = (Mask)0;
			var allDigitsMask = grid[in urCells];
			foreach (var digit in allDigitsMask)
			{
				var cellsToFillDigit = CandidatesMap[digit] & urCells;
				if (PopCount((uint)cellsToFillDigit.RowMask) == 2 && PopCount((uint)cellsToFillDigit.ColumnMask) == 2)
				{
					validDigitsMask |= (Mask)(1 << digit);
				}
			}
			if (PopCount((uint)validDigitsMask) < 2)
			{
				// No enough digits to form a UR.
				continue;
			}

			foreach (var digitPair in validDigitsMask.GetAllSets().GetSubsets(2))
			{
				var (digit1, digit2) = (digitPair[0], digitPair[1]);
				var urDigitsMask = (Mask)(1 << digit1 | 1 << digit2);
				var otherDigitsMask = (Mask)(allDigitsMask & (Mask)~urDigitsMask);
				if (PopCount((uint)otherDigitsMask) < 2)
				{
					continue;
				}

				var urCandidatesMap = CandidateMap.Empty;
				foreach (var cell in urCells)
				{
					foreach (var digit in digitPair)
					{
						if ((grid.GetCandidates(cell) >> digit & 1) != 0)
						{
							urCandidatesMap.Add(cell * 9 + digit);
						}
					}
				}

				var ur = new UniqueRectangle(in urCells, urDigitsMask, otherDigitsMask);
				switch (PopCount((uint)otherDigitsMask))
				{
					case 1:
					{
						// Split the digit into two parts.
						var otherOnlyDigit = Log2((uint)otherDigitsMask);
						var cellsContainingThisDigit = CandidatesMap[otherOnlyDigit] & urCells;
						var cellsEffected = cellsContainingThisDigit.ExpandedPeers & CandidatesMap[otherOnlyDigit];
						foreach (var cells2 in cellsEffected | 3)
						{
							if (!cells2.IsInIntersection)
							{
								continue;
							}

							var cells1 = cellsContainingThisDigit & cells2.PeerIntersection;
							var node1 = new Node(Subview.ExpandedCellFromDigit(in cells1, otherOnlyDigit), true, in urCandidatesMap);
							var node2 = new Node(Subview.ExpandedCellFromDigit(in cells2, otherOnlyDigit), false, in urCandidatesMap);
							linkDictionary.AddEntry(node1, node2, false, ur);
						}
						break;
					}
					case 2:
					{
						var otherDigit1 = TrailingZeroCount(otherDigitsMask);
						var cells1 = urCells & CandidatesMap[otherDigit1];
						if (cells1.IsInIntersection)
						{
							var node1 = new Node(Subview.ExpandedCellFromDigit(in cells1, otherDigit1), true, in urCandidatesMap);
							foreach (var cells2 in cells1.PeerIntersection & CandidatesMap[otherDigit1] | 3)
							{
								if (!cells2.IsInIntersection)
								{
									continue;
								}

								var node2 = new Node(Subview.ExpandedCellFromDigit(in cells2, otherDigit1), false, in urCandidatesMap);
								linkDictionary.AddEntry(node1, node2, false, ur);
							}
						}

						var otherDigit2 = otherDigitsMask.GetNextSet(otherDigit1);
						var cells3 = urCells & CandidatesMap[otherDigit2];
						if (cells3.IsInIntersection)
						{
							var node3 = new Node(Subview.ExpandedCellFromDigit(in cells3, otherDigit2), true, in urCandidatesMap);
							foreach (var cells4 in cells3.PeerIntersection & CandidatesMap[otherDigit1] | 3)
							{
								if (!cells4.IsInIntersection)
								{
									continue;
								}

								var node4 = new Node(Subview.ExpandedCellFromDigit(in cells4, otherDigit2), false, in urCandidatesMap);
								linkDictionary.AddEntry(node3, node4, false, ur);
							}
						}
						break;
					}
					default:
					{
						continue;
					}
				}
			}
		}
#endif
	}

	/// <inheritdoc/>
	public override void CollectExtraViewNodes(ref readonly Grid grid, ChainPattern pattern, ref View[] views)
	{
		var (view, id) = (views[0], ColorIdentifier.Auxiliary3);
		foreach (var link in pattern.Links)
		{
			if (link.GroupedLinkPattern is UniqueRectangle { Cells: var cells, DigitsMask: var digitsMask })
			{
				// If the cell has already been colorized, we should change the color into UR-categorized one.
				foreach (var cell in cells)
				{
					foreach (var digit in (Mask)(grid.GetCandidates(cell) & digitsMask))
					{
						var candidate = cell * 9 + digit;
						if (view.FindCandidate(candidate) is { } candidateViewNode)
						{
							view.Remove(candidateViewNode);
						}
						view.Add(new CandidateViewNode(id, candidate));
					}
				}
				foreach (var cell in cells)
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
}
