#define LIMIT_STRONG_LINK_NODE_IN_INTERSECTION
#define LIMIT_WEAK_LINK_NODE_IN_INTERSECTION

#if LIMIT_STRONG_LINK_NODE_IN_INTERSECTION && LIMIT_WEAK_LINK_NODE_IN_INTERSECTION
namespace Sudoku.Analytics.Chaining;

internal partial class CachedAlmostUniqueRectangleChainingRule
{
	partial void Type1Strong(short otherDigitsMask, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary)
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
			var node1 = new Node(Subview.ExpandedCellFromDigit(in cells1, otherOnlyDigit), false, true);
			var node2 = new Node(Subview.ExpandedCellFromDigit(in cells2, otherOnlyDigit), true, true);
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
			var node3 = new Node(Subview.ExpandedCellFromDigit(in cells3, otherOnlyDigit), false, true);
			var node4 = new Node(Subview.ExpandedCellFromDigit(in cells4, otherOnlyDigit), true, true);
			linkDictionary.AddEntry(node3, node4, false, ur);
		}
	}

	partial void Type2Strong(short otherDigitsMask, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary)
	{
		var theOtherDigit1 = TrailingZeroCount(otherDigitsMask);
		var theOtherDigit2 = otherDigitsMask.GetNextSet(theOtherDigit1);
		var cells1 = CandidatesMap[theOtherDigit1] & urCells;
		var cells2 = CandidatesMap[theOtherDigit2] & urCells;
#if LIMIT_STRONG_LINK_NODE_IN_INTERSECTION
		if (cells1.IsInIntersection && cells2.IsInIntersection)
#endif
		{
			var node1 = new Node(Subview.ExpandedCellFromDigit(in cells1, theOtherDigit1), false, true);
			var node2 = new Node(Subview.ExpandedCellFromDigit(in cells2, theOtherDigit2), true, true);
			linkDictionary.AddEntry(node1, node2, true, ur);
		}
	}

	partial void Type4Strong(short otherDigitsMask, ref readonly Grid grid, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary)
	{
		foreach (var otherDigit in otherDigitsMask)
		{
			var cells = CandidatesMap[otherDigit] & urCells;
			if (!cells.IsInIntersection)
			{
				continue;
			}

			// Determine whether such cells only contain UR digits and the selected digit.
			var urDigitsMask = ur.DigitsMask;
			var unrelatedDigitsMask = (Mask)0;
			foreach (var cell in cells)
			{
				unrelatedDigitsMask |= (Mask)((Mask)(grid.GetCandidates(cell) & (Mask)~urDigitsMask) & (Mask)~(1 << otherDigit));
			}
			if (unrelatedDigitsMask != 0)
			{
				continue;
			}

			// Determine whether the other cells are in an intersection.
			var urOtherSideCells = urCells & ~cells;
			if (!urOtherSideCells.IsInIntersection)
			{
				continue;
			}

			var node1 = new Node(Subview.ExpandedCellFromDigit(in cells, otherDigit), false, true);
			foreach (var lockedHouse in urOtherSideCells.SharedHouses)
			{
				var lockedUrDigitsMask = (Mask)0;
				foreach (var digit in urDigitsMask)
				{
					var lockedCells = HousesMap[lockedHouse] & CandidatesMap[digit];
					if (lockedCells == urOtherSideCells)
					{
						lockedUrDigitsMask |= (Mask)(1 << digit);
					}
				}

				if (lockedUrDigitsMask == 0 || urDigitsMask == lockedUrDigitsMask)
				{
					// No enough digits can be used.
					continue;
				}

				var lastUrDigit = Log2((uint)(urDigitsMask & (Mask)~lockedUrDigitsMask));
				var otherCellsContainingLastUrDigit = HousesMap[lockedHouse] & CandidatesMap[lastUrDigit] & ~urCells;
				if (!otherCellsContainingLastUrDigit.IsInIntersection)
				{
					continue;
				}

				var node2 = new Node(Subview.ExpandedCellFromDigit(in otherCellsContainingLastUrDigit, lastUrDigit), true, true);
				linkDictionary.AddEntry(node1, node2, true, ur);
			}
		}
	}

	partial void Type1Weak(short otherDigitsMask, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary)
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
			var node1 = new Node(Subview.ExpandedCellFromDigit(in cells1, otherOnlyDigit), true, true);
			var node2 = new Node(Subview.ExpandedCellFromDigit(in cells2, otherOnlyDigit), false, true);
			linkDictionary.AddEntry(node1, node2, false, ur);
		}
	}

	partial void Type2Weak(short otherDigitsMask, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary)
	{
		var otherDigit1 = TrailingZeroCount(otherDigitsMask);
		var cells1 = urCells & CandidatesMap[otherDigit1];
		if (cells1.IsInIntersection)
		{
			var node1 = new Node(Subview.ExpandedCellFromDigit(in cells1, otherDigit1), true, true);
			foreach (var cells2 in cells1.PeerIntersection & CandidatesMap[otherDigit1] | 3)
			{
				if (!cells2.IsInIntersection)
				{
					continue;
				}

				var node2 = new Node(Subview.ExpandedCellFromDigit(in cells2, otherDigit1), false, true);
				linkDictionary.AddEntry(node1, node2, false, ur);
			}
		}

		var otherDigit2 = otherDigitsMask.GetNextSet(otherDigit1);
		var cells3 = urCells & CandidatesMap[otherDigit2];
		if (cells3.IsInIntersection)
		{
			var node3 = new Node(Subview.ExpandedCellFromDigit(in cells3, otherDigit2), true, true);
			foreach (var cells4 in cells3.PeerIntersection & CandidatesMap[otherDigit1] | 3)
			{
				if (!cells4.IsInIntersection)
				{
					continue;
				}

				var node4 = new Node(Subview.ExpandedCellFromDigit(in cells4, otherDigit2), false, true);
				linkDictionary.AddEntry(node3, node4, false, ur);
			}
		}
	}
}
#endif
