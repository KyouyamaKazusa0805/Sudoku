#nullable disable warnings

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Extensions;
using static Sudoku.Constants.RegionLabel;
using static Sudoku.Data.CellStatus;

namespace Sudoku.Constants
{
	/// <summary>
	/// The tables for grid processing. All fields will be initialized in
	/// the static constructor.
	/// </summary>
	[DebuggerStepThrough]
	public static partial class Processings
	{
		/// <summary>
		/// The block table.
		/// </summary>
		public static readonly int[] BlockTable;

		/// <summary>
		/// The row table.
		/// </summary>
		public static readonly int[] RowTable;

		/// <summary>
		/// The column table.
		/// </summary>
		public static readonly int[] ColumnTable;

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
		/// Indicates the peer maps using <see cref="Peers"/> table.
		/// </summary>
		/// <seealso cref="Peers"/>
		public static readonly GridMap[] PeerMaps;

		/// <summary>
		/// <para>
		/// Indicates all maps that forms the each intersection. The pattern will be like:
		/// <code>
		/// .-------.-------.-------.
		/// | C C C | A A A | A A A |
		/// | B B B | . . . | . . . |
		/// | B B B | . . . | . . . |
		/// '-------'-------'-------'
		/// </code>
		/// </para>
		/// <para>
		/// In addition, in this data structure, "<c>_coverSet</c>" is a block and "<c>_baseSet</c>" is a line.
		/// </para>
		/// </summary>
		public static readonly IReadOnlyDictionary<(byte, byte), (GridMap, GridMap, GridMap)> IntersectionMaps;


		/// <summary>
		/// Get the region index for the specified cell and the region type.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <param name="label">The label.</param>
		/// <returns>The region index (<c>0..27</c>).</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetRegion(int cell, RegionLabel label) => (
			label switch
			{
				Row => RowTable,
				Column => ColumnTable,
				Block => BlockTable,
				_ => throw Throwings.ImpossibleCase
			})[cell];

		/// <summary>
		/// Get the name in the specified region.
		/// </summary>
		/// <param name="region">The region.</param>
		/// <returns>The name.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetLabel(int region) => ((RegionLabel)(region / 9)).ToString();

		/// <summary>
		/// Get cells with the specified mask, which consist of 9 bits and 1 is
		/// for yielding.
		/// </summary>
		/// <param name="region">The region.</param>
		/// <param name="mask">The mask.</param>
		/// <returns>The cells.</returns>
		public static IEnumerable<int> GetCells(int region, short mask)
		{
			for (int i = 0, t = mask; i < 9; i++, t >>= 1)
			{
				if ((t & 1) != 0)
				{
					yield return RegionCells[region][i];
				}
			}
		}

		/// <include file='..\GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
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
		/// <param name="values">
		/// (<see langword="out"/> parameter) The map of all cells that is the given or modifiable value,
		/// and the digit is the specified one.
		/// </param>
		public static void Deconstruct(
			this Grid @this, out GridMap empty, out GridMap bivalue,
			out GridMap[] candidates, out GridMap[] digits, out GridMap[] values) =>
			(empty, bivalue, candidates, digits, values) = (
				@this.GetEmptyCellsMap(), @this.GetBivalueCellsMap(),
				@this.GetCandidatesMap(), @this.GetDigitsMap(), @this.GetValuesMap());

		/// <summary>
		/// Get the map of all empty cells in this grid.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <returns>The map.</returns>
		private static GridMap GetEmptyCellsMap(this Grid @this)
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
		private static GridMap GetBivalueCellsMap(this Grid @this)
		{
			var result = GridMap.Empty;
			for (int cell = 0; cell < 81; cell++)
			{
				if (@this.GetCandidateMask(cell).CountSet() == 2)
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
		private static GridMap[] GetCandidatesMap(this Grid @this)
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
		/// Different with <see cref="GetCandidatesMap(Grid)"/>,
		/// this method will get all cells that contain the digit or fill this digit
		/// (given or modifiable).
		/// </para>
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <returns>The map.</returns>
		/// <seealso cref="GetCandidatesMap(Grid)"/>
		private static GridMap[] GetDigitsMap(this Grid @this)
		{
			var result = new GridMap[9];
			for (int digit = 0; digit < 9; digit++)
			{
				ref var map = ref result[digit];
				for (int cell = 0; cell < 81; cell++)
				{
					if ((@this.GetCandidateMask(cell) >> digit & 1) != 0)
					{
						map.Add(cell);
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Get the map of all distributions for digits.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <returns>The map.</returns>
		private static GridMap[] GetValuesMap(this Grid @this)
		{
			var result = new GridMap[9];
			for (int digit = 0; digit < 9; digit++)
			{
				ref var map = ref result[digit];
				for (int cell = 0; cell < 81; cell++)
				{
					if (@this[cell] == digit)
					{
						map.Add(cell);
					}
				}
			}

			return result;
		}
	}
}
