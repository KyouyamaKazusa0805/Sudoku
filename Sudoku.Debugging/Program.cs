#pragma warning disable CA1050

using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Solving.Manual;

Console.WriteLine("Hello");

public sealed class MyStepSearcher : StepSearcher
{
	public static TechniqueProperties? Properties { get; }

	public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid) =>
		throw new NotImplementedException();
}
