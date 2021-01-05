using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Solving.Manual.Singles;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Provides and encapsulates the fast properties that is used in solving and analyzing a sudoku puzzle.
	/// </summary>
	internal static class FastProperties
	{
		/// <summary>
		/// The empty cells map.
		/// </summary>
		/// <remarks>
		/// This map <b>should</b> be used after <see cref="InitializeMaps"/> called, and you<b>'d better</b>
		/// not use this field on <see cref="SingleStepSearcher"/> instance.
		/// </remarks>
		/// <seealso cref="InitializeMaps(in SudokuGrid)"/>
		/// <seealso cref="SingleStepSearcher"/>
		public static Cells EmptyMap { get; private set; }

		/// <summary>
		/// The bi-value cells map.
		/// </summary>
		/// <remarks>
		/// This map <b>should</b> be used after <see cref="InitializeMaps"/> called, and you<b>'d better</b>
		/// not use this field on <see cref="SingleStepSearcher"/> instance.
		/// </remarks>
		/// <seealso cref="InitializeMaps(in SudokuGrid)"/>
		/// <seealso cref="SingleStepSearcher"/>
		public static Cells BivalueMap { get; private set; }

		/// <summary>
		/// The candidate maps.
		/// </summary>
		/// <remarks>
		/// This map <b>should</b> be used after <see cref="InitializeMaps"/> called, and you<b>'d better</b>
		/// not use this field on <see cref="SingleStepSearcher"/> instance.
		/// </remarks>
		/// <seealso cref="InitializeMaps(in SudokuGrid)"/>
		/// <seealso cref="SingleStepSearcher"/>
		public static Cells[] CandMaps { get; private set; } = null!;

		/// <summary>
		/// The digit maps.
		/// </summary>
		/// <remarks>
		/// This map <b>should</b> be used after <see cref="InitializeMaps"/> called, and you<b>'d better</b>
		/// not use this field on <see cref="SingleStepSearcher"/> instance.
		/// </remarks>
		/// <seealso cref="InitializeMaps(in SudokuGrid)"/>
		/// <seealso cref="SingleStepSearcher"/>
		public static Cells[] DigitMaps { get; private set; } = null!;

		/// <summary>
		/// The value maps.
		/// </summary>
		/// <remarks>
		/// This map <b>should</b> be used after <see cref="InitializeMaps"/> called, and you<b>'d better</b>
		/// not use this field on <see cref="SingleStepSearcher"/> instance.
		/// </remarks>
		/// <seealso cref="InitializeMaps(in SudokuGrid)"/>
		public static Cells[] ValueMaps { get; private set; } = null!;


		/// <summary>
		/// Initialize the maps that used later.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		public static void InitializeMaps(in SudokuGrid grid)
		{
			var (e, b, c, d, v) = grid;
			EmptyMap = e;
			BivalueMap = b;
			CandMaps = c;
			DigitMaps = d;
			ValueMaps = v;
		}
	}
}
