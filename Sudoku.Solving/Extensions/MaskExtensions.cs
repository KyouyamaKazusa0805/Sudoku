using System.Collections.Generic;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Extensions
{
	/// <summary>
	/// Provides extensions for a mask.
	/// </summary>
	public static class MaskExtensions
	{
		/// <summary>
		/// Get cells with specified mask, which consist of 9 bits and 1 is
		/// for yielding.
		/// </summary>
		/// <param name="region">The region.</param>
		/// <param name="mask">The mask.</param>
		/// <returns>The cells.</returns>
		public static IEnumerable<int> GetCells(int region, short mask)
		{
			for (int i = 0, t = mask; i < 9; i++, t >>= 1)
			{
				if ((t & 1) != 0)
				{
					yield return RegionUtils.GetCellOffset(region, i);
				}
			}
		}
	}
}
