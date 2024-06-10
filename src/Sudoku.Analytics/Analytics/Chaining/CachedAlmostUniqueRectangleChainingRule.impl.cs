namespace Sudoku.Analytics.Chaining;

internal partial class CachedAlmostUniqueRectangleChainingRule
{
	partial void Type1Strong(Mask otherDigitsMask, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary)
	{
		// Split the digit into two parts.
		var otherOnlyDigit = Log2((uint)otherDigitsMask);
		var cellsContainingThisDigit = CandidatesMap[otherOnlyDigit] & urCells;

		var rowsSpanned = cellsContainingThisDigit.RowMask << 9;
		var row1 = TrailingZeroCount(rowsSpanned);
		var row2 = rowsSpanned.GetNextSet(row1);
		var cells1 = cellsContainingThisDigit & HousesMap[row1];
		var cells2 = cellsContainingThisDigit & HousesMap[row2];
		if (cells1.IsInIntersection && cells2.IsInIntersection)
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
		if (cells3.IsInIntersection && cells4.IsInIntersection)
		{
			var node3 = new Node(Subview.ExpandedCellFromDigit(in cells3, otherOnlyDigit), false, true);
			var node4 = new Node(Subview.ExpandedCellFromDigit(in cells4, otherOnlyDigit), true, true);
			linkDictionary.AddEntry(node3, node4, false, ur);
		}
	}

	partial void Type2Strong(Mask otherDigitsMask, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary)
	{
		var theOtherDigit1 = TrailingZeroCount(otherDigitsMask);
		var theOtherDigit2 = otherDigitsMask.GetNextSet(theOtherDigit1);
		var cells1 = CandidatesMap[theOtherDigit1] & urCells;
		var cells2 = CandidatesMap[theOtherDigit2] & urCells;
		if (cells1.IsInIntersection && cells2.IsInIntersection)
		{
			var node1 = new Node(Subview.ExpandedCellFromDigit(in cells1, theOtherDigit1), false, true);
			var node2 = new Node(Subview.ExpandedCellFromDigit(in cells2, theOtherDigit2), true, true);
			linkDictionary.AddEntry(node1, node2, true, ur);
		}
	}

	partial void Type4Strong(Mask otherDigitsMask, ref readonly Grid grid, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary)
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

	partial void Type5Strong(Mask otherDigitsMask, ref readonly Grid grid, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary)
	{
		var urCellsContainingOtherDigits = CellMap.Empty;
		foreach (var cell in urCells)
		{
			if ((grid.GetCandidates(cell) & otherDigitsMask) != 0)
			{
				urCellsContainingOtherDigits.Add(cell);
			}
		}
		if (!urCellsContainingOtherDigits.IsInIntersection)
		{
			return;
		}

		var urDigitsMask = ur.DigitsMask;
		var digit1 = TrailingZeroCount(urDigitsMask);
		var digit2 = urDigitsMask.GetNextSet(digit1);
		foreach (var lockedHouse in urCellsContainingOtherDigits.SharedHouses)
		{
			var cells1 = HousesMap[lockedHouse] & CandidatesMap[digit1] & ~urCells;
			var cells2 = HousesMap[lockedHouse] & CandidatesMap[digit2] & ~urCells;
			if (!cells1.IsInIntersection || !cells2.IsInIntersection)
			{
				continue;
			}

			var node1 = new Node(Subview.ExpandedCellFromDigit(in cells1, digit1), false, true);
			var node2 = new Node(Subview.ExpandedCellFromDigit(in cells2, digit2), true, true);
			linkDictionary.AddEntry(node1, node2, true, ur);
		}
	}
}
