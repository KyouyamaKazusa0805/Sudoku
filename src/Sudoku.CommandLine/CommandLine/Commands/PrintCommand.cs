namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Represents a print command.
/// </summary>
public sealed class PrintCommand : Command
{
	/// <summary>
	/// Initializes a <see cref="PrintCommand"/> instance.
	/// </summary>
	public PrintCommand() : base("print", "Prints the specified data (especially for some bulit-in data)")
	{
		var commands = (SymbolList<Command>)[new PrintTechniquesCommand { Parent = this }];
		this.AddRange(commands);
	}
}
