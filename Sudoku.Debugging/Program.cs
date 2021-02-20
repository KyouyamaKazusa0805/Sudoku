#pragma warning disable CA1050

using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Solving.Manual;

public sealed class MyStepSearcher : StepSearcher
{
	public static TechniqueProperties Properties { get; } = null!;

	public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid) =>
		throw new NotImplementedException();
}

internal static class Program
{
	private static void Main()
	{
	}
}