using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Extensions;
using System.IO;
using System.Text;
using Sudoku.Data;
using Sudoku.Solving.Extensions;
using static Sudoku.Constants.Processings;

const string separator = ", ";

var stopwatch = Stopwatch.StartNew();
method();
stopwatch.Stop();

void method()
{
	int count = 0;
	var tempLoop = new List<int>();
	var sb = new StringBuilder();

	for (int cell = 0; cell < 81; cell++)
	{
		tempLoop.Clear();

		var loopMap = Cells.Empty;

		f(cell);

		void f(int cell)
		{
			if (loopMap.Count > 14)
			{
				return;
			}

			var peers =
				tempLoop.Count != 0
				? PeerMaps[cell] - new Cells { tempLoop[^1], cell }.PeerIntersection
				: PeerMaps[cell];

			loopMap.AddAnyway(cell);
			tempLoop.Add(cell);

			foreach (int nextCell in peers)
			{
				if (tempLoop[0] == nextCell && tempLoop.Count >= 6 && tempLoop.IsValidLoop())
				{
					count++;

					Console.Write($@"({count,10}{(
						count % 100 / 10 != 1
						? (count % 10) switch { 1 => "st", 2 => "nd", 3 => "rd", _ => "th" }
						: "th"
					)}): ");

					sb.Clear().Append("{ ");
					foreach (int c in tempLoop)
					{
						sb.Append($"r{c / 9 + 1}c{c % 9 + 1}").Append(separator);
					}

					sb.RemoveFromEnd(separator.Length).Append(" }");

					try
					{
						File.AppendAllText(@"C:\Users\Howdy\Desktop\Loops.txt", $"{sb}\r\n");
					}
					catch
					{
					}

					Console.Write($"{sb,-90}");
					Console.WriteLine($"({stopwatch.Elapsed:hh\\:mm\\:ss\\.fff})");
				}
				else if (!loopMap.Contains(nextCell))
				{
					bool flag = false;
					for (int i = 0; i < tempLoop.Count - 1; i++)
					{
						var temp = new Cells { tempLoop[i], tempLoop[i + 1] };
						foreach (int r in temp.CoveredRegions)
						{
							foreach (int otherCell in tempLoop - temp)
							{
								if (RegionMaps[r].Contains(otherCell))
								{
									flag = true;
									goto Check;
								}
							}
						}
					}

				Check:
					if (flag)
					{
						continue;
					}

					f(nextCell);
				}
			}

			// Backtracking.
			loopMap.Remove(cell);
			tempLoop.RemoveLastElement();
		}
	}

	Console.WriteLine();
	Console.WriteLine($"Found {count} possible UL loops.");
	Console.WriteLine($"Time elapsed: {stopwatch.Elapsed:hh\\:mm\\:ss\\.fff}");
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
