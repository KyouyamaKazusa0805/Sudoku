namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Blossom Loop</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="houseIndex">Indicates the index of the house represented.</param>
/// <param name="digit">Indicates the digit of the chain bound with.</param>
/// <param name="chains">Indicates all possible branches in this loop.</param>
[Obsolete]
public sealed partial class BlossomLoopStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] House houseIndex,
	[PrimaryConstructorParameter] byte digit,
	[PrimaryConstructorParameter] MultipleForcingChains chains
) : IndirectStep(conclusions, views, options), IComplexChainLengthTrait
{
	internal BlossomLoopStep(Conclusion[] conclusions, StepSearcherOptions options, House houseIndex, byte digit, MultipleForcingChains chains) :
		this(conclusions, null!, options, houseIndex, digit, chains)
	{
	}

	internal BlossomLoopStep(BlossomLoopStep @base, View[]? views) :
		this(@base.Conclusions, views, @base.Options, @base.HouseIndex, @base.Digit, @base.Chains)
	{
	}


	/// <inheritdoc/>
	public override int BaseDifficulty => 80;

	/// <inheritdoc/>
	public override Technique Code => Technique.BlossomLoop;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [DigitStr, HouseStr]), new(ChineseLanguage, [HouseStr, DigitStr])];

	/// <inheritdoc/>
	public override FactorCollection Factors => [new BlossomLoopLengthFactor()];

	/// <inheritdoc/>
	int IComplexChainLengthTrait.ComplexLength => Chains.Potentials.Sum(ChainingStep.AncestorsCountOf);

	private string DigitStr => Options.Converter.DigitConverter((Mask)(1 << Digit));

	private string HouseStr => Options.Converter.HouseConverter(1 << HouseIndex);


	/// <summary>
	/// To create views via the specified values.
	/// </summary>
	/// <returns>The values.</returns>
	internal View[]? CreateViews()
	{
		var full = new View();
		var result = new List<View>(Chains.Count + 1) { full };
		for (var i = 0; i < Chains.Count; i++)
		{
			var eachBranchView = new View();
			foreach (var candidate in GetOffPotentials(i))
			{
				eachBranchView.Add(new CandidateViewNode(ColorIdentifier.Auxiliary1, candidate));
				full.Add(new CandidateViewNode(ColorIdentifier.Auxiliary1, candidate));
			}

			foreach (var candidate in GetOnPotentials(i))
			{
				eachBranchView.Add(new CandidateViewNode(ColorIdentifier.Normal, candidate));
				full.Add(new CandidateViewNode(ColorIdentifier.Normal, candidate));
			}

			eachBranchView.AddRange(GetLinks(i).AsReadOnlySpan());
			full.AddRange(GetLinks(i).AsReadOnlySpan());

			result.Add(eachBranchView);
		}

		return [.. result];
	}

	/// <inheritdoc cref="ChainingStep.GetOnPotentials(int)"/>
	private CandidateMap GetOnPotentials(int viewIndex) => ChainingStep.GetColorCandidates(GetPotentialAt(viewIndex), true, true);

	/// <inheritdoc cref="ChainingStep.GetOffPotentials(int)"/>
	private CandidateMap GetOffPotentials(int viewIndex) => ChainingStep.GetColorCandidates(GetPotentialAt(viewIndex), false, false);

	/// <summary>
	/// Gets the potential at the specified index.
	/// </summary>
	/// <param name="viewIndex">The view index.</param>
	/// <returns>The view index.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ChainNode GetPotentialAt(int viewIndex) => Chains[viewIndex].Potential;

	/// <inheritdoc cref="ChainingStep.GetLinks(int)"/>
	private List<LinkViewNode> GetLinks(int viewIndex) => ChainingStep.GetLinks(GetPotentialAt(viewIndex));


	/// <inheritdoc/>
	public override int CompareTo(Step? other)
		=> other is BlossomLoopStep comparer ? Math.Sign(Difficulty - comparer.Difficulty) : 1;
}
