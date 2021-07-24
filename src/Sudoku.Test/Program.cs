#pragma warning disable IDE0079
#pragma warning disable SS0101

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sudoku.Generating;
using Sudoku.Globalization;
using Sudoku.Resources;
using Sudoku.Solving.Manual;
using Sudoku.Techniques;

var stopwatch = new Stopwatch();

TextResources.Current.ChangeLanguage(CountryCode.ZhCn);

var generator = new HardPatternPuzzleGenerator();
var solver = new ManualSolver();
var techniqueDic = new Dictionary<Technique, long>();
var difficultyLevelDic = new Dictionary<DifficultyLevel, int>();

const int count = 1000;

stopwatch.Start();
for (int i = 0; i < count; i++)
{
	Console.Clear();
	Console.WriteLine($"Progress: {(double)i / count * 100:0.00}%  [{i} / {count}]");
	Console.WriteLine($"Time elapsed: {stopwatch.Elapsed:hh\\:mm\\:ss\\.fff}");

	var puzzle = generator.Generate()!.Value;
	var analysisResult = solver.Solve(puzzle);
	foreach (var techniqueUsage in analysisResult.Steps!)
	{
		var code = techniqueUsage.TechniqueCode;
		if (code is not (
			Technique.HiddenSingleRow or Technique.HiddenSingleColumn or Technique.HiddenSingleBlock
			or Technique.NakedSingle or Technique.FullHouse or Technique.LastDigit))
		{
			if (techniqueDic.ContainsKey(code))
			{
				techniqueDic[code]++;
			}
			else
			{
				techniqueDic.Add(code, 1);
			}
		}
	}

	var difficultyLevel = analysisResult.DifficultyLevel;
	if (difficultyLevelDic.ContainsKey(difficultyLevel))
	{
		difficultyLevelDic[difficultyLevel]++;
	}
	else
	{
		difficultyLevelDic.Add(difficultyLevel, 1);
	}
}

stopwatch.Stop();

Console.Clear();
Console.WriteLine($"Time elapsed: {stopwatch.Elapsed:hh\\:mm\\:ss\\.fff}");
Console.WriteLine();
Console.WriteLine("Technique used:");
Console.WriteLine(
	string.Join(
		"\r\n",
		from pair in techniqueDic
		orderby pair.Value descending, pair.Key
		select $"{TextResources.Current[pair.Key.ToString()]}: {pair.Value.ToString()}"
	)
);

Console.WriteLine();
Console.WriteLine("Difficulty level distribution:");
Console.WriteLine(
	string.Join(
		"\r\n",
		from pair in difficultyLevelDic
		orderby pair.Key
		select $"{pair.Key.ToString()}: {pair.Value.ToString()}"
	)
);
