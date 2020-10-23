namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Encapsulates an elimination step.
	/// </summary>
	/// <param name="Digit">Indicates the digit.</param>
	/// <param name="Cell">Indicates the cell.</param>
	public sealed unsafe record EliminationStep(int Digit, int Cell) : Step
	{
		/// <inheritdoc/>
		public override void UndoStepTo(UndoableGrid grid) =>
			grid._innerGrid._values[Cell] &= (short)~(1 << Digit);

		/// <inheritdoc/>
		public override void DoStepTo(UndoableGrid grid) =>
			grid._innerGrid._values[Cell] |= (short)(1 << Digit);
	}
}
