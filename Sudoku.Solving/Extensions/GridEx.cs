using Sudoku.Data;

namespace Sudoku.Solving.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="SudokuGrid"/>.
	/// </summary>
	/// <seealso cref="SudokuGrid"/>
	public static class SudokuGridEx
	{
		/// <summary>
		/// Get the mask that is a result after the bitwise and operation processed all cells
		/// in the specified map.
		/// </summary>
		/// <param name="grid">(<see langword="this in"/> parameter) The grid.</param>
		/// <param name="map">(<see langword="in"/> parameter) The map.</param>
		/// <returns>The result.</returns>
		public static short BitwiseAndMasks(this in SudokuGrid grid, in GridMap map)
		{
			short mask = SudokuGrid.MaxCandidatesMask;
			foreach (int cell in map)
			{
				mask &= grid.GetCandidateMask(cell);
			}

			return mask;
		}

		/// <summary>
		/// Get the mask that is a result after the bitwise or operation processed all cells
		/// in the specified map.
		/// </summary>
		/// <param name="grid">(<see langword="this in"/> parameter) The grid.</param>
		/// <param name="map">(<see langword="in"/> parameter) The map.</param>
		/// <returns>The result.</returns>
		public static short BitwiseOrMasks(this in SudokuGrid grid, in GridMap map)
		{
			short mask = 0;
			foreach (int cell in map)
			{
				mask |= grid.GetCandidateMask(cell);
			}

			return mask;
		}
	}
}
