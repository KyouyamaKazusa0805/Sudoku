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
		OptionsCore = [new TimeoutOption(), new CellMapOption(true), new MissingDigitOption(), new CountOption()];
		this.AddRange(OptionsCore);
		this.SetHandler(HandleCore);
	}


	/// <inheritdoc/>
	public SymbolList<Option> OptionsCore { get; }

	/// <inheritdoc/>
	public SymbolList<Argument> ArgumentsCore => [];


	/// <inheritdoc/>
	public void HandleCore(InvocationContext context)
	{
		var result = context.ParseResult;
		var timeout = result.GetValueForOption((Option<int>)OptionsCore[0]);
		var cells = result.GetValueForOption((Option<CellMap>)OptionsCore[1]);
		var missingDigit = result.GetValueForOption((Option<int>)OptionsCore[2]);
		var count = result.GetValueForOption((Option<int>)OptionsCore[3]);
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
