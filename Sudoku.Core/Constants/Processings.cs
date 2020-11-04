using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
using Sudoku.Extensions;
using static Sudoku.Constants.RegionLabel;
using static Sudoku.Data.CellStatus;

namespace Sudoku.Constants
{
	/// <summary>
	/// The tables for grid processing. All fields will be initialized in
	/// the static constructor.
	/// </summary>
	public static partial class Processings
	{
		/// <summary>
		/// Get the region index for the specified cell and the region type.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <param name="label">The label.</param>
		/// <returns>The region index (<c>0..27</c>).</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetRegion(int cell, RegionLabel label) =>
		(
			label switch
			{
				Row => RowTable,
				Column => ColumnTable,
				Block => BlockTable,
				_ => throw Throwings.ImpossibleCase
			}
		)[cell];

		/// <summary>
		/// Get the label in the specified region.
		/// </summary>
		/// <param name="region">The region.</param>
		/// <returns>The region label.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RegionLabel GetLabel(int region) => (RegionLabel)(region / 9);

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

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">(<see langword="this in"/> parameter) The grid.</param>
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
			this in SudokuGrid @this, out GridMap empty, out GridMap bivalue,
			out GridMap[] candidates, out GridMap[] digits, out GridMap[] values) =>
			(empty, bivalue, candidates, digits, values) = (
				@this.GetEmptyCellsMap(), @this.GetBivalueCellsMap(),
				@this.GetCandidatesMap(), @this.GetDigitsMap(), @this.GetValuesMap());

		/// <summary>
		/// Get the map of all empty cells in this grid.
		/// </summary>
		/// <param name="this">(<see langword="this in"/> parameter) The grid.</param>
		/// <returns>The map.</returns>
		private static GridMap GetEmptyCellsMap(this in SudokuGrid @this)
		{
			var result = GridMap.Empty;
			for (int cell = 0; cell < 81; cell++)
			{
				if (@this.GetStatus(cell) == Empty)
				{
					result.AddAnyway(cell);
				}
			}

			return result;
		}

		/// <summary>
		/// Get the map of all bi-value cells in this grid.
		/// </summary>
		/// <param name="this">(<see langword="this in"/> parameter) The grid.</param>
		/// <returns>The map.</returns>
		private static GridMap GetBivalueCellsMap(this in SudokuGrid @this)
		{
			var result = GridMap.Empty;
			for (int cell = 0; cell < 81; cell++)
			{
				if (@this.GetCandidateMask(cell).PopCount() == 2)
				{
					result.AddAnyway(cell);
				}
			}

			return result;
		}

		/// <summary>
		/// Get the map of all distributions for digits.
		/// </summary>
		/// <param name="this">(<see langword="this in"/> parameter) The grid.</param>
		/// <returns>The map.</returns>
		private static GridMap[] GetCandidatesMap(this in SudokuGrid @this)
		{
			var result = new GridMap[9];
			for (int digit = 0; digit < 9; digit++)
			{
				ref var map = ref result[digit];
				for (int cell = 0; cell < 81; cell++)
				{
					if (@this.Exists(cell, digit) is true)
					{
						map.AddAnyway(cell);
					}
				}
			}

			return result;
		}

		/// <summary>
		/// <para>Get the map of all distributions for digits.</para>
		/// <para>
		/// Different with <see cref="GetCandidatesMap(in SudokuGrid)"/>,
		/// this method will get all cells that contain the digit or fill this digit
		/// (given or modifiable).
		/// </para>
		/// </summary>
		/// <param name="this">(<see langword="this in"/> parameter) The grid.</param>
		/// <returns>The map.</returns>
		/// <seealso cref="GetCandidatesMap(in SudokuGrid)"/>
		private static GridMap[] GetDigitsMap(this in SudokuGrid @this)
		{
			var result = new GridMap[9];
			for (int digit = 0; digit < 9; digit++)
			{
				ref var map = ref result[digit];
				for (int cell = 0; cell < 81; cell++)
				{
					if ((@this.GetCandidateMask(cell) >> digit & 1) != 0)
					{
						map.AddAnyway(cell);
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Get the map of all distributions for digits.
		/// </summary>
		/// <param name="this">(<see langword="this in"/> parameter) The grid.</param>
		/// <returns>The map.</returns>
		private static GridMap[] GetValuesMap(this in SudokuGrid @this)
		{
			var result = new GridMap[9];
			for (int digit = 0; digit < 9; digit++)
			{
				ref var map = ref result[digit];
				for (int cell = 0; cell < 81; cell++)
				{
					if (@this[cell] == digit)
					{
						map.AddAnyway(cell);
					}
				}
			}

			return result;
		}
	}
}
