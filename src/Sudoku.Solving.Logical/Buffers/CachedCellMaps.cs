namespace Sudoku.Buffers;

/// <summary>
/// <para>Represents cached fields used by solving and analyzation for a sudoku puzzle, reducing repeated and redundant calculations.</para>
/// <para>
/// All names of fields given in this type can also be found in type <see cref="Grid"/>, but those values are defined as instant properties,
/// which means the value will be calculated after you invoke them.
/// For example, if we use a property of them for 5 times, the result value will be calculated for 5 times.
/// </para>
/// <para>
/// Sometimes, such values may only be calculated only once, and we just use those return values after the value had been calculated.
/// For the consideration of the calculation result caching, those results will be stored here as <see langword="static"/> fields,
/// we can use them if we can ensure that such fields have already been initialized.
/// For calling the method <see cref="Initialize(in Grid)"/>, we can ensure those fields are initialized, in order to be used later.
/// In other words,
/// <b>you must ensure the method <see cref="Initialize"/> having been called before using such fields if you want to use them</b>;
/// otherwise, <see cref="NullReferenceException"/> will be thrown for cached fields whose types are reference ones.
/// </para>
/// <para>
/// The reason why I defined them as <see langword="static"/> fields is that they are cached values.
/// "<see langword="static"/>" means caching, and "field" means the value to be fetched without encapsulation,
/// for a better performace.
/// </para>
/// <para>Some <see cref="IStepSearcher"/>s may rely on this type.</para>
/// </summary>
/// <seealso cref="Initialize"/>
/// <seealso cref="LogicalSolver"/>
/// <seealso cref="IStepSearcher"/>
/// <seealso cref="Grid"/>
internal static class CachedCellMaps
{
	/// <summary>
	/// <inheritdoc cref="Grid.EmptyCells"/>
	/// </summary>
	/// <remarks>
	/// This map <b>should</b> be used after <see cref="Initialize"/> called, and you<b>'d better</b>
	/// not use this field on instances which are marked the attribute
	/// <see cref="StepSearcherMetadataAttribute.IsDirect"/>.
	/// </remarks>
	/// <seealso cref="Initialize"/>
	/// <seealso cref="StepSearcherMetadataAttribute.IsDirect"/>
	public static CellMap EmptyCells;

	/// <summary>
	/// <inheritdoc cref="Grid.BivalueCells"/>
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="EmptyCells" path="/remarks"/>
	/// </remarks>
	public static CellMap BivalueCells;

#nullable disable warnings
	/// <summary>
	/// <inheritdoc cref="Grid.CandidatesMap"/>
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="EmptyCells" path="/remarks"/>
	/// </remarks>
	/// <exception cref="NullReferenceException">Throws when not initialized.</exception>
	public static CellMap[] CandidatesMap;

	/// <summary>
	/// <inheritdoc cref="Grid.DigitsMap"/>
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="EmptyCells" path="/remarks"/>
	/// </remarks>
	/// <exception cref="NullReferenceException">Throws when not initialized.</exception>
	public static CellMap[] DigitsMap;

	/// <summary>
	/// <inheritdoc cref="Grid.ValuesMap"/>
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="EmptyCells" path="/remarks"/>
	/// </remarks>
	/// <exception cref="NullReferenceException">Throws when not initialized.</exception>
	public static CellMap[] ValuesMap;
#nullable restore warnings


	/// <summary>
	/// Initialize the maps that used later.
	/// </summary>
	/// <param name="g">The grid.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Initialize(scoped in Grid g)
		=> (EmptyCells, BivalueCells, CandidatesMap, DigitsMap, ValuesMap) = (g.EmptyCells, g.BivalueCells, g.CandidatesMap, g.DigitsMap, g.ValuesMap);
}
