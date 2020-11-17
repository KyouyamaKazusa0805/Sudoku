#pragma warning disable IDE0079 // Unused suppressions.
#pragma warning disable IDE0005
#pragma warning disable IDE1006

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Diagnostics;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.IO;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.BruteForces.Bitwise;
using Sudoku.Solving.Extensions;
using Sudoku.Solving.Manual;
using Sudoku.Solving.Manual.Alses.Basic;
using Sudoku.Solving.Manual.Chaining;
using Sudoku.Solving.Manual.LastResorts;
using Sudoku.Windows;
using static System.Console;

string path = @"C:\Users\Howdy\Desktop\p.txt";
string resultPath = @"C:\Users\Howdy\Desktop\result.txt";
if (!File.Exists(path))
{
	return;
}

var stopwatch = new Stopwatch();
string[] lines = await File.ReadAllLinesAsync(path);

stopwatch.Start();
for (int i = 0, length = lines.Length; i < length; i++)
{
	string puzzle = lines[i];
	if (!SudokuGrid.TryParse(puzzle, out var grid))
	{
		continue;
	}

	var (_, _, total, max, pearl, diamond, _, _, _, stepCount, steps, _, _) = new ManualSolver().Solve(grid);

	int chainingTechniquesCount = steps!.Count(
		static step =>
			step is ChainingTechniqueInfo
			or AlsXzTechniqueInfo or AlsXyWingTechniqueInfo or AlsWWingTechniqueInfo
			or DeathBlossomTechniqueInfo or BowmanBingoTechniqueInfo);

	await File.AppendAllTextAsync(
		resultPath,
		$"{grid}\t{total:0.0} {max:0.0} {pearl:0.0} {diamond:0.0} {stepCount} {chainingTechniquesCount}\r\n");

	Clear();
	WriteLine(
		$"Current: {i + 1}/{length} ({(i + 1) * 100M / (decimal)length:0.000}%), " +
		$"Elapsed: {stopwatch.Elapsed:hh\\:mm\\:ss\\.fff}");
}

stopwatch.Stop();


#if FILE_COUNTER || false
string root = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.Parent!.FullName;

WriteLine(new FileCounter(root, "cs", withBinOrObjDirectory: false).CountUp());
#endif
