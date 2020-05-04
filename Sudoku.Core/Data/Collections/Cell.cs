using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sudoku.Data.Collections
{
	/// <summary>
	/// Provides methods for cell offsets.
	/// </summary>
	[DebuggerStepThrough]
	public readonly struct Cell
	{
		/// <summary>
		/// <para>Get the row, column and block index of the specified cell.</para>
		/// <para>
		/// Note that all row, column and block indices are always between 0 and 8.
		/// </para>
		/// </summary>
		/// <param name="cell">The cell offset.</param>
		/// <returns>
		/// The row, column and block index triplet.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static (int _row, int _column, int _block) GetRegion(int cell) =>
			(cell / 9, cell % 9, cell / 9 / 3 * 3 + cell % 9 / 3);
	}
}
