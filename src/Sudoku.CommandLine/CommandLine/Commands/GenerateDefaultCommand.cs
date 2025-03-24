namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Represents a generate default command.
/// </summary>
public sealed class GenerateDefaultCommand : Command, ICommand
{
	/// <summary>
	/// Initializes a <see cref="GenerateDefaultCommand"/> instance.
	/// </summary>
	public GenerateDefaultCommand() : base("default", "Generate a puzzle in default way")
	{
		var options = OptionsCore;
		this.AddRange(options);
		this.SetHandler(HandleCore, (Option<int>)options[0], (Option<SymmetricType>)options[1], (Option<int>)options[2]);
	}


	/// <inheritdoc/>
	public ReadOnlySpan<Option> OptionsCore => (Option[])[new CluesCountOption(), new SymmetricTypeOption(), new TimeoutOption()];

	/// <inheritdoc/>
	public ReadOnlySpan<Argument> ArgumentsCore => [];


	/// <inheritdoc/>
	void ICommand.HandleCore(__arglist)
	{
		var iterator = new ArgIterator(__arglist);
		var cluesCount = __refvalue(iterator.GetNextArg(), int);
		var symmetricType = __refvalue(iterator.GetNextArg(), SymmetricType);
		var timeout = __refvalue(iterator.GetNextArg(), int);
		HandleCore(cluesCount, symmetricType, timeout);
	}

	/// <inheritdoc cref="ICommand.HandleCore"/>
	private void HandleCore(int cluesCount, SymmetricType symmetricType, int timeout)
	{
		var generator = new Generator();
		using var cts = new CancellationTokenSource(timeout);
		var result = generator.Generate(cluesCount, symmetricType, cts.Token);
		if (result.IsUndefined)
		{
			//Console.WriteLine("Canceled.");
			return;
		}
		Console.WriteLine(result.ToString("."));
	}
}
