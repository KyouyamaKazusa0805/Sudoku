using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Solving.Manual;

unsafe
{
	var grid = SudokuGrid.Empty;
	SudokuGrid.ValueChanged(ref grid, new()); // SUDOKU014.
	SudokuGrid.RefreshingCandidates(ref grid); // SUDOKU014.
}

sealed class MyStepSearcher : StepSearcher
{
	public static TechniqueProperties? Properties { get; }

	public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid) => throw new System.NotImplementedException();
}