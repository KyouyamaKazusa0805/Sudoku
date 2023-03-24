namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Region (House) Forcing Chains</b> technique that is compatible with program <b>Sudoku Explainer</b>.
/// </summary>
public sealed class RegionForcingChainsStep(
	Conclusion[] conclusions,
	View[]? views,
	int houseIndex,
	byte digit,
	MultipleForcingChains chains,
	bool isDynamic,
	int dynamicNestingLevel = 0
) : ChainingStep(conclusions, views, isMultiple: true, isDynamic: isDynamic, dynamicNestingLevel: dynamicNestingLevel)
{
	internal RegionForcingChainsStep(
		Conclusion[] conclusions,
		int houseIndex,
		byte digit,
		MultipleForcingChains chains,
		bool isDynamic,
		int dynamicNestingLevel = 0
	) : this(conclusions, null!, houseIndex, digit, chains, isDynamic, dynamicNestingLevel)
	{
	}

	internal RegionForcingChainsStep(RegionForcingChainsStep @base, View[]? views) :
		this(@base.Conclusions, views, @base.HouseIndex, @base.Digit, @base.Chains, @base.IsDynamic, @base.DynamicNestingLevel)
	{
	}


	/// <summary>
	/// Indicates the digit of the chain bound with.
	/// </summary>
	public byte Digit { get; } = digit;

	/// <summary>
	/// Indicates the index of the house represented.
	/// </summary>
	public int HouseIndex { get; } = houseIndex;

	/// <summary>
	/// Indicates all possible branches in this technique.
	/// </summary>
	public MultipleForcingChains Chains { get; } = chains;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { DigitStr, HouseStr } }, { "zh", new[] { HouseStr, DigitStr } } };

	private string DigitStr => (Digit + 1).ToString();

	private string HouseStr => $"{char.ToLower(HouseIndex.ToHouseType().ToString()[0])}{HouseIndex % 9 + 1}";


	/// <inheritdoc/>
	protected override CandidateMap GetGreenPotentials(int viewIndex)
		=> viewIndex >= FlatViewsCount ? GetNestedGreenPotentials(viewIndex) : GetColorCandidates(GetPotentialAt(viewIndex), true, true);

	/// <inheritdoc/>
	protected override CandidateMap GetRedPotentials(int viewIndex)
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
	private ChainNode GetPotentialAt(int viewIndex) => Chains[viewIndex].Potential;
}
