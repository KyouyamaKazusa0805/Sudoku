namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Provides a undo-able data structure.
	/// </summary>
#if SUDOKU_UI
	[Obsolete("In the future, this type won't be used.", false)]
#endif
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
