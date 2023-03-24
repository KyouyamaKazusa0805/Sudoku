namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bidirectional Cycle</b> technique that is compatible with program <b>Sudoku Explainer</b>.
/// </summary>
public sealed class BidirectionalCycleStep(Conclusion[] conclusions, View[]? views, ChainNode destinationOn, bool isX, bool isY) :
	ChainingStep(conclusions, views, isX, isY)
{
	internal BidirectionalCycleStep(Conclusion[] conclusions, ChainNode destinationOn, bool isX, bool isY) :
		this(conclusions, null!, destinationOn, isX, isY)
	{
	}

	internal BidirectionalCycleStep(BidirectionalCycleStep @base, View[]? views) :
		this(@base.Conclusions, views, @base.DestinationOn, @base.IsX, @base.IsY)
	{
	}


	/// <summary>
	/// Indicates the destination node that is "on" status.
	/// </summary>
	public ChainNode DestinationOn { get; } = destinationOn;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { CandsStr } }, { "zh", new[] { CandsStr } } };

	private string CandsStr => RxCyNotation.ToCandidatesString(CandidateMap.Empty + from element in Conclusions select element.Candidate);


	/// <inheritdoc/>
	protected override CandidateMap GetGreenPotentials(int viewIndex) => GetColorCandidates(DestinationOn, true, false);

	/// <inheritdoc/>
	protected override CandidateMap GetRedPotentials(int viewIndex) => GetColorCandidates(DestinationOn, false, true);

	/// <inheritdoc/>
	protected override List<LinkViewNode> GetLinks(int viewIndex) => GetLinks(DestinationOn);
}
