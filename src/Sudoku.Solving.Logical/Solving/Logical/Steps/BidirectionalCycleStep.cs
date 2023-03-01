namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Bidirectional Cycle</b> technique that is compatible with program <b>Sudoku Explainer</b>.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="DestinationOn">Indicates the destination node that is "on" status.</param>
/// <param name="IsX"><inheritdoc/></param>
/// <param name="IsY"><inheritdoc/></param>
internal sealed record BidirectionalCycleStep(ConclusionList Conclusions, ChainNode DestinationOn, bool IsX, bool IsY) :
	ChainingStep(Conclusions, IsX, IsY)
{
	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { CandsStr } }, { "zh", new[] { CandsStr } } };

	private string CandsStr => RxCyNotation.ToCandidatesString(CandidateMap.Empty + from element in Conclusions select element.Candidate);


	/// <inheritdoc/>
	protected override CandidateMap GetGreenPotentials(int viewNum) => GetColorCandidates(DestinationOn, true, false);

	/// <inheritdoc/>
	protected override CandidateMap GetRedPotentials(int viewNum) => GetColorCandidates(DestinationOn, false, true);

	/// <inheritdoc/>
	protected override List<LinkViewNode> GetLinks(int viewIndex) => GetLinks(DestinationOn);
}
