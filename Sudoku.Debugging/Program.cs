#pragma warning disable IDE0005
#pragma warning disable IDE1006

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Diagnostics;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.BruteForces.Bitwise;
using Sudoku.Solving.Manual;
using Sudoku.Solving.Manual.Chaining;
using static System.Console;

#region Unsafe solver tester
#if true
var grid = Grid.Parse("050602000003000000040583001930000500000704000004000026700126050000000900000307060");
var solver = new UnsafeBitwiseSolver();
var result = solver.Solve(grid);
Console.WriteLine(result);
#endif
#endregion

#region Batch digit base conversion
#if false
string s = @"0777777777, 0777777000, 0777000777, 0777000000, 0777777, 0777000, 0777, 00,";
var regex = new Regex("0[0-7]+");
string result = regex.Replace(
	s,
	match =>
	{
		string matchValue = match.Value;
		long l = Convert.ToInt64(matchValue, 8);
		return $"0x{l:X}";
	});

Console.WriteLine(result);
#endif
#endregion

#region File counter
#if false
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
	$"total bytes: {g(bytes, out string unit):.000} {unit}, " +
	$"time elapsed: {w.Elapsed:hh\\:mm\\.ss\\.fff}");

static decimal g(long bytes, out string unit)
{
	switch (bytes)
	{
		case <= 1024L:
		{
			unit = "B";
			return bytes;
		}
		case <= 1048576L:
		{
			unit = "KB";
			return bytes / 1024M;
		}
		case <= 1073741824L:
		{
			unit = "MB";
			return bytes / 1048576M;
		}
		case <= 1099511627776L:
		{
			unit = "GB";
			return bytes / 1073741824M;
		}
		default:
		{
			unit = "TB";
			return bytes / 1099511627776M;
		}
	}
}
#endif
#endregion
