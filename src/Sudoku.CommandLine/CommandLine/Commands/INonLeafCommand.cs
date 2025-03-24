namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Provides a command that contains children commands (subcommands).
/// </summary>
internal interface INonLeafCommand
{
	/// <summary>
	/// Indicates the subcommands to add.
	/// </summary>
	public abstract ReadOnlySpan<Command> CommandsCore { get; }
}
