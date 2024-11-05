namespace Sudoku.Behaviors.DiffResults;

/// <summary>
/// Represents a result that describes two grids are same.
/// </summary>
[method: JsonConstructor]
public sealed class NothingChangedDiffResult() : DiffResult
{
	/// <inheritdoc/>
	public override string Notation => "N";

	/// <inheritdoc/>
	public override DiffType Type => DiffType.NothingChanged;


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] DiffResult? other) => other is NothingChangedDiffResult;

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(typeof(NothingChangedDiffResult));

	/// <inheritdoc/>
	public override string ToString() => "Nothing changed";

	/// <inheritdoc/>
	public override NothingChangedDiffResult Clone() => new();
}
