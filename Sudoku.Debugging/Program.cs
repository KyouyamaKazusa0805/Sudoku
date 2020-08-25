#pragma warning disable IDE0005
#pragma warning disable IDE1006

#warning This file should be compiled manually.

using System;
using System.Diagnostics;
using System.IO;
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
