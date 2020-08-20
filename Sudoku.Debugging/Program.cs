#pragma warning disable IDE0005

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Diagnostics;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Manual;
using Sudoku.Solving.Manual.Chaining;
using static System.Console;

var w = new Stopwatch();

var z = new CodeLineCounter(Solution.PathRoot, @".+\.cs$");

w.Start();
int codeLines = z.CountCodeLines(out int count);
w.Stop();

foreach (var fileName in z.FileList)
{
	WriteLine(fileName);
}

WriteLine($"Code lines: {codeLines}, found files: {count}, time elapsed: {w.Elapsed:hh\\:mm\\.ss\\.fff}");