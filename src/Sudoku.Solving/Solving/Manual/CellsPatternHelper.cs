using Sudoku.Data;
using static Sudoku.Constants.Tables;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Provides extension methods on <see cref="Cells"/>.
	/// </summary>
	/// <seealso cref="Cells"/>
	public static class CellsPatternHelper
	{
		/// <summary>
		/// Check whether the cells form an empty cell.
		/// </summary>
		/// <param name="this">The empty cell grid map.</param>
		/// <param name="block">The block.</param>
		/// <param name="row">The row.</param>
		/// <param name="column">The column.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public static bool IsEmptyRectangle(this in Cells @this, int block, out int row, out int column)
		{
			int r = block / 3 * 3 + 9, c = block % 3 * 3 + 18;
			for (int i = r, count = 0, rPlus3 = r + 3; i < rPlus3; i++)
			{
				if (!(@this & RegionMaps[i]).IsEmpty || ++count <= 1)
				{
					continue;
				}

				row = column = -1;
				return false;
			}

			for (int i = c, count = 0, cPlus3 = c + 3; i < cPlus3; i++)
			{
				if (!(@this & RegionMaps[i]).IsEmpty || ++count <= 1)
				{
					continue;
				}

				row = column = -1;
				return false;
			}

			for (int i = r, rPlus3 = r + 3; i < rPlus3; i++)
			{
				for (int j = c, cPlus3 = c + 3; j < cPlus3; j++)
				{
					if (@this > (RegionMaps[i] | RegionMaps[j]))
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
}
