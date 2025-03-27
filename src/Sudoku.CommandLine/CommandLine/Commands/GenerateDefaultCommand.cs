namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Represents a generate default command.
/// </summary>
internal sealed class GenerateDefaultCommand : Command, ICommand
{
	/// <summary>
	/// Initializes a <see cref="GenerateDefaultCommand"/> instance.
	/// </summary>
	public GenerateDefaultCommand() : base("default", "Generate a puzzle in default way")
	{
		OptionsCore = [new CluesCountOption(), new SymmetricTypeOption()];
		this.AddRange(OptionsCore);

		this.SetHandler(HandleCore);
	}


	/// <inheritdoc/>
	public SymbolList<Option> OptionsCore { get; }

	/// <inheritdoc/>
	public SymbolList<Argument> ArgumentsCore => [];

	/// <inheritdoc/>
	public Command? Parent { get; init; }


	/// <inheritdoc/>
	public void HandleCore(InvocationContext context)
	{
		var result = context.ParseResult;
		var cluesCount = result.GetValueForOption((CluesCountOption)OptionsCore[0]);
		var symmetricType = result.GetValueForOption((SymmetricTypeOption)OptionsCore[1]);
		var count = result.GetValueForOption((CountOption)((INonLeafCommand)Parent!).GlobalOptionsCore[0]);
		var timeout = result.GetValueForOption((TimeoutOption)((INonLeafCommand)Parent!).GlobalOptionsCore[1]);
		var generator = new Generator();
		using var cts = CommonPreprocessors.CreateCancellationTokenSource(timeout);
		for (var i = 0; i < count; i++)
		{
			var r = generator.Generate(cluesCount, symmetricType, cts.Token);
			if (r.IsUndefined)
			{
				//Console.WriteLine("Canceled.");
				return;
			}
			Console.WriteLine(r.ToString("."));
		}
	}
}
