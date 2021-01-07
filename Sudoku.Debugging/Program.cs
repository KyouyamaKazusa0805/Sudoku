using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Extensions;
using System.Text;
using System.Threading;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Solving.Extensions;
using static Sudoku.Constants.Processings;

var sb = new StringBuilder();
int cell = 0;
IList<int> listThatPointsTo = null!;
bool isFinished = false;

var stopwatch = Stopwatch.StartNew();
new Thread(method) { IsBackground = true }.Start();
new Thread(display) { IsBackground = true }.Start();

while (!isFinished) ;

stopwatch.Stop();

void display()
{
	int[] tempArray = new int[15];
	while (!isFinished)
	{
		Thread.Sleep(3000);
		for (int i = 0; i < listThatPointsTo.Count; i++)
		{
			tempArray[i] = listThatPointsTo[i];
		}

		sb.Clear();
		foreach (int cell in tempArray[..listThatPointsTo.Count])
		{
			sb.Append($"r{cell / 9 + 1}c{cell % 9 + 1}, ");
		}

		Console.Clear();
		Console.WriteLine($"Cells: {sb}, start cell: r{cell / 9 + 1}c{cell % 9 + 1}");
		Console.WriteLine($"Time elapsed: {stopwatch.Elapsed:hh\\:mm\\:ss\\.fff}");
	}
}

void method()
{
	var loops = new List<Cells>();
	var tempLoop = new List<int>();

	listThatPointsTo = tempLoop;
	for (; cell < 81; cell++)
	{
		Console.Clear();
		Console.WriteLine($"The current cell: r{cell / 9 + 1}c{cell % 9 + 1}");
		Console.WriteLine($"Time elapsed: {stopwatch.Elapsed:hh\\:mm\\:ss\\.fff}");

		loops.Clear();
		tempLoop.Clear();

		var loopMap = Cells.Empty;

		f(cell, (RegionLabel)byte.MaxValue);

		void f(int cell, RegionLabel lastLabel)
		{
			if (loopMap.Count > 14)
			{
				return;
			}

			loopMap.AddAnyway(cell);
			tempLoop.Add(cell);

			for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
			{
				if (label == lastLabel)
				{
					continue;
				}

				int region = label.ToRegion(cell);
				var cellsMap = RegionMaps[region] - cell;
				if (cellsMap.IsEmpty)
				{
					continue;
				}

				foreach (int nextCell in cellsMap)
				{
					if (tempLoop[0] == nextCell && tempLoop.Count >= 6 && tempLoop.IsValidLoop())
					{
						int z = loops.Count % 100;
						Console.Clear();
						Console.WriteLine($@"Found: {loops.Count + 1}{(
							loops.Count % 100 / 10 != 1
							? z switch { 1 => "st", 2 => "nd", 3 => "rd", _ => "th" }
							: "th")} loop.");
						Console.WriteLine($"Time elapsed: {stopwatch.Elapsed:hh\\:mm\\:ss\\.fff}");

						loops.Add(loopMap);
					}
					else if (!loopMap.Contains(nextCell))
					{
						bool flag = false;
						for (int i = 0; i < tempLoop.Count - 2; i++)
						{
							var temp = new Cells { tempLoop[i], tempLoop[i + 1] };
							foreach (int r in temp.CoveredRegions)
							{
								if (RegionMaps[r].Contains(tempLoop[i + 2]))
								{
									flag = true;
									goto Check;
								}
							}
						}

					Check:
						if (flag)
						{
							continue;
						}

						f(nextCell, label);
					}
				}
			}

			// Backtracking.
			loopMap.Remove(cell);
			tempLoop.RemoveLastElement();
			listThatPointsTo = tempLoop;
		}
	}

	Console.WriteLine($"Found {loops.Count} possible UL loops.");
	Console.WriteLine($"Time elapsed: {stopwatch.Elapsed:hh\\:mm\\:ss\\.fff}");

	isFinished = true;
}

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
