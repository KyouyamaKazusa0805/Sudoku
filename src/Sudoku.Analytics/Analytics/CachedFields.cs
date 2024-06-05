namespace Sudoku.Analytics;

/// <summary>
/// <para>Represents cached fields used by solving and analyzing for a sudoku puzzle, reducing repeated and redundant calculations.</para>
/// <para>
/// All names of fields given in this type can also be found in type <see cref="Grid"/>, but those values are defined as instant properties,
/// which means the value will be calculated after you invoke them.
/// For example, if we use a property of them for 5 times, the result value will be calculated for 5 times.
/// </para>
/// <para>
/// Sometimes, such values may only be calculated only once, and we just use those return values after the value had been calculated.
/// For the consideration of the calculation result caching, those results will be stored here as <see langword="static"/> fields,
/// we can use them if we can ensure that such fields have already been initialized.
/// For calling the method <see cref="Initialize(ref readonly Grid, ref readonly Grid)"/>, we can ensure those fields are initialized, in order to be used later.
/// In other words,
/// <b>you must ensure the method <see cref="Initialize(ref readonly Grid, ref readonly Grid)"/> having been called before using such fields if you want to use them</b>;
/// otherwise, <see cref="NullReferenceException"/> will be thrown for cached fields whose types are reference ones.
/// </para>
/// <para>
/// The reason why I defined them as <see langword="static"/> fields is that they are cached values.
/// "<see langword="static"/>" means caching, and "field" means the value to be fetched without encapsulation,
/// for a better performance.
/// </para>
/// <para>Some <see cref="StepSearcher"/>s may rely on this type.</para>
/// </summary>
/// <seealso cref="Initialize(ref readonly Grid, ref readonly Grid)"/>
/// <seealso cref="Analyzer"/>
/// <seealso cref="StepSearcher"/>
/// <seealso cref="Grid"/>
public static class CachedFields
{
	/// <summary>
	/// <inheritdoc cref="Grid.EmptyCells"/>
	/// </summary>
	/// <remarks>
	/// This map <b>should</b> be used after <see cref="Initialize(ref readonly Grid, ref readonly Grid)"/> called, and you<b>'d better</b>
	/// not use this field on instances which are set <see langword="true"/> for property <see cref="StepSearcherAttribute.IsCachingSafe"/>.
	/// </remarks>
	/// <seealso cref="Initialize(ref readonly Grid, ref readonly Grid)"/>
	/// <seealso cref="StepSearcherAttribute.IsCachingSafe"/>
	internal static CellMap EmptyCells;

	/// <summary>
	/// <inheritdoc cref="Grid.BivalueCells"/>
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="EmptyCells" path="/remarks"/>
	/// </remarks>
	internal static CellMap BivalueCells;

	/// <summary>
	/// <inheritdoc cref="Grid.CandidatesMap"/>
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="EmptyCells" path="/remarks"/>
	/// </remarks>
	/// <exception cref="NullReferenceException">Throws when not initialized.</exception>
	internal static CellMap[] CandidatesMap = null!;

	/// <summary>
	/// <inheritdoc cref="Grid.DigitsMap"/>
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="EmptyCells" path="/remarks"/>
	/// </remarks>
	/// <exception cref="NullReferenceException">Throws when not initialized.</exception>
	internal static CellMap[] DigitsMap = null!;

	/// <summary>
	/// <inheritdoc cref="Grid.ValuesMap"/>
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="EmptyCells" path="/remarks"/>
	/// </remarks>
	/// <exception cref="NullReferenceException">Throws when not initialized.</exception>
	internal static CellMap[] ValuesMap = null!;

	/// <summary>
	/// Indicates the solution.
	/// </summary>
	internal static Grid Solution;


	/// <summary>
	/// Initialize the maps that used later.
	/// </summary>
	/// <param name="g">The grid.</param>
	/// <param name="s">The solution of <paramref name="g"/>.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Initialize(ref readonly Grid g, ref readonly Grid s)
	{
		EmptyCells = g.EmptyCells;
		BivalueCells = g.BivalueCells;
		CandidatesMap = [.. g.CandidatesMap];
		DigitsMap = [.. g.DigitsMap];
		ValuesMap = [.. g.ValuesMap];
		Solution = s;
	}
}
