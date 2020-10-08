using Sudoku.Data;
using static Sudoku.Constants.Processings;

namespace Sudoku.Solving.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="GridMap"/>.
	/// </summary>
	/// <seealso cref="GridMap"/>
	public static class GridMapEx
	{
		/// <summary>
		/// Check whether the cells form an empty cell.
		/// </summary>
		/// <param name="blockMap">(<see langword="this"/> parameter) The empty cell grid map.</param>
		/// <param name="block">The block.</param>
		/// <param name="row">(<see langword="out"/> parameter) The row.</param>
		/// <param name="column">(<see langword="out"/> parameter) The column.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public static bool IsEmptyRectangle(this GridMap @this, int block, out int row, out int column)
		{
			int r = block / 3 * 3 + 9;
			int c = block % 3 * 3 + 18;
			for (int i = r, count = 0; i < r + 3; i++)
			{
				if ((@this & RegionMaps[i]).IsNotEmpty || ++count <= 1)
				{
					continue;
				}

				row = column = -1;
				return false;
			}

			for (int i = c, count = 0; i < c + 3; i++)
			{
				if ((@this & RegionMaps[i]).IsNotEmpty || ++count <= 1)
				{
					continue;
				}

				row = column = -1;
				return false;
			}

			for (int i = r; i < r + 3; i++)
			{
				for (int j = c; j < c + 3; j++)
				{
					if ((@this - (RegionMaps[i] | RegionMaps[j])).IsNotEmpty)
					{
						continue;
					}

					(row, column) = (i, j);
					return true;
				}
			}

			row = column = -1;
			return false;
		}
	}
}
