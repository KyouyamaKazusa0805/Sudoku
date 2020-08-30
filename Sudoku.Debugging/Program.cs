#pragma warning disable IDE0005
#pragma warning disable IDE1006

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Diagnostics;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.BruteForces.Bitwise;
using Sudoku.Solving.Manual;
using Sudoku.Solving.Manual.Chaining;
using Sudoku.Windows;
using static System.Console;

#region File counter
#if true
var w = new Stopwatch();

var z = new FileCounter(Solution.PathRoot, "cs", false);

w.Start();
int codeLines = z.CountCodeLines(out int filesCount, out long charsCount, out long bytes);
w.Stop();

foreach (var fileName in z.FileList)
{
	WriteLine(fileName);
}

WriteLine(
	$"Code lines: {codeLines}, found files: {filesCount}, total characters: {charsCount}, " +
	$@"total bytes: {SizeUnitConverter.Convert(bytes, out var unit):.000} {unit switch
	{
		SizeUnit.Byte => "B",
		SizeUnit.Kilobyte => "KB",
		SizeUnit.Megabyte => "MB",
		SizeUnit.Gigabyte => "GB",
		SizeUnit.Terabyte => "TB",
		_ => throw Throwings.ImpossibleCase
	}}, " +
	$"time elapsed: {w.Elapsed:hh\\:mm\\.ss\\.fff}");
#endif
#endregion
