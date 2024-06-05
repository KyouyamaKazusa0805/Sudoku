namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Region (House) Forcing Chains</b> technique that is compatible with program <b>Sudoku Explainer</b>.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="houseIndex">Indicates the index of the house represented.</param>
/// <param name="digit">Indicates the digit of the chain bound with.</param>
/// <param name="chains">Indicates all possible branches in this technique.</param>
/// <param name="isDynamic"><inheritdoc/></param>
/// <param name="dynamicNestingLevel"><inheritdoc/></param>
[Obsolete]
public sealed partial class RegionForcingChainsStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] House houseIndex,
	[PrimaryConstructorParameter] byte digit,
	[PrimaryConstructorParameter] MultipleForcingChains chains,
	bool isDynamic,
	int dynamicNestingLevel = 0
) : ChainingStep(conclusions, views, options, isMultiple: true, isDynamic: isDynamic, dynamicNestingLevel: dynamicNestingLevel)
{
	internal RegionForcingChainsStep(
		Conclusion[] conclusions,
		StepSearcherOptions options,
		House houseIndex,
		byte digit,
		MultipleForcingChains chains,
		bool isDynamic,
		int dynamicNestingLevel = 0
	) : this(conclusions, null!, options, houseIndex, digit, chains, isDynamic, dynamicNestingLevel)
	{
	}

	internal RegionForcingChainsStep(RegionForcingChainsStep @base, View[]? views) :
		this(@base.Conclusions, views, @base.Options, @base.HouseIndex, @base.Digit, @base.Chains, @base.IsDynamic, @base.DynamicNestingLevel)
	{
	}


	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [DigitStr, HouseStr]), new(ChineseLanguage, [HouseStr, DigitStr])];

	private string DigitStr => Options.Converter.DigitConverter((Mask)(1 << Digit));

	private string HouseStr => Options.Converter.HouseConverter(1 << HouseIndex);


	/// <inheritdoc/>
	protected internal override View[] CreateViews(ref readonly Grid grid)
	{
		var result = base.CreateViews(in grid);
		return [[.. result[0], new HouseViewNode(ColorIdentifier.Normal, HouseIndex)], .. result[1..]];
	}

	/// <inheritdoc/>
	protected override CandidateMap GetOnPotentials(int viewIndex)
		=> viewIndex >= FlatViewsCount ? GetNestedOnPotentials(viewIndex) : GetColorCandidates(GetPotentialAt(viewIndex), true, true);

	/// <inheritdoc/>
	protected override CandidateMap GetOffPotentials(int viewIndex)
		=> viewIndex >= FlatViewsCount ? GetNestedOffPotentials(viewIndex) : GetColorCandidates(GetPotentialAt(viewIndex), false, false);

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
