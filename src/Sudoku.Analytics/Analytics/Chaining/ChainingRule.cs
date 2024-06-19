namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Represents a rule that make inferences (strong or weak) between two <see cref="Node"/> instances.
/// </summary>
/// <seealso cref="Node"/>
[TypeImpl(
	TypeImplFlag.AllObjectMethods,
	EqualsBehavior = EqualsBehavior.ThrowNotSupportedException,
	OtherModifiersOnEquals = "sealed",
	GetHashCodeBehavior = GetHashCodeBehavior.ThrowNotSupportedException,
	OtherModifiersOnGetHashCode = "sealed",
	ToStringBehavior = ToStringBehavior.ThrowNotSupportedException,
	OtherModifiersOnToString = "sealed")]
public abstract partial class ChainingRule
{
	/// <summary>
	/// Indicates the elementary link types.
	/// </summary>
	public static readonly LinkType[] ElementaryLinkTypes = [LinkType.SingleDigit, LinkType.SingleCell];

	/// <summary>
	/// Indicates the advanced link types.
	/// </summary>
	public static readonly LinkType[] AdvancedLinkTypes = [
		LinkType.LockedCandidates,
		LinkType.AlmostLockedSet,
		LinkType.KrakenNormalFish,
		LinkType.AlmostUniqueRectangle,
		LinkType.AlmostAvoidableRectangle
	];


	/// <summary>
	/// Collects for both strong links and weak links appeared in argument <paramref name="grid"/>
	/// and insert all found links into <paramref name="strongLinks"/> and <paramref name="weakLinks"/> respectively.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="strongLinks">The dictionary that stores a list of strong links.</param>
	/// <param name="weakLinks">The dictionary that stores a list of weak links.</param>
	/// <param name="linkOption">
	/// An option that limits the collecting method rule for strong and weak links.
	/// By default the value is <see cref="LinkOption.Intersection"/>.
	/// </param>
	/// <param name="alsLinkOption">
	/// <para>
	/// An option that limits the collecting method rule for links <see cref="LinkType.AlmostLockedSet"/>.
	/// By default the value is <see cref="LinkOption.House"/>.
	/// </para>
	/// <para>
	/// This type of link is special. Most of cases we use all strong links in an ALS pattern.
	/// Therefore, the option should be configured individually.
	/// </para>
	/// </param>
	protected internal abstract void CollectLinks(
		ref readonly Grid grid,
		LinkDictionary strongLinks,
		LinkDictionary weakLinks,
		LinkOption linkOption,
		LinkOption alsLinkOption
	);

	/// <summary>
	/// Collects for extra view nodes for the pattern.
	/// This method will be useful in advanced chaining rules such as ALS and AUR extra maps checking.
	/// </summary>
	/// <param name="grid">The grid as candidate references.</param>
	/// <param name="pattern">The pattern to collect view nodes.</param>
	/// <param name="view">A <see cref="View"/> instance that is applied for view nodes appended.</param>
	/// <param name="nodes">A list of <see cref="ViewNode"/> that is created from this method.</param>
	/// <remarks>
	/// The method by default will do nothing, with an empty <see cref="ReadOnlySpan{T}"/> returned
	/// from argument <paramref name="nodes"/>.
	/// </remarks>
	/// <seealso cref="View"/>
	protected internal virtual void CollectExtraViewNodes(ref readonly Grid grid, ChainOrLoop pattern, View view, out ReadOnlySpan<ViewNode> nodes)
		=> nodes = [];

	/// <summary>
	/// Try to find extra eliminations that can only be created inside a Grouped Continuous Nice Loop.
	/// This method will be useful in advanced chaining rules such as ALS, AHS and AUR eliminations checking.
	/// </summary>
	/// <param name="loop">Indicates the base loop to be used.</param>
	/// <param name="grid">The grid to be checked.</param>
	/// <returns>A list of found conclusions.</returns>
	/// <remarks>
	/// This method should not be overridden if no eliminations exists in the loop pattern.
	/// </remarks>
	protected internal virtual ConclusionSet CollectLoopConclusions(Loop loop, ref readonly Grid grid) => [];

	/// <summary>
	/// Collects for extra view nodes for the pattern on multiple forcing chains.
	/// This method will be useful in advanced chaining rules such as ALS, and AUR extra maps checking.
	/// </summary>
	/// <param name="grid">The grid as candidate references.</param>
	/// <param name="pattern">The pattern to collect view nodes.</param>
	/// <param name="views">The <see cref="View"/> instances to be updated.</param>
	/// <seealso cref="View"/>
	internal void CollectExtraViewNodes(ref readonly Grid grid, MultipleForcingChains pattern, View[] views)
	{
		var viewIndex = 1;
		foreach (var branch in pattern.Values)
		{
			CollectExtraViewNodes(in grid, branch, views[viewIndex++], out var nodes);
			views[0].AddRange(nodes);
		}
	}
}
