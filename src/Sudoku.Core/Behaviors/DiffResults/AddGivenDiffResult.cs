namespace Sudoku.Behaviors.DiffResults;

/// <summary>
/// Represents a difference that describes a list of given digits are added.
/// </summary>
/// <param name="candidates"><inheritdoc path="/param[@name='candidates']"/></param>
/// <param name="areCorrect"><inheritdoc path="/param[@name='areCorrect']"/></param>
[method: JsonConstructor]
public sealed class AddGivenDiffResult(CandidateMap candidates, bool areCorrect) : AddDiffResult(candidates, areCorrect)
{
	/// <inheritdoc/>
	public override string NotationPrefix => "G+";

	/// <inheritdoc/>
	public override CellState CellType => CellState.Given;

	/// <inheritdoc/>
	public override DiffType Type => DiffType.AddGiven;


	/// <inheritdoc/>
	public override AddGivenDiffResult Clone() => new(Candidates, AreCorrect);
}
