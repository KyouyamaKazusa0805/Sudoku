using System.Diagnostics;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Grid"/>.
	/// </summary>
	/// <seealso cref="Grid"/>
	[DebuggerStepThrough]
	public static class GridEx
	{
		/// <summary>
		/// Deconstruct the grid.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <param name="emptyCells">
		/// (<see langword="out"/> parameter) The distribution of all empty cells.
		/// </param>
		/// <param name="bivalueCells">
		/// (<see langword="out"/> parameter) The distributions of all bivalue cells.
		/// </param>
		/// <param name="digitsDistributions">
		/// (<see langword="out"/> parameter) The distributions of all digits.
		/// </param>
		public static void Deconstruct(
			this Grid @this, out GridMap emptyCells,
			out GridMap bivalueCells, out GridMap[] digitsDistributions)
		{
			emptyCells = GetEmptyCellsGridMap(@this);
			bivalueCells = GetBivalueCellsMap(@this);
			digitsDistributions = GetAllDigitDistributionMaps(@this);
		}

		/// <summary>
		/// Get the grid map instance with all bivalue cells
		/// set <see langword="true"/>.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>The result.</returns>
		private static GridMap GetBivalueCellsMap(Grid grid)
		{
			var result = GridMap.Empty;
			for (int cell = 0; cell < 81; cell++)
			{
				var (status, candidates) = grid.GetMask(cell);
				if (status == CellStatus.Empty && candidates.CountSet() == 7)
				{
					result[cell] = true;
				}
			}

			return result;
		}

		/// <summary>
		/// Get the grid map instance with all empty cells set
		/// <see langword="true"/>.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>The result.</returns>
		private static GridMap GetEmptyCellsGridMap(Grid grid)
		{
			var result = GridMap.Empty;
			for (int cell = 0; cell < 81; cell++)
			{
				(var status, _) = grid.GetMask(cell);
				if (status == CellStatus.Empty)
				{
					result[cell] = true;
				}
			}

			return result;
		}

		/// <summary>
		/// Get 9 grid maps representing all digits' distributions.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>All grid maps.</returns>
		private static GridMap[] GetAllDigitDistributionMaps(Grid grid)
		{
			var result = new GridMap[9];
			for (int i = 0; i < 9; i++)
			{
				ref var map = ref result[i];

				for (int cell = 0; cell < 81; cell++)
				{
					if (grid.CandidateExists(cell, i))
					{
						map[cell] = true;
					}
				}
			}

			return result;
		}
	}
}
