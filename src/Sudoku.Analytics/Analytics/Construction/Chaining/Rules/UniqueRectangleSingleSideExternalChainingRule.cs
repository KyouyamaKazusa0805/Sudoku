namespace Sudoku.Analytics.Construction.Chaining.Rules;

/// <summary>
/// Represents a chaining rule on AUR rule (i.e. <see cref="LinkType.UniqueRectangle_SingleSideExternal"/>).
/// </summary>
/// <seealso cref="LinkType.UniqueRectangle_SingleSideExternal"/>
public sealed class UniqueRectangleSingleSideExternalChainingRule : UniqueRectangleChainingRule
{
	/// <inheritdoc/>
	public override void GetLinks(ref ChainingRuleLinkContext context)
	{
		if (context.GetLinkOption(LinkType.UniqueRectangle_SingleSideExternal) == LinkOption.None)
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

				var urDigitsMask = (Mask)(1 << digitPair[0] | 1 << digitPair[1]);
				var otherDigitsMask = (Mask)(allDigitsMask & ~urDigitsMask);
				var ur = new UniqueRectanglePattern(in urCells, urDigitsMask, otherDigitsMask);

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

				foreach (var lockedHouse in urCellsContainingOtherDigits.SharedHouses)
				{
					var cells1 = HousesMap[lockedHouse] & __CandidatesMap[d1] & ~urCells;
					var cells2 = HousesMap[lockedHouse] & __CandidatesMap[d2] & ~urCells;
					if (linkOption == LinkOption.Intersection && (cells1.IsInIntersection || cells2.IsInIntersection)
						|| linkOption != LinkOption.Intersection)
					{
						var node1 = new Node(cells1 * d1, false);
						var node2 = new Node(cells2 * d2, true);
						context.StrongLinks.AddEntry(node1, node2, true, ur);
					}
				}
			}
		}
	}
}
