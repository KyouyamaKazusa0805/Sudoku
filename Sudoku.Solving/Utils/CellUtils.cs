using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sudoku.Solving.Utils
{
	/// <summary>
	/// Provides extension method used for cell offsets.
	/// </summary>
	[DebuggerStepThrough]
	public static class CellUtils
	{
		/// <summary>
		/// Get the cell offset with specified row and column index.
		/// </summary>
		/// <param name="row">The row index.</param>
		/// <param name="column">The column index.</param>
		/// <returns>The cell offset.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetOffset(int row, int column) => row * 9 + column;

		/// <summary>
		/// Get the output string of the specified cell offset.
		/// </summary>
		/// <param name="cellOffset">The cell offset.</param>
		/// <returns>The output string described as 'r_c_'.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToString(int cellOffset) =>
			$"r{cellOffset / 9 + 1}c{cellOffset % 9 + 1}";

		/// <summary>
		/// <para>Get the row, column and block index of the specified cell.</para>
		/// <para>
		/// Note that all row, column and block indices are always between 0 and 8.
		/// </para>
		/// </summary>
		/// <param name="cellOffset">The cell offset.</param>
		/// <returns>
		/// The row, column and block index triplet.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static (int _row, int _column, int _block) GetRegion(int cellOffset) =>
			(cellOffset / 9, cellOffset % 9, cellOffset / 9 / 3 * 3 + cellOffset % 9 / 3);
	}
}
