using Sudoku.Data;
using Sudoku.Solving.Manual.Singles;

namespace Sudoku.Solving
{
	partial class TechniqueSearcher
	{
		/// <summary>
		/// The empty cells map.
		/// </summary>
		/// <remarks>
		/// This map <b>should</b> be used after <see cref="InitializeMaps"/> called, and you<b>'d better</b>
		/// not use this field on <see cref="SingleTechniqueSearcher"/> instance.
		/// </remarks>
		/// <seealso cref="InitializeMaps(in SudokuGrid)"/>
		/// <seealso cref="SingleTechniqueSearcher"/>
		internal static GridMap EmptyMap { get; set; }

		/// <summary>
		/// The bi-value cells map.
		/// </summary>
		/// <remarks>
		/// This map <b>should</b> be used after <see cref="InitializeMaps"/> called, and you<b>'d better</b>
		/// not use this field on <see cref="SingleTechniqueSearcher"/> instance.
		/// </remarks>
		/// <seealso cref="InitializeMaps(in SudokuGrid)"/>
		/// <seealso cref="SingleTechniqueSearcher"/>
		internal static GridMap BivalueMap { get; set; }

		/// <summary>
		/// The candidate maps.
		/// </summary>
		/// <remarks>
		/// This map <b>should</b> be used after <see cref="InitializeMaps"/> called, and you<b>'d better</b>
		/// not use this field on <see cref="SingleTechniqueSearcher"/> instance.
		/// </remarks>
		/// <seealso cref="InitializeMaps(in SudokuGrid)"/>
		/// <seealso cref="SingleTechniqueSearcher"/>
		internal static GridMap[] CandMaps { get; set; } = null!;

		/// <summary>
		/// The digit maps.
		/// </summary>
		/// <remarks>
		/// This map <b>should</b> be used after <see cref="InitializeMaps"/> called, and you<b>'d better</b>
		/// not use this field on <see cref="SingleTechniqueSearcher"/> instance.
		/// </remarks>
		/// <seealso cref="InitializeMaps(in SudokuGrid)"/>
		/// <seealso cref="SingleTechniqueSearcher"/>
		internal static GridMap[] DigitMaps { get; set; } = null!;

		/// <summary>
		/// The value maps.
		/// </summary>
		/// <remarks>
		/// This map <b>should</b> be used after <see cref="InitializeMaps"/> called, and you<b>'d better</b>
		/// not use this field on <see cref="SingleTechniqueSearcher"/> instance.
		/// </remarks>
		/// <seealso cref="InitializeMaps(in SudokuGrid)"/>
		internal static GridMap[] ValueMaps { get; set; } = null!;
	}
}
