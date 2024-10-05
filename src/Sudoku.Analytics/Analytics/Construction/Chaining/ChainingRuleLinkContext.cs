namespace Sudoku.Analytics.Construction.Chaining;

/// <summary>
/// Represents the properties used while chaining.
/// </summary>
/// <param name="grid">The grid to be checked.</param>
/// <param name="strongLinks">The dictionary that stores a list of strong links.</param>
/// <param name="weakLinks">The dictionary that stores a list of weak links.</param>
/// <param name="options">Indicates the step searcher options to be used.</param>
[TypeImpl(TypeImplFlag.AllObjectMethods)]
[SuppressMessage("Style", "IDE0250:Make struct 'readonly'", Justification = "<Pending>")]
public ref partial struct ChainingRuleLinkContext(
	[Field(Accessibility = "public", NamingRule = ">@")] ref readonly Grid grid,
	[Field(Accessibility = "public", NamingRule = ">@")] LinkDictionary strongLinks,
	[Field(Accessibility = "public", NamingRule = ">@")] LinkDictionary weakLinks,
	[Field(Accessibility = "public", NamingRule = ">@")] StepGathererOptions options
) : IContext
{
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
