using System.Runtime.CompilerServices;
using Sudoku.Extensions;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.CellStatus;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="SudokuGrid"/>.
	/// </summary>
	/// <seealso cref="SudokuGrid"/>
	public static class SudokuGridEx
	{
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
			@this.GetStatus(cell) == Empty ? @this[cell, digit] : null;

		/// <summary>
		/// Check whether the digit will be duplicate of its peers when it is filled in the specified cell.
		/// </summary>
		/// <param name="this">(<see langword="this in"/> parameter) The grid.</param>
		/// <param name="cell">The cell.</param>
		/// <param name="digit">The digit.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public static bool Duplicate(this in SudokuGrid @this, int cell, int digit)
		{
			unsafe
			{
				static bool duplicate(int c, in SudokuGrid grid, in int digit) => grid[c] == digit;
				return PeerMaps[cell].Any(&duplicate, @this, digit);
			}
		}
	}
}
