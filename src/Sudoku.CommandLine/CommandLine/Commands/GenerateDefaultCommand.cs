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
	public INonLeafCommand? Parent { get; init; }


	/// <inheritdoc/>
	public void HandleCore(InvocationContext context)
	{
		if (this is not (
			[CluesCountOption o1, SymmetricTypeOption o2],
			_,
			[
				CountOption go1,
				TimeoutOption go2,
				OutputFilePathOption go3,
				TechniqueOption go4,
				OutputInfoOption go5,
				OutputTargetGridOption go6,
				SeparatorOption go7
			]
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
		var alsoOutputInfo = result.GetValueForOption(go5);
		var outputTargetGridRatherThanOriginalGrid = result.GetValueForOption(go6);
		var separator = result.GetValueForOption(go7)!;
		CommonPreprocessors.GeneratePuzzles(
			new Generator(),
			(generator, cancellationToken) => generator.Generate(cluesCount, symmetricType, cancellationToken),
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
