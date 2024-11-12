namespace Sudoku.Analytics.Construction.Chaining.Rules;

/// <summary>
/// Represents a chaining rule on AUR rule (i.e. <see cref="LinkType.AlmostUniqueRectangle"/>).
/// </summary>
/// <seealso cref="LinkType.AlmostUniqueRectangle"/>
public sealed class AlmostUniqueRectangleChainingRule : ChainingRule
{
	/// <inheritdoc/>
	public override void GetLinks(ref ChainingRuleLinkContext context)
	{
		if (context.GetLinkOption(LinkType.AlmostUniqueRectangle) == LinkOption.None)
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

		var strongLinks = context.StrongLinks;
		var linkOption = context.GetLinkOption(LinkType.AlmostUniqueRectangle);
		foreach (var pattern in UniqueRectanglePattern.AllPatterns)
		{
			var urCells = pattern.AsCellMap();
			if ((__EmptyCells & urCells) != urCells)
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
			var allDigitsMask = grid[urCells];
			foreach (var digit in allDigitsMask)
			{
				var cellsToFillDigit = __CandidatesMap[digit] & urCells;
				if (Mask.PopCount(cellsToFillDigit.RowMask) == 2 && Mask.PopCount(cellsToFillDigit.ColumnMask) == 2)
				{
					validDigitsMask |= (Mask)(1 << digit);
				}
			}
			if (Mask.PopCount(validDigitsMask) < 2)
			{
				// Not enough digits to form a UR.
				continue;
			}

			foreach (var digitPair in validDigitsMask.GetAllSets().GetSubsets(2))
			{
				var urDigitsMask = (Mask)(1 << digitPair[0] | 1 << digitPair[1]);
				var otherDigitsMask = (Mask)(allDigitsMask & ~urDigitsMask);
				var ur = new UniqueRectanglePattern(in urCells, urDigitsMask, otherDigitsMask);
				switch (Mask.PopCount(otherDigitsMask))
				{
					case 1:
					{
						type1Strong(otherDigitsMask, in urCells, ur, strongLinks, linkOption, __CandidatesMap);
						break;
					}
					case 2:
					{
						type2Strong(otherDigitsMask, in urCells, ur, strongLinks, linkOption, __CandidatesMap);
						goto default;
					}
					default:
					{
						type4Strong(otherDigitsMask, in grid, in urCells, ur, strongLinks, linkOption, __CandidatesMap);
						type5Strong(otherDigitsMask, in grid, in urCells, ur, strongLinks, linkOption, __CandidatesMap);
						break;
					}
				}
			}
		}


		static void type1Strong(
			Mask otherDigitsMask,
			ref readonly CellMap urCells,
			UniqueRectanglePattern ur,
			LinkDictionary linkDictionary,
			LinkOption linkOption,
			ReadOnlySpan<CellMap> candidatesMap
		)
		{
			var otherOnlyDigit = Mask.Log2(otherDigitsMask);
			var cellsContainingThisDigit = candidatesMap[otherOnlyDigit] & urCells;
			var rowsSpanned = cellsContainingThisDigit.RowMask << 9;
			if (HouseMask.PopCount(rowsSpanned) == 2)
			{
				var row1 = HouseMask.TrailingZeroCount(rowsSpanned);
				var row2 = rowsSpanned.GetNextSet(row1);
				var cells1 = cellsContainingThisDigit & HousesMap[row1];
				var cells2 = cellsContainingThisDigit & HousesMap[row2];
				if (linkOption == LinkOption.Intersection && (cells1.IsInIntersection || cells2.IsInIntersection)
					|| linkOption != LinkOption.Intersection)
				{
					var node1 = new Node(cells1 * otherOnlyDigit, false);
					var node2 = new Node(cells2 * otherOnlyDigit, true);
					linkDictionary.AddEntry(node1, node2, true, ur);
				}
			}

			var columnsSpanned = cellsContainingThisDigit.ColumnMask << 18;
			if (HouseMask.PopCount(columnsSpanned) == 2)
			{
				var column1 = HouseMask.TrailingZeroCount(columnsSpanned);
				var column2 = columnsSpanned.GetNextSet(column1);
				var cells3 = cellsContainingThisDigit & HousesMap[column1];
				var cells4 = cellsContainingThisDigit & HousesMap[column2];
				if (linkOption == LinkOption.Intersection && (cells3.IsInIntersection || cells4.IsInIntersection)
					|| linkOption != LinkOption.Intersection)
				{
					var node3 = new Node(cells3 * otherOnlyDigit, false);
					var node4 = new Node(cells4 * otherOnlyDigit, true);
					linkDictionary.AddEntry(node3, node4, false, ur);
				}
			}
		}

		static void type2Strong(
			Mask otherDigitsMask,
			ref readonly CellMap urCells,
			UniqueRectanglePattern ur,
			LinkDictionary linkDictionary,
			LinkOption linkOption,
			ReadOnlySpan<CellMap> candidatesMap
		)
		{
			var theOtherDigit1 = Mask.TrailingZeroCount(otherDigitsMask);
			var theOtherDigit2 = otherDigitsMask.GetNextSet(theOtherDigit1);
			var cells1 = candidatesMap[theOtherDigit1] & urCells;
			var cells2 = candidatesMap[theOtherDigit2] & urCells;
			if (linkOption == LinkOption.Intersection && (cells1.IsInIntersection || cells2.IsInIntersection)
				|| linkOption != LinkOption.Intersection)
			{
				var node1 = new Node(cells1 * theOtherDigit1, false);
				var node2 = new Node(cells2 * theOtherDigit2, true);
				linkDictionary.AddEntry(node1, node2, true, ur);
			}
		}

		static void type4Strong(
			Mask otherDigitsMask,
			ref readonly Grid grid,
			ref readonly CellMap urCells,
			UniqueRectanglePattern ur,
			LinkDictionary linkDictionary,
			LinkOption linkOption,
			ReadOnlySpan<CellMap> candidatesMap
		)
		{
			foreach (var otherDigit in otherDigitsMask)
			{
				var cells = candidatesMap[otherDigit] & urCells;
				if (!cells.IsInIntersection)
				{
					continue;
				}

				// Determine whether such cells only contain UR digits and the selected digit.
				var urDigitsMask = ur.DigitsMask;
				var unrelatedDigitsMask = (Mask)0;
				foreach (var cell in cells)
				{
					unrelatedDigitsMask |= (Mask)(grid.GetCandidates(cell) & ~urDigitsMask & ~(1 << otherDigit));
				}
				if (unrelatedDigitsMask != 0)
				{
					continue;
				}

				// Determine whether the other cells are in an intersection.
				var urOtherSideCells = urCells & ~cells;
				if (linkOption == LinkOption.Intersection && !urOtherSideCells.IsInIntersection)
				{
					continue;
				}

				var node1 = new Node(cells * otherDigit, false);
				foreach (var lockedHouse in urOtherSideCells.SharedHouses)
				{
					var lockedUrDigitsMask = (Mask)0;
					foreach (var digit in urDigitsMask)
					{
						var lockedCells = HousesMap[lockedHouse] & candidatesMap[digit];
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

					var lastUrDigit = Mask.Log2((Mask)(urDigitsMask & ~lockedUrDigitsMask));
					var otherCellsContainingLastUrDigit = HousesMap[lockedHouse] & candidatesMap[lastUrDigit] & ~urCells;
					if (linkOption == LinkOption.Intersection && !otherCellsContainingLastUrDigit.IsInIntersection)
					{
						continue;
					}

					var node2 = new Node(otherCellsContainingLastUrDigit * lastUrDigit, true);
					linkDictionary.AddEntry(node1, node2, true, ur);
				}
			}
		}

		static void type5Strong(
			Mask otherDigitsMask,
			ref readonly Grid grid,
			ref readonly CellMap urCells,
			UniqueRectanglePattern ur,
			LinkDictionary linkDictionary,
			LinkOption linkOption,
			ReadOnlySpan<CellMap> candidatesMap
		)
		{
			var urCellsContainingOtherDigits = CellMap.Empty;
			foreach (var cell in urCells)
			{
				if ((grid.GetCandidates(cell) & otherDigitsMask) != 0)
				{
					urCellsContainingOtherDigits.Add(cell);
				}
			}
			if (linkOption == LinkOption.Intersection && !urCellsContainingOtherDigits.IsInIntersection)
			{
				return;
			}

			var urDigitsMask = ur.DigitsMask;
			var digit1 = Mask.TrailingZeroCount(urDigitsMask);
			var digit2 = urDigitsMask.GetNextSet(digit1);
			foreach (var lockedHouse in urCellsContainingOtherDigits.SharedHouses)
			{
				var cells1 = HousesMap[lockedHouse] & candidatesMap[digit1] & ~urCells;
				var cells2 = HousesMap[lockedHouse] & candidatesMap[digit2] & ~urCells;
				if (linkOption == LinkOption.Intersection && (cells1.IsInIntersection || cells2.IsInIntersection)
					|| linkOption != LinkOption.Intersection)
				{
					var node1 = new Node(cells1 * digit1, false);
					var node2 = new Node(cells2 * digit2, true);
					linkDictionary.AddEntry(node1, node2, true, ur);
				}
			}
		}
	}

	/// <inheritdoc/>
	public override void GetViewNodes(ref ChainingRuleViewNodeContext context)
	{
		ref readonly var grid = ref context.Grid;
		var pattern = context.Pattern;
		var view = context.View;

		var result = new List<ViewNode>();
		foreach (var link in pattern.Links)
		{
			if (link.GroupedLinkPattern is UniqueRectanglePattern { Cells: var cells, DigitsMask: var digitsMask })
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
						var node = new CandidateViewNode(ColorIdentifier.Rectangle1, candidate);
						view.Add(node);
						result.Add(node);
					}
				}
				foreach (var cell in cells)
				{
					if (view.FindCell(cell) is { } cellViewNode)
					{
						view.Remove(cellViewNode);
					}
					var node = new CellViewNode(ColorIdentifier.Rectangle1, cell);
					view.Add(node);
					result.Add(node);
				}
			}
		}
		context.ProducedViewNodes = result.AsSpan();
	}

	/// <inheritdoc/>
	public override void GetLoopConclusions(ref ChainingRuleLoopConclusionContext context)
	{
		// VARIABLE_DECLARATION_BEGIN
		_ = 42;
		// VARIABLE_DECLARATION_END

		// This example will contain extra eliminations:
		//   4.........2..5...6..8..23...9..8.7....59.+7.6.7+8.3.4..2.7..4.6....3..1..91......8.:548 549 167 168 695
		//
		// Extra eliminations should be r7c8(25):
		//   .-----------------------.-----------------------.------------------------.
		//   | 4      156(3)  679-1  | 1678   1679(3)  689-3 | 12589  12579    1578   |
		//   | 9(3)   2       79-1   | 1478   5        389   | 1489   1479     6      |
		//   | 569    156     8      | 1467   1679     2     | 3      14579    1457   |
		//   |-----------------------+-----------------------+------------------------|
		//   | (236)  9       (1246) | 1256   8        56    | 7      (134)    (134)  |
		//   | (23)   34-1    5      | 9      12       7     | 148    6        1348   |
		//   | 7      8       (16)   | 3      16       4     | 59     59       2      |
		//   |-----------------------+-----------------------+------------------------|
		//   | 2589   7       29     | 258    4        3589  | 6      -25(13)  5(13)  |
		//   | 2568   456     3      | 25678  267      1     | 245    2457     9      |
		//   | 1      456     2469   | 2567   279(3)   569-3 | 245    8        457(3) |
		//   '-----------------------'-----------------------'------------------------'
		//
		// However I don't have any unified rules to calculate such eliminations
		// because AUR eliminations should be treated as "forced" ones, meaning we should check them by supposing it with "true"
		// to make a contradiction.
		// This will be implemented in the future.
		//
		// Binds with issue #680: https://github.com/KyouyamaKazusa0805/Sudoku/issues/680
		base.GetLoopConclusions(ref context);
	}
}
