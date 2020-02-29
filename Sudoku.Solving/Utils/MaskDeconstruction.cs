using System.Diagnostics;
using Sudoku.Data;

namespace Sudoku.Solving.Utils
{
	/// <summary>
	/// Provides a deconstruction method for grid mask (<see cref="short"/> value).
	/// </summary>
	[DebuggerStepThrough]
	public static class MaskDeconstruction
	{
		/// <summary>
		/// Deconstruct the value to candidates and the cell status.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <param name="cellStatus">(<see langword="out"/> parameter) The cell status.</param>
		/// <param name="candidatesMask">(<see langword="out"/> parameter) The candidate mask.</param>
		public static void Deconstruct(
			this short @this, out CellStatus cellStatus, out short candidatesMask) =>
			(cellStatus, candidatesMask) = ((CellStatus)(@this >> 9 & (int)CellStatus.All), (short)(@this & 511));
	}
}
