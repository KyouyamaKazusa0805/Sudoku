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
		AddOption(symmetricTypeOption);
		AddOption(difficultyLevelOption);
		AddOption(techniqueOption);
		this.SetHandler(
			static (symmetricType, difficultyLevel, technique) =>
			{
				var analyzer = PredefinedAnalyzers.Balanced;
				while (true)
				{
					var puzzle = HodokuPuzzleGenerator.Generate(symmetricType);
					if (analyzer.Analyze(puzzle) is { IsSolved: true, DifficultyLevel: var dl, Steps: var steps }
						&& (difficultyLevel != 0 && dl == difficultyLevel || difficultyLevel == 0)
						&& (technique != 0 && Array.Exists(steps, step => step.Code == technique) || technique == 0))
					{
						Console.WriteLine(puzzle.ToString());
						return;
					}
				}
			},
			symmetricTypeOption,
			difficultyLevelOption,
			techniqueOption
		);
	}
}
