using Sudoku.Data;

namespace Sudoku.Solving.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Grid"/>.
	/// </summary>
	/// <seealso cref="Grid"/>
	public static class GridEx
	{
		/// <summary>
		/// Get the mask that is a result after the bitwise and operation processed all cells
		/// in the specified map.
		/// </summary>
		/// <param name="grid">(<see langword="this"/> parameter) The grid.</param>
		/// <param name="map">The map.</param>
		/// <returns>The result.</returns>
		public static short BitwiseAndMasks(this Grid grid, GridMap map)
		{
			short mask = Grid.MaxCandidatesMask;
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
		/// <param name="grid">(<see langword="this"/> parameter) The grid.</param>
		/// <param name="map">The map.</param>
		/// <returns>The result.</returns>
		public static short BitwiseOrMasks(this Grid grid, GridMap map)
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
