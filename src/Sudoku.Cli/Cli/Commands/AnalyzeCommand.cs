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
		var gridOption = IOption<GridOption, Grid>.CreateOption(static argumentResult =>
		{
			var str = argumentResult.Tokens.First(static token => token.Type == TokenType.Argument).Value;
			if (string.IsNullOrWhiteSpace(str))
			{
				argumentResult.ErrorMessage = "The target argument should not be an empty string or only contain whitespaces.";
				return Grid.Undefined;
			}
			else if (!Grid.TryParse(str, out var s))
			{
				argumentResult.ErrorMessage = "The target argument must be a valid sudoku text string.";
				return Grid.Undefined;
			}
			else
			{
				return s;
			}
		}, true, true);

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
