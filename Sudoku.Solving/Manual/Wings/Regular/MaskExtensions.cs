using Sudoku.Data;

namespace Sudoku.Solving.Manual.Wings.Regular
{
	/// <summary>
	/// Provides extensions for a mask.
	/// </summary>
	internal static class MaskExtensions
	{
		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <param name="cellStatus">(<see langword="out"/> parameter) The cell status.</param>
		/// <param name="candidatesMask">(<see langword="out"/> parameter) The candidate mask.</param>
		public static void Deconstruct(this short @this, out CellStatus cellStatus, out short candidatesMask) =>
			(cellStatus, candidatesMask) = ((CellStatus)(@this >> 9 & (int)CellStatus.All), (short)(@this & 511));
	}
}
