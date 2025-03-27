namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Provides generate pattern command.
/// </summary>
public sealed class GeneratePatternCommand : Command, ICommand
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
		var cells = result.GetValueForArgument((CellMapArgument)ArgumentsCore[0]);
		var generator = new PatternBasedPuzzleGenerator(in cells, missingDigit);
		using var cts = CommonPreprocessors.CreateCancellationTokenSource(timeout);
		for (var i = 0; i < count; i++)
		{
			var r = generator.Generate(cancellationToken: cts.Token);
			if (r.IsUndefined)
			{
				//Console.WriteLine("Canceled.");
				return;
			}
			Console.WriteLine(r.ToString("."));
		}
	}
}
