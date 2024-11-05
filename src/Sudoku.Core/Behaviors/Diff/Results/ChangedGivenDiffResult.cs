namespace Sudoku.Behaviors.Diff.Results;

/// <summary>
/// Represents a difference that describes a list of given digits is changed.
/// </summary>
/// <param name="candidates"><inheritdoc path="/param[@name='candidates']"/></param>
[IntroducedSince(3, 4)]
[method: JsonConstructor]
public sealed class ChangedGivenDiffResult(CandidateMap candidates) : ChangedDiffResult(candidates)
{
	/// <inheritdoc/>
	public override string NotationPrefix => "G^";

	/// <inheritdoc/>
	public override CellState CellType => CellState.Given;

	/// <inheritdoc/>
	public override DiffType Type => DiffType.ChangedGiven;


	/// <inheritdoc/>
	public override ChangedGivenDiffResult Clone() => new(Candidates);
}
