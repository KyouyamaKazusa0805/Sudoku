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
		if (this is not (
			[CluesCountOption o1, SymmetricTypeOption o2],
			_,
			INonLeafCommand([CountOption go1, TimeoutOption go2, OutputFilePathOption go3, TechniqueFilterOption go4])
		))
		{
			return;
		}

		var result = context.ParseResult;
		var cluesCount = result.GetValueForOption(o1);
		var symmetricType = result.GetValueForOption(o2);
		var count = result.GetValueForOption(go1);
		var timeout = result.GetValueForOption(go2);
		var outputFilePath = result.GetValueForOption(go3);
		var filteredTechnique = result.GetValueForOption(go4);
		var analyzer = filteredTechnique == Technique.None ? null : new Analyzer();
		var generator = new Generator();
		using var outputFileStream = outputFilePath is null ? null : new StreamWriter(outputFilePath);
		using var cts = CommonPreprocessors.CreateCancellationTokenSource(timeout);
		for (var i = 0; i < count;)
		{
			var r = generator.Generate(cluesCount, symmetricType, cts.Token);
			if (r.IsUndefined)
			{
				return;
			}

			if (filteredTechnique != Technique.None
				&& (
					analyzer!.Analyze(r) is not { IsSolved: true, StepsSpan: var steps }
					|| !steps.Any(step => step.Code == filteredTechnique)
				))
			{
				continue;
			}

			CommonPreprocessors.OutputTextTo(r, outputFileStream ?? Console.Out, static r => r.ToString("."), true);
			i++;
		}
	}
}
