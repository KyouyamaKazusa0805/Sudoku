namespace Sudoku.Behaviors.DiffResults;

/// <summary>
/// Represents a difference that describes a list of modifiable digits is changed.
/// </summary>
/// <param name="candidates"><inheritdoc path="/param[@name='candidates']"/></param>
public sealed class ChangedModifiableDiffResult(ref readonly CandidateMap candidates) : ChangedDiffResult(in candidates)
{
	/// <inheritdoc/>
	public override string NotationPrefix => "G^";

	/// <inheritdoc/>
	public override CellState CellType => CellState.Modifiable;

	/// <inheritdoc/>
	public override DiffType Type => DiffType.ChangedModifiable;


	/// <inheritdoc/>
	public override ChangedModifiableDiffResult Clone() => new(Candidates);
}
