namespace Sudoku.CommandLine;

/// <summary>
/// Provides an entry that parses command-line arguments.
/// </summary>
public static class CommandLineParser
{
	/// <summary>
	/// Try to parse the command-line.
	/// </summary>
	/// <param name="args">The arguments.</param>
	/// <returns>A <see cref="Task"/> object that handles the operation.</returns>
	public static async Task ParseAsync(string[] args)
	{
		RootCommand command = [
			new SolveCommand()
		];
		command.Description = "Sudoku Command Line";

		await command.InvokeAsync(args).ConfigureAwait(true);
	}
}
