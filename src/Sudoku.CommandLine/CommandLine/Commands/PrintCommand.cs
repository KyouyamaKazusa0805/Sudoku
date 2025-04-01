namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Represents a print command.
/// </summary>
internal sealed class PrintCommand : Command, ICommand
{
	/// <summary>
	/// Initializes a <see cref="PrintCommand"/> instance.
	/// </summary>
	public PrintCommand() : base("print", "Prints the specified data (especially for some bulit-in data)")
	{
		var commands = (SymbolList<Command>)[new PrintTechniquesCommand { Parent = this }];
		this.AddRange(commands);
	}


	/// <inheritdoc/>
	public SymbolList<Option> GlobalOptionsCore => [];
}
