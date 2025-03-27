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
		var result = context.ParseResult;
		var count = result.GetValueForOption((CountOption)((INonLeafCommand)Parent!).GlobalOptionsCore[0]);
		var timeout = result.GetValueForOption((TimeoutOption)((INonLeafCommand)Parent!).GlobalOptionsCore[1]);
		var outputFilePath = result.GetValueForOption((OutputFilePathOption)((INonLeafCommand)Parent!).GlobalOptionsCore[2]);
		var generator = new HardPatternPuzzleGenerator();
		using var cts = CommonPreprocessors.CreateCancellationTokenSource(timeout);
		using var outputFileStream = outputFilePath is null ? null : new StreamWriter(outputFilePath);
		for (var i = 0; i < count; i++)
		{
			var r = generator.Generate(cancellationToken: cts.Token);
			if (r.IsUndefined)
			{
				return;
			}
			CommonPreprocessors.OutputTextTo(r, outputFileStream ?? Console.Out, static r => r.ToString("."), true);
		}
	}
}
