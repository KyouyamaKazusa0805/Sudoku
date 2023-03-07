namespace Sudoku.Buffers;

/// <summary>
/// <para>
/// Represents fast properties used by solving and analyzation for a sudoku puzzle,
/// as cached properties that can prevent with redundant and repeated calculations and initialization.
/// </para>
/// <para>
/// All names of properties given in this type can also be found in <see cref="Grid"/>, but they will be calculated if you use them.
/// If you use such properties in a same grid, it will produce redundant calculations.
/// </para>
/// <para>
/// Some step searchers may rely on this type.
/// <b>If you want to use them, you should ensure the method <see cref="InitializeMaps"/> must be called before using such properties</b>.
/// </para>
/// </summary>
/// <seealso cref="InitializeMaps"/>
/// <seealso cref="LogicalSolver"/>
/// <seealso cref="Grid"/>
internal static class FastProperties
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
	public static CellMap EmptyCells { get; private set; }

	/// <summary>
	/// <inheritdoc cref="Grid.BivalueCells"/>
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="EmptyCells" path="/remarks"/>
	/// </remarks>
	public static CellMap BivalueCells { get; private set; }

	/// <summary>
	/// <inheritdoc cref="Grid.CandidatesMap"/>
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="EmptyCells" path="/remarks"/>
	/// </remarks>
	/// <exception cref="NullReferenceException">Throws when not initialized.</exception>
	public static CellMap[] CandidatesMap { get; private set; } = null!;

	/// <summary>
	/// <inheritdoc cref="Grid.DigitsMap"/>
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="EmptyCells" path="/remarks"/>
	/// </remarks>
	/// <exception cref="NullReferenceException">Throws when not initialized.</exception>
	public static CellMap[] DigitsMap { get; private set; } = null!;

	/// <summary>
	/// <inheritdoc cref="Grid.ValuesMap"/>
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="EmptyCells" path="/remarks"/>
	/// </remarks>
	/// <exception cref="NullReferenceException">Throws when not initialized.</exception>
	public static CellMap[] ValuesMap { get; private set; } = null!;


	/// <summary>
	/// Initialize the maps that used later.
	/// </summary>
	/// <param name="grid">The grid.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void InitializeMaps(scoped in Grid grid)
		=> (EmptyCells, BivalueCells, CandidatesMap, DigitsMap, ValuesMap) = (grid.EmptyCells, grid.BivalueCells, grid.CandidatesMap, grid.DigitsMap, grid.ValuesMap);
}
