namespace Sudoku.Analytics.Construction.Chaining;

/// <summary>
/// Represents a rule that make inferences (strong or weak) between two <see cref="Node"/> instances.
/// </summary>
/// <seealso cref="Node"/>
[TypeImpl(
	TypeImplFlags.AllObjectMethods,
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
		LinkType.AlmostLockedSets,
		//LinkType.KrakenNormalFish,
		LinkType.UniqueRectangle_SameDigit,
		LinkType.UniqueRectangle_DifferentDigit,
		LinkType.UniqueRectangle_SingleSideExternal,
		LinkType.UniqueRectangle_DoubleSideExternal,
		LinkType.AvoidableRectangle,
		//LinkType.XyzWing,
	];


	/// <summary>
	/// Collects for both strong links and weak links appeared <see cref="ChainingRuleLinkContext.Grid"/>
	/// and insert all found links into <see cref="ChainingRuleLinkContext.StrongLinks"/>
	/// and <see cref="ChainingRuleLinkContext.WeakLinks"/> respectively.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <seealso cref="ChainingRuleLinkContext"/>
	[Cached]
	public abstract void GetLinks(ref ChainingRuleLinkContext context);

	/// <summary>
	/// Collects nodes that is supposed to "on" from the current node supposed to "off",
	/// and store them into <see cref="ChainingRuleNextOnNodeContext.Nodes"/>.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <seealso cref="ChainingRuleNextOnNodeContext"/>
	public virtual void CollectOnNodes(ref ChainingRuleNextOnNodeContext context) => context.Nodes = [];

	/// <summary>
	/// Collects nodes that is supposed to "off" from the current node supposed to "on",
	/// and store them into <see cref="ChainingRuleNextOffNodeContext.Nodes"/>.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <seealso cref="ChainingRuleNextOffNodeContext"/>
	public virtual void CollectOffNodes(ref ChainingRuleNextOffNodeContext context) => context.Nodes = [];

	/// <summary>
	/// Collects for extra view nodes for the pattern.
	/// This method will be useful in advanced chaining rules such as ALS and AUR extra maps checking.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <remarks>
	/// The method by default will do nothing, with an empty sequence
	/// assigned to <see cref="ChainingRuleViewNodeContext.ProducedViewNodes"/>
	/// </remarks>
	/// <seealso cref="ChainingRuleViewNodeContext.ProducedViewNodes"/>
	public virtual void GetViewNodes(ref ChainingRuleViewNodeContext context) => context.ProducedViewNodes = [];

	/// <summary>
	/// Try to find extra eliminations that can only be created inside a Grouped Continuous Nice Loop.
	/// This method will be useful in advanced chaining rules such as ALS, AHS and AUR eliminations checking.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <returns>A list of found conclusions.</returns>
	/// <remarks>
	/// This method should not be overridden if no eliminations exists in the loop pattern.
	/// </remarks>
	[Cached]
	public virtual void GetLoopConclusions(ref ChainingRuleLoopConclusionContext context) => context.Conclusions = ConclusionSet.Empty;
}
