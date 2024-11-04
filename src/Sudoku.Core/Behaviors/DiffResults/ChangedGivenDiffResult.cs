namespace Sudoku.Behaviors.DiffResults;

/// <summary>
/// Represents a difference that describes a list of given digits is changed.
/// </summary>
/// <param name="candidates"><inheritdoc path="/param[@name='candidates']"/></param>
public sealed class ChangedGivenDiffResult(ref readonly CandidateMap candidates) : ChangedDiffResult(in candidates)
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
