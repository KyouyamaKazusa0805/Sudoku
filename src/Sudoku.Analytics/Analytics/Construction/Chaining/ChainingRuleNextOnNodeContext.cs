namespace Sudoku.Analytics.Construction.Chaining;

/// <summary>
/// Represents the properties used while chaining.
/// </summary>
/// <param name="currentNode">Indicates the current node.</param>
/// <param name="grid">Indicates the current grid.</param>
/// <param name="originalGrid">Indicates the original grid to be used.</param>
/// <param name="nodesSupposedOff">Indicates the nodes supposed to be "off".</param>
/// <param name="options">Indicates the step searcher options to be used.</param>
[StructLayout(LayoutKind.Auto)]
[TypeImpl(TypeImplFlags.AllObjectMethods)]
public ref partial struct ChainingRuleNextOnNodeContext(
	[Field(Accessibility = "public", NamingRule = NamingRules.Property)] Node currentNode,
	[Field(Accessibility = "public", NamingRule = NamingRules.Property)] ref readonly Grid grid,
	[Field(Accessibility = "public", NamingRule = NamingRules.Property)] ref readonly Grid originalGrid,
	[Field(Accessibility = "public", NamingRule = NamingRules.Property)] HashSet<Node> nodesSupposedOff,
	[Field(Accessibility = "public", NamingRule = NamingRules.Property)] StepGathererOptions options
) : IChainingRuleNextNodeContext
{
	/// <inheritdoc/>
	public ReadOnlySpan<Node> Nodes { get; set; }

	/// <inheritdoc/>
	readonly ref readonly Grid IContext.Grid => ref Grid;

	/// <inheritdoc/>
	readonly Node IChainingRuleNextNodeContext.CurrentNode => CurrentNode;

	/// <inheritdoc/>
	readonly StepGathererOptions IChainingRuleNextNodeContext.Options => Options;
}
