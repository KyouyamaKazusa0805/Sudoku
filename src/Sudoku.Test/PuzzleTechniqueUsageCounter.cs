using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sudoku.Generating;
using Sudoku.Globalization;
using Sudoku.Resources;
using Sudoku.Solving.Manual;
using Sudoku.Techniques;
using static System.Console;

namespace Sudoku.Test;

/// <summary>
/// Encapsulates the basic batch counter that counts the puzzle technique usages.
/// </summary>
internal static class PuzzleTechniqueUsageCounter
{
	/// <summary>
	/// Count up the technique usages.
	/// </summary>
	/// <param name="puzzlesCount">The number of puzzles should be checked.</param>
	internal static void CountUp(int puzzlesCount)
	{
		var stopwatch = new Stopwatch();

		TextResources.Current.ChangeLanguage(CountryCode.ZhCn);

		var generator = new HardPatternPuzzleGenerator();
		var solver = new ManualSolver();
		var techniqueDic = new Dictionary<Technique, long>();
		var difficultyLevelDic = new Dictionary<DifficultyLevel, int>();

		stopwatch.Start();
		for (int i = 0; i < puzzlesCount; i++)
		{
			Clear();
			WriteLine(
				"Progress: {0}%  [{1} / {2}]",
				(i * 100D / puzzlesCount).ToString("0.00"),
				i.ToString(),
				puzzlesCount.ToString()
			);
			WriteLine("Time elapsed: {0}", stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff"));

			var puzzle = generator.Generate()!.Value;
			var analysisResult = solver.Solve(puzzle);
			foreach (var techniqueUsage in analysisResult.Steps!)
			{
				if (
					techniqueUsage.TechniqueCode is var code and not (
						Technique.HiddenSingleRow or Technique.HiddenSingleColumn or Technique.HiddenSingleBlock
						or Technique.NakedSingle or Technique.FullHouse or Technique.LastDigit
					)
				)
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

		Clear();
		WriteLine("Time elapsed: {0}", stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff"));
		WriteLine();
		WriteLine("Technique used:");
		WriteLine(
			string.Join(
				Environment.NewLine,
				from pair in techniqueDic
				orderby pair.Value descending, pair.Key
				select $"{(string)TextResources.Current[pair.Key.ToString()]}: {pair.Value}"
			)
		);

		WriteLine();
		WriteLine("Difficulty level distribution:");
		WriteLine(
			string.Join(
				Environment.NewLine,
				from pair in difficultyLevelDic orderby pair.Key select $"{pair.Key}: {pair.Value}"
			)
		);
	}
}
