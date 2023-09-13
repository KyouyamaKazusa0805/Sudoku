using System.CommandLine;
using Sudoku.Algorithm.Generating;
using Sudoku.Analytics;
using Sudoku.Analytics.Categorization;
using Sudoku.Cli.Options;
using Sudoku.Concepts;

namespace Sudoku.Cli.Commands;

/// <summary>
/// Indicates a command that creates puzzles.
/// </summary>
public sealed class GenerateCommand : Command, ICommand<GenerateCommand>
{
	/// <summary>
	/// Creates a <see cref="GenerateCommand"/> instance.
	/// </summary>
	public GenerateCommand() : base("generate", "Generates a new puzzle with some customized options.")
	{
		var symmetricTypeOption = IOption<SymmetricTypeOption, SymmetricType>.CreateOption();
		var difficultyLevelOption = IOption<DifficultyLevelOption, DifficultyLevel>.CreateOption();
		var techniqueOption = IOption<TechniqueOption, Technique>.CreateOption();
		var limitCountOption = IOption<LimitCountOption, int>.CreateOption();
		AddOption(symmetricTypeOption);
		AddOption(difficultyLevelOption);
		AddOption(techniqueOption);
		AddOption(limitCountOption);
		this.SetHandler(static (symmetricType, difficultyLevel, technique, limitCount) =>
		{
			limitCount = limitCount == 0 ? int.MaxValue : limitCount;
			for (var (count, analyzer) = (0, PredefinedAnalyzers.Balanced); count < limitCount; count++)
			{
				var puzzle = HodokuPuzzleGenerator.Generate(HodokuPuzzleGenerator.AutoClues, symmetricType);
				if (analyzer.Analyze(puzzle) is { IsSolved: true, DifficultyLevel: var dl, Steps: var steps }
					&& (difficultyLevel != 0 && dl == difficultyLevel || difficultyLevel == 0)
					&& (technique != 0 && Array.Exists(steps, step => step.Code == technique) || technique == 0))
				{
					Console.WriteLine(puzzle.ToString());
					return;
				}
			}

			Console.WriteLine("Failed to create puzzles.");
		}, symmetricTypeOption, difficultyLevelOption, techniqueOption, limitCountOption);
	}
}
