namespace Sudoku.ComponentModel;

/// <summary>
/// Represents an operation.
/// </summary>
public abstract class Operation : ICloneable, IUndoable
{
	/// <summary>
	/// Indicates whether the operation can undo.
	/// </summary>
	public abstract bool CanUndo { get; }

	/// <summary>
	/// Indicates whether the operation can redo.
	/// </summary>
	public abstract bool CanRedo { get; }


	/// <inheritdoc/>
	public abstract void Undo();

	/// <inheritdoc/>
	public abstract void Redo();

	/// <summary>
	/// Creates a new <see cref="Operation"/> instance whose internal value is same as the current one, cloned.
	/// </summary>
	/// <returns>A new <see cref="Operation"/> instance cloned.</returns>
	public abstract Operation Clone();

	/// <inheritdoc/>
	object ICloneable.Clone() => Clone();
}
