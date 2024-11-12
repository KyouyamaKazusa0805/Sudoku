namespace Sudoku.Analytics.Caching;

/// <summary>
/// Provides with an easy way to create <see cref="ChainingRule"/> instances.
/// </summary>
/// <seealso cref="ChainingRule"/>
internal static class ChainingRulePool
{
	/// <summary>
	/// Indicates the rule router.
	/// </summary>
	private static readonly FrozenDictionary<LinkType, Func<ChainingRule>> RuleRouter = new Dictionary<LinkType, Func<ChainingRule>>
	{
		{ LinkType.SingleDigit, static () => new XChainingRule() },
		{ LinkType.SingleCell, static () => new YChainingRule() },
		{ LinkType.LockedCandidates, static () => new LockedCandidatesChainingRule() },
		{ LinkType.AlmostLockedSets, static () => new AlmostLockedSetsChainingRule() },
		{ LinkType.KrakenNormalFish, static () => new KrakenNormalFishChainingRule() },
		{ LinkType.XyzWing, static () => new XyzWingChainingRule() },
		{ LinkType.AlmostUniqueRectangle, static () => new AlmostUniqueRectangleChainingRule() },
		{ LinkType.AlmostAvoidableRectangle, static () => new AlmostAvoidableRectangleChainingRule() },
	}.ToFrozenDictionary();

	/// <summary>
	/// Indicates the cached rules.
	/// </summary>
	private static readonly Dictionary<LinkType, ChainingRule> CachedRules = [];


	/// <summary>
	/// Clears the cached rules.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void FlushCachedRules() => CachedRules.Clear();

	/// <summary>
	/// Creates a <see cref="ChainingRule"/> instance via the specified link type.
	/// </summary>
	/// <param name="linkType">The link type.</param>
	/// <returns>The created <see cref="ChainingRule"/> instance created.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ChainingRule? TryCreate(LinkType linkType)
	{
		if (CachedRules.TryGetValue(linkType, out var rule))
		{
			return rule;
		}

		if (RuleRouter.TryGetValue(linkType, out var func))
		{
			CachedRules.Remove(linkType);
			var instanceCreated = func();
			CachedRules.Add(linkType, instanceCreated);
			return instanceCreated;
		}

		return null;
	}
}
