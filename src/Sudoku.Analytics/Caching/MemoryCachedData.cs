namespace Sudoku.Caching;

/// <summary>
/// Represents cached fields used by solving and analyzing for a sudoku puzzle, reducing repeated and redundant calculations.
/// </summary>
internal static class MemoryCachedData
{
	/// <summary>
	/// The backing field storing on strong links.
	/// </summary>
	public static readonly LinkDictionary StrongLinkDictionary = [];

	/// <summary>
	/// The backing field storing on weak links.
	/// </summary>
	public static readonly LinkDictionary WeakLinkDictionary = [];


	/// <summary>
	/// The backing field storing on strong links, grouped by link type.
	/// </summary>
	public static LinkType StrongLinkTypesEntried = LinkType.Unknown;

	/// <summary>
	/// The backing field storing on weak links, grouped by link type.
	/// </summary>
	public static LinkType WeakLinkTypesEntried = LinkType.Unknown;

	/// <summary>
	/// Indicates the number of candidates appeared in the puzzle.
	/// </summary>
	public static Candidate CandidatesCount;

	/// <summary>
	/// <inheritdoc cref="Grid.EmptyCells"/>
	/// </summary>
	/// <remarks>
	/// This map <b>should</b> be used after <see cref="Initialize(ref readonly Grid, ref readonly Grid)"/> called, and you<b>'d better</b>
	/// not use this field on instances which are set <see langword="true"/> for property <see cref="StepSearcherAttribute.IsCachingSafe"/>.
	/// </remarks>
	/// <seealso cref="Initialize(ref readonly Grid, ref readonly Grid)"/>
	/// <seealso cref="StepSearcherAttribute.IsCachingSafe"/>
	public static CellMap EmptyCells;

	/// <summary>
	/// <inheritdoc cref="Grid.BivalueCells"/>
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="EmptyCells" path="/remarks"/>
	/// </remarks>
	public static CellMap BivalueCells;

	/// <summary>
	/// <inheritdoc cref="Grid.CandidatesMap"/>
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="EmptyCells" path="/remarks"/>
	/// </remarks>
	/// <exception cref="NullReferenceException">Throws when not initialized.</exception>
	public static CellMap[] CandidatesMap = null!;

	/// <summary>
	/// <inheritdoc cref="Grid.DigitsMap"/>
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="EmptyCells" path="/remarks"/>
	/// </remarks>
	/// <exception cref="NullReferenceException">Throws when not initialized.</exception>
	public static CellMap[] DigitsMap = null!;

	/// <summary>
	/// <inheritdoc cref="Grid.ValuesMap"/>
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="EmptyCells" path="/remarks"/>
	/// </remarks>
	/// <exception cref="NullReferenceException">Throws when not initialized.</exception>
	public static CellMap[] ValuesMap = null!;

	/// <summary>
	/// Indicates the solution.
	/// </summary>
	/// <remarks><b>
	/// Please note that a puzzle may contain multiple solutions, which will make this field to be <see cref="Grid.Undefined"/>.
	/// You must check validity of this field before using this field.
	/// </b></remarks>
	/// <seealso cref="Grid.Undefined"/>
	public static Grid Solution;


	/// <summary>
	/// Initialize the maps that used later.
	/// </summary>
	/// <param name="g">The grid.</param>
	/// <param name="s">The solution of <paramref name="g"/>.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Initialize(ref readonly Grid g, ref readonly Grid s)
	{
		CandidatesCount = g.CandidatesCount;
		EmptyCells = g.EmptyCells;
		BivalueCells = g.BivalueCells;
		CandidatesMap = [.. g.CandidatesMap];
		DigitsMap = [.. g.DigitsMap];
		ValuesMap = [.. g.ValuesMap];
		Solution = s;

		// Chaining-related fields.
		StrongLinkTypesEntried = LinkType.Unknown;
		WeakLinkTypesEntried = LinkType.Unknown;
		StrongLinkDictionary.Clear();
		WeakLinkDictionary.Clear();
	}

	/// <summary>
	/// Try to collect strong and weak links appeared in a grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="linkTypes">The link types to be checked.</param>
	/// <param name="options">The options used by step searchers.</param>
	/// <param name="rules">The <see cref="ChainingRule"/> instance that collects with strong and weak links if worth.</param>
	public static void InitializeLinks(ref readonly Grid grid, LinkType linkTypes, StepSearcherOptions options, out ChainingRules rules)
	{
		rules = from linkType in linkTypes select ChainingRulePool.TryCreate(linkType)!;
		if (!StrongLinkTypesEntried.HasFlag(linkTypes) || !WeakLinkTypesEntried.HasFlag(linkTypes))
		{
			var (strongDic, weakDic) = (new LinkDictionary(), new LinkDictionary());
			var context = new ChainingRuleLinkContext(in grid, strongDic, weakDic, options);
			foreach (var rule in rules)
			{
				rule.GetLinks(ref context);
			}

			if (strongDic.Count != 0)
			{
				StrongLinkTypesEntried |= linkTypes;
				StrongLinkDictionary.Merge(strongDic);
			}
			if (weakDic.Count != 0)
			{
				WeakLinkTypesEntried |= linkTypes;
				WeakLinkDictionary.Merge(weakDic);
			}
		}
	}
}
