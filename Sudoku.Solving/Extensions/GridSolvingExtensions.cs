using System.Diagnostics;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Grid"/> and <see cref="IReadOnlyGrid"/>.
	/// </summary>
	/// <seealso cref="Grid"/>
	/// <seealso cref="IReadOnlyGrid"/>
	[DebuggerStepThrough]
	public static class GridSolvingExtensions
	{
		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
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
			this IReadOnlyGrid @this, out GridMap emptyCells,
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
		private static GridMap GetBivalueCellsMap(IReadOnlyGrid grid)
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
		private static GridMap GetEmptyCellsGridMap(IReadOnlyGrid grid)
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
		private static GridMap[] GetAllDigitDistributionMaps(IReadOnlyGrid grid)
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
