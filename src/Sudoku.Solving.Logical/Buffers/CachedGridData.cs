namespace Sudoku.Buffers;

/// <summary>
/// <para>
/// Represents cached fields used by solving and analyzation for a sudoku puzzle,
/// as cached fields that can prevent with redundant and repeated calculations and initialization.
/// </para>
/// <para>
/// All names of fields given in this type can also be found in <see cref="Grid"/>, but they will be calculated if you use them.
/// If you use such fields in a same grid, it will produce redundant calculations.
/// </para>
/// <para>
/// Some step searchers may rely on this type.
/// <b>If you want to use them, you should ensure the method <see cref="InitializeMaps"/> must be called before using such fields</b>.
/// </para>
/// </summary>
/// <seealso cref="InitializeMaps"/>
/// <seealso cref="LogicalSolver"/>
/// <seealso cref="Grid"/>
internal static class CachedGridData
{
	/// <summary>
	/// <inheritdoc cref="Grid.EmptyCells"/>
	/// </summary>
	/// <remarks>
	/// This map <b>should</b> be used after <see cref="InitializeMaps"/> called, and you<b>'d better</b>
	/// not use this field on instances which are marked the attribute
	/// <see cref="StepSearcherMetadataAttribute.IsDirect"/>.
	/// </remarks>
	/// <seealso cref="InitializeMaps"/>
	/// <seealso cref="StepSearcherMetadataAttribute.IsDirect"/>
	internal static CellMap EmptyCells;

	/// <summary>
	/// <inheritdoc cref="Grid.BivalueCells"/>
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="EmptyCells" path="/remarks"/>
	/// </remarks>
	internal static CellMap BivalueCells;

#nullable disable warnings
	/// <summary>
	/// <inheritdoc cref="Grid.CandidatesMap"/>
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="EmptyCells" path="/remarks"/>
	/// </remarks>
	/// <exception cref="NullReferenceException">Throws when not initialized.</exception>
	internal static CellMap[] CandidatesMap;

	/// <summary>
	/// <inheritdoc cref="Grid.DigitsMap"/>
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="EmptyCells" path="/remarks"/>
	/// </remarks>
	/// <exception cref="NullReferenceException">Throws when not initialized.</exception>
	internal static CellMap[] DigitsMap;

	/// <summary>
	/// <inheritdoc cref="Grid.ValuesMap"/>
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="EmptyCells" path="/remarks"/>
	/// </remarks>
	/// <exception cref="NullReferenceException">Throws when not initialized.</exception>
	internal static CellMap[] ValuesMap;
#nullable restore warnings


	/// <summary>
	/// Initialize the maps that used later.
	/// </summary>
	/// <param name="grid">The grid.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void InitializeMaps(scoped in Grid grid)
		=> (EmptyCells, BivalueCells, CandidatesMap, DigitsMap, ValuesMap) = (grid.EmptyCells, grid.BivalueCells, grid.CandidatesMap, grid.DigitsMap, grid.ValuesMap);
}
