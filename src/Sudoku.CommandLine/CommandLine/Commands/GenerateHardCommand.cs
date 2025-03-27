namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Provides a generate hard puzzles command.
/// </summary>
internal sealed class GenerateHardCommand : Command, ICommand
{
	/// <summary>
	/// Initializes a <see cref="GenerateHardCommand"/> instance.
	/// </summary>
	public GenerateHardCommand() : base("hard", "Generates hard puzzles command") => this.SetHandler(HandleCore);


	/// <inheritdoc/>
	public SymbolList<Option> OptionsCore => [];

	/// <inheritdoc/>
	public SymbolList<Argument> ArgumentsCore => [];

	/// <inheritdoc/>
	public Command? Parent { get; init; }


	/// <inheritdoc/>
	public void HandleCore(InvocationContext context)
	{
		if (this is not
			{
				Parent: INonLeafCommand
				{
					GlobalOptionsCore: [CountOption go1, TimeoutOption go2, OutputFilePathOption go3, TechniqueFilterOption go4]
				}
			})
		{
			return;
		}

		var result = context.ParseResult;
		var count = result.GetValueForOption(go1);
		var timeout = result.GetValueForOption(go2);
		var outputFilePath = result.GetValueForOption(go3);
		var filteredTechnique = result.GetValueForOption(go4);
		var analyzer = filteredTechnique == Technique.None ? null : new Analyzer();
		var generator = new HardPatternPuzzleGenerator();
		using var cts = CommonPreprocessors.CreateCancellationTokenSource(timeout);
		using var outputFileStream = outputFilePath is null ? null : new StreamWriter(outputFilePath);
		for (var i = 0; i < count;)
		{
			var r = generator.Generate(cancellationToken: cts.Token);
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
