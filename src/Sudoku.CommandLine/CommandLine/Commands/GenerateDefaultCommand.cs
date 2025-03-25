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
		OptionsCore = [new CluesCountOption(), new SymmetricTypeOption(), new TimeoutOption(), new CountOption()];
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
		var cluesCount = result.GetValueForOption((Option<int>)OptionsCore[0]);
		var symmetricType = result.GetValueForOption((Option<SymmetricType>)OptionsCore[1]);
		var timeout = result.GetValueForOption((Option<int>)OptionsCore[2]);
		var count = result.GetValueForOption((Option<int>)OptionsCore[3]);
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
