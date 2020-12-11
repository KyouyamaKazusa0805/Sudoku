using System.Extensions;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
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
		/// Get the label in the specified region.
		/// </summary>
		/// <param name="region">The region.</param>
		/// <returns>The region label.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RegionLabel GetLabel(int region) => (RegionLabel)(region / 9);

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
			out GridMap[] candidates, out GridMap[] digits, out GridMap[] values)
		{
			empty = e(@this);
			bivalue = b(@this);
			candidates = c(@this);
			digits = d(@this);
			values = v(@this);

			static GridMap e(in SudokuGrid @this)
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

			static GridMap b(in SudokuGrid @this)
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

			static GridMap[] c(in SudokuGrid @this)
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

			static GridMap[] d(in SudokuGrid @this)
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

			static GridMap[] v(in SudokuGrid @this)
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
}
