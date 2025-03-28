namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Provides a generate hard puzzles command.
/// </summary>
internal sealed class GenerateHardCommand : Command, ICommand
{
	/// <summary>
	/// Initializes a <see cref="GenerateHardCommand"/> instance.
	/// </summary>
	public GenerateHardCommand() : base("hard", "Generates hard puzzles command") => this.SetHandler(HandleCore);


	/// <inheritdoc/>
	public SymbolList<Option> OptionsCore => [];

	/// <inheritdoc/>
	public SymbolList<Argument> ArgumentsCore => [];

	/// <inheritdoc/>
	public Command? Parent { get; init; }


	/// <inheritdoc/>
	public void HandleCore(InvocationContext context)
	{
		if (this is not (
			_,
			_,
			INonLeafCommand(
				[
					CountOption go1,
					TimeoutOption go2,
					OutputFilePathOption go3,
					TechniqueFilterOption go4,
					OutputInfoOption go5
				]
			)
		))
		{
			return;
		}

		var result = context.ParseResult;
		var count = result.GetValueForOption(go1);
		var timeout = result.GetValueForOption(go2);
		var outputFilePath = result.GetValueForOption(go3);
		var filteredTechnique = result.GetValueForOption(go4);
		var alsoOutputInfo = result.GetValueForOption(go5);
		CommonPreprocessors.GeneratePuzzles(
			new HardPatternPuzzleGenerator(),
			static (generator, cancellationToken) => generator.Generate(cancellationToken: cancellationToken),
			outputFilePath,
			timeout,
			count,
			filteredTechnique,
			alsoOutputInfo
		);
	}
}
