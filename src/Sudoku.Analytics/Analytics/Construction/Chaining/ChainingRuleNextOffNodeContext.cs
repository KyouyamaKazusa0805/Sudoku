namespace Sudoku.Analytics.Construction.Chaining;

/// <summary>
/// Represents the properties used while chaining.
/// </summary>
/// <param name="currentNode">Indicates the current node.</param>
/// <param name="grid">Indicates the current grid.</param>
/// <param name="options">Indicates the step searcher options to be used.</param>
[StructLayout(LayoutKind.Auto)]
[TypeImpl(TypeImplFlags.AllObjectMethods)]
public ref partial struct ChainingRuleNextOffNodeContext(
	[Field(Accessibility = "public", NamingRule = NamingRules.Property)] Node currentNode,
	[Field(Accessibility = "public", NamingRule = NamingRules.Property)] in Grid grid,
	[Field(Accessibility = "public", NamingRule = NamingRules.Property)] StepGathererOptions options
)
{
	/// <inheritdoc/>
	public HashSet<Node> Nodes { get; set; } = [];
}
