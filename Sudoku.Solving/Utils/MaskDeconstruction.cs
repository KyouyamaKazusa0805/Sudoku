using Sudoku.Data.Meta;
using Sudoku.Diagnostics.CodeAnalysis;

namespace Sudoku.Solving.Utils
{
	/// <summary>
	/// Provides a deconstruction method for grid mask (<see cref="short"/> value).
	/// </summary>
	public static class MaskDeconstruction
	{
		/// <summary>
		/// Deconstruct the value to candidates and the cell status.
		/// </summary>
		/// <param name="this">The value.</param>
		/// <param name="cellStatus">(out parameter) The cell status.</param>
		/// <param name="candidatesMask">(out parameter) The candidates.</param>
		[OnDeconstruction]
		public static void Deconstruct(
			this short @this, out CellStatus cellStatus, out short candidatesMask) =>
			(cellStatus, candidatesMask) = ((CellStatus)(@this >> 9 & (int)CellStatus.All), (short)(@this & 511));
	}
}
