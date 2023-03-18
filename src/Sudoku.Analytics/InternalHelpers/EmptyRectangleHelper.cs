namespace Sudoku.Analytics.InternalHelpers;

/// <summary>
/// Represents a helper type that operates with empty rectangles.
/// </summary>
internal static class EmptyRectangleHelper
{
	/// <summary>
	/// Determine whether the specified cells in the specified block form an empty rectangle.
	/// </summary>
	/// <param name="cells">The cells to be checked.</param>
	/// <param name="block">The block where the cells may form an empty rectangle structure.</param>
	/// <param name="row">The row that the empty rectangle used.</param>
	/// <param name="column">The column that the empty rectangle used.</param>
	/// <returns>
	/// A <see cref="bool"/> value indicating that. If <see langword="true"/>,
	/// both arguments <paramref name="row"/> and <paramref name="column"/> can be used;
	/// otherwise, both arguments should be discards.
	/// </returns>
	public static bool IsEmptyRectangle(scoped in CellMap cells, int block, out int row, out int column)
	{
		int r = block / 3 * 3 + 9, c = block % 3 * 3 + 18;
		for (int i = r, count = 0; i < r + 3; i++)
		{
			if ((cells & HousesMap[i]) is not [] || ++count <= 1)
			{
				continue;
			}

			row = column = -1;
			return false;
		}

		for (int i = c, count = 0; i < c + 3; i++)
		{
			if ((cells & HousesMap[i]) is not [] || ++count <= 1)
			{
				continue;
			}

			row = column = -1;
			return false;
		}

		for (var i = r; i < r + 3; i++)
		{
			for (var j = c; j < c + 3; j++)
			{
				if (cells - (HousesMap[i] | HousesMap[j]))
				{
					continue;
				}

				row = i;
				column = j;
				return true;
			}
		}

		row = column = -1;
		return false;
	}
}
