using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Solving.Manual;

Console.WriteLine();


[DirectSearcher]
record RecordType;

[DirectSearcher]
class ClassType { }

[DirectSearcher]
class TestStepSearcher : StepSearcher
{
	public static TechniqueProperties Properties { get; } = new(default, string.Empty);


	public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid) =>
		throw new NotImplementedException();
}
