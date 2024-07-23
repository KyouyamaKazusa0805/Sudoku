namespace Sudoku.ComponentModel;

/// <summary>
/// Represents an undoable object.
/// </summary>
public interface IUndoable
{
	/// <summary>
	/// Undo the current object.
	/// </summary>
	public abstract void Undo();

	/// <summary>
	/// Redo the current object.
	/// </summary>
	public abstract void Redo();
}
