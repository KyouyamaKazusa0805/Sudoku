#pragma warning disable IDE0079
#pragma warning disable CA1050
#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Solving.Manual;

SudokuGrid grid = default;
Console.WriteLine(grid.ToString("#"));

public sealed class StepSearcher100 : StepSearcher
{
	public static TechniqueProperties Properties { get; } = new(20, "");


	public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid) => throw new NotImplementedException();
}