namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Blossom Loop</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="houseIndex">Indicates the index of the house represented.</param>
/// <param name="digit">Indicates the digit of the chain bound with.</param>
/// <param name="chains">Indicates all possible branches in this loop.</param>
public sealed partial class BlossomLoopStep(
	Conclusion[] conclusions,
	View[]? views,
	[PrimaryConstructorParameter] House houseIndex,
	[PrimaryConstructorParameter] byte digit,
	[PrimaryConstructorParameter] MultipleForcingChains chains
) : Step(conclusions, views)
{
	internal BlossomLoopStep(Conclusion[] conclusions, House houseIndex, byte digit, MultipleForcingChains chains) :
		this(conclusions, null!, houseIndex, digit, chains)
	{
	}

	internal BlossomLoopStep(BlossomLoopStep @base, View[]? views) : this(@base.Conclusions, views, @base.HouseIndex, @base.Digit, @base.Chains)
	{
	}


	/// <inheritdoc/>
	public override decimal BaseDifficulty => 8.0M;

	/// <inheritdoc/>
	public override Technique Code => Technique.BlossomLoop;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => new[] { (ExtraDifficultyCaseNames.Length, LengthDifficulty) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { DigitStr, HouseStr } }, { "zh", new[] { HouseStr, DigitStr } } };

	/// <summary>
	/// Indicates the total length difficulty.
	/// </summary>
	private decimal LengthDifficulty => ChainDifficultyRating.GetExtraDifficultyByLength(Chains.Potentials.Sum(ChainingStep.AncestorsCountOf));

	private string DigitStr => (Digit + 1).ToString();

	private string HouseStr => $"{char.ToLower(HouseIndex.ToHouseType().ToString()[0])}{HouseIndex % 9 + 1}";


	/// <summary>
	/// To create views via the specified values.
	/// </summary>
	/// <returns>The values.</returns>
	internal View[]? CreateViews()
	{
		var result = View.Empty;
		for (var i = 0; i < Chains.Count; i++)
		{
			GetOffPotentials(i).ForEach(candidate => result.Add(new CandidateViewNode(WellKnownColorIdentifierKind.Auxiliary1, candidate)));
			GetOnPotentials(i).ForEach(candidate => result.Add(new CandidateViewNode(WellKnownColorIdentifierKind.Normal, candidate)));
			result.AddRange(GetLinks(i));
		}

		return new[] { result };
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


	/// <summary>
	/// Compares two <see cref="BlossomLoopStep"/> instance, and returns an <see cref="int"/>
	/// indicating which value is greater.
	/// </summary>
	/// <param name="left">The left-side value to be compared.</param>
	/// <param name="right">The right-side value to be compared.</param>
	/// <returns>An <see cref="int"/> value indicating which is greater.</returns>
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compare(BlossomLoopStep left, BlossomLoopStep right) => Sign(left.Difficulty - right.Difficulty);
}
