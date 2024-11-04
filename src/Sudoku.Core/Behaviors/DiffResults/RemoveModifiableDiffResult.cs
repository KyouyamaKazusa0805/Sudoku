namespace Sudoku.Behaviors.DiffResults;

/// <summary>
/// Represents a difference that describes a list of modifiable digits is removed.
/// </summary>
/// <param name="candidates"><inheritdoc path="/param[@name='candidates']"/></param>
public sealed class RemoveModifiableDiffResult(ref readonly CandidateMap candidates) : RemoveDiffResult(in candidates)
{
	/// <inheritdoc/>
	public override string NotationPrefix => "M-";

	/// <inheritdoc/>
	public override CellState CellType => CellState.Modifiable;

	/// <inheritdoc/>
	public override DiffType Type => DiffType.RemoveModifiable;


	/// <inheritdoc/>
	public override RemoveModifiableDiffResult Clone() => new(Candidates);
}
