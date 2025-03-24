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
		var options = OptionsCore;
		this.AddRange(options);
		this.SetHandler(HandleCore, (Option<int>)options[0]);
	}


	/// <inheritdoc/>
	public ReadOnlySpan<Option> OptionsCore => (Option[])[new TimeoutOption()];

	/// <inheritdoc/>
	public ReadOnlySpan<Argument> ArgumentsCore => [];


	/// <inheritdoc/>
	void ICommand.HandleCore(__arglist)
	{
		var iterator = new ArgIterator(__arglist);
		var timeout = __refvalue(iterator.GetNextArg(), int);
		HandleCore(timeout);
	}

	/// <inheritdoc/>
	private void HandleCore(int timeout)
	{
		var generator = new HardPatternPuzzleGenerator();
		using var cts = CommonPreprocessors.CreateCancellationTokenSource(timeout);
		var result = generator.Generate(cancellationToken: cts.Token);
		if (result.IsUndefined)
		{
			//Console.WriteLine("Canceled.");
			return;
		}
		Console.WriteLine(result.ToString("."));
	}
}
