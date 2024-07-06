namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Represents the properties used while chaining.
/// </summary>
/// <param name="grid">The grid to be checked.</param>
/// <param name="links">The links inside the loop or loop-like pattern.</param>
[TypeImpl(TypeImplFlag.AllObjectMethods)]
public readonly ref partial struct ChainingRuleLoopConclusionCollectingContext(
	[PrimaryConstructorParameter(MemberKinds.Field, Accessibility = "public", NamingRule = ">@")] ref readonly Grid grid,
	[PrimaryConstructorParameter(MemberKinds.Field, Accessibility = "public", NamingRule = ">@")] ReadOnlySpan<Link> links
) : IChainingRuleContext
{
	/// <inheritdoc/>
	readonly ref readonly Grid IChainingRuleContext.Grid => ref Grid;
}
