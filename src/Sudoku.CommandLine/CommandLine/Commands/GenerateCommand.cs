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
			new GenerateDefaultCommand()
		);


	/// <inheritdoc/>
	public static ReadOnlySpan<Option> OptionsCore => [];

	/// <inheritdoc/>
	public static ReadOnlySpan<Argument> ArgumentsCore => [];


	/// <inheritdoc/>
	static void ICommand.HandleCore(__arglist) => throw new NotImplementedException();
}
