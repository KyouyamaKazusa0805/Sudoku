namespace Sudoku.Analytics.Construction.Chaining;

/// <summary>
/// Represents the properties used while chaining.
/// </summary>
/// <param name="currentNode">Indicates the current node.</param>
/// <param name="grid">The grid to be checked.</param>
/// <param name="options">Indicates the step searcher options to be used.</param>
[StructLayout(LayoutKind.Auto)]
[TypeImpl(TypeImplFlags.AllObjectMethods)]
public ref partial struct ChainingRuleNextNodeContext(
	[Field(Accessibility = "public", NamingRule = NamingRules.Property)] Node currentNode,
	[Field(Accessibility = "public", NamingRule = NamingRules.Property)] ref readonly Grid grid,
	[Field(Accessibility = "public", NamingRule = NamingRules.Property)] StepGathererOptions options
) : IContext
{
	/// <summary>
	/// Indicates the collected nodes.
	/// </summary>
	public ReadOnlySpan<Node> Nodes { get; set; }

	/// <inheritdoc/>
	readonly ref readonly Grid IContext.Grid => ref Grid;


	/// <summary>
	/// Try to get the link option for the specified link type.
	/// </summary>
	/// <param name="linkType">The link type.</param>
	/// <returns>
	/// The link option returned.
	/// If there's no overridden link option, return <see cref="StepGathererOptions.DefaultLinkOption"/>.
	/// </returns>
	/// <seealso cref="StepGathererOptions.DefaultLinkOption"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly LinkOption GetLinkOption(LinkType linkType)
		=> Options.OverriddenLinkOptions is { } p && p.TryGetValue(linkType, out var lo) ? lo : Options.DefaultLinkOption;
}
