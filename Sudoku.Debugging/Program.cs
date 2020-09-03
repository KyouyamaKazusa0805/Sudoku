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
z.CountCodeLines();
w.Stop();

//foreach (string fileName in z.FileList)
//{
//	WriteLine(fileName);
//}
//WriteLine();

WriteLine(
	$"Results:\n" +
	$"* Code lines: {z.ResultLines} (Comment lines: {z.CommentLines})\n" +
	$"* Files: {z.FilesCount}\n" +
	$"* Characters: {z.CharactersCount}\n" +
	$"* Bytes: {SizeUnitConverter.Convert(z.Bytes, out var unit):.000} {Tostring(unit)} ({z.Bytes} Bytes)\n" +
	$"* Time elapsed: {w.Elapsed:hh\\:mm\\.ss\\.fff}");
#endif
#endregion

static string Tostring(SizeUnit @this) =>
	@this switch
	{
		SizeUnit.Byte => "B",
		SizeUnit.Kilobyte => "KB",
		SizeUnit.Megabyte => "MB",
		SizeUnit.Gigabyte => "GB",
		SizeUnit.Terabyte => "TB",
		_ => throw Throwings.ImpossibleCase
	};
