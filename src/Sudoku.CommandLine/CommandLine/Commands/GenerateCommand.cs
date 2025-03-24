namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Represents a generate command.
/// </summary>
public sealed class GenerateCommand : Command, ICommand
{
	/// <summary>
	/// Initializes a <see cref="GenerateCommand"/> instance.
	/// </summary>
	public GenerateCommand() : base("generate", "Generate a puzzle using the specified way")
		=> this.AddRange(
			new GenerateDefaultCommand(),
			new GeneratePatternCommand()
		);


	/// <inheritdoc/>
	public ReadOnlySpan<Option> OptionsCore => [];

	/// <inheritdoc/>
	public ReadOnlySpan<Argument> ArgumentsCore => [];


	/// <inheritdoc/>
	void ICommand.HandleCore(__arglist) => throw new NotImplementedException();
}
