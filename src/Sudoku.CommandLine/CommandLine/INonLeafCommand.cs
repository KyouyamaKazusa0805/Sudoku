namespace Sudoku.CommandLine;

/// <summary>
/// Represents a command that has sub-commands.
/// </summary>
internal interface INonLeafCommand
{
	/// <summary>
	/// Indicates the global options of the current command, applying to the current command and its sub-commands.
	/// </summary>
	public abstract SymbolList<Option> GlobalOptionsCore { get; }
}
