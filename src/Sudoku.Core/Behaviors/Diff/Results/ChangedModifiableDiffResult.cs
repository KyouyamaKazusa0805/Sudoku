namespace Sudoku.Behaviors.Diff.Results;

/// <summary>
/// Represents a difference that describes a list of modifiable digits is changed.
/// </summary>
/// <param name="candidates"><inheritdoc path="/param[@name='candidates']"/></param>
[IntroducedSince(3, 4)]
[method: JsonConstructor]
public sealed class ChangedModifiableDiffResult(CandidateMap candidates) : ChangedDiffResult(candidates)
{
	/// <inheritdoc/>
	public override string NotationPrefix => "M^";

	/// <inheritdoc/>
	public override CellState CellType => CellState.Modifiable;

	/// <inheritdoc/>
	public override DiffType Type => DiffType.ChangedModifiable;


	/// <inheritdoc/>
	public override ChangedModifiableDiffResult Clone() => new(Candidates);
}
