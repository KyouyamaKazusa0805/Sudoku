namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Encapsulates an anti-elimination step.
	/// </summary>
	/// <param name="Digit">The digit.</param>
	/// <param name="Cell">The cell.</param>
	public sealed record AntiEliminationStep(int Digit, int Cell) : Step
	{
		/// <inheritdoc/>
		public override void UndoStepTo(UndoableGrid grid) => grid._masks[Cell] |= (short)(1 << Digit);

		/// <inheritdoc/>
		public override void DoStepTo(UndoableGrid grid) => grid._masks[Cell] &= (short)~(1 << Digit);
	}
}
