using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Solving.Manual;

Console.WriteLine("");

internal sealed class S : StepSearcher
{
	public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
	{
	}
}