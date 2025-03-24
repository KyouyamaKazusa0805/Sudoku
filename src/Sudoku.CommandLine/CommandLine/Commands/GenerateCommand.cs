namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Represents a generate command.
/// </summary>
public sealed class GenerateCommand : Command, INonLeafCommand
{
	/// <summary>
	/// Initializes a <see cref="GenerateCommand"/> instance.
	/// </summary>
	public GenerateCommand() : base("generate", "Generate a puzzle using the specified way") => this.AddRange(CommandsCore);


	/// <inheritdoc/>
	public ReadOnlySpan<Command> CommandsCore
		=> (Command[])[new GenerateDefaultCommand(), new GeneratePatternCommand(), new GenerateHardCommand()];
}
