using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Extensions;
using static Sudoku.Data.CellStatus;

namespace Sudoku
{
	partial class GridProcessings
	{
#pragma warning disable 8618
		/// <summary>
		/// <para>Indicates a table for each cell's peers.</para>
		/// </summary>
		/// <example>
		/// '<c>Peers[0]</c>': the array of peers for the cell 0 (row 1 column 1).
		/// </example>
		public static readonly int[][] Peers;

		/// <summary>
		/// <para>
		/// The map of all cell offsets in its specified region.
		/// The indices is between 0 and 26, where <c>0..9</c> is for block 1 to 9,
		/// <c>9..18</c> is for row 1 to 9 and <c>18..27</c> is for column 1 to 9.
		/// </para>
		/// </summary>
		/// <example>
		/// '<c>RegionTable[0]</c>': all cell offsets in the region 0 (block 1).
		/// </example>
		public static readonly int[][] RegionCells;

		/// <summary>
		/// Indicates all grid maps that a grid contains.
		/// </summary>
		/// <example>
		/// '<c>RegionMaps[0]</c>': The map containing all cells in the block 1.
		/// </example>
		public static readonly GridMap[] RegionMaps;

		/// <summary>
		/// <para>
		/// Indicates all maps that forms the each intersection. The pattern will be like:
		/// <code>
		/// .-------.-------.-------.<br/>
		/// | C C C | A A A | A A A |<br/>
		/// | B B B | . . . | . . . |<br/>
		/// | B B B | . . . | . . . |<br/>
		/// '-------'-------'-------'
		/// </code>
		/// </para>
		/// </summary>
		public static readonly IReadOnlyDictionary<(byte _baseSet, byte _coverSet), (GridMap _a, GridMap _b, GridMap _c)> IntersectionMaps;
#pragma warning restore 8618


		/// <summary>
		/// Get the map of all empty cells in this grid.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <returns>The map.</returns>
		public static GridMap GetEmptyCellsMap(this IReadOnlyGrid @this)
		{
			var result = GridMap.Empty;
			for (int cell = 0; cell < 81; cell++)
			{
				if (@this.GetCellStatus(cell) == Empty)
				{
					result.Add(cell);
				}
			}

			return result;
		}

		/// <summary>
		/// Get the map of all bi-value cells in this grid.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <returns>The map.</returns>
		public static GridMap GetBivalueCellsMap(this IReadOnlyGrid @this)
		{
			var result = GridMap.Empty;
			for (int cell = 0; cell < 81; cell++)
			{
				if (@this.GetCandidatesReversal(cell).CountSet() == 2)
				{
					result.Add(cell);
				}
			}

			return result;
		}

		/// <summary>
		/// Get the map of all distributions for digits.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <returns>The map.</returns>
		public static GridMap[] GetCandidatesMap(this IReadOnlyGrid @this)
		{
			var result = new GridMap[9];
			for (int digit = 0; digit < 9; digit++)
			{
				ref var map = ref result[digit];
				for (int cell = 0; cell < 81; cell++)
				{
					if (@this.Exists(cell, digit) is true)
					{
						map.Add(cell);
					}
				}
			}

			return result;
		}
	}
}
