using Sudoku.Data.Stepping;
using static Sudoku.Constants.Tables;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="UndoableGrid"/>.
	/// </summary>
	/// <seealso cref="UndoableGrid"/>
#if SUDOKU_UI
	[Obsolete("In the future, this class won't be used.", false)]
#endif
	public static class UndoableGridEx
	{
		/// <summary>
		/// Check whether the digit will be duplicate of its peers when it is filled in the specified cell.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <param name="cell">The cell.</param>
		/// <param name="digit">The digit.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public static bool Duplicate(this UndoableGrid @this, int cell, int digit)
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
