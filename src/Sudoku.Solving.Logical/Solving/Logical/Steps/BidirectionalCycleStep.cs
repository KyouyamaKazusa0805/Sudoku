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
	[ResourceTextFormatter]
	internal string CandsStr() => RxCyNotation.ToCandidatesString(Candidates.Empty + from element in Conclusions select element.Candidate);

	/// <inheritdoc/>
	protected override Candidates GetGreenPotentials(int viewNum) => GetColorCandidates(DestinationOn, true, false);

	/// <inheritdoc/>
	protected override Candidates GetRedPotentials(int viewNum) => GetColorCandidates(DestinationOn, false, true);

	/// <inheritdoc/>
	protected override List<LinkViewNode> GetLinks(int viewIndex) => GetLinks(DestinationOn);
}
