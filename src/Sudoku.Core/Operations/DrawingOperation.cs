namespace Sudoku.Operations;

/// <summary>
/// Represents an operation that plays with drawing elements.
/// </summary>
public abstract class DrawingOperation : Operation
{
	/// <inheritdoc/>
	public sealed override bool CanRedo => UpdatedViewNodes.Length == 0;

	/// <inheritdocs/>
	public sealed override bool CanUndo => UpdatedViewNodes.Length != 0;

	/// <summary>
	/// Indicates the updated view nodes in the operation.
	/// </summary>
	protected ReadOnlyMemory<ViewNode> UpdatedViewNodes { get; set; } = ReadOnlyMemory<ViewNode>.Empty;
}
