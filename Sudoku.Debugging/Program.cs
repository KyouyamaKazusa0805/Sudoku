using System;
using Sudoku.Data;
using Sudoku.Globalization;
using Sudoku.Solving;
using Sudoku.Solving.BruteForces.Backtracking;

// Define a sudoku grid using the string code.
var grid = SudokuGrid.Parse("250010000001089402940700000510200000000000000000007031000001026807640100000020043");

// Now define a instance for solving the puzzle.
var solver = new BacktrackingSolver();

// Solve this puzzle.
var analysisResult = solver.Solve(grid);

// Output the result.
Console.WriteLine(analysisResult.ToString(AnalysisResultFormattingOptions.ShowDifficulty, CountryCode.ZhCn));

#if BATCH_RATING || false
#pragma warning disable IDE1006

using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using Sudoku.Data;
using Sudoku.Solving.Manual;
using static System.Console;
using Sudoku.Solving.Extensions;

string path = @"C:\Users\Howdy\Desktop\p.txt";
string resultPath = @"C:\Users\Howdy\Desktop\result.txt";
if (!File.Exists(path))
{
	return;
}

var stopwatch = new Stopwatch();
string[] lines = await File.ReadAllLinesAsync(path);

stopwatch.Start();
try
{
	for (int i = 0, length = lines.Length; i < length; i++)
	{
		string puzzle = lines[i];
		if (!SudokuGrid.TryParse(puzzle, out var grid))
		{
			continue;
		}

		var (_, _, total, max, pearl, diamond, _, _, _, stepCount, steps, _, _) = new ManualSolver().Solve(grid);

		int chainingTechniquesCount = steps!.Count(
			static step => step.IsAlsTechnique() || step.IsChainingTechnique());

		await File.AppendAllTextAsync(
			resultPath,
			$"{grid}\t{total:0.0} {max:0.0} {pearl:0.0} {diamond:0.0} {stepCount} {chainingTechniquesCount}\r\n");

		Clear();
		WriteLine(
			$"Current: {i + 1}/{length} ({(i + 1) * 100M / (decimal)length:0.000}%), " +
			$"Elapsed: {stopwatch.Elapsed:hh\\:mm\\:ss\\.fff}");
	}
}
catch (Exception e)
{
	WriteLine("出现问题，原因：");
	WriteLine(e);
}

stopwatch.Stop();
ReadKey();
#endif

#if FILE_COUNTER || false
using System;
using System.IO;
using Sudoku.Diagnostics;
using static System.Console;

string root = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.Parent!.FullName;

WriteLine(new FileCounter(root, "cs", withBinOrObjDirectory: false).CountUp());
#endif
