namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Provides a generate hard puzzles command.
/// </summary>
public sealed class GenerateHardCommand : Command, ICommand
{
	/// <summary>
	/// Initializes a <see cref="GenerateHardCommand"/> instance.
	/// </summary>
	public GenerateHardCommand() : base("hard", "Generates hard puzzles command")
	{
		OptionsCore = [new TimeoutOption(), new CountOption()];
		this.AddRange(OptionsCore);
		this.SetHandler(HandleCore, (Option<int>)OptionsCore[0], (Option<int>)OptionsCore[1]);
	}


	/// <inheritdoc/>
	public SymbolList<Option> OptionsCore { get; }

	/// <inheritdoc/>
	public SymbolList<Argument> ArgumentsCore => [];


	/// <inheritdoc/>
	void ICommand.HandleCore(__arglist)
	{
		var iterator = new ArgIterator(__arglist);
		var timeout = __refvalue(iterator.GetNextArg(), int);
		var count = __refvalue(iterator.GetNextArg(), int);
		HandleCore(timeout, count);
	}

	/// <inheritdoc/>
	private void HandleCore(int timeout, int count)
	{
		var generator = new HardPatternPuzzleGenerator();
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
