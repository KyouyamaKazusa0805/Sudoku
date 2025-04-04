namespace Sudoku.Analytics.Construction.Chaining;

/// <summary>
/// Represents the properties used while chaining.
/// </summary>
/// <param name="grid">The grid to be checked.</param>
/// <param name="links">The links inside the loop or loop-like pattern.</param>
[StructLayout(LayoutKind.Auto)]
[TypeImpl(TypeImplFlags.AllObjectMethods)]
public ref partial struct ChainingRuleLoopConclusionContext(
	[Field(Accessibility = "public", NamingRule = NamingRules.Property)] in Grid grid,
	[Field(Accessibility = "public", NamingRule = NamingRules.Property)] ReadOnlySpan<Link> links
)
{
	/// <summary>
	/// Indicates the found conclusions.
	/// </summary>
	public ConclusionSet Conclusions { get; set; } = ConclusionSet.Empty;
}
