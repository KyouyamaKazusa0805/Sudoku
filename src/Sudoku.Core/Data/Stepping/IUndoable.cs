namespace Sudoku.Data.Stepping;

/// <summary>
/// Provides a undo-able data structure.
/// </summary>
[Obsolete("In the future, this type won't be used.", false)]
public interface IUndoable
{
	/// <summary>
	/// To undo the step.
	/// </summary>
	void Undo();

	/// <summary>
	/// To redo the step.
	/// </summary>
	void Redo();
}
