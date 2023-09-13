using System.CommandLine;
using Sudoku.Analytics;
using Sudoku.Analytics.Categorization;
using Sudoku.Cli.Converters;
using Sudoku.Cli.Options;
using Sudoku.Concepts;
using Sudoku.Text.Formatting;

namespace Sudoku.Cli.Commands;

/// <summary>
/// Represents an analyze command.
/// </summary>
public sealed class AnalyzeCommand : Command, ICommand<AnalyzeCommand>
{
	/// <summary>
	/// Initializes an <see cref="AnalyzeCommand"/> instance.
	/// </summary>
	public AnalyzeCommand() : base("analyze", "To analyze a puzzle.")
	{
		var gridOption = IOption<GridOption, Grid, GridArgumentConverter>.CreateOption();
		var techniqueOption = IOption<TechniqueOption, Technique>.CreateOption();
		AddOption(gridOption);
		AddOption(techniqueOption);
		this.SetHandler(static (grid, technique) =>
		{
			var analyzer = PredefinedAnalyzers.Balanced;
			switch (technique, analyzer.Analyze(grid))
			{
				case (0, var analyzerResult):
				{
					Console.WriteLine(analyzerResult.ToString());
					break;
				}
				case (_, { IsSolved: true, SolvingPath: var path }):
				{
					var firstFoundStep = path.FirstOrDefault(path => path.Step.Code == technique);
					if (firstFoundStep == default)
					{
						Console.WriteLine("Sorry. The step whose technique used is specified one is not found.");
					}
					else
					{
						Console.WriteLine(firstFoundStep.SteppingGrid.ToString(new PencilMarkFormat()));
						Console.WriteLine(firstFoundStep.Step.ToString());
					}
					break;
				}
				case var (_, analyzerResult):
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(analyzerResult.ToString());
					Console.ResetColor();
					break;
				}
			}
		}, gridOption, techniqueOption);
	}
}
