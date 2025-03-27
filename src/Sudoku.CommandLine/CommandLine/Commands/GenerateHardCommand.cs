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
		if (this is not
			{
				Parent: INonLeafCommand { GlobalOptionsCore: [CountOption go1, TimeoutOption go2, OutputFilePathOption go3] }
			})
		{
			return;
		}

		var result = context.ParseResult;
		var count = result.GetValueForOption(go1);
		var timeout = result.GetValueForOption(go2);
		var outputFilePath = result.GetValueForOption(go3);
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
