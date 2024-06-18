namespace Sudoku.Caching;

/// <summary>
/// Represents a pool that stores a list of strong and weak links collected inside <see cref="StepSearcher"/>,
/// especially for chaining-related step searchers.
/// </summary>
/// <seealso cref="StepSearcher"/>
internal static class CachedLinkPool
{
	/// <summary>
	/// The backing field storing on strong links, grouped by link type.
	/// </summary>
	public static LinkType StrongLinkTypesEntried = LinkType.Unknown;

	/// <summary>
	/// The backing field storing on weak links, grouped by link type.
	/// </summary>
	public static LinkType WeakLinkTypesEntried = LinkType.Unknown;

	/// <summary>
	/// The backing field storing on strong links.
	/// </summary>
	public static readonly LinkDictionary StrongLinkDictionary = [];

	/// <summary>
	/// The backing field storing on weak links.
	/// </summary>
	public static readonly LinkDictionary WeakLinkDictionary = [];


	/// <summary>
	/// Try to clear elements for <see cref="StrongLinkDictionary"/> and <see cref="WeakLinkDictionary"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void FlushDictionaries()
	{
		StrongLinkTypesEntried = LinkType.Unknown;
		WeakLinkTypesEntried = LinkType.Unknown;
		StrongLinkDictionary.Clear();
		WeakLinkDictionary.Clear();
	}

	/// <summary>
	/// Try to collect strong and weak links appeared inside a grid statically.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="linkTypes">The link types to be checked.</param>
	/// <param name="linkOption">Indicates the applied link option.</param>
	/// <param name="alsLinkOption">Indicates the applied link option that only applied to ALS rules.</param>
	/// <param name="rules">The <see cref="ChainingRule"/> instance that collects with strong and weak links if worth.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Initialize(
		ref readonly Grid grid,
		LinkType linkTypes,
		LinkOption linkOption,
		LinkOption alsLinkOption,
		out ReadOnlySpan<ChainingRule> rules
	)
	{
		rules = from linkType in linkTypes select ChainingRulePool.TryCreate(linkType)!;
		if (!StrongLinkTypesEntried.HasFlag(linkTypes) || !WeakLinkTypesEntried.HasFlag(linkTypes))
		{
			var (strongDic, weakDic) = (new LinkDictionary(), new LinkDictionary());
			foreach (var rule in rules)
			{
				rule.CollectLinks(in grid, strongDic, weakDic, linkOption, alsLinkOption);
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
