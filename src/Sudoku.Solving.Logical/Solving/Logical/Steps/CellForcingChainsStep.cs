namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is an <b>Cell Forcing Chains</b> technique that is compatible with program <b>Sudoku Explainer</b>.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="SourceCell">Indicates the source cell that all branches start.</param>
/// <param name="Chains">Indicates all possible branches in this technique.</param>
/// <param name="IsDynamic"><inheritdoc/></param>
/// <param name="DynamicNestingLevel"><inheritdoc/></param>
internal sealed record CellForcingChainsStep(
	ConclusionList Conclusions,
	byte SourceCell,
	IReadOnlyDictionary<byte, Potential> Chains,
	bool IsDynamic,
	int DynamicNestingLevel
) : ChainingStep(Conclusions, IsMultiple: true, IsDynamic: IsDynamic, DynamicNestingLevel: DynamicNestingLevel)
{
	/// <inheritdoc/>
	protected override Candidates GetGreenPotentials(int viewIndex)
		=> viewIndex >= FlatViewsCount
			? GetNestedGreenPotentials(viewIndex)
			: GetColorCandidates(Chains[Chains.Keys.ElementAt(viewIndex)], true, true);

	/// <inheritdoc/>
	protected override Candidates GetRedPotentials(int viewIndex)
		=> viewIndex >= FlatViewsCount
			? GetNestedRedPotentials(viewIndex)
			: GetColorCandidates(Chains[Chains.Keys.ElementAt(viewIndex)], false, false);

	/// <inheritdoc/>
	protected override List<LinkViewNode> GetLinks(int viewIndex)
		=> viewIndex >= FlatViewsCount ? GetNestedLinks(viewIndex) : GetLinks(Chains[Chains.Keys.ElementAt(viewIndex)]);
}
