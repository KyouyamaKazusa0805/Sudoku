namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bidirectional Cycle</b> technique that is compatible with program <b>Sudoku Explainer</b>.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="destinationOn">Indicates the destination node that is "on" status.</param>
/// <param name="isX"><inheritdoc/></param>
/// <param name="isY"><inheritdoc/></param>
public sealed partial class BidirectionalCycleStep(
	Conclusion[] conclusions,
	View[]? views,
	[PrimaryConstructorParameter] ChainNode destinationOn,
	bool isX,
	bool isY
) : ChainingStep(conclusions, views, isX, isY)
{
	internal BidirectionalCycleStep(Conclusion[] conclusions, ChainNode destinationOn, bool isX, bool isY) :
		this(conclusions, null!, destinationOn, isX, isY)
	{
	}

	internal BidirectionalCycleStep(BidirectionalCycleStep @base, View[]? views) :
		this(@base.Conclusions, views, @base.DestinationOn, @base.IsX, @base.IsY)
	{
	}


	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts => [new(EnglishLanguage, [CandsStr]), new(ChineseLanguage, [CandsStr])];

	private string CandsStr => RxCyNotation.ToCandidatesString([.. from element in Conclusions select element.Candidate]);


	/// <inheritdoc/>
	protected override CandidateMap GetOnPotentials(int viewIndex) => GetColorCandidates(DestinationOn, true, false);

	/// <inheritdoc/>
	protected override CandidateMap GetOffPotentials(int viewIndex) => GetColorCandidates(DestinationOn, false, true);

	/// <inheritdoc/>
	protected override List<LinkViewNode> GetLinks(int viewIndex) => GetLinks(DestinationOn);
}
