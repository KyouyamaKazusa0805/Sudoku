using System;
using System.IO;
using Sudoku.Diagnostics;

var updater = new CSharpProjectUpdater(
	Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.Parent!.FullName
);

try
{
	await updater.UpdateNullableAsync("enable", "\t");
}
catch (Exception ex)
{
	Console.WriteLine(ex);
}