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
		var options = OptionsCore;
		this.AddRange(options);
		this.SetHandler(HandleCore, (Option<int>)options[0], (Option<CellMap>)options[1], (Option<int>)options[2], (Option<int>)options[3]);
	}


	/// <inheritdoc/>
	public ReadOnlySpan<Option> OptionsCore
		=> (Option[])[new TimeoutOption(), new CellMapOption(true), new MissingDigitOption(), new CountOption()];

	/// <inheritdoc/>
	public ReadOnlySpan<Argument> ArgumentsCore => [];


	/// <inheritdoc/>
	void ICommand.HandleCore(__arglist)
	{
		var iterator = new ArgIterator(__arglist);
		var timeout = __refvalue(iterator.GetNextArg(), int);
		var patternCells = __refvalue(iterator.GetNextArg(), CellMap);
		var missingDigit = __refvalue(iterator.GetNextArg(), int);
		var count = __refvalue(iterator.GetNextArg(), int);
		HandleCore(timeout, patternCells, missingDigit, count);
	}

	/// <inheritdoc cref="ICommand.HandleCore"/>
	private void HandleCore(int timeout, CellMap cells, int missingDigit, int count)
	{
		var generator = new PatternBasedPuzzleGenerator(in cells, missingDigit);
		using var cts = CommonPreprocessors.CreateCancellationTokenSource(timeout);
		for (var i = 0; i < count; i++)
		{
			var result = generator.Generate(cancellationToken: cts.Token);
			if (result.IsUndefined)
			{
				//Console.WriteLine("Canceled.");
				return;
			}
			Console.WriteLine(result.ToString("."));
		}
	}
}
