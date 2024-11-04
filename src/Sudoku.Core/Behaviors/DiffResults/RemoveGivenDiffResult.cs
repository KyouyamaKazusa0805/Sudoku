namespace Sudoku.Behaviors.DiffResults;

/// <summary>
/// Represents a difference that describes a list of given digits is removed.
/// </summary>
/// <param name="candidates"><inheritdoc path="/param[@name='candidates']"/></param>
public sealed class RemoveGivenDiffResult(ref readonly CandidateMap candidates) : RemoveDiffResult(in candidates)
{
	/// <inheritdoc/>
	public override string NotationPrefix => "G-";

	/// <inheritdoc/>
	public override CellState CellType => CellState.Given;

	/// <inheritdoc/>
	public override DiffType Type => DiffType.RemoveGiven;


	/// <inheritdoc/>
	public override RemoveGivenDiffResult Clone() => new(Candidates);
}
