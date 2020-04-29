using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Extensions;
using static Sudoku.Data.CellStatus;

namespace Sudoku
{
	/// <summary>
	/// The tables for grid processing. All fields will be initialized in
	/// the static constructor.
	/// </summary>
	[SuppressMessage("", "CS8618")]
	public static partial class GridProcessings
	{
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


		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <param name="empty">(<see langword="out"/> parameter) The map of all empty cells.</param>
		/// <param name="bivalue">(<see langword="out"/> parameter) The map of all bi-value cells.</param>
		/// <param name="candidates">
		/// (<see langword="out"/> parameter) The map of all cells that contain the candidate of that digit.
		/// </param>
		/// <param name="digits">
		/// (<see langword="out"/> parameter) The map of all cells that contain the candidate of that digit
		/// or that value in given or modifiable.
		/// </param>
		public static void Deconstruct(
			this IReadOnlyGrid @this, out GridMap empty, out GridMap bivalue,
			out GridMap[] candidates, out GridMap[] digits)
		{
			(empty, bivalue, candidates, digits) = (
				@this.GetEmptyCellsMap(), @this.GetBivalueCellsMap(),
				@this.GetCandidatesMap(), @this.GetDigitsMap());
		}

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
				if (@this.GetStatus(cell) == Empty)
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

		/// <summary>
		/// <para>Get the map of all distributions for digits.</para>
		/// <para>
		/// Different with <see cref="GetCandidatesMap(IReadOnlyGrid)"/>,
		/// this method will get all cells that contain the digit or fill this digit
		/// (given or modifiable).
		/// </para>
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <returns>The map.</returns>
		/// <seealso cref="GetCandidatesMap(IReadOnlyGrid)"/>
		public static GridMap[] GetDigitsMap(this IReadOnlyGrid @this)
		{
			var digitDistributions = new GridMap[9];
			for (int digit = 0; digit < 9; digit++)
			{
				ref var map = ref digitDistributions[digit];
				for (int cell = 0; cell < 81; cell++)
				{
					if ((@this.GetCandidatesReversal(cell) >> digit & 1) != 0)
					{
						map.Add(cell);
					}
				}
			}

			return digitDistributions;
		}
	}
}
