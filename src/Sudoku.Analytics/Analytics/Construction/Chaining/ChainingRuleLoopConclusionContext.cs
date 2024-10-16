namespace Sudoku.Analytics.Construction.Chaining;

/// <summary>
/// Represents the properties used while chaining.
/// </summary>
/// <param name="grid">The grid to be checked.</param>
/// <param name="links">The links inside the loop or loop-like pattern.</param>
[StructLayout(LayoutKind.Auto)]
[TypeImpl(TypeImplFlags.AllObjectMethods)]
public ref partial struct ChainingRuleLoopConclusionContext(
	[Field(Accessibility = "public", NamingRule = ">@")] ref readonly Grid grid,
	[Field(Accessibility = "public", NamingRule = ">@")] ReadOnlySpan<Link> links
) : IContext
{
	/// <summary>
	/// Indicates the found conclusions.
	/// </summary>
	public ConclusionSet Conclusions { get; set; } = ConclusionSet.Empty;

	/// <inheritdoc/>
	readonly ref readonly Grid IContext.Grid => ref Grid;
}
