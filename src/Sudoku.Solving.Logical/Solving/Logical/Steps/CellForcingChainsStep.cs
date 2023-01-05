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
	MultipleForcingChains Chains,
	bool IsDynamic,
	int DynamicNestingLevel = 0
) : ChainingStep(Conclusions, IsMultiple: true, IsDynamic: IsDynamic, DynamicNestingLevel: DynamicNestingLevel)
{
	[ResourceTextFormatter]
	internal string CellStr() => RxCyNotation.ToCellString(SourceCell);

	/// <inheritdoc/>
	protected override Candidates GetGreenPotentials(int viewIndex)
		=> viewIndex >= FlatViewsCount ? GetNestedGreenPotentials(viewIndex) : GetColorCandidates(GetPotentialAt(viewIndex), true, true);

	/// <inheritdoc/>
	protected override Candidates GetRedPotentials(int viewIndex)
		=> viewIndex >= FlatViewsCount ? GetNestedRedPotentials(viewIndex) : GetColorCandidates(GetPotentialAt(viewIndex), false, false);

	/// <inheritdoc/>
	protected override List<LinkViewNode> GetLinks(int viewIndex)
		=> viewIndex >= FlatViewsCount ? GetNestedLinks(viewIndex) : GetLinks(GetPotentialAt(viewIndex));

	/// <summary>
	/// Gets the potential at the specified index.
	/// </summary>
	/// <param name="viewIndex">The view index.</param>
	/// <returns>The view index.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Potential GetPotentialAt(int viewIndex) => Chains[viewIndex].Potential;
}
