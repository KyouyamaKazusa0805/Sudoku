namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Provides generate pattern command.
/// </summary>
internal sealed class GeneratePatternCommand : Command, ICommand
{
	/// <summary>
	/// Initializes a <see cref="GeneratePatternCommand"/> instance.
	/// </summary>
	public GeneratePatternCommand() : base("pattern", "Generate a pattern-based puzzle")
	{
		OptionsCore = [new MissingDigitOption()];
		this.AddRange(OptionsCore);

		ArgumentsCore = [new CellMapArgument(true)];
		this.AddRange(ArgumentsCore);

		this.SetHandler(HandleCore);
	}


	/// <inheritdoc/>
	public SymbolList<Option> OptionsCore { get; }

	/// <inheritdoc/>
	public SymbolList<Argument> ArgumentsCore { get; }

	/// <inheritdoc/>
	public Command? Parent { get; init; }


	/// <inheritdoc/>
	public void HandleCore(InvocationContext context)
	{
		if (this is not (
			[MissingDigitOption o1],
			[CellMapArgument a1],
			INonLeafCommand([CountOption go1, TimeoutOption go2, OutputFilePathOption go3, TechniqueFilterOption go4])
		))
		{
			return;
		}

		var result = context.ParseResult;
		var missingDigit = result.GetValueForOption(o1);
		var cells = result.GetValueForArgument(a1);
		var count = result.GetValueForOption(go1);
		var timeout = result.GetValueForOption(go2);
		var outputFilePath = result.GetValueForOption(go3);
		var filteredTechnique = result.GetValueForOption(go4);
		var analyzer = filteredTechnique == Technique.None ? null : new Analyzer();
		var generator = new PatternBasedPuzzleGenerator(in cells, missingDigit);
		using var outputFileStream = outputFilePath is null ? null : new StreamWriter(outputFilePath);
		using var cts = CommonPreprocessors.CreateCancellationTokenSource(timeout);
		for (var i = 0; count == -1 || i < count;)
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
