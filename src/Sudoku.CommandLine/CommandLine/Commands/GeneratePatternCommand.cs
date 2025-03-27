namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Provides generate pattern command.
/// </summary>
internal sealed class GeneratePatternCommand : Command, ICommand
{
	/// <summary>
	/// Initializes a <see cref="GeneratePatternCommand"/> instance.
	/// </summary>
	public GeneratePatternCommand() : base("pattern", "Generate a pattern-based puzzle")
	{
		OptionsCore = [new MissingDigitOption()];
		this.AddRange(OptionsCore);

		ArgumentsCore = [new CellMapArgument(true)];
		this.AddRange(ArgumentsCore);

		this.SetHandler(HandleCore);
	}


	/// <inheritdoc/>
	public SymbolList<Option> OptionsCore { get; }

	/// <inheritdoc/>
	public SymbolList<Argument> ArgumentsCore { get; }

	/// <inheritdoc/>
	public Command? Parent { get; init; }


	/// <inheritdoc/>
	public void HandleCore(InvocationContext context)
	{
		var result = context.ParseResult;
		var missingDigit = result.GetValueForOption((MissingDigitOption)OptionsCore[0]);
		var count = result.GetValueForOption((CountOption)((INonLeafCommand)Parent!).GlobalOptionsCore[0]);
		var timeout = result.GetValueForOption((TimeoutOption)((INonLeafCommand)Parent!).GlobalOptionsCore[1]);
		var outputFilePath = result.GetValueForOption((OutputFilePathOption)((INonLeafCommand)Parent!).GlobalOptionsCore[2]);
		var cells = result.GetValueForArgument((CellMapArgument)ArgumentsCore[0]);
		var generator = new PatternBasedPuzzleGenerator(in cells, missingDigit);
		using var outputFileStream = outputFilePath is null ? null : new StreamWriter(outputFilePath);
		using var cts = CommonPreprocessors.CreateCancellationTokenSource(timeout);
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
