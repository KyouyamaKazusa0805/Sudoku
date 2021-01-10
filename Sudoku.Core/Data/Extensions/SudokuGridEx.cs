using System.Runtime.CompilerServices;
using Sudoku.DocComments;
using static Sudoku.Constants.Tables;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="SudokuGrid"/>.
	/// </summary>
	/// <seealso cref="SudokuGrid"/>
	public static class SudokuGridEx
	{
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
			this in SudokuGrid @this, out Cells empty, out Cells bivalue,
			out Cells[] candidates, out Cells[] digits, out Cells[] values)
		{
			empty = @this.EmptyCells;
			bivalue = @this.BivalueCells;
			candidates = @this.CandidateMap;
			digits = @this.DigitsMap;
			values = @this.ValuesMap;
		}

		/// <summary>
		/// Indicates whether the specified grid contains the digit in the specified cell.
		/// </summary>
		/// <param name="this">(<see langword="this in"/> parameter) The grid.</param>
		/// <param name="cell">The cell offset.</param>
		/// <param name="digit">The digit.</param>
		/// <returns>
		/// The method will return a <see cref="bool"/>? value (contains three possible cases:
		/// <see langword="true"/>, <see langword="false"/> and <see langword="null"/>).
		/// All values corresponding to the cases are below:
		/// <list type="table">
		/// <item>
		/// <term><c><see langword="true"/></c></term>
		/// <description>
		/// The cell is an empty cell <b>and</b> contains the specified digit.
		/// </description>
		/// </item>
		/// <item>
		/// <term><c><see langword="false"/></c></term>
		/// <description>
		/// The cell is an empty cell <b>but doesn't</b> contain the specified digit.
		/// </description>
		/// </item>
		/// <item>
		/// <term><c><see langword="null"/></c></term>
		/// <description>The cell is <b>not</b> an empty cell.</description>
		/// </item>
		/// </list>
		/// </returns>
		/// <remarks>
		/// Note that the method will return a <see cref="bool"/>?, so you should use the code
		/// '<c>grid.Exists(candidate) is true</c>' or '<c>grid.Exists(candidate) == true</c>'
		/// to decide whether a condition is true.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool? Exists(this in SudokuGrid @this, int cell, int digit) =>
			@this.GetStatus(cell) == CellStatus.Empty ? @this[cell, digit] : null;

		/// <summary>
		/// Check whether the digit will be duplicate of its peers when it is filled in the specified cell.
		/// </summary>
		/// <param name="this">(<see langword="this in"/> parameter) The grid.</param>
		/// <param name="cell">The cell.</param>
		/// <param name="digit">The digit.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public static bool Duplicate(this in SudokuGrid @this, int cell, int digit)
		{
			foreach (int peerCell in PeerMaps[cell])
			{
				if (@this[peerCell] == digit)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Get the mask that is a result after the bitwise and operation processed all cells
		/// in the specified map.
		/// </summary>
		/// <param name="grid">(<see langword="this in"/> parameter) The grid.</param>
		/// <param name="map">(<see langword="in"/> parameter) The map.</param>
		/// <returns>The result.</returns>
		public static short BitwiseAndMasks(this in SudokuGrid grid, in Cells map)
		{
			short mask = SudokuGrid.MaxCandidatesMask;
			foreach (int cell in map)
			{
				mask &= grid.GetCandidates(cell);
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
		public static short BitwiseOrMasks(this in SudokuGrid grid, in Cells map)
		{
			short mask = 0;
			foreach (int cell in map)
			{
				mask |= grid.GetCandidates(cell);
			}

			return mask;
		}
	}
}
