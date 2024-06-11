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
internal abstract partial class ChainingRule
{
	/// <summary>
	/// Collects for both strong links and weak links appeared in argument <paramref name="grid"/>
	/// and insert all found links into <paramref name="strongLinks"/> and <paramref name="weakLinks"/> respectively.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="strongLinks">The dictionary that stores a list of strong links.</param>
	/// <param name="weakLinks">The dictionary that stores a list of weak links.</param>
	internal abstract void CollectLinks(ref readonly Grid grid, LinkDictionary strongLinks, LinkDictionary weakLinks);

	/// <summary>
	/// Collects for extra view nodes for the pattern.
	/// This method will be useful in advanced chaining rules such as ALS, AHS and AUR extra maps checking.
	/// </summary>
	/// <param name="grid">The grid as candidate references.</param>
	/// <param name="pattern">The pattern to collect view nodes.</param>
	/// <param name="views">The views.</param>
	/// <returns>A list of <see cref="ViewNode"/> instances.</returns>
	/// <remarks>
	/// The method by default will do nothing.
	/// </remarks>
	internal virtual void CollectExtraViewNodes(ref readonly Grid grid, ChainPattern pattern, ref View[] views)
	{
		// Do nothing.
	}

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
	internal virtual ConclusionSet CollectLoopConclusions(Loop loop, ref readonly Grid grid) => [];
}
