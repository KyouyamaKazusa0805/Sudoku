namespace Sudoku.Behaviors.Diff.Results;

/// <summary>
/// Represents a difference that describes a list of candidate digits are added.
/// </summary>
/// <param name="candidates"><inheritdoc path="/param[@name='candidates']"/></param>
/// <param name="areCorrect"><inheritdoc path="/param[@name='areCorrect']"/></param>
[IntroducedSince(3, 4)]
[method: JsonConstructor]
public sealed class AddCandidateDiffResult(CandidateMap candidates, bool areCorrect) : AddDiffResult(candidates, areCorrect)
{
	/// <inheritdoc/>
	public override string NotationPrefix => "C+";

	/// <inheritdoc/>
	public override CellState CellType => CellState.Empty;

	/// <inheritdoc/>
	public override DiffType Type => DiffType.AddCandidate;

	/// <inheritdoc/>
	protected override string CellTypeString => nameof(Candidate);


	/// <inheritdoc/>
	public override AddCandidateDiffResult Clone() => new(Candidates, AreCorrect);
}
