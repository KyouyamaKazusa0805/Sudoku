using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Rating;
using Sudoku.Rendering;
using Sudoku.Rendering.Nodes;
using Sudoku.Text;
using Sudoku.Text.Notation;

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
	[DataMember] House houseIndex,
	[DataMember] byte digit,
	[DataMember] MultipleForcingChains chains
) : Step(conclusions, views), IComparableStep<BlossomLoopStep>
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
	public override ExtraDifficultyCase[] ExtraDifficultyCases => [new(ExtraDifficultyCaseNames.Length, LengthDifficulty)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [DigitStr, HouseStr]), new(ChineseLanguage, [HouseStr, DigitStr])];

	/// <summary>
	/// Indicates the total length difficulty.
	/// </summary>
	private decimal LengthDifficulty => ChainDifficultyRating.GetExtraDifficultyByLength(Chains.Potentials.Sum(ChainingStep.AncestorsCountOf));

	private string DigitStr => DigitNotation.ToString((Digit)Digit);

	private string HouseStr => $"{char.ToLower(HouseIndex.ToHouseType().ToString()[0])}{HouseIndex % 9 + 1}";


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
				eachBranchView.Add(new CandidateViewNode(WellKnownColorIdentifier.Auxiliary1, candidate));
				full.Add(new CandidateViewNode(WellKnownColorIdentifier.Auxiliary1, candidate));
			}

			foreach (var candidate in GetOnPotentials(i))
			{
				eachBranchView.Add(new CandidateViewNode(WellKnownColorIdentifier.Normal, candidate));
				full.Add(new CandidateViewNode(WellKnownColorIdentifier.Normal, candidate));
			}

			eachBranchView.AddRange(GetLinks(i));
			full.AddRange(GetLinks(i));

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static int IComparableStep<BlossomLoopStep>.Compare(BlossomLoopStep left, BlossomLoopStep right) => Sign(left.Difficulty - right.Difficulty);
}
