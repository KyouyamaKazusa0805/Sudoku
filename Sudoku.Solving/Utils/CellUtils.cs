using System.Collections.Generic;
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
		/// Get the full string of the specified cell offset.
		/// </summary>
		/// <param name="cellOffset">The cell offset.</param>
		/// <returns>The output string described as 'r_c_b_'.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToFullString(int cellOffset)
		{
			var (r, c, b) = GetRegion(cellOffset);
			return $"r{r + 1}c{c + 1}b{b + 1}";
		}

		/// <summary>
		/// <para>Get the row, column and block index of the specified cell.</para>
		/// <para>
		/// Note that all row, column and block indices are always between 0 to 8.
		/// </para>
		/// </summary>
		/// <param name="cellOffset">The cell offset.</param>
		/// <returns>
		/// The row, column and block index triplet.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static (int _row, int _column, int _block) GetRegion(int cellOffset) =>
			(cellOffset / 9, cellOffset % 9, cellOffset / 9 / 3 * 3 + cellOffset % 9 / 3);

		/// <summary>
		/// Check two cells have different row, column and block.
		/// </summary>
		/// <param name="cell1">The cell offset 1.</param>
		/// <param name="cell2">The cell offset 2.</param>
		/// <param name="sameRegions">(out parameter) All same regions of two cells.</param>
		/// <returns>A <see cref="bool"/> indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSameRegion(int cell1, int cell2, out int[] sameRegions)
		{
			var temp = new List<int>(3);
			var ((r1, c1, b1), (r2, c2, b2)) = (GetRegion(cell1), GetRegion(cell2));
			if (r1 == r2) temp.Add(r1 + 9);
			if (c1 == c2) temp.Add(c1 + 18);
			if (b1 == b2) temp.Add(b1);
			sameRegions = temp.ToArray();
			return temp.Count != 0;
		}
	}
}
