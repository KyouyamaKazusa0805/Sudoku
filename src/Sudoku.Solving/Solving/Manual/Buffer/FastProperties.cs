#nullable disable warnings

namespace Sudoku.Solving.Manual.Buffer;

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
/// <see cref="InitializeMaps"/> in this class is called before using these properties.
/// </para>
/// <para>
/// If a step searcher doesn't rely on this class, it'll be <see langword="true"/> for the property named
/// <see cref="StepSearcherAttribute.IsDirect"/>.
/// If you find that the step searcher marks that attribute,
/// you can call it everywhere safely; however, if the searcher isn't marked that attribute, you
/// can't call it everywhere unless you call the method <see cref="InitializeMaps"/>.
/// </para>
/// <para>
/// Please note, all names of properties in this class can also be found in <see cref="Grid"/>,
/// which means you can also call the property by <see cref="Grid"/>. Of course, some of them
/// doesn't contain the same one in <see cref="Grid"/>, but you can find a same property whose
/// execution logic (handling logic) is totally same.
/// For example, <see cref="EmptyMap"/> is same as <see cref="Grid.EmptyCells"/>. The difference
/// between them is that you shouldn't use <see cref="EmptyMap"/> until you have called
/// <see cref="InitializeMaps"/>, while <see cref="Grid.EmptyCells"/> can be used
/// everywhere, because it isn't an instant property (which means the calculation begins
/// when you called them, i.e. lazy ones; in contrast, some properties only store values directly,
/// so their values can be got instantly, i.e. instant ones).
/// </para>
/// </remarks>
/// <seealso cref="InitializeMaps"/>
/// <seealso cref="ManualSolver"/>
/// <seealso cref="StepSearcherAttribute.IsDirect"/>
/// <seealso cref="Grid"/>
internal static class FastProperties
{
	/// <summary>
	/// The empty cells map.
	/// </summary>
	/// <remarks>
	/// This map <b>should</b> be used after <see cref="InitializeMaps"/> called, and you<b>'d better</b>
	/// not use this field on instances which are marked the attribute
	/// <see cref="StepSearcherAttribute.IsDirect"/>.
	/// </remarks>
	/// <seealso cref="InitializeMaps"/>
	/// <seealso cref="StepSearcherAttribute.IsDirect"/>
	public static Cells EmptyMap { get; private set; }

	/// <summary>
	/// The bi-value cells map.
	/// </summary>
	/// <remarks>
	/// This map <b>should</b> be used after <see cref="InitializeMaps"/> called, and you<b>'d better</b>
	/// not use this field on instances which are marked the attribute
	/// <see cref="StepSearcherAttribute.IsDirect"/>.
	/// </remarks>
	/// <seealso cref="InitializeMaps"/>
	/// <seealso cref="StepSearcherAttribute.IsDirect"/>
	public static Cells BivalueMap { get; private set; }

	/// <summary>
	/// The candidate maps.
	/// </summary>
	/// <remarks>
	/// This map <b>should</b> be used after <see cref="InitializeMaps"/> called, and you<b>'d better</b>
	/// not use this field on instances which are marked the attribute
	/// <see cref="StepSearcherAttribute.IsDirect"/>.
	/// </remarks>
	/// <seealso cref="InitializeMaps"/>
	/// <seealso cref="StepSearcherAttribute.IsDirect"/>
	public static Cells[] CandMaps { get; private set; }

	/// <summary>
	/// The digit maps.
	/// </summary>
	/// <remarks>
	/// This map <b>should</b> be used after <see cref="InitializeMaps"/> called, and you<b>'d better</b>
	/// not use this field on instances which are marked the attribute
	/// <see cref="StepSearcherAttribute.IsDirect"/>.
	/// </remarks>
	/// <seealso cref="InitializeMaps"/>
	/// <seealso cref="StepSearcherAttribute.IsDirect"/>
	public static Cells[] DigitMaps { get; private set; }

	/// <summary>
	/// The value maps.
	/// </summary>
	/// <remarks>
	/// This map <b>should</b> be used after <see cref="InitializeMaps"/> called, and you<b>'d better</b>
	/// not use this field on instances which are marked the attribute
	/// <see cref="StepSearcherAttribute.IsDirect"/>.
	/// </remarks>
	/// <seealso cref="InitializeMaps"/>
	/// <seealso cref="StepSearcherAttribute.IsDirect"/>
	public static Cells[] ValueMaps { get; private set; }


	/// <summary>
	/// Initialize the maps that used later.
	/// </summary>
	/// <param name="grid">The grid.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[MemberNotNull(nameof(CandMaps), nameof(DigitMaps), nameof(ValueMaps))]
	public static void InitializeMaps(in Grid grid) => (EmptyMap, BivalueMap, CandMaps, DigitMaps, ValueMaps) = grid;
}
