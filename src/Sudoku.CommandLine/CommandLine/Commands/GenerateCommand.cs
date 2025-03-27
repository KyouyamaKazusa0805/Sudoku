namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Represents a generate command.
/// </summary>
internal sealed class GenerateCommand : Command, INonLeafCommand
{
	/// <summary>
	/// Initializes a <see cref="GenerateCommand"/> instance.
	/// </summary>
	public GenerateCommand() : base("generate", "Generate a puzzle using the specified way")
	{
		var commands = (SymbolList<Command>)[
			new GenerateDefaultCommand { Parent = this },
			new GeneratePatternCommand { Parent = this },
			new GenerateHardCommand { Parent = this }
		];
		this.AddRange(commands);

		GlobalOptionsCore = [new CountOption(), new TimeoutOption()];
		this.AddRangeGlobal(GlobalOptionsCore);
	}


	/// <inheritdoc/>
	public SymbolList<Option> GlobalOptionsCore { get; }
}
