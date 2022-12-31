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
	IReadOnlyDictionary<byte, Potential> Chains,
	bool IsDynamic,
	int DynamicNestingLevel
) : ChainingStep(Conclusions, IsMultiple: true, IsDynamic: IsDynamic, DynamicNestingLevel: DynamicNestingLevel)
{
	/// <inheritdoc/>
	public override int SortKey => 6;

	/// <inheritdoc/>
	public override int FlatComplexity => Chains.Values.Sum(AncestorsCountOf);

	/// <inheritdoc/>
	public override decimal Difficulty => BaseDifficultyNonAlternatingInference + LengthDifficulty;

	/// <inheritdoc/>
	public override Technique TechniqueCode => IsDynamic ? Technique.DynamicRegionForcingChains : Technique.RegionForcingChains;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => TechniqueTags.LongChaining | TechniqueTags.ForcingChains;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.HardlyEver;

	/// <inheritdoc/>
	protected override int FlatViewsCount => Chains.Count;

	/// <inheritdoc/>
	protected override Potential Result => Chains.Values.First();


	/// <inheritdoc/>
	protected internal override List<Potential> GetChainsTargets() => Chains.Values.ToList();

	/// <inheritdoc/>
	protected override Potential GetChainTargetAt(int viewIndex) => Chains.Values.ElementAt(viewIndex);

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
