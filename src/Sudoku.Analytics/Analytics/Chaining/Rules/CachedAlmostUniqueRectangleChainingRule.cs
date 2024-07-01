namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Represents a chaining rule on AUR rule (i.e. <see cref="LinkType.AlmostUniqueRectangle"/>).
/// </summary>
/// <seealso cref="LinkType.AlmostUniqueRectangle"/>
internal sealed partial class CachedAlmostUniqueRectangleChainingRule : ChainingRule
{
	/// <inheritdoc/>
	protected internal override void CollectLinks(ref readonly ChainingRuleLinkCollectingContext context)
	{
		ref readonly var grid = ref context.Grid;
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
				var urDigitsMask = (Mask)(1 << digitPair[0] | 1 << digitPair[1]);
				var otherDigitsMask = (Mask)(allDigitsMask & (Mask)~urDigitsMask);
				var ur = new UniqueRectangle(in urCells, urDigitsMask, otherDigitsMask);
				switch (PopCount((uint)otherDigitsMask))
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
	protected internal override void MapViewNodes(ref ChainingRuleViewNodesMappingContext context)
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


	partial void Type1Strong(Mask otherDigitsMask, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary, LinkOption linkOption);
	partial void Type2Strong(Mask otherDigitsMask, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary, LinkOption linkOption);
	partial void Type4Strong(Mask otherDigitsMask, ref readonly Grid grid, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary, LinkOption linkOption);
	partial void Type5Strong(Mask otherDigitsMask, ref readonly Grid grid, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary, LinkOption linkOption);
}
