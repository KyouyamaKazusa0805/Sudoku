using System.Runtime.CompilerServices;

namespace Sudoku.Solving.Utils
{
	/// <summary>
	/// Provides extension method used for cell offsets.
	/// </summary>
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
		public static string ToString(int cellOffset) => $"r{cellOffset / 9 + 1}c{cellOffset % 9 + 1}";

		/// <summary>
		/// Get the full string of the specified cell offset.
		/// </summary>
		/// <param name="cellOffset">The cell offset.</param>
		/// <returns>The output string described as 'r_c_b_'.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToFullString(int cellOffset) =>
			$"r{cellOffset / 9 + 1}c{cellOffset % 9 + 1}b{cellOffset / 3 * 3 + cellOffset % 3 + 1}";
	}
}
