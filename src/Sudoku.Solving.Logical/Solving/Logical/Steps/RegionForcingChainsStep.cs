namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is an <b>Region (House) Forcing Chains</b> technique that is compatible with program <b>Sudoku Explainer</b>.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="HouseIndex">Indicates the index of the house represented.</param>
/// <param name="Digit">Indicates the digit of the chain bound with.</param>
/// <param name="Chains">Indicates all possible branches in this technique.</param>
/// <param name="IsDynamic"><inheritdoc/></param>
/// <param name="DynamicNestingLevel"><inheritdoc/></param>
internal sealed record RegionForcingChainsStep(
	ConclusionList Conclusions,
	int HouseIndex,
	byte Digit,
	MultipleForcingChains Chains,
	bool IsDynamic,
	int DynamicNestingLevel
) : ChainingStep(Conclusions, IsMultiple: true, IsDynamic: IsDynamic, DynamicNestingLevel: DynamicNestingLevel)
{
	[ResourceTextFormatter]
	internal string DigitStr() => (Digit + 1).ToString();

	[ResourceTextFormatter]
	internal string HouseStr() => $"{char.ToLower(HouseIndex.ToHouseType().ToString()[0])}{HouseIndex % 9 + 1}";

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
