#pragma warning disable CA1050

using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Solving.Manual;

Console.WriteLine();

/// <summary></summary>
public sealed class TempStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid) =>
		throw new NotImplementedException();
}