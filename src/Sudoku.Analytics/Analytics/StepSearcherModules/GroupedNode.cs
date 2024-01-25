namespace Sudoku.Analytics.StepSearcherModules;

/// <summary>
/// Represents a list of methods that operates with concept "Grouped Node".
/// </summary>
public static class GroupedNode
{
	/// <summary>
	/// Determine whether the specified cells can be split into two parts, and they form a grouped strong link with each other.
	/// </summary>
	/// <param name="cells">The cells to be checked.</param>
	/// <param name="digit">The digit to be checked.</param>
	/// <param name="house">The house.</param>
	/// <param name="spannedHouses">The spanned houses.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool IsGroupedStrongLink(scoped ref CellMap cells, Digit digit, House house, out HouseMask spannedHouses)
	{
		if ((HousesMap[house] & CandidatesMap[digit]).Count != 2)
		{
			goto ReturnFalse;
		}

		cells &= CandidatesMap[digit];
		switch (house.ToHouseType())
		{
			case HouseType.Block:
			{
				// Check row.
				var rowsSpanned = cells.RowMask << 9;
				if (PopCount((uint)rowsSpanned) == 2)
				{
					spannedHouses = rowsSpanned;
					return true;
				}

				// Check column.
				var columnSpanned = cells.ColumnMask << 18;
				if (PopCount((uint)columnSpanned) == 2)
				{
					spannedHouses = columnSpanned;
					return true;
				}

				break;
			}
			default:
			{
				// Check block.
				var blocksSpanned = cells.BlockMask;
				if (PopCount((uint)blocksSpanned) == 2)
				{
					spannedHouses = blocksSpanned;
					return true;
				}

				break;
			}
		}

	ReturnFalse:
		spannedHouses = default;
		return false;
	}
}
