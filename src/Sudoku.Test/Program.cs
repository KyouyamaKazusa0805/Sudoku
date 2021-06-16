using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Solving.Manual;

method1();
method2();

static void method1()
{
	var s = new TestStepSearcher();
	TestStepSearcher t = new();

	Console.WriteLine(s);
	Console.WriteLine(t);
}

static void method2()
{
	var grid = SudokuGrid.Undefined;
	FastProperties.InitializeMaps(grid);

	var s = new AnotherStepSearcher();
	AnotherStepSearcher t = new();

	Console.WriteLine(s);
	Console.WriteLine(t);
}


[DirectSearcher]
class AnotherStepSearcher : StepSearcher
{
	public static TechniqueProperties Properties { get; } = new(default, string.Empty);


	public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid) =>
		throw new NotImplementedException();
}

[DirectSearcher]
class TestStepSearcher : StepSearcher
{
	public static TechniqueProperties Properties { get; } = new(default, string.Empty);


	public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid) =>
		throw new NotImplementedException();
}
