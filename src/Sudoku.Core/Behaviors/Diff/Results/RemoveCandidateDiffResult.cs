namespace Sudoku.Behaviors.Diff.Results;

/// <summary>
/// Represents a difference that describes a list of candidate digits is removed.
/// </summary>
/// <param name="candidates"><inheritdoc path="/param[@name='candidates']"/></param>
[method: JsonConstructor]
public sealed class RemoveCandidateDiffResult(CandidateMap candidates) : RemoveDiffResult(candidates)
{
	/// <inheritdoc/>
	public override string NotationPrefix => "C-";

	/// <inheritdoc/>
	public override CellState CellType => CellState.Empty;

	/// <inheritdoc/>
	public override DiffType Type => DiffType.RemoveCandidate;

	/// <inheritdoc/>
	protected override string CellTypeString => "Candidate";


	/// <inheritdoc/>
	public override RemoveCandidateDiffResult Clone() => new(Candidates);
}
