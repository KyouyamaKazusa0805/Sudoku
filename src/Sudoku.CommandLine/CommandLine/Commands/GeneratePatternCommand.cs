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
	public INonLeafCommand? Parent { get; init; }


	/// <inheritdoc/>
	public void HandleCore(InvocationContext context)
	{
		if (this is not (
			[MissingDigitOption o1],
			[CellMapArgument a1],
			[
				CountOption go1,
				TimeoutOption go2,
				OutputFilePathOption go3,
				TechniqueFilterOption go4,
				OutputInfoOption go5,
				OutputTargetGridOption go6,
				SeparatorOption go7
			]
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
		var alsoOutputInfo = result.GetValueForOption(go5);
		var outputTargetGridRatherThanOriginalGrid = result.GetValueForOption(go6);
		var separator = result.GetValueForOption(go7)!;
		CommonPreprocessors.GeneratePuzzles(
			new PatternBasedPuzzleGenerator(in cells, missingDigit),
			static (generator, cancellationToken) => generator.Generate(cancellationToken: cancellationToken),
			outputFilePath,
			timeout,
			count,
			filteredTechnique,
			alsoOutputInfo,
			outputTargetGridRatherThanOriginalGrid,
			separator
		);
	}
}
