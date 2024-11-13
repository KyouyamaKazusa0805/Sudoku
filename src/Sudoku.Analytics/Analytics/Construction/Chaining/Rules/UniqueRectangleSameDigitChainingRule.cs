namespace Sudoku.Analytics.Construction.Chaining.Rules;

/// <summary>
/// Represents a chaining rule on AUR rule (i.e. <see cref="LinkType.UniqueRectangle_SameDigit"/>).
/// </summary>
/// <seealso cref="LinkType.UniqueRectangle_SameDigit"/>
public sealed class UniqueRectangleSameDigitChainingRule : UniqueRectangleChainingRule
{
	/// <inheritdoc/>
	public override void GetLinks(ref ChainingRuleLinkContext context)
	{
		if (context.GetLinkOption(LinkType.UniqueRectangle_SameDigit) == LinkOption.None)
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

		var linkOption = context.GetLinkOption(LinkType.UniqueRectangle_SameDigit);
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

				var urDigitsMask = (Mask)(1 << d1 | 1 << d2);
				var otherDigitsMask = (Mask)(allDigitsMask & ~urDigitsMask);
				if (!Mask.IsPow2(otherDigitsMask))
				{
					continue;
				}

				var ur = new UniqueRectanglePattern(in urCells, urDigitsMask, otherDigitsMask);

				var otherOnlyDigit = Mask.Log2(otherDigitsMask);
				var cellsContainingThisDigit = __CandidatesMap[otherOnlyDigit] & urCells;
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
						StrongLinkDictionary.AddEntry(node1, node2, true, ur);
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
						context.StrongLinks.AddEntry(node3, node4, false, ur);
					}
				}
			}
		}
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
