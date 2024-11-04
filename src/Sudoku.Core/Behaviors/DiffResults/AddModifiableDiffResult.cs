namespace Sudoku.Behaviors.DiffResults;

/// <summary>
/// Represents a difference that describes a list of modifiable digits are added.
/// </summary>
/// <param name="candidates"><inheritdoc path="/param[@name='candidates']"/></param>
/// <param name="areCorrect"><inheritdoc path="/param[@name='areCorrect']"/></param>
public sealed class AddModifiableDiffResult(ref readonly CandidateMap candidates, bool areCorrect) :
	AddDiffResult(in candidates, areCorrect)
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
