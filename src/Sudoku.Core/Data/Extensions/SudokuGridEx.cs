using static Sudoku.Constants.Tables;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="SudokuGrid"/>.
	/// </summary>
	/// <seealso cref="SudokuGrid"/>
	public static class SudokuGridEx
	{
		/// <summary>
		/// Check whether the digit will be duplicate of its peers when it is filled in the specified cell.
		/// </summary>
		/// <param name="this">The grid.</param>
		/// <param name="cell">The cell.</param>
		/// <param name="digit">The digit.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public static bool Duplicate(this in SudokuGrid @this, int cell, int digit)
		{
			foreach (int peerCell in PeerMaps[cell])
			{
				if (@this[peerCell] == digit)
				{
					return true;
				}
			}
			return false;
		}
	}
}
