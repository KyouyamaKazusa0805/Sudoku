using System.Collections.Generic;
using static Sudoku.GridProcessings;

namespace Sudoku.Data.Extensions
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
					yield return RegionCells[region][i];
				}
			}
		}

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <param name="cellStatus">(<see langword="out"/> parameter) The cell status.</param>
		/// <param name="candidatesMask">(<see langword="out"/> parameter) The candidate mask.</param>
		public static void Deconstruct(
			this short @this, out CellStatus cellStatus, out short candidatesMask) =>
			(cellStatus, candidatesMask) = ((CellStatus)(@this >> 9 & (int)CellStatus.All), (short)(@this & 511));
	}
}
