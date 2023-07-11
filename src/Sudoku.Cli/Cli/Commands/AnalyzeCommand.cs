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
		var gridOption = IOption<GridOption, string>.CreateOption();
		gridOption.IsRequired = true;
		gridOption.AddValidator(static optionResult =>
		{
			var str = optionResult.GetValueOrDefault<string?>();
			if (string.IsNullOrWhiteSpace(str))
			{
				optionResult.ErrorMessage = "The target argument should not be an empty string or only contain whitespaces.";
			}
			else if (!Grid.TryParse(str, out var s))
			{
				optionResult.ErrorMessage = "The target argument must be a valid sudoku text string.";
			}
		});

		var techniqueOption = IOption<TechniqueOption, Technique>.CreateOption();
		AddOption(gridOption);
		AddOption(techniqueOption);
		this.SetHandler(static (grid, technique) =>
		{
			var gridResolved = Grid.Parse(grid);
			var analyzer = PredefinedAnalyzers.Balanced;
			switch (technique, analyzer.Analyze(gridResolved))
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
				case (_, var analyzerResult):
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
