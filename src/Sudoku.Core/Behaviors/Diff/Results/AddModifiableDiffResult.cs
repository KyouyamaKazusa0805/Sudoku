namespace Sudoku.Behaviors.Diff.Results;

/// <summary>
/// Represents a difference that describes a list of modifiable digits are added.
/// </summary>
/// <param name="candidates"><inheritdoc path="/param[@name='candidates']"/></param>
/// <param name="areCorrect"><inheritdoc path="/param[@name='areCorrect']"/></param>
[method: JsonConstructor]
public sealed class AddModifiableDiffResult(CandidateMap candidates, bool areCorrect) : AddDiffResult(candidates, areCorrect)
{
	/// <inheritdoc/>
	public override string NotationPrefix => "M+";

	/// <inheritdoc/>
	public override CellState CellType => CellState.Modifiable;

	/// <inheritdoc/>
	public override DiffType Type => DiffType.AddModifiable;


	/// <inheritdoc/>
	public override AddModifiableDiffResult Clone() => new(Candidates, AreCorrect);
}
