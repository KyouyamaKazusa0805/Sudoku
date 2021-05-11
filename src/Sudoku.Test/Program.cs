using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Solving.Manual;

Console.WriteLine("Hello, world!");

/// <summary>
/// Default
/// </summary>
public sealed class StepSearcher1 : StepSearcher
{
	/// <inheritdoc/>
	public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid) => throw new NotImplementedException();
}