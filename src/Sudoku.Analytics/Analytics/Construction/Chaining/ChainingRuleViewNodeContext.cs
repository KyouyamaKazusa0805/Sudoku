namespace Sudoku.Analytics.Construction.Chaining;

/// <summary>
/// Represents the properties used while chaining.
/// </summary>
/// <param name="grid">The grid to be checked.</param>
/// <param name="pattern">The pattern.</param>
/// <param name="view">The view.</param>
[StructLayout(LayoutKind.Auto)]
[TypeImpl(TypeImplFlags.AllObjectMethods)]
public ref partial struct ChainingRuleViewNodeContext(
	[Field(Accessibility = "public", NamingRule = NamingRules.Property)] in Grid grid,
	[Field(Accessibility = "public", NamingRule = NamingRules.Property)] Chain pattern,
	[Field(Accessibility = "public", NamingRule = NamingRules.Property)] View view
) : IContext
{
	/// <summary>
	/// Indicates the currently used almost locked set index.
	/// </summary>
	public int CurrentAlmostLockedSetIndex { get; set; }

	/// <summary>
	/// Indicates the view nodes produced.
	/// </summary>
	public ReadOnlySpan<ViewNode> ProducedViewNodes { get; set; }

	/// <inheritdoc/>
	readonly ref readonly Grid IContext.Grid => ref Grid;
}
