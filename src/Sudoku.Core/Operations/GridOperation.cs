namespace Sudoku.Operations;

/// <summary>
/// Represents an operation that plays with grids.
/// </summary>
public abstract class GridOperation : Operation
{
	/// <inheritdoc/>
	public sealed override bool CanRedo => UpdatedCandidates.Count == 0;

	/// <inheritdocs/>
	public sealed override bool CanUndo => UpdatedCandidates.Count != 0;

	/// <summary>
	/// Indicates the updated candidates in the operation.
	/// </summary>
	protected CandidateMap UpdatedCandidates { get; set; } = CandidateMap.Empty;
}
