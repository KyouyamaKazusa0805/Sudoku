namespace Sudoku.Behaviors.DiffResults;

/// <summary>
/// Represents a difference result that describes candidates are reset.
/// </summary>
[method: JsonConstructor]
public sealed class ResetDiffResult() : DiffResult
{
	/// <inheritdoc/>
	public override string Notation => "R";

	/// <inheritdoc/>
	public override DiffType Type => DiffType.Reset;


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] DiffResult? other) => other is ResetDiffResult;

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(typeof(ResetDiffResult));

	/// <inheritdoc/>
	public override string ToString() => "Reset grid";

	/// <inheritdoc/>
	public override ResetDiffResult Clone() => new();
}
