namespace Sudoku.Behaviors.Diff.Results;

/// <summary>
/// Represents a difference that describes a list of modifiable digits is removed.
/// </summary>
/// <param name="candidates"><inheritdoc path="/param[@name='candidates']"/></param>
[IntroducedSince(3, 4)]
[method: JsonConstructor]
public sealed class RemoveModifiableDiffResult(CandidateMap candidates) : RemoveDiffResult(candidates)
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
