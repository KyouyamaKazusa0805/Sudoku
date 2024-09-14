namespace Sudoku.Analytics.Patterning.Chaining.Rules;

/// <summary>
/// Represents a chaining rule on AUR rule (i.e. <see cref="LinkType.AlmostUniqueRectangle"/>).
/// </summary>
/// <seealso cref="LinkType.AlmostUniqueRectangle"/>
internal sealed partial class CachedAlmostUniqueRectangleChainingRule : ChainingRule
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

		var strongLinks = context.StrongLinks;
		var linkOption = context.GetLinkOption(LinkType.AlmostUniqueRectangle);
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
				var ur = new UniqueRectangle(in urCells, urDigitsMask, otherDigitsMask);
				switch (Mask.PopCount(otherDigitsMask))
				{
					case 1:
					{
						Type1Strong(otherDigitsMask, in urCells, ur, strongLinks, linkOption);
						break;
					}
					case 2:
					{
						Type2Strong(otherDigitsMask, in urCells, ur, strongLinks, linkOption);
						goto default;
					}
					default:
					{
						Type4Strong(otherDigitsMask, in grid, in urCells, ur, strongLinks, linkOption);
						Type5Strong(otherDigitsMask, in grid, in urCells, ur, strongLinks, linkOption);
						break;
					}
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
						var node = new CandidateViewNode(ColorIdentifier.Auxiliary3, candidate);
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
					var node = new CellViewNode(ColorIdentifier.Auxiliary3, cell);
					view.Add(node);
					result.Add(node);
				}
			}
		}
		context.ProducedViewNodes = result.AsReadOnlySpan();
	}

	/// <inheritdoc/>
	public override void GetLoopConclusions(ref ChainingRuleLoopConclusionContext context)
	{
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
		// Binds with issue #680:
		//    https://github.com/SunnieShine/Sudoku/issues/680
		base.GetLoopConclusions(ref context);
	}

	partial void Type1Strong(Mask otherDigitsMask, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary, LinkOption linkOption);
	partial void Type2Strong(Mask otherDigitsMask, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary, LinkOption linkOption);
	partial void Type4Strong(Mask otherDigitsMask, ref readonly Grid grid, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary, LinkOption linkOption);
	partial void Type5Strong(Mask otherDigitsMask, ref readonly Grid grid, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary, LinkOption linkOption);
}
