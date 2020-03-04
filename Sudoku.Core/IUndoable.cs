namespace Sudoku
{
	/// <summary>
	/// Provides a undo-able data structure.
	/// </summary>
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
}
