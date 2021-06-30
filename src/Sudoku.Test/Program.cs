using System;
using System.IO;
using Sudoku.Diagnostics;

var fc = new FileCounter(
	root: Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.Parent!.FullName,
	extension: "md",
	withBinOrObjDirectory: false
);

await fc.CountUpAsync();

foreach (string path in fc.FileList)
{
	if (await File.ReadAllLinesAsync(path) is not { Length: not 0 } lines)
	{
		Console.WriteLine(path);
		continue;
	}

	if (!lines[0].StartsWith('#'))
	{
		Console.WriteLine(path);
		continue;
	}
}

Console.WriteLine("Done.");