using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Solving.Manual.Singles;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Provides and encapsulates the fast properties that is used in solving and analyzing a sudoku puzzle.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The whole class is a <see langword="static"/> one, which means you can't use <see langword="new"/>
	/// clause to create a new instance of this type. In contrast, the class is used for providing with
	/// <see langword="static"/> properties used in a cyclic manual searching
	/// (i.e. in <see cref="ManualSolver"/>).
	/// </para>
	/// <para>
	/// Some step searchers rely on this class. If you want to call them alone, you should ensure the method
	/// <see cref="InitializeMaps(in SudokuGrid)"/> in this class is called before using these properties.
	/// </para>
	/// <para>
	/// If a step searcher doesn't rely on this class, it'll be marked an attribute named
	/// <see cref="DirectSearcherAttribute"/>. If you find that the step searcher marks that attribute,
	/// you can call it everywhere safely; however, if the searcher isn't marked that attribute, you
	/// can't call it everywhere unless you call the method <see cref="InitializeMaps(in SudokuGrid)"/>.
	/// </para>
	/// <para>
	/// Please note, all names of properties in this class can also be found in <see cref="SudokuGrid"/>,
	/// which means you can also call the property by <see cref="SudokuGrid"/>. Of course, some of them
	/// doesn't contain the same one in <see cref="SudokuGrid"/>, but you can find a same property whose
	/// execution logic (handling logic) is totally same.
	/// For example, <see cref="EmptyMap"/> is same as <see cref="SudokuGrid.EmptyCells"/>. The difference
	/// between them is that you shouldn't use <see cref="EmptyMap"/> until you have called
	/// <see cref="InitializeMaps(in SudokuGrid)"/>, while <see cref="SudokuGrid.EmptyCells"/> can be used
	/// everywhere, because it isn't an instant property (which means the calculation begins
	/// when you called them, i.e. lazy ones; in contrast, some properties only store values directly,
	/// so their values can be got instantly, i.e. instant ones).
	/// </para>
	/// </remarks>
	/// <seealso cref="ManualSolver"/>
	/// <seealso cref="InitializeMaps(in SudokuGrid)"/>
	/// <seealso cref="DirectSearcherAttribute"/>
	/// <seealso cref="SudokuGrid"/>
	internal static partial class FastProperties
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
		/// <param name="grid">The grid.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InitializeMaps(in SudokuGrid grid) =>
			(EmptyMap, BivalueMap, CandMaps, DigitMaps, ValueMaps) = grid;
	}
}
