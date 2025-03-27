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
		if (this is not
			{
				OptionsCore: [CluesCountOption o1, SymmetricTypeOption o2],
				Parent: INonLeafCommand { GlobalOptionsCore: [CountOption go1, TimeoutOption go2, OutputFilePathOption go3] }
			})
		{
			return;
		}

		var result = context.ParseResult;
		var cluesCount = result.GetValueForOption(o1);
		var symmetricType = result.GetValueForOption(o2);
		var count = result.GetValueForOption(go1);
		var timeout = result.GetValueForOption(go2);
		var outputFilePath = result.GetValueForOption(go3);
		var generator = new Generator();
		using var outputFileStream = outputFilePath is null ? null : new StreamWriter(outputFilePath);
		using var cts = CommonPreprocessors.CreateCancellationTokenSource(timeout);
		for (var i = 0; i < count; i++)
		{
			var r = generator.Generate(cluesCount, symmetricType, cts.Token);
			if (r.IsUndefined)
			{
				return;
			}
			CommonPreprocessors.OutputTextTo(r, outputFileStream ?? Console.Out, static r => r.ToString("."), true);
		}
	}
}
