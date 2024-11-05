namespace Sudoku.Behaviors.Diff.Results;

/// <summary>
/// Represents a difference that describes a list of candidate digits is changed.
/// </summary>
/// <param name="candidates"><inheritdoc path="/param[@name='candidates']"/></param>
[IntroducedSince(3, 4)]
[method: JsonConstructor]
public sealed class ChangedCandidateDiffResult(CandidateMap candidates) : ChangedDiffResult(candidates)
{
	/// <inheritdoc/>
	public override string NotationPrefix => "C^";

	/// <inheritdoc/>
	public override CellState CellType => CellState.Given;

	/// <inheritdoc/>
	public override DiffType Type => DiffType.ChangedGiven;

	/// <inheritdoc/>
	protected override string CellTypeString => "Candidate";


	/// <inheritdoc/>
	public override ChangedCandidateDiffResult Clone() => new(Candidates);
}
