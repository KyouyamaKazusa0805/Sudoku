namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Represents the properties used while chaining.
/// </summary>
/// <param name="grid">The grid to be checked.</param>
/// <param name="pattern">The pattern.</param>
/// <param name="view">The view.</param>
[StructLayout(LayoutKind.Auto)]
[TypeImpl(TypeImplFlag.AllObjectMethods)]
public ref partial struct ChainingRuleViewNodesMappingContext(
	[PrimaryConstructorParameter(MemberKinds.Field, Accessibility = "public", NamingRule = ">@")] ref readonly Grid grid,
	[PrimaryConstructorParameter(MemberKinds.Field, Accessibility = "public", NamingRule = ">@")] ChainOrLoop pattern,
	[PrimaryConstructorParameter(MemberKinds.Field, Accessibility = "public", NamingRule = ">@")] View view
) : IChainingRuleContext
{
	/// <summary>
	/// Indicates the view nodes produced.
	/// </summary>
	public ReadOnlySpan<ViewNode> ProducedViewNodes { get; set; }

	/// <inheritdoc/>
	readonly ref readonly Grid IChainingRuleContext.Grid => ref Grid;
}
